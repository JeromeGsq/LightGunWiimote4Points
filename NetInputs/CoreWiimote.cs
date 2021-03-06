﻿using LightGunWiimote4Points.Models;
using LightGunWiimote4Points.Utils;
using System;
using System.Runtime.InteropServices;
using vJoyInterfaceWrap;
using WiimoteLib;

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

        double stickResolution = Math.Pow(2, 15);
        double percentX = 0;
        double percentY = 0;
        #endregion

        #region Events
        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);
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
            wm.WiimoteChanged += WiimoteChanged;
            wm.WiimoteExtensionChanged += WiimoteExtensionChanged;

            wm.SetReportType(InputReport.IRExtensionAccel, true);
            wm.SetLEDs(index == 1, index == 2, index == 3, index == 4);

            joystick = new vJoy();
            joystick.AcquireVJD(index);

            wm.Connect();
        }

        private void WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            ws = args.WiimoteState;

            corners = MathUtils.Sort(new PointF[4] {
                ws.IRState.IRSensors[0].Position,
                ws.IRState.IRSensors[1].Position,
                ws.IRState.IRSensors[2].Position,
                ws.IRState.IRSensors[3].Position
            });

            if (ws.IRState.IRSensors[0].Found && ws.IRState.IRSensors[1].Found && ws.IRState.IRSensors[2].Found && ws.IRState.IRSensors[3].Found)
            {
                percentX = (0.5 - corners[0].X) / (corners[1].X - corners[0].X);
                percentY = (0.5 - corners[0].Y) / (corners[3].Y - corners[0].Y);

                joystick.SetAxis((int)(stickResolution * percentX), index, HID_USAGES.HID_USAGE_X);
                joystick.SetAxis((int)(stickResolution * (1 - percentY)), index, HID_USAGES.HID_USAGE_Y);

                if (index == 1)
                {
                    SetCursorPos((int)(resolution.X * percentX), (int)(resolution.Y * (1 - percentY)));
                }
            }

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

            joystick.SetAxis((int)(stickResolution * (0.5f + ws.NunchukState.Joystick.X)), index, HID_USAGES.HID_USAGE_RX);
            joystick.SetAxis((int)(stickResolution * (0.5f + ws.NunchukState.Joystick.Y)), index, HID_USAGES.HID_USAGE_RY);
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
