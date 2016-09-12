package com.davincium.tables.holders;

import android.view.View;
import android.widget.TextView;

import com.davincium.tables.TableItem;
import com.davincium.tables.TableSection;
import com.davincium.tables.interfaces.CellClicker;
import com.davincium.tables.interfaces.ITableAdapterSimpleCell;

import java.util.concurrent.Callable;

public class TableAdapterCellHolder extends TableAdapterBaseHolder implements View.OnClickListener
{
    public TextView title;
    public TextView detail;

    CellClicker clicker;

    public TableAdapterCellHolder(View v, CellClicker clicked)
    {
        super(v);

        clicker = clicked;

        v.setOnClickListener(this);

        if (v instanceof ITableAdapterSimpleCell)
        {
            ITableAdapterSimpleCell cell = (ITableAdapterSimpleCell)v;
            title = cell.getTitle();
            detail = cell.getDetail();
        }
    }

    @Override
    public void update(TableSection tsection, TableItem item, int section, int row)
    {
        title.setText(item.text);
        detail.setText(item.detail);

        detail.setVisibility(detail.getText() == null || detail.getText().length() == 0 ? View.GONE : View.VISIBLE);
    }

    @Override
    public void onClick(View view)
    {
        if (clicker!=null)
            clicker.clickedCell(super.getAdapterPosition());
    }
}