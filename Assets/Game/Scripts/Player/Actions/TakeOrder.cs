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
    public class TakeOrder : Photon.PunBehaviour, IPunObservable
    {
        public CustomerGroup group;
        public Order currentOrder; //Order currently carried by employee
        public Orders ordersDesk; //Where to deliver the order

        PlayerController controller;

        public enum State
        {
            GoToCustomer,
            TakingOrder,
            DeliveringOrder,
            Done
        }
        public State state;

        private void Start()
        {
            controller = GetComponent<PlayerController>();
            controller.SetDestination(group.transform.position);

            currentOrder = null;
            ordersDesk = null;
            state = State.GoToCustomer;
        }

        private void Update()
        {
            switch(state)
            {
                case State.GoToCustomer:
                    if (Vector3.Distance(group.transform.position, transform.position) < controller.actionDistance)
                        SwitchState(State.TakingOrder);
                    break;

                case State.TakingOrder:
                    //TODO: Timeout?
                    break;
                case State.DeliveringOrder:
                    if (ordersDesk == null)
                        break;

                    if(Vector3.Distance(ordersDesk.transform.position, transform.position) < controller.actionDistance)
                    {
                        StatusIconLibrary.Get().ShowTaskCompleteTick(ordersDesk.GetDropIcon().transform.position);
                        ordersDesk.photonView.RPC("AddOrder", PhotonTargets.MasterClient, currentOrder);
                        SwitchState(State.Done);
                        Destroy(this);
                    }

                    break;
            }
        }

        void SwitchState(State newState)
        {
            if (state == newState)
                return;

            State oldState = state;
            state = newState;

            //Cleanup
            switch(oldState)
            {
                case State.DeliveringOrder:
                    GameObject.Find("CurrentOrder").GetComponent<Text>().text = "Order: None";

                    //Hide icons on all Orders locations
                    foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Orders"))
                        obj.GetComponent<Orders>().ShowDropIcon(false);
                    break;
            }

            //Setup
            switch(state)
            {
                case State.TakingOrder:
                    //Request Take Order trough server
                    group.photonView.RPC("TakeOrder", PhotonTargets.MasterClient, photonView.viewID);
                    break;
                case State.DeliveringOrder:
                    //TODO: Find better way to update current order
                    GameObject.Find("CurrentOrder").GetComponent<Text>().text = "Order: " + currentOrder.name;

                    //Show icon on all valid Order locations
                    foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Orders"))
                        obj.GetComponent<Orders>().ShowDropIcon(true);
                    break;
            }
        }

        [PunRPC]
        private void ReceiveOrder(Order order)
        {
            //If we fail to receive an order, then we stop this action
            if (order == null)
                Destroy(this);

            currentOrder = order;
            SwitchState(State.DeliveringOrder);
        }

        public void SetCustomerGroup(CustomerGroup group)
        {
            this.group = group;
        }

        public void DeliverOrder(Orders orders)
        {
            if (!photonView.isMine || state != State.DeliveringOrder)
                return;

            ordersDesk = orders;
            controller.SetDestination(ordersDesk.transform.position);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //We don't sync anything, we just use RPC
        }
    }
}
