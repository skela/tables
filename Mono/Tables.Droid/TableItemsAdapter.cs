using System;
using Android.Widget;
using Android.Views;

namespace Tables.Droid
{
    public class TableItemsAdapter : BaseAdapter
    {
        public TableItemsAdapter(ListView table=null,ITableSource source=null)
        {
            ListView = table;
            Source = source;
        }

        private ITableAdapterSimpleCell cell;
        public TableAdapterItemSelector ItemSelected {get;set;}
        public TableAdapterItemInformer ItemInformator { get; set;}
        private ListView tv;
        private ITableSource td;

        public ListView ListView
        {
            get
            {
                return tv;
            }
            set
            {
                if (tv != null)
                {
                    tv.Adapter = null;
                    tv.ItemClick -= ClickedItem;
                }
                tv = value;
                if (tv != null)
                {
                    tv.Adapter = this;
                    tv.ItemClick += ClickedItem;
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
            NotifyDataSetChanged();
        }

        #region BaseAdapter

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            View view = null;

            if (convertView == null) 
            {
                if (cell == null)
                {
                    TableAdapterSimpleCell body = new TableAdapterSimpleCell(parent.Context);
                    view = body;
                }
                else
                    view = cell as View;
            } 
            else
            {
                view = convertView;
            }

            if (view is ITableAdapterSimpleCell)
            {
                ITableAdapterSimpleCell c = view as ITableAdapterSimpleCell;
                UpdateView(c,position,0);
            }

            return view;
        }

        void ClickedItem(object sender, AdapterView.ItemClickEventArgs e)
        {
            RowSelected(e.Position, 0);           
        }

        public virtual void RowSelected (int row,int section)
        {
            var value = td.GetValue(row,section);
            if (ItemSelected != null)
                ItemSelected.DidSelectItem (value);
        }

        public virtual void UpdateView(ITableAdapterSimpleCell cell, int row, int section)
        {
            Object obj = td.GetValue (row, section);

            if (ItemInformator != null)
            {
                cell.Title.Text = ItemInformator.ItemText (obj);
                cell.Detail.Text = ItemInformator.ItemDetails (obj);
            }
            else
            {
                cell.Title.Text = "";
                cell.Detail.Text = "";
            }
        }

        public override int Count
        {
            get
            {
                return td==null?0:td.RowsInSection(0);
            }
        }

        #endregion       
    }
}
