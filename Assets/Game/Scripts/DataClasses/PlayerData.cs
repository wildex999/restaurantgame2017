using UnityEngine;

namespace Assets.Game.Scripts.DataClasses
{
    /// <summary>
    /// Contains all the values for patience/waiting time for different tasks or states
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player", order = 1)]
    public class PlayerData : ScriptableObject
    {
        static PlayerData instance;
        public static PlayerData Instance
        {
            get
            {
                if (!instance)
                    instance = Resources.Load<PlayerData>("PlayerData");
                return instance;
            }
        }

        [Tooltip("Player model for the Employee Role")]
        public GameObject employeeModel;

        [Tooltip("Player model for the Chef Role")]
        public GameObject chefModel;

        [Tooltip("Player model for the Manager Role")]
        public GameObject managerModel;
    }
}
