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

        public int[] mapping = new int[16];
        public UInt16 buttonstates = 0x0000;
        public byte[] axes;
        public byte[] datagram;
        public bool changed;

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
        private byte[] previous_datagram;

        public override bool Vibration(int Length)
        {
            return false;
        }

        int changecounter = 0;
        public override void Update()
        {
            changed = false;
            previous_datagram = datagram;
            datagram = new byte[9];

            buttonstates = 0;
            controllerstate = controller.GetState();
            //instead of directly passing the button states, which would be easy, we're filtering them to remap the output.
            //"mapping" array tells you how each button will be assigned.
            
            
            for (int i = 0; i < 16; i++)
            {
                if (((UInt16)Math.Pow(2, mapping[i]) & (UInt16)controllerstate.Gamepad.Buttons) == (UInt16)Math.Pow(2, mapping[i]))
                {
                    buttonstates |= (UInt16)Math.Pow(2, i);
                }
            }
            try
            {
                datagram[0] = 141;
                datagram[1] = (byte)((controllerstate.Gamepad.LeftThumbX + 32768) / 256);
                datagram[2] = (byte)((controllerstate.Gamepad.LeftThumbY + 32768) / 256);

                datagram[3] = (byte)((controllerstate.Gamepad.RightThumbX + 32768) / 256);
                datagram[4] = (byte)((controllerstate.Gamepad.RightThumbY + 32768) / 256);

                datagram[5] = (byte)((controllerstate.Gamepad.LeftTrigger) / 2);
                datagram[6] = (byte)((controllerstate.Gamepad.RightTrigger) / 2);
            }
            catch
            {

            }
            datagram[7] = BitConverter.GetBytes(buttonstates)[0];
            datagram[8] = BitConverter.GetBytes(buttonstates)[1];

            for (int i = 0; i < datagram.Length; i++)
            {
                if (datagram[i] != previous_datagram[i])
                {
                    changed = true;
                    break;
                }
            }
        }

        public XInputController(Controller xinputcontroller)
        {
            changed = true;
            type = "XInput";
            controller = xinputcontroller;
            buttonstates = 0x0000;
            mapping = new int[16] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            axes = new byte[6];

            datagram = new byte[8];
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
