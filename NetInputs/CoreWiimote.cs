using LightGunWiimote4Points.Models;
using LightGunWiimote4Points.Utils;
using System;
using System.IO;
using System.Runtime.InteropServices;
using vJoyInterfaceWrap;
using WiimoteLib;
using System.Net.Sockets;

namespace LightGunWiimote4Points
{
    public class CoreWiimote : IDisposable
    {
        #region Vars
        private Wiimote wm;
        private uint index;

        private WiimoteState ws;
        private Position[] corners;
        private vJoy joystick;

        double stickResolution = Math.Pow(2, 15);
        double percentX = 0;
        double percentY = 0;
        #endregion

        public CoreWiimote(Wiimote wm, int index, Position resolution)
        {
            this.wm = wm;
            this.index = (uint)index;

            Initialize();
        }

        public void Initialize()
        {
            wm.Connect();

            wm.WiimoteChanged += WiimoteChanged;
            wm.WiimoteExtensionChanged += WiimoteExtensionChanged;

            wm.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.WiiLevel5, true);
            wm.SetLEDs(index == 1, index == 2, index == 3, index == 4);


            joystick = new vJoy();
            VjdStat status = joystick.GetVJDStatus(index);

            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    Console.WriteLine("vJoy Device {0} is already owned by this feeder\n", index);
                    break;
                case VjdStat.VJD_STAT_FREE:
                    Console.WriteLine("vJoy Device {0} is free\n", index);
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    Console.WriteLine("vJoy Device {0} is already owned by another feeder\nCannot continue\n", index);
                    return;
                case VjdStat.VJD_STAT_MISS:
                    Console.WriteLine("vJoy Device {0} is not installed or disabled\nCannot continue\n", index);
                    return;
                default:
                    Console.WriteLine("vJoy Device {0} general error\nCannot continue\n", index);
                    return;
            };

            joystick.AcquireVJD(index);
        }

        private void WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            ws = args.WiimoteState;

            corners = MathUtils.Sort(ws.IRState.IRSensors);

            percentX = (0.5 - corners[2].X) / (corners[3].X - corners[2].X);
            percentY = (0.5 - corners[0].Y) / (corners[1].Y - corners[0].Y);

            if (joystick != null)
            {
                joystick.SetAxis((int)(stickResolution * percentX), index, HID_USAGES.HID_USAGE_X);
                joystick.SetAxis((int)(stickResolution * (1 - percentY)), index, HID_USAGES.HID_USAGE_Y);

                joystick.SetBtn(ws.ButtonState.A, index, 1);
                joystick.SetBtn(ws.ButtonState.B, index, 2);
                joystick.SetBtn(ws.ButtonState.Up, index, 3);
                joystick.SetBtn(ws.ButtonState.Down, index, 4);
                joystick.SetBtn(ws.ButtonState.Left, index, 5);
                joystick.SetBtn(ws.ButtonState.Right, index, 6);
                joystick.SetBtn(ws.ButtonState.Plus, index, 7);
                joystick.SetBtn(ws.ButtonState.Minus, index, 8);
                joystick.SetBtn(ws.ButtonState.Home, index, 9);
                joystick.SetBtn(ws.ButtonState.One, index, 10);
                joystick.SetBtn(ws.ButtonState.Two, index, 11);

                joystick.SetBtn(ws.NunchukState.Z, index, 12);
                joystick.SetBtn(ws.NunchukState.C, index, 13);

                // joystick.SetAxis((int)(stickResolution * (0.5f + ws.NunchukState.Joystick.X)), index, HID_USAGES.HID_USAGE_RX);
                // joystick.SetAxis((int)(stickResolution * (0.5f + ws.NunchukState.Joystick.Y)), index, HID_USAGES.HID_USAGE_RY);

                
                joystick.SetBtn(0.5f + ws.NunchukState.Joystick.X > 0.75f, index, 14);
                joystick.SetBtn(0.5f + ws.NunchukState.Joystick.X < 0.25f, index, 15);

                joystick.SetBtn(0.5f + ws.NunchukState.Joystick.Y > 0.75f, index, 16);
                joystick.SetBtn(0.5f + ws.NunchukState.Joystick.Y < 0.25f, index, 17);

            }


            wm.SetRumble(ws.ButtonState.B);

            if (index == 1)
            {
                string value = "";
                for (int i = 0; i <= 3; i++)
                {
                    value += corners[i].X + ":" + corners[i].Y + "\n";
                }
                Program.SendMessage(value);
            }
        }

        private void WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs args)
        {
            wm.SetReportType(args.Inserted ? InputReport.IRExtensionAccel : InputReport.IRAccel, true);
        }

        public void Dispose()
        {
            wm.SetLEDs(false, false, false, false);
            wm.Disconnect();
        }
    }
}
