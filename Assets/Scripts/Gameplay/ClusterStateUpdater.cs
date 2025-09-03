using System.Collections.Generic;
using Configuration;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public interface IClusterStateUpdater
    {
        void Start();
        void Stop();
        void TrySetState(Cluster cluster, ClusterState clusterState);
        ClusterState GetState(Cluster cluster);
        void SetNewClusters(IEnumerable<IUiClusterStateData> values);
        void ResetAll();
    }

    public class ClusterStateUpdater : IClusterStateUpdater
    {
        [Inject] private IGameFieldChecker _gameFieldChecker;
        [Inject] private IClusterColorConfig _clusterColorConfig;

        private readonly Dictionary<Cluster, IUiClusterStateData> _clusterStateDataByCluster = new();
        
        public void Start()
        {
            _gameFieldChecker.ClusterStateChangedEvent += OnClusterStateChanged;
        }

        private void OnClusterStateChanged(Cluster cluster, bool isCorrectPlace)
        {
            TrySetState(cluster, isCorrectPlace ? ClusterState.Correct : ClusterState.Incorrect);
        }

        public void Stop()
        {
            _clusterStateDataByCluster.Clear();
            _gameFieldChecker.ClusterStateChangedEvent -= OnClusterStateChanged;
        }

        public void TrySetState(Cluster cluster, ClusterState newState)
        {
            if (!_clusterStateDataByCluster.TryGetValue(cluster, out var uiClusterData))
            {
                Debug.LogError($"{GetType().Name} cluster {cluster.Text} not found");
                return;
            }

            if (uiClusterData.ClusterState == newState)
                return;
            
            SetState(uiClusterData, newState);
        }

        public ClusterState GetState(Cluster cluster)
        {
            return _clusterStateDataByCluster.TryGetValue(cluster, out var uiClusterData) ? uiClusterData.ClusterState : ClusterState.Default;
        }

        private void SetState(IUiClusterStateData uiClusterData, ClusterState newState)
        {
            uiClusterData.SetClusterState(newState);
            
            switch (newState)
            {
                case ClusterState.Default:
                    uiClusterData.BackImage.color = _clusterColorConfig.DefaultColor;
                    break;
                case ClusterState.Correct:
                    uiClusterData.BackImage.color = _clusterColorConfig.CorrectColor;
                    break;
                case ClusterState.Incorrect:
                    uiClusterData.BackImage.color = _clusterColorConfig.IncorrectColor;
                    break;
                case ClusterState.Drag:
                    uiClusterData.BackImage.color = _clusterColorConfig.DragColor;
                    break;
                default:
                    Debug.LogError($"{GetType().Name} unknown state {newState}");
                    break;
            }
        }

        public void SetNewClusters(IEnumerable<IUiClusterStateData> clusterStateDatas)
        {
            _clusterStateDataByCluster.Clear();
            
            foreach (var clusterStateData in clusterStateDatas)
                _clusterStateDataByCluster[clusterStateData.Cluster] = clusterStateData;
        }

        public void ResetAll()
        {
            if (_clusterStateDataByCluster.Count == 0)
                return;
            
            foreach (var clusterStateData in _clusterStateDataByCluster.Values)
                SetState(clusterStateData, ClusterState.Default);
        }
    }
    
    public enum ClusterState
    {
        Default,
        Correct,
        Incorrect,
        Drag
    }
}