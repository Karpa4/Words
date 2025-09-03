using Configuration;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public interface ICellPositionProvider
    {
        Vector2 GetCellPosition(int wordIndex, int cellIndex);
        ClusterPosition GetNearestClusterPosition(Vector2 position);
    }
    
    public class CellPositionProvider : ICellPositionProvider
    {
        [Inject] private IEmptyCellConfig _emptyCellConfig;
        [Inject] private IResultWordsContainer _resultWordsContainer;
        
        public Vector2 GetCellPosition(int wordIndex, int cellIndex)
        {
            var y = GetY(wordIndex);
            var x = GetX(_resultWordsContainer.GetWordLength(wordIndex), cellIndex);
            return new Vector2(x, y);
        }

        public ClusterPosition GetNearestClusterPosition(Vector2 position)
        {
            var nearestWordIndex = 0;
            var minDistanceY = Mathf.Infinity;

            for (var i = 0; i < _resultWordsContainer.WordsCount; i++)
            {
                var yPosition = GetY(i);
                var distanceY = Mathf.Abs(position.y - yPosition);

                if (distanceY > minDistanceY)
                    continue;
                
                minDistanceY = distanceY;
                nearestWordIndex = i;
            }

            var wordLength = _resultWordsContainer.GetWordLength(nearestWordIndex);
            var nearestCellIndex = 0;
            var minDistanceX = Mathf.Infinity;

            for (var i = 0; i < wordLength; i++)
            {
                var xPosition = GetX(wordLength, i);
                var distanceX = Mathf.Abs(position.x - xPosition);
                
                if (distanceX > minDistanceX)
                    continue;

                nearestCellIndex = i;
                minDistanceX = distanceX;
            }

            return new ClusterPosition(nearestWordIndex, nearestCellIndex);
        }

        private int GetX(int wordLength, int cellIndex)
        {
            var x = -((wordLength - 1) * _emptyCellConfig.CellXOffset) / 2f + cellIndex * _emptyCellConfig.CellXOffset;
            return (int)x;
        }
        
        private int GetY(int wordIndex)
        {
            return _emptyCellConfig.TopWordHeight + _emptyCellConfig.WordYOffset * wordIndex;;
        }
    }

    public struct NearestPositionInfo
    {
        public Vector2 Position { get; }
        public int WordIndex { get; }
        public int CellIndex { get; }
        
        public NearestPositionInfo(Vector2 position, int wordIndex, int cellIndex)
        {
            Position = position;
            WordIndex = wordIndex;
            CellIndex = cellIndex;
        }
    }
}