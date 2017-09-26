using Assets.Game.Scripts.Customers;
using Assets.Game.Scripts.Player.Actions;
using Assets.Game.Scripts.Tables;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Game.Scripts
{
    public class PlayerController : Photon.PunBehaviour, ICanSetDestination
    {
        [Tooltip("The layer which is used for raycasting the movement position")]
        public LayerMask movementLayer;
        [Tooltip("How fast the character turns towards the new destination")]
        public float rotationSpeed = 16f;
        [Tooltip("How close to something the Player has to be before acting.")]
        public float actionDistance = 1f;

        [HideInInspector]
        public bool allowUserInput = true;


        NavMeshAgent agent;
        ActionManager actionManager;
        Vector3 prevDestination;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            actionManager = GetComponent<ActionManager>();
        }

        private void Update()
        {
            if (photonView.isMine || PhotonNetwork.connected != true)
                LocalUpdate();

            //Turn the agent
            Vector3 direction = (agent.destination - transform.position).normalized;
            if (direction.magnitude > 0f)
            {
                Quaternion targetDir = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetDir, Time.deltaTime * rotationSpeed);
            }
        }

        private void LocalUpdate()
        {
            if (allowUserInput)
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, movementLayer))
                    {
                        //A new Move always overrides an existing
                        actionManager.RemoveAction<ActionMove>();

                        ActionMove move = actionManager.AddAction<ActionMove>();
                        if (move != null)
                            move.SetDestination(hit.point);
                    }
                }
            }
        }

        public void ActionSeatCustomerGroup(CustomerGroup group)
        {
            ActionSeatCustomers action = actionManager.AddAction<ActionSeatCustomers>();
            if (action != null)
            {
                action.SetCustomerGroup(group);
            }
        }

        public void ActionTakeOrder(CustomerGroup group)
        {
            ActionTakeOrder action = actionManager.AddAction<ActionTakeOrder>();
            if(action)
                action.SetCustomerGroup(group);
        }

        public void DeliverOrder(Orders orders)
        {
            ActionTakeOrder action = GetComponent<ActionTakeOrder>();
            if (action)
                action.DeliverOrder(orders);
        }

        public void ActionGetFood(FoodDesk foodDesk)
        {
            ActionDeliverFood action = actionManager.AddAction<ActionDeliverFood>();
            if(action)
                action.foodDesk = foodDesk;
        }

        public void DeliverFood(CustomerGroup group)
        {
            if (!photonView.isMine)
                return;

            ActionDeliverFood action = gameObject.GetComponent<ActionDeliverFood>();
            if (action)
                action.DeliverTo(group);
        }

        public void ActionTakeMoney(CustomerGroup group)
        {
            ActionActOnTarget action = actionManager.AddAction<ActionActOnTarget>();
            if(action)
            {
                action.SetAction((Transform target) => {
                    group.photonView.RPC("Pay", PhotonTargets.MasterClient);
                });
                action.SetTarget(group.transform);
            }
        }

        public void ActionCleanTrash(Table table)
        {
            ActionCleanTrash action = actionManager.AddAction<ActionCleanTrash>();
            if(action)
                action.table = table;
        }

        public void SetDestination(Vector3 destination)
        {
            agent.isStopped = false;
            if (destination == prevDestination)
                return;

            agent.SetDestination(destination);
            prevDestination = destination;
        }

        public Vector3 GetDestination()
        {
            return prevDestination;
        }

        public void ClearDestination()
        {
            agent.isStopped = true;
        }
    }
}
