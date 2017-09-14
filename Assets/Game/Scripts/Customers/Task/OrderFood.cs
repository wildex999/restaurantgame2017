using Assets.Game.Scripts.UI;
using UnityEngine;

namespace Assets.Game.Scripts.Customers.Task
{
    [RequireComponent(typeof(CustomerGroup))]
    public class OrderFood : Photon.PunBehaviour, IPunObservable
    {
        enum State
        {
            ReadingMenu,
            WaitingOrder,
            WaitingFood
        }

        State state;
        GameStatusIcon currentIcon;
        CustomerGroup group;

        [Tooltip("Number between 0 and 1 defining how fast they will read the menu. 0 = never, 1 = instantly")]
        public float menuReadingSpeed = 0.3f;

        private void Start()
        {
            group = GetComponent<CustomerGroup>();
            state = State.ReadingMenu;
        }

        private void Update()
        {
            //Update master
            if (photonView.isMine)
                UpdateMaster();

            //Update everyone
            switch (state)
            {
                case State.WaitingOrder:
                    //Update Icon to indicate waiting time
                    currentIcon.SetFade(1f - (group.waiting / 100f));
                    break;
            }

        }

        private void UpdateMaster()
        {
            switch (state)
            {
                case State.ReadingMenu:
                    if (Random.Range(0f, 1f) < menuReadingSpeed)
                    {
                        SwitchState(State.WaitingOrder);
                        return;
                    }
                    break;

                case State.WaitingOrder:
                    //Wait
                    group.waiting -= (100f / group.patience) * Time.deltaTime;
                    break;
            }
        }

        private void SwitchState(State newState)
        {
            State oldState = state;
            state = newState;

            switch(oldState)
            {

            }

            switch(newState)
            {
                case State.WaitingOrder:
                    currentIcon = Instantiate(StatusIconLibrary.Get().iconMenu, StatusIconLibrary.Get().mainCanvas.transform);
                    currentIcon.Follow(gameObject);
                    break;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.isWriting)
            {
                stream.SendNext(state);
            } else
            {
                State newState = (State)stream.ReceiveNext();
                SwitchState(newState);
            }
        }
    }
}
