using System;
using UIKit;
using CoreGraphics;
using Foundation;

namespace Tables.iOS
{
    public delegate void TextChangedDelegate(string changedString);

	public class TableTextEditor : TableEditor
    {
        private string value;
		private UITextView textView;
		private UITextField textField;
        private TextChangedDelegate textChanged;
		private TableRowType rowType;
		private UITextAutocapitalizationType capitalizationType=UITextAutocapitalizationType.Sentences;
		private UITextAutocorrectionType correctionType;
		private UIKeyboardType keyboardType;
		public bool ShouldAdjustTextContentInset;
        private bool secureTextEntry=false;
		
		public TableTextEditor(TableRowType rowType,string title,string value,TextChangedDelegate delg)
        {
            this.Title = title;
            this.value = value;
            this.textChanged = delg;
			this.rowType = rowType;
        }

		public void Configure (TableAdapterRowConfig config)
		{
			if (config!=null)
			{
				if (config.KeyboardType != Tables.KeyboardType.Ignore)
					KeyboardType = TableEditor.ConvertKeyboardType(config.KeyboardType);
				if (config.CapitalizationType != Tables.CapitalizationType.Ignore)
					CapitalizationType = TableEditor.ConvertCapitatilizationType(config.CapitalizationType);
				if (config.CorrectionType != Tables.CorrectionType.Ignore)
					CorrectionType = TableEditor.ConvertCorrectionType(config.CorrectionType);
                SecureTextEntry = config.SecureTextEditing;
			}
		}

		IUITextInputTraits text
		{
			get
			{
				if (textView != null)
					return textView;
				if (textField != null)
					return textField;
				return null;
			}
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (NavigationItem != null)
            {
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, ClickedCancel);
                NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, ClickedDone);
            }

			if (rowType == TableRowType.Blurb)
			{
				View.BackgroundColor = BackgroundColor;
				textView = new UITextView (View.Bounds);
				textView.TextColor = TextColor;
				textView.BackgroundColor = BackgroundColor;
				textView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
                textView.SecureTextEntry = SecureTextEntry;
				textView.Text = value;
				textView.AutocapitalizationType = capitalizationType;
				textView.AutocorrectionType = correctionType;
				textView.KeyboardType = keyboardType;
				textView.Font = UIFont.SystemFontOfSize (14);
				View.AddSubview (textView);
			}
			else if (rowType == TableRowType.Text)
			{
				View.BackgroundColor = BackgroundColor;
				textField = new UITextField (new CGRect (10, 10, View.Bounds.Size.Width - 20, 44));
				textField.TextColor = TextColor;
				textField.BackgroundColor = BackgroundColor;
				textField.BorderStyle = UITextBorderStyle.Line;
				textField.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
				textField.Text = value;
                textField.SecureTextEntry = SecureTextEntry;
				textField.AutocapitalizationType = capitalizationType;
				textField.AutocorrectionType = correctionType;
				textField.KeyboardType = keyboardType;
				textField.ShouldReturn = ClickedReturn;
				View.AddSubview (textField);
			}
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

			if (textView != null && ShouldAdjustTextContentInset)
            {
                ListenToKeyboardNotifications(true);
            }

			if (textView != null)
				textView.BecomeFirstResponder ();
			else if (textField != null)
				textField.BecomeFirstResponder ();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            if (textView != null)
            {
                ListenToKeyboardNotifications(false);
            }
        }

		bool ClickedReturn (UITextField textField)
		{
			ClickedDone (null, null);
			return true;
		}

        private void ClickedCancel(object obj,EventArgs e)
        {
			CloseViewController ();
        }

        private void ClickedDone(object obj,EventArgs e)
        {
			if (textChanged != null)
			{
				if (textView!=null)
					textChanged (textView.Text);            
				else if (textField!=null)
					textChanged (textField.Text);            
			}
			CloseViewController ();
        }

		public UITextAutocapitalizationType CapitalizationType
		{
			get
			{
				return capitalizationType;
			}
			set
			{
				capitalizationType = value;
				if (text != null)
					text.AutocapitalizationType = capitalizationType;
			}
		}

		public UITextAutocorrectionType CorrectionType
		{
			get
			{
				return correctionType;
			}
			set
			{
				correctionType = value;
				if (text != null)
					text.AutocorrectionType = correctionType;
			}
		}

        public bool SecureTextEntry
        {
            get
            {
                return secureTextEntry;
            }
            set
            {
                secureTextEntry = value;
                if (text != null)
                    text.SecureTextEntry = secureTextEntry;
            }
        }

		public UIKeyboardType KeyboardType
		{
			get
			{
				return keyboardType;
			}
			set
			{
				keyboardType = value;
				if (text != null)
					text.KeyboardType = keyboardType;
			}
		}

        #region Keyboard Offset

        public override UIRectEdge EdgesForExtendedLayout
        {
            get
            {
                return UIRectEdge.None;
            }
        }

        public override bool ExtendedLayoutIncludesOpaqueBars
        {
            get
            {
                return true;
            }
        }

        public override bool AutomaticallyAdjustsScrollViewInsets
        {
            get
            {
                return false;
            }
        }

        NSObject hideObserver;
        NSObject didHideObserver;
        NSObject showObserver;
        NSObject willShowObserver;

        public void ListenToKeyboardNotifications(bool shouldListen)
        {
            var c = NSNotificationCenter.DefaultCenter;
            if (shouldListen)
            {
                hideObserver = c.AddObserver (UIKeyboard.WillHideNotification, HandleKeyboardWillHide);
                didHideObserver = c.AddObserver (UIKeyboard.DidHideNotification, HandleKeyboardDidHide);
                showObserver = c.AddObserver (UIKeyboard.DidShowNotification, HandleKeyboardDidShow);           
                willShowObserver = c.AddObserver (UIKeyboard.WillShowNotification, HandleKeyboardWillShow);         
            }
            else
            {
                if (hideObserver!=null) c.RemoveObserver (hideObserver); hideObserver = null;
                if (didHideObserver!=null) c.RemoveObserver (didHideObserver); didHideObserver = null;
                if (showObserver!=null) c.RemoveObserver (showObserver); showObserver = null;
                if (willShowObserver!=null) c.RemoveObserver (willShowObserver); willShowObserver = null;
            }
        }

        NSNotification lastNotification;
        public float KeyboardHeight
        {
            get
            {
				return (float)Math.Min (KeyboardSize.Height, KeyboardSize.Width);
            }
        }
        public CGSize KeyboardSize;

        void HandleKeyboardWillShow(NSNotification notification)
        {
            lastNotification = notification;


            lastNotification = null;
        }

        void HandleKeyboardDidShow(NSNotification notification)
        {       
            lastNotification = notification;

            AdjustTextView();

            lastNotification = null;
        }

        void HandleKeyboardWillHide(NSNotification notification)
        {
            lastNotification = notification;

            ResetTextView();

            lastNotification = null;
        }

        void HandleKeyboardDidHide(NSNotification notification)
        {
            lastNotification = notification;


            lastNotification = null;
        }

        public void AdjustTextView()
        {
            if (lastNotification != null)
            {
                var args = new UIKeyboardEventArgs (lastNotification);
                var keyFrame = args.FrameBegin;
                var kbSize = keyFrame.Size;

				var num1 = kbSize.Width;
				var num2 = kbSize.Height;
				kbSize.Width = (float)Math.Max (num1, num2);
				kbSize.Height = (float)Math.Min (num1, num2);

                KeyboardSize = kbSize;
                if (textView != null)
                {
                    var contentInsets = new UIEdgeInsets (0.0f, 0.0f, kbSize.Height, 0.0f);
                    textView.ContentInset = contentInsets;
					textView.ScrollIndicatorInsets = contentInsets;
                }
            }
        }

        public void ResetTextView()
        {
            KeyboardSize.Width = 0;
            KeyboardSize.Height = 0;
            if (textView != null)
            {
                var contentInsets = new UIEdgeInsets ();
                textView.ContentInset = contentInsets;
                textView.ScrollIndicatorInsets = contentInsets;
            }
        }

        #endregion
    }
}
