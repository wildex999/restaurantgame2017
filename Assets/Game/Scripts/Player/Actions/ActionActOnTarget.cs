using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Game.Scripts.Player.Actions
{
    /// <summary>
    /// Simple Action of moving to a target and performing an action once there
    /// </summary>
    public class ActionActOnTarget : SyncedAction<ActionActOnTarget>
    {
        PlayerController controller;
        Action<Transform> action;
        Transform target;

        int stateMoveToTarget;
        int stateActOnTarget;

        StateGoTo<PlayerController> goToTarget;

        public ActionActOnTarget()
        {
            sync = false;
        }

        private void Awake()
        {
            controller = GetComponent<PlayerController>();

            stateActOnTarget = AddState(new StateAct());
            goToTarget = new StateGoTo<PlayerController>(controller, this, 1f, stateActOnTarget);
            stateMoveToTarget = AddState(goToTarget);

            SwitchState(stateMoveToTarget);
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
            goToTarget.SetDestination(target);
        }

        public void SetAction(Action<Transform> action)
        {
            this.action = action;
        }

        private class StateAct : ActionState<ActionActOnTarget>
        {
            public override void Setup() {}

            public override void Update() {
                if(action.action != null)
                {
                    action.action.Invoke(action.target);
                    action.End();
                }
            }

            public override void Cleanup() {}
        }
    }
}
