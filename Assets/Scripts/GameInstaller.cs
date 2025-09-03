using Configuration;
using Gameplay;
using Input;
using Services;
using UI;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameConfig _gameConfig;
    [SerializeField] private LocalPrefabConfig _localPrefabConfig;
    
    public override void InstallBindings()
    {
        Container.Bind(typeof(ILevelConfigKeyProvider), typeof(IEmptyCellConfig), typeof(IClusterColorConfig), typeof(IClusterPositionConfig)).To<GameConfig>().FromInstance(_gameConfig);
        Container.Bind<ILocalPrefabConfig>().FromInstance(_localPrefabConfig);
        Container.Bind<IClusterMover>().To<ClusterMover>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<IGameObjectsHolder>().To<GameObjectsHolder>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<IUIManager>().To<UIManager>().FromComponentsInHierarchy().AsSingle();

        Container.Bind<IInputProvider>().To<InputProvider>().AsSingle();
        Container.Bind<IGameStarter>().To<GameStarter>().AsSingle();
        Container.Bind<IInputHandler>().To<InputHandler>().AsSingle();
        Container.Bind<ILastLevelProvider>().To<LastLevelProvider>().AsSingle();
        Container.Bind<IMainGameProcessor>().To<MainGameProcessor>().AsSingle();
        Container.Bind<ILevelConfigProvider>().To<LevelConfigProvider>().AsSingle();
        Container.Bind<ILocalPrefabProvider>().To<LocalPrefabProvider>().AsSingle();
        Container.Bind<IContinueGameService>().To<ContinueGameService>().AsSingle();
        Container.Bind<IClusterStateUpdater>().To<ClusterStateUpdater>().AsSingle();
        Container.Bind<IEndGameChecker>().To<EndGameChecker>().AsSingle();
        Container.Bind<IFreeClusterZoneInfoProvider>().To<FreeClusterZoneInfoProvider>().AsSingle();
        Container.Bind<IGameFieldChecker>().To<GameFieldChecker>().AsSingle();
        Container.Bind<IClusterManager>().To<ClusterManager>().AsSingle();
        Container.Bind<IEmptyCellsCreator>().To<EmptyCellsCreator>().AsSingle();
        Container.Bind<IClusterPositionChecker>().To<ClusterPositionChecker>().AsSingle();
        Container.Bind<IGameFinisher>().To<GameFinisher>().AsSingle();
        Container.Bind<ILevelStarter>().To<LevelStarter>().AsSingle();
        Container.Bind<IUiClusterCreator>().To<UiClusterCreator>().AsSingle();
        Container.Bind<IClusterPositionContainer>().To<ClusterPositionContainer>().AsSingle();
        Container.Bind<IUiClusterChanger>().To<UiClusterChanger>().AsSingle();
        Container.Bind<IResultWordsContainer>().To<ResultWordsContainer>().AsSingle();
        Container.Bind<ICellPositionProvider>().To<CellPositionProvider>().AsSingle();
    }
}