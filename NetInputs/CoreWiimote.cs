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
        double scale = 1;

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

            computedEdges.Clear();
            edges.Clear();

            for (int i = 0; i < 4; i++)
            {
                computedEdges.Add(new Position(0, 0));
                edges.Add(new Position(0, 0));
            }

            wm.Connect();
        }

        bool edgeDown = false;

        private void WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            ws = args.WiimoteState;
            Position centerBar = new Position(
                ws.IRState.IRSensors[0].Position.X + (ws.IRState.IRSensors[1].Position.X - ws.IRState.IRSensors[0].Position.X) / 2,
                ws.IRState.IRSensors[0].Position.Y + (ws.IRState.IRSensors[1].Position.Y - ws.IRState.IRSensors[0].Position.Y) / 2
            );
            double currentScale = Math.Abs(ws.IRState.IRSensors[1].Position.X - ws.IRState.IRSensors[0].Position.X);

            if (ws.ButtonState.Home)
            {
                edges.Clear();
                for (int i = 0; i < 4; i++)
                {
                    edges.Add(new Position(0, 0));
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
                    if (edges[i].X == 0)
                    {
                        edges[i] = new Position(0.5 - (centerBar.X + 0.5), 0.5 - (centerBar.Y + 0.5));
                        scale = currentScale;
                        break;
                    }
                }
            }
            currentScale = (currentScale / scale) * 0.9;
            Console.WriteLine(currentScale);

            for (int i = 0; i < 4; i++)
            {
                computedEdges[i] = new Position(currentScale * (centerBar.X + edges[i].X) + 0.5, currentScale * (centerBar.Y + edges[i].Y) + 0.5);
            }

            computedEdges = MathUtils.Sort(computedEdges).ToList();

            percentX = (0.5 - computedEdges[2].X) / (computedEdges[3].X - computedEdges[2].X);
            percentY = (0.5 - computedEdges[0].Y) / (computedEdges[1].Y - computedEdges[0].Y);

            if (ws.ButtonState.A == false)
            {
                SetCursorPos((int)(resolution.X * percentX), (int)(resolution.Y * (1 - percentY)));
            }

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
