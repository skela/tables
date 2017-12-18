using System;
using System.Collections.Generic;

using UIKit;
using Foundation;

namespace Tables.iOS
{
	public class TableSectionsEventArgs : Tables.TableSectionsEventArgs
	{
		public NSIndexPath IndexPath;

		public TableSectionsEventArgs(TableSection section,TableItem item, NSIndexPath indexPath) : base(section,item)
		{
			IndexPath = indexPath;
		}
	}

	public interface ITableSectionsCell
	{
		void Update(TableSection section,TableItem item,NSIndexPath indexPath);
	}

	public interface ITableSectionsValueCell : ITableSectionsCell
	{
		EventHandler ValueChanged { get; set; }
	}

    public class TableSectionsViewController : UITableViewController, ITableSectionAdapter
	{
		public virtual TableSection[] Sections { get; set; }
		
		public virtual string DefaultDeleteButtonTitle { get { return "Delete"; } }
		
		public TableSectionsViewController (UITableViewStyle style) : base(style)
		{

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			TableView.EstimatedRowHeight = 0;
			TableView.EstimatedSectionFooterHeight = 0;
			TableView.EstimatedSectionHeaderHeight = 0;
		}

		public virtual TableItem ItemWithName(string name,string section)
		{
			foreach (var sec in Sections)
			{
				if (sec.Name.Equals (section) && sec.Items != null)
					foreach (var item in sec.Items)
						if (item.Text!=null && item.Text.Equals(name))
							return item;
			}
			return null;
		}

        public virtual TableItem ItemWithIndexes(int section, int row)
        {
        	if (section == -1 || row == -1)
        		return null;
            return Sections [section].Items [row];
        }

        public virtual TableItem ItemWithKey(string key)
        {
            foreach (var sec in Sections)
            {               
                foreach (var item in sec.Items)
                    if (item.Key!=null && item.Key.Equals(key))
                        return item;
            }
            return null;
        }

		public virtual TableSection SectionWithKey(string key)
		{
			foreach (var sec in Sections)
			{               				
				if (sec.Key!=null && sec.Key.Equals(key))
					return sec;
			}
			return null;
		}

		public virtual TableSection SectionAtIndexPath(NSIndexPath indexPath)
		{
			if (indexPath.Section == -1) return null;
			return Sections [indexPath.Section];
		}

		public virtual TableItem ItemAtIndexPath(NSIndexPath indexPath)
		{
            return ItemWithIndexes(indexPath.Section,indexPath.Row);			
		}

		public virtual void ReloadData()
		{			
			TableView.ReloadData ();
		}

		public virtual void RemovedItem(EventArgs ea)
		{
			var e = ea as TableSectionsEventArgs;
			if (e == null || e.IndexPath==null)
			{
				ReloadData();
			}
			else
			{
				TableView.DeleteRows(new NSIndexPath[] { e.IndexPath }, UITableViewRowAnimation.Fade);
			}
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return Sections == null ? 0 : Sections.Length;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return Sections [section].Items == null ? 0 : Sections [section].Items.Count;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			var sec = Sections [section];
			if (sec.HideIfEmpty && sec.ItemCount == 0)
				return null;
			return sec.Name;
		}

		public override string TitleForFooter (UITableView tableView, nint section)
		{
			var sec = Sections [section];
			if (sec.HideIfEmpty && sec.ItemCount == 0)
				return null;
			return sec.Footer;
		}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			var item = ItemAtIndexPath (indexPath);
			if (item.DeleteSelector != null)
				return true;
			var section = SectionAtIndexPath (indexPath);
			if (section.DeleteSelector != null)
				return true;
			return false;
		}

		public override string TitleForDeleteConfirmation (UITableView tableView, NSIndexPath indexPath)
		{
			var item = ItemAtIndexPath (indexPath);
			if (item != null && item.DeleteTitle != null)
				return item.DeleteTitle;
			var section = SectionAtIndexPath (indexPath);
			if (section != null && section.DeleteTitle != null)
				return section.DeleteTitle;
			return DefaultDeleteButtonTitle;
		}

		public virtual bool CanSelectRow(UITableView tableView,NSIndexPath indexPath)
		{
			var section = SectionAtIndexPath (indexPath);
			var item = ItemAtIndexPath (indexPath);
			if (item.Selector != null || section.Selector!=null)
				return true;
			return false;
		}
		
		public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return UITableViewCellEditingStyle.Delete;
		}
		
		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			var section = SectionAtIndexPath (indexPath);
			var item = ItemAtIndexPath (indexPath);
			if (editingStyle == UITableViewCellEditingStyle.Delete)
			{
				if (item.DeleteSelector!=null)
					ExecuteRowAction(item.DeleteSelector, section, item, indexPath);
				else if (section.DeleteSelector!=null)
					ExecuteRowAction(section.DeleteSelector,section, item,indexPath);
			}
		}
		
		protected virtual void ExecuteRowAction(EventHandler action,TableSection section,TableItem item,NSIndexPath indexPath)
		{
			action (this, new TableSectionsEventArgs (section, item, indexPath));
		}
		
		protected virtual Action<UITableViewRowAction,NSIndexPath>CreateRowAction(TableAction action)
		{
			return delegate(UITableViewRowAction ract, NSIndexPath ip)
			{
				var section = SectionAtIndexPath (ip);
				var item = ItemAtIndexPath (ip);
				WillExecuteItemRowAction(action,item,section);
				ExecuteRowAction(action.Action,section,item,ip);
			};
		}
		
		public override UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
		{
			var section = SectionAtIndexPath (indexPath);
			var item = ItemAtIndexPath (indexPath);
			if (item == null || section == null) return new UITableViewRowAction[]{};
			if (item.Actions!=null && item.Actions.Count > 0)
			{
				var actions = new List<UITableViewRowAction>();
				foreach (var action in item.Actions)
				{
					var ios = action.IOS;
					var ra = UITableViewRowAction.Create(ios.Style,action.Title,CreateRowAction(action));
					if (ios.BackgroundColor!=null)
						ra.BackgroundColor = ios.BackgroundColor;
					if (ios.BackgroundEffect!=null)
						ra.BackgroundEffect = ios.BackgroundEffect;
					actions.Add(ra);
				}
				return actions.ToArray();
			}
			else
			{
				TableAction action = null;
				if (item.DeleteSelector!=null)
					action = item.Delete;
				if (action==null && section.DeleteSelector!=null)				
					action = section.Delete;
				if (action!=null)
					return new UITableViewRowAction[]{ UITableViewRowAction.Create(UITableViewRowActionStyle.Destructive,action.Title,CreateRowAction(action)) };
				return null;
			}
		}
		
		protected virtual void WillExecuteItemRowAction(TableAction action,TableItem item,TableSection section)
		{
		
		}
		
		protected virtual void WillSelectItem(TableItem item,TableSection section)
		{
		
		}
		
		protected virtual void WillChangeItemValue(object sender,TableItem item,TableSection section)
		{
		
		}
		
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (!CanSelectRow (tableView, indexPath))
				return;
				
			var section = SectionAtIndexPath (indexPath);
			var item = ItemAtIndexPath (indexPath);
			WillSelectItem(item,section);
			if (item.Selector != null)
			{				
				item.Selector (this, new TableSectionsEventArgs (section, item, indexPath));
			}
			else if (section.Selector!=null)
			{
				section.Selector (this, new TableSectionsEventArgs (section, item, indexPath));
			}
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			var item = ItemAtIndexPath (indexPath);
			if (item.CellHeight > 0)
				return (nfloat)item.CellHeight;
			return base.GetHeightForRow (tableView, indexPath);
		}

		public UITableViewCellStyle DefaultCellStyle = UITableViewCellStyle.Value1;

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var section = SectionAtIndexPath (indexPath);
			var item = ItemAtIndexPath (indexPath);

			string ident = null;
			ident = item.CellIdentString;
			if (ident == null && section.CellIdentString != null)
				ident = section.CellIdentString;
			if (ident == null)
				ident = "TableSectionsVCCell";

			EventHandler valueChanged = null;
			if (section.ValueChanged != null)
				valueChanged = section.ValueChanged;
			if (item.ValueChanged != null)
				valueChanged = item.ValueChanged;

			UITableViewCell cell = tableView.DequeueReusableCell (ident);
			if (cell == null)
			{
				cell = new UITableViewCell (DefaultCellStyle, ident);
				DefaultCellCreated(cell);
			}
			
			var valCell = cell as ITableSectionsValueCell;
			if (valCell != null) 
				valCell.ValueChanged = null;

			var tscell = cell as ITableSectionsCell;
			if (tscell != null)
			{
				tscell.Update (section, item, indexPath);
			}
			else
			{
				if (item.AttributedText != null && item.AttributedText is NSAttributedString)
					cell.TextLabel.AttributedText = item.AttributedText as NSAttributedString;
				else
					cell.TextLabel.Text = item.Text;
				if (item.AttributedDetail != null && item.AttributedDetail is NSAttributedString)
					cell.DetailTextLabel.AttributedText = item.AttributedDetail as NSAttributedString;
				else
					cell.DetailTextLabel.Text = item.Detail;
				cell.ImageView.Image = item.ImageName != null ? UIImage.FromBundle (item.ImageName) : null;

				if (CanSelectRow (tableView, indexPath))
				{
					var checkable = false;
					if (section.Checkable)
						checkable = true;
					if (item.Checkable)
						checkable = true;
					if (checkable)
					{
						cell.Accessory = item.Checked ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
					}
					else
					{
						cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
					}
				}
				else
				{
					cell.Accessory = UITableViewCellAccessory.None;
				}
			}

			if (valCell != null && valueChanged != null)
			{
				valCell.ValueChanged = CellValueChanged;
			}

			return cell;
		}

		protected virtual void DefaultCellCreated(UITableViewCell cell)
		{
		
		}

		[Foundation.Export("scrollViewDidScroll:")]
		public virtual void Scrolled (UIScrollView scrollView)
		{

		}

		private void CellValueChanged(object sender,EventArgs e)
		{
			if (sender is UIView)
			{
				var sw = sender as UIView;
				var super = sw.Superview;
				if (super is UITableViewCell)
				{
					var indexPath = TableView.IndexPathForCell (super as UITableViewCell);
					if (indexPath != null)
					{
						var section = SectionAtIndexPath (indexPath);
						var item = ItemAtIndexPath (indexPath);

						EventHandler valueChanged = null;
						if (section.ValueChanged != null)
							valueChanged = section.ValueChanged;
						if (item.ValueChanged != null)
							valueChanged = item.ValueChanged;

						if (valueChanged!=null)
						{
							WillChangeItemValue(sender,item,section);
							valueChanged (sender, new TableSectionsEventArgs (section, item, indexPath));
						}
					}
				}
			}
		}
	}
}
