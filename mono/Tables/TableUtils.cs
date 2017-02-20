using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;

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
	
	public enum ReturnKeyType
	{
		Default,
		Go,
		Google,
		Join,
		Next,
		Route,
		Search,
		Send,
		Yahoo,
		Done,
		EmergencyCall,
		Continue
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

        public static T FromJSON<T>(string json)
        {
            T result=default(T);

            if (json != null)
            {
                MemoryStream stream = null;

                try
                {
                    var encoding = Encoding.UTF8;
                    byte[] bytes = encoding.GetBytes (json);
                    stream = new MemoryStream ();
                    stream.Write (bytes, 0, bytes.Length);
                    stream.Seek (0, SeekOrigin.Begin);

                    DataContractJsonSerializer ser = null;
                    ser = new DataContractJsonSerializer (typeof(T));
                    result = (T)ser.ReadObject (stream);
                }
                catch (Exception er)
                {
                    //Log.E ("JsonHelper","Failed to convert from json string: " + er.Message);
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close ();
                        stream.Dispose ();
                    }
                }
            }

            return result;
        }

        public static String ToJSON<T>(T obj)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());
            String json = null;
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream();
                ser.WriteObject(stream,obj);
                json = Encoding.UTF8.GetString(stream.ToArray());
            }
            catch (Exception er)
            {
                //Log.E ("JsonHelper","Failed to convert to json string: " + er.Message);
            }
            finally
            {
                if (stream!=null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
            return json;
        }
    }

    public class Item : object,IEquatable<Item>
    {
        public string Title;
        public string Detail;
        public object Object;
        public int Integer;
        public float Float;
        public double Double;
        public DateTime? DateTime;

        public string Key;
        public bool Selected;

        public Item (string val=null,bool selected=false,int aux=0) : base()
        {
            Selected = selected;
            Val = val;
            Integer = aux;
        }

        public bool Equals(Item other)
        {
            if (other == null) 
                return false;

            return String.Equals(this.Title, other.Title);
        }

        public string Val
        {
            get
            {
                return Title;
            }
            set
            {
                Title = value;
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
