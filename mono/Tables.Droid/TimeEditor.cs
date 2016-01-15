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
        void ChangedDate(Android.App.DialogFragment fragment,DateTime changedDate);
    }

    public class TimeEditor : Android.App.DialogFragment
    {
        private DateTime value;
        private TableRowType mode;

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
            int year = value.Year;
            int month = value.Month-1;
            int day = value.Day;

            if (mode == TableRowType.Time)
                return new TimePickerDialog(Activity, ChangedTime, hour, minute, true);
            else if (mode == TableRowType.Date)
                return new DatePickerDialog(Activity, ChangedDate, year, month, day);
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

        public void ChangedTimeForDateTimePicker(object obj,Android.App.TimePickerDialog.TimeSetEventArgs e)
        {    
            value = new DateTime(value.Year, value.Month, value.Day, e.HourOfDay, e.Minute, value.Second);
            var newFragment = SecondTimeEditor.CreateFragment(value);
            newFragment.Show(FragmentManager, "dateTimePicker");
        }

        public void ChangedDateForDateTimePicker(object obj,Android.App.DatePickerDialog.DateSetEventArgs e)
        {
            value = new DateTime(e.Year, e.MonthOfYear, e.DayOfMonth, value.Hour, value.Minute, value.Second);

            var listener = Listener;
            if (listener != null)
                listener.ChangedDate(this,value);
        }
    }

    public class SecondTimeEditor : Android.App.DialogFragment
    {
        int year;
        int month;
        int day;
        int hour;
        int minute;
        int second;

        public static SecondTimeEditor CreateFragment(DateTime value)
        {            
            var hour = value.Hour;
            var minute = value.Minute;
            var second = value.Second;
            var year = value.Year;
            var month = value.Month-1;
            var day = value.Day;

            var args = new Bundle();
            args.PutInt("hour", hour);
            args.PutInt("minute", minute);
            args.PutInt("second", second);
            args.PutInt("year", year);
            args.PutInt("month", month);
            args.PutInt("day", day);

            var fragment = new SecondTimeEditor();
            fragment.Arguments = args;
            return fragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            hour = Arguments.GetInt("hour", DateTime.Now.Hour);
            minute = Arguments.GetInt("minute", DateTime.Now.Minute);
            second = Arguments.GetInt("second", DateTime.Now.Second);

            year = Arguments.GetInt("year", DateTime.Now.Year);
            month = Arguments.GetInt("month", DateTime.Now.Month);
            day = Arguments.GetInt("day", DateTime.Now.Day);

            return new DatePickerDialog(Activity, ChangedDate, year, month, day);
        }

        public void ChangedDate(object obj,Android.App.DatePickerDialog.DateSetEventArgs e)
        {
            year = e.Year;
            month = e.MonthOfYear + 1;
            day = e.DayOfMonth;

            var value = new DateTime(year, month, day, hour, minute, second);

            var listener = Listener;
            if (listener != null)
                listener.ChangedDate(this,value);
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
    }
}
