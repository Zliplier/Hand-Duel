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
            var link = new PlayerLink()
            {
                Player = player,
            };
            var stat = new PlayerStats();
            
            f.Add(entity, link);
            f.Add(entity,stat);
            Debug.Log($"RPS: created player entity {entity} for {player}");
        }
    }
}
