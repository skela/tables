using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Tables.iOS
{
    public class TableAdapter : NSObject,ITableAdapter
    {
        public ITableAdapterRowSelector RowSelector {get;set;}
        public ITableAdapterRowConfigurator RowConfigurator {get;set;}
        public ITableAdapterRowChanged RowChanged {get;set;}
        private  UITableView tv;
        private TableSource td;

        public TableAdapter(UITableView table=null,Object data=null,ITableAdapterRowConfigurator configs=null) : base()
        {
            td = new TableSource();
            ListView = table;
            RowConfigurator = configs;
            Data = data;
        }

        public UITableView ListView
        {
            get
            {
                return tv;
            }
            set
            {
                if (tv != null)
                {
                    tv.WeakDataSource = null;
                    tv.WeakDelegate = null;
                }
                tv = value;
                if (tv != null)
                {
                    tv.WeakDelegate = this;
                    tv.WeakDataSource = this;
                }
            }
        }

        public Object Data
        {
            get
            {
                return td.Data;             
            }
            set
            {
                td = new TableSource(value);
                ReloadData();
            }
        }

        public void ReloadData()
        {
            if (tv != null)
                tv.ReloadData();
        }

        [Export ("numberOfSectionsInTableView:")]
        public int NumberOfSections (UITableView tableView)
        {
            return 1;
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection (UITableView tableView, int section)
        {
            return td.RowsInSection(section);
        }

        static UIFont titleFont = UIFont.SystemFontOfSize(16);
        static UIFont detailFont = UIFont.FromName ("Helvetica", 14);

        [Export("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var rowType = td.RowType(RowConfigurator,indexPath.Row,indexPath.Section);
            if (rowType == TableRowType.Blurb)
            {
                var name = td.DisplayName(RowConfigurator,indexPath.Row, indexPath.Section);
                var value = td.GetValue(indexPath.Row, indexPath.Section);
                string text = value as string;
				if (text == null) text = "";
                float contentMargin = 10;
                float contentWidth = tableView.Bounds.Width;

                SizeF constraint = new SizeF(contentWidth - (contentMargin * 2), 20000.0f);
                SizeF size = tv.StringSize (text, detailFont, constraint, UILineBreakMode.WordWrap);
                SizeF sizeT = tv.StringSize(name, titleFont);

                float height = Math.Max(size.Height,44.0f) + sizeT.Height;

                return height + (contentMargin * 4);
            }
            return 44;
        }

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var name = td.GetName(indexPath.Row, indexPath.Section);
            var editable = td.Editable(RowConfigurator,indexPath.Row,indexPath.Section);
            var rowType = td.RowType(RowConfigurator,indexPath.Row,indexPath.Section);
            var value = td.GetValue(indexPath.Row, indexPath.Section);

            UITableViewCell cell = tableView.DequeueReusableCell(name);
            if (cell == null )
            {
                cell = new UITableViewCell(rowType==TableRowType.Blurb?UITableViewCellStyle.Subtitle:UITableViewCellStyle.Value1,name);
                if (rowType==TableRowType.Checkbox)
                {
                    var s = td.RowSetting(RowConfigurator,name);
                    if (s == null || !s.SimpleCheckbox)
                    {
                        var sw = new UISwitch();
                        sw.UserInteractionEnabled = false;
                        cell.AccessoryView = sw;
                    }
                }
                if (rowType == TableRowType.Blurb)
                {
                    cell.TextLabel.Font = titleFont;
                    cell.DetailTextLabel.Lines = 0;
                    cell.DetailTextLabel.Font = detailFont;
                }
            }

            cell.TextLabel.Text = td.DisplayName(RowConfigurator,indexPath.Row,indexPath.Section);
            cell.SelectionStyle = editable ? UITableViewCellSelectionStyle.Default : UITableViewCellSelectionStyle.None;

            switch (rowType)
            {
                case TableRowType.Text:
                    cell.DetailTextLabel.Text = value as string; 
                    cell.Accessory = editable ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;
                break;
                case TableRowType.Blurb:
                    cell.DetailTextLabel.Text = value as string; 
                    cell.Accessory = editable ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;
                break;
                case TableRowType.Checkbox:
                    cell.DetailTextLabel.Text = "";
                    var s = td.RowSetting(RowConfigurator,name);
                    if (s!=null && s.SimpleCheckbox)
                        cell.Accessory = ((bool)value) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
                    else
                        (cell.AccessoryView as UISwitch).On = (bool)value;
                break;
                case TableRowType.DateTime:
                case TableRowType.Date:
                case TableRowType.Time:
                    cell.DetailTextLabel.Text = td.DisplayDate(RowConfigurator, indexPath.Row, indexPath.Section, (DateTime)value, rowType);
                    cell.Accessory = editable ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;
                break;
            }

            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected (UITableView tableView, NSIndexPath indexPath)
        {
            var name = td.GetName(indexPath.Row,indexPath.Section);
            var value = td.GetValue(indexPath.Row,indexPath.Section);
            if (!td.Editable(RowConfigurator,indexPath.Row,indexPath.Section))
                return;
            var rowType = td.RowType(RowConfigurator,indexPath.Row,indexPath.Section);

            if (RowSelector != null)
            if (RowSelector.DidSelectRow(this,name))
                return;

            TableAdapterRowConfig settings = td.RowSetting(RowConfigurator,name);
            if (settings != null)
            {
                if (settings.Clicked != null)
                {
                    settings.Clicked(this, null);
                    return;
                }
            }

            switch (rowType)
            {
                case TableRowType.Checkbox:
                    var obj = !(bool)value;
                    td.SetValue(obj, indexPath.Row, indexPath.Section);
                    tableView.ReloadRows(new NSIndexPath[]{ indexPath }, UITableViewRowAnimation.Fade);
                    ChangedValue(name,value,obj);
                break;
                
                case TableRowType.Text:
                case TableRowType.Blurb:
                    var tvc = FirstAvailableViewController;
                    if (tvc != null)
                    {
                        string str = value as string;
                        var dname = td.DisplayName(RowConfigurator, indexPath.Row, indexPath.Section);
						var textEditor = new TableTextEditor(dname,str,delegate(string changedString)
						{
							td.SetValue(changedString, indexPath.Row, indexPath.Section);
							ReloadData();
						});
						if (tvc.NavigationController == null)
							tvc.PresentViewController (new UINavigationController (textEditor), true, null);
						else
							tvc.NavigationController.PushViewController (textEditor, true);
                    }
                break;
                case TableRowType.Date:
                case TableRowType.Time:
                case TableRowType.DateTime:
                    var dvc = FirstAvailableViewController;
                    if (dvc != null)
                    {
                        DateTime str = (DateTime)value;
                        var dname = td.DisplayName(RowConfigurator, indexPath.Row, indexPath.Section);
                        UIDatePickerMode mode = UIDatePickerMode.DateAndTime;
                        if (rowType == TableRowType.Date)
                            mode = UIDatePickerMode.Date;
                        else if (rowType == TableRowType.Time)
                            mode = UIDatePickerMode.Time;

						var dateEditor = new TableTimeEditor (dname, str, mode, delegate(DateTime changedDate)
						{
							td.SetValue (changedDate, indexPath.Row, indexPath.Section);
							ReloadData ();
						});
						if (dvc.NavigationController == null)
							dvc.PresentViewController(new UINavigationController(dateEditor), true, null);
						else
							dvc.NavigationController.PushViewController (dateEditor, true);							
                    }
                break;
            }
        }

        private void ChangedValue(string rowName,object oldValue,object newValue)
        {
            if (RowChanged != null)
                RowChanged.RowChanged(this, rowName, oldValue, newValue);
        }

        private UIViewController FirstAvailableViewController
        {
            get
            {
                return TableAdapter.TraverseResponderChainForViewController(tv) as UIViewController;
            }
        }

        static private UIResponder TraverseResponderChainForViewController(UIView v)
        {
            UIResponder responder = v.NextResponder;
            if (responder is UIViewController)
            {
                return responder;
            }
            else if (responder is UIView)
            {
                return TraverseResponderChainForViewController(responder as UIView);
            }
            else
            {
                return null;
            }
        }
    }
}
