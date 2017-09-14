
using System;
using Assets.Game.Scripts.Customers.Task;
using UnityEngine;
using cakeslice;
using System.Collections.Generic;

namespace Assets.Game.Scripts.Customers
{
    public class CustomerGroup : Photon.PunBehaviour, IPunObservable
    {
        [Tooltip("How long they will wait before becoming dissatisfied, as a percentage.")]
        public float waiting = 100; //TODO: Find a better name for this variable
        [Tooltip("Patience in Percentage. Higher patience means slower Satisfaction loss.")]
        public float patience = 100;

        List<Outline> outlines;

        private void Start()
        {
            outlines = new List<Outline>();
            StartActionGetTable();
        }

        public void StartActionGetTable()
        {
            if(photonView.isMine)
                photonView.RPC("ActionGetTable", PhotonTargets.AllBuffered);
        }

        public void StartActionOrderFood()
        {
            if(photonView.isMine)
                photonView.RPC("ActionOrderFood", PhotonTargets.AllBuffered);
        }

        [PunRPC]
        void ActionGetTable()
        {
            GetTable action = gameObject.AddComponent<GetTable>();
            photonView.ObservedComponents.Add(action);
        }

        [PunRPC]
        void ActionOrderFood()
        {
            OrderFood action = gameObject.AddComponent<OrderFood>();
            photonView.ObservedComponents.Add(action);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.isWriting)
            {
                stream.SendNext(waiting);
                stream.SendNext(patience);
            } else
            {
                waiting = (float)stream.ReceiveNext();
                patience = (float)stream.ReceiveNext();
            }
        }

        public int GetCustomerCount()
        {
            return transform.childCount;
        }

        public Customer[] GetCustomers()
        {
            return GetComponentsInChildren<Customer>();
        }

        public bool HasCustomer(Customer customer)
        {
            foreach (Customer c in GetComponentsInChildren<Customer>())
                if(c == customer)
                    return true;
            return false;
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
