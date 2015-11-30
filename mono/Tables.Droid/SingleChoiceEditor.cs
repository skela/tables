using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

using Android.App;
using Android.OS;
using Android.Content;

namespace Tables.Droid
{
    public interface SingleChoiceEditorListener
    {
        void ChangedSingleChoiceValue(SingleChoiceEditor fragment,object changedValue);
    }

    public interface SingleChoiceItemAdapterCreator
    {
        TableSingleChoiceAdapter CreateSingleChoiceAdapter(SingleChoiceEditor fragment);
    }

    public class Item : object,IEquatable<Item>
    {
        public string Title;
        public string Detail;
        public object Object;
        public int Integer;
        public float Float;
        public double Double;
        public DateTime? DateTime;

        public string Key;
        public bool Selected;

        public Item (string val=null,bool selected=false,int aux=0) : base()
        {
            Selected = selected;
            Val = val;
            Integer = aux;
        }

        public bool Equals(Item other)
        {
            if (other == null) 
                return false;

            return String.Equals(this.Title, other.Title);
        }

        public string Val
        {
            get
            {
                return Title;
            }
            set
            {
                Title = value;
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }

    public class SingleChoiceEditor : DialogFragment
    {
        private bool isStatic;
        static IList<object>Choices;
        static object Chosen;

        private bool isStrings;
        private IList<string>choices;
        private string chosen;

        private bool isItems;
        static IList<Item>choiceItems;
        static Item chosenItem;

        public SingleChoiceEditor() : base()
        {

        }

        public static SingleChoiceEditor CreateFragmentWithStrings(SingleChoiceEditorListener listener,string title,IList<string>choices,string chosen)
        {
            var args = new Bundle();
            args.PutString("title",title);
            args.PutStringArrayList("choices_strings", choices);
            args.PutString("chosen", chosen);
            args.PutBoolean("strings", true);
            var fragment = new SingleChoiceEditor();
            fragment.Arguments = args;
            return fragment;
        }

        public static SingleChoiceEditor CreateFragmentWithStrings(SingleChoiceEditorListener listener,string title,int choices,string chosen)
        {
            var args = new Bundle();
            args.PutString("title",title);
            args.PutInt("choices_rid", choices);
            args.PutString("chosen", chosen);
            args.PutBoolean("strings", true);
            var fragment = new SingleChoiceEditor();
            fragment.Arguments = args;
            return fragment;
        }

        public static SingleChoiceEditor CreateFragmentWithItems(SingleChoiceEditorListener listener,string title,IList<Item>choices,Item chosen)
        {
            var items = TextHelper.ToJSON<IList<Item>>(choices);
            var item = TextHelper.ToJSON<Item>(chosen);

            var args = new Bundle();
            args.PutString("title",title);
            args.PutString("choices_items", items);
            if (item!=null)
                args.PutString("chosen_item", item);
            args.PutBoolean("items", true);
            var fragment = new SingleChoiceEditor();
            fragment.Arguments = args;
            return fragment;
        }

        public static SingleChoiceEditor CreateFragmentWithObjects(SingleChoiceEditorListener listener,string title,IList<object>theChoices,object chosenValue)
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

        SingleChoiceItemAdapterCreator Creator
        {
            get
            {
                SingleChoiceItemAdapterCreator creator = null;
                if (Activity is SingleChoiceItemAdapterCreator)
                {
                    creator = Activity as SingleChoiceItemAdapterCreator;
                }
                return creator;
            }
        }

        public override Dialog OnCreateDialog(Android.OS.Bundle savedInstanceState)
        {            
            isStatic = Arguments.GetBoolean("static", false);
            isStrings = Arguments.GetBoolean("strings", false);
            isItems = Arguments.GetBoolean("items", false);

            int selectedItemIndex = -1;
            var creator = Creator;
            TableSingleChoiceAdapter adapter = null;
            if (isItems)
            {                
                var jsChoices = Arguments.GetString("choices_items", null);
                var jsChosen = Arguments.GetString("chosen_item", null);

                choiceItems = TextHelper.FromJSON<IList<Item>>(jsChoices);
                chosenItem = TextHelper.FromJSON<Item>(jsChosen);

                if (choiceItems != null && chosenItem != null)
                {
                    selectedItemIndex = choiceItems.IndexOf(chosenItem);
                }

                if (creator != null)
                    adapter = creator.CreateSingleChoiceAdapter(this);
                if (adapter != null)
                    adapter.SetItems(choiceItems);
                if (adapter == null)
                    adapter = new TableSingleChoiceAdapter(Activity, choiceItems);
            }
            else if (isStrings)
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
                if (creator != null)
                    adapter = creator.CreateSingleChoiceAdapter(this);
                if (adapter != null)
                    adapter.SetOptions(choices);
                if (adapter == null)
                    adapter = new TableSingleChoiceAdapter(Activity, choices);
            }
            else if (isStatic)
            {
                if (Choices != null && Chosen != null)
                {
                    selectedItemIndex = Choices.IndexOf(Chosen);
                }
                if (creator != null)
                    adapter = creator.CreateSingleChoiceAdapter(this);
                if (adapter != null)
                    adapter.SetOptions(Choices);
                if (adapter==null)
                    adapter = new TableSingleChoiceAdapter(Activity,Choices);                
            }

            var alert = new AlertDialog.Builder(Activity);
            alert.SetTitle(Arguments.GetString("title"));
            alert.SetMessage(Arguments.GetString("message"));
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
                    else if (isItems)
                    {
                        if (choiceItems != null)
                            theChoice = choiceItems[index];
                        chosenItem = theChoice as Item;
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

