using Assets.Game.Scripts.Customers.Task;
using Assets.Game.Scripts.Tables;
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
        TableGroup table;

        Vector3 currentDestination;
        bool isMoving = false;

        private void Start()
        {
            actionManager = GetComponent<ActionManager>();
            ActionGetTable();
        }

        /// <summary>
        /// Move the group while allowing the Customers to retain their original world position
        /// </summary>
        private void MoveGroupWithoutCustomers(Vector3 newPos)
        {
            //Store original positions of children
            Customer[] customers = GetComponentsInChildren<Customer>();
            Vector3[] positions = new Vector3[customers.Length];
            for (int i = 0; i < customers.Length; i++)
                positions[i] = customers[i].transform.position;

            //Move the group then move the children back to their original position
            transform.position = newPos;
            for (int i = 0; i < customers.Length; i++)
                customers[i].transform.position = positions[i];
        }

        private void Update()
        {
            if (!photonView.isMine)
                return;

            if (isMoving)
            {
                bool atDestination = true;
                foreach (Customer customer in GetComponentsInChildren<Customer>())
                {
                    if(customer.GetComponent<PathAgent>().Moving())
                    {
                        atDestination = false;
                        break;
                    }
                }

                if(atDestination)
                {
                    isMoving = false;
                    MoveGroupWithoutCustomers(currentDestination);
                }
            }
        }

        public void ActionGetTable()
        {
            actionManager.AddActionSynced<ActionGetTable>();
        }

        public void ActionOrderFood()
        {
            actionManager.AddActionSynced<ActionOrderFood>();
        }

        public void ActionEatFood()
        {
            actionManager.AddActionSynced<ActionEatFood>();
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

        /// <summary>
        /// The table the customers are currently seated at, or null if not seated.
        /// </summary>
        public TableGroup Table
        {
            get
            {
                return table;
            }
            set
            {
                table = value;
            }
        }

        /// <summary>
        /// Try to place the agent at the NavMesh closest to its current position
        /// </summary>
        public bool PlaceAtNavMesh()
        {
            if (!photonView.isMine)
                return false;

            bool placed = true;
            foreach(Customer customer in GetComponentsInChildren<Customer>())
            {
                if (!customer.GetComponent<PathAgent>().PlaceAtNavMesh())
                    placed = false;
            }

            return placed;
        }

            public void SetDestination(Vector3 destination)
        {
            if (!photonView.isMine)
                return;

            currentDestination = destination;
            isMoving = true;

            //Set destination for all Customers
            float spread = 0.5f;
            foreach (Customer customer in GetComponentsInChildren<Customer>())
                customer.GetComponent<PathAgent>().SetDestination(destination + new Vector3(UnityEngine.Random.Range(-spread, spread), 0f, UnityEngine.Random.Range(-spread, spread)));
        }

        public Vector3 GetDestination()
        {
            if (!photonView.isMine) //TODO: Return destination synced from master?
                return transform.position;

            return currentDestination;
        }

        public void ClearDestination()
        {
            if (!photonView.isMine)
                return;

            isMoving = false;
            foreach (Customer customer in GetComponentsInChildren<Customer>())
                customer.GetComponent<PathAgent>().ClearDestination();
        }
    }
}
