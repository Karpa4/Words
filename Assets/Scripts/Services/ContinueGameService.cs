using System.Threading.Tasks;
using Gameplay;
using UI;
using UnityEngine;
using Zenject;

namespace Services
{
    public interface IContinueGameService
    {
        bool CanContinuePlay();
        Task ContinuePlay();
    }

    public class ContinueGameService : IContinueGameService
    {
        [Inject] private ILastLevelProvider _lastLevelProvider;
        [Inject] private ILevelConfigProvider _levelConfigProvider;
        [Inject] private ILevelStarter _levelStarter;
        [Inject] private IInputHandler _inputHandler;
        [Inject] private IUIManager _uiManager;
        
        public bool CanContinuePlay()
        {
            return _levelConfigProvider.HasConfigForLevel(_lastLevelProvider.LastCompletedLevelNumber + 1);
        }

        public async Task ContinuePlay()
        {
            if (!CanContinuePlay())
            {
                Debug.LogError($"{GetType().Name} need more levels. LastCompletedLevelNumber = {_lastLevelProvider.LastCompletedLevelNumber}");
                return;
            }
            
            await _levelStarter.StartNextLevel();
            _uiManager.Open(ViewName.Game);
            _inputHandler.Start();
        }
    }
}