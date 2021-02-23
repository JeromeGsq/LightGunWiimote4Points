//////////////////////////////////////////////////////////////////////////////////
//	MultipleWiimoteForm.cs
//	Managed Wiimote Library Tester
//	Written by Brian Peek (http://www.brianpeek.com/)
//  for MSDN's Coding4Fun (http://msdn.microsoft.com/coding4fun/)
//	Visit http://blogs.msdn.com/coding4fun/archive/2007/03/14/1879033.aspx
//  and http://www.codeplex.com/WiimoteLib
//  for more information
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using WiimoteLib;
using vJoyInterfaceWrap;
using WiimoteTest.Models;
using WiimoteTest.Utils;

using PointF = WiimoteLib.PointF;

namespace WiimoteTest
{
    public partial class WiimoteInfo : UserControl
    {
        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        static public vJoy joystick;
        static public vJoy.JoystickState iReport;
        static public uint id = 1;

        Position resolution = new Position(2560, 1440);

        private delegate void UpdateWiimoteStateDelegate(WiimoteChangedEventArgs args);
        private delegate void UpdateExtensionChangedDelegate(WiimoteExtensionChangedEventArgs args);

        private Bitmap b = new Bitmap(1024, 768, PixelFormat.Format24bppRgb);
        private Graphics g;

        public WiimoteInfo()
        {
            InitializeComponent();
            g = Graphics.FromImage(b);

            joystick = new vJoy();
            iReport = new vJoy.JoystickState();
            joystick.ResetVJD(id);
            joystick.AcquireVJD(id);
            joystick.ResetAll();
        }

        public WiimoteInfo(Wiimote wm) : this()
        {
        }

        public void UpdateState(WiimoteChangedEventArgs args)
        {
            BeginInvoke(new UpdateWiimoteStateDelegate(UpdateWiimoteChanged), args);
        }

        public void UpdateExtension(WiimoteExtensionChangedEventArgs args)
        {
            BeginInvoke(new UpdateExtensionChangedDelegate(UpdateExtensionChanged), args);
        }

        private void chkLED_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void chkRumble_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void UpdateWiimoteChanged(WiimoteChangedEventArgs args)
        {
            WiimoteState ws = args.WiimoteState;

            double a = Math.Pow(2, 15);
            joystick.SetBtn(ws.ButtonState.A, 1, 1);
            joystick.SetBtn(ws.ButtonState.B, 1, 2);
            joystick.SetBtn(ws.ButtonState.Up, 1, 3);
            joystick.SetBtn(ws.ButtonState.Down, 1, 4);
            joystick.SetBtn(ws.ButtonState.Left, 1, 5);
            joystick.SetBtn(ws.ButtonState.Right, 1, 6);
            joystick.SetBtn(ws.ButtonState.Plus, 1, 7);
            joystick.SetBtn(ws.ButtonState.Minus, 1, 8);
            joystick.SetBtn(ws.ButtonState.Home, 1, 9);
            joystick.SetBtn(ws.ButtonState.One, 1, 10); ;
            joystick.SetBtn(ws.ButtonState.Two, 1, 11); ;

            joystick.SetBtn(ws.NunchukState.Z, 1, 12);
            joystick.SetBtn(ws.NunchukState.C, 1, 13);

            joystick.SetAxis((int)(a * (0.5f + ws.NunchukState.Joystick.X)), 1, HID_USAGES.HID_USAGE_RX);
            joystick.SetAxis((int)(a * (0.5f + ws.NunchukState.Joystick.Y)), 1, HID_USAGES.HID_USAGE_RY);

            var corners = MathUtils.Sort(new PointF[4] {
                ws.IRState.IRSensors[0].Position,
                ws.IRState.IRSensors[1].Position,
                ws.IRState.IRSensors[2].Position,
                ws.IRState.IRSensors[3].Position
            });

            if (ws.IRState.IRSensors[0].Found && ws.IRState.IRSensors[1].Found && ws.IRState.IRSensors[2].Found && ws.IRState.IRSensors[3].Found)
            {

                double x1 = 0.5 - corners[0].X;
                double x2 = corners[1].X - corners[0].X;
                double percentX = x1 / x2;

                double y1 = 0.5 - corners[0].Y;
                double y2 = corners[3].Y - corners[0].Y;
                double percentY = y1 / y2;

                SetCursorPos((int)(resolution.X * percentX), (int)(resolution.Y * (1 - percentY)));

                joystick.SetAxis((int)(a * percentX), 1, HID_USAGES.HID_USAGE_X);
                joystick.SetAxis((int)(a * (1 - percentY)), 1, HID_USAGES.HID_USAGE_Y);
            }

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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
