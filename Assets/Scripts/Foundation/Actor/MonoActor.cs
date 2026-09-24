using UnityEngine;

namespace Arctic.Foundation.Actor
{
    public abstract class MonoActor<C, V> : MonoBehaviour where V : MonoActorView<C>
    {
        [SerializeField]
        protected V _view;
        
        public V View => _view;

        public abstract C GetViewContext();

        protected virtual void Update()
        {
            Simulate(Time.deltaTime);
            ProcessView();
        }

        protected abstract void Simulate(float deltaTime);

        private void ProcessView() 
        {
            if (_view != null)
            {
                _view?.UpdateView(GetViewContext(), Time.deltaTime);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning($"View is null for {this.name}. Please assign a view in the inspector, unless this is intended behavior.");
            }
#endif
        }
    }
}