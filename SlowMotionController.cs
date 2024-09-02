using UniRx;
using UnityEngine;

namespace SlowMotion
{
    public class SlowMotionController
    {
        private const float DecreaseSpeed = 0.2f;
        private const float RecoverySpeed = 0.1f;
    
        private readonly SlowMotionView _slowMotionView;
        private readonly float _percentage;
        private readonly float _recoveryTime;
        private readonly BoolReactiveProperty _isTimeSlowed = new BoolReactiveProperty(false);
        private readonly ReactiveProperty<float> _slowMotionPoints = new ReactiveProperty<float>(100f);
        private float _timeSinceLastActivation;
    
        public SlowMotionController(float percentage, float recoveryTime, SlowMotionView slowMotionView)
        {
            _percentage = percentage;
            _recoveryTime = recoveryTime;
            _slowMotionView = slowMotionView;
            _timeSinceLastActivation = 0f;
        }

        public void InitSlowMotion()
        {
            _isTimeSlowed.Where(x => x).
                Subscribe(_ => ActivateSlowMotion());
            _isTimeSlowed.Where(x => !x).
                Subscribe(_ => DeactivateSlowMotion());

            // updating UI
            Observable.EveryUpdate().
                Subscribe(_ => _slowMotionView.UpdateBar(_slowMotionPoints.Value));
        
            // Decrease slowMotionPoints each frame
            Observable.EveryUpdate()
                .Where(_ => _isTimeSlowed.Value && _slowMotionPoints.Value > 0)
                .Subscribe(_ =>
                {
                    _slowMotionPoints.Value = Mathf.Clamp(_slowMotionPoints.Value - DecreaseSpeed, 0, 100);
                    Debug.Log("Points left: " + _slowMotionPoints.Value);
                
                    if (_slowMotionPoints.Value <= 0)
                    {
                        _isTimeSlowed.Value = false;
                        _timeSinceLastActivation = 0f;
                    }
                });
        
            // Recover slowMotionPoints after 2 seconds 
            Observable.EveryUpdate()
                .Where(_ => !_isTimeSlowed.Value && _slowMotionPoints.Value < 100)
                .Subscribe(_ => {
                    _timeSinceLastActivation += Time.deltaTime;
                    if (_timeSinceLastActivation >= _recoveryTime) 
                    {
                        _slowMotionPoints.Value = Mathf.Clamp(_slowMotionPoints.Value + RecoverySpeed, 0, 100);
                        Debug.Log("Points restored: " + _slowMotionPoints.Value);
                    }
                });
        
            Time.timeScale = 1f;
        }
    
        public void ToggleTimeSlowed()
        {
            if (_slowMotionPoints.Value > 0) 
                _isTimeSlowed.Value = !_isTimeSlowed.Value;

            if (!_isTimeSlowed.Value)
                _timeSinceLastActivation = 0f;
        }
    
        private void ActivateSlowMotion() => Time.timeScale = Mathf.Lerp(1f, _percentage, 0.85f);

        private void DeactivateSlowMotion() => Time.timeScale = Mathf.Lerp(_percentage, 1f, 0.85f);
    }
}
