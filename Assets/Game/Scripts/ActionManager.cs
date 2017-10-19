using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Game.Scripts
{
    /// <summary>
    /// Manages the synced adding, removal and limits of Actions on this GameObject.
    /// </summary>
    public class ActionManager : ComponentManager
    {

        public override Component AddSynced(Type action)
        {
            if (!photonView.isMine)
                return null;

            if (!AllowNewAction(action))
                return null;

            return base.AddSynced(action);
        }

        public virtual Component AddActionSynced(Type action)
        {
            return AddSynced(action);
        }

        public T AddActionSynced<T>() where T : Component
        {
            return (T)AddSynced(typeof(T));
        }

        /// <summary>
        /// Add an action to the GameObject, but do not add it to any current or future clients.
        /// This also not added to the list of observed objects.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>New instance of the given Action, or null if it was not added.</returns>
        public Component AddAction(Type action)
        {
            if (!photonView.isMine || GetComponent(action) != null)
                return null;
            if (!AllowNewAction(action))
                return null;

            Component comp = gameObject.AddComponent(action);
            ((IAction)comp).OnAdd(this);
            InformNewAction((IAction)comp);

            return comp;
        }

        public T AddAction<T>() where T : Component
        {
            return (T)AddAction(typeof(T));
        }

        public void RemoveAction(Type action)
        {
            Remove(action);
        }

        public void RemoveAction(Component action)
        {
            Remove(action.GetType());
        }

        public void RemoveAction<T>()
        {
            Remove(typeof(T));
        }

        /// <summary>
        /// Get a list of the currently added actions
        /// </summary>
        /// <returns></returns>
        public List<IAction> GetActions()
        {
            List<IAction> actions = new List<IAction>();
            actions.AddRange(GetComponents<IAction>());

            return actions;
        }

        protected override Component DoAddSynced(Type componentType)
        {
            Component comp = base.DoAddSynced(componentType);
            ((IAction)comp).OnAdd(this);
            InformNewAction((IAction)comp);

            return comp;
        }

        /// <summary>
        /// Check with existing actions whether they allow the new action
        /// </summary>
        /// <param name="action"></param>
        /// <returns>True if the new action is allowed</returns>
        private bool AllowNewAction(Type newAction)
        {
            foreach(IAction action in GetActions())
            {
                if (!action.AllowNewAction(newAction))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Inform existing actions that a new action has been added.
        /// </summary>
        /// <param name="newAction"></param>
        private void InformNewAction(IAction newAction)
        {
            foreach (IAction action in GetActions())
            {
                if (action == newAction)
                    continue;

                action.OnNewAction(newAction);
            }
        }
    }
}
