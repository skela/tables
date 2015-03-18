using System;
using System.Text;

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
        DateTime,
		SingleChoiceList,
    } 

	public enum KeyboardType
	{
		Ignore,
		Default,
		ASCIICapable,
		NumbersAndPunctuation,
		Url,
		NumberPad,
		PhonePad,
		NamePhonePad,
		EmailAddress,
		DecimalPad,
	}

	public enum CapitalizationType
	{
		Ignore,
		None,
		Words,
		Sentences,
		AllCharacters
	}

	public enum CorrectionType
	{
		Ignore,
		Default,
		No,
		Yes
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
		
	public interface TableAdapterItemSelector
	{
		void DidSelectItem(Object item);
	}

	public interface TableAdapterItemInformer
	{
		string ItemText(Object item);
		string ItemDetails(Object item);
	}

    public static class TextHelper
    {
        public static string ScrambledText(string str)
        {
            if (str == null || str.Length==0)
                return "";
            return new string('*', str.Length);
        }
    }
}
