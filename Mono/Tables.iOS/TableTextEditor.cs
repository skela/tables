using System;
using MonoTouch.UIKit;

namespace Tables.iOS
{
    public delegate void TextChangedDelegate(string changedString);

	public class TableTextEditor : TableEditor
    {
        private string value;
        private UITextView text;
        private TextChangedDelegate textChanged;

        public TableTextEditor(string title,string value,TextChangedDelegate delg)
        {
            this.Title = title;
            this.value = value;
            this.textChanged = delg;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (NavigationItem != null)
            {
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, ClickedCancel);
                NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, ClickedDone);
            }
            text = new UITextView(View.Bounds);
            text.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            text.Text = value;
            View.AddSubview(text);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            text.BecomeFirstResponder();
        }

        private void ClickedCancel(object obj,EventArgs e)
        {
			CloseViewController ();
        }

        private void ClickedDone(object obj,EventArgs e)
        {
            if (textChanged != null)
                textChanged(text.Text);            
			CloseViewController ();
        }
    }
}
