using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace Arctic.InputManagement
{
    public sealed class DisabledInputProvider : InputProvider
    {
        //Menu
        public override bool PauseGame => UnityInput.GetKeyDown(KeyCode.Escape);
        public override bool TabMenu => UnityInput.GetKeyDown(KeyCode.Tab);
        public override bool OpenCommandConsole => UnityInput.GetKeyDown(KeyCode.F1);
        public override bool Submit => UnityInput.GetKey(KeyCode.Return);
   
        //Interaction
        public override bool PrimaryUse => false;
        public override bool SpecialUse => false;
        public override bool KeyboardInteract => false;
        public override bool MouseInteract => false;
        public override bool SelectNextQuickSlotItem => false;

        //Movement
        public override float RawInputX => 0;
        public override float RawInputY => 0;
        public override float InputX => 0;
        public override float InputY => 0;
        public override Vector2 MousePosition => Vector2.zero;
        public override Vector2 MouseScrollDelta => Vector2.zero;
        public override float MouseX => 0;
        public override float MouseY => 0;

        //Special Movement
        public override bool Jump => false;
        public override bool Sprint => false;
        public override bool Aim => false;
        public override bool Dash => false;
        public override bool GroundSlam => false;
    }
}