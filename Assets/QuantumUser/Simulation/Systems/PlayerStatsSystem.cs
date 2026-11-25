using UnityEngine.Scripting;

namespace Quantum
{
    using Photon.Deterministic;

    [Preserve]
    public unsafe class PlayerStatsSystem : SystemMainThreadFilter<PlayerStatsSystem.Filter>, ISignalOnComponentAdded<PlayerStats>
    {
        public override void Update(Frame frame, ref Filter filter)
        {
            
        }

        public struct Filter
        {
            public EntityRef Entity;
            public PlayerStats* Stats;
        }

        public void OnAdded(Frame f, EntityRef entity, PlayerStats* component)
        {
            var spec = f.FindAsset(component->Spec);
            component->Lives = spec.Lives;
        }
    }
}
