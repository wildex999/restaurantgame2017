using Assets.Game.Scripts.DataClasses;
using Assets.Game.Scripts.Player;
using Assets.Game.Scripts.UI;
using Assets.Game.Scripts.Util;
using System;
using UnityEngine;

namespace Assets.Game.Scripts.Customers.Task
{
    /// <summary>
    /// Customer Task for Ordering food.
    /// 1. Read the menu for x amount of time.
    /// 2. Wait for the Employee to take our Order.
    /// 3. Wait for the food to be Delivered.
    /// </summary>
    [RequireComponent(typeof(CustomerGroup))]
    public class ActionOrderFood : SyncedAction<ActionOrderFood>
    {
        GameStatusIcon currentIcon;
        CustomerGroup group;
        Observable<PlayerEmployee> employee;

        [Tooltip("Number between 0 and 1 defining how fast they will read the menu. 0 = never, 1 = instantly")]
        public float menuReadingSpeed = 0.3f;

        int stateReadingMenu;
        int stateWaitingOrder;
        int stateWaitingFood;

        private void Start()
        {
            group = GetComponent<CustomerGroup>();
            employee = GameManager.instance.localPlayer.Employee();

            stateReadingMenu = AddState(new StateReadingMenu());
            stateWaitingOrder = AddState(new StateWaitingOrder());
            stateWaitingFood = AddState(new StateWaitingFood());

            SwitchState(stateReadingMenu);
        }

        /// <summary>
        /// Take the order for the Customer Group and send it to the requesting player.
        /// Then proceed to wait for the ordered food.
        /// </summary>
        [PunRPC]
        private void TakeOrder(int senderPlayerId, PhotonMessageInfo info)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            if (currentStateId != stateWaitingOrder)
            {
                Debug.LogError("Trying to get Order when not in WaitingOrder state.");
                return;
            }

            //Reply with order to client
            Order order = new Order("Test Order", group);
            PhotonView senderView = PhotonView.Find(senderPlayerId);
            senderView.RPC("ReceiveOrder", info.sender, order);

            StatusIconLibrary.Get().ShowTaskCompleteTick(currentIcon.transform.position);


            SwitchState(stateWaitingFood);
        }

        [PunRPC]
        private void GiveFood(Food food)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            if (currentStateId != stateWaitingFood)
            {
                Debug.LogError("Trying to Give Food when not in WaitingFood state.");
                return;
            }

            //TODO: Show happy/sad face
            StatusIconLibrary.Get().ShowTaskCompleteTick(currentIcon.transform.position);
            End();

            group.ActionEatFood();
        }

        private void OnMouseUpAsButton()
        {
            if (!employee)
                return;

            if (currentStateId == stateWaitingOrder)
            {
                //Task Employee with taking their order
                employee.Value.ActionTakeOrder(group);
            }
            else if (currentStateId == stateWaitingFood)
            {
                employee.Value.DeliverFood(group);
            }
        }

        #region States

        /// <summary>
        /// Read the menu for a certain amount of time.
        /// TODO: Consider any variables for menu reading time.(Type of customer etc.)
        /// </summary>
        private class StateReadingMenu : ActionState<ActionOrderFood>
        {
            public override void Setup() {}

            public override void Update()
            {
                if (action.photonView.isMine)
                {
                    if (UnityEngine.Random.Range(0f, 1f) < action.menuReadingSpeed)
                    {
                        action.SwitchState(action.stateWaitingOrder);
                        return;
                    }
                }
            }

            public override void Cleanup() {}
        }

        /// <summary>
        /// Wait for an Employee to come take our order
        /// </summary>
        private class StateWaitingOrder : ActionState<ActionOrderFood>
        {
            public override void Setup()
            {
                Data.IntRange waitRange = PatienceData.Instance.waitTakeOrder;
                action.group.Patience.Run(UnityEngine.Random.Range(waitRange.min, waitRange.max));

                action.currentIcon = Instantiate(StatusIconLibrary.Get().iconMenu, StatusIconLibrary.Get().mainCanvas.transform);
                action.currentIcon.StopOverlap = true;
                action.currentIcon.Follow(action.gameObject);
                action.currentIcon.SetPatience(action.group.Patience);
            }

            public override void Update()
            {}

            public override void Cleanup()
            {
                action.group.Patience.Stop();
                Destroy(action.currentIcon.gameObject);
            }
        }

        private class StateWaitingFood : ActionState<ActionOrderFood>
        {
            public override void Setup()
            {
                Data.IntRange waitRange = PatienceData.Instance.waitForFood;
                action.group.Patience.Run(UnityEngine.Random.Range(waitRange.min, waitRange.max));

                action.currentIcon = Instantiate(StatusIconLibrary.Get().iconFood, StatusIconLibrary.Get().mainCanvas.transform);
                action.currentIcon.StopOverlap = true;
                action.currentIcon.Follow(action.gameObject);
                action.currentIcon.SetPatience(action.group.Patience);
            }

            public override void Update()
            {}

            public override void Cleanup()
            {
                action.group.Patience.Stop();
                Destroy(action.currentIcon.gameObject);
            }
        }

        #endregion
    }
}
