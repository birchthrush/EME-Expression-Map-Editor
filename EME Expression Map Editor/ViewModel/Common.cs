﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EME_Expression_Map_Editor.ViewModel
{
    public static class Common
    {
        public enum WindowReturnCode
        {
            OK,
            CANCEL,
            DELETE
        };

        public enum PopupType
        {
            NUMBER_INPUT,
            OUTPUT_EVENT_REPLACEMENT
        }

        static public bool ParseSignedByte(string str, out sbyte result)
        {
            if (Int32.TryParse(str, out int n))
            {
                if (n > 127)
                    result = 127;
                else if (n < 0)
                    result = 0;
                else
                    result = (sbyte)n;
                return true;
            }
            else
            {
                result = 0;
                return false;
            }
        }

        // Add/remove functions - parameters: collection to be operated on, current selection index, an optional post-insertion/deletion function to be executed
        // Returns index to new selected element after operation is finished
        public static int AddItem<T>(ObservableCollection<T> list, int idx, Action post_func) where T : ViewModelBase, new()
        {
            if (list.Count == 0)
            {
                list.Add(new T());
                post_func();
                return 0;
            }
            else
            {
                idx = Math.Clamp(idx, 0, list.Count - 1);
                T item = (T)list[idx].GetPrototype(list[idx]);
                list.Insert(idx + 1, item);
                post_func();
                return idx + 1;
            }
        }

        public static int RemoveItem<T>(ObservableCollection<T> list, int selection_idx, Action post_func) where T : ViewModelBase
        {
            var selection = list.Where(x => x.IsSelected).ToList();

            // Empty selection check: 
            if (selection.Count == 0)
                return selection_idx;

            int pre_idx = list.IndexOf(selection.First());

            foreach (var item in selection)
                list.Remove(item);

            post_func();

            return Math.Clamp(pre_idx, -1, list.Count - 1);
        }

        // Blank dummy function for when post_func above are not needed
        public static void DoNothing()
        {

        }

        static public int FactorToPercentage(double f)
            => (int)Math.Round((f * 100.0));

        static public double PercentageToFactor(int n)
            => (n / 100.0f);

        static public int ParsePercentage(string s, int default_value)
            => Int32.TryParse(s.Replace("%", String.Empty), out int n) ? n : default_value;

        public static class KeyModifiers
        {
            public static bool CascadeKeyActive()
                => Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);

            public static bool AlternativeInputKeyActive()
                => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }
    }
}
