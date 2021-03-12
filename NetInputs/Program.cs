using LightGunWiimote4Points.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WiimoteLib;

namespace LightGunWiimote4Points
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Position resolution = new Position(1920, 1080);
            WiimoteCollection mWC = new WiimoteCollection();

            try
            {
                mWC.FindAllWiimotes();
            }
            catch (WiimoteNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Wiimote not found error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (WiimoteException ex)
            {
                MessageBox.Show(ex.Message, "Wiimote error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unknown error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            for (int i = 0; i < mWC.Count; i++)
            {
                new CoreWiimote(mWC[i], i + 1, resolution);
            }

            // Don't do that
            while (true)
            {
            }
        }
    }
}