using UnityEngine;

namespace Arctic.Foundation.Actor
{
    public abstract class MonoActor<V> : MonoBehaviour where V : ActorView
    {
        [SerializeField]
        protected V _view;
        public V View => _view;

        protected virtual void Update()
        {
            UpdateView(Time.deltaTime);
        }

        public void UpdateView(float deltaTime) 
        {
            _view?.Update(deltaTime);
        }
    }
}