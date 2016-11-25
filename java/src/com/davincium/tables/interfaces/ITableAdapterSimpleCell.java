package com.davincium.tables.interfaces;

import android.widget.Switch;
import android.widget.TextView;

public interface ITableAdapterSimpleCell
{
    TextView getTitle();
    TextView getDetail();
    Switch getSwitch();

    void setEditable(boolean editable);
}