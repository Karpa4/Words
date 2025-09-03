using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScrollActivator : MonoBehaviour
    {
        [SerializeField] private ClusterMover _clusterMover;
        [SerializeField] private ScrollRect _scrollRect;

        private void OnEnable()
        {
            _clusterMover.MoveFinishEvent += OnMoveFinished;
            _clusterMover.MoveStartEvent += OnMoveStarted;
        }

        private void OnMoveStarted()
        {
            _scrollRect.enabled = false;
        }

        private void OnMoveFinished()
        {
            _scrollRect.enabled = true;
        }

        private void OnDisable()
        {
            _clusterMover.MoveFinishEvent -= OnMoveFinished;
            _clusterMover.MoveStartEvent -= OnMoveStarted;
        }
    }
}