package com.davincium.tables.interfaces;

import com.davincium.tables.TableItem;
import com.davincium.tables.TableSection;

public interface IRecyclerAdapterHolder
{
    void update(TableSection tsection, TableItem item, int section, int row);
}