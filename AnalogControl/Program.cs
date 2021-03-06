﻿using System;
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
        static List<Controller> xinputcontrollers = new List<Controller>();
        static public int XInpPlayerIndex = -1;

        public static UInt16 Port = 0;

        public static IPAddress outaddress;


        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            MainWindow TheMainForm = new MainWindow();

            for (int i = 0; i <= 3; i++)
            {
                if (new Controller((UserIndex)i).IsConnected)
                {
                    xinputcontrollers.Add(new Controller((UserIndex)i));
                    TheMainForm.AddItemToComboBox(String.Format("Xinput Controller (Player Index {0})", i+1));
                }
            }

            Application.Run(TheMainForm);

            Controller xinp = xinputcontrollers[XInpPlayerIndex];

            var P1VibrationOutput = new Vibration();
            
            List<VibrationEffect> ActiveEffects = new List<VibrationEffect>();

            var di = new DirectInput();
            var devices = di.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
            IPEndPoint ip = new IPEndPoint(outaddress, Port);
            var udp = new UdpClient(ip);

            var udpbuf = new byte[1];

            var lanalogbufferx = 0;
            var lanalogbuffery = 0;
            var ranalogbufferx = 0;
            var ranalogbuffery = 0;
            var ltriggerbuffer = 0;
            var rtriggerbuffer = 0;
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

            Console.WriteLine("AnaLua Controller Server (Generic Version 0.01)");
            Console.WriteLine("Sends input from the first connected controller to localhost:3478");
            Console.WriteLine("And receives force feedback commands on the same port.");

            while (true)
            {

                try
                {
                    lanalogbufferx = (xinp.GetState().Gamepad.LeftThumbX + 32768) / 256;
                    lanalogbuffery = (xinp.GetState().Gamepad.LeftThumbY + 32768) / 256;

                    ranalogbufferx = (xinp.GetState().Gamepad.RightThumbX + 32768) / 256;
                    ranalogbuffery = (xinp.GetState().Gamepad.RightThumbY + 32768) / 256;

                    ltriggerbuffer = (xinp.GetState().Gamepad.LeftTrigger) / 2;
                    rtriggerbuffer = (xinp.GetState().Gamepad.RightTrigger) / 2;

                    xinp.SetVibration(P1VibrationOutput);
                }
                catch
                {
                    
                }

                Console.WriteLine("LS - X: {0}, Y: {0} -- ", lanalogbufferx, lanalogbuffery);
                Console.WriteLine("RS - X: {0}, Y: {0}", ranalogbufferx, ranalogbuffery);
                Console.CursorTop-=2;

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




                //if (udpbuf[0].ToString())
                {
                    //Console.WriteLine("WORLD WAR ONE");
                }

                //00000000
                //ABXYNSWE
                //LRZZCCSS

                sendbuffer = new byte[9];

                sendbuffer[0] = 141;
                sendbuffer[1] = (byte)lanalogbufferx;
                sendbuffer[2] = (byte)lanalogbuffery;
                sendbuffer[3] = (byte)ranalogbufferx;
                sendbuffer[4] = (byte)ranalogbuffery;
                sendbuffer[5] = (byte)ltriggerbuffer;
                sendbuffer[6] = (byte)rtriggerbuffer;

                if (udp.Send(sendbuffer, 9, ip) > 0)
                {
                    //Console.WriteLine("Sent");
                }

                Thread.Sleep(8);
            }
        }
    }
}