using Combat.Interfaces;
using DefaultNamespace;
using Zenject;

namespace Combat.Misc
{
    public class RandomImpulseToCentre : ImpulseApplier
    {
        [Inject] private IPlayerMovement playerMovement;
        
        protected override void Configure()
        {
            base.Configure();
            direction.x = ( -transform.position.ConvertToVector2() + playerMovement.Position).normalized.x;
            direction.y = 1;
            impulse = UnityEngine.Random.Range(impulse*0.5f, impulse*2f);
        }
    }
}