using Assets.Game.Scripts.UI;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Game.Scripts
{
    public class OutlineHover : MonoBehaviour
    {
        public float iconOutlineSize = 2f;
        public Color iconOutlineColor = Color.red;

        List<MonoBehaviour> outlines;

        private void Start()
        {
            outlines = new List<MonoBehaviour>();
        }

        private void OnMouseOver()
        {
            //Outline Customers
            foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                if (renderer.gameObject.GetComponent<cakeslice.Outline>() == null)
                    outlines.Add(renderer.gameObject.AddComponent<cakeslice.Outline>());
            }

            //Outline icons
            foreach (GameStatusIcon icon in FindObjectsOfType<GameStatusIcon>())
            {
                if (icon.follow.target == gameObject)
                {
                    //Get the first icon(Which is the one behind, and the one NOT fading away)
                    CanvasRenderer renderer = icon.GetComponentInChildren<CanvasRenderer>();
                    if (renderer != null && renderer.gameObject.GetComponent<UnityEngine.UI.Outline>() == null)
                    {
                        UnityEngine.UI.Outline outline = renderer.gameObject.AddComponent<UnityEngine.UI.Outline>();
                        outline.effectColor = iconOutlineColor;
                        outline.effectDistance = new Vector2(iconOutlineSize, iconOutlineSize);
                        outlines.Add(outline);
                    }
                }
            }
        }

        private void OnMouseExit()
        {
            //Remove Outline components
            foreach (MonoBehaviour outline in outlines)
                Destroy(outline);
        }
    }
}
