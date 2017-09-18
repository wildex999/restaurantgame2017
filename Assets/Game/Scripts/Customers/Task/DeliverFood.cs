using UnityEngine;

namespace Assets.Game.Scripts.Customers.Task
{
    public class DeliverFood : MonoBehaviour
    {
        public string order;

        enum State
        {
            GetFood,
            DeliverFood
        }
        State state;
    }
}
