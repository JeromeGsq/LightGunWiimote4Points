using LightGunWiimote4Points.Models;
using LightGunWiimote4Points.Utils;
using System;
using System.IO;
using System.Runtime.InteropServices;
using vJoyInterfaceWrap;
using WiimoteLib;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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

        List<Position> edges = new List<Position>();
        List<Position> computedEdges = new List<Position>();
        bool mouseDown = false;
        bool mouseRightDown = false;
        double scale = 1;
        double baseScale = 0;

        Task rumble;
        Task rightClick;
        bool setupComplete = true;

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
            joystick?.AcquireVJD(index);

            computedEdges.Clear();
            edges.Clear();

            for (int i = 0; i < 4; i++)
            {
                computedEdges.Add(new Position(0, 0));
                edges.Add(new Position(0, 0));
            }
        }

        bool edgeDown = false;

        private void WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            ws = args.WiimoteState;

            Position centerBar = new Position(
                ws.IRState.IRSensors[0].Position.X + (ws.IRState.IRSensors[1].Position.X - ws.IRState.IRSensors[0].Position.X) / 2,
                ws.IRState.IRSensors[0].Position.Y + (ws.IRState.IRSensors[1].Position.Y - ws.IRState.IRSensors[0].Position.Y) / 2
            );

            double currentScale = MathUtils.GetDistance(ws.IRState.IRSensors[0].Position, ws.IRState.IRSensors[1].Position);

            if (ws.ButtonState.Home)
            {
                ClearEdges();
            }


            foreach (var edge in edges)
            {
                if (edge.X == 0)
                {
                    setupComplete = false;
                    break;
                }
                else
                {
                    setupComplete = true;
                }
            }

            if (ws.ButtonState.B == false && edgeDown == true)
            {
                edgeDown = false;
            }
            else if (ws.ButtonState.B && edgeDown == false)
            {
                edgeDown = true;

                for (int i = 0; i < 4; i++)
                {
                    if (i == 0)
                    {
                        baseScale = (currentScale / scale) * 0.9;
                    }

                    if (edges[i].X == 0)
                    {
                        edges[i] = new Position(0.5 - (centerBar.X + 0.5), 0.5 - (centerBar.Y + 0.5));
                        scale = currentScale;
                        break;
                    }
                }
            }

            currentScale = (currentScale / scale) * 0.9;
            if (setupComplete)
            {
                // Outside 10%
                // Alert user with rumble
                if ((currentScale / baseScale > 1.1 || currentScale / baseScale < 0.9) && rumble == null)
                {
                    rumble = Rumble(75);
                }
                // Inside 10%
                // Reset rumble
                else if ((currentScale / baseScale < 1.1 && currentScale / baseScale > 0.9))
                {
                    rumble = null;
                }
            }

            if (computedEdges.Count > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    computedEdges[i] = new Position(currentScale * (centerBar.X + edges[i].X) + 0.5, currentScale * (centerBar.Y + edges[i].Y) + 0.5);
                }

                computedEdges = MathUtils.Sort(computedEdges).ToList();

                percentX = (0.5 - computedEdges[2].X) / (computedEdges[3].X - computedEdges[2].X);
                percentY = (0.5 - computedEdges[0].Y) / (computedEdges[1].Y - computedEdges[0].Y);

                /*
                var angle = Math.Atan2(ws.IRState.IRSensors[1].Position.Y - ws.IRState.IRSensors[0].Position.Y, ws.IRState.IRSensors[1].Position.X - ws.IRState.IRSensors[0].Position.X) * (180 / Math.PI);

                var center = MathUtils.Center(computedEdges[0], computedEdges[1]);
                // * 0.005f => it's % and divide by 2 to get 50%
                // percentX += (angle * (0.005f * (center.Y)));

                var a = MathUtils.Lerp((float)computedEdges[0].Y, (float)computedEdges[1].Y, 0.5f);
                var b = MathUtils.Center(computedEdges[0], computedEdges[1]);

                percentX = percentX - ((angle/90)/100);
                Console.WriteLine(percentX);
                */

                // Move the pointer if the wiimote can see 2 LEDs (or more)
                if (ws.IRState.IRSensors.Where(e => e.Found == true).Count() >= 2 && setupComplete)
                {
                    rightClick = null;
                    SetCursorPos((int)(resolution.X * percentX), (int)(resolution.Y * (1 - percentY)));

                    if (mouseDown && ws.ButtonState.B == false)
                    {
                        mouse_event(MOUSEEVENTF_LEFTUP, (uint)(resolution.X * percentX), (uint)(resolution.Y * (1 - percentY)), 0, 0);
                        mouseDown = false;
                    }
                    if (mouseDown == false && ws.ButtonState.B)
                    {
                        mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)(resolution.X * percentX), (uint)(resolution.Y * (1 - percentY)), 0, 0);
                        mouseDown = true;
                    }

                    if (mouseRightDown && ws.ButtonState.A == false)
                    {
                        mouse_event(MOUSEEVENTF_RIGHTUP, (uint)(resolution.X * percentX), (uint)(resolution.Y * (1 - percentY)), 0, 0);
                        mouseRightDown = false;
                    }
                    if (mouseRightDown == false && ws.ButtonState.A)
                    {
                        mouse_event(MOUSEEVENTF_RIGHTDOWN, (uint)(resolution.X * percentX), (uint)(resolution.Y * (1 - percentY)), 0, 0);
                        mouseRightDown = true;
                    }
                }
                else if (rightClick == null)
                {
                    rightClick = RightClick();
                }

                SendMessageToFront(centerBar);
            }
        }

        public async Task Rumble(int rumble = 75)
        {
            wm.SetRumble(true);
            await Task.Delay(rumble);
            wm.SetRumble(false);
        }

        public async Task RightClick()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, (uint)(resolution.X * percentX), (uint)(resolution.Y * (1 - percentY)), 0, 0);
            mouseRightDown = true;

            wm.SetRumble(true);
            await Task.Delay(100);
            wm.SetRumble(false);

            mouse_event(MOUSEEVENTF_RIGHTUP, (uint)(resolution.X * percentX), (uint)(resolution.Y * (1 - percentY)), 0, 0);
            mouseRightDown = false;
        }

        private void SendMessageToFront(Position centerBar)
        {
            if (index == 1)
            {
                string value = "";
                for (int i = 0; i <= 1; i++)
                {
                    value += ws.IRState.IRSensors[i].Position.X + ":" + ws.IRState.IRSensors[i].Position.Y + "\n";
                }

                value += (centerBar.X) + ":" + (centerBar.Y) + "\n";

                for (int i = 0; i < 4; i++)
                {
                    value += (computedEdges[i].X) + ":" + (computedEdges[i].Y) + "\n";
                }

                Program.SendMessage(value);
            }
        }

        private void ClearEdges()
        {
            edges.Clear();
            for (int i = 0; i < 4; i++)
            {
                edges.Add(new Position(0, 0));
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
