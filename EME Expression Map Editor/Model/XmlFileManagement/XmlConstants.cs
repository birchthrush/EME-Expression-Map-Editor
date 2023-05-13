using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EME_Expression_Map_Editor.Model.XmlFileManagement
{
    public static class XmlConstants
    {
        public static readonly string FileExtension = ".expressionmap";

        public static readonly string DefaultID = "872612400";
        public static readonly string DefaultCultureInfo = "en-US";
        public static readonly string IntegerTypename = "int";
        public static readonly string FloatTypename = "float";
        public static readonly string StringTypename = "string";
        public static readonly string Name = "name";
        public static readonly string Value = "value";
        public static readonly string Wide = "wide";
        public static readonly string DefaultWide = "true";
        public static readonly string ListType = "list";
        public static readonly string Type = "type";
        public static readonly string Object = "obj";
        public static readonly string Class = "class";
        public static readonly string ID = "ID";
        public static readonly string Member = "member";
        public static readonly string Ownership = "ownership";
        public static readonly string Status = "status";

        public static class Articulation
        {
            public static readonly string XmlClass = "USlotVisuals";
            public static readonly string MemberName = "slotvisuals";
            public static readonly string DisplayType = "displaytype";
            public static readonly string ArticulationType = "articulationtype";
            public static readonly string Symbol = "symbol";
            public static readonly string Text = "text";
            public static readonly string Description = "description";
            public static readonly string Group = "group";
        }

        public static class OutputEvent
        {
            public static readonly string XmlClass = "POutputEvent";
            public static readonly string XmlEventType = "status";
            public static readonly string XmlData1 = "data1";
            public static readonly string XmlData2 = "data2";
        }

        public static class SoundSlot
        {
            public static readonly string MemberName = "slots";
            public static readonly string Channel = "channel";
            public static readonly string VelocityFactor = "velocityFact";
            public static readonly string LengthFactor = "lengthFact";
            public static readonly string MinVelocity = "minVelocity";
            public static readonly string MaxVelocity = "maxVelocity";
            public static readonly string Transpose = "transpose";
            public static readonly string MinPitch = "minPitch";
            public static readonly string MaxPitch = "maxPitch";
            public static readonly string XmlClass = "PSoundSlot";
            public static readonly string RemoteKeysXmlClass = "PSlotThruTrigger";
        }

        public static class ExpressionMap
        {
            public const string StartElement = "InstrumentMap";
            public const string Name = "name";
            public const string Controller = "controller";
        }
    }
}
