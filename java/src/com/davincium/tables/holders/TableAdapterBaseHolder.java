package com.davincium.tables.holders;

import android.support.v7.widget.RecyclerView;
import android.view.View;

import com.davincium.tables.TableItem;
import com.davincium.tables.TableSection;
import com.davincium.tables.interfaces.IRecyclerAdapterHolder;

public class TableAdapterBaseHolder extends RecyclerView.ViewHolder implements IRecyclerAdapterHolder
{
    public TableAdapterBaseHolder(View v)
    {
        super(v);
    }

    @Override
    public void update(TableSection tsection, TableItem item, int section, int row)
    {

    }
}