using System;

using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using Android.Text;

namespace Tables.Droid
{
    public static class TableEditor
    {
        public static void CloseKeyboard(Context context,View view)
        {
            InputMethodManager inputManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
            inputManager.HideSoftInputFromWindow(view.WindowToken,0);
        }

        public static void OpenKeyboard(Context context,View view)
        {
            InputMethodManager inputManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
            inputManager.ShowSoftInputFromInputMethod (view.WindowToken,Android.Views.InputMethods.ShowFlags.Forced); //show forced
            view.RequestFocus ();
        }

        static public InputTypes ConvertKeyboardType(Tables.KeyboardType kbType)
        {
            switch (kbType)
            {
                case Tables.KeyboardType.Default:
                    return InputTypes.ClassText;
                case Tables.KeyboardType.ASCIICapable:
                    return InputTypes.ClassText;
                case Tables.KeyboardType.NumbersAndPunctuation:
                    return InputTypes.TextVariationVisiblePassword | InputTypes.ClassText;
                case Tables.KeyboardType.NumberPad:
                    return InputTypes.ClassNumber;
                case Tables.KeyboardType.PhonePad:
                    return InputTypes.ClassPhone;
                case Tables.KeyboardType.NamePhonePad:
                    return InputTypes.TextVariationPersonName | InputTypes.ClassText;
                case Tables.KeyboardType.EmailAddress:
                    return InputTypes.TextVariationEmailAddress | InputTypes.ClassText;
                case Tables.KeyboardType.Url:
                    return InputTypes.TextVariationUri | InputTypes.ClassText;
                case Tables.KeyboardType.DecimalPad:
                    return InputTypes.NumberFlagDecimal | InputTypes.ClassNumber;
            }
            return InputTypes.ClassText;
        }
    }
}
