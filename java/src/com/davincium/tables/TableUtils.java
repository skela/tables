package com.davincium.tables;

import android.content.Context;
import android.graphics.Color;
import android.util.TypedValue;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.LinearLayout;

public class TableUtils
{
    public static int DefaultHeaderStyle = android.R.style.TextAppearance_DeviceDefault_Medium;
    public static int DefaultFooterStyle = android.R.style.TextAppearance_DeviceDefault_Small;
    public static int DefaultTitleStyle = android.R.style.TextAppearance_DeviceDefault_Medium;
    public static int DefaultDetailStyle = android.R.style.TextAppearance_DeviceDefault_Small;

    public static int getPixelsFromDPI(Context ctx, int dpi)
    {
        int pixels = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP,dpi,ctx.getResources().getDisplayMetrics());
        return pixels;
    }

    public static EditText createEditableTextField(Context ctx)
    {
        EditText et = new EditText(ctx);
        et.setBackgroundColor(Color.TRANSPARENT);
        et.setTextColor(Color.WHITE);
        et.setLayoutParams(new ViewGroup.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT));
        et.setMinimumHeight(TableUtils.getPixelsFromDPI(ctx,40));
        return et;
    }
}