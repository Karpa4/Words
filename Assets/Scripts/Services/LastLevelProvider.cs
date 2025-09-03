using UnityEngine;

namespace Services
{
    public interface ILastLevelProvider
    {
        int LastCompletedLevelNumber { get; }
        
        void Initialize();
        void IncreaseLastCompletedLevel();
        void Reset();
    }
    
    public class LastLevelProvider : ILastLevelProvider
    {
        public int LastCompletedLevelNumber { get; private set; }

        public void Initialize()
        {
            LastCompletedLevelNumber = PlayerPrefs.GetInt(GameConstants.LAST_COMPLETED_LEVEL_PREFS_KEY, 0);
        }

        public void IncreaseLastCompletedLevel()
        {
            PlayerPrefs.SetInt(GameConstants.LAST_COMPLETED_LEVEL_PREFS_KEY, ++LastCompletedLevelNumber);
        }

        public void Reset()
        {
            LastCompletedLevelNumber = 0;
            PlayerPrefs.SetInt(GameConstants.LAST_COMPLETED_LEVEL_PREFS_KEY, 0);
        }
    }
}