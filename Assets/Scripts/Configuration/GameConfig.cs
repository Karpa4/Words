using System;
using UnityEngine;

namespace Configuration
{
    public interface ILevelConfigKeyProvider
    {
        string GetKeyForLevel(int levelNumber);
        bool HasConfigForLevel(int levelNumber);
    }
    
    public interface IEmptyCellConfig
    {
        public int TopWordHeight { get; }
        public int WordYOffset { get; }
        public int CellXOffset { get; }
    }
    
    public interface IClusterColorConfig
    {
        public Color DefaultColor { get; }
        public Color DragColor { get; }
        public Color CorrectColor { get; }
        public Color IncorrectColor { get; }
    }
    
    public interface IClusterPositionConfig
    {
        public float ClusterEndMoveMaxSqrDistance { get; }
    }

    [CreateAssetMenu(fileName = @"GameConfig", menuName = @"Words/Configurations/GameConfig", order = 1)]
    public class GameConfig : ScriptableObject, ILevelConfigKeyProvider, IEmptyCellConfig, IClusterPositionConfig, IClusterColorConfig
    {
        [Header("Level keys")]
        [SerializeField] private LevelsWithKey[] _levelsWithKeys;
        [SerializeField] private string _defaultKey;
        [Header("Cell positions")]
        [SerializeField] private int _topWordHeight;
        [SerializeField] private int _wordYOffset;
        [SerializeField] private int _cellXOffset;
        [SerializeField] private float _clusterEndMoveMaxSqrDistance;
        [Header("Cluster colors")]
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _dragColor;
        [SerializeField] private Color _correctColor;
        [SerializeField] private Color _incorrectColor;

        public int TopWordHeight => _topWordHeight;
        public int WordYOffset => _wordYOffset;
        public int CellXOffset => _cellXOffset;
        public float ClusterEndMoveMaxSqrDistance => _clusterEndMoveMaxSqrDistance;
        public Color DefaultColor => _defaultColor;
        public Color DragColor => _dragColor;
        public Color CorrectColor => _correctColor;
        public Color IncorrectColor => _incorrectColor;
        
        public string GetKeyForLevel(int levelNumber)
        {
            foreach (var levelWithKey in _levelsWithKeys)
            {
                if (levelNumber < levelWithKey.LevelNumberFrom || levelNumber > levelWithKey.LevelNumberTo)
                    continue;
                
                return levelWithKey.Key;
            }

            Debug.LogError($"{GetType().Name} key for Level {levelNumber} not found");
            return _defaultKey;
        }

        public bool HasConfigForLevel(int levelNumber)
        {
            foreach (var levelWithKey in _levelsWithKeys)
            {
                if (levelNumber < levelWithKey.LevelNumberFrom || levelNumber > levelWithKey.LevelNumberTo)
                    continue;
                
                return true;
            }
            
            return false;
        }

        [Serializable]
        private class LevelsWithKey
        {
            public int LevelNumberFrom;
            public int LevelNumberTo;
            public string Key;
        }
    }
}