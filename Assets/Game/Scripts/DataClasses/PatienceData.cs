using UnityEditor;
using UnityEngine;

namespace Assets.Game.Scripts.DataClasses
{
    /// <summary>
    /// Contains all the values for patience/waiting time for different tasks or states
    /// </summary>
    [CreateAssetMenu(fileName = "PatienceData", menuName = "Data/Patience", order = 1)]
    public class PatienceData : ScriptableObject
    {
        static PatienceData instance;
        public static PatienceData Instance
        {
            get {
                if (!instance)
                    instance = Resources.Load<PatienceData>("PatienceData");
                return instance;
            }
        }

        [Tooltip("Waiting for a table")]
        public Data.IntRange waitForTable;

        [Tooltip("Waiting for Order to be taken")]
        public Data.IntRange waitTakeOrder;

        [Tooltip("Waiting for food to be delivered")]
        public Data.IntRange waitForFood;

        [Tooltip("Waiting to pay the bill")]
        public Data.IntRange waitPayBill;

        [Tooltip("Time before table is considered dirty")]
        public Data.IntRange waitCleanTable;
    }
}
