using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Game.Scripts
{
    /// <summary>
    /// Manages the synced adding, removal and limits of Actions on this GameObject.
    /// </summary>
    public class ActionManager : Photon.PunBehaviour, IPunObservable
    {
        List<string> syncedActions; //A list of all current Synced Actions

        public ActionManager()
        {
            syncedActions = new List<string>();
        }

        /// <summary>
        /// Add an action to the GameObject, and add it for any current and future client.
        /// The action is also added to the list of observed objects.
        /// Only one instance of each action can be added at any one time.
        /// </summary>
        public Component AddActionSynced(Type action)
        {
            if (!photonView.isMine)
                return null;

            string className = action.FullName;
            if (syncedActions.Contains(className))
                return null;
            if (!AllowNewAction(action))
                return null;

            Component comp = AddSynced(action);
            syncedActions.Add(className);

            return comp;
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

        /// <summary>
        /// Remove the action from both the local and remote clients, and stop syncing it.
        /// This works for both Synced and non-synec actions.
        /// </summary>
        public void RemoveAction(Type action)
        {
            if (!photonView.isMine)
                return;

            string className = action.FullName;
            syncedActions.Remove(className);

            RemoveSynced(action);
        }

        public void RemoveAction(IAction action)
        {
            RemoveAction(action.GetType());
        }

        public void RemoveAction<T>()
        {
            RemoveAction(typeof(T));
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

        /// <summary>
        /// Add and Remove any synced Actions
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.isWriting)
            {
                stream.SendNext(syncedActions.ToArray<string>());
            } else
            {
                List<string> actions = new List<string>((string[])stream.ReceiveNext());

                var toAdd = actions.Except(syncedActions);
                var toRemove = syncedActions.Except(actions);

                foreach (var action in toAdd)
                    AddSynced(Type.GetType(action));

                foreach (var action in toRemove)
                    RemoveSynced(Type.GetType(action));

                syncedActions = actions;
            }
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

        private Component AddSynced(Type actionType)
        {
            Component comp = gameObject.AddComponent(actionType);
            photonView.ObservedComponents.Add(comp);

            ((IAction)comp).OnAdd(this);
            InformNewAction((IAction)comp);

            return comp;
        }

        private void RemoveSynced(Type actionType)
        {
            Component comp = GetComponent(actionType);
            if (!comp)
                return;

            ((IAction)comp).OnRemove();
            photonView.ObservedComponents.Remove(comp);
            DestroyImmediate(comp);
        }
    }
}
