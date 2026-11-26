using UnityEngine;

namespace Quantum
{
    using Photon.Deterministic;
    using System.Text;
    
    public unsafe partial struct CardPlayer
    {
        public int HandSize => Hand.Length;
        public int DeckCapacity => Deck.Length;
        
        // Remove and return the top card of the deck
        CardType DrawTop()
        {
            var card = Deck[DeckIndex];
            Deck[DeckIndex] = CardType.None;          // keeps unused slots clean for debugging
            DeckIndex = (DeckIndex + 1) % DeckCapacity;
            DeckCount--;
            return card;
        }
        
        void PutBottom(CardType card) {
            Deck[(DeckIndex + DeckCount) % DeckCapacity] = card;
            DeckCount++;
        }
        
        // Play the card in a hand slot and draw its replacement immediately
        public bool TryPlay(int slot) {
            if (slot < 0 || slot >= HandSize) return false;
            if (PlayedCard != CardType.None) return false;   // already locked in this round
            if (DeckCount <= 0) return false;           // shouldn't happen, but never read garbage
            PlayedCard = Hand[slot];
            Hand[slot] = DrawTop();
            return true;
        }
        
        // Called after the reveal: the played card goes to the bottom
        public void EndRound() {
            if (PlayedCard == CardType.None) return;
            PutBottom(PlayedCard);
            PlayedCard = CardType.None;
        }
        
        // The only place a shuffle happens
        public void BuildShuffleAndDeal(RNGSession* rng) {
            int copies = DeckCapacity / 5;              // 10 cards = 2 of each of the 5 types
            int n = 0;
            for (int c = 0; c < copies; c++)
            for (int t = 1; t <= 5; t++) Deck[n++] = (CardType)t;

            for (int i = DeckCapacity - 1; i > 0; i--) { // Fisher-Yates
                int j = rng->Next(0, i + 1);
                (Deck[i], Deck[j]) = (Deck[j], Deck[i]);
            }

            DeckIndex = 0;
            DeckCount = DeckCapacity;
            PlayedCard = CardType.None;
            for (int s = 0; s < HandSize; s++) Hand[s] = DrawTop();
        }
        
        // cards in hand + played + deck must always equal the total
        public bool IsConsistent() {
            int inHand = 0;
            for (int i = 0; i < HandSize; i++) if (Hand[i] != CardType.None) inHand++;
            return inHand + (PlayedCard != CardType.None ? 1 : 0) + DeckCount == DeckCapacity;
        }
        
        // for log messages only, never for gameplay
        public string Describe() {
            var sb = new StringBuilder("Hand:");
            for (int i = 0; i < HandSize; i++) sb.Append(' ').Append(Hand[i]);
            sb.Append(" | Deck top->bottom:");
            for (int i = 0; i < DeckCount; i++) sb.Append(' ').Append(Deck[(DeckIndex + i) % DeckCapacity]);
            sb.Append(" | Played: ").Append(PlayedCard);
            return sb.ToString();
        }
    }
}
