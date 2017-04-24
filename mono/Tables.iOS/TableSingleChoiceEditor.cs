using System;
using UIKit;
using CoreGraphics;
using Foundation;
using System.Collections.Generic;

namespace Tables.iOS
{
	public delegate void SingleChoiceChangedDelegate(Object changedChoice);

	public class TableSingleChoiceEditor : TableEditor, IUIPickerViewDataSource
    {
		private SingleChoiceChangedDelegate choiceChanged;		
		private UIPickerView picker;
		private List<Object> options;
		private Object chosenOption;
		//private TableAdapterRowConfig config;
        //private TableRowType rowType;

		public TableSingleChoiceEditor(TableRowType rowType,string title,Object chosenOption,TableAdapterRowConfig config,SingleChoiceChangedDelegate delg)
        {
            this.Title = title;
			this.choiceChanged = delg;			
			this.chosenOption = chosenOption;
			this.options = config != null ? config.SingleChoiceOptions : null;
            //this.config = config;
            //this.rowType = rowType;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			View.BackgroundColor = BackgroundColor;

            if (NavigationItem != null)
            {
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, ClickedCancel);
                NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, ClickedDone);
            }

			picker = new UIPickerView (View.Bounds);
            picker.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleWidth;
            picker.WeakDelegate = this;			
			picker.DataSource = this;
			View.AddSubview (picker);
        }
			
        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            picker.Center = View.Center;
        }

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			picker.Center = View.Center;
		}

        public override void ViewDidAppear(bool animated)
        {
			base.ViewDidAppear(animated);

			if (options!=null && chosenOption!=null)
			{
				int index = options.IndexOf (chosenOption);
				picker.Select (index, 0, true);
			}
        }

        private void ClickedCancel(object obj,EventArgs e)
        {
			CloseViewController ();
        }

        private void ClickedDone(object obj,EventArgs e)
        {
			if (choiceChanged != null)
			{
				var index = picker.SelectedRowInComponent (0);
				Object theChoice = null;
				if (options!=null)
					theChoice = options[(int)index];
				choiceChanged (theChoice);
			}
			CloseViewController ();
        }

		#region Datasource

		public nint GetComponentCount (UIPickerView pickerView)
		{
			return 1;
		}

		public nint GetRowsInComponent (UIPickerView pickerView, nint component)
		{
			return options == null ? 0 : options.Count;
		}
		
		#endregion

		#region Delegate
		
		[Export("pickerView:titleForRow:forComponent:")]
		public string GetTitle(UIPickerView pickerView,nint row,nint component)
		{
			string returnValue = null;
			if (options != null)
			{
				var anObject = options [(int)row];
				returnValue = anObject.ToString ();
			}
			return returnValue;
		}
		
		[Export("pickerView:widthForComponent:")]
		public nfloat GetComponentWidth (UIPickerView picker, nint component)
		{
			return View.Bounds.Width;
		}
		
		[Export("pickerView:rowHeightForComponent:")]
		public nfloat GetRowHeight (UIPickerView picker, nint component)
		{
			return 40;
		}
		
		[Export("pickerView:viewForRow:forComponent:reusingView:")]
		public UIView GetView (UIPickerView picker, nint row, nint component, UIView view)
		{
			if (view!=null)
			{
				var lab = view as UILabel;
				lab.Text = GetTitle(picker,row,component);				
				return view;
			}
			
			var rect = new CGRect(0f,0f,GetComponentWidth(picker,component)-4,GetRowHeight(picker,component));
			
			var label = new UILabel(rect)
			{
				Text = GetTitle(picker,row,component),				
				MinimumFontSize = 10,
				AdjustsFontSizeToFitWidth = true,
				TextAlignment = UITextAlignment.Center,
				TextColor = TextColor,
				Opaque = false,
				BackgroundColor = UIColor.Clear
			};
			
			return label;
		}
		
		#endregion
    }
}
