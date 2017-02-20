using System;
using CoreGraphics;

using Foundation;
using UIKit;
using System.Collections.Generic;

namespace Tables.iOS
{
    public class TableAdapter : NSObject,ITableAdapter
    {
        public ITableAdapterRowSelector RowSelector {get;set;}
        public ITableAdapterRowConfigurator RowConfigurator {get;set;}
        public ITableAdapterRowChanged RowChanged {get;set;}
        private UITableView tv;
        private TableSource td;

		public bool ShouldAdjustTextContentInset=true;
		public UIColor DetailTextColor = UIColor.DarkGray;

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
			return td.NumberOfSections();
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection (UITableView tableView, int section)
        {
            return td.RowsInSection(section);
        }

        static UIFont titleFont = UIFont.SystemFontOfSize(16);
        static UIFont detailFont = UIFont.FromName ("Helvetica", 14);

		public nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			var rowType = td.RowType(RowConfigurator,indexPath.Row,indexPath.Section);
			if (rowType == TableRowType.Blurb)
			{
				var name = td.DisplayName(RowConfigurator,indexPath.Row, indexPath.Section);
				var value = td.GetValue(indexPath.Row, indexPath.Section);
				string text = value as string;
				if (text == null) text = "";
				nfloat contentMargin = 10;
				var contentWidth = tableView.Bounds.Width;

				CGSize constraint = new CGSize(contentWidth - (contentMargin * 2), 20000.0f);
				CGSize size = UIKit.UIStringDrawing.StringSize (text, detailFont, constraint, UILineBreakMode.WordWrap);
				CGSize sizeT = UIKit.UIStringDrawing.StringSize(name, titleFont);

				var height = TableEditor.Max(size.Height,44.0f) + sizeT.Height;

				return (height + (contentMargin * 4));
			}
			return 44;
		}

        [Export("tableView:heightForRowAtIndexPath:")]
        public nfloat HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
			return GetHeightForRow (tableView, indexPath);
        }

		[Export("tableView:estimatedHeightForRowAtIndexPath:")]
		public nfloat EstimatedHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return GetHeightForRow (tableView, indexPath);
		}

        public virtual TableAdapterSectionConfig GetSectionConfig(nint section)
        {
            if (td != null)
            {
                var config = td.SectionConfig((int)section);
                return config;
            }
            return null;
        }

        [Export("tableView:titleForFooterInSection:")]
        public string TitleForFooter(UITableView tableView, nint section)
        {
            var config = GetSectionConfig(section);
            return config == null ? null : config.Footer;
        }

        [Export("tableView:titleForHeaderInSection:")]
        public virtual string TitleForHeader(UITableView tableView, nint section)
        {
            var config = GetSectionConfig(section);
            return config == null ? null : config.Header;
        }

		public class TableAdapterCell : UITableViewCell
		{
			public TableAdapterCell (UITableViewCellStyle style, string ident) : base(style,ident)
			{

			}

			public override void LayoutSubviews ()
			{
				base.LayoutSubviews ();

				if (AccessoryView is UITextField)
				{
					TextLabel.SizeToFit ();
					var w = Bounds.Width;
					var x = TextLabel.Frame.X + TextLabel.Frame.Width + 10;
					AccessoryView.Frame = new CGRect (x, 0, w - x - 10, 44);
				}
			}
		}

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var name = td.GetName(indexPath.Row, indexPath.Section);
            var editable = td.Editable(RowConfigurator,indexPath.Row,indexPath.Section);
            var rowType = td.RowType(RowConfigurator,indexPath.Row,indexPath.Section);
            var value = td.GetValue(indexPath.Row, indexPath.Section);

			TableAdapterCell cell = tableView.DequeueReusableCell(name) as TableAdapterCell;
            if (cell == null )
            {
				cell = new TableAdapterCell(rowType==TableRowType.Blurb?UITableViewCellStyle.Subtitle:UITableViewCellStyle.Value1,name);
				cell.Accessory = UITableViewCellAccessory.None;
				//cell = new TableAdapterCell(UITableViewCellStyle.Subtitle,name);

				switch (rowType)
				{
					case TableRowType.Checkbox:
					{
                        var c = td.RowSetting(RowConfigurator,name,indexPath.Section);
						if (c == null || !c.SimpleCheckbox)
						{
							var sw = new UISwitch();
							sw.UserInteractionEnabled = false;
							cell.AccessoryView = sw;
						}
					} break;
					
					case TableRowType.Text:
					{
                        var c = td.RowSetting(RowConfigurator,name,indexPath.Section);
						if (c != null && c.InlineTextEditing)
						{
							var tf = new UITextField (new CGRect (0, 0, 160, 44));
							tf.UserInteractionEnabled = true;
							tf.BorderStyle = UITextBorderStyle.None;
							tf.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
							tf.Font = detailFont;
							tf.TextColor = DetailTextColor;
							tf.TextAlignment = UITextAlignment.Right;
							tf.WeakDelegate = this;

							var inp = new TableAdapterInlineTextInputAccessoryView (c, (float)tableView.Frame.Width);
							inp.PreviousButton.TouchUpInside += ClickedPrevious;
							inp.NextButton.TouchUpInside += ClickedNext;
							inp.DismissButton.TouchUpInside += ClickedDismiss;

							tf.InputAccessoryView = inp;
							TableEditor.ConfigureTextControl (c, tf);
							cell.AccessoryView = tf;
						}
						else
						{
							cell.DetailTextLabel.TextColor = DetailTextColor;
						}
					} break;

					case TableRowType.Blurb:
					{
						cell.TextLabel.Font = titleFont;
						cell.DetailTextLabel.Lines = 0;
						cell.DetailTextLabel.Font = detailFont;
						cell.DetailTextLabel.ClipsToBounds = true;
						cell.DetailTextLabel.TextColor = DetailTextColor;
						cell.ClipsToBounds = true;
					} break;

					default:
					{
						cell.DetailTextLabel.TextColor = DetailTextColor;
					} break;
				}
            }

            cell.TextLabel.Text = td.DisplayName(RowConfigurator,indexPath.Row,indexPath.Section);
            cell.SelectionStyle = editable ? UITableViewCellSelectionStyle.Default : UITableViewCellSelectionStyle.None;

            switch (rowType)
            {
                case TableRowType.Text:
                    {
                        var vs = value as string;
                        var c = td.RowSetting(RowConfigurator, name,indexPath.Section);
                        if (c != null && c.InlineTextEditing)
                        {
                            var tf = (cell.AccessoryView as UITextField);
                            tf.Enabled = editable;
                            tf.Text = vs;
                            tf.SecureTextEntry = c.SecureTextEditing;
                        	tf.ReturnKeyType = TableEditor.ConvertReturnKeyType(c.ReturnKeyType);
                        	tf.KeyboardType = TableEditor.ConvertKeyboardType(c.KeyboardType);
                            (tf.InputAccessoryView as TableAdapterInlineTextInputAccessoryView).IndexPath = indexPath;
                            cell.DetailTextLabel.Text = "";
                        }
                        else
                        {
                            cell.DetailTextLabel.Text = c != null && c.SecureTextEditing ? TextHelper.ScrambledText(vs) : vs;
                            cell.Accessory = editable ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;
                        }
                    }
                break;
                case TableRowType.Blurb:
                    {
                        var vs = value as string;
                        var c = td.RowSetting(RowConfigurator, name,indexPath.Section);
                        cell.DetailTextLabel.Text = c != null && c.SecureTextEditing ? TextHelper.ScrambledText(vs) : vs;
                        cell.Accessory = editable ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;
                    }
                break;
                case TableRowType.SingleChoiceList:
                    {
                        string choiceString = null;
                        if (value != null)
                            choiceString = value.ToString();
                        cell.DetailTextLabel.Text = choiceString;
                        cell.Accessory = editable ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;
                    }
				break;
                case TableRowType.Checkbox:
                    {
                        cell.DetailTextLabel.Text = "";
                        var s = td.RowSetting(RowConfigurator, name,indexPath.Section);
                        if (s != null && s.SimpleCheckbox)
                            cell.Accessory = ((bool)value) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
                        else
                            (cell.AccessoryView as UISwitch).On = (bool)value;
                    }
                break;
                case TableRowType.DateTime:
                case TableRowType.Date:
                case TableRowType.Time:
                    {
                        cell.DetailTextLabel.Text = td.DisplayDate(RowConfigurator, indexPath.Row, indexPath.Section, (DateTime)value, rowType);
                        cell.Accessory = editable ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;
                    }
                break;
            }

            return cell;
        }
		
		protected virtual void WillSelectRowWithConfig(TableAdapterRowConfig config)
		{
		
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

            TableAdapterRowConfig config = td.RowSetting(RowConfigurator,name,indexPath.Section);
			if (config != null)
            {
            	WillSelectRowWithConfig(config);
				if (config.Clicked != null)
                {
					config.Clicked(this, null);
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

						if (config != null && config.InlineTextEditing && rowType==TableRowType.Text)
						{
							var cell = tableView.CellAt (indexPath);
							var tf = (cell.AccessoryView as UITextField);
							tf.BecomeFirstResponder ();
						}
						else
						{
							var dname = td.DisplayName (RowConfigurator, indexPath.Row, indexPath.Section);
							var textEditor = new TableTextEditor (rowType, dname, str, delegate(string changedString)
							{
								td.SetValue (changedString, indexPath.Row, indexPath.Section);
								ReloadData ();
							});
							textEditor.ShouldAdjustTextContentInset = ShouldAdjustTextContentInset;
							textEditor.Configure (config);

							if (tvc.NavigationController == null)
								tvc.PresentViewController (new UINavigationController (textEditor), true, null);
							else
								tvc.NavigationController.PushViewController (textEditor, true);
						}
                    }
                break;

				case TableRowType.SingleChoiceList:
					var sclvc = FirstAvailableViewController;
					if (sclvc != null)
					{
						var dname = td.DisplayName (RowConfigurator, indexPath.Row, indexPath.Section);
						var singleChoiceEditor = new TableSingleChoiceEditor (rowType, dname, value, config, delegate(Object changedChoice)
						{
							td.SetValue (changedChoice, indexPath.Row, indexPath.Section);
							ReloadData ();
						});

						if (sclvc.NavigationController == null)
							sclvc.PresentViewController (new UINavigationController (singleChoiceEditor), true, null);
						else
							sclvc.NavigationController.PushViewController (singleChoiceEditor, true);
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

		#region Inline Text Editing

		NSIndexPath NextIndexPath(NSIndexPath indexPath)
		{
			int numOfSections = NumberOfSections(tv);				
			int nextSection = ((indexPath.Section + 1) % numOfSections);

			if ((indexPath.Row + 1) == RowsInSection(tv,indexPath.Section)) 
			{
				return NSIndexPath.FromRowSection(0,nextSection);
			} 
			else
			{
				return NSIndexPath.FromRowSection((indexPath.Row + 1),indexPath.Section);
			}
		}

		NSIndexPath PreviousIndexPath(NSIndexPath indexPath)
		{
			int numOfSections = NumberOfSections(tv);				
			int nextSection = ((indexPath.Section - 1) % numOfSections);

			if ((indexPath.Row - 1) < 0) 
			{
				return NSIndexPath.FromRowSection(0,nextSection);
			} 
			else
			{
				return NSIndexPath.FromRowSection((indexPath.Row - 1),indexPath.Section);
			}
		}

		void ClickedPrevious (object sender, EventArgs e)
		{
			NSIndexPath indexPath = ((sender as UIView).Superview as TableAdapterInlineTextInputAccessoryView).IndexPath;
			NSIndexPath nextIndexPath = PreviousIndexPath (indexPath);
			tv.SelectRow (nextIndexPath, true, UITableViewScrollPosition.Top);
			RowSelected (tv, nextIndexPath);
		}

		void ClickedNext (object sender, EventArgs e)
		{
			NSIndexPath indexPath = ((sender as UIView).Superview as TableAdapterInlineTextInputAccessoryView).IndexPath;
			NSIndexPath nextIndexPath = NextIndexPath (indexPath);
			tv.SelectRow (nextIndexPath, true, UITableViewScrollPosition.Top);
			RowSelected (tv, nextIndexPath);
		}

		void ClickedDismiss (object sender, EventArgs e)
		{
			UIApplication.SharedApplication.KeyWindow.EndEditing(true);
		}
		
		public EventHandler TextChanged;
		
		void TextFieldChanged(UITextField tf)
		{		
			NSIndexPath indexPath = (tf.InputAccessoryView as TableAdapterInlineTextInputAccessoryView).IndexPath;
			td.SetValue (tf.Text, indexPath.Row, indexPath.Section);
			if (TextChanged!=null)
				TextChanged(tf,null);
		}

		[Export ("textFieldShouldReturn:")]
		public bool TextFieldShouldReturn (UITextField tf)
		{
			NSIndexPath indexPath = (tf.InputAccessoryView as TableAdapterInlineTextInputAccessoryView).IndexPath;
			NSIndexPath nextIndexPath = NextIndexPath (indexPath);
			tv.SelectRow (nextIndexPath, true, UITableViewScrollPosition.Top);
			RowSelected (tv, nextIndexPath);
			return true;
		}

		[Export ("textField:shouldChangeCharactersInRange:replacementString:")]
		public bool TextFieldShouldChangeCharactersInRange (UITextField tf,NSRange range,string replacement)
		{
			var text = tf.Text;
			tf.Text = text.Substring (0, (int)range.Location) + replacement + text.Substring ((int)(range.Location + range.Length));
			TextFieldChanged (tf);
			return false;
		}

		#endregion
    }
}
