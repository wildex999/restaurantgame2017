using Assets.Game.Scripts.Customers;
using Assets.Game.Scripts.UI;
using Assets.Game.Scripts.Util;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Game.Scripts
{
    public class GameManager : Photon.PunBehaviour
    {
        public static GameManager instance;
        
        public GameObject playerPrefab;
        public GameObject roleSelectPrefab;

        public Transform playerSpawn;
        public CustomerSpawn customerSpawn;

        //Local
        public PlayerController localPlayer;
        public Observable<PlayerRoles> localPlayerRole = new Observable<PlayerRoles>(PlayerRoles.None);


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

        private void Awake()
        {
            instance = this;

            if (playerPrefab == null)
            {
                Debug.LogError("No player prefab set");
                return;
            }

            Debug.Log("Instantiating Local player");
            GameObject localPlayerObj = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawn.position, Quaternion.identity, 0);
            localPlayer = localPlayerObj.GetComponent<PlayerController>();
        }

        void Start()
        {
            //Allow the local player to select a role
            Instantiate(roleSelectPrefab, StatusIconLibrary.Get().mainCanvas.transform);
        }

        public void LeaveRoom()
        {
            //TODO: Do required tasks before leaving(Save etc.)
            PhotonNetwork.LeaveRoom();
        }
    }
}
