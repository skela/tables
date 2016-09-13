package com.davincium.tables;

import android.app.Activity;
import android.app.Fragment;
import android.content.Context;
import android.graphics.Color;
import android.support.v7.widget.TintContextWrapper;
import android.util.TypedValue;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.LinearLayout;

import com.davincium.tables.interfaces.FragmentContainer;

import java.lang.reflect.Method;

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

    public static void executeMethod(Context ctx,String selector,TableSection sec,TableItem item,int section,int row)
    {
        Activity act = null;

        if (ctx instanceof TintContextWrapper)
        {
            TintContextWrapper wrapper = (TintContextWrapper)ctx;
            Context bctx = wrapper.getBaseContext();
            if (bctx instanceof Activity)
            {
                act = (Activity)bctx;
            }
        }
        if (ctx instanceof Activity)
        {
            act = (Activity)ctx;
        }

        if (act!=null)
        {
            int id = 0;
            if (act instanceof FragmentContainer)
            {
                id = ((FragmentContainer)act).getFragmentContainer();
            }

            boolean didExectute = false;

            if (id!=0)
            {
                Fragment fragment = act.getFragmentManager().findFragmentById(id);
                didExectute = executeMethod(fragment,selector,sec,item,section,row);
            }

            if (!didExectute)
            {
                executeMethod(act,selector,sec,item,section,row);
            }
        }
    }

    static boolean executeMethod(Object object,String selector,TableSection sec,TableItem item,int section,int row)
    {
        Class classe = object.getClass();
        try
        {
            Method method1 = classe.getMethod(selector);
            method1.invoke(object, null);
            return true;
        }
        catch(Exception ex)
        {
            try
            {
                Method method2 = classe.getMethod(selector,TableItem.class);
                method2.invoke(object, item);
                return true;
            }
            catch(Exception ex2)
            {

            }
        }
        return false;
    }
}