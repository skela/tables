﻿using System;
using System.Collections.Generic;

using Android.Widget;
using Android.Content;
using Android.Views;

namespace Tables.Droid
{
    public class TableSingleChoiceAdapter : BaseAdapter
    {
        private IList<object> options;

        public TableSingleChoiceAdapter()
        {
            
        }

        public TableSingleChoiceAdapter(Context ctx, IList<object> options)
        {
            this.options = options;
        }

        public TableSingleChoiceAdapter(Context ctx, IList<string> options)
        {
            this.options = options as IList<object>;
        }

        public void SetOptions(IList<object> options)
        {
            this.options = options;
        }

        public void SetOptions(IList<string> options)
        {
            this.options = options as IList<object>;
        }

        #region BaseAdapter

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            View view = null;

            if (convertView == null)
            {
                TableAdapterSingleChoiceCell body = new TableAdapterSingleChoiceCell(parent.Context);
                view = body;
            }
            else
            {
                view = convertView;
            }

            if (view is ITableAdapterSingleChoiceCell)
            {
                ITableAdapterSingleChoiceCell c = view as ITableAdapterSingleChoiceCell;
                UpdateView(c,position,0);
            }

            return view;
        }
       
        public virtual void UpdateView(ITableAdapterSingleChoiceCell cell, int row, int section)
        {
            string returnValue = null;
            if (options != null)
            {
                var anObject = options [row];
                returnValue = anObject.ToString ();
            }

            cell.Text = returnValue;
        }

        public override int Count
        {
            get
            {
                return options == null ? 0 : options.Count;
            }
        }

        #endregion   
    }

    public class TableMultiChoiceAdapter : BaseAdapter
    {
        private List<Object> options;

        public TableMultiChoiceAdapter(Context ctx, TableAdapterRowConfig config, List<Object> options)
        {
            this.options = options;
        }

        public TableMultiChoiceAdapter(Context ctx, List<Object> options)
        {
            this.options = options;
        }

        #region BaseAdapter

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            View view = null;

            if (convertView == null)
            {
                TableAdapterSingleChoiceCell body = new TableAdapterSingleChoiceCell(parent.Context);
                view = body;
            }
            else
            {
                view = convertView;
            }

            if (view is ITableAdapterSingleChoiceCell)
            {
                ITableAdapterSingleChoiceCell c = view as ITableAdapterSingleChoiceCell;
                UpdateView(c,position,0);
            }

            return view;
        }

        public virtual void UpdateView(ITableAdapterSingleChoiceCell cell, int row, int section)
        {
            string returnValue = null;
            if (options != null)
            {
                var anObject = options [row];
                returnValue = anObject.ToString ();
            }
            cell.Text = returnValue;
        }

        public override int Count
        {
            get
            {
                return options == null ? 0 : options.Count;
            }
        }

        #endregion   
    }
}