using System;

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

    public class TableSectionsViewController : UITableViewController, ITableSectionAdapter
	{
		public virtual TableSection[] Sections { get; set; }

		public TableSectionsViewController (UITableViewStyle style) : base(style)
		{

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

		public virtual TableSection SectionAtIndexPath(NSIndexPath indexPath)
		{
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

		public override nint NumberOfSections (UITableView tableView)
		{
			return Sections == null ? 0 : Sections.Length;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return Sections [section].Items == null ? 0 : Sections [section].Items.Count;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return Sections [section].Name;
		}

		public override string TitleForFooter (UITableView tableView, nint section)
		{
			return Sections [section].Footer;
		}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			var item = ItemAtIndexPath (indexPath);
			if (item.DeleteSelector != null)
				return true;
			return false;
		}

		public override string TitleForDeleteConfirmation (UITableView tableView, NSIndexPath indexPath)
		{
			var item = ItemAtIndexPath (indexPath);
			if (item.DeleteTitle != null)
				return item.DeleteTitle;
			return "Delete";
		}

		public virtual bool CanSelectRow(UITableView tableView,NSIndexPath indexPath)
		{
			var item = ItemAtIndexPath (indexPath);
			if (item.Selector != null)
				return true;
			return false;
		}

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			var section = SectionAtIndexPath (indexPath);
			var item = ItemAtIndexPath (indexPath);
			if (editingStyle == UITableViewCellEditingStyle.Delete && item.DeleteSelector!=null)
			{
				item.DeleteSelector (this, new TableSectionsEventArgs (section, item, indexPath));
			}
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (!CanSelectRow (tableView, indexPath))
				return;
				
			var item = ItemAtIndexPath (indexPath);
			if (item.Selector != null)
			{
				var section = SectionAtIndexPath (indexPath);
				item.Selector (this, new TableSectionsEventArgs (section, item, indexPath));
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

			UITableViewCell cell = tableView.DequeueReusableCell (ident);
			if (cell == null)
				cell = new UITableViewCell (DefaultCellStyle, ident);

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
				cell.Accessory = CanSelectRow (tableView, indexPath) ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;

			}
			return cell;
		}
	}
}
