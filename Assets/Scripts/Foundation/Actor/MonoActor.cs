using UnityEngine;

namespace Arctic.Foundation.Actor
{
    public abstract class MonoActor<T> : MonoBehaviour where T : MonoActor<T>
    {
        [SerializeField]
        protected ActorView<T> _view;
        public ActorView<T> View => _view;

        private void Update()
        {
            _view?.Update(Time.deltaTime);
        }
    }
}