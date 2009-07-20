using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using Blogical.Shared.Adapters.Common.Schedules;

namespace Blogical.Shared.Adapters.Common.Schedules.UI
{
	/// <summary>
	/// ScheduleDialog: dialog for selecting schedule type and parameters
	/// </summary>
	public class ScheduleDialog : System.Windows.Forms.Form
	{
		private  XmlDocument configXml;
        /// <summary>
        /// Schedule properties
        /// </summary>
		public XmlDocument ConfigXml
		{
			get{return configXml;}
		}
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.DateTimePicker dateStart;
		private System.Windows.Forms.Label labelStartDate;
		private System.Windows.Forms.Label labelStartTime;
		private System.Windows.Forms.DateTimePicker timeStart;
		private System.Windows.Forms.RadioButton radioDayInterval;
		private System.Windows.Forms.Label labelDays;
		private System.Windows.Forms.RadioButton radioSelectDays;
		private System.Windows.Forms.Label labelweeks;
		private System.Windows.Forms.Label labelEvery;
		private System.Windows.Forms.RadioButton radioDayofMonth;
		private System.Windows.Forms.NumericUpDown dayofmonth;
		private System.Windows.Forms.RadioButton radioOrdinal;
		private System.Windows.Forms.ComboBox ordinalDropDown;
		private System.Windows.Forms.ComboBox weekdayDropDown;
		private System.Windows.Forms.CheckBox monthDecember;
		private System.Windows.Forms.CheckBox monthNovember;
		private System.Windows.Forms.CheckBox monthOctober;
		private System.Windows.Forms.CheckBox monthSeptember;
		private System.Windows.Forms.CheckBox monthAugust;
		private System.Windows.Forms.CheckBox monthJuly;
		private System.Windows.Forms.CheckBox monthJune;
		private System.Windows.Forms.CheckBox monthMay;
		private System.Windows.Forms.CheckBox monthApril;
		private System.Windows.Forms.CheckBox monthMarch;
		private System.Windows.Forms.CheckBox monthFebruary;
		private System.Windows.Forms.CheckBox monthJanuary;
		private System.Windows.Forms.CheckBox weekSaturday;
		private System.Windows.Forms.CheckBox weekFriday;
		private System.Windows.Forms.CheckBox weekThursday;
		private System.Windows.Forms.CheckBox weekWednesday;
		private System.Windows.Forms.CheckBox weekTuesday;
		private System.Windows.Forms.CheckBox weekMonday;
		private System.Windows.Forms.CheckBox weekSunday;
		private System.Windows.Forms.Label labelSelectDays;
		private System.Windows.Forms.NumericUpDown weekInterval;
		private System.Windows.Forms.CheckBox daySaturday;
		private System.Windows.Forms.CheckBox dayFriday;
		private System.Windows.Forms.CheckBox dayThursday;
		private System.Windows.Forms.CheckBox dayWednesday;
		private System.Windows.Forms.CheckBox dayTuesday;
		private System.Windows.Forms.CheckBox dayMonday;
		private System.Windows.Forms.CheckBox daySunday;
		private System.Windows.Forms.NumericUpDown dayInterval;
		private System.Windows.Forms.TabControl tabPages;
		private System.Windows.Forms.TabPage tabDaily;
		private System.Windows.Forms.TabPage tabWeekly;
		private System.Windows.Forms.TabPage tabMonthly;
        private TabPage tabTimely;
        private Label label1;
        private ComboBox timeType;
        private NumericUpDown timeInterval;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        /// <summary>
        /// Constructor
        /// </summary>
		public ScheduleDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			configXml = new XmlDocument();
			
			//default
			this.tabPages.SelectedTab = this.tabDaily;
			this.radioDayofMonth.Checked = true;
			this.radioDayInterval.Checked = true;
			DateTime now =   DateTime.Now;
			this.dateStart.Value = now;
			this.timeStart.Value = new DateTime(now.Year, now.Month, now.Day, 0,0,0);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScheduleDialog));
            this.monthDecember = new System.Windows.Forms.CheckBox();
            this.monthNovember = new System.Windows.Forms.CheckBox();
            this.monthOctober = new System.Windows.Forms.CheckBox();
            this.monthSeptember = new System.Windows.Forms.CheckBox();
            this.monthAugust = new System.Windows.Forms.CheckBox();
            this.monthJuly = new System.Windows.Forms.CheckBox();
            this.monthJune = new System.Windows.Forms.CheckBox();
            this.monthMay = new System.Windows.Forms.CheckBox();
            this.monthApril = new System.Windows.Forms.CheckBox();
            this.monthMarch = new System.Windows.Forms.CheckBox();
            this.monthFebruary = new System.Windows.Forms.CheckBox();
            this.monthJanuary = new System.Windows.Forms.CheckBox();
            this.weekdayDropDown = new System.Windows.Forms.ComboBox();
            this.ordinalDropDown = new System.Windows.Forms.ComboBox();
            this.radioOrdinal = new System.Windows.Forms.RadioButton();
            this.dayofmonth = new System.Windows.Forms.NumericUpDown();
            this.radioDayofMonth = new System.Windows.Forms.RadioButton();
            this.daySaturday = new System.Windows.Forms.CheckBox();
            this.dayFriday = new System.Windows.Forms.CheckBox();
            this.dayThursday = new System.Windows.Forms.CheckBox();
            this.dayWednesday = new System.Windows.Forms.CheckBox();
            this.dayTuesday = new System.Windows.Forms.CheckBox();
            this.dayMonday = new System.Windows.Forms.CheckBox();
            this.daySunday = new System.Windows.Forms.CheckBox();
            this.radioSelectDays = new System.Windows.Forms.RadioButton();
            this.dayInterval = new System.Windows.Forms.NumericUpDown();
            this.labelDays = new System.Windows.Forms.Label();
            this.radioDayInterval = new System.Windows.Forms.RadioButton();
            this.weekSaturday = new System.Windows.Forms.CheckBox();
            this.weekFriday = new System.Windows.Forms.CheckBox();
            this.weekThursday = new System.Windows.Forms.CheckBox();
            this.weekWednesday = new System.Windows.Forms.CheckBox();
            this.weekTuesday = new System.Windows.Forms.CheckBox();
            this.weekMonday = new System.Windows.Forms.CheckBox();
            this.weekSunday = new System.Windows.Forms.CheckBox();
            this.labelSelectDays = new System.Windows.Forms.Label();
            this.labelEvery = new System.Windows.Forms.Label();
            this.labelweeks = new System.Windows.Forms.Label();
            this.weekInterval = new System.Windows.Forms.NumericUpDown();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.dateStart = new System.Windows.Forms.DateTimePicker();
            this.labelStartDate = new System.Windows.Forms.Label();
            this.labelStartTime = new System.Windows.Forms.Label();
            this.timeStart = new System.Windows.Forms.DateTimePicker();
            this.tabPages = new System.Windows.Forms.TabControl();
            this.tabDaily = new System.Windows.Forms.TabPage();
            this.tabWeekly = new System.Windows.Forms.TabPage();
            this.tabMonthly = new System.Windows.Forms.TabPage();
            this.tabTimely = new System.Windows.Forms.TabPage();
            this.timeType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timeInterval = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.dayofmonth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dayInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weekInterval)).BeginInit();
            this.tabPages.SuspendLayout();
            this.tabDaily.SuspendLayout();
            this.tabWeekly.SuspendLayout();
            this.tabMonthly.SuspendLayout();
            this.tabTimely.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // monthDecember
            // 
            this.monthDecember.Location = new System.Drawing.Point(216, 168);
            this.monthDecember.Name = "monthDecember";
            this.monthDecember.Size = new System.Drawing.Size(80, 16);
            this.monthDecember.TabIndex = 16;
            this.monthDecember.Text = "December";
            // 
            // monthNovember
            // 
            this.monthNovember.Location = new System.Drawing.Point(216, 144);
            this.monthNovember.Name = "monthNovember";
            this.monthNovember.Size = new System.Drawing.Size(80, 16);
            this.monthNovember.TabIndex = 15;
            this.monthNovember.Text = "November";
            // 
            // monthOctober
            // 
            this.monthOctober.Location = new System.Drawing.Point(216, 120);
            this.monthOctober.Name = "monthOctober";
            this.monthOctober.Size = new System.Drawing.Size(80, 16);
            this.monthOctober.TabIndex = 14;
            this.monthOctober.Text = "October";
            // 
            // monthSeptember
            // 
            this.monthSeptember.Location = new System.Drawing.Point(216, 96);
            this.monthSeptember.Name = "monthSeptember";
            this.monthSeptember.Size = new System.Drawing.Size(80, 16);
            this.monthSeptember.TabIndex = 13;
            this.monthSeptember.Text = "September";
            // 
            // monthAugust
            // 
            this.monthAugust.Location = new System.Drawing.Point(112, 168);
            this.monthAugust.Name = "monthAugust";
            this.monthAugust.Size = new System.Drawing.Size(72, 16);
            this.monthAugust.TabIndex = 12;
            this.monthAugust.Text = "August";
            // 
            // monthJuly
            // 
            this.monthJuly.Location = new System.Drawing.Point(112, 144);
            this.monthJuly.Name = "monthJuly";
            this.monthJuly.Size = new System.Drawing.Size(72, 16);
            this.monthJuly.TabIndex = 11;
            this.monthJuly.Text = "July";
            // 
            // monthJune
            // 
            this.monthJune.Location = new System.Drawing.Point(112, 120);
            this.monthJune.Name = "monthJune";
            this.monthJune.Size = new System.Drawing.Size(72, 16);
            this.monthJune.TabIndex = 10;
            this.monthJune.Text = "June";
            // 
            // monthMay
            // 
            this.monthMay.Location = new System.Drawing.Point(112, 96);
            this.monthMay.Name = "monthMay";
            this.monthMay.Size = new System.Drawing.Size(72, 16);
            this.monthMay.TabIndex = 9;
            this.monthMay.Text = "May";
            // 
            // monthApril
            // 
            this.monthApril.Location = new System.Drawing.Point(8, 168);
            this.monthApril.Name = "monthApril";
            this.monthApril.Size = new System.Drawing.Size(72, 16);
            this.monthApril.TabIndex = 8;
            this.monthApril.Text = "April";
            // 
            // monthMarch
            // 
            this.monthMarch.Location = new System.Drawing.Point(8, 144);
            this.monthMarch.Name = "monthMarch";
            this.monthMarch.Size = new System.Drawing.Size(72, 16);
            this.monthMarch.TabIndex = 7;
            this.monthMarch.Text = "March";
            // 
            // monthFebruary
            // 
            this.monthFebruary.Location = new System.Drawing.Point(8, 120);
            this.monthFebruary.Name = "monthFebruary";
            this.monthFebruary.Size = new System.Drawing.Size(72, 16);
            this.monthFebruary.TabIndex = 6;
            this.monthFebruary.Text = "February";
            // 
            // monthJanuary
            // 
            this.monthJanuary.Location = new System.Drawing.Point(8, 96);
            this.monthJanuary.Name = "monthJanuary";
            this.monthJanuary.Size = new System.Drawing.Size(64, 16);
            this.monthJanuary.TabIndex = 5;
            this.monthJanuary.Text = "January";
            // 
            // weekdayDropDown
            // 
            this.weekdayDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.weekdayDropDown.Items.AddRange(new object[] {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "WeekDay",
            "Day"});
            this.weekdayDropDown.Location = new System.Drawing.Point(184, 48);
            this.weekdayDropDown.MaxDropDownItems = 9;
            this.weekdayDropDown.Name = "weekdayDropDown";
            this.weekdayDropDown.Size = new System.Drawing.Size(112, 21);
            this.weekdayDropDown.TabIndex = 4;
            // 
            // ordinalDropDown
            // 
            this.ordinalDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ordinalDropDown.Items.AddRange(new object[] {
            "First",
            "Second",
            "Third",
            "Fourth",
            "Last"});
            this.ordinalDropDown.Location = new System.Drawing.Point(72, 48);
            this.ordinalDropDown.MaxDropDownItems = 5;
            this.ordinalDropDown.Name = "ordinalDropDown";
            this.ordinalDropDown.Size = new System.Drawing.Size(96, 21);
            this.ordinalDropDown.TabIndex = 3;
            // 
            // radioOrdinal
            // 
            this.radioOrdinal.Location = new System.Drawing.Point(8, 46);
            this.radioOrdinal.Name = "radioOrdinal";
            this.radioOrdinal.Size = new System.Drawing.Size(56, 24);
            this.radioOrdinal.TabIndex = 2;
            this.radioOrdinal.Text = "The";
            // 
            // dayofmonth
            // 
            this.dayofmonth.Location = new System.Drawing.Point(64, 16);
            this.dayofmonth.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.dayofmonth.Name = "dayofmonth";
            this.dayofmonth.Size = new System.Drawing.Size(40, 20);
            this.dayofmonth.TabIndex = 1;
            // 
            // radioDayofMonth
            // 
            this.radioDayofMonth.Location = new System.Drawing.Point(8, 18);
            this.radioDayofMonth.Name = "radioDayofMonth";
            this.radioDayofMonth.Size = new System.Drawing.Size(56, 16);
            this.radioDayofMonth.TabIndex = 0;
            this.radioDayofMonth.Text = "Day";
            this.radioDayofMonth.CheckedChanged += new System.EventHandler(this.radioDayofMonth_CheckedChanged);
            // 
            // daySaturday
            // 
            this.daySaturday.Enabled = false;
            this.daySaturday.Location = new System.Drawing.Point(160, 152);
            this.daySaturday.Name = "daySaturday";
            this.daySaturday.Size = new System.Drawing.Size(104, 16);
            this.daySaturday.TabIndex = 10;
            this.daySaturday.Text = "Saturday";
            // 
            // dayFriday
            // 
            this.dayFriday.Enabled = false;
            this.dayFriday.Location = new System.Drawing.Point(160, 128);
            this.dayFriday.Name = "dayFriday";
            this.dayFriday.Size = new System.Drawing.Size(104, 16);
            this.dayFriday.TabIndex = 9;
            this.dayFriday.Text = "Friday";
            // 
            // dayThursday
            // 
            this.dayThursday.Enabled = false;
            this.dayThursday.Location = new System.Drawing.Point(160, 104);
            this.dayThursday.Name = "dayThursday";
            this.dayThursday.Size = new System.Drawing.Size(104, 16);
            this.dayThursday.TabIndex = 8;
            this.dayThursday.Text = "Thursday";
            // 
            // dayWednesday
            // 
            this.dayWednesday.Enabled = false;
            this.dayWednesday.Location = new System.Drawing.Point(160, 80);
            this.dayWednesday.Name = "dayWednesday";
            this.dayWednesday.Size = new System.Drawing.Size(104, 16);
            this.dayWednesday.TabIndex = 7;
            this.dayWednesday.Text = "Wednesday";
            // 
            // dayTuesday
            // 
            this.dayTuesday.Enabled = false;
            this.dayTuesday.Location = new System.Drawing.Point(40, 128);
            this.dayTuesday.Name = "dayTuesday";
            this.dayTuesday.Size = new System.Drawing.Size(104, 16);
            this.dayTuesday.TabIndex = 6;
            this.dayTuesday.Text = "Tuesday";
            // 
            // dayMonday
            // 
            this.dayMonday.Enabled = false;
            this.dayMonday.Location = new System.Drawing.Point(40, 104);
            this.dayMonday.Name = "dayMonday";
            this.dayMonday.Size = new System.Drawing.Size(104, 16);
            this.dayMonday.TabIndex = 5;
            this.dayMonday.Text = "Monday";
            // 
            // daySunday
            // 
            this.daySunday.Enabled = false;
            this.daySunday.Location = new System.Drawing.Point(40, 80);
            this.daySunday.Name = "daySunday";
            this.daySunday.Size = new System.Drawing.Size(104, 16);
            this.daySunday.TabIndex = 4;
            this.daySunday.Text = "Sunday";
            // 
            // radioSelectDays
            // 
            this.radioSelectDays.Location = new System.Drawing.Point(8, 56);
            this.radioSelectDays.Name = "radioSelectDays";
            this.radioSelectDays.Size = new System.Drawing.Size(104, 16);
            this.radioSelectDays.TabIndex = 3;
            this.radioSelectDays.Text = "On these days";
            // 
            // dayInterval
            // 
            this.dayInterval.Location = new System.Drawing.Point(64, 16);
            this.dayInterval.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.dayInterval.Name = "dayInterval";
            this.dayInterval.Size = new System.Drawing.Size(48, 20);
            this.dayInterval.TabIndex = 2;
            // 
            // labelDays
            // 
            this.labelDays.Location = new System.Drawing.Point(120, 16);
            this.labelDays.Name = "labelDays";
            this.labelDays.Size = new System.Drawing.Size(32, 16);
            this.labelDays.TabIndex = 10;
            this.labelDays.Text = "days";
            // 
            // radioDayInterval
            // 
            this.radioDayInterval.Checked = true;
            this.radioDayInterval.Location = new System.Drawing.Point(8, 16);
            this.radioDayInterval.Name = "radioDayInterval";
            this.radioDayInterval.Size = new System.Drawing.Size(64, 16);
            this.radioDayInterval.TabIndex = 1;
            this.radioDayInterval.TabStop = true;
            this.radioDayInterval.Text = "Every";
            this.radioDayInterval.CheckedChanged += new System.EventHandler(this.radioDayInterval_CheckedChanged);
            // 
            // weekSaturday
            // 
            this.weekSaturday.Location = new System.Drawing.Point(160, 152);
            this.weekSaturday.Name = "weekSaturday";
            this.weekSaturday.Size = new System.Drawing.Size(104, 16);
            this.weekSaturday.TabIndex = 11;
            this.weekSaturday.Text = "Saturday";
            // 
            // weekFriday
            // 
            this.weekFriday.Location = new System.Drawing.Point(160, 128);
            this.weekFriday.Name = "weekFriday";
            this.weekFriday.Size = new System.Drawing.Size(104, 16);
            this.weekFriday.TabIndex = 10;
            this.weekFriday.Text = "Friday";
            // 
            // weekThursday
            // 
            this.weekThursday.Location = new System.Drawing.Point(160, 104);
            this.weekThursday.Name = "weekThursday";
            this.weekThursday.Size = new System.Drawing.Size(104, 16);
            this.weekThursday.TabIndex = 9;
            this.weekThursday.Text = "Thursday";
            // 
            // weekWednesday
            // 
            this.weekWednesday.Location = new System.Drawing.Point(160, 80);
            this.weekWednesday.Name = "weekWednesday";
            this.weekWednesday.Size = new System.Drawing.Size(104, 16);
            this.weekWednesday.TabIndex = 8;
            this.weekWednesday.Text = "Wednesday";
            // 
            // weekTuesday
            // 
            this.weekTuesday.Location = new System.Drawing.Point(40, 128);
            this.weekTuesday.Name = "weekTuesday";
            this.weekTuesday.Size = new System.Drawing.Size(104, 16);
            this.weekTuesday.TabIndex = 7;
            this.weekTuesday.Text = "Tuesday";
            // 
            // weekMonday
            // 
            this.weekMonday.Location = new System.Drawing.Point(40, 104);
            this.weekMonday.Name = "weekMonday";
            this.weekMonday.Size = new System.Drawing.Size(104, 16);
            this.weekMonday.TabIndex = 6;
            this.weekMonday.Text = "Monday";
            // 
            // weekSunday
            // 
            this.weekSunday.Location = new System.Drawing.Point(40, 80);
            this.weekSunday.Name = "weekSunday";
            this.weekSunday.Size = new System.Drawing.Size(104, 16);
            this.weekSunday.TabIndex = 5;
            this.weekSunday.Text = "Sunday";
            // 
            // labelSelectDays
            // 
            this.labelSelectDays.Location = new System.Drawing.Point(8, 56);
            this.labelSelectDays.Name = "labelSelectDays";
            this.labelSelectDays.Size = new System.Drawing.Size(200, 16);
            this.labelSelectDays.TabIndex = 4;
            this.labelSelectDays.Text = "Select the day(s) of the week below:";
            // 
            // labelEvery
            // 
            this.labelEvery.Location = new System.Drawing.Point(8, 16);
            this.labelEvery.Name = "labelEvery";
            this.labelEvery.Size = new System.Drawing.Size(40, 16);
            this.labelEvery.TabIndex = 3;
            this.labelEvery.Text = "Every";
            // 
            // labelweeks
            // 
            this.labelweeks.Location = new System.Drawing.Point(112, 16);
            this.labelweeks.Name = "labelweeks";
            this.labelweeks.Size = new System.Drawing.Size(100, 16);
            this.labelweeks.TabIndex = 2;
            this.labelweeks.Text = "weeks";
            // 
            // weekInterval
            // 
            this.weekInterval.Location = new System.Drawing.Point(56, 16);
            this.weekInterval.Maximum = new decimal(new int[] {
            52,
            0,
            0,
            0});
            this.weekInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.weekInterval.Name = "weekInterval";
            this.weekInterval.Size = new System.Drawing.Size(48, 20);
            this.weekInterval.TabIndex = 1;
            this.weekInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(160, 360);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(248, 360);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            // 
            // dateStart
            // 
            this.dateStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateStart.Location = new System.Drawing.Point(104, 16);
            this.dateStart.Name = "dateStart";
            this.dateStart.Size = new System.Drawing.Size(104, 20);
            this.dateStart.TabIndex = 1;
            // 
            // labelStartDate
            // 
            this.labelStartDate.Location = new System.Drawing.Point(16, 18);
            this.labelStartDate.Name = "labelStartDate";
            this.labelStartDate.Size = new System.Drawing.Size(72, 16);
            this.labelStartDate.TabIndex = 8;
            this.labelStartDate.Text = "Start Date";
            // 
            // labelStartTime
            // 
            this.labelStartTime.Location = new System.Drawing.Point(16, 55);
            this.labelStartTime.Name = "labelStartTime";
            this.labelStartTime.Size = new System.Drawing.Size(72, 23);
            this.labelStartTime.TabIndex = 9;
            this.labelStartTime.Text = "Start Time";
            // 
            // timeStart
            // 
            this.timeStart.CustomFormat = "HH:mm tt";
            this.timeStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.timeStart.Location = new System.Drawing.Point(120, 56);
            this.timeStart.Name = "timeStart";
            this.timeStart.ShowUpDown = true;
            this.timeStart.Size = new System.Drawing.Size(88, 20);
            this.timeStart.TabIndex = 2;
            this.timeStart.Value = new System.DateTime(2005, 7, 30, 16, 22, 0, 0);
            // 
            // tabPages
            // 
            this.tabPages.Controls.Add(this.tabDaily);
            this.tabPages.Controls.Add(this.tabWeekly);
            this.tabPages.Controls.Add(this.tabMonthly);
            this.tabPages.Controls.Add(this.tabTimely);
            this.tabPages.Location = new System.Drawing.Point(8, 96);
            this.tabPages.Name = "tabPages";
            this.tabPages.SelectedIndex = 0;
            this.tabPages.Size = new System.Drawing.Size(320, 240);
            this.tabPages.TabIndex = 3;
            // 
            // tabDaily
            // 
            this.tabDaily.Controls.Add(this.dayInterval);
            this.tabDaily.Controls.Add(this.radioDayInterval);
            this.tabDaily.Controls.Add(this.labelDays);
            this.tabDaily.Controls.Add(this.radioSelectDays);
            this.tabDaily.Controls.Add(this.daySunday);
            this.tabDaily.Controls.Add(this.dayMonday);
            this.tabDaily.Controls.Add(this.dayTuesday);
            this.tabDaily.Controls.Add(this.dayWednesday);
            this.tabDaily.Controls.Add(this.dayThursday);
            this.tabDaily.Controls.Add(this.dayFriday);
            this.tabDaily.Controls.Add(this.daySaturday);
            this.tabDaily.Location = new System.Drawing.Point(4, 22);
            this.tabDaily.Name = "tabDaily";
            this.tabDaily.Size = new System.Drawing.Size(312, 214);
            this.tabDaily.TabIndex = 0;
            this.tabDaily.Text = "Daily";
            // 
            // tabWeekly
            // 
            this.tabWeekly.Controls.Add(this.labelSelectDays);
            this.tabWeekly.Controls.Add(this.labelEvery);
            this.tabWeekly.Controls.Add(this.labelweeks);
            this.tabWeekly.Controls.Add(this.weekInterval);
            this.tabWeekly.Controls.Add(this.weekSunday);
            this.tabWeekly.Controls.Add(this.weekMonday);
            this.tabWeekly.Controls.Add(this.weekTuesday);
            this.tabWeekly.Controls.Add(this.weekWednesday);
            this.tabWeekly.Controls.Add(this.weekThursday);
            this.tabWeekly.Controls.Add(this.weekFriday);
            this.tabWeekly.Controls.Add(this.weekSaturday);
            this.tabWeekly.Location = new System.Drawing.Point(4, 22);
            this.tabWeekly.Name = "tabWeekly";
            this.tabWeekly.Size = new System.Drawing.Size(312, 214);
            this.tabWeekly.TabIndex = 1;
            this.tabWeekly.Text = "Weekly";
            // 
            // tabMonthly
            // 
            this.tabMonthly.Controls.Add(this.monthApril);
            this.tabMonthly.Controls.Add(this.monthAugust);
            this.tabMonthly.Controls.Add(this.monthDecember);
            this.tabMonthly.Controls.Add(this.monthNovember);
            this.tabMonthly.Controls.Add(this.monthJuly);
            this.tabMonthly.Controls.Add(this.monthMarch);
            this.tabMonthly.Controls.Add(this.monthFebruary);
            this.tabMonthly.Controls.Add(this.monthJune);
            this.tabMonthly.Controls.Add(this.monthOctober);
            this.tabMonthly.Controls.Add(this.monthSeptember);
            this.tabMonthly.Controls.Add(this.monthMay);
            this.tabMonthly.Controls.Add(this.monthJanuary);
            this.tabMonthly.Controls.Add(this.ordinalDropDown);
            this.tabMonthly.Controls.Add(this.radioOrdinal);
            this.tabMonthly.Controls.Add(this.weekdayDropDown);
            this.tabMonthly.Controls.Add(this.radioDayofMonth);
            this.tabMonthly.Controls.Add(this.dayofmonth);
            this.tabMonthly.Location = new System.Drawing.Point(4, 22);
            this.tabMonthly.Name = "tabMonthly";
            this.tabMonthly.Size = new System.Drawing.Size(312, 214);
            this.tabMonthly.TabIndex = 2;
            this.tabMonthly.Text = "Monthly";
            // 
            // tabTimely
            // 
            this.tabTimely.BackColor = System.Drawing.SystemColors.Control;
            this.tabTimely.Controls.Add(this.timeInterval);
            this.tabTimely.Controls.Add(this.label1);
            this.tabTimely.Controls.Add(this.timeType);
            this.tabTimely.Location = new System.Drawing.Point(4, 22);
            this.tabTimely.Name = "tabTimely";
            this.tabTimely.Padding = new System.Windows.Forms.Padding(3);
            this.tabTimely.Size = new System.Drawing.Size(312, 214);
            this.tabTimely.TabIndex = 3;
            this.tabTimely.Text = "Timely";
            // 
            // timeType
            // 
            this.timeType.FormattingEnabled = true;
            this.timeType.Items.AddRange(new object[] {
            "Seconds",
            "Minutes",
            "Hours"});
            this.timeType.Location = new System.Drawing.Point(108, 28);
            this.timeType.Name = "timeType";
            this.timeType.Size = new System.Drawing.Size(95, 21);
            this.timeType.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Every:";
            // 
            // timeInterval
            // 
            this.timeInterval.Location = new System.Drawing.Point(54, 29);
            this.timeInterval.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.timeInterval.Name = "timeInterval";
            this.timeInterval.Size = new System.Drawing.Size(48, 20);
            this.timeInterval.TabIndex = 3;
            this.timeInterval.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // ScheduleDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(336, 400);
            this.Controls.Add(this.timeStart);
            this.Controls.Add(this.labelStartTime);
            this.Controls.Add(this.labelStartDate);
            this.Controls.Add(this.dateStart);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.tabPages);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScheduleDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Schedule Properties";
            this.Load += new System.EventHandler(this.ScheduleDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dayofmonth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dayInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weekInterval)).EndInit();
            this.tabPages.ResumeLayout(false);
            this.tabDaily.ResumeLayout(false);
            this.tabWeekly.ResumeLayout(false);
            this.tabMonthly.ResumeLayout(false);
            this.tabTimely.ResumeLayout(false);
            this.tabTimely.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeInterval)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		//
		// Load ConfigXml into controls
		//
		private void ScheduleDialog_Load(object sender, System.EventArgs e)
		{
			if (configXml.ChildNodes.Count > 0)
			{
				ScheduleType scheduleType = Schedule.IfExistsExtractScheduleType(configXml);
				this.dateStart.Value = Schedule.IfExistsExtractDate(configXml, "/schedule/startdate", DateTime.Now);
				this.timeStart.Value = Schedule.IfExistsExtractTime(configXml, "/schedule/starttime", DateTime.Now);

				switch(scheduleType)
				{
					case ScheduleType.Daily:
						this.LoadDailySchedule();
						break;
					case ScheduleType.Weekly:
						this.LoadWeeklySchedule();
						break;
					case ScheduleType.Monthly:
						this.LoadMonthlySchedule();
						break;
                    case ScheduleType.Timely:
                        this.LoadTimelySchedule();
                        break;
					default:
						configXml = new XmlDocument();
						break;
				}
			}
		}
		private void LoadDailySchedule()
		{
			this.tabPages.SelectedTab = this.tabDaily;
			this.dayInterval.Value =Convert.ToDecimal( Schedule.IfExistsExtractInt(configXml, "/schedule/interval", 0));
			if (this.dayInterval.Value == 0)
			{
				ScheduleDay days = Schedule.ExtractScheduleDay(configXml, "/schedule/days", false);
				if ((days & ScheduleDay.Sunday) > 0) daySunday.Checked = true;
				if ((days & ScheduleDay.Monday) > 0) dayMonday.Checked = true;
				if ((days & ScheduleDay.Tuesday) > 0) dayTuesday.Checked = true;
				if ((days & ScheduleDay.Wednesday) > 0) dayWednesday.Checked = true;
				if ((days & ScheduleDay.Thursday) > 0) dayThursday.Checked = true;
				if ((days & ScheduleDay.Friday) > 0) dayFriday.Checked = true;
				if ((days & ScheduleDay.Saturday) > 0) daySaturday.Checked = true;
				radioDayInterval.Checked = false;
			}
			else
			{
				radioDayInterval.Checked = true;
			}
		}
		private void LoadWeeklySchedule()
		{
			this.tabPages.SelectedTab = this.tabWeekly;
			this.weekInterval.Value = Convert.ToDecimal(Schedule.IfExistsExtractInt(configXml, "/schedule/interval", 1));
			ScheduleDay days = Schedule.ExtractScheduleDay(configXml, "/schedule/days", false);
			if ((days & ScheduleDay.Sunday) > 0) weekSunday.Checked = true;
			if ((days & ScheduleDay.Monday) > 0) weekMonday.Checked = true;
			if ((days & ScheduleDay.Tuesday) > 0) weekTuesday.Checked = true;
			if ((days & ScheduleDay.Wednesday) > 0) weekWednesday.Checked = true;
			if ((days & ScheduleDay.Thursday) > 0) weekThursday.Checked = true;
			if ((days & ScheduleDay.Friday) > 0) weekFriday.Checked = true;
			if ((days & ScheduleDay.Saturday) > 0) weekSaturday.Checked = true;
		}
		private void LoadMonthlySchedule()
		{
			this.tabPages.SelectedTab = this.tabMonthly;
			this.dayofmonth.Value = Convert.ToDecimal(Schedule.IfExistsExtractInt(configXml, "/schedule/dayofmonth", 0));
			if (this.dayofmonth.Value == 0)
			{
				ScheduleOrdinal ordinal = Schedule.ExtractScheduleOrdinal(configXml, "/schedule/ordinal", false);
				int index = this.ordinalDropDown.Items.IndexOf(ordinal.ToString());
				this.ordinalDropDown.SelectedIndex = index;
				ScheduleDay weekDay = Schedule.ExtractScheduleDay(configXml, "/schedule/weekday", false);
				string strWeekday = weekDay.ToString();
				if (strWeekday == "All"){strWeekday = "Day";}
				index = this.weekdayDropDown.Items.IndexOf(strWeekday);
				this.weekdayDropDown.SelectedIndex = index;
				radioDayofMonth.Checked = false;
			}
			else
			{
				radioDayofMonth.Checked = true;
			}
			ScheduleMonth months =Schedule. ExtractScheduleMonth(configXml, "/schedule/months", false);
			if ((months & ScheduleMonth.February) > 0) monthFebruary.Checked = true;
			if ((months & ScheduleMonth.March) > 0) monthMarch.Checked = true;
			if ((months & ScheduleMonth.April) > 0) monthApril.Checked = true;
			if ((months & ScheduleMonth.May) > 0) monthMay.Checked = true;
			if ((months & ScheduleMonth.June) > 0) monthJune.Checked = true;
			if ((months & ScheduleMonth.July) > 0) monthJuly.Checked = true;
			if ((months & ScheduleMonth.August) > 0) monthAugust.Checked = true;
			if ((months & ScheduleMonth.September) > 0) monthSeptember.Checked = true;
			if ((months & ScheduleMonth.October) > 0) monthOctober.Checked = true;
			if ((months & ScheduleMonth.November) > 0) monthNovember.Checked = true;
			if ((months & ScheduleMonth.December)> 0) monthDecember.Checked = true;
		}
        private void LoadTimelySchedule()
        {
            this.tabPages.SelectedTab = this.tabTimely;
            this.timeInterval.Value = Convert.ToDecimal(Schedule.IfExistsExtractInt(configXml, "/schedule/interval", 0));

            ScheduleTimeType scheduleTimeType = Schedule.ExtractScheduleTimeType(configXml, "/schedule/timeintervalltype", false);

            if (scheduleTimeType == ScheduleTimeType.Seconds) 
                timeType.Text = "Seconds";
            if (scheduleTimeType == ScheduleTimeType.Minutes)
                timeType.Text="Minutes";
            if (scheduleTimeType == ScheduleTimeType.Hours) 
                timeType.Text = "Hours";
            

        }
		//
		// Unload controls into ConfigXml
		//
		private void ScheduleDialog_UnLoad(object sender, System.EventArgs e)
		{
			configXml = new XmlDocument();
			XmlNode root = configXml.CreateNode("element", "schedule", "");
			XmlNode startdate = configXml.CreateNode("element", "startdate", "");
			XmlNode starttime = configXml.CreateNode("element", "starttime", "");
			startdate.InnerText = dateStart.Value.ToString("yyyy-MM-dd");
			starttime.InnerText = timeStart.Value.ToString("HH:mm");
			root.AppendChild(startdate);
			root.AppendChild(starttime);
			configXml.AppendChild(root);
			
			switch(this.tabPages.SelectedTab.Text)
			{
				case "Daily":
					this.UnloadDailySchedule();
					break;
				case "Weekly":
					this.UnloadWeeklySchedule();
					break;
				case "Monthly":
					this.UnloadMonthlySchedule();
					break;
                case "Timely":
                    this.UnloadTimelySchedule();
                    break;
				default:
					break;
			}	
		}
		private void UnloadDailySchedule()
		{
			configXml.DocumentElement.SetAttribute("type", "", "Daily");
			if (radioDayInterval.Checked)
			{
				if (this.dayInterval.Value == 0)
				{
					throw(new ApplicationException("Must select a daily interval"));
				}
				XmlNode interval = configXml.CreateNode("element", "interval","");
				interval.InnerText = dayInterval.Value.ToString();
				configXml.DocumentElement.AppendChild(interval);
			}
			else
			{
				ScheduleDay result = ScheduleDay.None;
				if (this.daySunday.Checked) {result = result | ScheduleDay.Sunday;}
				if (this.dayMonday.Checked) {result = result | ScheduleDay.Monday;}
				if (this.dayTuesday.Checked) {result = result | ScheduleDay.Tuesday;}
				if (this.dayWednesday.Checked) {result = result | ScheduleDay.Wednesday;}
				if (this.dayThursday.Checked) {result = result | ScheduleDay.Thursday;}
				if (this.dayFriday.Checked) {result = result | ScheduleDay.Friday;}
				if (this.daySaturday.Checked) {result = result | ScheduleDay.Saturday;}
				if (result == ScheduleDay.None)
				{
					throw(new ApplicationException("Must select one or more days of the week"));
				}
				else
				{
					XmlNode days = configXml.CreateNode("element", "days","");
					days.InnerText = result.ToString();
					configXml.DocumentElement.AppendChild(days);
				}
			}
		}
		private void UnloadWeeklySchedule()
		{
			configXml.DocumentElement.SetAttribute("type", "", "Weekly");
			XmlNode interval = configXml.CreateNode("element", "interval","");
			interval.InnerText = weekInterval.Value.ToString();
			configXml.DocumentElement.AppendChild(interval);

			ScheduleDay result = ScheduleDay.None;
			if (this.weekSunday.Checked) {result = result | ScheduleDay.Sunday;}
			if (this.weekMonday.Checked) {result = result | ScheduleDay.Monday;}
			if (this.weekTuesday.Checked) {result = result | ScheduleDay.Tuesday;}
			if (this.weekWednesday.Checked) {result = result | ScheduleDay.Wednesday;}
			if (this.weekThursday.Checked) {result = result | ScheduleDay.Thursday;}
			if (this.weekFriday.Checked) {result = result | ScheduleDay.Friday;}
			if (this.weekSaturday.Checked) {result = result | ScheduleDay.Saturday;}
			if (result == ScheduleDay.None)
			{
				throw(new ApplicationException("Must select one or more days of the week"));
			}
			else
			{
				XmlNode days = configXml.CreateNode("element", "days","");
				days.InnerText = result.ToString();
				configXml.DocumentElement.AppendChild(days);
			}
		}
		private void UnloadMonthlySchedule()
		{
			configXml.DocumentElement.SetAttribute("type", "", "Monthly");
			if (this.radioDayofMonth.Checked)
			{
				if (this.dayofmonth.Value == 0)
				{
					throw(new ApplicationException("Must select a day of the month"));
				}
				XmlNode dayofmonth = configXml.CreateNode("element", "dayofmonth","");
				dayofmonth.InnerText = this.dayofmonth.Value.ToString();
				configXml.DocumentElement.AppendChild(dayofmonth);
			}
			else
			{
				if (this.ordinalDropDown.SelectedItem == null)
				{
					throw(new ApplicationException("Must select an ordinal day"));
				}
				XmlNode ordinal = configXml.CreateNode("element", "ordinal","");
				ordinal.InnerText = this.ordinalDropDown.SelectedItem.ToString();
				configXml.DocumentElement.AppendChild(ordinal);
				
				if (this.weekdayDropDown.SelectedItem == null)
				{
					throw(new ApplicationException("Must select an ordinal week day"));
				}
				XmlNode weekday = configXml.CreateNode("element", "weekday","");
				weekday.InnerText = this.weekdayDropDown.SelectedItem.ToString();
				if (weekday.InnerText == "Day"){	weekday.InnerText = "All";}
				configXml.DocumentElement.AppendChild(weekday);

			}
			ScheduleMonth result = ScheduleMonth.None;
			if (this.monthJanuary.Checked) {result = result | ScheduleMonth.January;}
			if (this.monthFebruary.Checked) {result = result | ScheduleMonth.February;}
			if (this.monthMarch.Checked) {result = result | ScheduleMonth.March;}
			if (this.monthApril.Checked) {result = result | ScheduleMonth.April;}
			if (this.monthMay.Checked) {result = result | ScheduleMonth.May;}
			if (this.monthJune.Checked) {result = result | ScheduleMonth.June;}
			if (this.monthJuly.Checked) {result = result | ScheduleMonth.July;}
			if (this.monthAugust.Checked) {result = result | ScheduleMonth.August;}
			if (this.monthSeptember.Checked) {result = result | ScheduleMonth.September;}
			if (this.monthOctober.Checked) {result = result | ScheduleMonth.October;}
			if (this.monthNovember.Checked) {result = result | ScheduleMonth.November;}
			if (this.monthDecember.Checked) {result = result | ScheduleMonth.December;}
			if (result == ScheduleMonth.None)
			{
				throw(new ApplicationException("Must select one or more months"));
			}
			else
			{
				XmlNode months = configXml.CreateNode("element", "months","");
				months.InnerText = result.ToString();
				configXml.DocumentElement.AppendChild(months);
			}
		}
        private void UnloadTimelySchedule()
        {
            configXml.DocumentElement.SetAttribute("type", "", "Timely");

            XmlNode intervalNode = configXml.CreateNode("element", "interval", "");
            intervalNode.InnerText = this.timeInterval.Value.ToString();
            configXml.DocumentElement.AppendChild(intervalNode);
        
            ScheduleTimeType result = ScheduleTimeType.Seconds;
            switch (timeType.Text.ToUpper())
            {
                case "HOURS":
                    result = ScheduleTimeType.Hours;
                    break;
                case "MINUTES":
                    result = ScheduleTimeType.Minutes;
                    break;
                default:
                    result = ScheduleTimeType.Seconds;
                    break;
            }
        
            XmlNode intervalType = configXml.CreateNode("element", "timeintervalltype", "");
            intervalType.InnerText = result.ToString();
            configXml.DocumentElement.AppendChild(intervalType);
        
        }
		//
		private void radioDayInterval_CheckedChanged(object sender, System.EventArgs e)
		{
			if (radioDayInterval.Checked == true)
			{
				dayInterval.Enabled = true;
				daySunday.Enabled = false;
				dayMonday.Enabled = false;
				dayTuesday.Enabled = false;
				dayWednesday.Enabled = false;
				dayThursday.Enabled = false;
				dayFriday.Enabled = false;
				daySaturday.Enabled = false;
				
			}
			else
			{
				dayInterval.Enabled = false;
				daySunday.Enabled = true;
				dayMonday.Enabled = true;
				dayTuesday.Enabled = true;
				dayWednesday.Enabled = true;
				dayThursday.Enabled = true;
				dayFriday.Enabled = true;
				daySaturday.Enabled = true;
			}
		}
		private void radioDayofMonth_CheckedChanged(object sender, System.EventArgs e)
		{
			if (radioDayofMonth.Checked == true)
			{
				this.dayofmonth.Enabled = true;
				this.weekdayDropDown.Enabled = false;
				this.ordinalDropDown.Enabled = false;
			}
			else
			{
				this.dayofmonth.Enabled = false;
				this.weekdayDropDown.Enabled = true;
				this.ordinalDropDown.Enabled = true;
			}
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.None;
			ScheduleDialog_UnLoad(sender,  e);
			this.DialogResult = DialogResult.OK;
		}
	}
}
