using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public interface IResultWordsContainer
    {
        int WordsCount { get; }
        
        void SetNewWords(string[] resultWords);
        int GetWordLength(int wordIndex);
        bool TryGetCorrectWordIndexForCluster(int startCellIndex, Cluster cluster , int requiredWordLength, out int wordIndex);
        bool IsClusterInCorrectPlace(int wordIndex, int startCellIndex, Cluster cluster, int wordLength);
        void Clear();
        bool TryGetCorrectWordIndex(List<ClusterInfo> clusterInfos, int rowClusterWordLength, out int wordIndex);
    }

    public class ResultWordsContainer : IResultWordsContainer
    {
        private string[] _resultWords;

        public int WordsCount => _resultWords.Length;

        public void SetNewWords(string[] resultWords)
        {
            _resultWords = resultWords;
        }

        public int GetWordLength(int wordIndex)
        {
            return CheckIndex(wordIndex) ? _resultWords[wordIndex].Length : 0;
        }
        
        public bool TryGetCorrectWordIndexForCluster(int startCellIndex, Cluster cluster, int requiredWordLength, out int wordIndex)
        {
            var result = false;
            var clusterLength = cluster.Text.Length;
            
            for (var i = 0; i < _resultWords.Length; i++)
            {
                var wordLength = _resultWords[i].Length;
                
                if (requiredWordLength != wordLength)
                    continue;
                
                if (wordLength - startCellIndex < clusterLength)
                    continue;

                for (var j = startCellIndex; j < startCellIndex + clusterLength; j++)
                {
                    if (_resultWords[i][j] == cluster.Text[j - startCellIndex])
                    {
                        result = true;
                        continue;
                    }

                    result = false;
                    break;
                }

                if (!result) 
                    continue;
                
                wordIndex = i;
                return true;
            }

            wordIndex = 0;
            return false;
        }
        
        public bool IsClusterInCorrectPlace(int wordIndex, int startCellIndex, Cluster cluster, int wordLength)
        {
            if (!CheckIndex(wordIndex))
                return false;
            
            var result = false;
            var clusterLength = cluster.Text.Length;
            var word = _resultWords[wordIndex];
            
            if (word.Length != wordLength)
                return false;

            if (wordLength - startCellIndex < clusterLength)
                return false;

            for (var i = startCellIndex; i < startCellIndex + clusterLength; i++)
            {
                if (word[i] == cluster.Text[i - startCellIndex])
                {
                    result = true;
                    continue;
                }

                result = false;
                break;
            }
                
            return result;
        }

        public void Clear()
        {
            _resultWords = null;
        }

        public bool TryGetCorrectWordIndex(List<ClusterInfo> clusterInfos, int wordLength, out int wordIndex)
        {
            for (var i = 0; i < _resultWords.Length; i++)
            {
                var resultWord = _resultWords[i];
                
                if (resultWord.Length != wordLength)
                    continue;
                
                var isCorrect = true;
                
                foreach (var clusterInfo in clusterInfos)
                {
                    if (clusterInfo.StartCellIndex + clusterInfo.Cluster.Text.Length > resultWord.Length)
                        break;

                    for (var j = 0; j < clusterInfo.Cluster.Text.Length; j++)
                    {
                        if (clusterInfo.Cluster.Text[j] == resultWord[j + clusterInfo.StartCellIndex]) 
                            continue;
                        
                        isCorrect = false;
                        break;
                    }
                }
                
                if (!isCorrect) 
                    continue;
                    
                wordIndex = i;
                return true;
            }
            
            wordIndex = 0;
            return false;
        }

        private bool CheckIndex(int wordIndex)
        {
            if (_resultWords != null) 
                return _resultWords.CheckIndexWithLog(wordIndex);
            
            Debug.LogError($"{GetType().Name} can't check index, _resultWords = null");
            return false;
        }
    }
}