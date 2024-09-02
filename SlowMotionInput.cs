using UniRx;
using UnityEngine;

namespace SlowMotion
{
    public class SlowMotionInput : MonoBehaviour
    {
        [SerializeField] private SlowMotionView _slowMotionView;
        [SerializeField, Range(0f, 0.2f)] private float slowMotionPercentage;
        [SerializeField] private float recoveryTime;
        private SlowMotionController _slowMotionController;

        private void Awake()
        {
            _slowMotionController = new SlowMotionController(slowMotionPercentage, recoveryTime, _slowMotionView);
            _slowMotionController.InitSlowMotion();
        }
        
        private void Start()
        {
            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.F))
                .Subscribe(_ => {
                    _slowMotionController.ToggleTimeSlowed();
                });
        }
    }
}