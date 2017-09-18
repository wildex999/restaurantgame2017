using Assets.Game.Scripts.UI;
using UnityEngine;
namespace Assets.Game.Scripts
{
    public class Orders : MonoBehaviour
    {
        GameStatusIcon icon;

        private void OnMouseUpAsButton()
        {
            //Task Employee with delivering order
            GameManager.instance.localPlayer.DeliverOrder(this);
        }

        /// <summary>
        /// Show a Green Arrow Icon above the Orders Table
        /// </summary>
        /// <param name="show"></param>
        public void ShowIcon(bool show)
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
    }
}
