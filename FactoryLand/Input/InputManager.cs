using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    /// <summary>
    /// Input type constants sent to game objects
    /// Keyboard keys are mapped to them
    /// </summary>
    public enum InputType
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,

        CameraUp = 4,
        CameraDown = 5,
        CameraLeft = 6,
        CameraRight = 7,
        CameraZoom = 8,
        
        Click = 9
    }

    /// <summary>
    /// State of input to trigger on
    /// </summary>
    public enum InputState
    {
        Released = 0,
        Pressed = 1,
        Up = 2,
        Down = 3
    }

    /// <summary>
    /// Type of input recieved from the mouse
    /// </summary>
    public enum MouseAction
    {
        RightButton = 0,
        LeftButton = 1,
        MiddleButton = 2,
        XButton1 = 3,
        XButton2 = 4,
        Move = 5,
        Scroll = 6
    }

    /// <summary>
    /// Sends keyboard input to game objects
    /// </summary>
    class InputManager
    {
        private KeyboardState currentKeyboard;
        private KeyboardState previousKeyboard;
        private MouseState currentMouse;
        private MouseState previousMouse;

        // Key and mouse button bindings map to InputTypes. These can be user configured key bindings
        public Dictionary<Keys, InputType> KeyBindings { get; private set; } = new Dictionary<Keys, InputType>();
        public Dictionary<MouseAction, InputType> MouseBindings { get; private set; } = new Dictionary<MouseAction, InputType>();

        // InputTypes map to recievers
        private Dictionary<InputType, RecieverBinding> recievers = new Dictionary<InputType, RecieverBinding>();

        private class RecieverBinding
        {
            public RecieverBinding(InputType inputType)
            {
                this.inputType = inputType;
            }

            private readonly InputType inputType;
            private List<Tuple<IInputReciever, InputState>> bindings = new List<Tuple<IInputReciever, InputState>>();

            public void Add(IInputReciever reciever, InputState state)
            {
                bindings.Add(new Tuple<IInputReciever, InputState>(reciever, state));
            }

            public void SendInput(bool currentlyDown, bool previouslyDown, Point mousePos, int scrollDelta)
            {
                // Sends input to each binding with a matching InputState
                foreach (Tuple<IInputReciever, InputState> pair in bindings)
                {
                    if (currentlyDown)
                    {
                        if (pair.Item2 == InputState.Down)
                        {
                            pair.Item1.RecieveInput(inputType, pair.Item2, mousePos, scrollDelta);
                        }
                        else if (!previouslyDown && pair.Item2 == InputState.Pressed)
                        {
                            pair.Item1.RecieveInput(inputType, pair.Item2, mousePos, scrollDelta);
                        }
                    }
                    else
                    {
                        if (pair.Item2 == InputState.Up)
                        {
                            pair.Item1.RecieveInput(inputType, pair.Item2, mousePos, scrollDelta);
                        }
                        else if (previouslyDown && pair.Item2 == InputState.Released)
                        {
                            pair.Item1.RecieveInput(inputType, pair.Item2, mousePos, scrollDelta);
                        }
                    }
                }
            }
        }

        public InputManager()
        {
            foreach (InputType inputType in Enum.GetValues(typeof(InputType)))
            {
                recievers.Add(inputType, new RecieverBinding(inputType));
            }
        }

        public bool AddKeyBinding(Keys key, InputType inputType)
        {
            if (KeyBindings.ContainsKey(key))
            {
                return false;
            }
            KeyBindings.Add(key, inputType);
            return true;
        }

        public bool RemoveKeyBinding(Keys key)
        {
            if (KeyBindings.ContainsKey(key))
            {
                return false;
            }
            KeyBindings.Remove(key);
            return true;
        }

        public bool AddMouseBinding(MouseAction mouseAction, InputType inputType)
        {
            if (MouseBindings.ContainsKey(mouseAction))
            {
                return false;
            }
            MouseBindings.Add(mouseAction, inputType);
            return true;
        }

        public bool RemoveMouseBinding(MouseAction mouseAction)
        {
            if (MouseBindings.ContainsKey(mouseAction))
            {
                return false;
            }
            MouseBindings.Remove(mouseAction);
            return true;
        }

        // Adds an IInputReciever to recieve input on a certain keypress
        public void AddInputReciever(IInputReciever reciever, InputType input, InputState state)
        {
            recievers[input].Add(reciever, state);
        }

        public void Update()
        {
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();

            Point mousePos = currentMouse.Position;
            int scrollDelta = currentMouse.ScrollWheelValue - previousMouse.ScrollWheelValue;

            // Send keyboard input
            foreach (Keys key in KeyBindings.Keys)
            {
                InputType inputType = KeyBindings[key];
                recievers[inputType].SendInput(currentKeyboard.IsKeyDown(key), previousKeyboard.IsKeyDown(key), mousePos, scrollDelta);
            }

            // Send mouse input
            foreach (MouseAction mouseAction in MouseBindings.Keys)
            {
                InputType inputType = MouseBindings[mouseAction];
                switch (mouseAction)
                {
                    case MouseAction.RightButton:
                        recievers[inputType].SendInput(currentMouse.RightButton == ButtonState.Pressed, previousMouse.RightButton == ButtonState.Pressed, mousePos, scrollDelta);
                        break;

                    case MouseAction.LeftButton:
                        recievers[inputType].SendInput(currentMouse.LeftButton == ButtonState.Pressed, previousMouse.LeftButton == ButtonState.Pressed, mousePos, scrollDelta);
                        break;

                    case MouseAction.MiddleButton:
                        recievers[inputType].SendInput(currentMouse.MiddleButton == ButtonState.Pressed, previousMouse.MiddleButton == ButtonState.Pressed, mousePos, scrollDelta);
                        break;

                    case MouseAction.XButton1:
                        recievers[inputType].SendInput(currentMouse.XButton1 == ButtonState.Pressed, previousMouse.XButton1 == ButtonState.Pressed, mousePos, scrollDelta);
                        break;

                    case MouseAction.XButton2:
                        recievers[inputType].SendInput(currentMouse.XButton2 == ButtonState.Pressed, previousMouse.XButton2 == ButtonState.Pressed, mousePos, scrollDelta);
                        break;

                    case MouseAction.Move:
                        recievers[inputType].SendInput(currentMouse.Position != previousMouse.Position, false, mousePos, scrollDelta);
                        break;

                    case MouseAction.Scroll:
                        recievers[inputType].SendInput(currentMouse.ScrollWheelValue != previousMouse.ScrollWheelValue, false, mousePos, scrollDelta);
                        break;
                }
            }
        }
    }
}
