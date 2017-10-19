
using System;
using Assets.Game.Scripts.Customers;
using Assets.Game.Scripts.Player.Actions;
using Assets.Game.Scripts.Tables;
using UnityEngine;
using Assets.Game.Scripts.DataClasses;

namespace Assets.Game.Scripts.Player
{
    public class PlayerEmployee : Photon.PunBehaviour, IPlayerRole, IPunObservable
    {
        ActionManager actionManager;
        GameObject model;

        void Start()
        {
            actionManager = GetComponent<ActionManager>();
            Controller = GetComponent<PlayerController>();

            model = Instantiate(PlayerData.Instance.employeeModel, transform);
        }

        private void OnDestroy()
        {
            Destroy(model);
        }

        public PlayerController Controller { get; set; }

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
            if (action)
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
            if (action)
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
            if (action)
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
            if (action)
                action.table = table;
        }

        public PlayerRoles GetRole()
        {
            return PlayerRoles.Employee;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
        }
    }
}
