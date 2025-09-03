using System.Threading.Tasks;
using Gameplay;
using UI;
using Zenject;

namespace Services
{
    public interface IGameStarter
    {
        Task Start();
    }

    public class GameStarter : IGameStarter
    {
        [Inject] private IInputHandler _inputHandler;
        [Inject] private IUIManager _uiManager;
        [Inject] private IClusterMover _clusterMover;
        [Inject] private ILevelStarter _levelStarter;
        [Inject] private IClusterStateUpdater _clusterStateUpdater;
        
        public async Task Start()
        {
            _clusterStateUpdater.Start();
            _clusterMover.Initialize();
            await _levelStarter.StartNextLevel();
            _inputHandler.Start();
            _uiManager.Open(ViewName.Game);
        }
    }
}