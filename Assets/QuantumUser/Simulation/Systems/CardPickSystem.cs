namespace Quantum
{
    using UnityEngine;
    using UnityEngine.Scripting;
    using Photon.Deterministic;
    
    [Preserve]
    public unsafe class CardPickSystem : SystemMainThreadFilter<CardPickSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public PlayerLink* Link;
            public CardPlayer* Cards;
        }
        
        public override void Update(Frame f, ref Filter filter)
        {
            /*for (int i = 0; i < f.MaxPlayerCount; i++)
            {
                var cmd = f.GetPlayerCommand(i) as PlayCardCommand;
                if (cmd != null) Debug.Log($"[debug] player index {i} sent card slot {cmd.Slot}");
            }*/
            
            if (!f.Unsafe.TryGetPointerSingleton(out CardMatch* match)
                || match->Phase != GamePhase.Picking) return;
            
            var cmd = f.GetPlayerCommand(filter.Link->Player) as PlayCardCommand;
            if (cmd == null)
                return;

            bool ok = filter.Cards->TryPlay(cmd.Slot);
            Debug.Log($"{filter.Link->Player} slot {cmd.Slot} -> {(ok ? "played" : "rejected")} | {filter.Cards->Describe()} (verified: {f.IsVerified})");
        }
    }
}
