package com.davincium.tables;

public class TableItem
{
    public String text;
    public String detail;
    public String selector;
    public boolean checked;

    public String key;
    public String cellIdentString;
    public int cellIdentInt;

    public Object object;

    public TableItem()
    {

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
