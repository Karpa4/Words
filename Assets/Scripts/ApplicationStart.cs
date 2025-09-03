using Gameplay;
using Input;
using Services;
using UI;
using UnityEngine;
using Zenject;

public class ApplicationStart : MonoBehaviour
{
    [Inject] private ILastLevelProvider _lastLevelProvider;
    [Inject] private IGameStarter _gameStarter;
    [Inject] private IInputProvider _inputProvider;
    [Inject] private IUIManager _uiManager;

    private void Awake()
    {
        _inputProvider.Initialize();
        _lastLevelProvider.Initialize();
        _uiManager.Initialize();
        _uiManager.Open(ViewName.MainMenu);
    }
}