package com.davincium.tables;

import android.support.v7.widget.RecyclerView;
import android.view.View;
import android.view.ViewGroup;

import com.davincium.tables.interfaces.CellClicker;
import com.davincium.tables.interfaces.IRecyclerAdapterDelegate;

public abstract class RecyclerAdapterDelegate implements IRecyclerAdapterDelegate
{
    public String getIdentifier() { return null; }

    public int getLayout() { return 0; }

    public int getViewType() { return vt; }
    public void setViewType(int vt) { this.vt = vt; }
    private int vt;

    public RecyclerView.ViewHolder createViewHolder(View view, CellClicker clicked) { return null; }

    public View createView(ViewGroup parent) { return null; }
}
