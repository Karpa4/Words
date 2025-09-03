using System.Collections.Generic;
using Components;
using Services;
using UnityEngine.UI;
using Zenject;

namespace Gameplay
{
    public interface IEmptyCellsCreator
    {
        void Create(string[] words);
        void Clear();
        void ClearWithDestroy();
    }

    public class EmptyCellsCreator : IEmptyCellsCreator
    {
        [Inject] private ICellPositionProvider _cellPositionProvider;
        [Inject] private IGameObjectsHolder _gameObjectsHolder;
        [Inject] private ILocalPrefabProvider _prefabProvider;
        
        private SimpleFuncPool<Image> _pool;
        private readonly List<Image> _activeImages = new();
        
        public void Create(string[] words)
        {
            _pool ??= new SimpleFuncPool<Image>(GetNew);
            Clear();

            for (var i = 0; i < words.Length; i++)
            {
                var word = words[i];
                
                for (var j = 0; j < word.Length; j++)
                {
                    var image = _pool.Get();
                    image.rectTransform.anchoredPosition = _cellPositionProvider.GetCellPosition(i, j);
                    _activeImages.Add(image);
                }
            }
        }

        public void Clear()
        {
            if (_activeImages.Count == 0)
                return;
            
            foreach (var activeImage in _activeImages)
                _pool.Return(activeImage);
            
            _activeImages.Clear();
        }

        public void ClearWithDestroy()
        {
            Clear();
            _pool.Clear();
        }

        private Image GetNew()
        {
            return _prefabProvider.GetEmptyCell(_gameObjectsHolder.EmptyCellParent);
        }
    }
}