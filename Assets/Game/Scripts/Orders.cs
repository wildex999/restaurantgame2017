using System;
using Assets.Game.Scripts.UI;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Game.Scripts
{
    public class Orders : Photon.PunBehaviour, IPunObservable
    {
        [SerializeField]
        private List<Order> orders;

        GameStatusIcon icon;

        private void Start()
        {
            orders = new List<Order>();
        }

        private void OnMouseUpAsButton()
        {
            //Task Employee with delivering order
            GameManager.instance.localPlayer.DeliverOrder(this);
        }

        [PunRPC]
        public void AddOrder(Order order)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            orders.Add(order);

            //TODO: Implement Chef and remove this part
            //Test food being ready
            GameObject.FindGameObjectWithTag("FoodDesk").GetComponent<FoodDesk>().AddFood(new Food(order.name, order.customer));
        }

        /// <summary>
        /// Take the oldest order and give it to the player who called this RPC.
        /// GiveOrder will be called on the player character.
        /// </summary>
        /// <param name="playerViewId">The viewId of the photonView of the player character</param>
        [PunRPC]
        public void TakeOrder(int playerViewId)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            if (orders.Count == 0)
                return;

            //PhotonView.Find(playerViewId);
            //TODO: The Chef will use this
        }

        public List<Order> GetOrders()
        {
            return orders;
        }

        /// <summary>
        /// Show a Green Arrow Icon above the Orders Table
        /// </summary>
        /// <param name="show"></param>
        public void ShowDropIcon(bool show)
        {
            if(show && icon == null)
            {
                icon = Instantiate(StatusIconLibrary.Get().iconArrow, StatusIconLibrary.Get().mainCanvas.transform);
                icon.Follow(gameObject);
            } else if(!show && icon != null)
            {
                Destroy(icon.gameObject);
            }
        }

        public GameStatusIcon GetDropIcon()
        {
            return icon;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.isWriting)
            {
                stream.SendNext(orders.Count);
                foreach (Order order in orders)
                    stream.SendNext(order);
            } else
            {
                int orderCount = (int)stream.ReceiveNext();
                orders.Clear();
                for (int i = 0; i < orderCount; i++)
                    orders.Add((Order)stream.ReceiveNext());
            }
        }
    }
}
