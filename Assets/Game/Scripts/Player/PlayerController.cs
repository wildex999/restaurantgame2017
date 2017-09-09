using UnityEngine;
using UnityEngine.AI;

namespace Assets.Game.Scripts
{
    public class PlayerController : Photon.PunBehaviour
    {
        [Tooltip("The layer which is used for raycasting the movement position")]
        public LayerMask movementLayer;

        NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (photonView.isMine || PhotonNetwork.connected != true)
                LocalUpdate();
        }

        private void LocalUpdate()
        {
            //TODO: Rewrite this to use Input configuration and hit specific "Click" object(Floor plate)
            if (Input.GetButtonDown("Fire2"))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, movementLayer))
                {
                    agent.destination = hit.point;
                }
            }
        }
    }
}
