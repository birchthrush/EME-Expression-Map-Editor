using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EME_Expression_Map_Editor.Model
{
    public static class XmlConstants
    {
        /*
         * NOTES ON EXPRESSION MAP FILE FORMAT
         * This program relies on reverse engineering the expression map format from maps generated natively from within
         * Cubase. Since the format is entirely undocumented, a certain amount of guesswork is involved and absolute
         * reliability cannot be guaranteed. The format appears to contain a certain amount of data duplication as well as
         * some parameters whose utility or purpose is currently unknown. Details below. 
         * 
         * STRUCTURE
         * Maps are saved as a standard XML document with the following approximate structure: 
         * 
         * [ Header ]
         *      [ Name ]
         *      [ Articulations / SlotVisuals ]
         *          [ Art/SlotVisual 1..n ]
         *      [ SoundSlots / Slots ]
         *             [ Remote Key / PSlotThruTrigger ]
         *              [ Slot Attributes ]
         *             [ OutputEvent 1..n ]
         *         [ Slot Attributes duplicate ]
         *          [ Articulation 1..n duplicate ]
         *      [ Controller ]
         * [ Footer ]
         * 
         * 
         * UNSOLVED MYSTERIES: 
         * - Multiple object parameters include an ID tag. Its use is unknown, changing it or leaving it out appears to
         * have no effect. For now this is handled as a simple pseudo-random numbers. 
         * - Parameter Duplication: for unknown reasons, in Cubase-generated maps every SoundSlot record has its parameters written
         * twice and also contains its assigned Articulations/SlotVisuals in full, leading to considerable data duplication within 
         * the XML file. This structure has been retained for the sake of consistency. The side effect is many Articulations will 
         * be read multiple times, to be cleaned up at a later stage ExpressionMap.RemapArticulations() function. 
         * 
         * UNKNOWN PARAMETERS: 
         * The following parameters appear in Cubase-generated expression maps, but their purpose and use is unknown. 
         * They are presently discarded on read, and written with arbitrary default values derives from Cubase-generated maps. 
         *      - Ownership (appears for every object record) 
         *      - Status
         *      - Controller (object record near end of file)
         *       
         */

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
