using System.Collections.Generic;
using System.Threading.Tasks;
using Configuration;
using Gameplay;
using UnityEngine;
using Zenject;

namespace Services
{
    public interface ILevelStarter
    {
        Task StartNewLevel(int level);
        Task StartNextLevel();
    }
    
    public class LevelStarter : ILevelStarter
    {
        [Inject] private ILevelConfigProvider _levelConfigProvider;
        [Inject] private IClusterManager _clusterManager;
        [Inject] private ILastLevelProvider _lastLevelProvider;
        [Inject] private IClusterPositionContainer _clusterPositionContainer;
        [Inject] private IEmptyCellsCreator _emptyCellsCreator;
        [Inject] private IResultWordsContainer _resultWordsContainer;
        [Inject] private IGameFieldChecker _gameFieldChecker;
        
        public Task StartNewLevel(int level)
        {
            return StartLevel(level);
        }
        
        public Task StartNextLevel()
        {
            return StartLevel(_lastLevelProvider.LastCompletedLevelNumber + 1);
        }

        private async Task StartLevel(int level)
        {
            var config = await _levelConfigProvider.GetLevelConfig(level);
            _clusterPositionContainer.Clear();
            _clusterManager.SetNewClusters(GetClusters(config));
            var resultWords = GetWords(config);
            _resultWordsContainer.SetNewWords(resultWords);
            _gameFieldChecker.Start();
            _emptyCellsCreator.Create(resultWords);
        }

        private ICollection<Cluster> GetClusters(LevelConfig config)
        {
            var clusters = new List<Cluster>();
            
            foreach (var wordConfig in config.WordConfigs)
            {
                foreach (var clusterText in wordConfig.Clusters)
                    clusters.Add(new Cluster(clusterText));
            }
            
            Shuffle(clusters);
            return clusters;
        }

        private void Shuffle(List<Cluster> clusters)
        {
            for (var i = clusters.Count - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (clusters[i], clusters[j]) = (clusters[j], clusters[i]);
            }
        }
        
        private string[] GetWords(LevelConfig config)
        {
            var result = new string[config.WordConfigs.Length];

            for (var i = 0; i < config.WordConfigs.Length; i++)
                result[i] = string.Concat(config.WordConfigs[i].Clusters);
            
            return result;
        }
    }
}