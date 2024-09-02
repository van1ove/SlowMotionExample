using UnityEngine;
using UnityEngine.UI;

namespace SlowMotion
{
    public class SlowMotionView : MonoBehaviour
    {
        [SerializeField] private Image bar;
        private const float MaxValue = 100f;

        public void UpdateBar(float value) => bar.fillAmount = value / MaxValue;
    }
}