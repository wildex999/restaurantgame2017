using Assets.Game.Scripts.DataClasses;
using Assets.Game.Scripts.Player;
using Assets.Game.Scripts.Tables;
using Assets.Game.Scripts.UI;
using Assets.Game.Scripts.Util;
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
        public float eatTime;

        CustomerGroup group;
        GameStatusIcon icon;
        Observable<PlayerEmployee> employee;

        int stateEat;
        int statePay;
        int stateLeave;
        int stateDestroy;

        StateGoTo<CustomerGroup> goToExit;

        private void Awake()
        {
            group = GetComponent<CustomerGroup>();
            employee = GameManager.instance.localPlayer.Employee();

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
            if(employee)
                employee.Value.ActionTakeMoney(group);
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
            public override void Setup() {
                Data.IntRange timeRange = TimingData.Instance.timeToEat;
                action.eatTime = UnityEngine.Random.Range(timeRange.min, timeRange.max);
            }

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
                {
                    Data.IntRange waitRange = PatienceData.Instance.waitPayBill;
                    action.group.Patience.Run(UnityEngine.Random.Range(waitRange.min, waitRange.max));
                }

                action.icon = Instantiate(StatusIconLibrary.Get().iconMoney, StatusIconLibrary.Get().mainCanvas.transform);
                action.icon.StopOverlap = true;
                action.icon.Follow(action.gameObject);
                action.icon.SetPatience(action.group.Patience);
            }

            public override void Update()
            {}

            public override void Cleanup()
            {
                if (action.photonView.isMine)
                    action.group.Patience.Stop();

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
