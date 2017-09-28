using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Game.Scripts.DataClasses
{
    /// <summary>
    /// Timings for how long certain actions or events should take.
    /// </summary>
    [CreateAssetMenu(fileName = "TimingData", menuName = "Data/Timing", order = 2)]
    public class TimingData : ScriptableObject
    {
        static TimingData instance;
        public static TimingData Instance
        {
            get
            {
                if (!instance)
                    instance = Resources.Load<TimingData>("TimingData");
                return instance;
            }
        }

        [Tooltip("How many seconds Customers will use to eat a meal")]
        public Data.IntRange timeToEat;

        [Tooltip("How many seconds it will take to create a meal after receiving the order")]
        public Data.IntRange timeToMakeFood;
    }
}
