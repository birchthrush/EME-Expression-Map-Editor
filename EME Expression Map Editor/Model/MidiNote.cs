using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EME_Refactored.Model
{
    public static class MidiNote
    {
        public static string MidiNoteName(int n)
        {
            n %= 12;
            if (n == 0)
                return "C";
            if (n == 1)
                return "C#";
            if (n == 2)
                return "D";
            if (n == 3)
                return "D#";
            if (n == 4)
                return "E";
            if (n == 5)
                return "F";
            if (n == 6)
                return "F#";
            if (n == 7)
                return "G";
            if (n == 8)
                return "G#";
            if (n == 9)
                return "A";
            if (n == 10)
                return "A#";
            else
                return "B";
        }

        public static int MidiNoteOctave(int n)
            => (n == 0) ? -2 : -2 + (n / 12);

        public static string MidiNoteToString(int n)
            => MidiNoteName(n) + MidiNoteOctave(n).ToString();

        public static int StringToOctaveOffset(string str)
        {
            int n = Int32.Parse(str) + 2;
            return n * 12;
        }

        public static int NoteNameToMidi(string str)
        {
            if (!IsValidNoteName(str))
                return -1;

            int n = 0;
            str = str.ToUpper();

            if (str[0] == 'C')
                n = 0;
            else if (str[0] == 'D')
                n = 2;
            else if (str[0] == 'E')
                n = 4;
            else if (str[0] == 'F')
                n = 5;
            else if (str[0] == 'G')
                n = 7;
            else if (str[0] == 'A')
                n = 9;
            else if (str[0] == 'B')
                n = 11;


            if (str.Length == 2)
            {
                n += StringToOctaveOffset(str.Substring(1, 1));
            }
            else
            {
                if (str[1] == '#')
                {
                    ++n;
                    n += StringToOctaveOffset(str.Substring(2));
                }
                else if (str[1] == 'B')
                {
                    --n;
                    n += StringToOctaveOffset(str.Substring(2));
                }
                else if (str[1] == '-')
                {
                    n += StringToOctaveOffset(str.Substring(1));
                }
            }

            if (n < 0)
                return 0;
            else if (n > 127)
                return 127;
            else
                return n;
        }

        public static bool IsValidNoteName(string str)
        {
            if (str.Length < 2)
                return false;
            str = str.ToUpper();
            if (str[0] != 'C' && str[0] != 'D' && str[0] != 'E' && str[0] != 'F' && str[0] != 'G' && str[0] != 'A' && str[0] != 'B')
                return false;
            if (Char.IsDigit(str[1]))
            {
                if (str.Length != 2)
                    return false;
            }
            else if (str[1] == '-' && str.Length != 3)
                return false;
            else
            {
                if (str[1] != '#' && str[1] != 'B')
                {
                    if (str.Length >= 4 && str[2] != '-' && !Char.IsDigit(str[3]))
                        return false;
                    else if (str.Length != 3 || !Char.IsDigit(str[2]))
                        return false;
                }
            }
            return true;
        }
    }
}
