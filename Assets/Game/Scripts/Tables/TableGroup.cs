using System.Collections.Generic;

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

        public Table GetTable()
        {
            return GetComponentInChildren<Table>();
        }

        public List<Chair> GetChairs()
        {
            return chairs;
        }
    }
}
