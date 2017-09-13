using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Tables
{
    public class TableGroup : Photon.PunBehaviour
    {
        public List<Chair> chairs;

        private void Start()
        {
            chairs = new List<Chair>();
            chairs.AddRange(GetComponentsInChildren<Chair>());
        }

        public List<Chair> GetChairs()
        {
            return chairs;
        }
    }
}
