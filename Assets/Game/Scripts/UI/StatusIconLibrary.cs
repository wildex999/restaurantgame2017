using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.Game.Scripts.UI
{
    //TODO: Rename to UI library
    public class StatusIconLibrary : MonoBehaviour
    {
        static StatusIconLibrary instance;

        //Prefabs
        public GameStatusIcon iconTable;
        public GameStatusIcon iconMenu;

        //Instances
        public TableSelection TableSelectionUi;
        public GameObject mainCanvas;

        public static StatusIconLibrary Get()
        {
            if(instance == null)
                instance = GameObject.FindObjectOfType<StatusIconLibrary>();
            return instance;
        }
    }
}
