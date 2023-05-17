using System;
using System.Collections.Generic;

namespace EME_Expression_Map_Editor.Model
{
	public static class ExpressionMapCommon
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
	}

	public class ExpressionMap
	{        
		public static class Common
        {
            public const int DATA_MIN = 0;
            public const int DATA_MAX = 127;
            public static int ConstrainToRange(int value, int min = DATA_MIN, int max = DATA_MAX)
                => Math.Clamp(value, min, max);

            public static double ConstrainToRange(double val, double min = (double)DATA_MIN, double max = (double)DATA_MAX)
                => Math.Clamp(val, min, max);
        }

		private string _name = ""; 
		public string Name
		{
			get => _name;
			set => _name = value;
		}

		private List<Articulation> _articulations = new List<Articulation>();
		public List<Articulation> Articulations
		{
			get => _articulations; 
		}

		private List<SoundSlot> _soundslots = new List<SoundSlot>(); 
		public List<SoundSlot> SoundSlots
		{
			get => _soundslots;
			set => _soundslots = value; 
		}

		public ExpressionMap()
		{

		}

		public ExpressionMap(string name)
		{
			Name = name; 
		}

		public ExpressionMap Duplicate()
		{
			ExpressionMap copy = new ExpressionMap();

			copy.Name = this.Name;

			foreach (Articulation art in Articulations)
				copy.Articulations.Add(art.Duplicate());

			foreach (SoundSlot slot in SoundSlots)
				copy.SoundSlots.Add(slot.Duplicate()); 

			return copy; 
		}
		public void RemapArticulations()
		{
			foreach (SoundSlot slot in SoundSlots)
			{
				for (int i = 0; i < 4; ++i)
				{
					if (!Articulation.IsBlank(slot.Articulations[i]))
					{
						int ref_idx = this.Articulations.FindIndex(x => x.IsEquivalentTo(slot.Articulations[i]));
						if (ref_idx >= 0)
							slot.Articulations[i] = this.Articulations[ref_idx]; 
					}
				}
			}
		}

		public override string ToString()
			=> this.Name; 
	}




}