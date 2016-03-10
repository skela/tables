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

	public interface ITableSectionsValueCell : ITableSectionsCell
	{
		EventHandler ValueChanged { get; set; }
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
			if (item.DeleteTitle != null)
				return item.DeleteTitle;
			var section = SectionAtIndexPath (indexPath);
			if (section.DeleteTitle != null)
				return section.DeleteTitle;
			return "Delete";
		}

		public virtual bool CanSelectRow(UITableView tableView,NSIndexPath indexPath)
		{
			var section = SectionAtIndexPath (indexPath);
			var item = ItemAtIndexPath (indexPath);
			if (item.Selector != null || section.Selector!=null)
				return true;
			return false;
		}

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			var section = SectionAtIndexPath (indexPath);
			var item = ItemAtIndexPath (indexPath);
			if (editingStyle == UITableViewCellEditingStyle.Delete)
			{
				if (item.DeleteSelector!=null)
					item.DeleteSelector (this, new TableSectionsEventArgs (section, item, indexPath));
				if (section.DeleteSelector!=null)
					section.DeleteSelector (this, new TableSectionsEventArgs (section, item, indexPath));
			}
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (!CanSelectRow (tableView, indexPath))
				return;
				
			var section = SectionAtIndexPath (indexPath);
			var item = ItemAtIndexPath (indexPath);
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
				cell = new UITableViewCell (DefaultCellStyle, ident);

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
							valueChanged (sender, new TableSectionsEventArgs (section, item, indexPath));
					}
				}
			}
		}
	}
}
