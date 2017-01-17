package com.davincium.tables.views;

import android.content.Context;
import android.content.res.TypedArray;
import android.graphics.Color;
import android.util.AttributeSet;
import android.widget.LinearLayout;
import android.widget.Switch;
import android.widget.TextView;

import com.davincium.tables.TableUtils;
import com.davincium.tables.interfaces.ITableAdapterSimpleCell;

public class TableAdapterRecyclerCell extends LinearLayout implements ITableAdapterSimpleCell
{
    private TextView title;
    private TextView detail;
    private Switch checkbox;

    private int titleStyle = TableUtils.DefaultTitleStyle;
    private int detailStyle = TableUtils.DefaultDetailStyle;

    public TableAdapterRecyclerCell(Context context)
    {
        super(context);
        prepare(context);
    }

    public TableAdapterRecyclerCell (Context context, AttributeSet attrs)
    {
        super(context,attrs);
        prepare (context);
    }

    public TableAdapterRecyclerCell (Context context, AttributeSet attrs, int defStyle)
    {
        super(context,attrs,defStyle);
        prepare (context);
    }

    public TableAdapterRecyclerCell (Context context, AttributeSet attrs, int defStyle, int aTitleStyle, int aDetailStyle)
    {
        super(context,attrs,defStyle);
        titleStyle = aTitleStyle;
        detailStyle = aDetailStyle;
        prepare (context);
    }

    private void prepare(Context context)
    {
        setLayoutParams(new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.WRAP_CONTENT, 1f));

        int[] attrs = new int[] { android.R.attr.selectableItemBackground };
        TypedArray typedArray = context.obtainStyledAttributes(attrs);
        int backgroundResource = typedArray.getResourceId(0, 0);
        setBackgroundResource(backgroundResource);
        typedArray.recycle();

        LinearLayout container = new LinearLayout(context);
        container.setLayoutParams(new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.WRAP_CONTENT, 1f));
        container.setOrientation(LinearLayout.VERTICAL);

        int pixels = TableUtils.getPixelsFromDPI(context, 2);

        title = new TextView(context);
        title.setText("Title");

        LayoutParams tparams = new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.WRAP_CONTENT, 1f);
        tparams.setMargins(0, pixels, 0, pixels);
        title.setLayoutParams(tparams);
        title.setTextAppearance(context, titleStyle);
        container.addView(title);

        detail = new TextView(context);
        detail.setText("Detail");

        LayoutParams lparams = new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.WRAP_CONTENT, 1f);
        lparams.setMargins(0, pixels, 0, pixels);
        detail.setLayoutParams(lparams);
        detail.setTextAppearance(context, detailStyle);
        container.addView(detail);

        addView(container);

        checkbox = new Switch(context);
        checkbox.setLayoutParams(new LayoutParams(LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT, 0f));
        addView(checkbox);

        setOrientation(LinearLayout.HORIZONTAL);
        pixels = TableUtils.getPixelsFromDPI(context, 10);
        setPadding(pixels, pixels, pixels, pixels);
    }

    public TextView getTitle()
    {
        return title;
    }

    public TextView getDetail()
    {
        return detail;
    }

    public Switch getSwitch()
    {
        return checkbox;
    }

    public void setEditable(boolean editable)
    {
        setFocusable(!editable);
    }

    public void setTitleStyle(Context ctx,int style)
    {
        titleStyle = style;
        if (title != null)
            title.setTextAppearance(ctx, titleStyle);
    }

    public void setDetailStyle(Context ctx,int style)
    {
        detailStyle = style;
        if (detail != null)
            detail.setTextAppearance(ctx, detailStyle);
    }
}
