using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Game.Scripts
{
    public abstract class ActionState<T> : IActionState where T : IAction
    {
        public T action;
        public int id;

        protected int stateNone = -1;

        public abstract void Setup();
        public abstract void Update();
        public abstract void Cleanup();

        public void SetId(int id)
        {
            this.id = id;
        }

        public void SetAction(IAction action)
        {
            this.action = (T)action;
        }
    }
}
