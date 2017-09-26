using Assets.Game.Scripts.Other.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Game.Scripts.Tables
{
    public class Table : Photon.PunBehaviour
    {
        ActionManager actionManager;

        private void Awake()
        {
            actionManager = GetComponent<ActionManager>();
        }

        public void MarkTableDirty()
        {
            actionManager.AddAction<ActionTableDirty>();
        }
    }
}
