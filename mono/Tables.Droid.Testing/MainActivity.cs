using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Tables.Droid.Testing
{
    [Activity(Label = "Tables.Droid.Testing", MainLauncher = true,Theme = "@style/Theme.Tables")]
    public class MainActivity : Activity
    {
        public BaseAdapter Adapter;
        ListView listView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            listView = new ListView(this);

            SetContentView(listView);

            //            var data = TestData.CreateTestData();
            //            Adapter = new TableAdapter(this,listView,data);

            //Adapter = new TableAdapter(this,listView,TestData.CreateSectionedTestData());
            Adapter = new TableSectionAdapter(this,listView,TestData.CreateSectionsTestData());

        }
    }
}
