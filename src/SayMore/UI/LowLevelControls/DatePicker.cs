using System;
using System.Globalization;
using System.Windows.Forms;
using Palaso.Extensions;

namespace SayMore.UI.LowLevelControls
{
	/// ----------------------------------------------------------------------------------------
	/// A DateTimePicker that allows null values and displays an empty picker when the value
	/// is null. When the control has focus and Del is pressed, the value will be set to null.
	/// ----------------------------------------------------------------------------------------
	public class DatePicker : DateTimePicker
	{
		private string _value;
		private bool _ignoreEvents;

		/// ------------------------------------------------------------------------------------
		public override string Text
		{
			get { return base.Text; }
			set { SetValue(value); }
		}

		/// ------------------------------------------------------------------------------------
		public string GetISO8601DateValueOrNull()
		{
			return (_value == null ? null :
				DateTime.Parse(_value, CultureInfo.CurrentCulture).ToISO8601DateOnlyString());
		}

		/// ------------------------------------------------------------------------------------
		public void SetValue(string value)
		{
			_value = (value ?? string.Empty).Trim();
			if (_value == string.Empty)
				_value = null;

			if (_value == null)
				SetBlankFormat();
			else
				SetDefaultFormat();

			_ignoreEvents = true;
			base.Text = value;
			_ignoreEvents = false;
		}

		/// ------------------------------------------------------------------------------------
		private void SetBlankFormat()
		{
			_ignoreEvents = true;
			SilTools.Utils.SetWindowRedraw(this, false);
			Format = DateTimePickerFormat.Custom;
			CustomFormat = " ";
			SilTools.Utils.SetWindowRedraw(this, true);
			_ignoreEvents = false;
		}

		/// ------------------------------------------------------------------------------------
		private void SetDefaultFormat()
		{
			_ignoreEvents = true;
			SilTools.Utils.SetWindowRedraw(this, false);
			Format = DateTimePickerFormat.Short;
			CustomFormat = string.Empty;
			SilTools.Utils.SetWindowRedraw(this, true);
			_ignoreEvents = false;
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);

			if (_ignoreEvents)
				return;

			SetDefaultFormat();

			if (_value != null)
				return;

			_ignoreEvents = true;
			base.Text = new DateTime(DateTime.Now.Year, 1, 1).ToShortDateString();
			_ignoreEvents = false;
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnLeave(EventArgs e)
		{
			base.OnLeave(e);

			if (_ignoreEvents)
				return;

			if (_value == null)
				SetBlankFormat();
			else
				SetDefaultFormat();
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.KeyCode != Keys.Delete || _value == null)
				return;

			SetValue(null);

			// The effects of setting the value to null cannot be seen unless the selected
			// control changes. Therefore, selecte the next control. REVIEW: Will the user
			// find this annoying? Inquiring minds want to know.
			// I can't get SelectNextControl to work, so I'll try it this way.
			var nextCtrl = Parent.GetNextControl(this, true);
			while (nextCtrl != null && !nextCtrl.CanSelect)
				nextCtrl = Parent.GetNextControl(nextCtrl, true);

			if (nextCtrl != null)
				nextCtrl.Focus();
			else
				SelectNextControl(this, true, true, true, true);
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnValueChanged(EventArgs e)
		{
			// Setting the Text property will trigger this event and sometimes
			// we don't want to set _value when that happens.
			if (!_ignoreEvents)
				_value = Value.ToShortDateString();

			base.OnValueChanged(e);
		}
	}
}