using System;

using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Views;

namespace Tables.Droid
{
    public interface ITableAdapterCell
    {
        TextView Title { get; }
        TextView Detail { get; }
        TextView Blurb { get; }
        CheckBox Switch { get; }

        bool Editable {set;}
    }

    public class TableAdapterCell : LinearLayout,ITableAdapterCell
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
            int pixels = GetPixelsFromDPI(context, 10);
            SetPadding(pixels, pixels, pixels, pixels);
        }

        public bool Editable 
        {
            set
            {
                Focusable = !value;
            }
        }


        public static int GetPixelsFromDPI(Context ctx, int dpi)
        {
            int pixels = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip,dpi,ctx.Resources.DisplayMetrics);
            return pixels;
        }
    }
}