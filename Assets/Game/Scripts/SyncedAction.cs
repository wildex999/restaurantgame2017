using System;
using System.Collections.Generic;

namespace Assets.Game.Scripts
{
    public abstract class SyncedAction<T> : Photon.PunBehaviour, IPunObservable, IAction
    {
        public ActionManager manager;
        public bool sync = true;

        List<IActionState> states;

        IActionState currentState;
        protected int currentStateId;

        protected int stateNone = -1; //Special state doing nothing. This is the state which is set before removing(To allow cleanup of any existing state)
        protected int stateEnd = -2; //Special state which will End the action.

        public SyncedAction()
        {
            states = new List<IActionState>();
            currentStateId = stateNone;
        }

        public virtual void OnAdd(ActionManager manager)
        {
            this.manager = manager;
        }

        public virtual void OnRemove() {
            SwitchState(stateNone);
        }

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

        public void SwitchState(int id)
        {
            if(id == stateEnd)
            {
                End();
                return;
            }

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

        /// <summary>
        /// End this action, removing it through the manager.
        /// </summary>
        public virtual void End()
        {
            manager.RemoveAction(this);
        }

        //Sync the current state
        public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
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

        public virtual bool AllowNewAction(Type action)
        {
            return false;
        }
        public virtual void OnNewAction(IAction action) { }
    }
}
