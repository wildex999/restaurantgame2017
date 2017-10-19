using ExitGames.Client.Photon;
using UnityEngine;

namespace Assets.Game.Scripts
{
    public class Login : Photon.PunBehaviour
    {
        [Tooltip("Panel with the name input and play button")]
        public GameObject panelControl;
        [Tooltip("Label showing the connection progress")]
        public GameObject labelProgress;

        [Tooltip("The scene to load once connected")]
        public string levelSceneName = "Restaurant";

        public PhotonLogLevel networkLogLevel = PhotonLogLevel.Informational;

        /// <summary>
        /// Game version should be increased every time there is breaking changes. Network players are sepparated by their gameVersion.
        /// </summary>
        string _gameVersion = "4";

        byte _maxPlayers = 3;

        /// <summary>
        /// Track whether we are trying to connect due to user action.
        /// </summary>
        bool isConnecting;

        private void Awake()
        {
            PhotonNetwork.logLevel = networkLogLevel;
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true; //Allow master client to choose level
        }

        private void Start()
        {
            labelProgress.SetActive(false);
            panelControl.SetActive(true);

            //Register types
            //ALWAYS ADD AT THE BOTTOM FOR ID COMPATIBILITY
            //ALWAYS LEAVE A typeId++ IF A TYPE IS REMOVED, TO MAINTAIN ID COMPATIBILITY
            byte typeId = 0;
            PhotonPeer.RegisterType(typeof(Order), typeId++, Order.Serialize, Order.Deserialize);
            PhotonPeer.RegisterType(typeof(Food), typeId++, Food.Serialize, Food.Deserialize);
        }

        /// <summary>
        /// Connect to the Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            isConnecting = true;
            labelProgress.SetActive(true);
            panelControl.SetActive(false);

            if (PhotonNetwork.connected)
                PhotonNetwork.JoinRandomRoom();
            else
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Master");

            //Only join room if player is trying to connect
            if (isConnecting)
            {
                //TODO: Allow player to select room
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined random room: " + PhotonNetwork.room.Name);

            PhotonNetwork.LoadLevel(levelSceneName);
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.Log("No random room exists, creating room");
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = _maxPlayers }, null);
        }

        public override void OnDisconnectedFromPhoton()
        {
            labelProgress.SetActive(false);
            panelControl.SetActive(true);
            Debug.LogWarning("Disconnected from Photon");
        }
    }
}
