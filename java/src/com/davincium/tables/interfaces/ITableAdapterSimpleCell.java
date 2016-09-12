package com.davincium.tables.interfaces;

import android.widget.TextView;

public interface ITableAdapterSimpleCell
{
    TextView getTitle();
    TextView getDetail();
    void setEditable(boolean editable);
}