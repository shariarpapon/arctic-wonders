using UnityEngine;

namespace Arctic.Foundation.Actor
{
    public abstract class ActorView
    {
        public abstract void Update(float deltaTime);
        public abstract void Enable ();
        public abstract void Disable();
    }
}