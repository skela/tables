using System;
using System.Collections.Generic;

using Android.Views;
using Android.Support.V7.Widget;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Content.Res;

namespace Tables.Droid
{
    public interface IRecyclerAdapterDelegate
    {
        string Identifier {get;}
        int Layout { get; }
        int ViewType { get; set;}
        RecyclerView.ViewHolder CreateViewHolder(View view,Action<int>clicked);
        View CreateView(ViewGroup parent);
    }

    public interface IRecyclerAdapterHolder
    {
        void Update(TableSection tsection,TableItem item,int section, int row);
    }

    public abstract class RecyclerAdapterDelegate : IRecyclerAdapterDelegate
    {
        public virtual string Identifier { get { return null; } }

        public virtual int Layout { get { return 0; } }

        public virtual int ViewType { get; set;}

        public virtual RecyclerView.ViewHolder CreateViewHolder(View view,Action<int>clicked) { return null; }

        public virtual View CreateView(ViewGroup parent) { return null; }
    }

    public class RecyclerAdapter : RecyclerBaseAdapter,ITableSectionAdapter
    {
        public RecyclerAdapter(TableSection[]sections=null) : base()
        {
            this.sections = sections;
            ReloadData();
        }

        private RecyclerView tv;
        private TableSection[]sections;

        public RecyclerView ListView
        {
            get
            {
                return tv;
            }
            set
            {
                if (tv != null)
                {
                    tv.SetAdapter(null);
                }
                tv = value;
                if (tv != null)
                {
                    if (tv.GetLayoutManager() == null)
                    {
                        var layout = new LinearLayoutManager(tv.Context);
                        tv.SetLayoutManager(layout);
                    }
                    tv.SetAdapter(this);
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

        public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
        {
            var delegates = DelegatesForViewKind(viewType);

            IRecyclerAdapterDelegate delg = null;

            if (delegates.TryGetValue(viewType, out delg))
            {
                View itemView = null;
                if (delg.Layout != 0)
                {
                    itemView = LayoutInflater.From(parent.Context).Inflate(delg.Layout, parent, false);
                }
                else
                {
                    itemView = delg.CreateView(parent);
                }
                if (delegates == CellDelegates)
                    return delg.CreateViewHolder(itemView,ClickedCell);
                else
                    return delg.CreateViewHolder(itemView,null);
            }
            else
            {
                var itemView = CreateViewForType(viewType,parent);
                return CreateViewHolder(viewType,itemView);
            }
        }

        public override void OnBindViewHolder (RecyclerView.ViewHolder holder, int position)
        {
            var pos = Positions[position];

            Tables.TableSection section = null;
            Tables.TableItem row = null;

            section = GetSection(pos.Section);

            if (pos.Kind == ViewKind.Cell)
            {
                row = section.Items[pos.Row];
            }

            var vh = holder as IRecyclerAdapterHolder;
            vh?.Update(section, row, pos.Section, pos.Row);
        }

        protected override void ClickedCell(int position)
        {            
            var vp = Positions[position];
            if (vp.Kind == ViewKind.Cell)
            {
                var section = GetSection(vp.Section);
                var item = section.Items[vp.Row];
                if (item.Selector != null)
                {
                    item.Selector(this, new TableSectionsEventArgs(section, item, vp.Section, vp.Row));
                }
                else if (section.Selector != null)
                {
                    section.Selector(this, new TableSectionsEventArgs(section, item, vp.Section, vp.Row));
                }
            }
        }

        public override int GetItemViewType(int position)
        {
            return Positions[position].Type;
        }

        public override int GetLayoutForHeader(int section)
        {            
            return GetSection(section).HeaderLayout;
        }

        public override int GetLayoutForCell(int section,int row)
        {            
            return ItemWithIndexes(section,row).CellIdentInt;
        }

        public override int GetLayoutForFooter(int section)
        {
            return GetSection(section).FooterLayout;
        }

        public override int GetLayoutForDivider(int section)
        {
            return GetSection(section).DividerLayout;
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
       
        public override string GetTitleForHeader(int section)
        {
            var sec = GetSection(section);
            return sec == null ? null : sec.Name;
        }

        public override string GetTitleForFooter(int section)
        {
            var sec = GetSection(section);
            return sec == null ? null : sec.Footer;
        }

        public override int GetTypeForCell(int section,int row)
        {
            var s = GetSection(section);
            var e = s.Items[row];
            if (e.CellIdentString != null)
                return GetCellTypeFromIdent(e.CellIdentString);
            if (s.CellIdentString != null)
                return GetCellTypeFromIdent(s.CellIdentString);
            return (int)ViewKind.Cell;
        }

        public override int GetTypeForHeader(int section)
        {
            var s = GetSection(section);
            if (s.HeaderIdentifier != null)
                return GetHeaderTypeFromIdent(s.HeaderIdentifier);
            return (int)ViewKind.Header;
        }

        public override int GetTypeForFooter(int section)
        {
            var s = GetSection(section);
            if (s.FooterIdentifier != null)
                return GetFooterTypeFromIdent(s.FooterIdentifier);
            return (int)ViewKind.Footer;
        }

        public override int GetTypeForDivider(int section)
        {
            var s = GetSection(section);
            if (s.DividerIdentifier != null)
                return GetDividerTypeFromIdent(s.DividerIdentifier);
            return (int)ViewKind.Divider;
        }

        protected virtual View CreateViewForType(int viewType,ViewGroup parent)
        {
            var pos = ViewTypes[viewType];
            switch (pos.Kind)
            {
                case ViewKind.Cell:
                    return CreateCell(parent);
                case ViewKind.Footer:
                    return CreateFooter(parent);
                case ViewKind.Header:
                    return CreateHeader(parent);
                case ViewKind.Divider:
                    return CreateDivider(parent);
            }
            return null;
        }

        protected virtual RecyclerView.ViewHolder CreateViewHolder(int viewType,View view)
        {
            var pos = ViewTypes[viewType];
            switch (pos.Kind)
            {
                case ViewKind.Cell:
                return CreateCellHolder(view);
                case ViewKind.Footer:
                return CreateFooterHolder(view);
                case ViewKind.Header:
                return CreateHeaderHolder(view);
                case ViewKind.Divider:
                return CreateDividerHolder(view);
            }
            return null;
        }
    }

    public abstract class RecyclerBaseAdapter : RecyclerView.Adapter
    {
        public enum ViewKind : int
        {
            Header=1,
            Footer=1000,
            Divider=10000,
            Cell=100000,
        }

        public struct ViewPosition
        {
            public ViewKind Kind;
            public int Row;
            public int Section;
            public int Position;
            public int Type;
            public int Layout;
            public string Cell;
        }

        protected List<ViewPosition> Positions = new List<ViewPosition>();
        protected Dictionary<int,ViewPosition> ViewTypes = new Dictionary<int,ViewPosition>();

        protected Dictionary<int,IRecyclerAdapterDelegate> CellDelegates = new Dictionary<int,IRecyclerAdapterDelegate>();
        protected Dictionary<int,IRecyclerAdapterDelegate> HeaderDelegates = new Dictionary<int,IRecyclerAdapterDelegate>();
        protected Dictionary<int,IRecyclerAdapterDelegate> FooterDelegates = new Dictionary<int,IRecyclerAdapterDelegate>();
        protected Dictionary<int,IRecyclerAdapterDelegate> DividerDelegates = new Dictionary<int,IRecyclerAdapterDelegate>();

        protected Dictionary<int,IRecyclerAdapterDelegate> DelegatesForViewKind(int viewType)
        {
            var vp = ViewTypes[viewType];
            switch (vp.Kind)
            {
                case ViewKind.Cell:
                    return CellDelegates;
                case ViewKind.Divider:
                    return DividerDelegates;
                case ViewKind.Footer:
                    return FooterDelegates;
                case ViewKind.Header:
                    return HeaderDelegates;
            }
            return null;
        }

        public void RegisterCell(IRecyclerAdapterDelegate deleg)
        {
            int type = ((int)Tables.Droid.RecyclerAdapter.ViewKind.Cell) + CellDelegates.Count + 1;
            deleg.ViewType = type;
            CellDelegates.Add(type, deleg);
        }

        public void RegisterHeader(IRecyclerAdapterDelegate deleg)
        {
            int type = ((int)Tables.Droid.RecyclerAdapter.ViewKind.Header) + HeaderDelegates.Count + 1;
            deleg.ViewType = type;
            HeaderDelegates.Add(type, deleg);
        }

        public void RegisterFooter(IRecyclerAdapterDelegate deleg)
        {
            int type = ((int)Tables.Droid.RecyclerAdapter.ViewKind.Footer) + FooterDelegates.Count + 1;
            deleg.ViewType = type;
            FooterDelegates.Add(type, deleg);
        }

        public void RegisterDivider(IRecyclerAdapterDelegate deleg)
        {
            int type = ((int)Tables.Droid.RecyclerAdapter.ViewKind.Divider) + DividerDelegates.Count + 1;
            deleg.ViewType = type;
            DividerDelegates.Add(type, deleg);
        }

        public virtual int GetLayoutForHeader(int section) { return 0; }
        public virtual int GetLayoutForFooter(int section) { return 0; }
        public virtual int GetLayoutForDivider(int section) { return 0; }
        public virtual int GetLayoutForCell(int section,int row) { return 0; }

        public virtual int GetTypeForCell(int section,int row)
        {
            return (int)ViewKind.Cell;
        }

        public virtual int GetTypeForHeader(int section)
        {
            return (int)ViewKind.Header;
        }

        public virtual int GetTypeForFooter(int section)
        {
            return (int)ViewKind.Footer;
        }

        public virtual int GetTypeForDivider(int section)
        {
            return (int)ViewKind.Divider;
        }

        public virtual int NumberOfSections
        {
            get
            {
                return 1;
            }
        }

        protected int RowCount = 0;
        protected int HeaderCount = 0;
        protected int FooterCount = 0;
        protected int SectionCount = 0;
        protected int DividerCount = 0;

        public int GetCellTypeFromIdent(string cell)
        {
            foreach (var ch in CellDelegates.Values)
            {
                if (ch.Identifier.Equals(cell))
                    return ch.ViewType;
            }
            return (int)Tables.Droid.RecyclerAdapter.ViewKind.Cell;
        }

        public int GetHeaderTypeFromIdent(string cell)
        {
            foreach (var ch in HeaderDelegates.Values)
            {
                if (ch.Identifier.Equals(cell))
                    return ch.ViewType;
            }
            return (int)Tables.Droid.RecyclerAdapter.ViewKind.Header;
        }

        public int GetFooterTypeFromIdent(string cell)
        {
            foreach (var ch in FooterDelegates.Values)
            {
                if (ch.Identifier.Equals(cell))
                    return ch.ViewType;
            }
            return (int)Tables.Droid.RecyclerAdapter.ViewKind.Footer;
        }

        public int GetDividerTypeFromIdent(string cell)
        {
            foreach (var ch in DividerDelegates.Values)
            {
                if (ch.Identifier.Equals(cell))
                    return ch.ViewType;
            }
            return (int)Tables.Droid.RecyclerAdapter.ViewKind.Divider;
        }

        public override int ItemCount
        {
            get { return RowCount + HeaderCount + FooterCount + DividerCount; }
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

        protected virtual void ClickedCell(int position)
        {

        }

        public TableStyle Style = new TableStyle();

        protected virtual View CreateHeader(ViewGroup parent)
        {
            return new TableAdapterHeader(parent.Context,null,Style.DefaultHeaderLayoutStyle,Style.DefaultHeaderStyle);
        }

        protected virtual View CreateCell(ViewGroup parent)
        {
            return new TableAdapterRecyclerCell(parent.Context, null, Style.DefaultCellLayoutStyle,Style.DefaultTitleStyle,Style.DefaultDetailStyle);
        }

        protected virtual View CreateFooter(ViewGroup parent)
        {
            return new TableAdapterFooter(parent.Context,null,Style.DefaultFooterLayoutStyle,Style.DefaultFooterStyle);
        }

        protected virtual View CreateDivider(ViewGroup parent)
        {
            return new TableAdapterSectionSeperator(parent.Context, null, Style.DefaultSectionSeparatorStyle);
        }

        protected virtual RecyclerView.ViewHolder CreateHeaderHolder(View view)
        {
            return new TableAdapterHeaderHolder(view);
        }

        protected virtual RecyclerView.ViewHolder CreateCellHolder(View view)
        {
            return new TableAdapterCellHolder(view,ClickedCell);
        }

        protected virtual RecyclerView.ViewHolder CreateFooterHolder(View view)
        {
            return new TableAdapterFooterHolder(view);
        }

        protected virtual RecyclerView.ViewHolder CreateDividerHolder(View view)
        {
            return new TableAdapterDividerHolder(view);
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        private void Reload()
        {
            RowCount = 0;
            HeaderCount = 0;
            FooterCount = 0;
            DividerCount = 0;

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

                if (GetTypeForDivider(i) != (int)ViewKind.Divider)
                    DividerCount++;
            }

            var p = 0;
            var headers = HeaderCount;
            var footers = FooterCount;
            var seperators = DividerCount;

            Positions.Clear();

            for(int sec=0; sec<SectionCount; sec++)
            {
                if (headers > 0 && GetTitleForHeader(sec)!=null)
                {
                    var pos = new ViewPosition();
                    pos.Section = sec;
                    pos.Kind = ViewKind.Header;
                    pos.Position = p;
                    pos.Layout = GetLayoutForHeader(sec);
                    pos.Type = GetTypeForHeader(sec);
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
                    pos.Layout = GetLayoutForFooter(sec);
                    pos.Type = GetTypeForFooter(sec);
                    Positions.Add(pos);
                    footers--;
                    p++;
                }

                if (seperators > 0)
                {
                    var pos = new ViewPosition();
                    pos.Section = sec;
                    pos.Kind = ViewKind.Divider;
                    pos.Position = p;
                    pos.Layout = GetLayoutForDivider(sec);
                    pos.Type = GetTypeForDivider(sec);
                    Positions.Add(pos);
                    seperators--;
                    p++;
                }
            }

            ViewTypes.Clear();
            foreach (var pos in Positions)
                ViewTypes[pos.Type] = pos;
        }

        public virtual void ReloadData()
        {
            Reload();
            NotifyDataSetChanged();
        }
    }

    public class TableAdapterRecyclerCell : LinearLayout,ITableAdapterSimpleCell
    {
        public TextView Title { get; private set;}
        public TextView Detail { get; private set;}

        private int titleStyle = TableUtils.DefaultTitleStyle;
        private int detailStyle = TableUtils.DefaultDetailStyle;

        public TableAdapterRecyclerCell(Context context) : base(context)
        {
            Prepare(context);
        }

        public TableAdapterRecyclerCell (Context context, IAttributeSet attrs) : base (context, attrs)
        {
            Prepare (context);
        }

        public TableAdapterRecyclerCell (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
        {
            Prepare (context);
        }

        public TableAdapterRecyclerCell (Context context, IAttributeSet attrs, int defStyle, int aTitleStyle, int aDetailStyle) : base (context, attrs, defStyle)
        {
            titleStyle = aTitleStyle;
            detailStyle = aDetailStyle;
            Prepare (context);
        }

        private void Prepare(Context context)
        {            
            LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent, 1f);

            var attrs = new int[] { Android.Resource.Attribute.SelectableItemBackground };
            var typedArray = context.ObtainStyledAttributes(attrs);
            var backgroundResource = typedArray.GetResourceId(0, 0);
            SetBackgroundResource(backgroundResource);
            typedArray.Recycle();

            Title = new TextView(context);
            int pixels = TableUtils.GetPixelsFromDPI(context, 2);

            var tparams = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent, 1f);
            tparams.SetMargins(0, pixels, 0, pixels);
            Title.LayoutParameters = tparams;
            Title.SetTextAppearance(context, titleStyle);
            Title.Text = "Title";
            AddView(Title);

            Detail = new TextView(context);
            Detail.Text = "Detail";
            pixels = TableUtils.GetPixelsFromDPI(context, 2);

            var lparams = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent, 1f);
            lparams.SetMargins(0, pixels, 0, pixels);
            Detail.LayoutParameters = lparams;
            Detail.SetTextAppearance(context, detailStyle);
            AddView(Detail);

            Orientation = Android.Widget.Orientation.Vertical;
            pixels = TableUtils.GetPixelsFromDPI(context, 10);
            SetPadding(pixels, pixels, pixels, pixels);
        }

        public bool Editable 
        {
            set
            {
                Focusable = !value;
            }
        }

        public void SetTitleStyle(Context ctx,int style)
        {
            titleStyle = style;
            if (Title != null)
                Title.SetTextAppearance(ctx, titleStyle);
        }

        public void SetDetailStyle(Context ctx,int style)
        {
            detailStyle = style;
            if (Detail != null)
                Detail.SetTextAppearance(ctx, detailStyle);
        }
    }

    public class TableAdapterBaseHolder : RecyclerView.ViewHolder,IRecyclerAdapterHolder
    {
        public TableAdapterBaseHolder(View v) : base(v)
        {

        }

        public virtual void Update(TableSection tsection,TableItem item,int section, int row)
        {

        }
    }

    public class TableAdapterHeaderHolder : TableAdapterBaseHolder
    {
        public TextView TitleLabel;

        public TableAdapterHeaderHolder(View v) : base(v)
        {
            var header = v as TableAdapterHeader;
            TitleLabel = header?.TitleLabel;
        }

        public override void Update(TableSection tsection,TableItem item,int section, int row)
        {
            TitleLabel.Text = tsection.Name;
        }
    }

    public class TableAdapterCellHolder : TableAdapterBaseHolder
    {
        public TextView Title { get; private set;}
        public TextView Detail { get; private set;}

        public TableAdapterCellHolder(View v,Action<int> clicked) : base(v)
        {
            v.Click += (sender, e) => clicked (base.AdapterPosition);

            var cell = v as ITableAdapterSimpleCell;
            Title = cell?.Title;
            Detail = cell?.Detail;
        }

        public override void Update(TableSection tsection,TableItem item,int section, int row)
        {
            Title.Text = item.Text;
            Detail.Text = item.Detail;

            Detail.Visibility = Detail.Text == null || Detail.Text.Length == 0 ? ViewStates.Gone : ViewStates.Visible;
        }
    }

    public class TableAdapterFooterHolder : TableAdapterBaseHolder
    {
        public TextView TitleLabel;

        public TableAdapterFooterHolder(View v) : base(v)
        {
            var footer = v as TableAdapterFooter;
            TitleLabel = footer?.TitleLabel;
        }

        public override void Update(TableSection tsection,TableItem item,int section, int row)
        {
            TitleLabel.Text = tsection.Footer;
        }
    }

    public class TableAdapterDividerHolder : TableAdapterBaseHolder
    {
        public TableAdapterDividerHolder(View v) : base(v)
        {

        }
    }
}
