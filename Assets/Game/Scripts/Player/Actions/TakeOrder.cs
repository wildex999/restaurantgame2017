using System;
using Assets.Game.Scripts.Customers;
using Assets.Game.Scripts.Customers.Task;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.Player.Actions
{
    /// <summary>
    /// 1. Get order from Customer Group
    /// 2. Deliver order to Kitchen
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class TakeOrder : Photon.PunBehaviour, IPunObservable
    {
        public CustomerGroup group;
        public string currentOrder; //Order currently carried by employee
        public Orders ordersDesk; //Where to deliver the order

        PlayerController controller;

        enum State
        {
            GoToCustomer,
            TakingOrder,
            DeliveringOrder,
            Done
        }
        State state;

        private void Start()
        {
            controller = GetComponent<PlayerController>();
            controller.SetDestination(group.transform.position);

            currentOrder = "";
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
                        obj.GetComponent<Orders>().ShowIcon(false);
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
                    GameObject.Find("CurrentOrder").GetComponent<Text>().text = "Order: " + currentOrder;

                    //Show icon on all valid Order locations
                    foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Orders"))
                        obj.GetComponent<Orders>().ShowIcon(true);
                    break;
            }
        }

        [PunRPC]
        private void ReceiveOrder(string order)
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
