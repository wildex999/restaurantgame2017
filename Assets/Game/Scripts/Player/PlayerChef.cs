using Assets.Game.Scripts.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Game.Scripts.Player
{
    public class PlayerChef : Photon.PunBehaviour, IPlayerRole, IPunObservable
    {
        ActionManager actionManager;
        GameObject model;

        void Start()
        {
            actionManager = GetComponent<ActionManager>();
            Controller = GetComponent<PlayerController>();

            model = Instantiate(PlayerData.Instance.chefModel, transform);
        }

        private void OnDestroy()
        {
            Destroy(model);
        }

        public PlayerRoles GetRole()
        {
            return PlayerRoles.Chef;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
        }

        public PlayerController Controller { get; set; }
    }
}
