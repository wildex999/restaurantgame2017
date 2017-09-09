
using System;
using Assets.Game.Scripts.Customers.Task;
using UnityEngine;
using cakeslice;
using System.Collections.Generic;

namespace Assets.Game.Scripts.Customers
{
    public class CustomerGroup : Photon.PunBehaviour, IPunObservable
    {
        public float satisfaction = 100;
        public float patience = 100;

        List<Outline> outlines;

        private void Start()
        {
            outlines = new List<Outline>();
            if (photonView.isMine)
            {
                photonView.RPC("ActionGetTable", PhotonTargets.AllBuffered);
            }
        }

        [PunRPC]
        void ActionGetTable()
        {
            GetTable action = gameObject.AddComponent<GetTable>();
            photonView.ObservedComponents.Add(action);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.isWriting)
            {
                stream.SendNext(satisfaction);
                stream.SendNext(patience);
            } else
            {
                satisfaction = (float)stream.ReceiveNext();
                patience = (float)stream.ReceiveNext();
            }
        }

        private void OnMouseOver()
        {
            foreach(MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                if(renderer.gameObject.GetComponent<Outline>() == null)
                    outlines.Add(renderer.gameObject.AddComponent<Outline>());
            }
        }

        private void OnMouseExit()
        {
            foreach(Outline outline in outlines)
                Destroy(outline);
        }
    }
}
