using System;
using MonoTouch.UIKit;

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
    }
}