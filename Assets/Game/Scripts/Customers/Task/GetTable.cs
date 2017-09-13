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

        GameObject mainCanvas;
        GameStatusIcon currentIcon;

        private void Start()
        {
            state = State.MoveToDoor;
            agent = GetComponent<NavMeshAgent>();
            queue = GameObject.FindObjectOfType<CustomerQueue>();
            group = GetComponent<CustomerGroup>();

            mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
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
                    //Update Icon to indicate satisfaction
                    currentIcon.SetFade(1f - (group.satisfaction / 100f));
                    break;
            }
            
        }

        private void UpdateMaster()
        {
            switch (state)
            {
                case State.MoveToDoor:
                    Vector3 point = queue.NextQueuePosition();

                    if (agent.destination != point)
                        agent.SetDestination(point);

                    //Enter queue if near
                    if (Vector3.Distance(transform.position, point) < 2f)
                    {
                        queue.EnterQueue(group);
                        SwitchState(State.WaitForTable);
                    }

                    break;

                case State.WaitForTable:
                    //Decrease satisfaction
                    group.satisfaction -= (100f / group.patience) * Time.deltaTime;
                    break;

                case State.MoveToTable:
                    //TODO: Sit if close to table
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
                    Destroy(currentIcon.gameObject);
                    break;
            }

            //Setup the new state
            switch(state)
            {
                case State.WaitForTable:
                    currentIcon = Instantiate(StatusIconLibrary.Get().waitingTable, mainCanvas.transform);
                    currentIcon.Follow(this.gameObject);
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
