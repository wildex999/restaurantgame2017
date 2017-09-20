using UnityEngine;

namespace Assets.Game.Scripts.UI
{
    public class GameStatusIcon : MonoBehaviour
    {
        [HideInInspector]
        public UiFollowGameObject follow;
        [HideInInspector]
        public FadeBetweenImages imageFade;

        public bool StopOverlap
        {
            get
            {
                BoxCollider2D col = GetComponent<BoxCollider2D>();
                if (col == null)
                    return true;
                return col.enabled;
            }
            set
            {
                BoxCollider2D col = GetComponent<BoxCollider2D>();
                if (col == null)
                    return;
                col.enabled = value;
            }
        }

        void Awake()
        {
            follow = GetComponent<UiFollowGameObject>();
            imageFade = GetComponent<FadeBetweenImages>();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            GameStatusIcon otherIcon = collision.GetComponent<GameStatusIcon>();

            float xOffset = 0;
            float yOffset = 0;
            float speed = 0.5f;

            if (transform.position.x <= otherIcon.transform.position.x)
                xOffset -= speed;
            else
                xOffset += speed;

           /* if (follow)
            {
                if (transform.position.y >= otherIcon.transform.position.y)
                {
                    if (transform.position.y >= follow.GetTargetScreenPosition().y)
                    {
                        yOffset += speed;
                    }
                }
            }*/

            transform.position = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, transform.position.z);
        }

        public void Follow(GameObject obj)
        {
            if (!follow)
                return;

            follow.SetTarget(obj);
        }

        /// <summary>
        /// Fade the first state image to show the second state image.
        /// 0 for no fade, 1 for fully transparent.
        /// </summary>
        /// <param name="fade"></param>
        public void SetFade(float fade)
        {
            if (!imageFade)
                return;

            imageFade.SetFade(fade);
        }
    }
}
