using System;
using Assets.Game.Scripts.Customers;
using Assets.Game.Scripts.Customers.Task;
using UnityEngine;
using UnityEngine.UI;
using Assets.Game.Scripts.UI;

namespace Assets.Game.Scripts.Player.Actions
{
    /// <summary>
    /// Player Employee task.
    /// 1. Get order from Customer Group
    /// 2. Deliver order to Kitchen
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class ActionTakeOrder : SyncedAction<ActionTakeOrder>
    {
        public CustomerGroup group;
        public Order currentOrder; //Order currently carried by employee
        public Orders ordersDesk; //Where to deliver the order

        PlayerController controller;
        bool takeSent = false;

        int stateTakeOrder;
        int stateDeliverOrder;

        public ActionTakeOrder()
        {
            sync = false;

            stateTakeOrder = AddState(new StateTakeOrder());
            stateDeliverOrder = AddState(new StateDeliverOrder());
        }

        private void Start()
        {
            controller = GetComponent<PlayerController>();
            

            currentOrder = null;
            ordersDesk = null;

            SwitchState(stateTakeOrder);
        }

        [PunRPC]
        private void ReceiveOrder(Order order)
        {
            //If we fail to receive an order, then we stop this action
            if (order == null)
            {
                End();
                return;
            }

            currentOrder = order;
            SwitchState(stateDeliverOrder);
        }

        public void SetCustomerGroup(CustomerGroup group)
        {
            this.group = group;
        }

        public void DeliverOrder(Orders orders)
        {
            if (!photonView.isMine || currentStateId != stateDeliverOrder)
                return;

            ordersDesk = orders;
            controller.SetDestination(ordersDesk.transform.position);
        }

        public void OnPlayerAction(IAction action)
        {
            if (this == action)
                return;

            if (currentStateId == stateTakeOrder && !takeSent)
                End();
        }

        public override bool AllowNewAction(Type action)
        {
            return true;
        }

        public override void OnNewAction(IAction action)
        {
            if (currentStateId == stateTakeOrder && !takeSent)
                End();
        }

        #region States
        /// <summary>
        /// Pick up the order from the customers
        /// </summary>
        private class StateTakeOrder : ActionState<ActionTakeOrder>
        {
            public override void Setup()
            {
                action.controller.SetDestination(action.group.transform.position);
            }

            public override void Update() {
                if (action.takeSent)
                    return;

                if (Vector3.Distance(action.group.transform.position, action.transform.position) < action.controller.actionDistance)
                {
                    //Request Take Order trough server
                    action.group.photonView.RPC("TakeOrder", PhotonTargets.MasterClient, action.photonView.viewID);
                    action.takeSent = true;
                }
            }
            public override void Cleanup() {}
        }

        /// <summary>
        /// Deliver the order to a Order Desk
        /// </summary>
        private class StateDeliverOrder : ActionState<ActionTakeOrder>
        {
            public override void Setup()
            {
                //TODO: Find better way to update current order
                GameObject.Find("CurrentOrder").GetComponent<Text>().text = "Order: " + action.currentOrder.name;

                //Show icon on all valid Order locations
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Orders"))
                    obj.GetComponent<Orders>().ShowDropIcon(true);
            }

            public override void Update()
            {
                if (action.ordersDesk == null)
                    return;

                if (Vector3.Distance(action.ordersDesk.transform.position, action.transform.position) < action.controller.actionDistance)
                {
                    StatusIconLibrary.Get().ShowTaskCompleteTick(action.ordersDesk.GetDropIcon().transform.position);
                    action.ordersDesk.photonView.RPC("AddOrder", PhotonTargets.MasterClient, action.currentOrder);
                    action.manager.RemoveAction(action);
                }
            }

            public override void Cleanup()
            {
                GameObject.Find("CurrentOrder").GetComponent<Text>().text = "Order: None";

                //Hide icons on all Orders locations
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Orders"))
                    obj.GetComponent<Orders>().ShowDropIcon(false);
            }
        }
#endregion
    }
}
