using System;

namespace Tables
{
    public interface ITableAdapter
    {
        ITableAdapterRowSelector RowSelector {get;set;}
        void ReloadData();
        Object Data {get;set;}
    }

    public class TableAdapterRowConfig
    {
        public bool Editable = true;
        public EventHandler Clicked = null;
        public TableRowType RowType = TableRowType.Unknown;
        public string DisplayName = null;

        public string DateFormat = null;
        public string TimeFormat = null;
        public string DateTimeFormat = null;
        public bool SimpleCheckbox = false;
    }

    public class TestData : ITableAdapterRowConfigurator
    {
        public string Version { get; private set;}
        public string Build { get; set; }
        public string Name { get; set; }
        public bool Cool { get; set; }
        public DateTime Time1 { get; set; }
        public DateTime Time2 { get; set; }
        public DateTime Time3 { get; set; }
        public string Stuff {get;set;}
        public string Stuff2 {get;set;}
        public bool Chosen { get; set; }

        public TestData(string version)
        {
            Version = version;
        }

        public static TestData CreateTestData()
        {
            var data = new TestData(version: "1.0.0")
            {
                Name = "Bob", 
                Chosen = false, 
                Build = "2",
                Stuff = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                Cool = true,
                Time1 = new DateTime(2013,3,14,13,37,00),
                Time2 = new DateTime(2013,3,14,13,37,00),
                Time3 = new DateTime(2013,3,14,13,37,00),
            };
            return data;
        }

        public TableAdapterRowConfig ConfigForRow(string rowName)
        {
            if (rowName.Equals("Build"))
                return new TableAdapterRowConfig(){ Editable = false };
            if (rowName.Equals("Stuff"))
                return new TableAdapterRowConfig(){ RowType = TableRowType.Blurb };
            if (rowName.Equals("Cool"))
                return new TableAdapterRowConfig(){ DisplayName = "Is Cool?" };
            if (rowName.Equals("Time2"))
                return new TableAdapterRowConfig(){ RowType = TableRowType.Date };
            if (rowName.Equals("Time3"))
                return new TableAdapterRowConfig(){ RowType = TableRowType.Time };
            if (rowName.Equals("Chosen"))
                return new TableAdapterRowConfig(){ SimpleCheckbox = true };
            return null;
        }
    }
}
