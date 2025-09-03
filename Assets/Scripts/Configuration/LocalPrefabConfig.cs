using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Configuration
{
    public interface ILocalPrefabConfig
    {
        TextMeshProUGUI LetterPrefab { get; }
        RectTransform ClusterPrefab { get; }
        Image EmptyCellPrefab { get; }
    }

    [CreateAssetMenu(fileName = @"LocalPrefabConfig", menuName = @"Words/Configurations/Local Prefab Config", order = 2)]
    public class LocalPrefabConfig : ScriptableObject, ILocalPrefabConfig
    {
        [SerializeField] private TextMeshProUGUI _letterPrefab;
        [SerializeField] private RectTransform _clusterPrefab;
        [SerializeField] private Image _emptyCellPrefab;
        
        public TextMeshProUGUI LetterPrefab => _letterPrefab;
        public RectTransform ClusterPrefab => _clusterPrefab;
        public Image EmptyCellPrefab => _emptyCellPrefab;
    }
}