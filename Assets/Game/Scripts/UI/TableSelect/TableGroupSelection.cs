using Assets.Game.Scripts.Customers;
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

        TableSelection tableSelectionUi;
        List<GameObject> chairs;
        GameObject table;

        bool hover = false;

        private void Start()
        {
            tableSelectionUi = GetComponentInParent<TableSelection>();

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
            if (CountFreeChairs() >= tableSelectionUi.GetCustomerCount())
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

        public void OnClick()
        {
            CustomerGroup customerGroup = tableSelectionUi.GetGroup();
            if(customerGroup == null)
            {
                Debug.LogError("Customer Group is null when selecting table");
                return;
            }

            //Send customers to their new table(On Master client)
            int tableViewId = boundGroup.GetComponent<PhotonView>().photonView.viewID;
            customerGroup.GetComponent<PhotonView>().RPC("SetTable", PhotonTargets.MasterClient, tableViewId);

            //End Table Selection for player
            tableSelectionUi.Close();
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
            int toSeat = tableSelectionUi.GetCustomerCount();
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
    }
}
