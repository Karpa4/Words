using Services;
using Zenject;

namespace Gameplay
{
    public interface IUiClusterChanger
    {
        void Change(UiClusterData uiData, Cluster cluster);
    }

    public class UiClusterChanger : IUiClusterChanger
    {
        [Inject] private ILocalPrefabProvider _localPrefabProvider;

        public void Change(UiClusterData uiData, Cluster cluster)
        {
            var difference = cluster.Text.Length - uiData.Texts.Count;
            uiData.SetCluster(cluster);
            
            if (difference > 0)
                AddNewTexts(uiData, difference);

            for (var i = 0; i < uiData.Texts.Count; i++)
            {
                if (i < cluster.Text.Length)
                {
                    uiData.Texts[i].text = cluster.Text[i].ToString();
                    uiData.Texts[i].gameObject.SetActive(true);
                }
                else
                    uiData.Texts[i].gameObject.SetActive(false);
            }
        }

        private void AddNewTexts(UiClusterData uiData, int count)
        {
            for (var i = 0; i < count; i++)
                uiData.Texts.Add(_localPrefabProvider.GetLetter(uiData.ClusterObject));
        }
    }
}