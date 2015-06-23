using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Content;

namespace Tables.Droid
{
    public interface SingleChoiceEditorListener
    {
        void ChangedSingleChoiceValue(SingleChoiceEditor fragment,object changedValue);
    }

    public class SingleChoiceEditor : DialogFragment
    {
        static IList<object>Choices;
        static object Chosen;

        private IList<string>choices;
        private string chosen;
        private bool isStatic;

        public SingleChoiceEditor() : base()
        {

        }

        public static SingleChoiceEditor CreateFragment(SingleChoiceEditorListener listener,string title,IList<string>choices,string chosen)
        {
            var args = new Bundle();
            args.PutString("title",title);
            args.PutStringArrayList("choices_strings", choices);
            args.PutString("chosen", chosen);
            var fragment = new SingleChoiceEditor();
            fragment.Arguments = args;
            return fragment;
        }

        public static SingleChoiceEditor CreateFragment(SingleChoiceEditorListener listener,string title,int choices,string chosen)
        {
            var args = new Bundle();
            args.PutString("title",title);
            args.PutInt("choices_rid", choices);
            args.PutString("chosen", chosen);
            var fragment = new SingleChoiceEditor();
            fragment.Arguments = args;
            return fragment;
        }

        public static SingleChoiceEditor CreateFragment(SingleChoiceEditorListener listener,string title,IList<object>theChoices,object chosenValue)
        {
            SingleChoiceEditor.Choices = theChoices;
            SingleChoiceEditor.Chosen = chosenValue;

            var args = new Bundle();
            args.PutString("title",title);
            args.PutBoolean("static", true);
            var fragment = new SingleChoiceEditor();
            fragment.Arguments = args;
            return fragment;
        }

        SingleChoiceEditorListener Listener
        {
            get
            {
                SingleChoiceEditorListener listener = null;
                if (Activity is SingleChoiceEditorListener)
                {
                    listener = Activity as SingleChoiceEditorListener;
                }
                else
                {
                    var adapter = TableEditor.AdapterForActivity(Activity);
                    if (adapter is SingleChoiceEditorListener)
                    {
                        listener = adapter as SingleChoiceEditorListener;
                    }
                }
                return listener;
            }
        }

        public override Dialog OnCreateDialog(Android.OS.Bundle savedInstanceState)
        {
            var title = Arguments.GetString("title");
            isStatic = Arguments.GetBoolean("static", false);

            int selectedItemIndex = -1;
            TableSingleChoiceAdapter adapter = null;
            if (!isStatic)
            {                
                chosen = Arguments.GetString("chosen", null);
                choices = Arguments.GetStringArrayList("choices_strings");
                if (choices == null)
                {
                    var rid = Arguments.GetInt("choices_rid", -1);
                    if (rid != -1)
                    {
                        choices = Activity.Resources.GetStringArray(rid);
                    }
                }
                if (choices != null && chosen != null)
                {
                    selectedItemIndex = choices.IndexOf(chosen);
                }
                adapter = new TableSingleChoiceAdapter(Activity,choices);
            }
            else
            {                
                if (Choices != null && Chosen != null)
                {
                    selectedItemIndex = Choices.IndexOf(Chosen);
                }
                adapter = new TableSingleChoiceAdapter(Activity,Choices);
            }

            var alert = new AlertDialog.Builder(Activity);
            alert.SetTitle(title);
            alert.SetSingleChoiceItems(adapter, selectedItemIndex, delegate(object sender, DialogClickEventArgs e)
            {
                if (e != null)
                {
                    var index = e.Which;
                    object theChoice = null;
                    if (isStatic)
                    {
                        if (Choices != null)
                            theChoice = Choices[index];
                        Chosen = theChoice;
                    }
                    else
                    {
                        if (choices != null)
                            theChoice = choices[index];
                        chosen = theChoice as string;
                    }
                    var listener = Listener;
                    if (listener!=null)
                        listener.ChangedSingleChoiceValue(this,theChoice);                    
                    Dismiss();
                }
            });
            return alert.Create();
        }
    }
}

