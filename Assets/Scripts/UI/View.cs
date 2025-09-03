using UnityEngine;

namespace UI
{
    public class View : MonoBehaviour
    {
        public bool IsOpened => gameObject.activeSelf;

        private void Awake()
        {
            Initialize();
        }
        
        public virtual void Open() {}
        public virtual void Clear() {}
        public virtual void Close() {}
        protected virtual void Initialize() {}
    }
}