using System;

using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Text;
using Android.Support.V7.Widget;

namespace Tables.Droid
{
    public interface ITableAdapterSimpleCell
    {
        TextView Title { get; }
        TextView Detail { get; }
        bool Editable {set;}
    }

    public interface ITableAdapterCell : ITableAdapterSimpleCell
    {
        TextView Blurb { get; }
        CheckBox Switch { get; }
        EditText Edit { get; }
        void UpdateTextCell(TableRowType type,string value,TableAdapterRowConfig s,TextChangedDelegate textChanged);
    }

    public interface ITableAdapterHeaderFooter
    {
        string Title { set; get; }
    }

    public interface ITableAdapterSectionCell
    {
        string Title { set; get; }
        string Detail { set; get; }    
    }

    public interface ITableSectionsCell
    {
        void Update(TableSection tsection,TableItem titem,int section, int row);
    }

    public interface ITableCellHolder
    {
        void Bind(View view);
    }

    public interface ITableAdapterHeader : ITableAdapterHeaderFooter
    {
        
    }

    public interface ITableAdapterFooter : ITableAdapterHeaderFooter
    {
        
    }

    public interface ITableAdapterSingleChoiceCell : ICheckable
    {
        string Text { get; set; }
    }

    public delegate void TextChangedDelegate(string changed);

    public static class TableUtils
    {
        public static int DefaultHeaderStyle = Android.Resource.Style.TextAppearanceDeviceDefaultMedium;
        public static int DefaultFooterStyle = Android.Resource.Style.TextAppearanceDeviceDefaultSmall;
        public static int DefaultTitleStyle = Android.Resource.Style.TextAppearanceDeviceDefaultMedium;
        public static int DefaultDetailStyle = Android.Resource.Style.TextAppearanceDeviceDefaultSmall;

        public static int GetPixelsFromDPI(Context ctx, int dpi)
        {
            int pixels = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip,dpi,ctx.Resources.DisplayMetrics);
            return pixels;
        }

        public static EditText CreateEditableTextField(Context ctx)
        {
            var et = new EditText(ctx);
            et.SetBackgroundColor(Android.Graphics.Color.Transparent);
            et.SetTextColor(Android.Graphics.Color.White);
            et.LayoutParameters = new ViewGroup.LayoutParams(Android.Widget.LinearLayout.LayoutParams.MatchParent, Android.Widget.LinearLayout.LayoutParams.WrapContent);
            et.SetMinimumHeight(TableUtils.GetPixelsFromDPI(ctx,40));
            return et;
        }
        
        public static void SetTextAppearance(TextView tv,int style)
        {
        	tv.SetTextAppearance(tv.Context,style);
        }
    }

    public class TableStyle
    {
        public int DefaultHeaderLayoutStyle = 0;
        public int DefaultCellLayoutStyle = 0;
        public int DefaultFooterLayoutStyle = 0;
        public int DefaultSectionSeparatorStyle = 0;
        public int DefaultHeaderStyle = TableUtils.DefaultHeaderStyle;
        public int DefaultFooterStyle = TableUtils.DefaultFooterStyle;
        public int DefaultTitleStyle = TableUtils.DefaultTitleStyle;
        public int DefaultDetailStyle = TableUtils.DefaultDetailStyle;
    }

    public class TableAdapterCell : LinearLayout,ITableAdapterCell,ICheckable,ITextWatcher
    {
        public TextView Title { get; private set;}
        public TextView Detail { get; private set;}
        public TextView Blurb { get; private set;}
        public CheckBox Switch { get; private set;}
        public EditText Edit { get; private set;}

        private int titleStyle = TableUtils.DefaultTitleStyle;
        private int detailStyle = TableUtils.DefaultDetailStyle;

        public TableAdapterCell(Context context) : base(context)
        {
            Prepare(context);
        }

        public TableAdapterCell (Context context, IAttributeSet attrs) : base (context, attrs)
        {
            Prepare (context);
        }

        public TableAdapterCell (Context context, IAttributeSet attrs, int defStyle, int aTitleStyle, int aDetailStyle) : base (context, attrs,defStyle)
        {
            titleStyle = aTitleStyle;
            detailStyle = aDetailStyle;
            Prepare (context);
        }

        private void Prepare(Context context)
        {
            LinearLayout line = new LinearLayout(context);
            {
                line.SetVerticalGravity(GravityFlags.Center);
                line.Orientation = Orientation.Horizontal;

                Title= new TextView(Context);
                Title.Text = "Title";                
                TableUtils.SetTextAppearance(Title, titleStyle);
                line.AddView(Title);

                var filler = new View(context);
                filler.LayoutParameters = new LinearLayout.LayoutParams(Android.Widget.LinearLayout.LayoutParams.WrapContent, Android.Widget.LinearLayout.LayoutParams.WrapContent, 1f);

                line.AddView(filler);

                Detail = new TextView(context);
                Detail.Text = "Detail";
                Detail.LayoutParameters = new ViewGroup.LayoutParams(Android.Widget.LinearLayout.LayoutParams.MatchParent, Android.Widget.LinearLayout.LayoutParams.WrapContent);
                Detail.SetMinimumHeight(TableUtils.GetPixelsFromDPI(context,40));
                TableUtils.SetTextAppearance(Detail, detailStyle);
                line.AddView(Detail);

                Switch = new CheckBox(context);
                Switch.Clickable = false;
                Switch.Focusable = false;
                line.AddView(Switch);
            }
            AddView(line);

            Blurb = new TextView(context);
            Blurb.Text = "Blurb";
            TableUtils.SetTextAppearance(Blurb,detailStyle);
            AddView(Blurb);

            Edit = TableUtils.CreateEditableTextField(context);
            Edit.Text = "Edit";
            Edit.Visibility = ViewStates.Gone;
            TableUtils.SetTextAppearance(Edit, detailStyle);
            Edit.AddTextChangedListener(this);
            AddView(Edit);

            Orientation = Orientation.Vertical;
            int pixels = TableUtils.GetPixelsFromDPI(context, 10);
            SetPadding(pixels, pixels, pixels, pixels);
        }

        public void Toggle()
        {
            Switch.Checked = !Switch.Checked;
        }

        public bool Checked
        {
            get
            {
                return Switch.Checked;
            }
            set
            {
                Switch.Checked = value;
            }
        }

        public bool Editable 
        {
            set
            {
                Focusable = !value;
            }
        }

        TextChangedDelegate textListener;

        public void UpdateTextCell(TableRowType type,string value,TableAdapterRowConfig s,TextChangedDelegate textChanged)
        {
            textListener = null;
            TextView tv = null;
            if (type==TableRowType.Text)
            {
                Blurb.Text = "";
                tv = Detail;
                Switch.Visibility = ViewStates.Gone;
                Blurb.Visibility = ViewStates.Gone;
            }
            else if (type==TableRowType.Blurb)
            {
                Detail.Text = "";
                Switch.Visibility = ViewStates.Gone;
                Blurb.Visibility = ViewStates.Visible;
                Detail.Visibility = ViewStates.Gone;
                tv = Blurb;
            }

            if (tv != null)
            {
                var text = s != null && s.SecureTextEditing ? TextHelper.ScrambledText(value) : value;
                var inline = s != null && s.InlineTextEditing ? true : false;
                tv.Visibility = inline ? ViewStates.Gone : ViewStates.Visible;
                Edit.Visibility = !inline ? ViewStates.Gone : ViewStates.Visible;
                if (inline)
                {
                    Edit.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;
                    Edit.Text = value;
                    textListener = textChanged;
                }
                else
                {
                    tv.Text = text;
                }
            }
        }

        public void AfterTextChanged(IEditable s)
        {
            
        }

        public void BeforeTextChanged(Java.Lang.ICharSequence s, int start, int count, int after)
        {
            
        }

        public void OnTextChanged(Java.Lang.ICharSequence s, int start, int before, int count)
        {
            var sh = s.ToString();
            if (textListener != null)
                textListener(sh);
        }
    }

    public class TableAdapterSingleChoiceCell : LinearLayout,ITableAdapterSingleChoiceCell
    {
        public TextView Title { get; private set;}
        public CheckBox Switch { get; private set;}

        public TableAdapterSingleChoiceCell(Context context) : base(context)
        {
            Prepare(context);
        }

        public TableAdapterSingleChoiceCell (Context context, IAttributeSet attrs) : base (context, attrs)
        {
            Prepare (context);
        }

        public TableAdapterSingleChoiceCell (Context context, IAttributeSet attrs,int defStyle) : base (context, attrs, defStyle)
        {
            Prepare (context);
        }

        private void Prepare(Context context)
        {
            LinearLayout line = new LinearLayout(context);
            {
                line.SetVerticalGravity(GravityFlags.Center);
                line.Orientation = Orientation.Horizontal;

                Title= new TextView(Context);
                Title.Text = "Title";
                Title.Typeface = Android.Graphics.Typeface.DefaultBold;
                line.AddView(Title);

                View filler = new View(context);
                filler.LayoutParameters = new LinearLayout.LayoutParams(Android.Widget.LinearLayout.LayoutParams.WrapContent, Android.Widget.LinearLayout.LayoutParams.WrapContent, 1f);

                line.AddView(filler);

                Switch = new CheckBox(context);
                Switch.Clickable = false;
                Switch.Focusable = false;
                line.AddView(Switch);
            }
            AddView(line);

            Orientation = Orientation.Vertical;
            int pixels = TableUtils.GetPixelsFromDPI(context, 10);
            SetPadding(pixels, pixels, pixels, pixels);
        }

        public void Toggle()
        {
            Switch.Checked = !Switch.Checked;
        }

        public bool Checked
        {
            get
            {
                return Switch.Checked;
            }
            set
            {
                Switch.Checked = value;
            }
        }

        public string Text 
        { 
            get
            {
                return Title.Text;
            }
            set
            {
                Title.Text = value;
            }
        }

        public bool Editable 
        {
            set
            {
                Focusable = !value;
            }
        }
    }

    public class TableAdapterSimpleCell : LinearLayout,ITableAdapterSimpleCell
    {
        public TextView Title { get; private set;}
        public TextView Detail { get; private set;}

        private int titleStyle = TableUtils.DefaultTitleStyle;
        private int detailStyle = TableUtils.DefaultDetailStyle;

        public TableAdapterSimpleCell(Context context) : base(context)
        {
            Prepare(context);
        }

        public TableAdapterSimpleCell (Context context, IAttributeSet attrs) : base (context, attrs)
        {
            Prepare (context);
        }

        public TableAdapterSimpleCell (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
        {
            Prepare (context);
        }

        public TableAdapterSimpleCell (Context context, IAttributeSet attrs, int defStyle, int aTitleStyle, int aDetailStyle) : base (context, attrs, defStyle)
        {
            titleStyle = aTitleStyle;
            detailStyle = aDetailStyle;
            Prepare (context);
        }

        private void Prepare(Context context)
        {
            Title = new TextView(context);
            int pixels = TableUtils.GetPixelsFromDPI(context, 2);
            Title.SetPadding(pixels, pixels, pixels, pixels);
            Title.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent, 1f);
            TableUtils.SetTextAppearance(Title, titleStyle);
            Title.Text = "Title";
            AddView(Title);

            Detail = new TextView(context);
            Detail.Text = "Detail";
            pixels = TableUtils.GetPixelsFromDPI(context, 2);
            Detail.SetPadding(pixels, pixels, pixels, pixels);
            Detail.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent, 1f);
            TableUtils.SetTextAppearance(Detail, detailStyle);
            AddView(Detail);

            Orientation = Orientation.Vertical;
            pixels = TableUtils.GetPixelsFromDPI(context, 10);
            SetPadding(pixels, pixels, pixels, pixels);
        }

        public bool Editable 
        {
            set
            {
                Focusable = !value;
            }
        }

        public void SetTitleStyle(Context ctx,int style)
        {
            titleStyle = style;
            if (Title != null)
                TableUtils.SetTextAppearance(Title, titleStyle);
        }

        public void SetDetailStyle(Context ctx,int style)
        {
            detailStyle = style;
            if (Detail != null)
                TableUtils.SetTextAppearance(Detail, detailStyle);
        }
    }

    public class TableAdapterSectionCell : LinearLayout,ITableAdapterSectionCell
    {
        public TextView TitleLabel { get; private set;}
        public TextView DetailLabel { get; private set;}

        private int titleStyle = TableUtils.DefaultTitleStyle;
        private int detailStyle = TableUtils.DefaultDetailStyle;

        public TableAdapterSectionCell(Context context) : base(context)
        {
            Prepare(context);
        }

        public TableAdapterSectionCell (Context context, IAttributeSet attrs) : base (context, attrs)
        {
            Prepare (context);
        }

        public TableAdapterSectionCell (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
        {
            Prepare (context);
        }

        public TableAdapterSectionCell (Context context, IAttributeSet attrs, int defStyle, int aTitleStyle, int aDetailStyle) : base (context, attrs, defStyle)
        {
            titleStyle = aTitleStyle;
            detailStyle = aDetailStyle;
            Prepare (context);
        }

        private void Prepare(Context context)
        {
            TitleLabel = new TextView(context);
            int pixels = TableUtils.GetPixelsFromDPI(context, 2);
            TitleLabel.SetPadding(pixels, pixels, pixels, pixels);
            TitleLabel.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent, 1f);
            TableUtils.SetTextAppearance(TitleLabel, titleStyle);
            TitleLabel.Text = "Title";
            AddView(TitleLabel);

            DetailLabel = new TextView(context);
            DetailLabel.Text = "Detail";
            pixels = TableUtils.GetPixelsFromDPI(context, 2);
            DetailLabel.SetPadding(pixels, pixels, pixels, pixels);
            DetailLabel.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent, 1f);
            TableUtils.SetTextAppearance(DetailLabel, detailStyle);
            AddView(DetailLabel);

            Orientation = Orientation.Vertical;
            pixels = TableUtils.GetPixelsFromDPI(context, 10);
            SetPadding(pixels, pixels, pixels, pixels);
        }

        public bool Editable 
        {
            set
            {
                Focusable = !value;
            }
        }

        public virtual string Title
        {
            get
            {
                return TitleLabel.Text;
            }
            set
            {                
                TitleLabel.Text = value;
                TitleLabel.Visibility = value == null || value.Length == 0 ? ViewStates.Gone : ViewStates.Visible;
            }
        }

        public virtual string Detail
        {
            get
            {
                return DetailLabel.Text;
            }
            set
            {                
                DetailLabel.Text = value;
                DetailLabel.Visibility = value == null || value.Length == 0 ? ViewStates.Gone : ViewStates.Visible;
            }
        }
    }

    public class TableAdapterHF : LinearLayout
    {
        public TextView TitleLabel { get; protected set;}
        public View Separator  { get; protected set;}

        public TableAdapterHF(Context context) : base(context) 
        {
            Construct(context);
        }

        public TableAdapterHF (Context context, IAttributeSet attrs) : base (context, attrs)
        {
            Construct(context);
        }

        public TableAdapterHF (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle) 
        {
            Construct(context);
            ApplyLayoutStyle(context,defStyle);
        }

        private void Construct(Context context)
        {
            Orientation = Orientation.Vertical;

            Separator = new View(context);
            Separator.LayoutParameters = new ViewGroup.LayoutParams(Android.Widget.LinearLayout.LayoutParams.MatchParent, 1);
            Separator.SetBackgroundColor(Android.Graphics.Color.DimGray);
            var isFooter = IsFooter;
            if (!isFooter)
                AddView(Separator);

            TitleLabel = new TextView(context);
            TitleLabel.Text = "Title";
            TableUtils.SetTextAppearance(TitleLabel,textStyle);
            TitleLabel.Gravity = GravityFlags.Left;
            TitleLabel.LayoutParameters = new ViewGroup.LayoutParams(Android.Widget.LinearLayout.LayoutParams.MatchParent, Android.Widget.LinearLayout.LayoutParams.WrapContent);
            AddView(TitleLabel);
            if (isFooter)
                AddView(Separator);

            Focusable = false;
            Clickable = false;
            FocusableInTouchMode = false;

            int pixels = TableUtils.GetPixelsFromDPI(context, 10);
            TitleLabel.SetPadding(pixels, pixels, pixels, pixels);
        }

        protected virtual bool IsFooter
        {
            get
            {
                return false;
            }
        }

        public int TextStyle
        {
            get
            {
                return textStyle;
            }
            set
            {
                textStyle = value;
                if (TitleLabel != null)
                    TableUtils.SetTextAppearance(TitleLabel, TextStyle);
            }
        }
        protected int textStyle;

        public virtual string Title
        {
            get
            {
                return TitleLabel.Text;
            }
            set
            {                
                TitleLabel.Text = value;
                TitleLabel.Visibility = value == null || value.Length == 0 ? ViewStates.Gone : ViewStates.Visible;
            }
        }

        void ApplyLayoutStyle(Context context,int defStyle)
        {
            if (defStyle == 0) return;

            int[] attrs = {Android.Resource.Attribute.Background,Android.Resource.Attribute.MinHeight};

            var a = context.ObtainStyledAttributes(defStyle, attrs);
            var bg = a.GetResourceId(0, 0);
            if (bg != 0)
                SetBackgroundResource(bg);
            var mh = a.GetDimensionPixelSize(1, 0);
            if (mh != 0)
                SetMinimumHeight(mh);
            a.Recycle();
        }
    }

    public class TableAdapterHeader : TableAdapterHF,ITableAdapterHeader
    {
        private int kDefaultTextStyle = TableUtils.DefaultHeaderStyle;

        public TableAdapterHeader(Context context) : base(context)
        {
            TextStyle = kDefaultTextStyle;
            Prepare(context);
        }

        public TableAdapterHeader (Context context, IAttributeSet attrs) : base (context, attrs)
        {
            TextStyle = kDefaultTextStyle;
            Prepare (context);
        }

        public TableAdapterHeader (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
        {
            TextStyle = kDefaultTextStyle;
            Prepare (context);
        }

        public TableAdapterHeader (Context context, IAttributeSet attrs, int defStyle, int textStyle) : base (context, attrs, defStyle)
        {
            TextStyle = textStyle;
            Prepare (context);
        }

        private void Prepare(Context context)
        {

        }
    }

    public class TableAdapterFooter : TableAdapterHF,ITableAdapterFooter
    {
        private int kDefaultTextStyle = TableUtils.DefaultFooterStyle;

        public TableAdapterFooter(Context context) : base(context)
        {
            TextStyle = kDefaultTextStyle;
            Prepare(context);
        }

        public TableAdapterFooter (Context context, IAttributeSet attrs) : base (context, attrs)
        {
            TextStyle = kDefaultTextStyle;
            Prepare (context);
        }

        public TableAdapterFooter (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
        {
            TextStyle = kDefaultTextStyle;
            Prepare (context);
        }

        public TableAdapterFooter (Context context, IAttributeSet attrs, int defStyle, int textStyle) : base (context, attrs, defStyle)
        {
            TextStyle = textStyle;
            Prepare (context);
        }

        private void Prepare(Context context)
        {
            TitleLabel.Gravity = GravityFlags.Center;
        }

        protected override bool IsFooter
        {
            get
            {
                return true;
            }
        }
    }

    public class TableAdapterSectionSeperator : View
    {
        public TableAdapterSectionSeperator(Context context) : base(context)
        {            
            Prepare(context);
        }

        public TableAdapterSectionSeperator (Context context, IAttributeSet attrs) : base (context, attrs)
        {            
            Prepare (context);
        }

        public TableAdapterSectionSeperator (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
        {            
            Prepare (context);
            ApplyStyle(context,defStyle);
        }

        private void Prepare(Context context)
        {            
            Focusable = false;
            Clickable = false;
            FocusableInTouchMode = false;
        }

        void ApplyStyle(Context context,int defStyle)
        {
            if (defStyle == 0) return;

            int[] attrs = {Android.Resource.Attribute.Background,Android.Resource.Attribute.MinHeight};

            var a = context.ObtainStyledAttributes(defStyle, attrs);

            var bg = a.GetResourceId(0, 0);
            if (bg != 0) SetBackgroundResource(bg);

            var mh = a.GetDimensionPixelSize(1, 0);
            if (mh != 0) SetMinimumHeight(mh);

            a.Recycle();
        }
    }
}
