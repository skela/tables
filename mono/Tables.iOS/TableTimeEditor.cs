using System;
using UIKit;
using Foundation;

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

            View.BackgroundColor = BackgroundColor;

            if (NavigationItem != null)
            {
                NavigationItem.LeftBarButtonItem  = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, ClickedCancel);
                NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, ClickedDone);
            }
            picker = new UIDatePicker(View.Bounds);
            picker.SetValueForKey(TextColor,new NSString("textColor"));
            picker.Center = View.Center;
            picker.Mode = mode;
			picker.Date = (NSDate)value;
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
				dateChanged((DateTime)picker.Date);
			CloseViewController ();            
        }
    }    
}