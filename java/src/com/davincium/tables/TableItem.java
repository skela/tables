package com.davincium.tables;

import android.app.Application;
import android.content.Context;

public class TableItem
{
    public String text;
    public String detail;
    public String selector;

    public String checkedChanged;
    public boolean checked;
    public boolean checkable;

    public String key;
    public String cellIdentString;
    public int cellIdentInt;

    public Object object;

    public TableItem()
    {

    }

    public TableItem(int name)
    {
        text = TableUtils.getString(name);
    }

    public TableItem(int name,String method)
    {
        text = TableUtils.getString(name);
        selector = method;
    }

    public TableItem(int name,String det,String method)
    {
        text = TableUtils.getString(name);
        detail = det;
        selector = method;
    }

    public TableItem(String name)
    {
        text = name;
    }

    public TableItem(String name,String method)
    {
        text = name;
        selector = method;
    }

    public TableItem(String name,String det,String method)
    {
        text = name;
        detail = det;
        selector = method;
    }

    public TableItem setKey(String key)
    {
        this.key = key; return this;
    }

    public TableItem setObject(Object obj)
    {
        this.object = obj; return this;
    }
}
