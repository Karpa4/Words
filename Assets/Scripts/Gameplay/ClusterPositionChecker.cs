using Configuration;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public interface IClusterPositionChecker
    {
        bool IsInFreeZone();
        bool TryGetNearestClusterPosition(Vector2 firstLetterPosition, Cluster cluster, out ClusterPosition nearestPosition);
    }

    public class ClusterPositionChecker : IClusterPositionChecker
    {
        [Inject] private ICellPositionProvider _cellPositionProvider;
        [Inject] private IClusterPositionConfig _clusterPositionConfig;
        [Inject] private IResultWordsContainer _resultWordsContainer;
        [Inject] private IFreeClusterZoneInfoProvider _freeClusterZoneInfoProvider;

        public bool IsInFreeZone()
        {
            return _freeClusterZoneInfoProvider.IsInFreeZone();
        }

        public bool TryGetNearestClusterPosition(Vector2 firstLetterPosition, Cluster cluster, out ClusterPosition nearestPosition)
        {
            nearestPosition = _cellPositionProvider.GetNearestClusterPosition(firstLetterPosition);
            var uiPosition = _cellPositionProvider.GetCellPosition(nearestPosition.WordIndex, nearestPosition.StartCellIndex);

            if ((uiPosition - firstLetterPosition).sqrMagnitude > _clusterPositionConfig.ClusterEndMoveMaxSqrDistance)
                return false;
            
            var wordLength = _resultWordsContainer.GetWordLength(nearestPosition.WordIndex);
            var cellOffset = wordLength - nearestPosition.StartCellIndex - cluster.Text.Length;
            var needMoveLeft = cellOffset <= 0;

            if (!needMoveLeft)
                return true;
            
            var resultCellIndex = nearestPosition.StartCellIndex + cellOffset;
            
            if (resultCellIndex <= 0)
                return false;
            
            nearestPosition = new ClusterPosition(nearestPosition.WordIndex, resultCellIndex);
            return true;
        }
    }
}