using Assets.Game.Scripts.Customers;
using System;
using UnityEngine;

namespace Assets.Game.Scripts.Player.Actions
{
    /// <summary>
    /// Player task
    /// 1. Pick up the ready food from the FoodDesk.
    /// 2. Deliver the food to the waiting CustomerGroup.
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class DeliverFood : Photon.PunBehaviour, IPunObservable
    {
        public Food food;
        public FoodDesk foodDesk;

        PlayerController controller;

        public enum State
        {
            GetFood, //Going to the food desk
            GettingFood, //Waiting for server to give us food
            WaitForCustomer, //Wait for player to select the correct customer
            DeliverFood, //Going to the Customer to deliver the food
            Done
        }
        public State state;

        [PunRPC]
        public void GiveFood(Food food)
        {
            if (state != State.GettingFood)
                return;

            if(food == null)
            {
                End();
                return;
            }

            this.food = food;
            SwitchState(State.WaitForCustomer);
        }

        private void Start()
        {
            controller = GetComponent<PlayerController>();
            controller.SetDestination(foodDesk.transform.position);

            state = State.GetFood;
        }

        private void Update()
        {
            switch(state)
            {
                case State.GetFood:
                    if (foodDesk.CountFood() == 0)
                    {
                        End();
                        return;
                    }

                    if (Vector3.Distance(transform.position, foodDesk.transform.position) < controller.actionDistance)
                        SwitchState(State.GettingFood);
                    break;

                case State.DeliverFood:
                    if(Vector3.Distance(transform.position, food.customer.transform.position) < controller.actionDistance)
                    {
                        food.customer.photonView.RPC("GiveFood", PhotonTargets.MasterClient, food);
                        End();
                    }
                    break;
            }
        }

        private void End()
        {
            SwitchState(State.Done);
            Destroy(this);
        }

        private void SwitchState(State newState)
        {
            if (state == newState)
                return;
            State oldState = state;
            state = newState;

            //Cleanup
            switch(oldState)
            {
                case State.GetFood:
                    controller.StopMoving();
                    break;

                case State.WaitForCustomer:
                    food.customer.GetComponent<OutlineHover>().ForceOutline(false);
                    break;
                case State.DeliverFood:
                    controller.StopMoving();
                    break;
            }

            //Setup
            switch(state)
            {
                case State.GettingFood:
                    foodDesk.photonView.RPC("TakeOrder", PhotonTargets.MasterClient, photonView.viewID);
                    break;

                case State.WaitForCustomer:
                    //Highlight the correct customer
                    //TODO: Better show which customer(Replace icon with arrow? Different outline color? etc.)
                    food.customer.GetComponent<OutlineHover>().ForceOutline(true);
                    break;
                case State.DeliverFood:
                    controller.SetDestination(food.customer.transform.position);
                    break;
            }

        }

        public void DeliverTo(CustomerGroup group)
        {
            if (state != State.WaitForCustomer)
                return;

            if(group != food.customer)
            {
                Debug.LogError("Trying to deliver Food to incorrect customers: " + group.photonView.viewID + " instead of " + food.customer.photonView.viewID);
                return;
            }

            SwitchState(State.DeliverFood);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //Nothing to sync
            return;
        }
    }
}
