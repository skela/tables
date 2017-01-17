package com.davincium.tables.holders;

import android.view.View;
import android.widget.TextView;

import com.davincium.tables.TableItem;
import com.davincium.tables.TableSection;
import com.davincium.tables.views.TableAdapterHeader;

public class TableAdapterHeaderHolder extends TableAdapterBaseHolder
{
    public TextView titleLabel;

    public TableAdapterHeaderHolder(View v)
    {
        super(v);

        if (v instanceof TableAdapterHeader)
        {
            TableAdapterHeader header = (TableAdapterHeader)v;
            titleLabel = header.titleLabel;
        }
    }

    @Override
    public void update(TableSection tsection, TableItem item, int section, int row)
    {
        titleLabel.setText(tsection.name);
    }
}