using System;

using UIKit;
using Foundation;

namespace Tables.iOS
{
	public class TableItemsAdapter : NSObject
	{
		public TableAdapterItemSelector ItemSelected {get;set;}
		public TableAdapterItemInformer ItemInformator { get; set;}
		private UITableView tv;
		private ITableSource td;

		public TableItemsAdapter(UITableView table=null,ITableSource source=null) : base()
		{		
			ListView = table;
			Source = source;
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

		public ITableSource Source
		{
			get
			{
				return td;             
			}
			set
			{
				td = value;
				if (td is TableAdapterItemInformer)
					ItemInformator = td as TableAdapterItemInformer;
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
			return td == null ? 0 : td.NumberOfSections ();
		}

		[Export ("tableView:numberOfRowsInSection:")]
		public int RowsInSection (UITableView tableView, int section)
		{
			return td == null ? 0 : td.RowsInSection(section);
		}

		[Export ("tableView:cellForRowAtIndexPath:")]
		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell("ItemsTableAdapterCell");
			if (cell == null )
			{
				cell = new UITableViewCell(UITableViewCellStyle.Subtitle,"ItemsTableAdapterCell");
				cell.SelectionStyle = UITableViewCellSelectionStyle.Default;
			}

			Object obj = td.GetValue (indexPath.Row, indexPath.Section);

			if (ItemInformator != null)
			{
				cell.TextLabel.Text = ItemInformator.ItemText (obj);
				cell.DetailTextLabel.Text = ItemInformator.ItemDetails (obj);
			}
			else
			{
				cell.TextLabel.Text = "";
				cell.DetailTextLabel.Text = "";
			}

			return cell;
		}

		[Export ("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var value = td.GetValue(indexPath.Row,indexPath.Section);
			if (ItemSelected != null)
				ItemSelected.DidSelectItem (value);
		}
	}
}
