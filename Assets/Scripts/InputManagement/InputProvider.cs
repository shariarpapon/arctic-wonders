using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace Arctic.InputManagement
{
    public class InputProvider
    {
        //Menu Input
        public virtual bool PauseGame => UnityInput.GetKeyDown(KeyCode.Escape);
        public virtual bool TabMenu => UnityInput.GetKeyDown(KeyCode.Tab);
        public virtual bool OpenCommandConsole => UnityInput.GetKeyDown(KeyCode.F1);
        public virtual bool Submit => UnityInput.GetKeyDown(KeyCode.Return);
        public virtual bool SelectNextQuickSlotItem => UnityInput.GetKeyDown(KeyCode.X);

        //Interaction Input
        public virtual bool PrimaryUse => UnityInput.GetMouseButtonDown(0);
        public virtual bool SpecialUse => UnityInput.GetKeyDown(KeyCode.F);
        public virtual bool KeyboardInteract => UnityInput.GetKeyDown(KeyCode.E);
        public virtual bool MouseInteract => UnityInput.GetMouseButtonDown(0);

        //Basic Movement Input
        public virtual float RawInputX => UnityInput.GetAxisRaw("Horizontal");
        public virtual float RawInputY => UnityInput.GetAxisRaw("Vertical");
        public virtual float InputX => UnityInput.GetAxis("Horizontal");
        public virtual float InputY => UnityInput.GetAxis("Vertical");
        public virtual Vector2 MousePosition => UnityInput.mousePosition;
        public virtual Vector2 MouseScrollDelta => UnityInput.mouseScrollDelta;
        public virtual float MouseX => UnityInput.GetAxis("Mouse X");
        public virtual float MouseY => UnityInput.GetAxis("Mouse Y");

        //Special Movement Input
        public virtual bool Jump => UnityInput.GetKeyDown(KeyCode.Space);
        public virtual bool Sprint => UnityInput.GetKey(KeyCode.LeftShift);
        public virtual bool Aim => UnityInput.GetMouseButton(1);
        public virtual bool Dash => UnityInput.GetKeyDown(KeyCode.F);
        public virtual bool GroundSlam => UnityInput.GetKeyDown(KeyCode.R);
    }
}