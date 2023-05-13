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

		public static List<T> SortElementsByReference<T>(List<T> src, List<T> reference)
        {
			src.Sort((x, y) => reference.IndexOf(x).CompareTo(reference.IndexOf(y)));
			return src; 
        }

		public static void MoveElements<T>(List<T> src, int tgt_idx, List<T> tgt_collection) where T : class
		{
			src = SortElementsByReference(src, tgt_collection); 
			foreach (T elem in src)
			{
				int src_idx = tgt_collection.IndexOf(elem);
				tgt_collection.RemoveAt(src_idx);
				if (src_idx > tgt_idx)  // Moving up
				{
					tgt_collection.Insert(tgt_idx, elem);
					++tgt_idx;
				}
				else
				{
					tgt_collection.Insert(tgt_idx - 1, elem);
				}
			}
		}
	}

	public class ExpressionMap
	{        
		public static class Common
        {
            const int DATA_MIN = 0;
            const int DATA_MAX = 127;
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

        // ----------------------------
        // Old utility functions below: 
        // ----------------------------

        public void DuplicateSlots(List<SoundSlot> tgt)
        {
			tgt = ExpressionMapCommon.SortElementsByReference(tgt, _soundslots);
			int ins_idx = _soundslots.IndexOf(tgt[tgt.Count - 1]) + 1;

			foreach (SoundSlot s in tgt)
				_soundslots.Insert(ins_idx++, s.Duplicate()); 
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

		public void SortArticulationsByGroup()
        {
			List<Articulation> sorted_list = new List<Articulation>(); 
			for (int i = 0; i <= 3; ++i)
            {
				foreach (Articulation art in Articulations.FindAll(x => x.Group == i))
					sorted_list.Add(art); 
            }
			_articulations = sorted_list; 
        }

		public void AddSlot(SoundSlot s, int idx)
        {
			if (_soundslots.Count == 0 || idx < 0)
				_soundslots.Add(s);
			else
			{				
				s.Color = SoundSlot.GetNextColor(s.Color); 
				
				if (idx >= _soundslots.Count - 1)
					_soundslots.Add(s);
				else
					_soundslots.Insert(idx + 1, s); 
			}
        }

		public void RemoveSlot(SoundSlot tgt)
        {
			_soundslots.Remove(tgt); 
        }

		public void RemoveSlots(IList<SoundSlot> tgt)
        {
			foreach (SoundSlot s in tgt)
				_soundslots.Remove(s); 
        }

		public void AddArticulation(Articulation a, int idx)
        {
			if (_articulations.Count == 0)
			{
				_articulations.Add(a); 
            }
			else
            {
				if (idx < 0 || idx > _articulations.Count)
					idx = _articulations.Count - 1;
				a.Group = _articulations[idx].Group;
				_articulations.Insert(idx + 1, a); 
            }

			SortArticulationsByGroup(); 
        }

		public bool ChangeArticulationGroup(Articulation a, int group, bool sort = true)
        {
			if (group < 0 || group > 3)
				return false; 

			foreach (Articulation art in _articulations)
            {
				if (ReferenceEquals(art, a))
					art.Group = (sbyte)group;
            }

			foreach (SoundSlot s in _soundslots)
            {
				if (s.UnassignArticulation(a))
					s.AssignArticulation(a, group); 
            }

			if (sort)
				SortArticulationsByGroup(); 

			return true; 
        }

		public bool ChangeArticulationGroup(List<Articulation> arts, int to_group)
        {
			if (to_group < 0 || to_group > 3)
				return false;

			ExpressionMapCommon.SortElementsByReference(arts, _articulations); 

			foreach (Articulation art in arts)
				ChangeArticulationGroup(art, to_group, false);
			
			SortArticulationsByGroup(); 

			return true; 
        }

		public bool ChangeArticulationGroup(int idx, int from_group, int to_group)
        {
			IList<Articulation> arts = ArticulationGroup(from_group);
			if (idx < 0 || idx >= arts.Count)
				return false;

			arts[idx].Group = (sbyte)to_group;

			// Remap assigned articulations
			foreach (SoundSlot s in _soundslots)
            {
				if (s.UnassignArticulation(arts[idx]))
					s.AssignArticulation(arts[idx], to_group); 
            }

			SortArticulationsByGroup(); 

			return true; 
		}

		public void RemoveUnusedArticulations()
        {
			Dictionary<Articulation, bool> dictionary = new Dictionary<Articulation, bool>();
			foreach (Articulation a in _articulations)
				dictionary.Add(a, false);

			foreach (SoundSlot s in _soundslots)
				foreach (Articulation a in s.Articulations)
					if (!Articulation.IsBlank(a))
						dictionary[a] = true; 

			foreach (var pair in dictionary)
            {
				if (pair.Value == false)
					RemoveArticulation(pair.Key); 
            }
        }

		public IList<Articulation> ArticulationGroup(int g)
			=> _articulations.FindAll(x => x.Group == g); 
        
		public void RemoveArticulation(Articulation tgt)
        {
			_articulations.Remove(tgt);

			foreach (SoundSlot s in _soundslots)
				s.UnassignArticulation(tgt); 
        }

		public void MoveSoundSlots(List<SoundSlot> slots, int tgt_idx)
        {
			ExpressionMapCommon.MoveElements(slots, tgt_idx, _soundslots); 
        }

		public void MoveArticulations(List<Articulation> arts, int tgt_idx)
        {
			ExpressionMapCommon.MoveElements(arts, tgt_idx, _articulations);
			SortArticulationsByGroup(); 
        }

		public void CascadeColors(List<SoundSlot> slots, int col)
        {
			slots = ExpressionMapCommon.SortElementsByReference(slots, _soundslots); 

			foreach (SoundSlot slot in slots)
            {
				slot.Color = col;				
				col = SoundSlot.GetNextColor(col); 
            }
        }

		public override string ToString()
			=> this.Name; 
	}




}