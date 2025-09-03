using System.Collections.Generic;
using Components;
using Gameplay;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class EndGameView : View
    {
        [SerializeField] private TextMeshProUGUI _textWordPrefab;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private RectTransform _parent;
        [SerializeField] private Button _nextLevelButton;

        private SimpleFuncPool<TextMeshProUGUI> _prefabPool;
        private readonly List<TextMeshProUGUI> _activeTexts = new();

        [Inject] private IContinueGameService _continueGameService;
        [Inject] private IGameFieldChecker _gameFieldChecker;
        [Inject] private IGameFinisher _gameFinisher;
        
        protected override void Initialize()
        {
            _prefabPool = new SimpleFuncPool<TextMeshProUGUI>(GetNew);
        }
        
        public override void Open()
        {
            var canContinuePlay = _continueGameService.CanContinuePlay();
            _nextLevelButton.gameObject.SetActive(canContinuePlay);
            _title.text = canContinuePlay ? "You win" : "All levels complete";
            var wordsOrderedByTime = _gameFieldChecker.GetWordsOrderedByTime();

            foreach (var word in wordsOrderedByTime)
            {
                var text = _prefabPool.Get();
                text.text = word;
                _activeTexts.Add(text);
            }
        }

        public override void Close()
        {
            ClearActiveTexts();
        }
        
        public override void Clear()
        {
            ClearActiveTexts();
            _prefabPool.Clear();
        }

        /// <summary> Invoke in UI </summary>
        public void NextLevel()
        {
            _continueGameService.ContinuePlay();
        }
        
        /// <summary> Invoke in UI </summary>
        public void MainMenu()
        {
            _gameFinisher.Finish();
        }

        private void ClearActiveTexts()
        {
            if (_activeTexts.Count == 0)
                return;
            
            foreach (var text in _activeTexts)
                _prefabPool.Return(text);
            
            _activeTexts.Clear();
        }

        private TextMeshProUGUI GetNew()
        {
            return Instantiate(_textWordPrefab, _parent);
        }
    }
}