using Assets.Game.Scripts.Customers;
using System;
using UnityEngine;

namespace Assets.Game.Scripts.Tables
{
    public class Chair : Photon.PunBehaviour, IPunObservable
    {
        public Customer seatedCustomer;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.isWriting)
            {
                if (seatedCustomer != null)
                    stream.SendNext(seatedCustomer.photonView.viewID);
                else
                    stream.SendNext(-1);
            }
            else
            {
                int customerId = (int)stream.ReceiveNext();
                PhotonView customerView = PhotonView.Find(customerId);
                if (customerView != null)
                    seatedCustomer = customerView.GetComponent<Customer>();
                else
                    seatedCustomer = null;
            }
        }
    }
}
