using System;

using Android.Widget;
using Android.Views;
using Android.Content;
using Android.Util;

using Android.App;
using Android.Text;
using Android.Text.Method;
using Android.Views.InputMethods;

namespace Tables.Droid
{
    public class TableAdapter : TableSectionedAdapter,ITableAdapter
    {
        public TableAdapter(Context ctx,ListView table=null,Object data=null,ITableAdapterRowConfigurator settings=null) : base(ctx)
        {
            td = new TableSource(null,TableRowType.Blurb);
            ListView = table;
            RowConfigurator = settings;
            Data = data;
        }

        public TableAdapter(Context ctx,ListView table=null,Object[] datas=null,ITableAdapterRowConfigurator settings=null) : base(ctx)
        {
            td = new TableSource(null,TableRowType.Blurb);
            ListView = table;
            RowConfigurator = settings;
            Datas = datas;
        }

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

        public Object[] Datas
        {
            get
            {
                return td.Datas;             
            }
            set
            {
                td = new TableSource(value,TableRowType.Blurb);
                ReloadData();
            }
        }

        #region BaseAdapter

        protected override void UpdateHeader(int section,View view)
        {
            var header = GetTitleForHeader(section);
            if (view is ITableAdapterHeader)
            {
                var c = view as ITableAdapterHeader;
                c.Title = header;
            }
        }

        protected override void UpdateCell(int section,int row,View view)
        {
            if (view is ITableAdapterCell)
            {
                ITableAdapterCell c = view as ITableAdapterCell;
                UpdateView(c,section,row);
            }
        }

        protected override void UpdateFooter(int section,View view)
        {
            var footer = GetTitleForFooter(section);
            if (view is ITableAdapterFooter)
            {
                var c = view as ITableAdapterFooter;
                c.Title = footer;
            }
        }

        void ClickedItem(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (tv == null)
                return;
            var list = (e.Parent as ListView);
            int pos = e.Position;
            if (list!=null && list.HeaderViewsCount > 0)
                pos -= list.HeaderViewsCount;
            if (pos < 0) return;

            var vp = ViewPositionForPosition(pos);
            if (vp.Kind == ViewKind.Cell)
            {
                RowSelected(vp.Row, vp.Section);
            }
        }

        public virtual void RowSelected (int row,int section,AdapterView.ItemClickEventArgs ea = null)
        {
            var name = td.GetName(row,section);
            var value = td.GetValue(row,section);
            if (!td.Editable(RowConfigurator,row,section))
                return;
            var rowType = td.RowType(RowConfigurator,row,section);

            if (RowSelector != null)
            if (RowSelector.DidSelectRow(this,name))
                return;

            TableAdapterRowConfig settings = td.RowSetting(RowConfigurator,name,section);
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
                {
                    var obj = !(bool)value;
                    td.SetValue(obj, row, section);
                    ReloadData();
                    ChangedValue(name, value, obj);
                }
                break;

                case TableRowType.Blurb:
                case TableRowType.Text:
                {
                        if (settings != null && settings.InlineTextEditing)
                        {
                            // TODO: Figure out why this doesn't quite work
//                            if (ea != null && ea.View != null)
//                            {
//                                ITableAdapterCell c = ea.View as ITableAdapterCell;
//                                if (c != null)
//                                {
//                                    if (c.Edit.RequestFocus())
//                                    {
//                                        var ctx = tv.Context;
//                                        InputMethodManager inputManager = (InputMethodManager)ctx.GetSystemService(Context.InputMethodService);
//                                        inputManager.ShowSoftInput(c.Edit, 0);
//                                    }
//                                }
//                            }
                        }
                        else
                        {
                            var str = value as string;
                            var dname = td.DisplayName(RowConfigurator, row, section);
                            var act = tv.Context as Activity;
                    
                            var builder = new AlertDialog.Builder(act);
                            builder.SetTitle(dname);

                            var input = new EditText(act);                    
                            input.InputType = TableEditor.ConvertKeyboardType(KeyboardType.Default);
                            input.Gravity = GravityFlags.Center;

                            if (settings != null)
                            {
                                var inputTypes = TableEditor.ConvertKeyboardType(settings.KeyboardType);
                                if (settings.SecureTextEditing)
                                    inputTypes = Android.Text.InputTypes.TextVariationPassword | inputTypes;
                                input.InputType = inputTypes;
                                if (settings.SecureTextEditing)
                                    input.TransformationMethod = PasswordTransformationMethod.Instance;
                            }

                            input.Text = str;
                            input.SetSelection(str == null ? 0 : str.Length);

                            if (rowType == TableRowType.Blurb)
                                input.SetSingleLine(false);

                            var layout = new LinearLayout(act);
                            layout.Orientation = Orientation.Vertical;
                            LinearLayout.LayoutParams parameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.WrapContent);
                            parameters.SetMargins(20, 0, 20, 0);         
                            layout.AddView(input, parameters);

                            builder.SetView(layout);
                            builder.SetPositiveButton(PositiveButtonTitle, delegate(object o, DialogClickEventArgs e)
                            {
                                var s = input.Text;
                                td.SetValue(s, row, section);
                                ReloadData();
                                ChangedValue(name, value, s);
                            });
                            builder.SetNeutralButton(NeutralButtonTitle, delegate(object o, DialogClickEventArgs e)
                            {

                            });
                            builder.Show();
                        }
                }
                break;

                case TableRowType.Date:
                case TableRowType.Time:
                case TableRowType.DateTime:
                {
                    if (tv.Context is Activity)
                    {
                        var fr = tv.Context as Activity;
                        var frag = fr.FragmentManager;

                        DateTime v = (DateTime)value;

                        var newFragment = new TableTimeEditor(v, rowType, delegate(DateTime changedDate)
                        {
                            td.SetValue(changedDate, row, section);
                            ReloadData();
                            ChangedValue(name, value, changedDate);
                        });

                        newFragment.Show(frag, "datePicker");
                    }
                }
                break;

                case TableRowType.SingleChoiceList:
                {
                    var actSC = tv.Context as Activity;
                    var dnameSC = td.DisplayName(RowConfigurator, row, section);
                    var options = settings.SingleChoiceOptions;

                    var singleChoiceAdapter = new TableSingleChoiceEditor(actSC, settings, options, value);

                    int selectedItemIndex = -1;
                    if (options != null && value != null)
                    {
                        selectedItemIndex = options.IndexOf(value);
                    }

                    //ContextThemeWrapper wrapper = new ContextThemeWrapper(actSC, Android.Resource.Style.ThemeDialog);
                    //AlertDialog.Builder alert = new AlertDialog.Builder(wrapper);
                    var alert = new AlertDialog.Builder(actSC);
                    alert.SetTitle(dnameSC);
                    alert.SetSingleChoiceItems(singleChoiceAdapter, selectedItemIndex, delegate(object sender, DialogClickEventArgs e)
                    {
                        if (e != null)
                        {
                            var index = e.Which;
                            object theChoice = null;
                            if (options != null)
                                theChoice = options[index];
                            td.SetValue(theChoice, row, section);
                            ReloadData();
                            ChangedValue(name, value, theChoice);
                        }
                    });
                    alert.Show();
                }
                break;
            }
        }

        private void ChangedValue(string rowName,object oldValue,object newValue)
        {
            Log.Debug("change", string.Format("old '{0}' new '{1}'", oldValue, newValue));
            if (RowChanged != null)
                RowChanged.RowChanged(this, rowName, oldValue, newValue);
        }

        public virtual void UpdateView(ITableAdapterCell c,int section,int row)
        {            
            var name = td.GetName(row, section);
            var rowType = td.RowType(RowConfigurator,row,section);
            var value = td.GetValue(row, section);
            var editable = td.Editable(RowConfigurator,row,section);

            c.Title.Text = td.DisplayName(RowConfigurator,row,section);
            c.Editable = editable;

            switch (rowType)
            {
                case TableRowType.Text:
                case TableRowType.Blurb:
                    {
                        var s = td.RowSetting(RowConfigurator, name,section);
                        c.UpdateTextCell(rowType,value as string, s,delegate(string changed) 
                        {
                            var old = td.GetValue(row,section) as string;
                            td.SetValue(changed, row, section);
                            ChangedValue(name, old, changed);
                        });
                    }
                break;

                case TableRowType.SingleChoiceList:
                    {
                        c.Detail.Text = "";
                        string choiceString = null;
                        if (value != null)
                            choiceString = value.ToString();
                        c.Switch.Visibility = ViewStates.Gone;
                        c.Blurb.Visibility = ViewStates.Visible;
                        c.Blurb.Text = choiceString;
                    }
                break;

                case TableRowType.Checkbox:
                    {
                        c.Detail.Text = "";
                        c.Blurb.Text = "";
                        var s = td.RowSetting(RowConfigurator, name,section);
                        if (s != null && s.SimpleCheckbox)
                        {
                            c.Switch.Visibility = ViewStates.Gone;
                            c.Blurb.Visibility = ViewStates.Gone;
                            c.Detail.Text = (bool)value ? "\u2713" : "";
                        }
                        else
                        {
                            c.Switch.Visibility = ViewStates.Visible;
                            c.Blurb.Visibility = ViewStates.Gone;
                            c.Switch.Checked = (bool)value;
                        }
                    }
                break;
                case TableRowType.DateTime:
                case TableRowType.Date:
                case TableRowType.Time:
                    {
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
                    }
                break;
            }
        }

        public override int NumberOfSections
        {
            get
            {
                return td == null ? 1 : td.NumberOfSections();
            }
        }

        public override int NumberOfRowsForSection(int section)
        {
            return td==null?0:td.RowsInSection(section);
        }

        public virtual TableAdapterSectionConfig GetSectionConfig(int section)
        {
            if (td != null)
            {
                var config = td.SectionConfig(section);
                return config;
            }
            return null;
        }

        public override string GetTitleForHeader(int section)
        {
            var config = GetSectionConfig(section);
            return config == null ? null : config.Header;
        }

        public override string GetTitleForFooter(int section)
        {
            var config = GetSectionConfig(section);
            return config == null ? null : config.Footer;
        }

        #endregion       
    }
}
