using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.UI
{
    /// <summary>
    /// A Notification Icon is a short lived icon which will slowly move up and fade away.
    /// It is usually used to notify of some event or action happening at a certain place.
    /// </summary>
    public class GameNotificationIcon : MonoBehaviour
    {
        public float fadeSpeed = 0.35f;
        public Vector2 moveSpeed = new Vector2(0f, 32f);

        public Vector3 worldPosition; //Store world position to allow camera movement

        private RawImage image;
        private double timeStart;


        private void Start()
        {
            image = GetComponent<RawImage>();
            timeStart = Time.time;
        }

        private void Update()
        {
            if (worldPosition == null)
                return;

            float deltaTime = (float)(Time.time - timeStart);
            float dx = moveSpeed.x * deltaTime;
            float dy = moveSpeed.y * deltaTime;

            //The camera might move, so we always get the screen position from the original world position
            Vector2 pos = Camera.main.WorldToScreenPoint(worldPosition);
            transform.position = new Vector2(pos.x + dx, pos.y + dy);

            float df = fadeSpeed * Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - df);

            if (image.color.a <= 0)
                Destroy(gameObject);
        }
    }
}
