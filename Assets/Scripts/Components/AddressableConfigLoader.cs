using System;
using System.Threading.Tasks;
using Configuration;
using Gameplay;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Components
{
    public interface IConfigLoader
    {
        Task<LevelConfigs> LoadLevelConfigs(string key);
    }
    
    public class AddressableConfigLoader : IConfigLoader
    {
        public async Task<LevelConfigs> LoadLevelConfigs(string key)
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<TextAsset>(key);
                await handle.Task;
                
                if (handle.Status == AsyncOperationStatus.Failed || !handle.IsValid())
                {
                    Debug.LogError($"[{GetType().Name}] Content loading failed: {key}. Loading operation status: {handle.Status}, isValid: {handle.IsValid()}");
                    return null;
                }
                
                var content = handle.Result;

                if (content == null)
                {
                    Debug.LogError($"[{GetType().Name}] Text Asset with key {key} = null");
                    return null;
                }

                var levelConfigs = JsonUtility.FromJson<LevelConfigs>(content.text);
                Addressables.Release(handle);
                return levelConfigs;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{GetType().Name}] Failed to load config with key {key} \n {e.Message} {e.StackTrace}");
            }
            
            return null;
        }
    }
}