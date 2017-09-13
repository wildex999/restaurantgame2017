using UnityEngine;

namespace Assets.Game.Scripts.UI
{
    public class GameStatusIcon : MonoBehaviour
    {
        [HideInInspector]
        public UiFollowGameObject follow;
        [HideInInspector]
        public FadeBetweenImages imageFade;

        void Start()
        {
            follow = GetComponent<UiFollowGameObject>();
            imageFade = GetComponent<FadeBetweenImages>();
        }

        public void Follow(GameObject obj)
        {
            follow.target = obj;
        }

        /// <summary>
        /// Fade the first state image to show the second state image.
        /// 0 for no fade, 1 for fully transparent.
        /// </summary>
        /// <param name="fade"></param>
        public void SetFade(float fade)
        {
            imageFade.SetFade(fade);
        }
    }
}
