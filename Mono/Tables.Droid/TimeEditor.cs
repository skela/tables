using System;

using Java.Util;
using Java.Text;

using Android.OS;
using Android.App;
using Android.Widget;
using Android.Content;

namespace Tables.Droid
{
    public interface TimeEditorListener
    {
        void ChangedDate(TimeEditor fragment,DateTime changedDate);
    }

    public class TimeEditor : Android.App.DialogFragment
    {
        private DateTime value;
        private TableRowType mode;
        private bool hasShownHack = false;

        public TimeEditor() : base()
        {

        }

        public static TimeEditor CreateFragment(TimeEditorListener listener,DateTime value,TableRowType mode)
        {
            var args = new Bundle();
            args.PutInt("mode",(int)mode);
            args.PutLong("value", value.ToFileTime());
            var fragment = new TimeEditor();
            fragment.Arguments = args;
            return fragment;
        }

        TimeEditorListener Listener
        {
            get
            {
                TimeEditorListener listener = null;
                if (Activity is TimeEditorListener)
                {
                    listener = Activity as TimeEditorListener;
                }
                else
                {
                    var adapter = TableEditor.AdapterForActivity(Activity);
                    if (adapter is TimeEditorListener)
                    {
                        listener = adapter as TimeEditorListener;
                    }
                }
                return listener;
            }
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState) 
        {
            value = DateTime.FromFileTime(Arguments.GetLong("value", DateTime.Now.ToFileTime()));
            mode = (TableRowType)Arguments.GetInt("mode", (int)TableRowType.DateTime);

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
            var listener = Listener;
            if (listener != null)
                listener.ChangedDate(this,new DateTime(now.Year, now.Month, now.Day, e.HourOfDay, e.Minute, 0));
        }

        public void ChangedDate(object obj,Android.App.DatePickerDialog.DateSetEventArgs e)
        {
            var listener = Listener;
            if (listener != null)
                listener.ChangedDate(this,e.Date);
        }

        public void ChangedDateTime(object obj,Android.App.DatePickerDialog.DateSetEventArgs e)
        {
            var listener = Listener;
            if (listener != null)
                listener.ChangedDate(this,e.Date);
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

            var listener = Listener;
            if (listener != null)
                listener.ChangedDate(this,value);
        }
    }
}
