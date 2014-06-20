using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Tables.iOS.Testing
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("TablesIOSTestingAppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        UIWindow window;

        //
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            window = new UIWindow(UIScreen.MainScreen.Bounds);
			
            window.RootViewController = new TableViewController();
		
            window.MakeKeyAndVisible();
			
            return true;
        }
    }

    public class TableViewController : UIViewController
    {
        public TableAdapter Adapter;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UITableView tv = new UITableView(this.View.Bounds);
            tv.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            Adapter = new TableAdapter(tv,TestData.CreateTestData());
            View.AddSubview(tv);
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }
    }
}
