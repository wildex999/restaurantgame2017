using System;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Game.Scripts
{
    /// <summary>
    /// Sync whether the navigation is enabled or not
    /// </summary>
    public class SyncNavigationState : Photon.PunBehaviour, IPunObservable
    {
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (stream.isWriting)
            {
                if (agent == null)
                    stream.SendNext(false);
                else
                    stream.SendNext(agent.enabled);
            }
            else
            {
                bool enabled = (bool)stream.ReceiveNext();
                if (agent != null)
                    agent.enabled = enabled;
            }
        }
    }
}
