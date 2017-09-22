using Assets.Game.Scripts.Customers.Task;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Game.Scripts.Customers
{
    public class CustomerGroup : Photon.PunBehaviour, IPunObservable
    {
        [Tooltip("How long they will wait before becoming dissatisfied, as a percentage.")]
        public float waiting = 100; //TODO: Find a better name for this variable
        [Tooltip("Patience in Percentage. Higher patience means slower Satisfaction loss.")]
        public float patience = 100;

        ActionManager actionManager;

        private void Start()
        {
            //Non-master clients don't need to move about on their own
            if(!photonView.isMine)
                Destroy(GetComponent<NavMeshAgent>());

            actionManager = GetComponent<ActionManager>();
            ActionGetTable();
        }

        [PunRPC]
        public void ActionGetTable()
        {
            actionManager.AddActionSynced(typeof(ActionGetTable));
        }

        [PunRPC]
        public void ActionOrderFood()
        {
            actionManager.AddActionSynced(typeof(ActionOrderFood));
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
    }
}
