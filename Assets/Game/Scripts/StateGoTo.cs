using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Game.Scripts
{
    /// <summary>
    /// A General Action State for going to a location and switching to a new state once close enough
    /// </summary>
    public class StateGoTo<T> : ActionState<IAction> where T : MonoBehaviour, ICanSetDestination
    {
        IAction actionObject;
        T moveObject;
        int nextState;

        float goalDistance;

        bool hasDestination;

        Transform destinationTarget;
        Func<Vector3> destinationFunction;

        Vector3 destination;
        Vector3 prevDestination = new Vector3(-1000000, -1000000);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveObject">The gameObject which we are moving</param>
        /// <param name="goalDistance">How close to be for us to be considered at the destination</param>
        /// <param name="nextStateId">The state to switch to ocne reaching the destination</param>
        public StateGoTo(T moveObject, IAction actionObject, float goalDistance, int nextStateId)
        {
            this.moveObject = moveObject;
            this.actionObject = actionObject;

            nextState = nextStateId;
            this.goalDistance = goalDistance;
            hasDestination = false;
        }

        /// <summary>
        /// Set the destination to a given static point
        /// </summary>
        /// <param name="destination"></param>
        public void SetDestination(Vector3 destination)
        {
            this.destination = destination;
            hasDestination = true;
        }

        /// <summary>
        /// Set the destination to a given dynamic position.
        /// Will update destination to follow the target.
        /// </summary>
        /// <param name="destinationTarget"></param>
        public void SetDestination(Transform destinationTarget)
        {
            this.destinationTarget = destinationTarget;
        }

        /// <summary>
        /// Set the destination from the given dynamic action.
        /// This action will be called regularly to get the updated destination point
        /// </summary>
        /// <param name="destinationAction"></param>
        public void SetDestination(Func<Vector3> destinationAction)
        {
            this.destinationFunction = destinationAction;
        }

        /// <summary>
        /// Stop moving towards the destination.
        /// </summary>
        public void Stop()
        {
            hasDestination = false;
            moveObject.ClearDestination();
        }

        public override void Cleanup()
        {
            moveObject.ClearDestination();
        }

        public override void Setup()
        {
            if (hasDestination)
                moveObject.SetDestination(destination);
        }

        public override void Update()
        {
            if (destinationFunction != null)
                destination = destinationFunction.Invoke();
            else if (destinationTarget != null)
                destination = destinationTarget.position;
            else if (!hasDestination)
                return;

            //Update Destination if it has changed
            if (Vector3.Distance(prevDestination, destination) > 0.1f)
            {
                moveObject.SetDestination(destination);
                prevDestination = destination;
            }

            //Check if we have reached our destination
            if(Vector3.Distance(moveObject.transform.position, destination) < goalDistance)
                actionObject.SwitchState(nextState);
        }
    }
}
