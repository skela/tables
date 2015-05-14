﻿using System;

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
        public TableAdapter Adapter;
        ListView listView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            listView = new ListView(this);

            SetContentView(listView);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);

//            var data = TestData.CreateTestData();
//            Adapter = new TableAdapter(this,listView,data);

            var sdata = TestData.CreateSectionedTestData();
            Adapter = new TableAdapter(this,listView,sdata);
        }
    }
}
