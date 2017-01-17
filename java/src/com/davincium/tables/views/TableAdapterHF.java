package com.davincium.tables.views;

import android.content.Context;
import android.content.res.TypedArray;
import android.graphics.Color;
import android.util.AttributeSet;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.davincium.tables.TableUtils;

public class TableAdapterHF extends LinearLayout
{
    public interface ITableAdapterHeaderFooter
    {
        void setTitle(String title);
        String getTitle();
    }

    public interface ITableAdapterHeader extends ITableAdapterHeaderFooter
    {

    }

    public interface ITableAdapterFooter extends ITableAdapterHeaderFooter
    {

    }

    public TextView titleLabel;
    public View separator;

    public TableAdapterHF(Context context)
    {
        super(context);
        construct(context);
    }

    public TableAdapterHF(Context context, AttributeSet attrs)
    {
        super(context, attrs);
        construct(context);
    }

    public TableAdapterHF(Context context, AttributeSet attrs, int defStyle)
    {
        super(context, attrs, defStyle);
        construct(context);
        applyLayoutStyle(context, defStyle);
    }

    private void construct(Context context)
    {
        setOrientation(LinearLayout.VERTICAL);

        separator = new View(context);
        separator.setLayoutParams(new ViewGroup.LayoutParams(LayoutParams.MATCH_PARENT, 1));
        separator.setBackgroundColor(Color.DKGRAY);
        boolean isFooter = isFooter();
        if (!isFooter)
            addView(separator);

        titleLabel = new TextView(context);
        titleLabel.setText("Title");
        titleLabel.setTextAppearance(context, textStyle);
        titleLabel.setGravity(Gravity.LEFT);
        titleLabel.setLayoutParams(new ViewGroup.LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.WRAP_CONTENT));
        addView(titleLabel);
        if (isFooter)
            addView(separator);

        setFocusable(false);
        setClickable(false);
        setFocusableInTouchMode(false);

        int pixels = TableUtils.getPixelsFromDPI(context, 10);
        titleLabel.setPadding(pixels, pixels, pixels, pixels);
    }

    protected boolean isFooter()
    {
        return false;
    }

    public int getTextStyle()
    {
        return textStyle;
    }

    public void setTextStyle(int value)
    {
        textStyle = value;
        if (titleLabel != null)
            titleLabel.setTextAppearance(titleLabel.getContext(), getTextStyle());
    }
    protected int textStyle;

    public String getTitle()
    {
        return titleLabel.getText().toString();
    }

    public void setTitle(String value)
    {
        titleLabel.setText(value);
        titleLabel.setVisibility(value == null || value.length() == 0 ? View.GONE : View.VISIBLE);
    }

    void applyLayoutStyle(Context context,int defStyle)
    {
        if (defStyle == 0) return;

        int[] attrs = {android.R.attr.background,android.R.attr.minHeight};

        TypedArray a = context.obtainStyledAttributes(defStyle, attrs);

        int bg = a.getResourceId(0, 0);
        if (bg != 0)
            setBackgroundResource(bg);

        int mh = a.getDimensionPixelSize(1, 0);
        if (mh != 0)
            setMinimumHeight(mh);

        a.recycle();
    }
}