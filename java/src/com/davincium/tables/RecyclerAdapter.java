package com.davincium.tables;

import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.davincium.tables.interfaces.IRecyclerAdapterDelegate;
import com.davincium.tables.interfaces.IRecyclerAdapterHolder;
import com.davincium.tables.interfaces.ITableSectionAdapter;

import java.util.Map;

public class RecyclerAdapter extends RecyclerBaseAdapter implements ITableSectionAdapter
{
    public RecyclerAdapter(TableSection[]secs)
    {
        super();
        sections = secs;
        reloadData();
    }

    public RecyclerAdapter()
    {
        super();
        this.sections = null;
        reloadData();
    }

    private RecyclerView tv;
    private TableSection[] sections;

    public RecyclerView getListView()
    {
        return tv;
    }

    public void setListView(RecyclerView value)
    {
        if (tv != null)
        {
            tv.setAdapter(null);
        }
        tv = value;
        if (tv != null)
        {
            if (tv.getLayoutManager() == null)
            {
                LinearLayoutManager layout = new LinearLayoutManager(tv.getContext());
                tv.setLayoutManager(layout);
            }
            tv.setAdapter(this);
        }
    }

    public TableSection[] getSections()
    {
        return sections;
    }

    public void setSections(TableSection[]ts)
    {
        sections = ts;
        reloadData();
    }

    @Override
    public RecyclerView.ViewHolder onCreateViewHolder (ViewGroup parent, int viewType)
    {
        Map<Integer,IRecyclerAdapterDelegate> delegates = delegatesForViewKind(viewType);

        IRecyclerAdapterDelegate delg = delegates.get(viewType);

        if (delg!=null)
        {
            View itemView = null;
            if (delg.getLayout() != 0)
            {
                itemView = LayoutInflater.from(parent.getContext()).inflate(delg.getLayout(), parent, false);
            }
            else
            {
                itemView = delg.createView(parent);
            }
            if (delegates == cellDelegates)
                return delg.createViewHolder(itemView,this);
            else
                return delg.createViewHolder(itemView,null);
        }
        else
        {
            View itemView = createViewForType(viewType,parent);
            return createViewHolder(viewType,itemView);
        }
    }

    @Override
    public void onBindViewHolder (RecyclerView.ViewHolder holder, int position)
    {
        ViewPosition pos = positions.get(position);

        TableSection section = null;
        TableItem row = null;

        section = getSection(pos.Section);

        if (pos.Kind == ViewKind.Cell)
        {
            row = section.items[pos.Row];
        }

        if (holder instanceof IRecyclerAdapterHolder)
        {
            IRecyclerAdapterHolder vh = (IRecyclerAdapterHolder)holder;
            vh.update(section, row, pos.Section, pos.Row);
        }
    }

    @Override
    public void clickedCell(int position)
    {
        ViewPosition vp = positions.get(position);
        if (vp.Kind == ViewKind.Cell)
        {
            TableSection section = getSection(vp.Section);
            TableItem item = section.items[vp.Row];
            // TODO: Need to figure out how to enter user code from here
//            if (item.Selector != null)
//            {
//                item.Selector(this, new TableSectionsEventArgs(section, item, vp.Section, vp.Row));
//            }
//            else if (section.Selector != null)
//            {
//                section.Selector(this, new TableSectionsEventArgs(section, item, vp.Section, vp.Row));
//            }

            /*

            public class ReflectionExample {
    private static class A {
        public void foo() {
            System.out.println("fooing A now");
        }
    }

    public static void main(String[] args) throws SecurityException, NoSuchMethodException, IllegalArgumentException,
            IllegalAccessException, InvocationTargetException {
        Method method = A.class.getMethod("foo");
        method.invoke(new A());
    }
}
             */
        }
    }

    @Override
    public int getItemViewType(int position)
    {
        return positions.get(position).Type;
    }

    @Override
    public int getLayoutForHeader(int section)
    {
        return getSection(section).headerLayout;
    }

    @Override
    public int getLayoutForCell(int section,int row)
    {
        return itemWithIndexes(section,row).cellIdentInt;
    }

    @Override
    public int getLayoutForFooter(int section)
    {
        return getSection(section).footerLayout;
    }

    @Override
    public int getLayoutForDivider(int section)
    {
        return getSection(section).dividerLayout;
    }

    @Override
    public int getNumberOfSections()
    {
        TableSection[] ds = sections;
        return ds != null ? ds.length : 0;
    }

    public TableSection getSection(int section)
    {
        TableSection[] ds = sections;
        if (ds!= null)
        {
            return ds[section];
        }
        return null;
    }

    public TableItem itemWithIndexes(int section, int row)
    {
        TableSection sec = getSection(section);
        return sec.items[row];
    }

    public TableItem itemWithName(String name,String section)
    {
        for (TableSection sec : sections)
        {
            if (sec.name == section && sec.items != null)
                for (TableItem item : sec.items)
                    if (item.text!=null && item.text == name)
                        return item;
        }
        return null;
    }

    public TableItem itemWithKey(String key)
    {
        for (TableSection sec : sections)
        {
            for (TableItem item : sec.items)
                if (item.key!=null && item.key == key)
                    return item;
        }
        return null;
    }

    public TableSection sectionWithKey(String key)
    {
        for (TableSection sec : sections)
        {
            if (sec.key!=null && sec.key == key)
                return sec;
        }
        return null;
    }

    @Override
    public int numberOfRowsForSection(int section)
    {
        TableSection sec = getSection(section);
        return sec != null ? sec.count() : 0;
    }

    @Override
    public String getTitleForHeader(int section)
    {
        TableSection sec = getSection(section);
        if (sec != null && sec.hideIfEmpty && sec.count() == 0)
            return null;
        return sec == null ? null : sec.name;
    }

    @Override
    public String getTitleForFooter(int section)
    {
        TableSection sec = getSection(section);
        if (sec != null && sec.hideIfEmpty && sec.count() == 0)
            return null;
        return sec == null ? null : sec.footer;
    }

    @Override
    public int getTypeForCell(int section,int row)
    {
        TableSection s = getSection(section);
        TableItem e = s.items[row];
        if (e.cellIdentString != null)
            return getCellTypeFromIdent(e.cellIdentString);
        if (s.cellIdentString != null)
            return getCellTypeFromIdent(s.cellIdentString);
        return ViewKind.Cell;
    }

    @Override
    public int getTypeForHeader(int section)
    {
        TableSection s = getSection(section);
        if (s.headerIdentifier != null)
            return getHeaderTypeFromIdent(s.headerIdentifier);
        return ViewKind.Header;
    }

    @Override
    public int getTypeForFooter(int section)
    {
        TableSection s = getSection(section);
        if (s.footerIdentifier != null)
            return getFooterTypeFromIdent(s.footerIdentifier);
        return ViewKind.Footer;
    }

    @Override
    public int getTypeForDivider(int section)
    {
        TableSection s = getSection(section);
        if (s.dividerIdentifier != null)
            return getDividerTypeFromIdent(s.dividerIdentifier);
        return ViewKind.Divider;
    }

    protected View createViewForType(int viewType,ViewGroup parent)
    {
        ViewPosition pos = viewTypes.get(viewType);
        switch (pos.Kind)
        {
        case ViewKind.Cell:
        return createCell(parent);
        case ViewKind.Footer:
        return createFooter(parent);
        case ViewKind.Header:
        return createHeader(parent);
        case ViewKind.Divider:
        return createDivider(parent);
        }
        return null;
    }

    protected RecyclerView.ViewHolder createViewHolder(int viewType,View view)
    {
        ViewPosition pos = viewTypes.get(viewType);
        switch (pos.Kind)
        {
        case ViewKind.Cell:
        return createCellHolder(view);
        case ViewKind.Footer:
        return createFooterHolder(view);
        case ViewKind.Header:
        return createHeaderHolder(view);
        case ViewKind.Divider:
        return createDividerHolder(view);
        }
        return null;
    }
}
