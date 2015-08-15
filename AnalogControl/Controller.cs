using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using SharpDX;
using SharpDX.DirectInput;
using SharpDX.XInput;
using System.Collections;

namespace AnalogControl
{

    public class GenericController
    {

        public static List<GenericController> FindControllers()
        {
            var list = new List<GenericController>();
            for (int i = 0; i < 4; i++)
            {
                if (new Controller((UserIndex)i).IsConnected)
                {
                    list.Add(new XInputController(new Controller((UserIndex)i)));
                }
            }

            return list;
        }

        public string type = "none";

        int[] mapping = new int[16];
        bool[] buttonstates = new bool[16];
        public UInt16 bitwise_buttonstates = 0x0000;

        public virtual bool Vibration(int Length)
        {
            return false;
        }

        public virtual void Update()
        {
            
        }
    }

    public class DInputController : GenericController
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

    public class XInputController : GenericController
    {
        private Controller controller;
        private State controllerstate;
        public UInt16 bitwise_buttonstates = 0x0000;
        public int[] mapping = new int[16] { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15 };

        byte lanalogbufferx = 0;
        byte lanalogbuffery = 0;
        byte ranalogbufferx = 0;
        byte ranalogbuffery = 0;
        byte ltriggerbuffer = 0;
        byte rtriggerbuffer = 0;

        public override bool Vibration(int Length)
        {
            return false;
        }

        public override void Update()
        {
            bitwise_buttonstates = 0;
            controllerstate = controller.GetState();
            //I could just directly pass the bits to the output, but this will later be used to remap things
            //changing
            for (int i = 0; i < 16; i++)
            {
                if (((UInt16)Math.Pow(2, mapping[i]) & (UInt16)controllerstate.Gamepad.Buttons) == (UInt16)Math.Pow(2, mapping[i]))
                {
                    bitwise_buttonstates |= (UInt16)Math.Pow(2, i);
                }
            }

            Console.WriteLine(bitwise_buttonstates);
            
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
    }
}
