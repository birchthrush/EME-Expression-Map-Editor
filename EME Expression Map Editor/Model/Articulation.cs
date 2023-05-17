using System;
using System.Collections.Generic;

namespace EME_Expression_Map_Editor.Model
{
	public class Articulation
	{
        public static Dictionary<string, string> Abbreviations = new Dictionary<string, string>()
        {
            { "s", "Short" },
            { "m", "Medium" },
            { "l", "Long" },
            { "f", "Fast" },
            { "sl", "Slow" },
            { "tr", "Trills" },
            { "stac", "Staccato" },
            { "trem", "Tremolo" },
            { "det", "Detaché" },
            { "marc", "Marcato" },
            { "msrd", "Measured" },
            { "leg", "Legato" },
            { "cresc", "Crescendo" },
            { "dim", "Diminuendo" },
            { "port", "Portato" },
            { "flaut", "Flautando" },
            { "cs", "Con Sordino" },
            { "ss", "Senza Sordino" },
            { "sus", "Sustains" },
            { "espr", "Espressivo" },
            { "acc", "Accented" },
            { "fp", "Fortepiano" },
            { "dbl", "Double" },
            { "trpl", "Triple" },
            { "spic", "Spiccato" },
            { "norm", "Normal" },
            { "nat", "Natural" },
            { "pizz", "Pizzicato" },
            { "rep", "Repetitions" },
            { "gliss", "Glissando" },
            { "sp", "Sul Ponticello" },
            { "st", "Sul Tasto" },
            { "pp", "Pianissimo" },
            { "ff", "Fortissimo" },
            { "nv", "Non-Vibrato" },
            { "v", "Vibrato" },
            { "mv", "Molto Vibrato" },
            { "sv", "Strong Vibrato" },
            { "pv", "Progressive Vibrato" },
            { "xf", "Crossfade" },
        };

        public static string TextToDescription(string text)
        {
            string[] words = text.Split(' ');
            List<string> descriptors = new List<string>();

            string CapitalizeFirstLetter(string str) => str.Length > 0 ?
                str.Substring(0, 1).ToUpper() + str.Substring(1) :
                str;

            foreach (String word in words)
            {
                if (Abbreviations.ContainsKey(word.ToLower()))
                    descriptors.Add(Abbreviations[word.ToLower()]);
                else
                    descriptors.Add(CapitalizeFirstLetter(word.ToLower()));
            }

            string description = string.Empty;
            foreach (string s in descriptors)
            {
                if (description.Length > 0)
                    description += ' ';
                description += s;
            }

            return description;
        }


        private static Articulation _blank = new Articulation();
		public static Articulation Blank
		{
			get => _blank; 
		}

		public static bool IsBlank(Articulation art)
			=> art.Equals(Blank); 

		public static readonly int MinGroup = 0;
		public static readonly int MaxGroup = 3; 

		public enum Display
		{
			Symbol = 0,
			Text = 1
		}

		public enum ArtType
		{
			Attribute = 0,
			Direction = 1
		}

		public enum SymbolCode
		{
			TenutoDash = 43,
			StaccatoDot = 73,
			Tremolo1Thin = 90,
			Tremolo2Thin = 91,
			Tremolo3Thin = 92
		}

		private Display _displayType;
		public Display DisplayType
		{
			get => _displayType;
			set => _displayType = value;
		}

		private ArtType _articulationType;
		public ArtType ArticulationType
		{
			get => _articulationType;
			set => _articulationType = value;
		}

		private int _symbol;
		public int Symbol
		{
			get => _symbol;
			set => _symbol = value;
		}

		private string _text = "";
		public string Text
		{
			get => _text;
			set => _text = value;
		}

		private string _description = "";
		public string Description
		{
			get => _description;
			set => _description = value;
		}

		// Articulations belong to one of four groups, numbered 0-3. 
        private int _group;
		public int Group
		{
			get => _group;
			set => _group = Math.Clamp(value, MinGroup, MaxGroup); 
		}

		public Articulation()
		{
			InitDefaults();
		}

		public void InitDefaults()
		{
			DisplayType = Display.Text;
			ArticulationType = ArtType.Attribute;
			Symbol = (int)SymbolCode.StaccatoDot;
			Text = "";
			Description = "";
			Group = 0;
		}
		
		public override string ToString()
		{
			if (DisplayType == Display.Symbol)
				return Symbol.ToString();
			else
				return Text; 
		}

		public bool IsEquivalentTo(Articulation art)
		{
			if (this.DisplayType == Display.Symbol)
			{
				return art.DisplayType == Display.Symbol && this.Symbol == art.Symbol;
			}
			else
			{
				return art.DisplayType == Display.Text && this.Text == art.Text;
			}
		}

		public Articulation Duplicate()
		{
			if (IsBlank(this))
				return Articulation.Blank; 

			Articulation copy = new Articulation();

			copy.DisplayType = this.DisplayType;
			copy.ArticulationType = this.ArticulationType;
			copy.Symbol = this.Symbol;
			copy.Text = this.Text;
			copy.Description = this.Description;
			copy.Group = this.Group;

			return copy;
		}
	}
}
