using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Customers
{
    public class CustomerSpawn : Photon.PunBehaviour
    {
        public GameObject customerGroupPrefab;

        [Tooltip("List of Customer prefabs which will be randomly choosen to spawn in a group")]
        public List<Customer> customerPrefabs;

        //[Tooltip("The percentage chance of a Customer group to spawn every tick. A number between 0 and 100.")]
        //public float spawnChance;
        [Tooltip("Max number of customer groups after which it will stop spawning more.")]
        public int maxGroups = 10;

        [System.Serializable]
        public struct GroupChance
        {
            public int groupSize;
            public float groupChance;
        }
        [Tooltip("The group size and the chance of that group spawning. Chance is a number between 0 and 100")]
        public List<GroupChance> groupChance;


        private void Update()
        {
            //Only the Master Client will control the spawning
            if (PhotonNetwork.isMasterClient)
            {
                //TODO: Make spawn independent of framerate
                int customersCount = GameObject.FindGameObjectsWithTag("CustomerGroup").Length;
                if (customersCount < maxGroups && Random.Range(0f, 100f) < 100f/(1 + (customersCount*200)))
                {
                    SpawnCustomers();
                }
            }
        }

        /// <summary>
        /// Spawn a random group of customers.
        /// </summary>
        private void SpawnCustomers()
        {
            groupChance.Sort((obj1, obj2) =>
            {
                if (obj1.groupChance < obj2.groupChance)
                    return -1;
                if (obj1.groupChance == obj2.groupChance)
                    return 0;

                return 1;
            });

            float dice = Random.Range(0, 100);
            foreach (GroupChance group in groupChance)
            {
                if (dice > group.groupChance)
                    continue;

                GameObject groupObject = PhotonNetwork.Instantiate(this.customerGroupPrefab.name, transform.position, Quaternion.identity, 0);
                CustomerGroup customerGroup = groupObject.GetComponent<CustomerGroup>();
                for (int i = 0; i < group.groupSize; i++)
                {
                    //Get random Customer Prefab
                    int index = Random.Range(0, customerPrefabs.Count);

                    Vector3 spawn = transform.position;
                    float spawnRange = 0.5f;
                    spawn = new Vector3(spawn.x + Random.Range(-spawnRange, spawnRange), spawn.y, spawn.z + Random.Range(-spawnRange, spawnRange));

                    GameObject customerObj = PhotonNetwork.Instantiate(this.customerPrefabs[index].name, spawn, Quaternion.identity, 0);
                    Customer customer = customerObj.GetComponent<Customer>();
                    customer.SetGroup(customerGroup);
                }

                return;
            }
        }

    }
}
