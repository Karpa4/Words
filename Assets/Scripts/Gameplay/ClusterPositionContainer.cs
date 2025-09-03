using System.Collections.Generic;

namespace Gameplay
{
    public interface IClusterPositionContainer
    {
        void Add(Cluster cluster, ClusterPosition position);
        bool TryExtract(Cluster cluster,out ClusterPosition position);
        void Remove(Cluster cluster);
        void Clear();
    }

    public class ClusterPositionContainer : IClusterPositionContainer
    {
        private readonly Dictionary<Cluster, ClusterPosition> _clusterPositions = new();
        
        public void Add(Cluster cluster, ClusterPosition position)
        {
            _clusterPositions[cluster] = position;
        }
        
        public bool TryExtract(Cluster cluster,out ClusterPosition position)
        {
            return _clusterPositions.Remove(cluster, out position);
        }
        
        public void Remove(Cluster cluster)
        {
            _clusterPositions.Remove(cluster);
        }

        public void Clear()
        {
            _clusterPositions.Clear();
        }
    }
}