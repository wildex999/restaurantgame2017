using Assets.Game.Scripts.Player;
using Assets.Game.Scripts.Player.Actions;
using Assets.Game.Scripts.UI;
using Assets.Game.Scripts.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts
{
    public class FoodDesk : Photon.PunBehaviour, IPunObservable
    {
        Queue<Food> readyFood; //Food orders ready to pick up
        GameStatusIcon icon;
        Observable<PlayerEmployee> employee;

        private void Start()
        {
            employee = GameManager.instance.localPlayer.Employee();
            readyFood = new Queue<Food>();
        }

        private void UpdateReadyIcon()
        {
            if (readyFood.Count > 0)
                ShowReadyIcon(true);
            else
                ShowReadyIcon(false);
        }

        private void OnMouseUpAsButton()
        {
            if (!employee || readyFood.Count == 0)
                return;

            //Task Employee with getting food
            employee.Value.ActionGetFood(this);
        }

        /// <summary>
        /// Show an icon indicating there is food to pickup
        /// </summary>
        /// <param name="show"></param>
        public void ShowReadyIcon(bool show)
        {
            if (show && icon == null)
            {
                icon = Instantiate(StatusIconLibrary.Get().iconFoodReady, StatusIconLibrary.Get().mainCanvas.transform);
                icon.Follow(gameObject);
            }
            else if (!show && icon != null)
            {
                Destroy(icon.gameObject);
            }
        }

        [PunRPC]
        public void AddFood(Food food)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            readyFood.Enqueue(food);
            UpdateReadyIcon();
        }

        /// <summary>
        /// Take the oldest food and give it to the player who called this RPC.
        /// GiveFood will be called on the player character.
        /// </summary>
        /// <param name="playerViewId">The viewId of the photonView of the player character</param>
        [PunRPC]
        public void TakeOrder(int playerViewId, PhotonMessageInfo info)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            Food food = null;
            if (readyFood.Count > 0)
                food = readyFood.Dequeue();

            PhotonView.Find(playerViewId).photonView.RPC("GiveFood", info.sender, food);
            StatusIconLibrary.Get().ShowTaskCompleteTick(icon.transform.position);
            UpdateReadyIcon();
        }

        public int CountFood()
        {
            return readyFood.Count;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(readyFood.Count);
                foreach (Food food in readyFood)
                    stream.SendNext(food);
            }
            else
            {
                int foodCount = (int)stream.ReceiveNext();
                readyFood.Clear();
                for (int i = 0; i < foodCount; i++)
                    readyFood.Enqueue((Food)stream.ReceiveNext());

                UpdateReadyIcon();
            }
        }
    }
}
