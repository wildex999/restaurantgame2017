

using Assets.Game.Scripts.Tables;
using Assets.Game.Scripts.UI;
using System;

namespace Assets.Game.Scripts.Other.Actions
{
    /// <summary>
    /// Mark table as dirty
    /// </summary>
    public class ActionTableDirty : SyncedAction<ActionTableDirty> 
    {
        private GameStatusIcon icon;
        private Table table;

        private void Start()
        {
            table = GetComponent<Table>();
            SwitchState(AddState(new StateDirty()));
        }

        private void OnMouseUpAsButton()
        {
            //Task Employee with delivering trash
            GameManager.instance.localPlayer.ActionCleanTrash(table);
        }

        [PunRPC]
        private void CleanTrash(int playerViewId, PhotonMessageInfo info)
        {
            PhotonView.Find(playerViewId).RPC("GetTrash", info.sender);
            End();
        }

        private class StateDirty : ActionState<ActionTableDirty>
        {
            public override void Setup()
            {
                action.icon = Instantiate(StatusIconLibrary.Get().iconTrash, StatusIconLibrary.Get().mainCanvas.transform);
                action.icon.Follow(action.gameObject);
                action.icon.StopOverlap = true;

                action.gameObject.AddComponent<OutlineHover>();
            }

            public override void Update() {
            }

            public override void Cleanup() {
                Destroy(action.GetComponent<OutlineHover>());
                Destroy(action.icon.gameObject);
            }
        }
    }
}
