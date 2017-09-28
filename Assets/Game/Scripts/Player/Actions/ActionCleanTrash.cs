using Assets.Game.Scripts.Other.Actions;
using Assets.Game.Scripts.Tables;
using Assets.Game.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Game.Scripts.Player.Actions
{
    public class ActionCleanTrash : SyncedAction<ActionCleanTrash>
    {
        public Table table;
        public Washer washer;
        public bool hasTrash = false;

        PlayerController controller;

        int stateMoveToTable;
        int stateCleanTable;
        int stateSelectWasher;
        int stateMoveToWasher;
        int stateThrowTrash;

        StateGoTo<PlayerController> goToTable;
        StateGoTo<PlayerController> goToWasher;

        private void Start()
        {
            sync = false;

            controller = GetComponent<PlayerController>();

            stateCleanTable = AddState(new StateCleanTable());
            stateSelectWasher = AddState(new StateSelectWasher());
            stateThrowTrash = AddState(new StateThrowTrash());

            goToTable = new StateGoTo<PlayerController>(controller, this, controller.actionDistance, stateCleanTable);
            stateMoveToTable = AddState(goToTable);
            goToWasher = new StateGoTo<PlayerController>(controller, this, controller.actionDistance, stateThrowTrash);
            stateMoveToWasher = AddState(goToWasher);

            goToTable.SetDestination(table.transform.position);
            SwitchState(stateMoveToTable);
        }

        [PunRPC]
        private void GetTrash()
        {
            hasTrash = true;
        }

        public void SetWasher(Washer washer)
        {
            this.washer = washer;
        }

        public override bool AllowNewAction(Type action)
        {
            return true;
        }

        public override void OnNewAction(IAction action) {
            if (currentStateId == stateMoveToTable)
                End();
            else if (currentStateId == stateMoveToWasher)
            {
                washer = null;
                SwitchState(stateSelectWasher);
            }
        }

        #region States

        /// <summary>
        /// Pick up all the trash at the table
        /// </summary>
        private class StateCleanTable : ActionState<ActionCleanTrash>
        {
            public override void Setup()
            {
                //Ask server to clean table
                action.table.photonView.RPC("CleanTrash", PhotonTargets.MasterClient, action.photonView.viewID);
            }

            public override void Update()
            {
                if (action.hasTrash)
                {
                    action.SwitchState(action.stateSelectWasher);
                    return;
                }

                //Make sure we end the action if it's no longer possible to do(Avoid being stuck in a race condition if another players cleans before us)
                if (action.table == null || !action.table.GetComponent<ActionTableDirty>())
                    action.End();
            }

            public override void Cleanup()
            {}
        }

        /// <summary>
        /// Mark valid Washers and wait for one to be selected
        /// </summary>
        private class StateSelectWasher : ActionState<ActionCleanTrash>
        {
            public override void Setup()
            {
                //Mark any valid Washers
                foreach (GameObject washer in GameObject.FindGameObjectsWithTag(Washer.washerTag))
                    washer.GetComponent<Washer>().ShowDropIcon(true);

            }

            public override void Update()
            {
                if (action.washer != null)
                {
                    action.goToWasher.SetDestination(action.washer.transform.position);
                    action.SwitchState(action.stateMoveToWasher);
                }
            }

            public override void Cleanup()
            {
                //Remove mark
                foreach (GameObject washer in GameObject.FindGameObjectsWithTag(Washer.washerTag))
                    washer.GetComponent<Washer>().ShowDropIcon(false);
            }
        }

        /// <summary>
        /// Throw away the trash/cleanup
        /// </summary>
        private class StateThrowTrash : ActionState<ActionCleanTrash>
        {
            public override void Setup()
            {
                //TODO: Actually place the trash in the Washer? 
                StatusIconLibrary.Get().ShowTaskCompleteTick(Camera.main.WorldToScreenPoint(action.washer.transform.position));
                action.End();
            }

            public override void Update() {}
            public override void Cleanup() {}
        }
        #endregion
    }
}
