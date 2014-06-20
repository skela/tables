using System;

namespace Tables
{
    public class TableSource
    {
        private Object data;
        public TableRowType DefaultStringRowType;

        public TableSource(Object data=null,TableRowType defaultStringRowType=TableRowType.Text)
        {
            this.data = data;
            this.DefaultStringRowType = defaultStringRowType;
        }

        public Object Data
        {
            get
            {
                return data;
            }
        }

        public int RowsInSection(int section)
        {
            if (data != null)
            {
                return data.GetType().GetProperties().Length;
            }
            return 0;
        }

        public string GetName(int row, int section)
        {
            var prop = data.GetType().GetProperties()[row];
            return prop.Name;
        }

        public Object GetValue(int row, int section)
        {
            var prop = data.GetType().GetProperties()[row];
            return prop.GetValue(data);
        }

        public void SetValue(Object obj, int row, int section)
        {
            var prop = data.GetType().GetProperties()[row];
            prop.SetValue(data,obj);
        }

        public string DisplayName(ITableAdapterRowConfigurator configurator,int row,int section)
        {
            var prop = data.GetType().GetProperties()[row];
            var s = RowSetting(configurator,prop.Name);
            if (s != null && s.DisplayName != null)
                return s.DisplayName;
            return prop.Name;
        }

        public string DisplayDate(ITableAdapterRowConfigurator configurator, int row, int section, DateTime date, TableRowType rowType)
        {
            var prop = data.GetType().GetProperties()[row];
            var s = RowSetting(configurator,prop.Name);

            string format = null;

            switch (rowType)
            {
                case TableRowType.Date:
                    format = "d MMM yyyy";
                break;
                case TableRowType.Time:
                    format = "HH:mm";
                break;
                case TableRowType.DateTime:
                    format = "HH:mm d MMM yyyy";
                break;
            }

            if (s != null)
            {
                switch (rowType)
                {
                    case TableRowType.Date:
                        if (s.DateFormat!=null)
                            format = s.DateFormat;
                    break;
                    case TableRowType.Time:
                        if (s.TimeFormat!=null)
                            format = s.TimeFormat;
                    break;
                    case TableRowType.DateTime:
                        if (s.DateTimeFormat!=null)
                            format = s.DateTimeFormat;
                    break;
                }
            }
            return format == null ? date.ToString() : date.ToString(format);
        }

        public TableRowType RowType(ITableAdapterRowConfigurator configurator,int row,int section)
        {
            var prop = data.GetType().GetProperties()[row];
            var ptype = prop.PropertyType;
            var s = RowSetting(configurator,prop.Name);
            if (s != null && s.RowType != TableRowType.Unknown)
                return s.RowType;
            var rowType = DefaultStringRowType;
            if (ptype == typeof(string))
                rowType = DefaultStringRowType;
            else if (ptype == typeof(bool))
                rowType = TableRowType.Checkbox;
            else if (ptype == typeof(DateTime))
                rowType = TableRowType.DateTime;
            return rowType;
        }

        public TableAdapterRowConfig RowSetting(ITableAdapterRowConfigurator configurator,string rowName)
        {
            TableAdapterRowConfig settings = null;
            if (data is ITableAdapterRowConfigurator)
                settings = (data as ITableAdapterRowConfigurator).ConfigForRow(rowName);
            else if (configurator != null)
                settings = configurator.ConfigForRow(rowName);
            return settings;
        }

        public bool Editable(ITableAdapterRowConfigurator configurator,int row,int section)
        {
            var prop = data.GetType().GetProperties()[row];
            var name = prop.Name;
            var editable = prop.CanWrite && prop.SetMethod.IsPublic;
            TableAdapterRowConfig settings = RowSetting(configurator,name);
            if (settings!=null) 
                editable = settings.Editable;
            return editable;
        }
    }
}
