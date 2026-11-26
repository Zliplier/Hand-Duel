using UnityEngine.Scripting;

namespace Quantum
{
    using Photon.Deterministic;

    [Preserve]
    public unsafe class PlayerStatsSystem : SystemMainThreadFilter<PlayerStatsSystem.Filter>, ISignalOnEntityPrototypeMaterialized
    {
        public override void Update(Frame frame, ref Filter filter)
        {
            
        }

        public struct Filter
        {
            public EntityRef Entity;
            public PlayerData* Data;
        }
        
        public void OnEntityPrototypeMaterialized(Frame f, EntityRef entity, EntityPrototypeRef prototypeRef)
        {
            if (f.Unsafe.TryGetPointer(entity, out PlayerData* data))
            {
                var stats = f.FindAsset(data->Stats);
                data->Lives = stats.Lives;
            }
        }
    }
}
