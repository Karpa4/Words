using UnityEngine;

namespace Gameplay
{
    public interface IGameObjectsHolder
    {
        RectTransform ClustersScrollContentParent { get; }
        RectTransform ClustersScrollParent { get; }
        RectTransform GameRectTransform { get; }
        Transform ClustersFieldParent { get; }
        Transform EmptyCellParent { get; }
        RectTransform CanvasRect { get; }
        Canvas Canvas { get; }
    }

    public class GameObjectsHolder : MonoBehaviour, IGameObjectsHolder
    {
        [SerializeField] private RectTransform _clustersScrollParent;
        [SerializeField] private RectTransform _clustersScrollContentParent;
        [SerializeField] private RectTransform _gameRectTransform;
        [SerializeField] private Transform _clustersFieldParent;
        [SerializeField] private Transform _emptyCellParent;
        [SerializeField] private RectTransform _canvasRect;
        [SerializeField] private Canvas _canvas;

        public RectTransform ClustersScrollContentParent => _clustersScrollContentParent;
        public RectTransform ClustersScrollParent => _clustersScrollParent;
        public RectTransform GameRectTransform => _gameRectTransform;
        public Transform ClustersFieldParent => _clustersFieldParent;
        public Transform EmptyCellParent => _emptyCellParent;
        public RectTransform CanvasRect => _canvasRect;
        public Canvas Canvas => _canvas;
    }
}