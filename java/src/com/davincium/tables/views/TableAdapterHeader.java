package com.davincium.tables.views;

import android.content.Context;
import android.util.AttributeSet;

import com.davincium.tables.TableUtils;

public class TableAdapterHeader extends TableAdapterHF implements TableAdapterHF.ITableAdapterHeader
{
    private int kDefaultTextStyle = TableUtils.DefaultHeaderStyle;

    public TableAdapterHeader(Context context)
    {
        super(context);
        textStyle = kDefaultTextStyle;
        prepare(context);
    }

    public TableAdapterHeader (Context context, AttributeSet attrs)
    {
        super(context,attrs);
        textStyle = kDefaultTextStyle;
        prepare (context);
    }

    public TableAdapterHeader (Context context, AttributeSet attrs, int defStyle)
    {
        super(context,attrs,defStyle);
        textStyle = kDefaultTextStyle;
        prepare (context);
    }

    public TableAdapterHeader (Context context, AttributeSet attrs, int defStyle, int textStyle)
    {
        super(context,attrs,defStyle);
        textStyle = textStyle;
        prepare (context);
    }

    private void prepare(Context context)
    {

    }
}