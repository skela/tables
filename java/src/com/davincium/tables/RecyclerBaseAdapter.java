package com.davincium.tables;

import android.support.v7.widget.RecyclerView;
import android.view.View;
import android.view.ViewGroup;

import com.davincium.tables.holders.TableAdapterCellHolder;
import com.davincium.tables.holders.TableAdapterDividerHolder;
import com.davincium.tables.holders.TableAdapterFooterHolder;
import com.davincium.tables.holders.TableAdapterHeaderHolder;
import com.davincium.tables.interfaces.CellClicker;
import com.davincium.tables.interfaces.IRecyclerAdapterDelegate;
import com.davincium.tables.views.TableAdapterFooter;
import com.davincium.tables.views.TableAdapterHeader;
import com.davincium.tables.views.TableAdapterRecyclerCell;
import com.davincium.tables.views.TableAdapterSectionSeperator;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public abstract class RecyclerBaseAdapter extends RecyclerView.Adapter implements CellClicker
{
    static class ViewKind
    {
        public static final int Header=1;
        public static final int Footer=1000;
        public static final int Divider=10000;
        public static final int Cell=100000;
    }

    public class ViewPosition
    {
        public int Kind;
        public int Row;
        public int Section;
        public int Position;
        public int Type;
        public int Layout;
        public String Cell;
    }

    protected List<ViewPosition> positions = new ArrayList<ViewPosition>();
    protected Map<Integer,ViewPosition> viewTypes = new HashMap<Integer,ViewPosition>();

    protected Map<Integer,IRecyclerAdapterDelegate> cellDelegates = new HashMap<Integer,IRecyclerAdapterDelegate>();
    protected Map<Integer,IRecyclerAdapterDelegate> headerDelegates = new HashMap<Integer,IRecyclerAdapterDelegate>();
    protected Map<Integer,IRecyclerAdapterDelegate> footerDelegates = new HashMap<Integer,IRecyclerAdapterDelegate>();
    protected Map<Integer,IRecyclerAdapterDelegate> dividerDelegates = new HashMap<Integer,IRecyclerAdapterDelegate>();

    protected Map<Integer,IRecyclerAdapterDelegate> delegatesForViewKind(int viewType)
    {
        ViewPosition vp = viewTypes.get(viewType);
        switch (vp.Kind)
        {
            case ViewKind.Cell:
                return cellDelegates;
            case ViewKind.Divider:
                return dividerDelegates;
            case ViewKind.Footer:
                return footerDelegates;
            case ViewKind.Header:
                return headerDelegates;
        }
        return null;
    }

    public void registerCell(IRecyclerAdapterDelegate deleg)
    {
        int type = ViewKind.Cell + cellDelegates.size() + 1;
        deleg.setViewType(type);
        cellDelegates.put(type, deleg);
    }

    public void registerHeader(IRecyclerAdapterDelegate deleg)
    {
        int type = ViewKind.Header + headerDelegates.size() + 1;
        deleg.setViewType(type);
        headerDelegates.put(type, deleg);
    }

    public void registerFooter(IRecyclerAdapterDelegate deleg)
    {
        int type =ViewKind.Footer + footerDelegates.size() + 1;
        deleg.setViewType(type);
        footerDelegates.put(type, deleg);
    }

    public void registerDivider(IRecyclerAdapterDelegate deleg)
    {
        int type = ViewKind.Divider + dividerDelegates.size() + 1;
        deleg.setViewType(type);
        dividerDelegates.put(type, deleg);
    }

    public int getLayoutForHeader(int section) { return 0; }
    public int getLayoutForFooter(int section) { return 0; }
    public int getLayoutForDivider(int section) { return 0; }
    public int getLayoutForCell(int section,int row) { return 0; }

    public int getTypeForCell(int section,int row)
    {
        return ViewKind.Cell;
    }

    public int getTypeForHeader(int section)
    {
        return ViewKind.Header;
    }

    public int getTypeForFooter(int section)
    {
        return ViewKind.Footer;
    }

    public int getTypeForDivider(int section)
    {
        return ViewKind.Divider;
    }

    public int getNumberOfSections()
    {
        return 1;
    }

    protected int RowCount = 0;
    protected int HeaderCount = 0;
    protected int FooterCount = 0;
    protected int SectionCount = 0;
    protected int DividerCount = 0;

    public int getCellTypeFromIdent(String cell)
    {
        for (IRecyclerAdapterDelegate ch : cellDelegates.values())
        {
            if (ch.getIdentifier() == cell)
                return ch.getViewType();
        }
        return ViewKind.Cell;
    }

    public int getHeaderTypeFromIdent(String cell)
    {
        for (IRecyclerAdapterDelegate ch : headerDelegates.values())
        {
            if (ch.getIdentifier() == cell)
                return ch.getViewType();
        }
        return ViewKind.Header;
    }

    public int getFooterTypeFromIdent(String cell)
    {
        for (IRecyclerAdapterDelegate ch : footerDelegates.values())
        {
            if (ch.getIdentifier() == cell)
                return ch.getViewType();
        }
        return ViewKind.Footer;
    }

    public int getDividerTypeFromIdent(String cell)
    {
        for (IRecyclerAdapterDelegate ch : dividerDelegates.values())
        {
            if (ch.getIdentifier() == cell)
                return ch.getViewType();
        }
        return ViewKind.Divider;
    }

    @Override
    public int getItemCount()
    {
        return RowCount + HeaderCount + FooterCount + DividerCount;
    }

    public int numberOfRowsForSection(int section)
    {
        return 0;
    }

    public String getTitleForHeader(int section)
    {
        return null;
    }

    public String getTitleForFooter(int section)
    {
        return null;
    }

    public void clickedCell(int position)
    {

    }

    public TableStyle Style = new TableStyle();

    protected View createHeader(ViewGroup parent)
    {
        return new TableAdapterHeader(parent.getContext(),null,Style.DefaultHeaderLayoutStyle,Style.DefaultHeaderStyle);
    }

    protected View createCell(ViewGroup parent)
    {
        return new TableAdapterRecyclerCell(parent.getContext(), null, Style.DefaultCellLayoutStyle,Style.DefaultTitleStyle,Style.DefaultDetailStyle);
    }

    protected View createFooter(ViewGroup parent)
    {
        return new TableAdapterFooter(parent.getContext(),null,Style.DefaultFooterLayoutStyle,Style.DefaultFooterStyle);
    }

    protected View createDivider(ViewGroup parent)
    {
        return new TableAdapterSectionSeperator(parent.getContext(), null, Style.DefaultSectionSeparatorStyle);
    }

    protected RecyclerView.ViewHolder createHeaderHolder(View view)
    {
        return new TableAdapterHeaderHolder(view);
    }

    protected RecyclerView.ViewHolder createCellHolder(View view)
    {
        return new TableAdapterCellHolder(view,this);
    }

    protected RecyclerView.ViewHolder createFooterHolder(View view)
    {
        return new TableAdapterFooterHolder(view);
    }

    protected RecyclerView.ViewHolder createDividerHolder(View view)
    {
        return new TableAdapterDividerHolder(view);
    }

    @Override
    public long getItemId(int position)
    {
        return position;
    }

    private void reload()
    {
        RowCount = 0;
        HeaderCount = 0;
        FooterCount = 0;
        DividerCount = 0;

        SectionCount = getNumberOfSections();

        for (int i = 0; i < SectionCount; i++)
        {
            RowCount += numberOfRowsForSection(i);

            String headerTitle = getTitleForHeader(i);
            if (headerTitle != null)
                HeaderCount++;

            String footerTitle = getTitleForFooter(i);
            if (footerTitle != null)
                FooterCount++;

            if (getTypeForDivider(i) != ViewKind.Divider)
                DividerCount++;
        }

        int p = 0;
        int headers = HeaderCount;
        int footers = FooterCount;
        int seperators = DividerCount;

        positions.clear();

        for(int sec=0; sec < SectionCount; sec++)
        {
            if (headers > 0 && getTitleForHeader(sec)!=null)
            {
                ViewPosition pos = new ViewPosition();
                pos.Section = sec;
                pos.Kind = ViewKind.Header;
                pos.Position = p;
                pos.Layout = getLayoutForHeader(sec);
                pos.Type = getTypeForHeader(sec);
                positions.add(pos);
                headers--;
                p++;
            }

            for (int row = 0; row < numberOfRowsForSection(sec); row++)
            {
                ViewPosition pos = new ViewPosition();
                pos.Row = row;
                pos.Section = sec;
                pos.Position = p;
                pos.Kind = ViewKind.Cell;
                pos.Layout = getLayoutForCell(sec,row);
                pos.Type = getTypeForCell(sec, row);
                positions.add(pos);
                p++;
            }

            if (footers > 0 && getTitleForFooter(sec)!=null)
            {
                ViewPosition pos = new ViewPosition();
                pos.Section = sec;
                pos.Kind = ViewKind.Footer;
                pos.Position = p;
                pos.Layout = getLayoutForFooter(sec);
                pos.Type = getTypeForFooter(sec);
                positions.add(pos);
                footers--;
                p++;
            }

            if (seperators > 0)
            {
                ViewPosition pos = new ViewPosition();
                pos.Section = sec;
                pos.Kind = ViewKind.Divider;
                pos.Position = p;
                pos.Layout = getLayoutForDivider(sec);
                pos.Type = getTypeForDivider(sec);
                positions.add(pos);
                seperators--;
                p++;
            }
        }

        viewTypes.clear();
        for (ViewPosition pos : positions)
            viewTypes.put(pos.Type,pos);
    }

    public void reloadData()
    {
        reload();
        notifyDataSetChanged();
    }
}