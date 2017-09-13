using Assets.Game.Scripts.Customers;
using Assets.Game.Scripts.Customers.Task;
using Assets.Game.Scripts.UI;
using Assets.Game.Scripts.UI.TableSelect;
using UnityEngine;

namespace Assets.Game.Scripts.Player.Actions
{
    /// <summary>
    /// Action performed by the Employee.
    /// 1. Go to the Customer Group that needs to be seated
    /// 2. Open Table Selection screen
    /// 3. Lead Customer to selected table (Maybe future?)
    /// </summary>

    [RequireComponent(typeof(PlayerController))]
    public class SeatCustomers : MonoBehaviour
    {
        [Tooltip("How close to a Customer Group to be before opening the Table Selection")]
        public float distanceToOpenTableSelection = 1f;

        PlayerController controller;
        CustomerGroup group;
        State state;

        enum State
        {
            GoToCustomer,
            SelectTable,
            //SeatCustomer
        }

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (group == null)
                return;

            GetTable customerTask = group.GetComponent<GetTable>();
            if(customerTask == null || !customerTask.AwaitingTable())
            {
                OnClose();
                return;
            }

            switch(state)
            {
                case State.GoToCustomer:
                    if (Vector3.Distance(transform.position, group.transform.position) < distanceToOpenTableSelection)
                        SwitchState(State.SelectTable);
                    break;
            }
        }

        private void OnClose()
        {
            controller.allowUserInput = true;
            StatusIconLibrary.Get().TableSelectionUi.gameObject.SetActive(false);
            Destroy(this);
        }

        private void SwitchState(State newState)
        {
            State oldState = state;
            state = newState;

            switch(oldState)
            {
                case State.GoToCustomer:
                    controller.StopMoving();
                    break;
            }

            switch(newState)
            {
                case State.GoToCustomer:
                    controller.SetDestination(group.transform.position);
                    break;
                case State.SelectTable:
                    //Open the Table selection screen
                    controller.allowUserInput = false;
                    TableSelection tableSelection = StatusIconLibrary.Get().TableSelectionUi;
                    tableSelection.SetCloseCallback(OnClose);
                    tableSelection.SetGroup(group);
                    tableSelection.gameObject.SetActive(true);
                    break;
            }
        }

        public void SetCustomerGroup(CustomerGroup group)
        {
            this.group = group;
            SwitchState(State.GoToCustomer);
        }
    }
}
