
using UnityEngine;

namespace Assets.Game.Scripts
{
    public interface ICanSetDestination
    {
        /// <summary>
        /// Set destionation and start moving
        /// </summary>
        /// <param name="destination"></param>
        void SetDestination(Vector3 destination);

        /// <summary>
        /// Get the previously set Destination(Might not match the final destination due to obstacles etc.)
        /// </summary>
        /// <returns></returns>
        Vector3 GetDestination();

        /// <summary>
        /// Clear the destination and stop moving
        /// </summary>
        void ClearDestination();
    }
}
