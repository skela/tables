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
            Cell=2,
            Footer=3
        }

        public struct ViewPosition
        {
            public int Row;
            public int Section;
            public ViewKind Kind;
            public int Position;
            public int Type;
            public int Layout;
        }

        int RowCount = 0;
        int HeaderCount = 0;
        int FooterCount = 0;
        int SectionCount = 0;

        int ViewTypes = 1;

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
                return RowCount + HeaderCount + FooterCount;
            }
        }

        public ViewPosition ViewPositionForPosition (int position)
        {
            return Positions[position];
        }

        public ViewKind ViewKindForPosition (int position)
        {
            return ViewPositionForPosition(position).Kind;
        }

        public override View GetView(int position, View convertView, ViewGroup parent) 
        {
            var vp = ViewPositionForPosition(position);

            View view = null;

            if (convertView == null) 
            {
                if (vp.Layout != 0)
                {
                    view = Inflator.Inflate(vp.Layout,null);
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
                if (view!=null) UpdateHeader(vp.Section, view);
            }
            else if (vp.Kind == ViewKind.Footer)
            {
                if (view!=null) UpdateFooter(vp.Section, view);
            }
            else
            {
                if (view!=null) UpdateCell(vp.Section,vp.Row, view);
            }
            return view;
        }

        protected virtual View CreateHeader(ViewGroup parent)
        {
            return new TableAdapterHeader(parent.Context);
        }

        protected virtual View CreateFooter(ViewGroup parent)
        {
            return new TableAdapterFooter(parent.Context);
        }

        protected virtual View CreateCell(ViewGroup parent)
        {
            return new TableAdapterCell(parent.Context);
        }

        protected virtual void UpdateHeader(int section,View view)
        {

        }

        protected virtual void UpdateCell(int section,int row,View view)
        {

        }

        protected virtual void UpdateFooter(int section,View view)
        {

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
                return 3;
            }
        }

        private void Reload()
        {
            RowCount = 0;
            HeaderCount = 0;
            FooterCount = 0;

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
            }

            var p = 0;
            var headers = HeaderCount;
            var footers = FooterCount;

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
                    pos.Type = (int)ViewKind.Cell;
                    pos.Section = sec;
                    pos.Position = p;
                    pos.Kind = ViewKind.Cell;
                    pos.Layout = GetLayoutForCell(row,sec);
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

        public virtual int GetLayoutForCell(int row,int section)
        {
            return 0;
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
