using System;

using Android.Widget;
using Android.Views;
using Android.Content;
using Android.Util;
using Android.Support.V4.App;
using Android.App;

namespace Tables.Droid
{
    public class TableAdapter : BaseAdapter,ITableAdapter
    {
        public TableAdapter(ListView table=null,Object data=null,ITableAdapterRowConfigurator settings=null,ITableAdapterCell aCell=null)
        {
            td = new TableSource(null,TableRowType.Blurb);
            cell = aCell;
            ListView = table;
            RowConfigurator = settings;
            Data = data;
        }

        private ITableAdapterCell cell;
        private ListView tv;
        private TableSource td;
        public ITableAdapterRowSelector RowSelector {get;set;}
        public ITableAdapterRowConfigurator RowConfigurator {get;set;}
        public ITableAdapterRowChanged RowChanged {get;set;}

        public string PositiveButtonTitle = "OK";
        public string NeutralButtonTitle = "Cancel";

        public ListView ListView
        {
            get
            {
                return tv;
            }
            set
            {
                if (tv != null)
                {
                    tv.Adapter = null;
                    tv.ItemClick -= ClickedItem;
                }
                tv = value;
                if (tv != null)
                {
                    tv.Adapter = this;
                    tv.ItemClick += ClickedItem;
                }
            }
        }

        public Object Data
        {
            get
            {
                return td.Data;             
            }
            set
            {
                td = new TableSource(value,TableRowType.Blurb);
                ReloadData();
            }
        }

        public void ReloadData()
        {
            NotifyDataSetChanged();
        }

        #region BaseAdapter

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            View view = null;

            if (convertView == null) 
            {
                if (cell == null)
                {
                    TableAdapterCell body = new TableAdapterCell(parent.Context);
                    view = body;
                }
                else
                    view = cell as View;
            } 
            else 
            {
                view = convertView;
            }

            if (view is ITableAdapterCell)
            {
                ITableAdapterCell c = view as ITableAdapterCell;
                UpdateView(c,position);
            }

            return view;
        }

        void ClickedItem(object sender, AdapterView.ItemClickEventArgs e)
        {
            RowSelected(e.Position, 0);           
        }

        public virtual void RowSelected (int row,int section)
        {
            var name = td.GetName(row,section);
            var value = td.GetValue(row,section);
            if (!td.Editable(RowConfigurator,row,section))
                return;
            var rowType = td.RowType(RowConfigurator,row,section);

            if (RowSelector != null)
            if (RowSelector.DidSelectRow(this,name))
                return;

            TableAdapterRowConfig settings = td.RowSetting(RowConfigurator,name);
            if (settings != null)
            {
                if (settings.Clicked != null)
                {
                    settings.Clicked(this, null);
                    return;
                }
            }

            switch (rowType)
            {
                case TableRowType.Checkbox:
                    var obj = !(bool)value;
                    td.SetValue(obj, row, section);
                    ReloadData();
                    ChangedValue(name,value,obj);
                break;

                case TableRowType.Blurb:
                case TableRowType.Text:

                    string str = value as string;
                    var dname = td.DisplayName(RowConfigurator, row, section);
                    var act = tv.Context as Activity;

                    AlertDialog.Builder builder = new AlertDialog.Builder(act);
                    builder.SetTitle(dname);
                    EditText input = new EditText(act);
                    input.InputType = Android.Text.InputTypes.ClassText;
                    input.Text = str;
                    input.Gravity = GravityFlags.Center;
                    input.SetPadding(10, 0, 10, 0);
                    if (rowType == TableRowType.Blurb)
                    {
                        input.SetSingleLine(false);
                    }
                    builder.SetView(input);
                    builder.SetPositiveButton(PositiveButtonTitle, delegate(object o, DialogClickEventArgs e)
                    {
                        var s = input.Text;
                        td.SetValue(s, row, section);
                        ReloadData();
                    });
                    builder.SetNeutralButton(NeutralButtonTitle, delegate(object o, DialogClickEventArgs e)
                    {

                    });
                    builder.Show();

                break;

                case TableRowType.Date:
                case TableRowType.Time:
                case TableRowType.DateTime:

                    if (tv.Context is FragmentActivity)
                    {
                        var fr = tv.Context as FragmentActivity;
                        var frag = fr.SupportFragmentManager;

                        DateTime v = (DateTime)value;

                        DialogFragment newFragment = new TableTimeEditor(v,rowType,delegate(DateTime changedDate)
                        {
                            td.SetValue(changedDate, row, section);
                            ReloadData();
                        });

                        newFragment.Show(frag, "datePicker");
                    }
                break;
            }
        }

        private void ChangedValue(string rowName,object oldValue,object newValue)
        {
            if (RowChanged != null)
                RowChanged.RowChanged(this, rowName, oldValue, newValue);
        }

        public virtual void UpdateView(ITableAdapterCell c,int row)
        {
            int section = 0;
            var name = td.GetName(row, section);
            var rowType = td.RowType(RowConfigurator,row,section);
            var value = td.GetValue(row, section);
            var editable = td.Editable(RowConfigurator,row,section);

            c.Title.Text = td.DisplayName(RowConfigurator,row,section);
            c.Editable = editable;

            switch (rowType)
            {
                case TableRowType.Text:
                    c.Blurb.Text = "";
                    c.Detail.Text = value as string;
                    c.Switch.Visibility = ViewStates.Gone;
                    c.Blurb.Visibility = ViewStates.Gone;
                break;
                case TableRowType.Blurb:
                    c.Detail.Text = "";
                    c.Switch.Visibility = ViewStates.Gone;
                    c.Blurb.Visibility = ViewStates.Visible;
                    c.Blurb.Text = value as string;
                break;
                case TableRowType.Checkbox:
                    c.Detail.Text = "";
                    c.Blurb.Text = "";
                    var s = td.RowSetting(RowConfigurator,name);
                    if (s != null && s.SimpleCheckbox)
                    {
                        c.Switch.Visibility = ViewStates.Gone;
                        c.Blurb.Visibility = ViewStates.Gone;
                        c.Detail.Text = (bool)value?"\u2713":"";
                    }
                    else
                    {
                        c.Switch.Visibility = ViewStates.Visible;
                        c.Blurb.Visibility = ViewStates.Gone;
                        c.Switch.Checked = (bool)value;
                    }
                break;
                case TableRowType.DateTime:
                case TableRowType.Date:
                case TableRowType.Time:
                    c.Switch.Visibility = ViewStates.Gone;
                    if (td.DefaultStringRowType == TableRowType.Blurb)
                    {
                        c.Blurb.Visibility = ViewStates.Visible;
                        c.Detail.Text = "";
                        c.Blurb.Text = td.DisplayDate(RowConfigurator, row, section, (DateTime)value, rowType);
                    }
                    else
                    {
                        c.Blurb.Visibility = ViewStates.Gone;
                        c.Blurb.Text = "";
                        c.Detail.Text = td.DisplayDate(RowConfigurator, row, section, (DateTime)value, rowType);
                    }
                break;
            }
        }

        public override int Count
        {
            get
            {
                return td.RowsInSection(0);
            }
        }

        #endregion       
    }
}

