using System;
using System.Collections.Generic;

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
		public bool InlineTextEditing = false;
        public bool SecureTextEditing = false;
		public KeyboardType KeyboardType = KeyboardType.Ignore;
		public CorrectionType CorrectionType = CorrectionType.Ignore;
		public CapitalizationType CapitalizationType = CapitalizationType.Ignore;
		public List<Object>SingleChoiceOptions = null;
    }

    public class TableAdapterSectionConfig
    {
        public string Header {get;set;}
        public string Footer {get;set;}
    }

    public interface ITableAdapterSectionConfigurator
    {
        TableAdapterSectionConfig ConfigForSection();
    }

	public class TableAdapterRowConfigs : ITableAdapterRowConfigurator
	{
		public Dictionary<string,TableAdapterRowConfig> Configs = new Dictionary<string,TableAdapterRowConfig> ();

		public TableAdapterRowConfig ConfigForRow(string rowName)
		{
			if (Configs.ContainsKey (rowName))
			{
				return Configs [rowName];
			}
			return null;
		}
	}

    public enum ETestDataItemOption
    {
        Nothing,
        Cats,
        Pizza
    }

    public class TestData2 : ITableAdapterSectionConfigurator
    {
        public string Row1 { get; private set;}
        public string Row2{ get; private set;}

        private TableAdapterSectionConfig config = new TableAdapterSectionConfig() { Header = "Section 2" };
        public TableAdapterSectionConfig ConfigForSection() { return config; }
    }

    public class TestData : ITableAdapterRowConfigurator
    {
        public string Version { get; private set;}
        public string Build { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Password { get; set; }
        public ETestDataItemOption SingleChoice { get; set; }
        public bool Cool { get; set; }
        public DateTime Time1 { get; set; }
        public DateTime Time2 { get; set; }
        public DateTime Time3 { get; set; }
        public string Stuff {get;set;}
        public string Stuff2 {get;set;}
        public bool Chosen { get; set; }

        private TableAdapterRowConfigs configs;

        public TestData(string version)
        {
            Version = version;

            configs = new TableAdapterRowConfigs();

            configs.Configs.Add("Build", new TableAdapterRowConfig(){ Editable = false });
            configs.Configs.Add("Stuff", new TableAdapterRowConfig(){ RowType = TableRowType.Blurb });
            configs.Configs.Add("Cool", new TableAdapterRowConfig(){ DisplayName = "Is Cool?" });
            configs.Configs.Add("Telephone", new TableAdapterRowConfig(){ KeyboardType=KeyboardType.PhonePad });
            configs.Configs.Add("Password", new TableAdapterRowConfig(){ SecureTextEditing = true, InlineTextEditing = true });
            configs.Configs.Add("Time2", new TableAdapterRowConfig(){ RowType = TableRowType.Date });
            configs.Configs.Add("Time3", new TableAdapterRowConfig(){ RowType = TableRowType.Time });
            configs.Configs.Add("Chosen", new TableAdapterRowConfig(){ SimpleCheckbox = true });

            var singleChoiceOptions = new List<object>();
            singleChoiceOptions.Add(ETestDataItemOption.Nothing);
            singleChoiceOptions.Add(ETestDataItemOption.Cats);
            singleChoiceOptions.Add(ETestDataItemOption.Pizza);

            configs.Configs.Add("SingleChoice", new TableAdapterRowConfig(){ RowType = TableRowType.SingleChoiceList, DisplayName = "Single Choice",SingleChoiceOptions=singleChoiceOptions });
        }

        public static TestData CreateTestData()
        {
            var data = new TestData(version: "1.0.0")
            {
                Name = "Bob", 
                Chosen = false, 
                Build = "2",
                Telephone = "1337",
                Password = "1234",
                Stuff = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                Cool = true,
                Time1 = new DateTime(2013,3,14,13,37,00),
                Time2 = new DateTime(2013,3,14,13,37,00),
                Time3 = new DateTime(2013,3,14,13,37,00),
                SingleChoice = ETestDataItemOption.Pizza,
            };
            return data;
        }

        public static object[] CreateSectionedTestData()
        {
            var objs = new object[]{ CreateTestData(), new TestData2() };
            return objs;
        }

        public static TableSection[] CreateSectionsTestData()
        {
            var section1 = new TableSection("Section 1")
            {
                ItemArray = new TableItem[]
                {
                    new TableItem("Test 1")
                    {
                        Detail = "This is a test"
                    }
                }
            };
            var section2 = new TableSection("Section 2")
            {
                ItemArray = new TableItem[]
                {
                    new TableItem("Test 2")
                    {
                        Detail = "This is another test"
                    }
                }
            };

            return new TableSection[] { section1, section2 };
        }

        public TableAdapterRowConfig ConfigForRow(string rowName)
        {
            return configs.ConfigForRow(rowName);
        }
    }

    public interface ITableSectionAdapter
    {
        TableItem ItemWithIndexes(int section, int row);
        TableItem ItemWithName(string name,string section);
        TableItem ItemWithKey(string key);
    }

    public class TableSection
    {
        public List<TableItem> Items;
        public string Name;
        public object Object;
		public string Footer;

        public string CellIdentString;
        public int CellIdentInt;

		public TableSection(string name)
		{
			Name = name;
		}

		public TableSection(string name,TableItem[] items)
		{
			Name = name;
			ItemArray = items;
		}

		public TableItem[] ItemArray
		{
			set
			{
				if (value != null)
					Items = new List<TableItem> (value);
				else
					Items = null;
			}
		}

        public int ItemCount
        {
            get
            {
                return Items != null ? Items.Count : 0;
            }
        }
    }

    public class TableItem
    {
        public string Text;
        public string Detail;
        public string Key;
        public EventHandler Selector;
        public EventHandler DeleteSelector;
		public string DeleteTitle;
        public object Object;
        public bool Checked;
        public int Badge;

		public object AttributedText;
		public object AttributedDetail;
        public string ImageName;
        public int ImageResource;
        public string CellIdentString;
        public int CellIdentInt;
		public double CellHeight;

		public TableItem()
		{

		}

		public TableItem(string text)
		{
			Text = text;
		}
    }
}
