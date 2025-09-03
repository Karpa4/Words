using System.Collections.Generic;
using Input;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Gameplay
{
    public interface IClusterManager
    {
        void SetNewClusters(ICollection<Cluster> clusters);
        void Clear();
        void ClearWithDestroy();
        bool TryGetCluster(int instanceID, out Cluster cluster);
        void MoveClusterToField(int instanceID, Vector2 firstTextPosition);
        void MoveClusterToFreeZone(int instanceID);
        bool TryGetUiClusterData(int instanceID, out IUiClusterData uiClusterData);
    }

    public class ClusterManager : IClusterManager
    {
        [Inject] private IInputProvider _inputProvider;
        [Inject] private IUiClusterCreator _uiClusterCreator;
        [Inject] private IUiClusterChanger _uiClusterChanger;
        [Inject] private IGameObjectsHolder _objectsHolder;
        [Inject] private IClusterStateUpdater _clusterStateUpdater;
        
        private readonly Dictionary<int, UiClusterData> _uiClustersById = new();
        private readonly Queue<UiClusterData> _pool = new();
        
        public bool TryGetCluster(int instanceID, out Cluster cluster)
        {
            TryGetData(instanceID, out var clusterData);
            cluster = clusterData?.Cluster;
            return cluster != null;
        }
        
        public void SetNewClusters(ICollection<Cluster> clusters)
        {
            Clear();
            
            foreach (var cluster in clusters)
            {
                var uiCluster = GetUiClusterData(cluster);
                _uiClustersById[uiCluster.BackImage.gameObject.GetInstanceID()] = uiCluster;
            }

            _clusterStateUpdater.SetNewClusters(_uiClustersById.Values);
        }

        public void Clear()
        {
            _clusterStateUpdater.ResetAll();
            
            if (_uiClustersById.Count == 0)
                return;

            foreach (var uiCluster in _uiClustersById.Values)
            {
                uiCluster.ClusterObject.gameObject.SetActive(false);
                uiCluster.SetCluster(null);
                uiCluster.InField = false;
                _pool.Enqueue(uiCluster);
            }
            
            _uiClustersById.Clear();
        }
        
        public void ClearWithDestroy()
        {
            Clear();
            
            if (_pool.Count == 0)
                return;

            foreach (var uiClusterData in _pool)
                Object.Destroy(uiClusterData.ClusterObject.gameObject);
            
            _pool.Clear();
        }

        public bool TryGetUiClusterData(int instanceID, out IUiClusterData uiClusterData)
        {
            TryGetData(instanceID, out var clusterData);
            uiClusterData = clusterData;
            return uiClusterData != null;
        }
        
        public void MoveClusterToField(int instanceID, Vector2 firstTextPosition)
        {
            if (!TryGetData(instanceID, out var uiClusterData))
                return;

            if (!uiClusterData.InField)
                uiClusterData.ClusterObject.SetParent(_objectsHolder.EmptyCellParent);

            uiClusterData.InField = true;
            var parentPosition = firstTextPosition - (Vector2)uiClusterData.Texts[0].rectTransform.localPosition;
            uiClusterData.ClusterObject.localPosition = parentPosition;
        }

        public void MoveClusterToFreeZone(int instanceID)
        {
            if (!TryGetData(instanceID, out var uiClusterData))
                return;
            
            _clusterStateUpdater.TrySetState(uiClusterData.Cluster, ClusterState.Default);

            if (!uiClusterData.InField)
                return;

            uiClusterData.InField = false;
            var index = GetScrollIndex();
            uiClusterData.ClusterObject.SetParent(_objectsHolder.ClustersScrollContentParent);
            uiClusterData.ClusterObject.SetSiblingIndex(index);
        }

        private int GetScrollIndex()
        {
            var childCount = _objectsHolder.ClustersScrollContentParent.childCount;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_objectsHolder.ClustersScrollContentParent, _inputProvider.PointerPosition, 
                _objectsHolder.Canvas.worldCamera,out var localCursor);
            var totalContentWidth = _objectsHolder.ClustersScrollContentParent.rect.width;
            var normalizedPosition = Mathf.InverseLerp(0, totalContentWidth, localCursor.x);
            var estimatedIndex = Mathf.RoundToInt(normalizedPosition * childCount);
            return Mathf.Clamp(estimatedIndex, 0, childCount);
        }

        private bool TryGetData(int instanceID, out UiClusterData clusterData)
        {
            if (!_uiClustersById.TryGetValue(instanceID, out clusterData))
                Debug.LogError($"{GetType().Name} cluster with id {instanceID} not found");
            
            return clusterData != null;
        }

        private UiClusterData GetUiClusterData(Cluster cluster)
        {
            if (!_pool.TryDequeue(out var uiClusterData))
                return _uiClusterCreator.GetNew(cluster, _objectsHolder.ClustersScrollContentParent);
            
            uiClusterData.ClusterObject.SetParent(_objectsHolder.ClustersScrollContentParent);
            _uiClusterChanger.Change(uiClusterData, cluster);
            uiClusterData.ClusterObject.gameObject.SetActive(true);
            return uiClusterData;
        }
    }
}