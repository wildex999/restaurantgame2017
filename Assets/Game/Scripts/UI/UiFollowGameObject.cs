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
        [SerializeField]
        private GameObject target;
        public Camera uiCamera;

        Vector3 prevTargetPosition;

        void Update()
        {

            Vector3 targetPos = GetTargetScreenPosition();
            if (Vector3.Distance(prevTargetPosition, targetPos) > 0.1f)
            {
                //Only set position when target moves.
                //This allows icon to move about when standing still(Don't overlap etc. on table)
                prevTargetPosition = targetPos;
                transform.position = targetPos;
            } else
            {
                //Move the icon towards its "real" position.
                transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.1f);
            }



        }

        public Vector2 GetTargetScreenPosition()
        {
            if (target == null)
                return Vector2.zero;
            if (uiCamera == null)
                uiCamera = Camera.main;

            return uiCamera.WorldToScreenPoint(target.transform.position + offset);
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;

            transform.position = GetTargetScreenPosition();
        }

        public GameObject GetTarget()
        {
            return target;
        }
    }
}
