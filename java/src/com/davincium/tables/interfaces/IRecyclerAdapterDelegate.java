package com.davincium.tables.interfaces;

import android.support.v7.widget.RecyclerView;
import android.view.View;
import android.view.ViewGroup;

public interface IRecyclerAdapterDelegate
{
    String getIdentifier();
    int getLayout();
    int getViewType();
    void setViewType(int vt);
    RecyclerView.ViewHolder createViewHolder(View view, CellClicker clicked);
    View createView(ViewGroup parent);
}