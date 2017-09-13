using Assets.Game.Scripts.Customers;
using Assets.Game.Scripts.UI.TableSelect;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.UI
{
    public class TableSelection : MonoBehaviour
    {
        private CustomerGroup group;
        private Action closeCallback;

        private void Update()
        {
            //Have it as last so it is always in front
            transform.SetAsLastSibling();
        }

        public void SetCloseCallback(Action callback)
        {
            closeCallback = callback;
        }

        public void SetGroup(CustomerGroup group)
        {
            this.group = group;
        }

        public CustomerGroup GetGroup()
        {
            return group;
        }

        public int GetCustomerCount()
        {
            if (group == null)
                return 0;

            return group.GetCustomerCount();
        }

        public void Close()
        {
            gameObject.SetActive(false);
            if (closeCallback != null)
                closeCallback();
        }
    }
}
