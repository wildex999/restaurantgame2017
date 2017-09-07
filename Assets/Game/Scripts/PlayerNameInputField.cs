using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts
{
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        static string playerNamePrefKey = "PlayerName";

        private void Start()
        {
            string defaultName = PlayerPrefs.GetString(playerNamePrefKey);
            if (defaultName == null)
                defaultName = "";

            InputField inputField = this.GetComponent<InputField>();
            if (inputField != null)
                inputField.text = defaultName;

            PhotonNetwork.playerName = defaultName;
        }

        public void SetPlayerName(string value)
        {
            if (value.Length == 0)
                value = " ";

            PhotonNetwork.playerName = value;
            PlayerPrefs.SetString(playerNamePrefKey, value);
        }
    }
}
