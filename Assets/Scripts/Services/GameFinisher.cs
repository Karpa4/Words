using Gameplay;
using UI;
using Zenject;

namespace Services
{
    public interface IGameFinisher
    {
        void Finish();
    }
    
    public class GameFinisher : IGameFinisher
    {
        [Inject] private IInputHandler _inputHandler;
        [Inject] private IClusterManager _clusterManager;
        [Inject] private IEmptyCellsCreator _emptyCellsCreator;
        [Inject] private IResultWordsContainer _resultWordsContainer;
        [Inject] private IClusterPositionContainer _clusterPositionContainer;
        [Inject] private IGameFieldChecker _gameFieldChecker;
        [Inject] private IUIManager _uiManager;
        [Inject] private IClusterMover _clusterMover;
        [Inject] private IClusterStateUpdater _clusterStateUpdater;
        
        public void Finish()
        {
            _clusterPositionContainer.Clear();
            _clusterStateUpdater.Stop();
            _inputHandler.Stop();
            _clusterManager.ClearWithDestroy();
            _emptyCellsCreator.ClearWithDestroy();
            _resultWordsContainer.Clear();
            _uiManager.Clear();
            _gameFieldChecker.Clear();
            _clusterMover.Clear();
            _uiManager.Open(ViewName.MainMenu);
        }
    }
}