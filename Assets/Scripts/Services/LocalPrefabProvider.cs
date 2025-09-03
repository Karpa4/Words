using Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Services
{
    public interface ILocalPrefabProvider
    {
        TextMeshProUGUI GetLetter(Transform parent);
        RectTransform GetCluster(Transform parent);
        Image GetEmptyCell(Transform parent);
    }

    public class LocalPrefabProvider : ILocalPrefabProvider
    {
        [Inject] private ILocalPrefabConfig _localPrefabConfig;
        
        public TextMeshProUGUI GetLetter(Transform parent)
        {
            return Object.Instantiate(_localPrefabConfig.LetterPrefab, parent);
        }
        
        public RectTransform GetCluster(Transform parent)
        {
            return Object.Instantiate(_localPrefabConfig.ClusterPrefab, parent);
        }
        
        public Image GetEmptyCell(Transform parent)
        {
            return Object.Instantiate(_localPrefabConfig.EmptyCellPrefab, parent);
        }
    }
}