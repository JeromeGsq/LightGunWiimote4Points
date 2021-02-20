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

using WiimoteTest.Models;
using WiimoteTest.Utils;

using PointF = WiimoteLib.PointF;

namespace WiimoteTest
{
    public partial class WiimoteInfo : UserControl
    {

        Position resolution = new Position(2560, 1440);

        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public const int KEYEVENTF_KEYUP = 0x02;
        public const int VK_ENTER = 0x57;

        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;

        private delegate void UpdateWiimoteStateDelegate(WiimoteChangedEventArgs args);
        private delegate void UpdateExtensionChangedDelegate(WiimoteExtensionChangedEventArgs args);

        private Bitmap b = new Bitmap(1024, 768, PixelFormat.Format24bppRgb);
        private Graphics g;
        private Wiimote mWiimote;

        public WiimoteInfo()
        {
            InitializeComponent();
            g = Graphics.FromImage(b);
        }

        public WiimoteInfo(Wiimote wm) : this()
        {
            mWiimote = wm;
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
            LeftMouseClick(ws.ButtonState.B);
            RightMouseClick(ws.ButtonState.A);
            DownMouseClick(ws.ButtonState.Down);
            EnterKey(ws.ButtonState.Plus);

            clbButtons.SetItemChecked(0, ws.ButtonState.A);
            clbButtons.SetItemChecked(1, ws.ButtonState.B);
            clbButtons.SetItemChecked(2, ws.ButtonState.Minus);
            clbButtons.SetItemChecked(3, ws.ButtonState.Home);
            clbButtons.SetItemChecked(4, ws.ButtonState.Plus);
            clbButtons.SetItemChecked(5, ws.ButtonState.One);
            clbButtons.SetItemChecked(6, ws.ButtonState.Two);
            clbButtons.SetItemChecked(7, ws.ButtonState.Up);
            clbButtons.SetItemChecked(8, ws.ButtonState.Down);
            clbButtons.SetItemChecked(9, ws.ButtonState.Left);
            clbButtons.SetItemChecked(10, ws.ButtonState.Right);

            switch (ws.ExtensionType)
            {
                case ExtensionType.Nunchuk:
                    break;

                case ExtensionType.ClassicController:
                    break;

                case ExtensionType.Guitar:
                    break;

                case ExtensionType.Drums:
                    break;

                case ExtensionType.BalanceBoard:

                    break;
            }

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

                /*
                 * With two sensors only 
                 * 
                double x1 = 0.5 - corners[0].X;
                double x2 = corners[1].X - corners[0].X;
                double percentX = x1 / x2;

                double y1 = 0.5 - corners[0].Y;
                double y2 = corners[3].Y - corners[0].Y;
                double percentY = y1 / y2;

                percentX = percentX - 0.5;
                percentX = (resolution.X / 2) + (resolution.X * (percentX / 2.7));

                SetCursorPos((int)(percentX), (int)(resolution.Y * (1 - percentY)));
                */
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

        bool tempPressedLeft = false;
        public void LeftMouseClick(bool pressed)
        {
            if (pressed != tempPressedLeft)
            {
                uint X = (uint)Cursor.Position.X;
                uint Y = (uint)Cursor.Position.Y;
                if (pressed)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
                }
                else
                {
                    mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                }
                tempPressedLeft = pressed;
            }
        }

        bool tempPressedRight = false;
        public void RightMouseClick(bool pressed)
        {
            if (pressed != tempPressedRight)
            {
                uint X = (uint)Cursor.Position.X;
                uint Y = (uint)Cursor.Position.Y;
                if (pressed)
                {
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, X, Y, 0, 0);
                }
                else
                {
                    mouse_event(MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
                }
                tempPressedRight = pressed;
            }
        }

        bool tempPressedDown = false;
        public void DownMouseClick(bool pressed)
        {
            if (pressed != tempPressedDown)
            {
                uint X = (uint)Cursor.Position.X;
                uint Y = (uint)Cursor.Position.Y;
                if (pressed)
                {
                    mouse_event(MOUSEEVENTF_MIDDLEDOWN, X, Y, 0, 0);
                }
                else
                {
                    mouse_event(MOUSEEVENTF_MIDDLEUP, X, Y, 0, 0);
                }
                tempPressedDown = pressed;
            }
        }

        bool tempPressedPlus = false;
        public void EnterKey(bool pressed)
        {
            if (pressed != tempPressedPlus)
            {
                if (pressed)
                {
                    keybd_event((byte)Keys.Return, 0, 0, 0);
                }
                else
                {
                    keybd_event((byte)Keys.Return, 0, KEYEVENTF_KEYUP, 0);
                }
                tempPressedPlus = pressed;
            }
        }
        private void UpdateIR(IRSensor irSensor, Label lblNorm, Label lblRaw, CheckBox chkFound, Color color)
        {
            chkFound.Checked = irSensor.Found;

            if (irSensor.Found)
            {
                lblNorm.Text = irSensor.Position.ToString() + ", " + irSensor.Size;
                lblRaw.Text = irSensor.RawPosition.ToString();

                g.DrawEllipse(new Pen(color), (int)(1024 / 8), (int)(768 / 8), irSensor.Size + 1, irSensor.Size + 1);
            }
        }

        private void UpdateExtensionChanged(WiimoteExtensionChangedEventArgs args)
        {

        }

        public Wiimote Wiimote
        {
            set { mWiimote = value; }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
