using System;
using UIKit;
using Tables;
using CoreGraphics;
using Foundation;
using System.Collections.Generic;

namespace Tables.iOS
{
	public class TableEditor : UIViewController
    {
		public TableEditor() : base()
		{
			HidesBottomBarWhenPushed = true;
		}

		public override bool ExtendedLayoutIncludesOpaqueBars
		{
			get
			{
				return true;
			}
		}

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
				NavigationController.PopViewController (true);
		}
		
		static public UIReturnKeyType ConvertReturnKeyType(Tables.ReturnKeyType type)
		{
			switch (type)
			{
				case ReturnKeyType.Default: return UIReturnKeyType.Default;
				case ReturnKeyType.Go: return UIReturnKeyType.Go;
				case ReturnKeyType.Google: return UIReturnKeyType.Google;
				case ReturnKeyType.Join: return UIReturnKeyType.Join;
				case ReturnKeyType.Next: return UIReturnKeyType.Next;
				case ReturnKeyType.Route: return UIReturnKeyType.Route;
				case ReturnKeyType.Search: return UIReturnKeyType.Search;
				case ReturnKeyType.Send: return UIReturnKeyType.Send;
				case ReturnKeyType.Yahoo: return UIReturnKeyType.Yahoo;
				case ReturnKeyType.Done: return UIReturnKeyType.Done;
				case ReturnKeyType.EmergencyCall: return UIReturnKeyType.EmergencyCall;
				case ReturnKeyType.Continue: return UIReturnKeyType.Continue;
			}
			return UIReturnKeyType.Default;
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

		static public void ConfigureTextControl (TableAdapterRowConfig config,IUITextInputTraits control)
		{
			if (config!=null && control!=null)
			{
				if (config.KeyboardType != Tables.KeyboardType.Ignore)
					control.KeyboardType = TableEditor.ConvertKeyboardType(config.KeyboardType);
				if (config.CapitalizationType != Tables.CapitalizationType.Ignore)
					control.AutocapitalizationType = TableEditor.ConvertCapitatilizationType(config.CapitalizationType);
				if (config.CorrectionType != Tables.CorrectionType.Ignore)
					control.AutocorrectionType = TableEditor.ConvertCorrectionType(config.CorrectionType);
			}
		}

		public static float OperatingSystemVersion
		{
			get
			{
				string ver = UIDevice.CurrentDevice.SystemVersion;
				float verF = 4.0f;
				if (float.TryParse (ver, out verF))
				{
					return verF;
				}

				var ls = ver.Split ('.');
				List<float> lf = new List<float> ();
				foreach (string s in ls)
					lf.Add (float.Parse (s));

				if (lf.Count > 2)
				{
					verF = lf [0] + lf [1] * 0.1f + lf [2] * 0.01f;
				}
				else if (lf.Count > 1)
				{
					verF = lf [0] + lf [1] * 0.1f;
				}
				else if (lf.Count > 0)
				{
					verF = lf [0];
				}
				return verF;
			}
		}

		public static nfloat Max(nfloat val,float val2)
		{
			var res = Math.Max ((double)val,(double)val2);
			return (nfloat)res;
		}
    }

	public class TableAdapterInlineTextInputAccessoryView : UIView
	{
		public UIButton NextButton;
		public UIButton PreviousButton;
		public UIButton DismissButton;
		public NSIndexPath IndexPath;

		public TableAdapterInlineTextInputAccessoryView (TableAdapterRowConfig config,float width) : base(new CGRect(0,0,width,40))
		{
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

			BackgroundColor = UIColor.FromRGB (209, 213, 218);
			var textColor = UIColor.Black;

			NextButton = new UIButton (new CGRect (0, 0, 40, 40));
			PreviousButton = new UIButton (new CGRect (0, 0, 40, 40));
			DismissButton = new UIButton (new CGRect (0, 0, 40, 40));

			PreviousButton.SetTitle ("\u25C4", UIControlState.Normal);
			NextButton.SetTitle ("\u25BA", UIControlState.Normal);
			DismissButton.SetTitle ("\u2637", UIControlState.Normal);

			NextButton.SetTitleColor (textColor, UIControlState.Normal);
			PreviousButton.SetTitleColor (textColor, UIControlState.Normal);
			DismissButton.SetTitleColor (textColor, UIControlState.Normal);

			AddSubview (NextButton);
			AddSubview (PreviousButton);
			AddSubview (DismissButton);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			PreviousButton.Frame = new CGRect (10, 0, 40, 40);
			NextButton.Frame = new CGRect (PreviousButton.Frame.Width+20, 0, 40, 40);
			DismissButton.Frame = new CGRect (Frame.Width-10-40, 0, 40, 40);
		}
	}
}
