using System.Collections.Generic;

namespace Gameplay
{
    public class RowCluster
    {
        public int WordLength { get; }
        public int ResultWordIndex { get; set; } = -1;
        public List<ClusterInfo> ClusterInfos { get; } = new();

        public RowCluster(int wordLength)
        {
            WordLength = wordLength;
        }
        
        public bool CheckForCorrect()
        {
            var length = 0;
            
            foreach (var clusterInfo in ClusterInfos)
            {
                if (!clusterInfo.IsCorrectPlace)
                    return false;
                
                length += clusterInfo.Cluster.Text.Length;
            }
            
            return length == WordLength;
        }

        public string GetResultWord()
        {
            return string.Concat(GetClusterTexts());
        }

        private IEnumerable<string> GetClusterTexts()
        {
            foreach (var clusterInfo in ClusterInfos)
                yield return clusterInfo.Cluster.Text;
        }
    }
    
    public class ClusterInfo
    {
        public Cluster Cluster { get; }
        public int StartCellIndex { get; set; }
        public bool IsCorrectPlace { get; set; }
        public int LastCellIndex => StartCellIndex + Cluster.Text.Length - 1;
            
        public ClusterInfo(Cluster cluster, int startCellIndex)
        {
            Cluster = cluster;
            StartCellIndex = startCellIndex;
        }
    }
}