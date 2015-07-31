using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using SharpDX;
using SharpDX.DirectInput;
using SharpDX.XInput;

namespace AnalogControl
{
    class GenericController
    {
        public string type = "none";
        public virtual bool Vibration(int Length)
        {
            return false;
        }

        public virtual void Update()
        {
            
        }
    }

    class DInputController : GenericController
    {
        private Joystick controller;
        private JoystickState controllerstate;

        public override bool Vibration(int Length)
        {
            return false;
        }

        public override void Update()
        {
            controller.GetCurrentState(ref controllerstate);
            
        }

        public DInputController(Joystick dinputjoystick)
        {
            type = "dinput";
            controller = dinputjoystick;
            controller.Acquire();
            controller.GetCurrentState(ref controllerstate);
        }   
    }

    class XInputController : GenericController
    {
        private Controller controller;
        private State controllerstate;

        private int A = 1;
        private int B = 2;
        private int X = 3;
        private int Y = 4;
        private int L = 5;
        private int R = 6;
        private int Select = 7;
        private int Start = 8;
        private int LS = 9;
        private int RS = 10;
        
        private int Up = 4;
        private int Down = 5;
        private int Left = 6;
        private int Right = 7;
        
        public override bool Vibration(int Length)
        {
            return false;
        }

        public override void Update()
        {
            controllerstate = controller.GetState();

        }

        public XInputController(Controller xinputcontroller)
        {
            type = "xinput";
            controller = xinputcontroller;
            controllerstate = controller.GetState();
        }
    }

    class VibrationEffect
    {
        public Vector2[] values;
        public int counter = 0;
        public VibrationEffect(int length)
        {
            values = new Vector2[length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = new Vector2(1,1);
            }
        }

        public VibrationEffect(Vector2[] in_values)
        {
            values = in_values;
        }

        public VibrationEffect()
        {
            
        }
    }

    class ShortGunShot : VibrationEffect
    {
        public ShortGunShot()
        {
            values = new Vector2[]
            {
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1)
            };
        }
    }

    class LongGunShot : VibrationEffect
    {
        public LongGunShot()
        {
            values = new Vector2[]
            {
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1),
                new Vector2(1,1)
            };
        }
    }
}
