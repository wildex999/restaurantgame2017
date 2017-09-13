using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.Game.Scripts.UI
{
    //TODO: Rename to UI library
    public class StatusIconLibrary : MonoBehaviour
    {
        static StatusIconLibrary instance;

        //Prefabs
        public GameStatusIcon waitingTable;

        //Singletons/Static
        public TableSelection TableSelectionUi;

        public static StatusIconLibrary Get()
        {
            if(instance == null)
                instance = GameObject.FindObjectOfType<StatusIconLibrary>();
            return instance;
        }
    }
}
