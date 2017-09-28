using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Game.Scripts.DataClasses
{
    public class Data
    {
        [System.Serializable]
        public class FloatRange
        {
            public float min = 100f;
            public float max = 100f;
        }

        [System.Serializable]
        public class IntRange
        {
            public int min = 100;
            public int max = 100;
        }
    }
}
