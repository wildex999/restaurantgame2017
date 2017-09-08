using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts
{
    /// <summary>
    /// Fade the opacity of walls when they are between the camera and the player.
    /// </summary>
    public class WallFade : MonoBehaviour
    {
        public Transform player;

        [Tooltip("The layer of the wall objects to test against")]
        public LayerMask wallLayer;

        private Camera camera;
        private List<MeshRenderer> renderers;

        private void Start()
        {
            camera = GetComponent<Camera>();
            renderers = new List<MeshRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            //Keep track of which walls we need to re-show if they are no longer in the way
            List<MeshRenderer> toShow = new List<MeshRenderer>(renderers);

            //Check if there are any walls between the camera and the player, and hide them
            RaycastHit hitInfo;
            Vector3 screenPos = camera.WorldToScreenPoint(player.position);
            Ray ray = camera.ScreenPointToRay(screenPos);
            if(Physics.Raycast(ray, out hitInfo, 1000, wallLayer))
            {
                MeshRenderer renderer = hitInfo.collider.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    if (!toShow.Remove(renderer)) //We still need to hide this wall
                    {
                        //Track which walls we have hidden
                        renderers.Add(renderer);

                        renderer.enabled = false;
                    }
                }
            }
            //Debug.DrawRay(ray.origin, ray.direction*1000);

            //Re-Show any walls which are not longer in the way
            foreach (MeshRenderer renderer in toShow)
            {
                renderer.enabled = true;
                renderers.Remove(renderer);
            }
        }
    }
}
