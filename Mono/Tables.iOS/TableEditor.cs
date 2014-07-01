using System;
using MonoTouch.UIKit;
using Tables;

namespace Tables.iOS
{
	public class TableEditor : UIViewController
    {
		public bool IsModal
		{
			get
			{
				if (NavigationController == null || NavigationController.ViewControllers.Length == 1)
					return true;
				return false;
			}
		}

		public void CloseViewController()
		{
			if (IsModal)
				DismissViewController (true, null);
			else
				NavigationController.PopViewControllerAnimated (true);
		}

		static public UIKeyboardType ConvertKeyboardType(Tables.KeyboardType kbType)
		{
			switch (kbType)
			{
				case Tables.KeyboardType.Default:
					return UIKeyboardType.Default;
				case Tables.KeyboardType.ASCIICapable:
					return UIKeyboardType.ASCIICapable;
				case Tables.KeyboardType.NumbersAndPunctuation:
					return UIKeyboardType.NumbersAndPunctuation;
				case Tables.KeyboardType.Url:
					return UIKeyboardType.Url;
				case Tables.KeyboardType.NumberPad:
					return UIKeyboardType.NumberPad;
				case Tables.KeyboardType.PhonePad:
					return UIKeyboardType.PhonePad;
				case Tables.KeyboardType.NamePhonePad:
					return UIKeyboardType.NamePhonePad;
				case Tables.KeyboardType.EmailAddress:
					return UIKeyboardType.EmailAddress;
				case Tables.KeyboardType.DecimalPad:
					return UIKeyboardType.DecimalPad;
			}
			return UIKeyboardType.Default;
		}

		static public UITextAutocapitalizationType ConvertCapitatilizationType(Tables.CapitalizationType capType)
		{
			switch (capType)
			{
				case Tables.CapitalizationType.None:
					return UITextAutocapitalizationType.None;
				case Tables.CapitalizationType.Words:
					return UITextAutocapitalizationType.Words;
				case Tables.CapitalizationType.Sentences:
					return UITextAutocapitalizationType.Sentences;
				case Tables.CapitalizationType.AllCharacters:
					return UITextAutocapitalizationType.AllCharacters;
			}
			return UITextAutocapitalizationType.None;
		}

		static public UITextAutocorrectionType ConvertCorrectionType(Tables.CorrectionType correctionType)
		{
			switch (correctionType)
			{
				case Tables.CorrectionType.Default:
					return UITextAutocorrectionType.Default;
				case Tables.CorrectionType.No:
					return UITextAutocorrectionType.No;
				case Tables.CorrectionType.Yes:
					return UITextAutocorrectionType.Yes;
			}
			return UITextAutocorrectionType.Default;
		}
    }
}