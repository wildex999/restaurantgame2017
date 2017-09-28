using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Game.Scripts.UI
{
    public class GameStatusIcon : MonoBehaviour
    {
        [HideInInspector]
        public UiFollowGameObject follow;
        [HideInInspector]
        public FadeBetweenImages imageFade;

        EventTrigger trigger;

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

        private void Start()
        {
            //Setup UI events
            trigger = gameObject.AddComponent<EventTrigger>();
            AddUIEvent(EventTriggerType.PointerEnter, (ev) => OnHoverStart());
            AddUIEvent(EventTriggerType.PointerExit, (ev) => OnHoverEnd());
            AddUIEvent(EventTriggerType.PointerClick, (ev) => OnClick());
        }

        private void AddUIEvent(EventTriggerType type, UnityAction<BaseEventData> handler)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(handler);

            trigger.triggers.Add(entry);
        }

        private void OnClick()
        {
            if (!follow || follow.GetTarget() == null)
                return;

            //Send click to the target object
            follow.GetTarget().SendMessage("OnMouseUpAsButton");
        }

        private void OnHoverStart()
        {
            if (!follow || follow.GetTarget() == null)
                return;

            follow.GetTarget().SendMessage("OnMouseOver");
        }

        private void OnHoverEnd()
        {
            if (!follow || follow.GetTarget() == null)
                return;

            follow.GetTarget().SendMessage("OnMouseExit");
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
