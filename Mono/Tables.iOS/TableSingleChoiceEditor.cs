using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace Tables.iOS
{
	public delegate void SingleChoiceChangedDelegate(Object changedChoice);

	public class TableSingleChoiceEditor : TableEditor, IUIPickerViewDataSource
    {
		private SingleChoiceChangedDelegate choiceChanged;
		private TableRowType rowType;
		private UIPickerView picker;
		private List<Object> options;
		private Object chosenOption;
		private TableAdapterRowConfig config;

		public TableSingleChoiceEditor(TableRowType rowType,string title,Object chosenOption,TableAdapterRowConfig config,SingleChoiceChangedDelegate delg)
        {
            this.Title = title;
			this.choiceChanged = delg;
			this.rowType = rowType;
			this.chosenOption = chosenOption;
			this.config = config;
			this.options = config != null ? config.SingleChoiceOptions : null;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			View.BackgroundColor = UIColor.White;

            if (NavigationItem != null)
            {
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, ClickedCancel);
                NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, ClickedDone);
            }

			picker = new UIPickerView (View.Bounds);
			picker.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			picker.WeakDelegate = this;
			picker.DataSource = this;
			View.AddSubview (picker);
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
					theChoice = options[index];
				choiceChanged (theChoice);
			}
			CloseViewController ();
        }

		#region Datasource

		public int GetComponentCount (UIPickerView pickerView)
		{
			return 1;
		}

		public int GetRowsInComponent (UIPickerView pickerView, int component)
		{
			return options == null ? 0 : options.Count;
		}

		#endregion

		#region Delegate

		[Export ("pickerView:titleForRow:forComponent:")]
		public string TitleForRowAndComponent(UIPickerView pickerView,int row,int component)
		{
			string returnValue = null;
			if (options != null)
			{
				var anObject = options [row];
				returnValue = anObject.ToString ();
			}
			return returnValue;
		}

		#endregion
    }
}
