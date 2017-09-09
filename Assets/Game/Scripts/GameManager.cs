using Assets.Game.Scripts.Customers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Game.Scripts
{
    public class GameManager : Photon.PunBehaviour
    {
        
        public GameObject playerPrefab;
        public Transform playerSpawn;
        public CustomerSpawn customerSpawn;

        //Local
        public PlayerController localPlayer;
        public PlayerRoles localPlayerRole;

        #region Photon Messages

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            Debug.Log("New player connected: " + newPlayer.NickName);
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            Debug.Log("Player disconnected: " + otherPlayer.NickName);
        }

        /// <summary>
        /// Called when the Local player has left the room.
        /// We will return to the login screen.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }


        #endregion

        void Start()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("No player prefab set");
                return;
            }

            Debug.Log("Instantiating Local player");
            GameObject localPlayerObj = PhotonNetwork.Instantiate(this.playerPrefab.name, playerSpawn.position, Quaternion.identity, 0);
            localPlayer = localPlayerObj.GetComponent<PlayerController>();
            localPlayerRole = PlayerRoles.Employee; //TODO: Allow player to select role on joining
        }

        public void LeaveRoom()
        {
            //TODO: Do required tasks before leaving(Save etc.)
            PhotonNetwork.LeaveRoom();
        }
    }
}
