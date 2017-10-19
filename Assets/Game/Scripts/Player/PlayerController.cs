using Assets.Game.Scripts.Customers;
using Assets.Game.Scripts.Player;
using Assets.Game.Scripts.Player.Actions;
using Assets.Game.Scripts.Tables;
using Assets.Game.Scripts.Util;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Game.Scripts
{
    public class PlayerController : Photon.PunBehaviour, ICanSetDestination
    {
        [Tooltip("The layer which is used for raycasting the movement position")]
        public LayerMask movementLayer;
        [Tooltip("How close to something the Player has to be before acting.")]
        public float actionDistance = 1f;

        [HideInInspector]
        public bool allowUserInput = true;

        ActionManager actionManager;
        ComponentManager componentManager;
        PathAgent agent;

        //Observables for allowing actions to act if not null
        Observable<PlayerEmployee> observableEmployee = new Observable<PlayerEmployee>(null);
        Observable<PlayerChef> observableChef = new Observable<PlayerChef>(null);
        //Observable<PlayerManager> observableManager;

        void Start()
        {
            actionManager = GetComponent<ActionManager>();
            componentManager = GetComponent<ComponentManager>();
            agent = GetComponent<PathAgent>();

            if(this == GameManager.instance.localPlayer)
            {
                SetCurrentRole(GameManager.instance.localPlayerRole.Value);
                GameManager.instance.localPlayerRole.OnValueChanged += (ev) => SetCurrentRole(GameManager.instance.localPlayerRole.Value);
            }
        }

        private void Update()
        {
            if (photonView.isMine || PhotonNetwork.connected != true)
                LocalUpdate();
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

        private void SetCurrentRole(PlayerRoles newRole)
        {
            //Remove the current role
            PlayerRoles currentRole = GetCurrentRole();
            switch(currentRole)
            {
                case PlayerRoles.Employee:
                    observableEmployee.Value = null;
                    componentManager.Remove<PlayerEmployee>();
                    break;

                case PlayerRoles.Chef:
                    observableChef.Value = null;
                    componentManager.Remove<PlayerChef>();
                    break;

                case PlayerRoles.Manager:
                    //TODO
                    break;
            }

            //Add the new role
            switch(newRole)
            {
                case PlayerRoles.Employee:
                    observableEmployee.Value = componentManager.AddSynced<PlayerEmployee>();
                    break;

                case PlayerRoles.Chef:
                    observableChef.Value = componentManager.AddSynced<PlayerChef>();
                    break;

                case PlayerRoles.Manager:
                    //TODO
                    break;
            }
        }

        public PlayerRoles GetCurrentRole()
        {
            if (GetComponent<PlayerEmployee>() != null)
                return PlayerRoles.Employee;

            return PlayerRoles.None;
        }

        public Observable<PlayerEmployee> Employee()
        {
            return observableEmployee;
        }

        public void SetDestination(Vector3 destination)
        {
            agent.SetDestination(destination);
        }

        public Vector3 GetDestination()
        {
            return agent.GetDestination();
        }

        public void ClearDestination()
        {
            agent.ClearDestination();
        }
    }
}
