using Input;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public interface IFreeClusterZoneInfoProvider
    {
        bool IsInFreeZone();
    }

    public class FreeClusterZoneInfoProvider : IFreeClusterZoneInfoProvider
    {
        [Inject] private IGameObjectsHolder _gameObjectsHolder;
        [Inject] private IInputProvider _inputHandler;
        
        public bool IsInFreeZone()
        {
            return RectTransformUtility.RectangleContainsScreenPoint(_gameObjectsHolder.ClustersScrollParent, _inputHandler.PointerPosition, _gameObjectsHolder.Canvas.worldCamera);
        }
    }
}