using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Customers
{
    /// <summary>
    /// Tracks the customer queue, giving them the correct position to stand given their place in the queue
    /// </summary>
    public class CustomerQueue : MonoBehaviour
    {
        Queue<CustomerGroup> queue;
        float positionSpacing = 1.5f;

        void Start()
        {
            queue = new Queue<CustomerGroup>();
        }

        public Vector3 NextQueuePosition()
        {
            //TODO: Spread queue positions depending on group size on every occupied spot
            float offset = queue.Count * positionSpacing;
            return transform.position + transform.TransformDirection(new Vector3(0, 0, offset));
        }

        public void EnterQueue(CustomerGroup group)
        {
            queue.Enqueue(group);
        }

        public void LeaveQueue(CustomerGroup group)
        {

        }

        /*public Vector3 GetQueuePosition(CustomerGroup group)
        {

        }*/
    }
}
