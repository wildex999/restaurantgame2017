using Assets.Game.Scripts.Tables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.UI.TableSelect
{
    public class TableGroupSelection : MonoBehaviour
    {
        [Tooltip("The Table Group GameObject in the Restaurant for which this represents")]
        public TableGroup boundGroup;

        public Color colorChairTaken = Color.red;
        public Color colorChairFree = Color.green;
        public Color colorChairHighlight = Color.white;

        List<GameObject> chairs;
        GameObject table;

        public static int customerCount = 0; //TODO: Move away from singleton static, and instead use the parent object
        bool hover = false;

        private void Start()
        {
            //Get chairs
            chairs = new List<GameObject>();
            foreach(Transform child in transform)
            {
                if(child.CompareTag("Chair"))
                    chairs.Add(child.gameObject);
            }

            UpdateChairColors();
        }

        private void Update()
        {
            //TODO: Only update when we know it has changed(Network update etc.)
            UpdateChairColors();
        }

        public void OnPointerEnter()
        {
            if (CountFreeChairs() >= customerCount)
            {
                hover = true;
                UpdateChairColors();
            }
        }

        public void OnPointerExit()
        {
            hover = false;
            UpdateChairColors();
        }

        private int CountFreeChairs()
        {
            int freeChairs = 0;
            foreach(Chair chair in boundGroup.GetChairs())
            {
                if (chair.seatedCustomer == null)
                    freeChairs++;
            }

            return freeChairs;
        }

        private void UpdateChairColors()
        {
            List<Chair> floorChairs = boundGroup.GetChairs();
            int chairIndex = 0;
            int toSeat = customerCount;
            foreach(GameObject chair in chairs)
            {
                Chair floorChair = floorChairs[chairIndex++];
                Image chairImage = chair.GetComponent<Image>();
                if (floorChair.seatedCustomer != null)
                {
                    chairImage.color = colorChairTaken;
                    continue;
                }

                if(toSeat > 0 && hover)
                {
                    chairImage.color = colorChairHighlight;
                    toSeat--;
                    continue;
                }

                chairImage.color = colorChairFree;
            }
        }

        /// <summary>
        /// Set the number of customers looking for a table, which is how many chairs to highlight.
        /// </summary>
        /// <param name="customers"></param>
        public void SetCustomerCount(int customers)
        {
            customerCount = customers;
        }
    }
}
