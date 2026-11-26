using System;

namespace Quantum
{
    using System;
    using Photon.Deterministic;

    public class PlayCardCommand : DeterministicCommand
    {
        public Int32 Slot;
        
        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref Slot);
        }
    }
}
