package com.davincium.tables.views;

import android.content.Context;
import android.content.res.TypedArray;
import android.util.AttributeSet;
import android.view.View;

public class TableAdapterSectionSeperator extends View
{
    public TableAdapterSectionSeperator(Context context)
    {
        super(context);
        prepare(context);
    }

    public TableAdapterSectionSeperator (Context context, AttributeSet attrs)
    {
        super(context,attrs);
        prepare (context);
    }

    public TableAdapterSectionSeperator (Context context, AttributeSet attrs, int defStyle)
    {
        super(context,attrs,defStyle);
        prepare (context);
        applyStyle(context,defStyle);
    }

    private void prepare(Context context)
    {
        setFocusable(false);
        setClickable(false);
        setFocusableInTouchMode(false);
    }

    void applyStyle(Context context,int defStyle)
    {
        if (defStyle == 0) return;

        int[] attrs = {android.R.attr.background,android.R.attr.minHeight};

        TypedArray a = context.obtainStyledAttributes(defStyle, attrs);

        int bg = a.getResourceId(0, 0);
        if (bg != 0) setBackgroundResource(bg);

        int mh = a.getDimensionPixelSize(1, 0);
        if (mh != 0) setMinimumHeight(mh);

        a.recycle();
    }
}