using System;
using UnityEngine;

namespace Assets.Game.Scripts.Player.Actions
{
    /// <summary>
    /// Basic action for just moving to a destination
    /// </summary>
    public class ActionMove : SyncedAction<PlayerController>
    {
        PlayerController controller;
        StateGoTo<PlayerController> move;

        public ActionMove()
        {
            sync = false;
        }

        private void Awake()
        {
            controller = GetComponent<PlayerController>();

            move = new StateGoTo<PlayerController>(controller, this, 0.1f, stateEnd);
            SwitchState(AddState(move));
        }

        public override bool AllowNewAction(Type action)
        {
            return true;
        }

        public override void OnNewAction(IAction action)
        {
            End();
        }

        public void SetDestination(Vector3 destination)
        {
            move.SetDestination(destination);
        }

        public void SetDestination(Transform target)
        {
            move.SetDestination(target);
        }
    }
}
