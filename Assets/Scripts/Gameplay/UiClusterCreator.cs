using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay
{
    public interface IUiClusterCreator
    {
        UiClusterData GetNew(Cluster cluster, Transform parent);
        UiClusterData GetEmpty(Transform parent);
    }

    public class UiClusterCreator : IUiClusterCreator
    {
        [Inject] private ILocalPrefabProvider _localPrefabProvider;

        public UiClusterData GetNew(Cluster cluster, Transform parent)
        {
            var clusterObject = _localPrefabProvider.GetCluster(parent);
            var image = clusterObject.GetComponentInChildren<Image>();
            var texts = new List<TextMeshProUGUI>(cluster.Text.Length);

            for (var i = 0; i < cluster.Text.Length; i++)
            {
                var letter = cluster.Text[i];
                var text = _localPrefabProvider.GetLetter(clusterObject);
                text.text = letter.ToString();
                texts.Add(text);
            }
            
            return new UiClusterData(cluster, clusterObject, image, texts);
        }
        
        public UiClusterData GetEmpty(Transform parent)
        {
            var clusterObject = _localPrefabProvider.GetCluster(parent);
            var image = clusterObject.GetComponentInChildren<Image>();
            return new UiClusterData(clusterObject, image);
        }
    }
}