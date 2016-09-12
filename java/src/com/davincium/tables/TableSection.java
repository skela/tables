package com.davincium.tables;

import java.util.ArrayList;
import java.util.List;

public class TableSection
{
    public TableItem[] items;

    public String name;
    public String footer;

    public String key;
    public boolean hideIfEmpty;
    public String cellIdentString;
    public String headerIdentifier;
    public String footerIdentifier;
    public String dividerIdentifier;

    public int footerLayout;
    public int headerLayout;
    public int dividerLayout;

    public int count()
    {
        if (items!=null) return items.length;
        return 0;
    }
}
