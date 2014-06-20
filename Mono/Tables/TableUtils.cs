using System;

namespace Tables
{
    public enum TableRowType
    {
        Unknown,
        Text,
        Blurb,
        Checkbox,
        Date,
        Time,
        DateTime
    } 

    public interface ITableAdapterRow
    {
        string TableRowName { get; }
        object TableRowValue { get; }
    }

    public interface ITableAdapterRowChanged
    {
        void RowChanged(ITableAdapter tableAdapter, string rowName, object oldValue, object newValue);
    }

    public interface ITableAdapterRowConfigurator
    {
        TableAdapterRowConfig ConfigForRow(string rowName);
    }

    public interface ITableAdapterRowSelector
    {
        bool DidSelectRow(ITableAdapter adapter,string rowName);
    }
}
