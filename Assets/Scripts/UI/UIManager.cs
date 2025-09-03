using System;
using UnityEngine;

namespace UI
{
    public interface IUIManager
    {
        void Initialize();
        void Clear();
        void Open(ViewName viewName);
        void Close(ViewName viewName);
    }

    public class UIManager : MonoBehaviour, IUIManager
    {
        [SerializeField] private GameObject _uiContent;
        [SerializeField] private NamedView[] _views;
        
        private View _currentView;
        
        public void Initialize()
        {
            _uiContent.gameObject.SetActive(true);
        }

        public void Clear()
        {
            foreach (var view in _views)
                view.View.Clear();
        }

        public void Open(ViewName viewName)
        {
            if (!TryGetView(viewName, out var view))
                return;
            
            if (view.IsOpened)
                return;

            _currentView?.Close();
            _currentView?.gameObject.SetActive(false);
            _currentView = view;
            view.gameObject.SetActive(true);
            view.Open();
        }

        public void Close(ViewName viewName)
        {
            if (!TryGetView(viewName, out var view))
                return;
            
            if (!view.IsOpened)
                return;

            view.Close();
            view.gameObject.SetActive(false);
        }

        private bool TryGetView(ViewName viewName, out View view)
        {
            foreach (var namedView in _views)
            {
                if (viewName != namedView.ViewName)
                    continue;

                view = namedView.View;
                return true;
            }
            
            view = null;
            return false;
        }
    }

    [Serializable]
    public class NamedView
    {
        public ViewName ViewName;
        public View View;
    }

    public enum ViewName
    {
        Game,
        EndGame,
        MainMenu,
    }
}