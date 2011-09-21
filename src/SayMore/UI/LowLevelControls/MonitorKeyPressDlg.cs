using System.Windows.Forms;

namespace SayMore.UI.LowLevelControls
{
	public class MonitorKeyPressDlg : Form, IMessageFilter
	{
		private const int WM_KEYDOWN = 0x100;
		private const int WM_KEYUP = 0x101;

		/// ------------------------------------------------------------------------------------
		public MonitorKeyPressDlg()
		{
			Application.AddMessageFilter(this);
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			Application.RemoveMessageFilter(this);
			base.OnFormClosed(e);
		}

		/// ------------------------------------------------------------------------------------
		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg == WM_KEYDOWN)
				return OnLowLevelKeyDown((Keys)m.WParam);

			if (m.Msg == WM_KEYUP)
				return OnLowLevelKeyUp((Keys)m.WParam);

			return false;
		}

		/// ------------------------------------------------------------------------------------
		protected virtual bool OnLowLevelKeyDown(Keys key)
		{
			return false;
		}

		/// ------------------------------------------------------------------------------------
		protected virtual bool OnLowLevelKeyUp(Keys key)
		{
			return false;
		}
	}
}
