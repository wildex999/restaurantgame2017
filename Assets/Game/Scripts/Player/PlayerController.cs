﻿using Assets.Game.Scripts.Customers;
using Assets.Game.Scripts.Player.Actions;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Game.Scripts
{
    public class PlayerController : Photon.PunBehaviour
    {
        [Tooltip("The layer which is used for raycasting the movement position")]
        public LayerMask movementLayer;
        [Tooltip("How fast the character turns towards the new destination")]
        public float rotationSpeed = 16f;

        [HideInInspector]
        public bool allowUserInput = true;


        NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
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
                    //Remove any current actions (TODO: Write a more generic system for this)
                    Destroy(GetComponent<SeatCustomers>());

                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, movementLayer))
                        SetDestination(hit.point);
                }
            }
        }

        public void SeatCustomerGroup(CustomerGroup group)
        {
            if (!gameObject.GetComponent<SeatCustomers>())
            {
                SeatCustomers task = gameObject.AddComponent<SeatCustomers>();
                task.SetCustomerGroup(group);
            }

        }

        public void SetDestination(Vector3 destination)
        {
            agent.SetDestination(destination);
        }

        public void StopMoving()
        {
            agent.destination = transform.position;
        }
    }
}
