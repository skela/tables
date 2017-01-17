package com.davincium.tables.views;

import android.content.Context;
import android.util.AttributeSet;
import android.view.Gravity;

import com.davincium.tables.TableUtils;

public class TableAdapterFooter extends TableAdapterHF implements TableAdapterHF.ITableAdapterFooter
{
    private int kDefaultTextStyle = TableUtils.DefaultFooterStyle;

    public TableAdapterFooter(Context context)
    {
        super(context);
        textStyle = kDefaultTextStyle;
        prepare(context);
    }

    public TableAdapterFooter (Context context, AttributeSet attrs)
    {
        super(context,attrs);
        textStyle = kDefaultTextStyle;
        prepare (context);
    }

    public TableAdapterFooter (Context context, AttributeSet attrs, int defStyle)
    {
        super(context,attrs,defStyle);
        textStyle = kDefaultTextStyle;
        prepare (context);
    }

    public TableAdapterFooter (Context context, AttributeSet attrs, int defStyle, int textStyl)
    {
        super(context,attrs,defStyle);
        textStyle = textStyl;
        prepare (context);
    }

    private void prepare(Context context)
    {
        titleLabel.setGravity(Gravity.CENTER);
    }

    @Override
    protected boolean isFooter()
    {
        return true;
    }
}