using Assets.Game.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public interface IAction
    {
        void OnAdd(ActionManager manager);
        void OnRemove();
    }
}
