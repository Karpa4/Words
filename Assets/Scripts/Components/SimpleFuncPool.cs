using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Components
{
    public class SimpleFuncPool<T>
        where T : MonoBehaviour
    {
        private readonly Func<T> _newItemFunc;
        private readonly Queue<T> _pool = new();
        
        public SimpleFuncPool(Func<T> newItemFunc)
        {
            _newItemFunc = newItemFunc;
        }

        public T Get()
        {
            if (!_pool.TryDequeue(out var result))
                result = _newItemFunc.Invoke();
            
            result.gameObject.SetActive(true);
            return result;
        }

        public void Return(T item)
        {
            item.gameObject.SetActive(false);
            _pool.Enqueue(item);
        }
        
        public void Clear()
        {
            if (_pool.Count == 0)
                return;
            
            foreach (var item in _pool)
                Object.Destroy(item.gameObject);
            
            _pool.Clear();
        }
    }
}