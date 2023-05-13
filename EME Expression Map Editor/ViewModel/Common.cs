using System;
using System.Collections.Generic;
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

        static public int FactorToPercentage(float f)
            => (int)MathF.Round((f * 100.0f));

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
