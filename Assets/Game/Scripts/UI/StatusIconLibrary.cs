using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.Game.Scripts.UI
{
    //TODO: Rename to UI library
    public class StatusIconLibrary : MonoBehaviour
    {
        static StatusIconLibrary instance;

        public GameStatusIcon waitingTable;
        public GameObject selectTablePanel;

        public static StatusIconLibrary Get()
        {
            if(instance == null)
                instance = GameObject.FindObjectOfType<StatusIconLibrary>();
            return instance;
        }
    }
}
