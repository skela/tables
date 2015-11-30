using System;

namespace Tables
{
	public interface ITableSource
	{
		Object GetValue(int row, int section);
		int RowsInSection(int section);
		int NumberOfSections();
	}

	public class TableSource : ITableSource
    {
        private Object data;
        private Object[] datas;
        public TableRowType DefaultStringRowType;

        public TableSource(Object data=null,TableRowType defaultStringRowType=TableRowType.Text)
        {
            this.data = data;
            this.DefaultStringRowType = defaultStringRowType;
        }

        public TableSource(Object[] datas,TableRowType defaultStringRowType=TableRowType.Text)
        {
            this.datas = datas;
            this.DefaultStringRowType = defaultStringRowType;
        }

        public Object Data
        {
            get
            {
                return data;
            }
        }

        public Object[] Datas
        {
            get
            {
                return datas;
            }
        }

        public Object DataForSection(int section)
        {
            if (datas != null)
                return datas[section];
            return data;
        }

		public int NumberOfSections()
		{
            if (datas != null)
                return datas.Length;
			return 1;
		}

        public int RowsInSection(int section)
        {
            var d = DataForSection(section);
            if (d != null)
            {
                return d.GetType().GetProperties().Length;
            }
            return 0;
        }

        public string GetName(int row, int section)
        {
            var d = DataForSection(section);
            var prop = d.GetType().GetProperties()[row];
            return prop.Name;
        }

        public Object GetValue(int row, int section)
        {
            var d = DataForSection(section);
            var prop = d.GetType().GetProperties()[row];
            return prop.GetValue(d);
        }

        public void SetValue(Object obj, int row, int section)
        {
            var d = DataForSection(section);
            var prop = d.GetType().GetProperties()[row];
            prop.SetValue(d,obj);
        }

        public string DisplayName(ITableAdapterRowConfigurator configurator,int row,int section)
        {
            var d = DataForSection(section);
            var prop = d.GetType().GetProperties()[row];
            var s = RowSetting(configurator,prop.Name,section);
            if (s != null && s.DisplayName != null)
                return s.DisplayName;
            return prop.Name;
        }

        public string DisplayDate(ITableAdapterRowConfigurator configurator, int row, int section, DateTime date, TableRowType rowType)
        {
            var d = DataForSection(section);
            var prop = d.GetType().GetProperties()[row];
            var s = RowSetting(configurator,prop.Name,section);

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
            var d = DataForSection(section);
            var prop = d.GetType().GetProperties()[row];
            var ptype = prop.PropertyType;
            var s = RowSetting(configurator,prop.Name,section);
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

        public TableAdapterRowConfig RowSetting(ITableAdapterRowConfigurator configurator,string rowName,int section)
        {
            var d = DataForSection(section);
            TableAdapterRowConfig settings = null;
            if (d is ITableAdapterRowConfigurator)
                settings = (d as ITableAdapterRowConfigurator).ConfigForRow(rowName);
            else if (configurator != null)
                settings = configurator.ConfigForRow(rowName);
            return settings;
        }

        public TableAdapterSectionConfig SectionConfig(int section)
        {
            var d = DataForSection(section);
            if (d is ITableAdapterSectionConfigurator)
                return (d as ITableAdapterSectionConfigurator).ConfigForSection();
            return null;
        }

        public bool Editable(ITableAdapterRowConfigurator configurator,int row,int section)
        {
            var d = DataForSection(section);
            var prop = d.GetType().GetProperties()[row];
            var name = prop.Name;
            var editable = prop.CanWrite && prop.SetMethod.IsPublic;
            TableAdapterRowConfig settings = RowSetting(configurator,name,section);
            if (settings!=null) 
                editable = settings.Editable;
            return editable;
        }
    }
}
