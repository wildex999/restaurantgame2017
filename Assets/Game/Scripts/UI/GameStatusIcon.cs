using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Game.Scripts.UI
{
    /// <summary>
    /// Icon which will follow a object, and fade between two different images based on a patience value.
    /// The icon will only be active and visible if the local player role matches any of those set on the icon.
    /// </summary>
    public class GameStatusIcon : MonoBehaviour
    {
        [HideInInspector]
        public UiFollowGameObject follow;
        [HideInInspector]
        public FadeBetweenImages imageFade;

        [Tooltip("Which player roles to be active for. " +
            "If the local player has a different role than these, the icon will be invisible and disabled. " +
            "If no role is selected, it will match all roles.")]
        public PlayerRoles[] activeForRoles;

        Patience patience;
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

            GameManager.instance.localPlayerRole.OnValueChanged += (ev) => UpdateForRole();
            UpdateForRole();
        }

        private void Update()
        {
            if (!patience)
            {
                SetFade(0);
                return;
            }

            //Update Icon to indicate waiting time
            SetFade(1f - (patience.Remaining / patience.Initial));
        }

        private void UpdateForRole()
        {
            if (activeForRoles.Length == 0 || Array.IndexOf(activeForRoles, GameManager.instance.localPlayerRole.Value) != -1)
            {
                if (!gameObject.activeSelf)
                    gameObject.SetActive(true);
            }
            else
            {
                if (gameObject.activeSelf)
                    gameObject.SetActive(false);
            }
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
        /// Set Patience object which will be tracked to fade the icon
        /// </summary>
        /// <param name="patience"></param>
        public void SetPatience(Patience patience)
        {
            this.patience = patience;
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
