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
    public class ActionDeliverFood : SyncedAction<ActionDeliverFood>
    {
        public Food food;
        public FoodDesk foodDesk;

        PlayerController controller;

        int stateWalkToFood;
        int stateGettingFood;
        int stateSelectCustomer;
        int stateWalkToCustomer;
        int stateDeliverFood;

        StateGoTo<PlayerController> goToFood;
        StateGoTo<PlayerController> goToCustomer;

        public ActionDeliverFood()
        {
            sync = false;
        }

        private void Start()
        {
            controller = GetComponent<PlayerController>();
            controller.SetDestination(foodDesk.transform.position);

            stateGettingFood = AddState(new StateGettingFood());
            stateSelectCustomer = AddState(new StateSelectCustomer());
            stateDeliverFood = AddState(new StateDeliverFood());

            goToFood = new StateGoTo<PlayerController>(controller, this, controller.actionDistance, stateGettingFood);
            stateWalkToFood = AddState(goToFood);
            goToFood.SetDestination(foodDesk.transform.position);

            goToCustomer = new StateGoTo<PlayerController>(controller, this, controller.actionDistance, stateDeliverFood);
            stateWalkToCustomer = AddState(goToCustomer);

            SwitchState(stateWalkToFood);
        }

        [PunRPC]
        public void GiveFood(Food food)
        {
            if (currentStateId != stateGettingFood)
                return;

            if (food == null)
            {
                End();
                return;
            }

            this.food = food;
            SwitchState(stateSelectCustomer);
        }

        public void DeliverTo(CustomerGroup group)
        {
            if (currentStateId != stateSelectCustomer || group == null)
            {
                Debug.LogError("Trying to deliver Food when not in correct state or to null group: " + currentStateId + " | " + group);
                return;
            }

            if (group != food.customer)
            {
                Debug.LogError("Trying to deliver Food to incorrect customers: " + group.photonView.viewID + " instead of " + food.customer.photonView.viewID);
                return;
            }

            goToCustomer.SetDestination(group.transform.position);
            SwitchState(stateWalkToCustomer);
        }

        public override bool AllowNewAction(Type action)
        {
            return true;
        }

        public override void OnNewAction(IAction action)
        {
            if (currentStateId == stateWalkToFood)
                End();
        }

        #region States
        /// <summary>
        /// Tell the server we want to take an Order, and wait for a response on GiveFood
        /// </summary>
        private class StateGettingFood : ActionState<ActionDeliverFood>
        {
            public override void Setup()
            {
                action.foodDesk.photonView.RPC("TakeOrder", PhotonTargets.MasterClient, action.photonView.viewID);
            }

            public override void Update() {}
            public override void Cleanup() {}
        }

        /// <summary>
        /// Wait for the player to select the highlighted customer for delivery of the Food
        /// </summary>
        private class StateSelectCustomer : ActionState<ActionDeliverFood>
        {
            public override void Setup()
            {
                //Highlight the correct customer
                //TODO: Better show which customer(Replace icon with arrow? Different outline color? etc.)
                action.food.customer.GetComponent<OutlineHover>().ForceOutline(true);
            }

            public override void Update() { }

            public override void Cleanup() {
                if (action.food == null || action.food.customer == null)
                    return;

                action.food.customer.GetComponent<OutlineHover>().ForceOutline(false);
            }
        }

        /// <summary>
        /// Tell the server to give the food to the customer, and then end this action.
        /// </summary>
        private class StateDeliverFood : ActionState<ActionDeliverFood>
        {
            public override void Setup()
            {
                action.food.customer.photonView.RPC("GiveFood", PhotonTargets.MasterClient, action.food);
                action.End();
            }

            public override void Update() {}
            public override void Cleanup() {}
        }
        #endregion
    }
}
