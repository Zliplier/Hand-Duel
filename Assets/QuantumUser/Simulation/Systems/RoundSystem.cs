using UnityEngine;
using UnityEngine.Scripting;

namespace Quantum
{
    using Photon.Deterministic;

    [Preserve]
    public unsafe class RoundSystem : SystemMainThread
    {
        public override void OnInit(Frame f)
        {
            f.SetSingleton(new CardMatch { Phase = GamePhase.WaitingForPlayers });
        }
        
        public override void Update(Frame frame)
        {
            var match = frame.Unsafe.GetPointerSingleton<CardMatch>();

            switch (match->Phase)
            {
                case GamePhase.WaitingForPlayers:
                    if (CountPlayers(frame) == 2) StartMatch(frame, match);
                    break;

                case GamePhase.Picking:
                    match->TicksLeft--;
                    if (match->TicksLeft <= 0 || AllPlayed(frame)) EndPicking(frame, match);
                    break;

                case GamePhase.Reveal:
                    match->TicksLeft--;
                    if (match->TicksLeft <= 0) NextRound(frame, match);
                    break;
            }
        }
        
        static int TicksPerSecond(Frame f) => f.SessionConfig.UpdateFPS;   // verify, see notes
        
        static int CountPlayers(Frame f)
        {
            int n = 0;
            foreach (var pair in f.Unsafe.GetComponentBlockIterator<CardPlayer>()) n++;
            return n;
        }
        
        static bool AllPlayed(Frame f)
        {
            foreach (var pair in f.Unsafe.GetComponentBlockIterator<CardPlayer>())
                if (pair.Component->PlayedCard == CardType.None) return false;
            return true;
        }

        static void StartMatch(Frame f, CardMatch* match)
        {
            foreach (var pair in f.Unsafe.GetComponentBlockIterator<CardPlayer>())
                pair.Component->BuildShuffleAndDeal(f.RNG);

            match->Round = 1;
            BeginPicking(f, match);
        }
        
        static void BeginPicking(Frame f, CardMatch* match)
        {
            int seconds = RpsRules.PickSeconds(match->Round);
            match->TicksLeft = seconds * TicksPerSecond(f);
            match->Phase = GamePhase.Picking;
            Debug.Log($"Round {match->Round} picking: {seconds}s");
        }

        static void EndPicking(Frame f, CardMatch* match)
        {
            // resolution (winner, lives) comes in the next step
            foreach (var pair in f.Unsafe.GetComponentBlockIterator<CardPlayer>())
                Debug.Log($"Round {match->Round}: {pair.Entity} played {pair.Component->PlayedCard}");

            match->TicksLeft = RpsRules.RevealSeconds * TicksPerSecond(f);
            match->Phase = GamePhase.Reveal;
        }

        static void NextRound(Frame f, CardMatch* match)
        {
            foreach (var pair in f.Unsafe.GetComponentBlockIterator<CardPlayer>())
            {
                pair.Component->EndRound();
                if (!pair.Component->IsConsistent())
                    Debug.LogError($"Card count broken for {pair.Entity}");
            }

            match->Round++;
            BeginPicking(f, match);
        }
    }
    
    public static class RpsRules {
        public const int StartingLives = 3;
        public const int FirstRoundSeconds = 10;
        public const int MinRoundSeconds = 2;
        public const int RevealSeconds = 2;

        // round 1 = 10s, round 2 = 9s ... never below 2s
        public static int PickSeconds(int round) =>
            System.Math.Max(MinRoundSeconds, FirstRoundSeconds - (round - 1));
    }
}
