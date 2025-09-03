using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Components;
using Configuration;
using UnityEngine;
using Zenject;

namespace Services
{
    public interface ILevelConfigProvider
    {
        Task<LevelConfig> GetLevelConfig(int levelNumber);
        bool HasConfigForLevel(int levelNumber);
    }

    public class LevelConfigProvider : ILevelConfigProvider
    {
        [Inject] private ILevelConfigKeyProvider _levelConfigKeyProvider;
        
        private readonly Dictionary<int, LevelConfig> _levelConfigs = new();
        private readonly IConfigLoader _configLoader = new AddressableConfigLoader();
        
        public async Task<LevelConfig> GetLevelConfig(int levelNumber)
        {
            if (_levelConfigs.TryGetValue(levelNumber, out var levelConfig))
                return levelConfig;

            await LoadNewLevelConfigs(levelNumber);
            return _levelConfigs.TryGetValue(levelNumber, out levelConfig) ? levelConfig : GetErrorLevelConfig();
        }

        public bool HasConfigForLevel(int levelNumber)
        {
            return _levelConfigKeyProvider.HasConfigForLevel(levelNumber);
        }

        private async Task LoadNewLevelConfigs(int levelNumber)
        {
            var key = _levelConfigKeyProvider.GetKeyForLevel(levelNumber);
            var levelConfigs = await _configLoader.LoadLevelConfigs(key);
            
            if (levelConfigs == null)
            {
                Debug.LogError($"{GetType().Name} level config for level {levelNumber} and key {key} not found.");
                return;
            }

            foreach (var config in levelConfigs.Configs)
                _levelConfigs[config.LevelNumber] = config;
        }

        private LevelConfig GetErrorLevelConfig()
        {
            return _levelConfigs.Values.FirstOrDefault();
        }
    }
}