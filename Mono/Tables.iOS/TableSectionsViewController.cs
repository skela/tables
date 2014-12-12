using System;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Tables.iOS
{
	public class TableSectionsEventArgs : EventArgs
	{
		public TableSection Section;
		public TableItem Item;
		public NSIndexPath IndexPath;

		public TableSectionsEventArgs(TableSection section,TableItem item, NSIndexPath indexPath) : base()
		{
			Section = section;
			Item = item;
			IndexPath = indexPath;
		}
	}

	public interface ITableSectionsCell
	{
		void Update(TableSection section,TableItem item,NSIndexPath indexPath);
	}

	public class TableSectionsViewController : UITableViewController
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

		public virtual TableSection SectionAtIndexPath(NSIndexPath indexPath)
		{
			return Sections [indexPath.Section];
		}

		public virtual TableItem ItemAtIndexPath(NSIndexPath indexPath)
		{
			return SectionAtIndexPath(indexPath).Items [indexPath.Row];
		}

		public virtual void ReloadData()
		{
			TableView.ReloadData ();
		}

		public override int NumberOfSections (UITableView tableView)
		{
			return Sections == null ? 0 : Sections.Length;
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return Sections [section].Items == null ? 0 : Sections [section].Items.Length;
		}

		public override string TitleForHeader (UITableView tableView, int section)
		{
			return Sections [section].Name;
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
			return base.TitleForDeleteConfirmation (tableView, indexPath);
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
				cell = new UITableViewCell (UITableViewCellStyle.Value1, ident);

			var tscell = cell as ITableSectionsCell;
			if (tscell != null)
			{
				tscell.Update (section, item, indexPath);
			}
			else
			{
				cell.TextLabel.Text = item.Text;
				cell.DetailTextLabel.Text = item.Detail;
				cell.Accessory = CanSelectRow (tableView, indexPath) ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;
			}
			return cell;
		}
	}
}
