using Assets.Game.Scripts.UI;
using Assets.Game.Scripts.UI.TableSelect;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Game.Scripts.Customers.Task
{
    /// <summary>
    /// This action has the goal of getting the group of customers to a table.
    /// 1. Go to the door.
    /// 2. Wait to be assigned a table.
    /// 3. When assigned the table, go to it and sit down(Place customers at seats)
    /// </summary>
    [RequireComponent(typeof(CustomerGroup))]
    public class GetTable : Photon.PunBehaviour, IPunObservable
    {

        enum State
        {
            MoveToDoor,
            WaitForTable,
            MoveToTable
        }
        State state;

        CustomerGroup group;
        CustomerQueue queue;
        NavMeshAgent agent;

        GameObject mainCanvas;
        GameStatusIcon currentIcon;

        private void Start()
        {
            state = State.MoveToDoor;
            agent = GetComponent<NavMeshAgent>();
            queue = GameObject.FindObjectOfType<CustomerQueue>();
            group = GetComponent<CustomerGroup>();

            mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        }

        private void Update()
        {
            //Update state and decisions for the Master Client
            if (photonView.isMine)
                UpdateMaster();

            //Updater the visual for all clients

        }

        private void UpdateMaster()
        {
            switch (state)
            {
                case State.MoveToDoor:
                    Vector3 point = queue.NextQueuePosition();

                    if (agent.destination != point)
                        agent.SetDestination(point);

                    //Enter queue if near
                    if (Vector3.Distance(transform.position, point) < 2f)
                    {
                        queue.EnterQueue(group);
                        SwitchState(State.WaitForTable);
                    }

                    break;

                case State.WaitForTable:

                    break;
            }
        }

        private void SwitchState(State newState)
        {
            State oldState = state;
            state = newState;

            //Cleanup the old state
            switch(oldState)
            {
                case State.WaitForTable:
                    Destroy(currentIcon.gameObject);
                    break;
            }

            //Setup the new state
            switch(state)
            {
                case State.WaitForTable:
                    currentIcon = Instantiate(StatusIconLibrary.Get().waitingTable, mainCanvas.transform);
                    currentIcon.Follow(this.gameObject);
                    break;
            }
        }

        private void OnMouseUpAsButton()
        {
            if (state != State.WaitForTable)
                return;

            //TODO: Move Employee towards group.

            //Open the Table selection screen
            //TODO: Disable other input while selecting table
            TableGroupSelection.customerCount = group.GetCustomerCount();
            GameObject tableSelection = StatusIconLibrary.Get().TableSelectionUi;
            tableSelection.SetActive(true);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.isWriting)
            {
                stream.SendNext(state);
            } else
            {
                State newState = (State)stream.ReceiveNext();
                if (newState != state)
                    SwitchState(newState);
            }
        }
    }
}
