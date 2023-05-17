using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EME_Expression_Map_Editor.ViewModel
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

    public static class Common
    {

        public static int AddItem<T>(ObservableCollection<T> list, int idx, Action<T, T> copy_func) where T : ViewModelBase, new()
        {
            if (list.Count == 0)
            {
                T item = new T(); 
                //copy_func(item);
                list.Add(item);
                return 0;
            }
            else
            {
                idx = Math.Clamp(idx, 0, list.Count - 1);
                //T item = (T)list[idx].Clone();
                T item = new T(); 
                list.Insert(idx + 1, item);
                copy_func(list[idx], item);
                return idx + 1;
            }
        }

        public static int RemoveItem<T>(ObservableCollection<T> list, int selection_idx, Func<T, bool> predicate, Action post_func) where T : ViewModelBase
        {
            var selection = list.Where(predicate).ToList();

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
        public static void DoNothing<T>(T src, T dest)
        {

        }

        public static void DoNothing<T>(T src)
        {

        }

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
