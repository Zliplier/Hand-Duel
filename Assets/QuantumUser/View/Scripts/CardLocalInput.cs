using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QuantumUser.View.Scripts
{
    public class CardLocalInput : MonoBehaviour
    {
        [SerializeField] int playerSlot;
        [SerializeField] InputActionReference slot0;
        [SerializeField] InputActionReference slot1;
        [SerializeField] InputActionReference slot2;
        
        void Update()
        {
            if (slot0.action.WasPressedThisFrame())      Play(0);
            else if (slot1.action.WasPressedThisFrame()) Play(1);
            else if (slot2.action.WasPressedThisFrame()) Play(2);
        }

        void Play(int slot)
        {
            //Debug.Log($"Play slot: {slot}");
            var game = QuantumRunner.Default?.Game;
            if (game == null) { Debug.LogWarning("RPS: no running Quantum game"); return; }
            game.SendCommand(playerSlot, new PlayCardCommand { Slot = slot });
        }
    }
}