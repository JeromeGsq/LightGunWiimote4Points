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
