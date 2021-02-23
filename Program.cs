using System;
using System.Windows.Forms;

namespace LightGunWiimote4Points
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new SingleWiimoteForm());
		}
	}
}