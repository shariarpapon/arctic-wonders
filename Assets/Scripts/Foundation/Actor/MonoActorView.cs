using UnityEngine;

namespace Arctic.Foundation.Actor
{
    public abstract class MonoActorView<C>: MonoBehaviour
    {
        public virtual void UpdateView(C ctx, float deltaTime) => throw new System.NotImplementedException();
        public virtual void EnableView() => throw new System.NotImplementedException();
        public virtual void DisableView() => throw new System.NotImplementedException();
    }
}