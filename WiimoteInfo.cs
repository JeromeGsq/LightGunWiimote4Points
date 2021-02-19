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
using WiimoteTest.Utils;
using PointF = WiimoteLib.PointF;

namespace WiimoteTest
{
    public partial class WiimoteInfo : UserControl
    {

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

        private Bitmap b = new Bitmap(256, 192, PixelFormat.Format24bppRgb);
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
            mWiimote.SetLEDs(chkLED1.Checked, chkLED2.Checked, chkLED3.Checked, chkLED4.Checked);
        }

        private void chkRumble_CheckedChanged(object sender, EventArgs e)
        {
            mWiimote.SetRumble(chkRumble.Checked);
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

            lblAccel.Text = ws.AccelState.Values.ToString();

            chkLED1.Checked = ws.LEDState.LED1;
            chkLED2.Checked = ws.LEDState.LED2;
            chkLED3.Checked = ws.LEDState.LED3;
            chkLED4.Checked = ws.LEDState.LED4;

            switch (ws.ExtensionType)
            {
                case ExtensionType.Nunchuk:
                    lblChuk.Text = ws.NunchukState.AccelState.Values.ToString();
                    lblChukJoy.Text = ws.NunchukState.Joystick.ToString();
                    chkC.Checked = ws.NunchukState.C;
                    chkZ.Checked = ws.NunchukState.Z;
                    break;

                case ExtensionType.ClassicController:
                    clbCCButtons.SetItemChecked(0, ws.ClassicControllerState.ButtonState.A);
                    clbCCButtons.SetItemChecked(1, ws.ClassicControllerState.ButtonState.B);
                    clbCCButtons.SetItemChecked(2, ws.ClassicControllerState.ButtonState.X);
                    clbCCButtons.SetItemChecked(3, ws.ClassicControllerState.ButtonState.Y);
                    clbCCButtons.SetItemChecked(4, ws.ClassicControllerState.ButtonState.Minus);
                    clbCCButtons.SetItemChecked(5, ws.ClassicControllerState.ButtonState.Home);
                    clbCCButtons.SetItemChecked(6, ws.ClassicControllerState.ButtonState.Plus);
                    clbCCButtons.SetItemChecked(7, ws.ClassicControllerState.ButtonState.Up);
                    clbCCButtons.SetItemChecked(8, ws.ClassicControllerState.ButtonState.Down);
                    clbCCButtons.SetItemChecked(9, ws.ClassicControllerState.ButtonState.Left);
                    clbCCButtons.SetItemChecked(10, ws.ClassicControllerState.ButtonState.Right);
                    clbCCButtons.SetItemChecked(11, ws.ClassicControllerState.ButtonState.ZL);
                    clbCCButtons.SetItemChecked(12, ws.ClassicControllerState.ButtonState.ZR);
                    clbCCButtons.SetItemChecked(13, ws.ClassicControllerState.ButtonState.TriggerL);
                    clbCCButtons.SetItemChecked(14, ws.ClassicControllerState.ButtonState.TriggerR);

                    lblCCJoy1.Text = ws.ClassicControllerState.JoystickL.ToString();
                    lblCCJoy2.Text = ws.ClassicControllerState.JoystickR.ToString();

                    lblTriggerL.Text = ws.ClassicControllerState.TriggerL.ToString();
                    lblTriggerR.Text = ws.ClassicControllerState.TriggerR.ToString();
                    break;

                case ExtensionType.Guitar:
                    clbGuitarButtons.SetItemChecked(0, ws.GuitarState.FretButtonState.Green);
                    clbGuitarButtons.SetItemChecked(1, ws.GuitarState.FretButtonState.Red);
                    clbGuitarButtons.SetItemChecked(2, ws.GuitarState.FretButtonState.Yellow);
                    clbGuitarButtons.SetItemChecked(3, ws.GuitarState.FretButtonState.Blue);
                    clbGuitarButtons.SetItemChecked(4, ws.GuitarState.FretButtonState.Orange);
                    clbGuitarButtons.SetItemChecked(5, ws.GuitarState.ButtonState.Minus);
                    clbGuitarButtons.SetItemChecked(6, ws.GuitarState.ButtonState.Plus);
                    clbGuitarButtons.SetItemChecked(7, ws.GuitarState.ButtonState.StrumUp);
                    clbGuitarButtons.SetItemChecked(8, ws.GuitarState.ButtonState.StrumDown);

                    clbTouchbar.SetItemChecked(0, ws.GuitarState.TouchbarState.Green);
                    clbTouchbar.SetItemChecked(1, ws.GuitarState.TouchbarState.Red);
                    clbTouchbar.SetItemChecked(2, ws.GuitarState.TouchbarState.Yellow);
                    clbTouchbar.SetItemChecked(3, ws.GuitarState.TouchbarState.Blue);
                    clbTouchbar.SetItemChecked(4, ws.GuitarState.TouchbarState.Orange);

                    lblGuitarJoy.Text = ws.GuitarState.Joystick.ToString();
                    lblGuitarWhammy.Text = ws.GuitarState.WhammyBar.ToString();
                    lblGuitarType.Text = ws.GuitarState.GuitarType.ToString();
                    break;

                case ExtensionType.Drums:
                    clbDrums.SetItemChecked(0, ws.DrumsState.Red);
                    clbDrums.SetItemChecked(1, ws.DrumsState.Blue);
                    clbDrums.SetItemChecked(2, ws.DrumsState.Green);
                    clbDrums.SetItemChecked(3, ws.DrumsState.Yellow);
                    clbDrums.SetItemChecked(4, ws.DrumsState.Orange);
                    clbDrums.SetItemChecked(5, ws.DrumsState.Pedal);
                    clbDrums.SetItemChecked(6, ws.DrumsState.Minus);
                    clbDrums.SetItemChecked(7, ws.DrumsState.Plus);

                    lbDrumVelocity.Items.Clear();
                    lbDrumVelocity.Items.Add(ws.DrumsState.RedVelocity);
                    lbDrumVelocity.Items.Add(ws.DrumsState.BlueVelocity);
                    lbDrumVelocity.Items.Add(ws.DrumsState.GreenVelocity);
                    lbDrumVelocity.Items.Add(ws.DrumsState.YellowVelocity);
                    lbDrumVelocity.Items.Add(ws.DrumsState.OrangeVelocity);
                    lbDrumVelocity.Items.Add(ws.DrumsState.PedalVelocity);

                    lblDrumJoy.Text = ws.DrumsState.Joystick.ToString();
                    break;

                case ExtensionType.BalanceBoard:
                    if (chkLbs.Checked)
                    {
                        lblBBTL.Text = ws.BalanceBoardState.SensorValuesLb.TopLeft.ToString();
                        lblBBTR.Text = ws.BalanceBoardState.SensorValuesLb.TopRight.ToString();
                        lblBBBL.Text = ws.BalanceBoardState.SensorValuesLb.BottomLeft.ToString();
                        lblBBBR.Text = ws.BalanceBoardState.SensorValuesLb.BottomRight.ToString();
                        lblBBTotal.Text = ws.BalanceBoardState.WeightLb.ToString();
                    }
                    else
                    {
                        lblBBTL.Text = ws.BalanceBoardState.SensorValuesKg.TopLeft.ToString();
                        lblBBTR.Text = ws.BalanceBoardState.SensorValuesKg.TopRight.ToString();
                        lblBBBL.Text = ws.BalanceBoardState.SensorValuesKg.BottomLeft.ToString();
                        lblBBBR.Text = ws.BalanceBoardState.SensorValuesKg.BottomRight.ToString();
                        lblBBTotal.Text = ws.BalanceBoardState.WeightKg.ToString();
                    }
                    lblCOG.Text = ws.BalanceBoardState.CenterOfGravity.ToString();
                    break;
            }

            g.Clear(Color.Black);

            UpdateIR(ws.IRState.IRSensors[0], lblIR1, lblIR1Raw, chkFound1, Color.Red);
            UpdateIR(ws.IRState.IRSensors[1], lblIR2, lblIR2Raw, chkFound2, Color.Blue);
            UpdateIR(ws.IRState.IRSensors[2], lblIR3, lblIR3Raw, chkFound3, Color.Yellow);
            UpdateIR(ws.IRState.IRSensors[3], lblIR4, lblIR4Raw, chkFound4, Color.Orange);

            for (int i = 0; i <= 3; i++)
            {
                if (ws.IRState.IRSensors[i].Found)
                    g.DrawEllipse(new Pen(Color.White), (int)(ws.IRState.IRSensors[i].RawPosition.X / 4), (int)(ws.IRState.IRSensors[i].RawPosition.Y / 4), 2, 2);
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

                percentX = percentX - 0.5;
                Console.WriteLine(percentX);

                var x = 1280;
                var y = 1024;

                percentX = (x / 2) + (x * (percentX / 2.7));

                SetCursorPos((int)(percentX), (int)(y * (1 - percentY)));
            }

            if (ws.IRState.IRSensors[0].Found && ws.IRState.IRSensors[1].Found && ws.IRState.IRSensors[2].Found && ws.IRState.IRSensors[3].Found)
            {
                g.DrawLine(new Pen(Color.Red), (int)(corners[0].X / 4), (int)(corners[0].Y / 4), (int)(corners[1].X / 4), (int)(corners[1].Y / 4));
                g.DrawLine(new Pen(Color.Blue), (int)(corners[1].X / 4), (int)(corners[1].Y / 4), (int)(corners[2].X / 4), (int)(corners[2].Y / 4));
                g.DrawLine(new Pen(Color.Yellow), (int)(corners[2].X / 4), (int)(corners[2].Y / 4), (int)(corners[3].X / 4), (int)(corners[3].Y / 4));
                g.DrawLine(new Pen(Color.Orange), (int)(corners[3].X / 4), (int)(corners[3].Y / 4), (int)(corners[0].X / 4), (int)(corners[0].Y / 4));
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
            chkExtension.Text = args.ExtensionType.ToString();
            chkExtension.Checked = args.Inserted;
        }

        public Wiimote Wiimote
        {
            set { mWiimote = value; }
        }
        
    }
}
