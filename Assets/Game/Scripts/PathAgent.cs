
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Game.Scripts
{
    /// <summary>
    /// Custom NavMesh Agent. Will move along the NavMesh and ignore other agents(Collision).
    /// </summary>
    public class PathAgent : Photon.PunBehaviour
    {
        [Tooltip("How fast the character turns towards the new destination")]
        public float rotationSpeed = 16f;
        [Tooltip("How fast the character moves")]
        public float moveSpeed = 3.5f;

        NavMeshPath path;
        public int currentCorner = 0;
        Vector3 prevDestination = new Vector3(-1000000,-100000);

        private void Start()
        {
            path = new NavMeshPath();
        }

        private void Update()
        {
            if (!photonView.isMine)
                return;

            //Move along path
            if (path.corners.Length > 0)
            {
                Vector3 corner = path.corners[currentCorner];
                transform.position = Vector3.MoveTowards(transform.position, corner, moveSpeed * Time.deltaTime);

                //Turn the agent
                Vector3 direction = (corner - transform.position).normalized;
                if (direction.magnitude > 0f)
                {
                    Quaternion targetDir = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetDir, Time.deltaTime * rotationSpeed);
                }

                //Update corner
                if (Vector3.Distance(transform.position, corner) < 0.05f)
                {
                    transform.position = corner;
                    currentCorner++;
                }

                if (currentCorner >= path.corners.Length)
                    path.ClearCorners();
            }
        }

        /// <summary>
        /// Try to place the agent at the NavMesh closest to its current position
        /// </summary>
        public bool PlaceAtNavMesh()
        {
            NavMeshHit hit;
            bool gotPoint = NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas);
            if (!gotPoint)
                return false;

            transform.position = hit.position;
            return true;
        }

        public void SetDestination(Vector3 destination)
        {
            if (destination == prevDestination)
                return;

            NavMeshHit closest;
            bool gotDestination = NavMesh.SamplePosition(destination, out closest, 1f, NavMesh.AllAreas);
            if(!gotDestination)
            {
                Debug.LogError("Unable to find destination point for " + destination);
                return;
            }

            bool gotPath = NavMesh.CalculatePath(transform.position, closest.position, NavMesh.AllAreas, path);
            if(!gotPath)
            {
                Debug.LogError("Unable to find path to " + closest.position);
            }

            currentCorner = 0;
            prevDestination = destination;
        }

        public Vector3 GetDestination()
        {
            return prevDestination;
        }

        public bool Moving()
        {
            return path.corners.Length > 0;
        }

        public void ClearDestination()
        {
            path.ClearCorners();
        }
    }
}
