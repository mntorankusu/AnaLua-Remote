using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SharpDX.DirectInput;
using SharpDX.XInput;

namespace AnalogControl
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles(); 

            //Application.Run(new Form());

            var xinp = new Controller(UserIndex.One);

            var P1VibrationOutput = new Vibration();

            List<VibrationEffect> ActiveEffects = new List<VibrationEffect>();

            var di = new DirectInput();
            var devices = di.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
            var ip = new IPEndPoint(IPAddress.Loopback, 3478);
            var udp = new UdpClient(ip);

            foreach (var dev in devices)
            {
                Console.WriteLine(dev.ProductName);
            }
            var udpbuf = new byte[1];

            var js = new Joystick(di, devices[0].InstanceGuid);
            var jss = new JoystickState();

            var lanalogbufferx = 0;
            var lanalogbuffery = 0;
            var ranalogbufferx = 0;
            var ranalogbuffery = 0;
            var buttons1 = Convert.ToByte("00000000", 2);
            var buttons2 = Convert.ToByte("00000000", 2);

            var sendbuffer = new byte[16];
            //ANN - LSX - LSY - RSX - RSY - ACC - BRK - BT1 - BT2
            //[0] announce is 141 for P1 and 142 for P2
            //[1] Left Stick X / Wheel
            //[2] Left Stick Y 
            //[3] Right Stick X
            //[4] Right Stick Y
            //[5] Left Trigger / Accelerator
            //[6] Right Trigger / Brake
            //[7] Buttons 1
            //[8] Buttons 2
            //[9] Buttons 3
            //[A] Buttons 4
            //[B-C] Mouse X (screen coordinates)
            //[D-E] Mouse Y (screen coordinates)

            //Buttons are A, B, X, Y, Up, Down, Left, Right
            //L, R, LT, RT, LS, RS, Select, Start
            //Mouse Left, Mouse Right, Mouse Middle, Mouse scroll up, Mouse scroll down, X1, X2

            js.Acquire();

            Console.WriteLine(js.GetEffects().Count);

            js.GetCurrentState(ref jss);

            Console.WriteLine("AnaLua Controller Server (Generic Version 0.01)");
            Console.WriteLine("Sends input from the first connected controller to localhost:3478");
            Console.WriteLine("And receives force feedback commands on the same port.");

            while (true)
            {
                js.GetCurrentState(ref jss);

                lanalogbufferx = (jss.X*255)/65500;
                lanalogbuffery = (jss.Y*255)/65500;

                ranalogbufferx = (jss.RotationX*255)/65500;
                ranalogbuffery = (jss.RotationY*255)/65500;

                for(int i = 0; i < ActiveEffects.Count; i++)
                {
                    Vibration vib = new Vibration();
                    try
                    {
                        vib.LeftMotorSpeed = (ushort) (ActiveEffects[i].values[ActiveEffects[i].counter].X*65535);
                    }
                    catch
                    {
                    }
                    xinp.SetVibration(vib);
                    ActiveEffects[i].counter++;
                }

                for (int i = 0; i < ActiveEffects.Count; i++)
                {
                    if (ActiveEffects[i].counter > ActiveEffects[i].values.Length)
                    {
                        ActiveEffects.RemoveAt(i);
                    }
                }

                

                //Console.WriteLine("LS - X: {0}, Y: {0} -- ", lanalogbufferx, lanalogbuffery);
                //Console.Write("RS - X: {0}, Y: {0}", ranalogbufferx, ranalogbuffery);

                P1VibrationOutput.LeftMotorSpeed = 0;
                P1VibrationOutput.RightMotorSpeed = 0;
                while (udp.Available > 0)
                {
                    try
                    {
                        udpbuf = udp.Receive(ref ip);
                    }
                    catch
                    {
                        udpbuf = new byte[0];
                    }
                    if (Encoding.ASCII.GetString(udpbuf) == "r1")
                    {
                        ActiveEffects.Add(new ShortGunShot());
                        Console.WriteLine("Vibration");
                    }
                    else if (Encoding.ASCII.GetString(udpbuf) == "r2")
                    {
                        ActiveEffects.Add(new LongGunShot());
                        Console.WriteLine("Good Vibration");
                    }
                    else
                    {
                        
                    }
                }
                xinp.SetVibration(P1VibrationOutput);


                //if (udpbuf[0].ToString())
                {
                    //Console.WriteLine("WORLD WAR ONE");
                }

                //00000000
                //ABXYNSWE
                //LRZZCCSS

                sendbuffer = new byte[9];

                sendbuffer[0] = 141;
                sendbuffer[1] = (byte) lanalogbufferx;
                sendbuffer[2] = (byte) lanalogbuffery;
                sendbuffer[3] = (byte) ranalogbufferx;
                sendbuffer[4] = (byte) ranalogbuffery;

                if (udp.Send(sendbuffer, 9, ip) > 0)
                {
                    //Console.WriteLine("Sent");
                }

                Thread.Sleep(8);
            }
        }
    }
}