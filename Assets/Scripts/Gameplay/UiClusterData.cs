using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public interface IUiClusterData
    {
        public Cluster Cluster { get; }
        public RectTransform ClusterObject { get; }
    }
    
    public interface IUiClusterStateData
    {
        public Cluster Cluster { get; }
        public Image BackImage { get; }
        public ClusterState ClusterState { get; }

        void SetClusterState(ClusterState state);
    }

    public class UiClusterData : IUiClusterData , IUiClusterStateData
    {
        public Cluster Cluster { get; private set; }
        public bool InField { get; set; }
        public RectTransform ClusterObject { get; }
        public Image BackImage { get; }
        public ClusterState ClusterState { get; private set; }
        public List<TextMeshProUGUI> Texts { get; }

        public UiClusterData(Cluster cluster, RectTransform clusterObject, Image backImage, List<TextMeshProUGUI> texts)
        {
            Cluster = cluster;
            ClusterObject = clusterObject;
            Texts = texts;
            ClusterState = ClusterState.Default;
            BackImage = backImage;
        }
        
        public UiClusterData(RectTransform clusterObject, Image backImage)
        {
            ClusterObject = clusterObject;
            ClusterState = ClusterState.Default;
            Texts = new List<TextMeshProUGUI>();
            BackImage = backImage;
        }
        
        public Vector2 GetFirstLetterLocalPosition()
        {
            if (Texts.Count == 0)
                return ClusterObject.localPosition;
            
            return ClusterObject.localPosition + Texts[0].rectTransform.localPosition;
        }

        public void SetCluster(Cluster cluster)
        {
            Cluster = cluster;
        }
        
        public void SetClusterState(ClusterState state)
        {
            ClusterState = state;
        }
    }
}