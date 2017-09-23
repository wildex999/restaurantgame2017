using Assets.Game.Scripts.Customers.Task;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Game.Scripts.Customers
{
    public class CustomerGroup : Photon.PunBehaviour, IPunObservable, ICanSetDestination
    {
        [Tooltip("How long they will wait before becoming dissatisfied, as a percentage.")]
        public float waiting = 100; //TODO: Find a better name for this variable
        [Tooltip("Patience in Percentage. Higher patience means slower Satisfaction loss.")]
        public float patience = 100;

        ActionManager actionManager;
        NavMeshAgent agent;
        Vector3 prevDestination;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            //Non-master clients don't need to move about on their own
            if (!photonView.isMine)
            {
                Destroy(agent);
                agent = null;
            }

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

        public void SetDestination(Vector3 destination)
        {
            if (agent == null)
                return;

            agent.isStopped = false;
            if(destination != prevDestination)
                agent.SetDestination(destination);
            prevDestination = destination;
        }

        public Vector3 GetDestination()
        {
            return prevDestination;
        }

        public void ClearDestination()
        {
            if (agent == null)
                return;

            agent.isStopped = true;
        }
    }
}
