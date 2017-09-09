using System;
using UnityEngine;

namespace Assets.Game.Scripts.Customers
{
    public class Customer : Photon.PunBehaviour
    {
        CustomerGroup group;

        public void SetGroup(CustomerGroup group)
        {
            photonView.RPC("NetworkSetGroup", PhotonTargets.AllBuffered, group.GetComponent<PhotonView>().viewID);
        }

        [PunRPC]
        void NetworkSetGroup(int viewId)
        {
            PhotonView groupView = PhotonView.Find(viewId);
            if(groupView == null)
            {
                Debug.LogError("Unable to set Customer group, group does not exist: " + viewId);
                return;
            }

            this.group = groupView.GetComponent<CustomerGroup>();
            this.transform.SetParent(group.transform);
        }
    }
}
