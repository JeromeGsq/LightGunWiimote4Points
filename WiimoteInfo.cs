using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WiimoteLib;
using vJoyInterfaceWrap;
using LightGunWiimote4Points.Models;
using LightGunWiimote4Points.Utils;
using PointF = WiimoteLib.PointF;

namespace LightGunWiimote4Points
{
    public partial class WiimoteInfo : UserControl
    {
        Position resolution = new Position(2560, 1440);
        bool moveMouseWithIR = true;

        public vJoy joystick;
        public vJoy.JoystickState iReport;

        #region vars
        private Bitmap b = new Bitmap(1024, 768, PixelFormat.Format24bppRgb);
        private Graphics g;

        double stickResolution = Math.Pow(2, 15);
        double percentX = 0;
        double percentY = 0;
        #endregion

        #region Events
        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        private delegate void UpdateWiimoteStateDelegate(WiimoteChangedEventArgs args);
        private delegate void UpdateExtensionChangedDelegate(WiimoteExtensionChangedEventArgs args);

        public void UpdateState(WiimoteChangedEventArgs args) => BeginInvoke(new UpdateWiimoteStateDelegate(UpdateWiimoteChanged), args);
        public void UpdateExtension(WiimoteExtensionChangedEventArgs args) => BeginInvoke(new UpdateExtensionChangedDelegate(UpdateExtensionChanged), args);
        #endregion

        public WiimoteInfo()
        {
            InitializeComponent();
            g = Graphics.FromImage(b);

            joystick = new vJoy();
            iReport = new vJoy.JoystickState();
            joystick.AcquireVJD(1);
            joystick.AcquireVJD(2);
        }

        private void UpdateWiimoteChanged(WiimoteChangedEventArgs args)
        {
            WiimoteState ws = args.WiimoteState;
            var corners = MathUtils.Sort(new PointF[4] {
                ws.IRState.IRSensors[0].Position,
                ws.IRState.IRSensors[1].Position,
                ws.IRState.IRSensors[2].Position,
                ws.IRState.IRSensors[3].Position
            });

            UpdateInputs(ws, corners);
            UpdateUI(ws, corners);
        }

        private void UpdateInputs(WiimoteState ws, Position[] corners)
        {
            if (ws.IRState.IRSensors[0].Found && ws.IRState.IRSensors[1].Found && ws.IRState.IRSensors[2].Found && ws.IRState.IRSensors[3].Found)
            {
                percentX = (0.5 - corners[0].X) / (corners[1].X - corners[0].X);
                percentY = (0.5 - corners[0].Y) / (corners[3].Y - corners[0].Y);

                if (moveMouseWithIR)
                {
                    SetCursorPos((int)(resolution.X * percentX), (int)(resolution.Y * (1 - percentY)));
                }
            }

            for (uint i = 1; i <= 2; i++)
            {
                joystick.SetBtn(ws.ButtonState.A, i, 1);
                joystick.SetBtn(ws.ButtonState.B, i, 2);
                joystick.SetBtn(ws.ButtonState.Up, i, 3);
                joystick.SetBtn(ws.ButtonState.Down, i, 4);
                joystick.SetBtn(ws.ButtonState.Left, i, 5);
                joystick.SetBtn(ws.ButtonState.Right, i, 6);
                joystick.SetBtn(ws.ButtonState.Plus, i, 7);
                joystick.SetBtn(ws.ButtonState.Minus, i, 8);
                joystick.SetBtn(ws.ButtonState.Home, i, 9);
                joystick.SetBtn(ws.ButtonState.One, i, 10);
                joystick.SetBtn(ws.ButtonState.Two, i, 11);

                joystick.SetBtn(ws.NunchukState.Z, i, 12);
                joystick.SetBtn(ws.NunchukState.C, i, 13);

                if (ws.IRState.IRSensors[0].Found && ws.IRState.IRSensors[1].Found && ws.IRState.IRSensors[2].Found && ws.IRState.IRSensors[3].Found)
                {
                    joystick.SetAxis((int)(stickResolution * percentX), i, HID_USAGES.HID_USAGE_X);
                    joystick.SetAxis((int)(stickResolution * (1 - percentY)), i, HID_USAGES.HID_USAGE_Y);
                }

                joystick.SetAxis((int)(stickResolution * (0.5f + ws.NunchukState.Joystick.X)), i, HID_USAGES.HID_USAGE_RX);
                joystick.SetAxis((int)(stickResolution * (0.5f + ws.NunchukState.Joystick.Y)), i, HID_USAGES.HID_USAGE_RY);
            }
        }

        private void UpdateUI(WiimoteState ws, Position[] corners)
        {
            g.Clear(Color.Black);

            // Center target
            g.DrawEllipse(new Pen(Color.Green), (int)(1024 / 2), (int)(768 / 2), 20, 20);

            if (ws.IRState.IRSensors[0].Found && ws.IRState.IRSensors[1].Found && ws.IRState.IRSensors[2].Found && ws.IRState.IRSensors[3].Found)
            {
                g.DrawLine(new Pen(Color.Purple), (int)(1024 * corners[0].X), (int)(768 * corners[0].Y), (int)(1024 * corners[1].X), (int)(768 * corners[1].Y));
                g.DrawLine(new Pen(Color.Green), (int)(1024 * corners[1].X), (int)(768 * corners[1].Y), (int)(1024 * corners[2].X), (int)(768 * corners[2].Y));
                g.DrawLine(new Pen(Color.Cyan), (int)(1024 * corners[2].X), (int)(768 * corners[2].Y), (int)(1024 * corners[3].X), (int)(768 * corners[3].Y));
                g.DrawLine(new Pen(Color.Yellow), (int)(1024 * corners[3].X), (int)(768 * corners[3].Y), (int)(1024 * corners[0].X), (int)(768 * corners[0].Y));

                g.DrawLine(new Pen(Color.WhiteSmoke), (int)(1024 * corners[0].X), (int)(768 * corners[0].Y), (int)(1024 * corners[2].X), (int)(768 * corners[2].Y));
                g.DrawLine(new Pen(Color.WhiteSmoke), (int)(1024 * corners[1].X), (int)(768 * corners[1].Y), (int)(1024 * corners[3].X), (int)(768 * corners[3].Y));

                g.DrawEllipse(new Pen(Color.White), (int)(1024 * corners[0].X), (int)(768 * corners[0].Y), 5, 5);
                g.DrawEllipse(new Pen(Color.White), (int)(1024 * corners[1].X), (int)(768 * corners[1].Y), 5, 5);
                g.DrawEllipse(new Pen(Color.White), (int)(1024 * corners[2].X), (int)(768 * corners[2].Y), 5, 5);
                g.DrawEllipse(new Pen(Color.White), (int)(1024 * corners[3].X), (int)(768 * corners[3].Y), 5, 5);
            }

            pbIR.Image = b;
        }

        private void UpdateExtensionChanged(WiimoteExtensionChangedEventArgs args)
        {
        }
    }
}
