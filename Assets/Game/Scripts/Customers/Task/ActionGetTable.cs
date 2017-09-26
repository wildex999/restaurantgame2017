using Assets.Game.Scripts.Tables;
using Assets.Game.Scripts.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

namespace Assets.Game.Scripts.Customers.Task
{
    /// <summary>
    /// This action has the goal of getting the group of customers to a table.
    /// 1. Go to the door.
    /// 2. Wait to be assigned a table.
    /// 3. When assigned the table, go to it and sit down(Place customers at seats)
    /// </summary>
    [RequireComponent(typeof(CustomerGroup))]
    public class ActionGetTable : SyncedAction<ActionGetTable>
    {
        CustomerGroup group;
        CustomerQueue queue;
        TableGroup targetTable;

        GameStatusIcon currentIcon;

        int stateWalkToDoor;
        int stateWaitForTable;
        int stateWalkToTable;
        int stateSeatTable;

        StateGoTo<CustomerGroup> goToTable;

        private void Start()
        {
            queue = FindObjectOfType<CustomerQueue>();
            group = GetComponent<CustomerGroup>();

            stateWaitForTable = AddState(new StateWaitForTable());
            stateSeatTable = AddState(new StateSeatTable());

            StateGoTo<CustomerGroup> goToDoor = new StateGoTo<CustomerGroup>(group, this, 1f, stateWaitForTable);
            goToDoor.SetDestination(() => { return queue.NextQueuePosition(); });
            stateWalkToDoor = AddState(goToDoor);

            goToTable = new StateGoTo<CustomerGroup>(group, this, 1f, stateSeatTable);
            stateWalkToTable = AddState(goToTable);

            SwitchState(stateWalkToDoor);
        }

        private void OnMouseUpAsButton()
        {
            if (currentStateId != stateWaitForTable)
                return;

            //Task Employee with seating the customers
            GameManager.instance.localPlayer.ActionSeatCustomerGroup(group);
        }

        [PunRPC]
        public void SetTable(int tableId)
        {
            if (!photonView.isMine)
                return;

            PhotonView tableObj = PhotonView.Find(tableId);
            if (tableObj == null)
            {
                Debug.LogError("Failed to find target table: " + tableId);
                return;
            }
            targetTable = tableObj.GetComponent<TableGroup>();

            //Occupy Seats
            Queue<Customer> customers = new Queue<Customer>(group.GetCustomers());
            foreach (Chair chair in targetTable.GetChairs())
            {
                if (customers.Count == 0)
                    break;

                if (chair.seatedCustomer == null)
                    chair.seatedCustomer = customers.Dequeue();
            }

            //Move Customers to their seats
            //TODO: Move each customer sepparate from group
            StatusIconLibrary.Get().ShowTaskCompleteTick(currentIcon.transform.position);
            goToTable.SetDestination(targetTable.transform.position);
            SwitchState(stateWalkToTable);
        }

        public bool AwaitingTable()
        {
            return currentStateId == stateWaitForTable;
        }

        #region States

        /// <summary>
        /// Join the queue and wait to be given a table.
        /// </summary>
        private class StateWaitForTable : ActionState<ActionGetTable>
        {
            public override void Setup() {
                //Enter the queue
                if (action.photonView.isMine)
                    action.queue.EnterQueue(action.group);

                action.currentIcon = Instantiate(StatusIconLibrary.Get().iconTable, StatusIconLibrary.Get().mainCanvas.transform);
                action.currentIcon.Follow(action.gameObject);
            }

            public override void Update()
            {
                //Master Client
                if(action.photonView.isMine)
                {
                    //Move along the queue
                    Vector3 point = action.queue.GetQueuePosition(action.group);
                    if (action.group.GetDestination() != point)
                        action.group.SetDestination(point);

                    //Wait
                    action.group.waiting -= (100f / action.group.patience) * Time.deltaTime;
                }

                //Common
                //Update Icon to indicate waiting time
                action.currentIcon.SetFade(1f - (action.group.waiting / 100f));
            }

            public override void Cleanup() {
                if (action.photonView.isMine)
                {
                    action.queue.LeaveQueue(action.group);
                    action.group.waiting = 100;
                }

                Destroy(action.currentIcon.gameObject);
            }

        }

        /// <summary>
        /// Seat the customers at the table
        /// </summary>
        private class StateSeatTable : ActionState<ActionGetTable>
        {
            public override void Setup()
            {
                if (action.photonView.isMine)
                {
                    action.GetComponent<NavMeshAgent>().enabled = false;
                    Vector3 tablePos = action.targetTable.transform.position;
                    action.group.transform.position = new Vector3(tablePos.x, action.group.transform.position.y, tablePos.z);
                    foreach (Chair chair in action.targetTable.GetChairs())
                    {
                        Customer customer = chair.seatedCustomer;
                        if (customer == null)
                            continue;
                        if (!action.group.HasCustomer(customer))
                            continue;

                        Vector3 chairPos = chair.transform.position;
                        customer.transform.position = new Vector3(chairPos.x, customer.transform.position.y, chairPos.z);
                    }
                    action.group.Table = action.targetTable;

                    action.End();
                    action.group.ActionOrderFood();
                }
            }

            public override void Update() {}
            public override void Cleanup() {}
        }

        #endregion
    }
}
