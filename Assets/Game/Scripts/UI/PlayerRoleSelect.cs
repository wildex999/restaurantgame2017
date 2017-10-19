using Assets.Game.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoleSelect : MonoBehaviour {

    public void SelectRoleEmployee()
    {
        SelectRole(PlayerRoles.Employee);
    }

    public void SelectRoleChef()
    {
        SelectRole(PlayerRoles.Chef);
    }

    public void SelectRoleManager()
    {
        SelectRole(PlayerRoles.Manager);
    }

    public void SelectRole(PlayerRoles role)
    {
        GameManager.instance.localPlayerRole.Value = role;
        Destroy(gameObject);
    }
}
