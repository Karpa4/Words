using Services;
using UI;
using Zenject;

namespace Gameplay
{
    public interface IEndGameChecker
    {
        void CheckForEnd();
    }

    public class EndGameChecker : IEndGameChecker
    {
        [Inject] private IUIManager _uiManager;
        [Inject] private IInputHandler _inputHandler;
        [Inject] private IGameFieldChecker _gameFieldChecker;
        [Inject] private ILastLevelProvider _lastLevelProvider;

        public void CheckForEnd()
        {
            if (!_gameFieldChecker.CheckFieldForFinish())
                return;
            
            _inputHandler.Stop();
            _lastLevelProvider.IncreaseLastCompletedLevel();
            _uiManager.Open(ViewName.EndGame);
        }
    }
}