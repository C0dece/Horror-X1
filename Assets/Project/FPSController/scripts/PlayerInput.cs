using System;
using UnityEngine.InputSystem;

namespace PlayerController 
{
    [Serializable]
    public class PlayerInput
    {
        public  InputAction move;
        public  InputAction run;

        public  InputAction crouch;
        public  InputAction jump;

        public  InputAction mouseX;
        public  InputAction mouseY;

        public  InputAction leftMouseButton;
        public  InputAction escape;

        public void InputEnable()
        {
            if (move.bindings.Count == 0)             
            move.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        
        if (run.bindings.Count == 0)
            run.AddBinding("Run")
                .WithPath("<Keyboard>/shift");
        if (crouch.bindings.Count == 0)        
            crouch.AddBinding("Crouch")
                .WithPath("<Keyboard>/c");
        if (jump.bindings.Count == 0)
            jump.AddBinding("Jump")
                .WithPath("<Keyboard>/space");
        if (mouseX.bindings.Count == 0)
            mouseX.AddBinding("MouseX")
                .WithPath("<Mouse>/delta/x");
        if (mouseY.bindings.Count == 0)
            mouseY.AddBinding("MouseY")
                .WithPath("<Mouse>/delta/y");
        if (leftMouseButton.bindings.Count == 0)
            leftMouseButton.AddBinding("LeftMouseButton")
                .WithPath("<Mouse>/leftButton");
        if (escape.bindings.Count == 0)
            escape.AddBinding("Escape")
                .WithPath("<Keyboard>/escape");
                
            move.Enable();
            run.Enable();
            crouch.Enable();
            jump.Enable();
            mouseX.Enable();
            mouseY.Enable();
            leftMouseButton.Enable();
            escape.Enable();      
        }   
    }
}