using Zenject;

namespace Gameplay
{
    public interface IMainGameProcessor
    {
        bool IsBusy { get; }
        
        bool TryStart(int instanceID);
        void Finish();
    }

    public class MainGameProcessor : IMainGameProcessor
    {
        [Inject] private IClusterManager _clusterManager;
        [Inject] private IClusterPositionContainer _clusterPositionContainer;
        [Inject] private IGameFieldChecker _gameFieldChecker;
        [Inject] private IClusterPositionChecker _clusterPositionChecker;
        [Inject] private ICellPositionProvider _cellPositionProvider;
        [Inject] private IClusterStateUpdater _clusterStateUpdater;
        [Inject] private IEndGameChecker _endGameChecker;
        [Inject] private IClusterMover _clusterMover;

        private int _cachedInstanceID;
        private ClusterPosition _cachedPosition;
        private ClusterState _cachedState;

        public bool IsBusy => _cachedInstanceID != 0;
        
        public bool TryStart(int instanceID)
        {
            if (IsBusy || !_clusterManager.TryGetUiClusterData(instanceID, out var clusterData))
                return false;

            _cachedPosition = _clusterPositionContainer.TryExtract(clusterData.Cluster, out var position) ? position : ClusterPosition.Default();
            _cachedState = _clusterStateUpdater.GetState(clusterData.Cluster);
            _clusterStateUpdater.TrySetState(clusterData.Cluster, ClusterState.Drag);
            _cachedInstanceID = instanceID;
            _clusterMover.StartMove(clusterData.Cluster, clusterData.ClusterObject);
            return true;
        }

        public void Finish()
        {
            if (_cachedInstanceID == 0)
                return;
            
            var instanceID = _cachedInstanceID;
            _cachedInstanceID = 0;
            var firstLetterPosition = _clusterMover.GetFirstLetterPosition();
            _clusterMover.StopMove();
            
            if (!_clusterManager.TryGetCluster(instanceID, out var cluster))
                return;

            if (_clusterPositionChecker.IsInFreeZone())
            {
                _gameFieldChecker.Remove(_cachedPosition);
                _clusterManager.MoveClusterToFreeZone(instanceID);
                return;
            }

            if (!_clusterPositionChecker.TryGetNearestClusterPosition(firstLetterPosition, cluster, out var clusterPosition))
            {
                TryRestorePosition(cluster);
                return;
            }

            if (!_gameFieldChecker.MoveCluster(clusterPosition, cluster, _cachedPosition))
            {
                TryRestorePosition(cluster);
                return;
            }

            _clusterPositionContainer.Add(cluster, clusterPosition);
            _clusterManager.MoveClusterToField(instanceID, _cellPositionProvider.GetCellPosition(clusterPosition.WordIndex, clusterPosition.StartCellIndex));
            _endGameChecker.CheckForEnd();
        }
        
        private void TryRestorePosition(Cluster cluster)
        {
            _clusterStateUpdater.TrySetState(cluster, _cachedState);
            
            if (_cachedPosition.IsDefault)
                return;
                
            _clusterPositionContainer.Add(cluster, _cachedPosition);
        }
    }
}