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
        private Position resolution;
        private uint index;

        private WiimoteState ws;
        private Position[] corners;
        private vJoy joystick;

        private bool mouseTracking = true;

        double stickResolution = Math.Pow(2, 15);
        double percentX = 0;
        double percentY = 0;
        #endregion

        #region Events
        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        #endregion

        public CoreWiimote(Wiimote wm, int index, Position resolution)
        {
            this.wm = wm;
            this.index = (uint)index;
            this.resolution = resolution;
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
            joystick.AcquireVJD(index);

        }

        bool mouseDown = false;

        private void WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            ws = args.WiimoteState;
            mouseTracking = !ws.ButtonState.Home;

            corners = MathUtils.Sort(ws.IRState.IRSensors);

            percentX = (0.5 - corners[2].X) / (corners[3].X - corners[2].X);
            percentY = (0.5 - corners[0].Y) / (corners[1].Y - corners[0].Y);

            joystick?.SetAxis((int)(stickResolution * percentX), index, HID_USAGES.HID_USAGE_X);
            joystick?.SetAxis((int)(stickResolution * (1 - percentY)), index, HID_USAGES.HID_USAGE_Y);

            if (mouseTracking)
            {
                if (ws.IRState.IRSensors[0].Found)
                {
                    if (index == 1 && mouseTracking)
                    {
                        SetCursorPos((int)(resolution.X * percentX), (int)(resolution.Y * (1 - percentY)));
                    }
                }

                if (mouseDown && ws.ButtonState.B == false)
                {
                    mouse_event(MOUSEEVENTF_LEFTUP, (uint)(stickResolution * percentX), (uint)(stickResolution * (1 - percentY)), 0, 0);
                    mouseDown = false;
                }

                if (mouseDown == false && ws.ButtonState.B)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)(stickResolution * percentX), (uint)(stickResolution * (1 - percentY)), 0, 0);
                    mouseDown = true;
                }
            }

            joystick?.SetBtn(ws.ButtonState.A, index, 1);
            joystick?.SetBtn(ws.ButtonState.B, index, 2);
            joystick?.SetBtn(ws.ButtonState.Up, index, 3);
            joystick?.SetBtn(ws.ButtonState.Down, index, 4);
            joystick?.SetBtn(ws.ButtonState.Left, index, 5);
            joystick?.SetBtn(ws.ButtonState.Right, index, 6);
            joystick?.SetBtn(ws.ButtonState.Plus, index, 7);
            joystick?.SetBtn(ws.ButtonState.Minus, index, 8);
            joystick?.SetBtn(ws.ButtonState.Home, index, 9);
            joystick?.SetBtn(ws.ButtonState.One, index, 10);
            joystick?.SetBtn(ws.ButtonState.Two, index, 11);

            joystick?.SetBtn(ws.NunchukState.Z, index, 12);
            joystick?.SetBtn(ws.NunchukState.C, index, 13);

            joystick?.SetAxis((int)(stickResolution * (0.5f + ws.NunchukState.Joystick.X)), index, HID_USAGES.HID_USAGE_RX);
            joystick?.SetAxis((int)(stickResolution * (0.5f + ws.NunchukState.Joystick.Y)), index, HID_USAGES.HID_USAGE_RY);

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
