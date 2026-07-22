using UnityEngine;

namespace Arctic.Foundation.Actor
{
    public abstract class ActorView<T> where T : MonoActor<T>
    {
        public abstract void Init(T actor);
        public abstract void Update(float deltaTime);
        public abstract void Enable ();
        public abstract void Disable();
    }
}