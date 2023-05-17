using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace EME_Expression_Map_Editor.Model
{
	public class OutputEvent
	{
		// Predefined constants in ExpressionMap spec to define whether an Output Event transmits a midi note, program change or continuous controller
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
			set => _data1 = Math.Clamp(value, Common.DATA_MIN, Common.DATA_MAX); 
		}

		private int _data2;
		public int Data2
		{
			get => _data2;
			set => _data2 = Math.Clamp(value, Common.DATA_MIN, Common.DATA_MAX);
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
    }
}
