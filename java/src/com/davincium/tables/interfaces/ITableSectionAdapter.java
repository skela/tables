package com.davincium.tables.interfaces;

import com.davincium.tables.TableItem;
import com.davincium.tables.TableSection;

public interface ITableSectionAdapter
{
    void reloadData();
    TableItem itemWithIndexes(int section, int row);
    TableItem itemWithName(String name,String section);
    TableItem itemWithKey(String key);
    TableSection sectionWithKey(String key);
}