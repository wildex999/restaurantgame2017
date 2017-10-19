using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Game.Scripts
{
    /// <summary>
    /// Manages the adding and removal of synchronized Components.
    /// If the owner adds a component through this, then that will be synced to all players syncing this object.
    /// </summary>
    public class ComponentManager : Photon.PunBehaviour, IPunObservable
    {
        public List<string> syncedComponents; //A list of all current Synced Components

        public ComponentManager()
        {
            syncedComponents = new List<string>();
        }

        /// <summary>
        /// Add an component to the GameObject, and add it for any current and future client.
        /// The action is also added to the list of observed objects.
        /// Only one instance of each component can be added at any one time.
        /// </summary>
        public virtual Component AddSynced(Type component)
        {
            if (!photonView.isMine)
                return null;

            string className = component.FullName;
            if (syncedComponents.Contains(className))
                return null;

            Component comp = DoAddSynced(component);
            syncedComponents.Add(className);

            return comp;
        }

        public T AddSynced<T>() where T : Component
        {
            return (T)AddSynced(typeof(T));
        }


        /// <summary>
        /// Remove the component from both the local and remote clients, and stop syncing it.
        /// This works for both Synced and non-synced components.
        /// </summary>
        public virtual void Remove(Type component)
        {
            if (!photonView.isMine)
                return;

            string className = component.FullName;
            syncedComponents.Remove(className);

            DoRemoveSynced(component);
        }

        public void Remove(Component comp)
        {
            Remove(comp.GetType());
        }

        public void Remove<T>()
        {
            Remove(typeof(T));
        }

        /// <summary>
        /// Add and Remove any synced Components
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(syncedComponents.ToArray<string>());
            }
            else
            {
                List<string> components = new List<string>((string[])stream.ReceiveNext());

                var toAdd = components.Except(syncedComponents);
                var toRemove = syncedComponents.Except(components);

                foreach (var comp in toAdd)
                    DoAddSynced(Type.GetType(comp));

                foreach (var comp in toRemove)
                    DoRemoveSynced(Type.GetType(comp));

                syncedComponents = components;
            }
        }

        protected virtual Component DoAddSynced(Type componentType)
        {
            Component comp = gameObject.AddComponent(componentType);
            photonView.ObservedComponents.Add(comp);

            return comp;
        }

        protected virtual void DoRemoveSynced(Type actionType)
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
