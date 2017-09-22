using System.Collections.Generic;

namespace Assets.Game.Scripts
{
    public abstract class SyncedAction<T> : Photon.PunBehaviour, IPunObservable, IAction
    {
        public ActionManager manager;

        List<IActionState> states;

        IActionState currentState;
        protected int currentStateId;

        protected int stateNone = -1;

        public SyncedAction()
        {
            states = new List<IActionState>();
            currentStateId = stateNone;
        }

        public void OnAdd(ActionManager manager)
        {
            this.manager = manager;
        }

        public void OnRemove() { }

        protected virtual void Update()
        {
            if (currentState != null)
                currentState.Update();
        }

        protected int AddState(IActionState state)
        {
            int id = states.Count;
            states.Add(state);
            state.SetId(id);

            return id;
        }

        protected void SwitchState(int id)
        {
            IActionState newState = null;
            if(id >= 0 && id < states.Count)
                newState = states[id];
            currentStateId = id;

            if (newState == currentState)
                return;

            if (currentState != null)
                currentState.Cleanup();

            currentState = newState;

            if (currentState != null)
            {
                currentState.SetAction(this);
                currentState.Setup();
            }
        }

        //Sync the current state
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.isWriting)
            {
                if (currentState == null)
                    stream.SendNext(-1);
                else
                    stream.SendNext(currentStateId);
            } else
            {
                int id = (int)stream.ReceiveNext();
                SwitchState(id);
            }
        }
    }
}
