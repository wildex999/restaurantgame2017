using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.Game.Scripts.UI
{
    //TODO: Rename to UI library
    public class StatusIconLibrary : Photon.PunBehaviour
    {
        static StatusIconLibrary instance;

        //Prefabs
        public GameStatusIcon iconTable;
        public GameStatusIcon iconMenu;
        public GameStatusIcon iconFood;
        public GameStatusIcon iconArrow;
        public GameStatusIcon iconFoodReady;
        public GameStatusIcon iconMoney;
        public GameStatusIcon iconTrash;
        public GameNotificationIcon iconActionCompleteTick;

        //Instances
        public TableSelection TableSelectionUi;
        public GameObject mainCanvas;

        [PunRPC]
        private void _ShowTaskCompleteTick(Vector3 worldPos)
        {
            GameNotificationIcon icon = Instantiate(iconActionCompleteTick, mainCanvas.transform);
            icon.worldPosition = worldPos;
        }

        public void ShowTaskCompleteTick(Vector3 screenPos)
        {
            photonView.RPC("_ShowTaskCompleteTick", PhotonTargets.All, Camera.main.ScreenToWorldPoint(screenPos));
        }

        public static StatusIconLibrary Get()
        {
            if(instance == null)
                instance = GameObject.FindObjectOfType<StatusIconLibrary>();
            return instance;
        }
    }
}
