using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Game.Scripts
{
    /// <summary>
    /// Patience will always decrease while waiting for a player to complete a specific task.
    /// The score applied for completing a task will depend on how low Patience remains at that time.
    /// </summary>
    public class Patience : Photon.MonoBehaviour, IPunObservable
    {
        bool running = false;

        public float Initial { get; set; }

        [SerializeField]
        private float remaining;
        public float Remaining
        {
            get
            {
                return remaining;
            }
            set
            {
                remaining = value;
            }
        }

        private void Update()
        {
            if (!running || !photonView.isMine)
                return;

            Remaining -= Time.deltaTime;
        }

        public void Run(float initialPatience)
        {
            if (!photonView.isMine)
                return;

            Initial = initialPatience;
            Remaining = Initial;
            running = true;
        }

        public void Pause()
        {
            if (!photonView.isMine)
                return;

            running = false;
        }

        public void Resume()
        {
            if (!photonView.isMine)
                return;

            running = true;
        }

        public void Stop()
        {
            if (!photonView.isMine)
                return;

            running = false;
            Remaining = 0;
            Initial = 0;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.isWriting)
            {
                stream.SendNext(Initial);
                stream.SendNext(Remaining);
            } else
            {
                Initial = (float)stream.ReceiveNext();
                Remaining = (float)stream.ReceiveNext();
            }
        }
    }
}
