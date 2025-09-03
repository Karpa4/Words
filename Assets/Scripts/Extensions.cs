using UnityEngine;

public static class Extensions
{
    public static bool CheckIndexWithLog<T>(this T[] array, int index)
    {
        var isCorrect = index >= 0 && index < array.Length;
            
        if (!isCorrect)
            Debug.LogError($"item with index {index} not found");
            
        return isCorrect;
    }
    
    public static bool CheckIndexWithLog(this string item, int index)
    {
        var isCorrect = index >= 0 && index < item.Length;
            
        if (!isCorrect)
            Debug.LogError($"item with index {index} not found");
            
        return isCorrect;
    }
}