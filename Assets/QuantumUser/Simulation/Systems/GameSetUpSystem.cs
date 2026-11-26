using UnityEngine;
using UnityEngine.Scripting;

namespace Quantum
{
    using Photon.Deterministic;

    [Preserve]
    public unsafe class GameSetUpSystem : SystemSignalsOnly, ISignalOnPlayerAdded
    {
        public void OnPlayerAdded(Frame f, PlayerRef player, bool firstTime)
        {
            var runtimePlayer = f.GetPlayerData(player);
            var entity = f.Create(runtimePlayer.PlayerAvatar);
            
            if (f.Unsafe.TryGetPointer(entity, out PlayerLink* link))
                link->Player = player;
            else
                f.Add(entity, new PlayerLink { Player = player });
            
            if (!f.Unsafe.TryGetPointer(entity, out PlayerData* data))
                f.Add(entity, new PlayerData());
            
            if (!f.Unsafe.TryGetPointer(entity, out CardPlayer* cards))
                f.Add(entity, new CardPlayer());
            
            Debug.Log($"RPS: created player entity {entity} for {player}");
            
            if (f.Unsafe.TryGetPointer(entity, out CardPlayer* cardPlayer))
            {
                cardPlayer->BuildShuffleAndDeal(f.RNG);
                Debug.Log($"capacity = {cardPlayer->DeckCapacity}, deckCount = {cardPlayer->DeckCount}");
                Debug.Log($"{player} -> {cardPlayer->Describe()}");
            }
            
            /*// TEMPORARY TEST: remove once the round system deals the cards
            if (f.Unsafe.TryGetPointer(entity, out CardPlayer* cardPlayer))
            {
                cardPlayer->BuildShuffleAndDeal(f.RNG);
                Debug.Log($"capacity={cardPlayer->DeckCapacity} deckCount={cardPlayer->DeckCount}");
                Debug.Log($"{player} -> {cardPlayer->Describe()}");

                var copy = *cardPlayer;   // dereferencing copies, so the real hand stays untouched
                
                for (int r = 1; r <= 16; r++)
                {
                    int slot = (r - 1) % 3;
                    var played = copy.Hand[slot];
                    copy.TryPlay(slot);
                    var drawn = copy.Hand[slot];
                    copy.EndRound();
                    Debug.Log($"round {r}: slot {slot} played {played}, drew {drawn}, consistent={copy.IsConsistent()}");
                }
            }*/
        }
    }
}
