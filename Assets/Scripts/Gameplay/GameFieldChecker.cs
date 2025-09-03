using System;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public interface IGameFieldChecker
    {
        event Action<Cluster, bool> ClusterStateChangedEvent; 
        
        void Start();
        void Clear();
        bool CheckFieldForFinish();
        string[] GetWordsOrderedByTime();

        void Remove(ClusterPosition clusterPosition);
        bool MoveCluster(ClusterPosition clusterPosition, Cluster cluster, ClusterPosition previousPosition);
    }

    public class GameFieldChecker : IGameFieldChecker
    {
        [Inject] private IResultWordsContainer _resultWordsContainer;
        
        private RowCluster[] _rowClusters;
        private float[] _wordCompleteTimes;

        public event Action<Cluster, bool> ClusterStateChangedEvent;

        public void Start()
        {
            _wordCompleteTimes = new float[_resultWordsContainer.WordsCount];
            _rowClusters = new RowCluster[_resultWordsContainer.WordsCount];

            for (var i = 0; i < _rowClusters.Length; i++)
                _rowClusters[i] = new RowCluster(_resultWordsContainer.GetWordLength(i));
        }

        public void Clear()
        {
            _wordCompleteTimes = null;
            _rowClusters = null;
        }
        
        public void Remove(ClusterPosition clusterPosition)
        {
            if (clusterPosition.IsDefault)
                return;
            
            if (!CheckIndexes(clusterPosition.WordIndex, clusterPosition.StartCellIndex, 0))
                return;

            for (var i = 0; i < _rowClusters[clusterPosition.WordIndex].ClusterInfos.Count; i++)
            {
                if (_rowClusters[clusterPosition.WordIndex].ClusterInfos[i].StartCellIndex != clusterPosition.StartCellIndex)
                    continue;
                
                _rowClusters[clusterPosition.WordIndex].ClusterInfos.RemoveAt(i);
                break;
            }
        }
        
        private bool IsFree(ClusterPosition clusterPosition, Cluster cluster)
        {
            if (!CheckIndexes(clusterPosition.WordIndex, clusterPosition.StartCellIndex, cluster.Text.Length))
                return false;

            var lastCellIndex = clusterPosition.StartCellIndex + cluster.Text.Length - 1;
            
            foreach (var clusterInfo in _rowClusters[clusterPosition.WordIndex].ClusterInfos)
            {
                if (clusterPosition.StartCellIndex <= clusterInfo.LastCellIndex && lastCellIndex >= clusterInfo.StartCellIndex && clusterInfo.Cluster != cluster)
                    return false;
            }
            
            return true;
        }
        
        public bool MoveCluster(ClusterPosition clusterPosition, Cluster cluster, ClusterPosition previousPosition)
        {
            if (!IsFree(clusterPosition, cluster))
                return false;

            var isPreviousPositionDefault = previousPosition.IsDefault;
            
            if (!isPreviousPositionDefault)
                Remove(previousPosition);

            Add(clusterPosition, cluster);

            if (!isPreviousPositionDefault && previousPosition.WordIndex != clusterPosition.WordIndex)
                ChangeClustersInRow(_rowClusters[previousPosition.WordIndex]);

            ChangeClustersInRow(_rowClusters[clusterPosition.WordIndex]);
            return true;
        }
        
        private void Add(ClusterPosition clusterPosition, Cluster cluster)
        {
            if (!CheckIndexes(clusterPosition.WordIndex, clusterPosition.StartCellIndex, cluster.Text.Length))
                return;

            var index = 0;

            for (var i = 0; i < _rowClusters[clusterPosition.WordIndex].ClusterInfos.Count; i++)
            {
                if (clusterPosition.StartCellIndex >= _rowClusters[clusterPosition.WordIndex].ClusterInfos[i].StartCellIndex)
                {
                    index++;
                    continue;
                }
                
                index = i;
                break;
            }

            _rowClusters[clusterPosition.WordIndex].ClusterInfos.Insert(index, new ClusterInfo(cluster, clusterPosition.StartCellIndex));
        }

        private void ChangeClustersInRow(RowCluster rowCluster)
        {
            if (rowCluster.ClusterInfos.Count == 0)
            {
                rowCluster.ResultWordIndex = -1;
                return;
            }
            
            var isCorrect = _resultWordsContainer.TryGetCorrectWordIndex(rowCluster.ClusterInfos, rowCluster.WordLength, out var wordIndex);
            
            if (!isCorrect && rowCluster.ClusterInfos.Count == 1)
            {
                rowCluster.ResultWordIndex = -1;
                SetIsCorrectPlace(rowCluster, false);
                return;
            }
            
            if (!isCorrect && rowCluster.ResultWordIndex != -1)
            {
                foreach (var clusterInfo in rowCluster.ClusterInfos)
                {
                    var isClusterInCorrectPlace = _resultWordsContainer.IsClusterInCorrectPlace(rowCluster.ResultWordIndex, clusterInfo.StartCellIndex, clusterInfo.Cluster, rowCluster.WordLength);
                    SetIsCorrectPlace(clusterInfo, isClusterInCorrectPlace);
                }
                
                return;
            }
            
            if (!isCorrect && rowCluster.ResultWordIndex == -1)
            {
                TrySetWordIndex(rowCluster);
                return;
            }

            foreach (var row in _rowClusters)
            {
                if (row == rowCluster)
                    continue;

                if (row.ResultWordIndex == wordIndex)
                {
                    rowCluster.ResultWordIndex = -1;
                    SetIsCorrectPlace(rowCluster, false);
                    return;
                }
            }
            
            rowCluster.ResultWordIndex = wordIndex;
            SetIsCorrectPlace(rowCluster, true);
        }

        private void TrySetWordIndex(RowCluster rowCluster)
        {
            foreach (var clusterInfo in rowCluster.ClusterInfos)
            {
                var wordIndex = 0;
                bool isClusterInCorrectPlace;
                
                if (rowCluster.ResultWordIndex == -1)
                    isClusterInCorrectPlace = _resultWordsContainer.TryGetCorrectWordIndexForCluster(clusterInfo.StartCellIndex, clusterInfo.Cluster, rowCluster.WordLength, out wordIndex) && IsWordIndexFree(wordIndex);
                else
                    isClusterInCorrectPlace = _resultWordsContainer.IsClusterInCorrectPlace(rowCluster.ResultWordIndex, clusterInfo.StartCellIndex, clusterInfo.Cluster, rowCluster.WordLength);
                
                if (isClusterInCorrectPlace)
                    rowCluster.ResultWordIndex = wordIndex;
                
                SetIsCorrectPlace(clusterInfo, isClusterInCorrectPlace);
            }
        }

        private void SetIsCorrectPlace(ClusterInfo clusterInfo, bool isCorrectPlace)
        {
            clusterInfo.IsCorrectPlace = isCorrectPlace;
            ClusterStateChangedEvent?.Invoke(clusterInfo.Cluster, isCorrectPlace);
        }
        
        private void SetIsCorrectPlace(RowCluster rowCluster, bool isCorrectPlace)
        {
            foreach (var clusterInfo in rowCluster.ClusterInfos)
                SetIsCorrectPlace(clusterInfo, isCorrectPlace);
        }
        
        private bool IsWordIndexFree(int wordIndex)
        {
            foreach (var rowCluster in _rowClusters)
            {
                if (rowCluster.ResultWordIndex == wordIndex)
                    return false;
            }
            
            return true;
        }
        
        public bool CheckFieldForFinish()
        {
            for (var i = 0; i < _rowClusters.Length; i++)
            {
                if (!_rowClusters[i].CheckForCorrect())
                    _wordCompleteTimes[i] = 0;
                else if (_wordCompleteTimes[i] == 0)
                    _wordCompleteTimes[i] = Time.time;
            }

            foreach (var wordCompleteTime in _wordCompleteTimes)
            {
                if (wordCompleteTime == 0)
                    return false;
            }
            
            return true;
        }

        public string[] GetWordsOrderedByTime()
        {
            var resultWordsByTime = new string[_rowClusters.Length];

            if (!IsCompleteTimesValid())
            {
                Debug.LogError($"{GetType().Name} GetWordsOrderedByTime error. _wordCompleteTimes invalid");
                return resultWordsByTime;
            }
            
            var addedWords = 0;
            var lastSaveTime = float.MinValue;
            var selectedIndex = -1;
            
            while (addedWords < resultWordsByTime.Length)
            {
                var minTime = float.MaxValue;
                
                for (var i = 0; i < _wordCompleteTimes.Length; i++)
                {
                    if (_wordCompleteTimes[i] > minTime)
                        continue;
                    
                    if (_wordCompleteTimes[i] <= lastSaveTime)
                        continue;
                    
                    minTime = _wordCompleteTimes[i];
                    selectedIndex = i;
                }

                lastSaveTime = _wordCompleteTimes[selectedIndex];
                resultWordsByTime[addedWords] = _rowClusters[selectedIndex].GetResultWord();
                addedWords++;
            }
            
            return resultWordsByTime;
        }
        
        private bool IsCompleteTimesValid()
        {
            for (var i = 0; i < _wordCompleteTimes.Length; i++)
            {
                if (_wordCompleteTimes[i] == 0)
                    return false;
            }
            
            return true;
        }

        private bool CheckIndexes(int wordIndex, int startCellIndex, int textLength)
        {
            if (!_rowClusters.CheckIndexWithLog(wordIndex))
                return false;

            var rowCluster = _rowClusters[wordIndex];
            return startCellIndex < rowCluster.WordLength && startCellIndex >= 0 && (textLength == 0 || startCellIndex + textLength - 1 < rowCluster.WordLength);
        }
    }
}