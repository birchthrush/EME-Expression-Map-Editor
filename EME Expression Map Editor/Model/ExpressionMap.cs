using System;
using System.Collections.Generic;

namespace EME_Expression_Map_Editor.Model
{
	public static class Common
	{
        public const int DATA_MIN = 0;
        public const int DATA_MAX = 127;
	}

	public class ExpressionMap
	{     
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

		// Must be performed after map has been read from file or VM layer. See documentation in XmlFileManagement.XmlConstants
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