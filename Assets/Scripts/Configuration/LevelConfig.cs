using System;

namespace Configuration
{
    [Serializable]
    public class LevelConfigs
    {
        public LevelConfig[] Configs;
    }
    
    [Serializable]
    public class LevelConfig
    {
        public int LevelNumber;
        public WordConfig[] WordConfigs;
    }
    
    [Serializable]
    public class WordConfig
    {
        public string[] Clusters;
    }
}
