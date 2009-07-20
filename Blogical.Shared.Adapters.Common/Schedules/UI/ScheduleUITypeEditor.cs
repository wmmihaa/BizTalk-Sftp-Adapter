using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml;


namespace Blogical.Shared.Adapters.Common.Schedules.UI
{
	/// <summary>
	/// Implements a user interface for setting schedule parameters
	/// within a visual designer.
	/// </summary>
	public class ScheduleUITypeEditor : UITypeEditor 
	{
		private IWindowsFormsEditorService service = null;
		private ScheduleDialog dialog;

        /// <summary>
        /// Returns Modal Type
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) 
		{
			if (null != context && null != context.Instance) 
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
        /// <summary>
        /// Called when editor is closed
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) 
		{
			if (null != context && null != context.Instance && null != provider) 
			{
				this.service = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
				if (null != this.service) 
				{
					this.dialog = new ScheduleDialog();
					if (value != null)
						this.dialog.ConfigXml.LoadXml((string)value);
					if (this.service.ShowDialog(this.dialog) == DialogResult.OK)
					{
						value = this.dialog.ConfigXml.OuterXml;
					}
				}
			}
			return value;
		}
		private void Exit(object sender, EventArgs e) 
		{
			if (null != this.service) 
			{
				this.service.CloseDropDown();
			}
		}
	}
    /// <summary>
    /// ...
    /// </summary>
	public class ScheduleConverter : System.ComponentModel.StringConverter 
	{
        /// <summary>
        /// Return false
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return false;
		}
        /// <summary>
        /// Return true
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		} 
        /// <summary>
        /// ...
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(string) == destinationType && value is string)
			{
				if ((string)value == string.Empty)
					return string.Empty;			   
				XmlDocument configXml = new XmlDocument();
				configXml.LoadXml((string)value);
				return configXml.DocumentElement.GetAttribute("type") + " Schedule";
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}


} 
