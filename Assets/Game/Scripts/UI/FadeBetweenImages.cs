using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.UI
{
    /// <summary>
    /// Fade image 1 to show image 2 behind
    /// </summary>
    public class FadeBetweenImages : MonoBehaviour
    {
        public RawImage image1;
        public RawImage image2;

        //Set how much to fade image 1(0 is no fade, 1 is fully transparent)
        public void SetFade(float fade)
        {
            image1.color = new Color(image1.color.r, image1.color.g, image1.color.b, 1f-fade);
        }
    }
}
