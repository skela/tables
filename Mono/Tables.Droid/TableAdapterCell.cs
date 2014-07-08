using System;

using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Views;

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
    }

    public interface ITableAdapterSingleChoiceCell : ICheckable
    {
        string Text { get; set; }
    }

    public static class TableUtils
    {
        public static int GetPixelsFromDPI(Context ctx, int dpi)
        {
            int pixels = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip,dpi,ctx.Resources.DisplayMetrics);
            return pixels;
        }
    }

    public class TableAdapterCell : LinearLayout,ITableAdapterCell,ICheckable
    {
        public TextView Title { get; private set;}
        public TextView Detail { get; private set;}
        public TextView Blurb { get; private set;}
        public CheckBox Switch { get; private set;}

        public TableAdapterCell(Context context) : base(context)
        {
            Prepare(context);
        }

        public TableAdapterCell (Context context, IAttributeSet attrs) : base (context, attrs)
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

                Detail = new TextView(context);
                Detail.Text = "Detail";
                line.AddView(Detail);

                Switch = new CheckBox(context);
                Switch.Clickable = false;
                Switch.Focusable = false;
                line.AddView(Switch);
            }
            AddView(line);

            Blurb = new TextView(context);
            Blurb.Text = "Blurb";
            AddView(Blurb);

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

        public TableAdapterSimpleCell(Context context) : base(context)
        {
            Prepare(context);
        }

        public TableAdapterSimpleCell (Context context, IAttributeSet attrs) : base (context, attrs)
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
            }
            AddView(line);

            Detail = new TextView(context);
            Detail.Text = "Detail";
            AddView(Detail);

            Orientation = Orientation.Vertical;
            int pixels = TableUtils.GetPixelsFromDPI(context, 10);
            SetPadding(pixels, pixels, pixels, pixels);
        }

        public bool Editable 
        {
            set
            {
                Focusable = !value;
            }
        }
    }
}