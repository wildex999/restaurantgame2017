using UnityEngine;

namespace Assets.Game.Scripts
{
    public class CameraController : MonoBehaviour
    {
        [Tooltip("How far away from the edge of the screen to start moving")]
        public int cameraMoveBorder = 30;
        public int moveSpeed = 25;

        Camera camera;

        private void Start()
        {
            camera = GetComponent<Camera>();
        }

        private void Update()
        {
            UpdateMovement();
            UpdateZoom();
            UpdateRotation();
        }

        private void UpdateMovement()
        {
            Vector3 movement = Vector3.zero;
            if (Input.mousePosition.x > Screen.width - cameraMoveBorder && Input.mousePosition.x <= Screen.width)
            {
                movement.x += moveSpeed * Time.deltaTime;
            }
            else if (Input.mousePosition.x < cameraMoveBorder && Input.mousePosition.x >= 0)
            {
                movement.x -= moveSpeed * Time.deltaTime;
            }
            else if (Input.mousePosition.y > Screen.height - cameraMoveBorder && Input.mousePosition.y < Screen.height)
            {
                movement.z += moveSpeed * Time.deltaTime;
            }
            else if (Input.mousePosition.y < cameraMoveBorder && Input.mousePosition.y >= 0)
            {
                movement.z -= moveSpeed * Time.deltaTime;
            }

            //We set x roation to 0 before translate to avoid moving in the y axis on world space
            Vector3 original = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(0, original.y, 0);

            transform.Translate(movement);

            //Set back to original rotation
            transform.localRotation = Quaternion.Euler(original);
        }

        private void UpdateZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if(scroll > 0f)
            {
                camera.orthographicSize--;
            } else if(scroll < 0f)
            {
                camera.orthographicSize++;
            }
        }

        private void UpdateRotation()
        {
            if(Input.GetButtonDown("RotateCamera"))
            {
                Vector3 original = camera.transform.localRotation.eulerAngles;
                camera.transform.localRotation = Quaternion.Euler(original.x, original.y - 25, original.z);
            }
        }
    }
}
