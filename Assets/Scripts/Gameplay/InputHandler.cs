using System.Collections.Generic;
using Input;
using UnityEngine.EventSystems;
using Zenject;

namespace Gameplay
{
    public interface IInputHandler
    {
        void Start();
        void Stop();
    }

    public class InputHandler : IInputHandler
    {
        [Inject] private IInputProvider _inputProvider;
        [Inject] private IMainGameProcessor _mainGameProcessor;

        private readonly List<RaycastResult> _raycastResults = new();
        private PointerEventData _pointerEventData;
        
        public void Start()
        {
            _pointerEventData ??= new PointerEventData(EventSystem.current);
            _inputProvider.PointerDownEvent += OnPointerDown;
            _inputProvider.PointerUpEvent += OnPointerUp;
        }

        public void Stop()
        {
            _inputProvider.PointerDownEvent -= OnPointerDown;
            _inputProvider.PointerUpEvent -= OnPointerUp;
        }

        private void OnPointerDown()
        {
            if (_mainGameProcessor.IsBusy)
                return;
            
            _pointerEventData.position = _inputProvider.PointerPosition;
            EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);
            
            foreach (var raycastResult in _raycastResults)
            {
                if (raycastResult.gameObject == null)
                    continue;
                
                if (!raycastResult.gameObject.CompareTag(GameConstants.CLUSTER_TAG))
                    continue;
                
                if (_mainGameProcessor.TryStart(raycastResult.gameObject.GetInstanceID()))
                    break;
            }
            
            _raycastResults.Clear();
        }
        
        private void OnPointerUp()
        {
            _mainGameProcessor.Finish();
        }
    }
}