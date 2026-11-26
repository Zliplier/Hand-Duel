using UnityEngine.Scripting;

namespace Quantum
{
    using Photon.Deterministic;

    [Preserve]
    public unsafe class PlayerStatsSystem : SystemMainThreadFilter<PlayerStatsSystem.Filter>, ISignalOnComponentAdded<PlayerData>
    {
        public override void Update(Frame frame, ref Filter filter)
        {
            
        }

        public struct Filter
        {
            public EntityRef Entity;
            public PlayerData* Data;
        }

        public void OnAdded(Frame f, EntityRef entity, PlayerData* component)
        {
            var stats = f.FindAsset(component->Stats);
            component->Lives = stats.Lives;
        }
    }
}
