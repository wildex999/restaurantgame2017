using Assets.Game.Scripts.Tables;
using Assets.Game.Scripts.UI;
using Assets.Game.Scripts.UI.TableSelect;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Game.Scripts.Customers.Task
{
    /// <summary>
    /// This action has the goal of getting the group of customers to a table.
    /// 1. Go to the door.
    /// 2. Wait to be assigned a table.
    /// 3. When assigned the table, go to it and sit down(Place customers at seats)
    /// </summary>
    [RequireComponent(typeof(CustomerGroup))]
    public class GetTable : Photon.PunBehaviour, IPunObservable
    {

        enum State
        {
            MoveToDoor,
            WaitForTable,
            MoveToTable
        }
        State state;

        CustomerGroup group;
        CustomerQueue queue;
        NavMeshAgent agent;
        TableGroup targetTable;

        GameStatusIcon currentIcon;

        private void Start()
        {
            state = State.MoveToDoor;
            agent = GetComponent<NavMeshAgent>();
            queue = GameObject.FindObjectOfType<CustomerQueue>();
            group = GetComponent<CustomerGroup>();
        }

        private void Update()
        {
            //Update state and decisions for the Master Client
            if (photonView.isMine)
                UpdateMaster();

            //Common update for master and all clients
            switch(state)
            {
                case State.WaitForTable:
                    //Update Icon to indicate waiting time
                    currentIcon.SetFade(1f - (group.waiting / 100f));
                    break;
            }
            
        }

        private void UpdateMaster()
        {
            Vector3 point;

            switch (state)
            {
                case State.MoveToDoor:
                    point = queue.NextQueuePosition();

                    if (agent.destination != point)
                        agent.SetDestination(point);

                    //Enter queue if near
                    if (Vector3.Distance(transform.position, point) < 2f)
                        SwitchState(State.WaitForTable);

                    break;

                case State.WaitForTable:
                    //Move along the queue
                    point = queue.GetQueuePosition(group);
                    if (agent.destination != point)
                        agent.SetDestination(point);

                    //Wait
                    group.waiting -= (100f / group.patience) * Time.deltaTime;
                    break;

                case State.MoveToTable:
                    //Sit if close to the table
                    if(Vector3.Distance(transform.position, targetTable.transform.position) < 1f)
                    {
                        group.GetComponent<NavMeshAgent>().enabled = false;
                        group.transform.position = targetTable.transform.position;
                        foreach(Chair chair in targetTable.GetChairs())
                        {
                            Customer customer = chair.seatedCustomer;
                            if (customer == null)
                                continue;
                            if (!group.HasCustomer(customer))
                                continue;

                            customer.transform.position = chair.transform.position;
                        }

                        photonView.RPC("End", PhotonTargets.All);
                        group.StartActionOrderFood();
                    }
                    break;
            }
        }

        private void SwitchState(State newState)
        {
            State oldState = state;
            state = newState;

            //Cleanup the old state
            switch(oldState)
            {
                case State.WaitForTable:
                    queue.LeaveQueue(group);
                    Destroy(currentIcon.gameObject);

                    group.waiting = 100;
                    //TODO: Show Green tick icon indicating completion

                    break;
            }

            //Setup the new state
            switch(state)
            {
                case State.WaitForTable:
                    queue.EnterQueue(group);
                    currentIcon = Instantiate(StatusIconLibrary.Get().iconTable, StatusIconLibrary.Get().mainCanvas.transform);
                    currentIcon.Follow(gameObject);
                    break;

                case State.MoveToTable:
                    agent.SetDestination(targetTable.transform.position);
                    break;
            }
        }

        private void OnMouseUpAsButton()
        {
            if (state != State.WaitForTable)
                return;

            //Task Employee with seating the customers
            GameManager.instance.localPlayer.SeatCustomerGroup(group);
        }

        [PunRPC]
        public void SetTable(int tableId)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            PhotonView tableObj = PhotonView.Find(tableId);
            if(tableObj == null)
            {
                Debug.LogError("Failed to find target table: " + tableId);
                return;
            }
            targetTable = tableObj.GetComponent<TableGroup>();

            //Occupy Seats
            Queue<Customer> customers = new Queue<Customer>(group.GetCustomers());
            foreach(Chair chair in targetTable.GetChairs())
            {
                if (customers.Count == 0)
                    break;

                if(chair.seatedCustomer == null)
                    chair.seatedCustomer = customers.Dequeue();
            }

            //Move Customers to their seats
            //TODO: Move each customer sepparate from group
            targetTable = tableObj.GetComponent<TableGroup>();
            SwitchState(State.MoveToTable);
        }

        [PunRPC]
        public void End()
        {
            Destroy(this);
        }

        public bool AwaitingTable()
        {
            return state == State.WaitForTable;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.isWriting)
            {
                stream.SendNext(state);
            } else
            {
                State newState = (State)stream.ReceiveNext();
                if (newState != state)
                    SwitchState(newState);
            }
        }
    }
}
