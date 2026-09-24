using Arctic.Foundation.Actor;
using UnityEngine;

namespace Arctic.Gameplay.Survival.Actors
{
    public class FuelBurnerActor : MonoActor<FuelBurner, FuelBurnerView>
    {
        [SerializeField]
        protected FuelBurner burner;
        public FuelBurner Burner => burner;

        public override FuelBurner GetViewContext()
        {
            return burner;
        }

        protected override void Simulate(float deltaTime)
        {
            burner.Update(deltaTime);
        }
    }
}