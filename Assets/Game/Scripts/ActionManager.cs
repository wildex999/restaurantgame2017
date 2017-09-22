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
        public void AddActionSynced(Type action)
        {
            if (!photonView.isMine)
                return;

            string className = action.FullName;
            if (syncedActions.Contains(className))
                return;

            AddSynced(action);
            syncedActions.Add(className);
        }

        /*public void AddAction()
        {

        }*/

        /// <summary>
        /// Remove the action from both the local and remote clients, and stop syncing it.
        /// </summary>
        public void RemoveActionSynced(Type action)
        {
            if (!photonView.isMine)
                return;

            string className = action.FullName;
            syncedActions.Remove(className);

            RemoveSynced(action);
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

        private void AddSynced(Type actionType)
        {
            Component comp = gameObject.AddComponent(actionType);
            photonView.ObservedComponents.Add(comp);
            ((IAction)comp).OnAdd(this);
        }

        private void RemoveSynced(Type actionType)
        {
            Component comp = GetComponent(actionType);
            if (!comp)
                return;

            ((IAction)comp).OnRemove();
            photonView.ObservedComponents.Remove(comp);
            Destroy(comp);
        }
    }
}
