using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace EME_Expression_Map_Editor.Model
{
	public class OutputEvent
	{
		public static readonly int NoteEvent = 144;
		public static readonly int ProgramChangeEvent = 192;
		public static readonly int ControllerEvent = 176;

        private int _eventType;
		public int EventType
		{
			get => _eventType;
			set
			{
				if (value == ControllerEvent || value == ProgramChangeEvent)
					_eventType = value;
				else
					_eventType = NoteEvent;
			}
		}

        private int _data1;
        public int Data1
		{
			get => _data1;
			set => _data1 = ExpressionMap.Common.ConstrainToRange(value);
		}

		private int _data2;
		public int Data2
		{
			get => _data2;
			set => _data2 = ExpressionMap.Common.ConstrainToRange(value);
		}

		public OutputEvent()
		{
			InitDefaults();
		}

		public OutputEvent(int event_type, int d1, int d2)
		{
			EventType = event_type;
			Data1 = d1;
			Data2 = d2;
		}

		public OutputEvent Duplicate()
		{
			OutputEvent copy = new OutputEvent();

			copy.EventType = this.EventType;
			copy.Data1 = this.Data1;
			copy.Data2 = this.Data2;

			return copy;
		}

		public void InitDefaults()
		{
			EventType = NoteEvent;
			Data1 = 60;
			Data2 = 120;
		}

		/*
		public override bool Equals(Object obj)
		{
			OutputEvent other = obj as OutputEvent;
			if (other == null)
				return false; 
			else
				return this.EventType == other.EventType 
					&& this.Data1 == other.Data1 
					&& this.Data2 == other.Data2; 
		}

        public override int GetHashCode()
			=> HashCode.Combine(EventType, Data1, Data2);	
		*/
    }
}
