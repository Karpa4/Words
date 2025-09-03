using Gameplay;
using Services;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class MainMenuView : View
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _buttonText;

        [Inject] private ILastLevelProvider _lastLevelProvider;
        [Inject] private IContinueGameService _continueGameService;
        [Inject] private IGameStarter _gameStarter;
        
        public override void Open()
        {
            _buttonText.text = _continueGameService.CanContinuePlay() ? "Play" : "Restart";
            _levelText.text = _lastLevelProvider.LastCompletedLevelNumber.ToString();
        }
        
        /// <summary> Invoke in UI </summary>
        public void Play()
        {
            if (!_continueGameService.CanContinuePlay())
                _lastLevelProvider.Reset();
            
            _gameStarter.Start();
        }
    }
}