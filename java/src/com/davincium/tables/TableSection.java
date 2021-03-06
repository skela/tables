package com.davincium.tables;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class TableSection
{
    public TableItem[] items;

    public String name;
    public String footer;
    public String selector;

    public String key;
    public boolean hideIfEmpty;
    public String cellIdentString;
    public String headerIdentifier;
    public String footerIdentifier;
    public String dividerIdentifier;

    public int footerLayout;
    public int headerLayout;
    public int dividerLayout;

    public String checkedChanged;
    public boolean checkable;

    public TableSection()
    {

    }

    public TableSection(int name)
    {
        this.name = TableUtils.getString(name);
    }

    public TableSection(String name)
    {
        this.name = name;
    }

    public int count()
    {
        if (items!=null) return items.length;
        return 0;
    }

    public TableItem addItem(TableItem item)
    {
        List<TableItem>list = items != null ? new ArrayList<TableItem>(Arrays.asList(items)) : new ArrayList<TableItem>();
        list.add(item);
        items = list.toArray(new TableItem[list.size()]);
        return item;
    }

    public TableSection setFooter(int footer)
    {
        this.footer = TableUtils.getString(footer); return this;
    }
}
