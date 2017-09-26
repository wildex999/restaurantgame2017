
using Assets.Game.Scripts.Player.Actions;
using Assets.Game.Scripts.UI;
using UnityEngine;

namespace Assets.Game.Scripts
{
    public class Washer : MonoBehaviour
    {
        public static string washerTag = "Washer";
        bool show;
        GameStatusIcon icon;

        private void OnMouseUpAsButton()
        {
            ActionCleanTrash playerAction = GameManager.instance.localPlayer.GetComponent<ActionCleanTrash>();
            if (!playerAction)
                return;

            playerAction.SetWasher(this);
        }

        /// <summary>
        /// Show a Green Arrow Icon above the Washer
        /// </summary>
        /// <param name="show"></param>
        public void ShowDropIcon(bool show)
        {
            if (show && icon == null)
            {
                icon = Instantiate(StatusIconLibrary.Get().iconArrow, StatusIconLibrary.Get().mainCanvas.transform);
                icon.Follow(gameObject);
            }
            else if (!show && icon != null)
            {
                Destroy(icon.gameObject);
            }
        }

        public GameStatusIcon GetDropIcon()
        {
            return icon;
        }
    }
}
