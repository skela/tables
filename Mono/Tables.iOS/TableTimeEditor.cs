using System;
using MonoTouch.UIKit;

namespace Tables.iOS
{
    public delegate void DateChangedDelegate(DateTime changedDate);

	public class TableTimeEditor : TableEditor
    {
        private DateTime value;
        private UIDatePicker picker;
        private DateChangedDelegate dateChanged;
        private UIDatePickerMode mode;

        public TableTimeEditor(string title,DateTime value,UIDatePickerMode mode,DateChangedDelegate delg)
        {
            this.Title = title;
            this.value = value;
            this.mode = mode;
            this.dateChanged = delg;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;

            if (NavigationItem != null)
            {
                NavigationItem.LeftBarButtonItem  = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, ClickedCancel);
                NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, ClickedDone);
            }
            picker = new UIDatePicker(View.Bounds);
            picker.Center = View.Center;
            picker.Mode = mode;
            picker.Date = value;
            View.AddSubview(picker);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            picker.Center = View.Center;
        }

        private void ClickedCancel(object obj,EventArgs e)
        {
			CloseViewController ();            
        }

        private void ClickedDone(object obj,EventArgs e)
        {
            if (dateChanged != null)
                dateChanged(picker.Date);
			CloseViewController ();            
        }
    }
}