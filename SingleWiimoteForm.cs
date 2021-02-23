using System;
using System.Windows.Forms;

using WiimoteLib;

namespace LightGunWiimote4Points
{
    public partial class SingleWiimoteForm : Form
    {
        Wiimote wm = new Wiimote();

        public SingleWiimoteForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            wm.WiimoteChanged += wm_WiimoteChanged;
            wm.WiimoteExtensionChanged += wm_WiimoteExtensionChanged;
            wm.Connect();
            wm.SetReportType(InputReport.IRExtensionAccel, true);
            wm.SetLEDs(true, false, false, false);
        }

        private void wm_WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            wiimoteInfo1.UpdateState(args);
        }

        private void wm_WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs args)
        {
            wiimoteInfo1.UpdateExtension(args);

            if (args.Inserted)
            {
                wm.SetReportType(InputReport.IRExtensionAccel, true);
            }
            else
            {
                wm.SetReportType(InputReport.IRAccel, true);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            wm.SetLEDs(false, false, false, false);
            wm.Disconnect();
        }
    }
}
