using System;

using Android.Content;
using Android.Widget;
using Android.Views;
using Android.App;
using System.Collections.Generic;

namespace Tables.Droid
{
    public class TableSectionsEventArgs : Tables.TableSectionsEventArgs
    {
        public int RowIndex;
        public int SectionIndex;

        public TableSectionsEventArgs(TableSection section,TableItem item, int sectionIndex,int rowIndex) : base(section,item)
        {
            RowIndex = rowIndex;
            SectionIndex = sectionIndex;
        }
    }

    public class TableSectionAdapter : TableSectionedAdapter, ITableSectionAdapter
    {
        public TableSectionAdapter(Context ctx,TableSection[]sections=null) : base(ctx)
        {
            this.sections = sections;
            ReloadData();
        }

        private ListView tv;
        private TableSection[]sections;

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
                    tv.ItemLongClick -= LongClickedItem;
                }
                tv = value;
                if (tv != null)
                {
                    tv.Adapter = this;
                    tv.ItemClick += ClickedItem;
                    tv.ItemLongClick += LongClickedItem;
                }
            }
        }

        public TableSection[] Sections
        {
            set
            {
                sections = value;
                ReloadData();
            }
            get
            {
                return sections;
            }
        }

        public ViewPosition? ViewPositionForAdapterClick(AdapterView.ItemClickEventArgs e)
        {
            var list = (e.Parent as ListView);
            int pos = e.Position;
            if (list!=null && list.HeaderViewsCount > 0)
                pos -= list.HeaderViewsCount;
            if (pos < 0) return null;
            var vp = ViewPositionForPosition(pos);
            return vp;
        }

        public void ClickedItem(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (tv == null)
                return;
            
            var vp = ViewPositionForAdapterClick(e);

            if (vp!=null && vp.Value.Kind == ViewKind.Cell)
            {
                RowSelected(vp.Value.Row, vp.Value.Section);
            }
        }

        void LongClickedItem(object sender, Android.Widget.AdapterView.ItemLongClickEventArgs e)
        {
            if (tv == null)
                return;
            var list = (e.Parent as ListView);
            int pos = e.Position;
            if (list!=null && list.HeaderViewsCount > 0)
                pos -= list.HeaderViewsCount;
            if (pos < 0) return;

            var vp = ViewPositionForPosition(pos);
            if (vp.Kind == ViewKind.Cell)
            {
                RowLongSelected(vp.Row, vp.Section);
            }
        }
		
		protected virtual void WillSelectItem(TableItem item,TableSection section)
		{
		
		}
		
		protected virtual void WillLongSelectItem(TableItem item,TableSection section)
		{
		
		}
		
		protected virtual void WillExecuteItemRowAction(TableAction action,TableItem item,TableSection section)
		{
		
		}
				
        public virtual void RowSelected (int row,int section,AdapterView.ItemClickEventArgs ea = null)
        {
            var item = ItemWithIndexes(section,row);
            var sec = GetSection(section);
            
            if (item.Selector!=null || sec.Selector!=null)
            	WillSelectItem(item,sec);
            
            if (item.Selector != null)
                item.Selector(this, new TableSectionsEventArgs(sec, item, section, row));
            else if (sec.Selector != null)
                sec.Selector(this, new TableSectionsEventArgs(sec, item, section, row));            
        }

        public virtual void RowLongSelected (int row,int section,AdapterView.ItemClickEventArgs ea = null)
        {
            var item = ItemWithIndexes(section,row);
            var sec = GetSection(section);
            
            var actions = new List<TableAction>();
            if (item.Actions.Count > 0)
            	actions.AddRange(item.Actions);
            else if (item.Delete.Action!=null)
            	actions.Add(item.Delete);
            
            if (actions.Count == 0)
        	{
        		if (sec.Actions.Count > 0)
        			actions.AddRange(sec.Actions);
        		else if (sec.Delete.Action!=null)
        			actions.Add(sec.Delete);
        	}
            
            if (actions.Count > 0)
            {
            	WillLongSelectItem(item,sec);
            	
            	var cancel = CancelAction;
            	if (cancel!=null)
            		actions.Add(cancel);
            	
            	ShowActions(actions,new TableSectionsEventArgs(sec, item, section, row));
            }
        }
		
		protected virtual TableAction CancelAction
		{
			get
			{
				return new TableAction() { Title = "Cancel" };
			}
		}
		
		public class TableActionAlert
		{
			AlertDialog.Builder Builder;
			List<TableAction>Actions;
			TableSectionsEventArgs Args;
			
			public TableActionAlert(TableSectionAdapter sender,Context context,List<TableAction>actions,TableSectionsEventArgs args)
			{
				Actions = actions;
				Args = args;
				
				var titles = new List<string>();
				foreach (var act in Actions)
					titles.Add(act.Title);
				
				Builder = new AlertDialog.Builder(context);
        		Builder.SetItems(titles.ToArray(),delegate(object send, DialogClickEventArgs e)
                {
                    var i = e.Which;
                    var btn = Actions[i];
                    if (btn.Action!=null)
                    {
                    	sender.WillExecuteItemRowAction(btn,args.Item,args.Section);
                    	btn.Action(sender,Args);
                    }
                });
			}
			
			public void Show()
			{
				Builder.Show();
			}
		}
		
		protected virtual void ShowActions(List<TableAction>actions,TableSectionsEventArgs args)
        {
        	var alert = new TableActionAlert(this,ListView.Context,actions,args);        	
        	alert.Show();
        }
		
        public override int NumberOfSections
        {
            get
            {
                var ds = sections;
                return ds != null ? ds.Length : 0;
            }
        }

        public virtual TableSection GetSection(int section)
        {
            var ds = sections;
            if (ds!= null)
            {
                return ds[section];
            }
            return null;
        }

        public virtual TableItem ItemWithIndexes(int section, int row)
        {
            var sec = GetSection(section);
            return sec.Items[row];
        }

        public virtual TableItem ItemWithName(string name,string section)
        {
            foreach (var sec in sections)
            {
                if (sec.Name.Equals (section) && sec.Items != null)
                    foreach (var item in sec.Items)
                        if (item.Text!=null && item.Text.Equals(name))
                            return item;
            }
            return null;
        }

        public virtual TableItem ItemWithKey(string key)
        {
            foreach (var sec in sections)
            {               
                foreach (var item in sec.Items)
                    if (item.Key!=null && item.Key.Equals(key))
                        return item;
            }
            return null;
        }

        public virtual TableSection SectionWithKey(string key)
        {
            foreach (var sec in sections)
            {                               
                if (sec.Key!=null && sec.Key.Equals(key))
                    return sec;
            }
            return null;
        }

        public override int NumberOfRowsForSection(int section)
        {
            var sec = GetSection(section);
            return sec != null ? sec.ItemCount : 0;
        }

        public override int GetLayoutForCell(int section,int row)
        {
            var e = ItemWithIndexes(section,row);
            return e.CellIdentInt;
        }

        public override int GetTypeForCell(int section,int row)
        {
            var e = ItemWithIndexes(section,row);
            if (e.CellIdentString != null)
                return GetCellTypeFromCellIdent(e.CellIdentString);
            return base.GetTypeForCell(section, row);
        }

        public override string GetTitleForHeader(int section)
        {
            var sec = GetSection(section);
            if (sec != null && sec.HideIfEmpty && sec.ItemCount == 0)
                return null;
            return sec == null ? null : sec.Name;
        }

        public override string GetTitleForFooter(int section)
        {
            var sec = GetSection(section);
            if (sec != null && sec.HideIfEmpty && sec.ItemCount == 0)
                return null;           
            return sec == null ? null : sec.Footer;
        }

        protected override View CreateCell(ViewGroup parent)
        {
            var cell = new TableAdapterSectionCell(parent.Context,null,Style.DefaultCellLayoutStyle,Style.DefaultTitleStyle,Style.DefaultDetailStyle);
            return cell;
        }

        protected override void UpdateHeader(int section,View view)
        {
            var header = GetTitleForHeader(section);
            if (view is ITableAdapterHeader)
            {
                var c = view as ITableAdapterHeader;
                c.Title = header;
            }
        }

        protected override void UpdateCell(int section,int row,View view)
        {
            if (view is ITableAdapterSectionCell)
            {
                var s = GetSection(section);
                var e = ItemWithIndexes(section,row); 
                var c = view as ITableAdapterSectionCell;
                c.Title = e.Text;
                c.Detail = e.Detail;

                var checkable = false;
                if (s.Checkable)
                    checkable = true;
                if (e.Checkable)
                    checkable = true;

                if (checkable)
                {
                    c.Detail = e.Checked ? "✓" : "";
                }
            }
            if (view is ITableSectionsCell)
            {
                var s = GetSection(section);
                var e = ItemWithIndexes(section,row); 
                var c = view as ITableSectionsCell;
                c.Update(s, e, section, row);
            }            
        }

        protected override void UpdateHolder(int section,int row,object holder)
        {
            if (holder is ITableSectionsCell)
            {
                var s = GetSection(section);
                var e = ItemWithIndexes(section,row); 
                var c = holder as ITableSectionsCell;
                c.Update(s, e, section, row);
            }  
        }

        protected override void UpdateHolder(int section,int row,Java.Lang.Object holder)
        {
            if (holder is ITableSectionsCell)
            {
                var s = GetSection(section);
                var e = ItemWithIndexes(section,row); 
                var c = holder as ITableSectionsCell;
                c.Update(s, e, section, row);
            }  
        }

        protected override void UpdateFooter(int section,View view)
        {
            var footer = GetTitleForFooter(section);
            if (view is ITableAdapterFooter)
            {
                var c = view as ITableAdapterFooter;
                c.Title = footer;
            }
        }
    }
}

