using System;

using Java.Util;
using Java.Text;

using Android.OS;
using Android.App;
using Android.Widget;
using Android.Content;

namespace Tables.Droid
{
    public class TableTimeEditor : Android.Support.V4.App.DialogFragment
    {
        public delegate void DateChangedDelegate(DateTime changedDate);

        private DateTime value;
        private TableRowType mode;
        private DateChangedDelegate dateChanged;
        private bool hasShownHack = false;

        public TableTimeEditor(DateTime value,TableRowType mode,DateChangedDelegate delg)
        {
            this.value = value;
            this.mode = mode;
            this.dateChanged = delg;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState) 
        {
            int hour = value.Hour;
            int minute = value.Minute;
            if (mode == TableRowType.Time)
                return new TimePickerDialog(Activity, ChangedTime, hour, minute, true);
            else if (mode == TableRowType.Date)
                return new DatePickerDialog(Activity, ChangedDate, value.Year, value.Month, value.Day);
            else
                return new TimePickerDialog(Activity, ChangedTimeForDateTimePicker, hour, minute, true);
        }

        public void ChangedTime(object obj,Android.App.TimePickerDialog.TimeSetEventArgs e)
        {        
            var now = DateTime.Now;
            if (dateChanged != null)
                dateChanged(new DateTime(now.Year, now.Month, now.Day, e.HourOfDay, e.Minute, 0));
        }

        public void ChangedDate(object obj,Android.App.DatePickerDialog.DateSetEventArgs e)
        {
            if (dateChanged != null)
                dateChanged(e.Date);
        }

        public void ChangedDateTime(object obj,Android.App.DatePickerDialog.DateSetEventArgs e)
        {
            if (dateChanged != null)
                dateChanged(e.Date);
        }

        public void ChangedTimeForDateTimePicker(object obj,Android.App.TimePickerDialog.TimeSetEventArgs e)
        {    
            if (hasShownHack)
                return;

            value = new DateTime(value.Year, value.Month, value.Day, e.HourOfDay, e.Minute, value.Second);

            var d = new DatePickerDialog(Activity, ChangedDateForDateTimePicker, value.Year, value.Month, value.Day);

            d.Show();

            hasShownHack = true;
        }

        public void ChangedDateForDateTimePicker(object obj,Android.App.DatePickerDialog.DateSetEventArgs e)
        {
            value = new DateTime(e.Year, e.MonthOfYear, e.DayOfMonth, value.Hour, value.Minute, value.Second);

            if (dateChanged != null)
                dateChanged(value);
        }
    }
}
