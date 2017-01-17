package com.davincium.tables.holders;

import android.view.View;
import android.widget.CompoundButton;
import android.widget.Switch;
import android.widget.TextView;

import com.davincium.tables.TableItem;
import com.davincium.tables.TableSection;
import com.davincium.tables.interfaces.CellChecker;
import com.davincium.tables.interfaces.CellClicker;
import com.davincium.tables.interfaces.ITableAdapterSimpleCell;

import java.util.concurrent.Callable;

public class TableAdapterCellHolder extends TableAdapterBaseHolder implements View.OnClickListener,CompoundButton.OnCheckedChangeListener
{
    public TextView title;
    public TextView detail;
    public Switch checkbox;

    CellClicker clicker;
    CellChecker checker;

    public TableAdapterCellHolder(View v, CellClicker clicked)
    {
        super(v);

        clicker = clicked;

        if (clicked instanceof CellChecker)
            checker = (CellChecker)clicked;

        v.setOnClickListener(this);

        if (v instanceof ITableAdapterSimpleCell)
        {
            ITableAdapterSimpleCell cell = (ITableAdapterSimpleCell)v;
            title = cell.getTitle();
            detail = cell.getDetail();
            checkbox = cell.getSwitch();
        }
    }

    @Override
    public void update(TableSection tsection, TableItem item, int section, int row)
    {
        title.setText(item.text);
        detail.setText(item.detail);

        detail.setVisibility(detail.getText() == null || detail.getText().length() == 0 ? View.GONE : View.VISIBLE);
        if (checkbox!=null)
        {
            checkbox.setOnCheckedChangeListener(null);
            checkbox.setChecked(item.checked);
            checkbox.setVisibility(item.checked || (item.checkable || tsection.checkable) ? View.VISIBLE : View.GONE);
            checkbox.setOnCheckedChangeListener(this);
        }
    }

    @Override
    public void onClick(View view)
    {
        if (clicker!=null)
            clicker.clickedCell(super.getAdapterPosition());
    }

    @Override
    public void onCheckedChanged(CompoundButton compoundButton, boolean checked)
    {
        if (checker!=null)
            checker.cellChecked(super.getAdapterPosition(),checked);
    }
}