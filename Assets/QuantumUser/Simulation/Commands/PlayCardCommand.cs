namespace Quantum
{
    using Photon.Deterministic;

    public class PlayCardCommand : DeterministicCommand
    {
        public byte Slot;
        
        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref Slot);
        }
    }
}
