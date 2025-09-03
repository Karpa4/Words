using System;
using Input;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay
{
    public interface IClusterMover
    {
        void Initialize();
        void StartMove(Cluster cluster, RectTransform targetRect);
        Vector2 GetFirstLetterPosition();
        void StopMove();
        void Clear();
    }

    public class ClusterMover : MonoBehaviour, IClusterMover
    {
        [Inject] private IInputProvider _inputProvider;
        [Inject] private IUiClusterChanger _uiClusterChanger;
        [Inject] private IUiClusterCreator _uiClusterCreator;
        [Inject] private IGameObjectsHolder _objectsHolder;

        private UiClusterData _uiClusterData;
        private Vector2 _offset;
        private bool _isActive;
        
        public event Action MoveStartEvent;
        public event Action MoveFinishEvent;

        public void Initialize()
        {
            
            _uiClusterData = _uiClusterCreator.GetEmpty(_objectsHolder.GameRectTransform);
            var sizeFitter = _uiClusterData.ClusterObject.gameObject.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        
        public void StartMove(Cluster cluster, RectTransform targetRect)
        {
            var worldPosition = targetRect.TransformPoint(targetRect.rect.center);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_objectsHolder.CanvasRect, _inputProvider.PointerPosition, _objectsHolder.Canvas.worldCamera, out var pointerUiPosition);
            var localPosition = _uiClusterData.ClusterObject.parent.InverseTransformPoint(worldPosition);
            _uiClusterData.ClusterObject.localPosition = localPosition;
            _offset = pointerUiPosition - (Vector2)localPosition;
            _uiClusterChanger.Change(_uiClusterData, cluster);
            _uiClusterData.ClusterObject.gameObject.SetActive(true);
            _isActive = true;
            MoveStartEvent?.Invoke();
        }
        
        public Vector2 GetFirstLetterPosition()
        {
            return _uiClusterData.GetFirstLetterLocalPosition();
        }

        public void StopMove()
        {
            _isActive = false;
            _uiClusterData.ClusterObject.gameObject.SetActive(false);
            
            foreach (var text in _uiClusterData.Texts)
                text.gameObject.SetActive(false);
            
            MoveFinishEvent?.Invoke();
        }

        public void Clear()
        {
            Destroy(_uiClusterData.ClusterObject.gameObject);
            _uiClusterData = null;
        }

        private void Update()
        {
            if (!_isActive)
                return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_objectsHolder.CanvasRect, _inputProvider.PointerPosition, _objectsHolder.Canvas.worldCamera, out var localPoint);
            _uiClusterData.ClusterObject.localPosition = localPoint - _offset;
        }
    }
}
