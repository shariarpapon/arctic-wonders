using Arctic.Foundation.Actor;
using UnityEngine;

namespace Arctic.Gameplay.Survival.Actors
{
    public class FuelBurnerActor : MonoActor<FuelBurnerView>
    {
        public FuelBurner burner;

        protected override void Update()
        {
            burner.Update(Time.deltaTime);
            base.Update();
        }
    }
}