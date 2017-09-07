using UnityEngine;

namespace Assets.Game.Scripts
{
    public class CameraController : MonoBehaviour
    {
        [Tooltip("How far away from the edge of the screen to start moving")]
        public int cameraMoveBorder = 30;
        public int moveSpeed = 25;


        private void Update()
        {
            Vector3 movement = Vector3.zero;
            if (Input.mousePosition.x > Screen.width - cameraMoveBorder && Input.mousePosition.x <= Screen.width)
            {
                movement.x += moveSpeed * Time.deltaTime;
            } else if(Input.mousePosition.x < cameraMoveBorder && Input.mousePosition.x >= 0)
            {
                movement.x -= moveSpeed * Time.deltaTime;
            } else if(Input.mousePosition.y > Screen.height - cameraMoveBorder && Input.mousePosition.y < Screen.height)
            {
                movement.y += moveSpeed * Time.deltaTime;
            } else if(Input.mousePosition.y < cameraMoveBorder && Input.mousePosition.y >= 0)
            {
                movement.y -= moveSpeed * Time.deltaTime;
            }

            transform.Translate(movement);
        }
    }
}
