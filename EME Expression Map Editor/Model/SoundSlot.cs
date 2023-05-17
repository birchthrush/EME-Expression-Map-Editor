using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace EME_Expression_Map_Editor.Model
{
	public class SoundSlot
	{
		public static readonly int AnyChannel = -1;
		public static readonly int NoRemoteKey = -1;

		public static readonly int DefaultVersion = 600;

        public static readonly int MinColor = 1;
        public static readonly int MaxColor = 16;

        public static int GetNextColor(int c)
            => (c + 1) > MaxColor ? MinColor : c + 1;

		// Unknown what this parameter actually does in the ExpMap specification
		private int _version; 
		public int Version
		{
			get => _version; 
			set => _version = value;
		}

        private int _remoteKey;
        public int RemoteKey
		{
			get => _remoteKey;
			set => _remoteKey = ExpressionMap.Common.ConstrainToRange(value, -1, 127);
		}

		// Channels are numbered 0-15: slots set to 'any' channel represented as Channel = -1
        private int _channel;
        public int Channel
		{
			get => _channel;
			set => _channel = ExpressionMap.Common.ConstrainToRange(value, AnyChannel, 15); 
		}

        private double _velocityFactor;
        public double VelocityFactor
		{
			get => _velocityFactor;
			set => _velocityFactor = ExpressionMap.Common.ConstrainToRange(value, 0.2f, 2.0f);
		}

        private double _lengthFactor;
        public double LengthFactor
		{
			get => _lengthFactor;
			set => _lengthFactor = ExpressionMap.Common.ConstrainToRange(value, 0.2f, 2.0f);
		}

        private int _minVelocity;
        public int MinVelocity
		{
			get => _minVelocity;
			set => _minVelocity = ExpressionMap.Common.ConstrainToRange(value, 0, 127);
		}

        private int _maxVelocity;
        public int MaxVelocity
		{
			get => _maxVelocity;
			set => _maxVelocity = ExpressionMap.Common.ConstrainToRange(value, 0, 127);
		}

        private int _transpose;
        public int Transpose
		{
			get => _transpose;
			set => _transpose = ExpressionMap.Common.ConstrainToRange(value, -48, 48);
		}

        private int _minPitch;
        public int MinPitch
		{
			get => _minPitch;
			set => _minPitch = ExpressionMap.Common.ConstrainToRange(value, 0, 127);
		}

        private int _maxPitch;
        public int MaxPitch
		{
			get => _maxPitch;
			set => _maxPitch = ExpressionMap.Common.ConstrainToRange(value, 0, 127);
		}

		private string _name = ""; 
		public string Name
		{
			get => _name;
			set => _name = value; 
		}

		// Colors are numbered 1-16
        private int _color;
        public int Color
		{
			get => _color;
			set => _color = ExpressionMap.Common.ConstrainToRange(value, MinColor, MaxColor);
		}

        private List<Articulation> _articulations = new List<Articulation>();
        public List<Articulation> Articulations
		{
			get => _articulations;
		}

        private List<OutputEvent> _outputEvents = new List<OutputEvent>();
        public List<OutputEvent> OutputEvents
		{
			get => _outputEvents;
		}

		public SoundSlot()
		{
			this.InitDefaults();
		}

		public void InitDefaults()
		{
			RemoteKey = NoRemoteKey;
			Channel = AnyChannel;
			VelocityFactor = 1.0f;
			LengthFactor = 1.0f;
			MinVelocity = 0;
			MaxVelocity = 127;
			Transpose = 0;
			MinPitch = 0;
			MaxPitch = 127;
			Color = 1;
			Version = DefaultVersion;

            _articulations = new List<Articulation>
            {
                Articulation.Blank,
                Articulation.Blank,
                Articulation.Blank,
                Articulation.Blank
            };
        }

		public SoundSlot Duplicate()
		{
			SoundSlot copy = new SoundSlot();

			copy.RemoteKey = this.RemoteKey;
			copy.Channel = this.Channel;
			copy.VelocityFactor = this.VelocityFactor;
			copy.LengthFactor = this.LengthFactor;
			copy.MinVelocity = this.MinVelocity;
			copy.MaxVelocity = this.MaxVelocity;
			copy.Transpose = this.Transpose;
			copy.MinPitch = this.MinPitch;
			copy.MaxPitch = this.MaxPitch;
			copy.Color = this.Color;
			copy.Name = this.Name; 
			copy.Version = this.Version;

			// Shallow copy of references; no duplication
			for (int i = 0; i < 4; ++i)
				copy.Articulations[i] = this.Articulations[i]; 

			foreach (OutputEvent oe in OutputEvents)
				copy.OutputEvents.Add(oe.Duplicate());

			return copy;
		}

		private bool IsValidGroup(int group)
			=> group >= 0 && group <= 3;

		private bool IsValidGroup(Articulation art, int group)
			=> IsValidGroup(group) && art.Group == group; 

        public bool AssignArticulation(Articulation art, int group)
		{
			if (!IsValidGroup(art, group))
				return false;
			
			_articulations[group] = art;

			return true;
		}

		public bool UnassignArticulation(int group)
		{
			if (!IsValidGroup(group))
				return false; 

			_articulations[group] = Articulation.Blank;
			return true;
		}

		public bool UnassignArticulation(Articulation tgt)
        {
			for (int i = 0; i < _articulations.Count; ++i)
            {
				if (Articulation.IsBlank(_articulations[i]) && ReferenceEquals(_articulations[i], tgt))
                {
					_articulations[i] = Articulation.Blank;
					return true; 
                }
            }

			return false; 
        }
	}
}
