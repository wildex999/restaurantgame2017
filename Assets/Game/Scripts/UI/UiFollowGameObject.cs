using UnityEngine;

namespace Assets.Game.Scripts.UI
{
    /// <summary>
    /// Make the Screen space UI follow the World Space GameObject
    /// </summary>
    public class UiFollowGameObject : MonoBehaviour
    {
        [Tooltip("The offset to apply in World Space")]
        public Vector3 offset;
        public GameObject target;
        public Camera uiCamera;

        void Update()
        {
            if (target == null)
                return;
            if (uiCamera == null)
                uiCamera = Camera.main;
            if (offset == null)
                offset = Vector2.zero;

            Vector2 pos = uiCamera.WorldToScreenPoint(target.transform.position + offset);
            transform.position = pos;
        }
    }
}
