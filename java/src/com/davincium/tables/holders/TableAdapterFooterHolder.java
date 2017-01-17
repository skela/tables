package com.davincium.tables.holders;

import android.view.View;
import android.widget.TextView;

import com.davincium.tables.TableItem;
import com.davincium.tables.TableSection;
import com.davincium.tables.views.TableAdapterFooter;

public class TableAdapterFooterHolder extends TableAdapterBaseHolder
{
    public TextView titleLabel;

    public TableAdapterFooterHolder(View v)
    {
        super(v);

        if (v instanceof TableAdapterFooter)
        {
            TableAdapterFooter footer = (TableAdapterFooter) v;
            titleLabel = footer.titleLabel;
        }
    }

    @Override
    public void update(TableSection tsection, TableItem item, int section, int row)
    {
        titleLabel.setText(tsection.footer);
    }
}