using System;
using System.Collections.Generic;

using Android.Widget;
using Android.Views;
using Android.Content;

namespace Tables.Droid
{
    

    public class TableSectionedAdapter : BaseAdapter
    {        
        protected LayoutInflater Inflator;

        public TableSectionedAdapter (Context context) : base()
        {           
            Inflator =  (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);                           
        }

        public enum ViewKind : int
        {
            Header=1,
            Footer=2,
            Seperator=3,
            Cell=4,
        }

        public struct ViewPosition
        {
            public int Row;
            public int Section;
            public ViewKind Kind;
            public int Position;
            public int Type;
            public int Layout;
            public string Cell;
        }

        public class ViewHolder
        {
            public Object Holder;
            public int Type;
            public string Ident;
        }

        int RowCount = 0;
        int HeaderCount = 0;
        int FooterCount = 0;
        int SectionCount = 0;
        int SeperatorCount = 0;

        List<ViewPosition> Positions = new List<ViewPosition>();

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get 
            {
                return RowCount + HeaderCount + FooterCount + SeperatorCount;
            }
        }

        public virtual ViewPosition ViewPositionForPosition (int position)
        {
            return Positions[position];
        }

        public ViewKind ViewKindForPosition (int position)
        {
            return ViewPositionForPosition(position).Kind;
        }

        public override int GetItemViewType(int position)
        {
            var vp = ViewPositionForPosition(position);
            return vp.Type;
        }

        public override int ViewTypeCount
        {
            get
            {
                return kDefaultViewTypeCount + CellHolders.Count;
            }
        }
        private const int kDefaultViewTypeCount = 5;

        public Dictionary<int,ViewHolder> CellHolders = new Dictionary<int,ViewHolder>();

        public void RegisterCell(string cell,Object holder)
        {
            int type = ((int)ViewKind.Cell) + CellHolders.Count + 1;
            var vh = new ViewHolder() { Ident = cell, Type = type, Holder = holder };
            CellHolders.Add(type, vh);
        }

        public int GetCellTypeFromCellIdent(string cell)
        {
            foreach (var ch in CellHolders.Values)
            {
                if (ch.Ident.Equals(cell))
                    return ch.Type;
            }
            return (int)ViewKind.Cell;
        }

        public override bool AreAllItemsEnabled()
        {
            return false;
        }

        public override bool IsEnabled(int position)
        {
            return ViewKindForPosition(position) == ViewKind.Cell;
        }

        public override View GetView(int position, View convertView, ViewGroup parent) 
        {
            var vp = ViewPositionForPosition(position);

            View view = null;

            if (convertView == null) 
            {
                if (vp.Layout != 0)
                {
                    view = Inflator.Inflate(vp.Layout, null);
                    ViewHolder holder = null;
                    if (CellHolders.TryGetValue(vp.Type, out holder))
                    {
                        if (holder.Holder is ITableCellHolder)
                        {
                            (holder.Holder as ITableCellHolder).Bind(view);
                        }
                    }
                }
                else
                {
                    if (vp.Kind == ViewKind.Header)
                    {
                        var header = CreateHeader(parent);
                        view = header;
                    }
                    else if (vp.Kind == ViewKind.Footer)
                    {
                        var footer = CreateFooter(parent);
                        view = footer;
                    }
                    else if (vp.Kind == ViewKind.Seperator)
                    {
                        var seperator = CreateSeperator(parent);
                        view = seperator;
                    }
                    else
                    {
                        var body = CreateCell(parent);
                        view = body;
                    }
                }
            }
            else
            {
                view = convertView;
            }

            if (vp.Kind == ViewKind.Header)
            {
                if (view != null)
                    UpdateHeader(vp.Section, view);
            }
            else if (vp.Kind == ViewKind.Footer)
            {
                if (view != null)
                    UpdateFooter(vp.Section, view);
            }
            else if (vp.Kind == ViewKind.Seperator)
            {

            }
            else
            {
                if (vp.Layout != 0)
                {
                    ViewHolder holder = null;
                    if (CellHolders.TryGetValue(vp.Type, out holder))
                    {
                        if (holder.Holder is ITableCellHolder)
                        {
                            (holder.Holder as ITableCellHolder).Bind(view);
                            UpdateHolder(vp.Section, vp.Row, holder.Holder);
                        }
                    }
                    else
                    {
                        if (view != null)
                            UpdateCell(vp.Section, vp.Row, view);
                    }
                }
                else
                {
                    if (view != null)
                        UpdateCell(vp.Section, vp.Row, view);
                }
            }
            return view;
        }

        public TableStyle Style = new TableStyle();

        protected virtual View CreateHeader(ViewGroup parent)
        {
            return new TableAdapterHeader(parent.Context,null,Style.DefaultHeaderLayoutStyle,Style.DefaultHeaderStyle);
        }

        protected virtual View CreateCell(ViewGroup parent)
        {
            return new TableAdapterCell(parent.Context, null, Style.DefaultCellLayoutStyle);
        }

        protected virtual View CreateFooter(ViewGroup parent)
        {
            return new TableAdapterFooter(parent.Context,null,Style.DefaultFooterLayoutStyle,Style.DefaultFooterStyle);
        }

        protected virtual View CreateSeperator(ViewGroup parent)
        {
            return new TableAdapterSectionSeperator(parent.Context, null, Style.DefaultSectionSeparatorStyle);
        }

        protected virtual void UpdateHeader(int section,View view)
        {

        }

        protected virtual void UpdateCell(int section,int row,View view)
        {

        }

        protected virtual void UpdateHolder(int section,int row,Object holder)
        {
            
        }

        protected virtual void UpdateFooter(int section,View view)
        {

        }

        private void Reload()
        {
            RowCount = 0;
            HeaderCount = 0;
            FooterCount = 0;
            SeperatorCount = 0;

            SectionCount = NumberOfSections;

            for (int i = 0; i < SectionCount; i++)
            {
                RowCount += NumberOfRowsForSection(i);

                var headerTitle = GetTitleForHeader(i);
                if (headerTitle != null)
                    HeaderCount++;

                var footerTitle = GetTitleForFooter(i);
                if (footerTitle != null)
                    FooterCount++;

                if (Style.DefaultSectionSeparatorStyle > 0)
                    SeperatorCount++;
            }

            var p = 0;
            var headers = HeaderCount;
            var footers = FooterCount;
            var seperators = SeperatorCount;

            Positions.Clear();

            for(int sec=0; sec<SectionCount; sec++)
            {
                if (headers > 0 && GetTitleForHeader(sec)!=null)
                {
                    var pos = new ViewPosition();
                    pos.Section = sec;
                    pos.Type = (int)ViewKind.Header;
                    pos.Kind = ViewKind.Header;
                    pos.Position = p;
                    pos.Layout = GetLayoutForHeader(sec);
                    Positions.Add(pos);

                    headers--;
                    p++;
                }

                for (int row = 0; row < NumberOfRowsForSection(sec); row++)
                {
                    var pos = new ViewPosition();
                    pos.Row = row;
                    pos.Section = sec;
                    pos.Position = p;
                    pos.Kind = ViewKind.Cell;
                    pos.Layout = GetLayoutForCell(sec,row);
                    pos.Type = GetTypeForCell(sec, row);
                    Positions.Add(pos);
                    p++;
                }

                if (footers > 0 && GetTitleForFooter(sec)!=null)
                {
                    var pos = new ViewPosition();
                    pos.Section = sec;
                    pos.Kind = ViewKind.Footer;
                    pos.Position = p;
                    pos.Type = (int)ViewKind.Footer;
                    pos.Layout = GetLayoutForFooter(sec);
                    Positions.Add(pos);
                    footers--;
                    p++;
                }

                if (seperators > 0)
                {
                    var pos = new ViewPosition();
                    pos.Section = sec;
                    pos.Kind = ViewKind.Seperator;
                    pos.Position = p;
                    pos.Type = (int)ViewKind.Seperator;
                    Positions.Add(pos);
                    seperators--;
                    p++;
                }
            }
        }

        public override void NotifyDataSetChanged()
        {            
            Reload();
            base.NotifyDataSetChanged();
        }

        #region iOS

        public virtual int NumberOfSections
        {
            get
            {
                return 1;
            }
        }

        public virtual int NumberOfRowsForSection(int section)
        {
            return 0;
        }

        public virtual string GetTitleForHeader(int section)
        {
            return null;
        }

        public virtual string GetTitleForFooter(int section)
        {
            return null;
        }

        public virtual int GetLayoutForHeader(int section)
        {
            return 0;
        }

        public virtual int GetLayoutForCell(int section,int row)
        {
            return 0;
        }

        public virtual int GetTypeForCell(int section,int row)
        {
            return (int)ViewKind.Cell;
        }

        public virtual string GetIdentifierForCell(int section,int row)
        {
            return null;
        }

        public virtual int GetLayoutForFooter(int section)
        {
            return 0;
        }

        public void ReloadData()
        {                        
            NotifyDataSetChanged();
        }

        #endregion
    }
}
