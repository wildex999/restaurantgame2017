using System;
using Assets.Game.Scripts.Customers;
using Assets.Game.Scripts.Customers.Task;
using Assets.Game.Scripts.UI;
using Assets.Game.Scripts.UI.TableSelect;
using UnityEngine;

namespace Assets.Game.Scripts.Player.Actions
{
    /// <summary>
    /// Action performed by the Employee.
    /// 1. Go to the Customer Group that needs to be seated
    /// 2. Open Table Selection screen
    /// 3. Lead Customer to selected table (Maybe future?)
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class ActionSeatCustomers : SyncedAction<ActionSeatCustomers>
    {
        PlayerController controller;
        CustomerGroup group;

        int stateGoToCustomer;
        int stateSelectTable;

        ActionSeatCustomers()
        {
            //TODO: Find a better way to do unsynced actions without having to copy paste a lot of code
            sync = false;

            stateGoToCustomer = AddState(new StateGoToCustomer());
            stateSelectTable = AddState(new StateSelectTable());
        }

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
        }

        protected override void Update()
        {
            if (group == null || !photonView.isMine)
                return;

            ActionGetTable customerTask = group.GetComponent<ActionGetTable>();
            if (customerTask == null || !customerTask.AwaitingTable())
            {
                OnClose();
                return;
            }

            base.Update();
        }

        private void OnClose()
        {
            End();
        }

        /// <summary>
        /// Set the Customer Group which we are seating
        /// </summary>
        /// <param name="group"></param>
        public void SetCustomerGroup(CustomerGroup group)
        {
            this.group = group;
            SwitchState(stateGoToCustomer);
        }

        public override bool AllowNewAction(Type action)
        {
            return currentStateId == stateGoToCustomer;
        }

        public override void OnNewAction(IAction action)
        {
            if (currentStateId == stateGoToCustomer)
                End();
        }

        #region States

        /// <summary>
        /// Go to the selected customer group.
        /// </summary>
        private class StateGoToCustomer : ActionState<ActionSeatCustomers>
        {
            public override void Setup()
            {
                action.controller.SetDestination(action.group.transform.position);
            }

            public override void Update()
            {
                if (Vector3.Distance(action.transform.position, action.group.transform.position) < action.controller.actionDistance)
                    action.SwitchState(action.stateSelectTable);
            }

            public override void Cleanup()
            {
                action.controller.ClearDestination();
            }
        }

        /// <summary>
        /// Select a table for the customer group
        /// </summary>
        private class StateSelectTable : ActionState<ActionSeatCustomers>
        {
            public override void Setup() {
                //Open the Table selection screen
                action.controller.allowUserInput = false;
                TableSelection tableSelection = StatusIconLibrary.Get().TableSelectionUi;
                tableSelection.SetCloseCallback(action.OnClose);
                tableSelection.SetGroup(action.group);
                tableSelection.gameObject.SetActive(true);
            }

            public override void Update() {}

            public override void Cleanup() {
                TableSelection tableSelection = StatusIconLibrary.Get().TableSelectionUi;
                tableSelection.SetCloseCallback(null);
                tableSelection.SetGroup(null);
                tableSelection.gameObject.SetActive(false);
                action.controller.allowUserInput = true;
            }
        }
        #endregion
    }
}
