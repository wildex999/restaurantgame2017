using Assets.Game.Scripts.Tables;
using Assets.Game.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Game.Scripts.Customers.Task
{
    /// <summary>
    /// 1. Eat the food until finished.
    /// 2. Pay the bill.
    /// 3. Leave the Restaurant
    /// </summary>
    public class ActionEatFood : SyncedAction<ActionEatFood>
    {
        public float eatTime = 1f;

        CustomerGroup group;
        GameStatusIcon icon;

        int stateEat;
        int statePay;
        int stateLeave;
        int stateDestroy;

        StateGoTo<CustomerGroup> goToExit;

        private void Awake()
        {
            group = GetComponent<CustomerGroup>();

            stateEat = AddState(new StateEat());
            statePay = AddState(new StatePay());
            stateDestroy = AddState(new StateDestroy());

            goToExit = new StateGoTo<CustomerGroup>(group, this, 1f, stateDestroy);
            stateLeave = AddState(goToExit);

            SwitchState(stateEat);
        }

        private void OnMouseUpAsButton()
        {
            if (currentStateId != statePay)
                return;

            //Task Employee with getting payment from the customers
            GameManager.instance.localPlayer.ActionTakeMoney(group);
        }

        [PunRPC]
        public void Pay()
        {
            if (!photonView.isMine || currentStateId != statePay)
                return;

            //TODO: Increment money & score depending on satisfaction?
            StatusIconLibrary.Get().ShowTaskCompleteTick(icon.transform.position);

            //Mark the table as needing a cleanup
            if (group.Table)
                group.Table.GetTable().MarkTableDirty();
            group.Table = null;

            //Leave the Restaurant
            group.PlaceAtNavMesh(); //Make sure we get the customers on the NavMesh, or else they will teleport
            goToExit.SetDestination(GameObject.Find("CustomerExit").transform.position);
            SwitchState(stateLeave);
        }

        #region States
        /// <summary>
        /// Eat the food until finished
        /// </summary>
        private class StateEat : ActionState<ActionEatFood>
        {
            public override void Setup() {}

            public override void Update()
            {
                if (!action.photonView.isMine)
                    return;

                action.eatTime -= Time.deltaTime;
                if (action.eatTime <= 0f)
                    action.SwitchState(action.statePay);
            }

            public override void Cleanup() {}
        }

        /// <summary>
        /// Wait until the Employee take our payment
        /// </summary>
        private class StatePay : ActionState<ActionEatFood>
        {
            public override void Setup()
            {
                if (action.photonView.isMine)
                    action.group.waiting = 100;

                action.icon = Instantiate(StatusIconLibrary.Get().iconMoney, StatusIconLibrary.Get().mainCanvas.transform);
                action.icon.StopOverlap = true;
                action.icon.Follow(action.gameObject);
            }

            public override void Update()
            {
                //Master
                if (action.photonView.isMine)
                    action.group.waiting -= (100f / action.group.patience) * Time.deltaTime;

                //Common
                //Update Icon to indicate waiting time
                action.icon.SetFade(1f - (action.group.waiting / 100f));
            }

            public override void Cleanup()
            {
                Destroy(action.icon.gameObject);
            }
        }

        private class StateDestroy : ActionState<ActionEatFood>
        {
            public override void Setup()
            {
                Destroy(action.gameObject);
            }

            public override void Update() {}

            public override void Cleanup() {}
        }
        #endregion
    }
}
