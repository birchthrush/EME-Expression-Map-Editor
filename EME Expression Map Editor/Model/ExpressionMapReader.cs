using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static EME_Expression_Map_Editor.Model.Articulation;
using static EME_Expression_Map_Editor.Model.XmlConstants;
using static System.Net.Mime.MediaTypeNames;

namespace EME_Expression_Map_Editor.Model
{
    public static class ExpressionMapReader
    {
        public static XmlReader CreateStandardXmlReader(string filename)
        {
            return XmlReader.Create(filename);
        }

        public static string NextString(XmlReader reader)
        {
            reader.ReadToFollowing(XmlConstants.StringTypename);
            string? val = reader.GetAttribute(XmlConstants.Value);
            reader.Read();
            return val != null ? val : string.Empty; 

            /*
            reader.ReadToFollowing(XmlConstants.StringTypename);
            string val = reader[Value];
            reader.Read();
            return val;
            */
        }

        public static int NextInteger(XmlReader reader)
        {
            reader.ReadToFollowing(XmlConstants.IntegerTypename);
            Int32.TryParse(reader.GetAttribute(XmlConstants.Value), out int val);
            reader.Read();
            return val;

            /*
            reader.ReadToFollowing(XmlConstants.IntegerTypename);
            int val = Int32.Parse(reader[XmlConstants.Value]);
            reader.Read();
            return val;
            */
        }

        public static double NextFloat(XmlReader reader)
        {
            reader.ReadToFollowing(XmlConstants.FloatTypename);
            Double.TryParse(reader.GetAttribute(XmlConstants.Value), out double val);
            reader.Read(); 
            return val;

            /*
            reader.ReadToFollowing(XmlConstants.FloatTypename);
            double val = Double.Parse(reader[XmlConstants.Value], new CultureInfo(XmlConstants.DefaultCultureInfo));
            reader.Read();
            return val;
            */
        }

        public static void ReadArticulation(XmlReader reader, Articulation art)
        {
            art.DisplayType = (Display)NextInteger(reader);
            art.ArticulationType = (ArtType)NextInteger(reader);
            art.Symbol = NextInteger(reader);
            art.Text = NextString(reader);
            art.Description = NextString(reader);
            art.Group = NextInteger(reader);
            reader.ReadEndElement();
        }

        public static void ReadOutputEvent(XmlReader reader, OutputEvent oe)
        {
            oe.EventType = NextInteger(reader);
            oe.Data1 = NextInteger(reader);
            oe.Data2 = NextInteger(reader);
            reader.ReadEndElement();
        }

        public static void ReadSoundSlot(XmlReader reader, SoundSlot slot)
        {
            bool ReadUntilAttributeFound(XmlReader r, string attr_name, string tgt_value)
            {
                string? str = r[attr_name];
                if (str != null)
                    return !str.Equals(tgt_value);
                else
                    return true; 
            }

            
            NextInteger(reader); // Throwaway 'Status' element
            slot.RemoteKey = NextInteger(reader);

            do
            {
                reader.ReadToFollowing("obj");
            } while (ReadUntilAttributeFound(reader, "class", "PSlotNoteChanger")); 
            
            //while (!reader["class"].Equals("PSlotNoteChanger"));


            // SoundSlot Attributes
            slot.Channel = NextInteger(reader);
            slot.VelocityFactor = NextFloat(reader);
            slot.LengthFactor = NextFloat(reader);
            slot.MinVelocity = NextInteger(reader);
            slot.MaxVelocity = NextInteger(reader);
            slot.Transpose = NextInteger(reader);
            slot.MinPitch = NextInteger(reader);
            slot.MaxPitch = NextInteger(reader);

            //Find Output Mapping: 
            do
            {
                reader.ReadToFollowing("member");
            } while (ReadUntilAttributeFound(reader, "name", "midiMessages")); 
            
            //while (!reader["name"].Equals("midiMessages"));

            NextInteger(reader); // Throw away version number

            do
            {
                reader.Read();
            } while (!reader.IsStartElement());

            if (reader.Name.Equals("list"))
            {
                reader.Read();
                do
                {
                    OutputEvent output_event = new OutputEvent();
                    ReadOutputEvent(reader, output_event); 
                    slot.AddOutputEvent(output_event);
                    reader.Read(); 

                    /*
                    output_event.ReadXml(reader);
                    AddOutputEvent(output_event);
                    reader.Read();
                    */
                } while (reader.Name.Equals("obj"));
                reader.ReadEndElement(); // End of list
            }

            // Read Soundslots
            reader.ReadToFollowing("member");
            NextInteger(reader); // Throw away ownership
            reader.Read();
            if (reader.Name.Equals("list"))
            {
                reader.Read();
                do
                {
                    Articulation art = new Articulation();
                    ReadArticulation(reader, art);
                    slot.AssignArticulation(art, art.Group);
                    reader.Read(); 

                    /*
                    Articulation art = new Articulation();
                    art.ReadXml(reader);
                    AssignArticulation(art, art.Group);
                    reader.Read();
                    */
                } while (reader.Name.Equals("obj"));
            }

            // Read Name Here 
            reader.ReadToFollowing("member");
            slot.Name = NextString(reader);
            slot.Color = NextInteger(reader);
            reader.ReadEndElement();
        }

        public static void ReadExpressionMap(XmlReader reader, ExpressionMap expmap)
        {
            reader.ReadToFollowing("string");
            reader.MoveToFirstAttribute();
            reader.MoveToNextAttribute();
            expmap.Name = reader.Value;

            while (!reader.EOF)
            {
                reader.ReadToFollowing(XmlConstants.Object);
                if (reader[XmlConstants.Class] == XmlConstants.Articulation.XmlClass)
                {
                    do
                    {
                        Articulation art = new Articulation();
                        ReadArticulation(reader, art);
                        //art.ReadXml(reader);
                        expmap.Articulations.Add(art);
                        reader.Read();
                    } while (reader.Name.Equals("obj"));
                }
                else if (reader[XmlConstants.Class] == XmlConstants.SoundSlot.XmlClass)
                {
                    do
                    {
                        SoundSlot slot = new SoundSlot();
                        //slot.ReadXml(reader);
                        ReadSoundSlot(reader, slot);
                        expmap.SoundSlots.Add(slot);
                        reader.Read();
                    } while (reader.Name.Equals("obj"));
                }

                // Read controllers here? 
            }

            // Remap Articulations
            expmap.RemapArticulations();
        }
    }

    public static class ExpressionMapWriter
    {
        public static XmlWriter CreateStandardXmlWriter(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;

            if (String.IsNullOrEmpty(filename))
                return XmlWriter.Create(Console.Out, settings);
            else
                return XmlWriter.Create(filename, settings);
        }

        public static void WriteVariable(XmlWriter writer, string name, int value)
        {
            writer.WriteStartElement(XmlConstants.IntegerTypename);
            writer.WriteAttributeString(XmlConstants.Name, name);
            writer.WriteAttributeString(XmlConstants.Value, value.ToString());
            writer.WriteEndElement();
        }

        public static void WriteVariable(XmlWriter writer, string name, double value)
        {
            writer.WriteStartElement(XmlConstants.FloatTypename);
            writer.WriteAttributeString(XmlConstants.Name, name);
            writer.WriteAttributeString(XmlConstants.Value, value.ToString(new CultureInfo(XmlConstants.DefaultCultureInfo)));
            writer.WriteEndElement();
        }

        public static void WriteVariable(XmlWriter writer, string name, string value)
        {
            writer.WriteStartElement(XmlConstants.StringTypename);
            writer.WriteAttributeString(XmlConstants.Name, name);
            writer.WriteAttributeString(XmlConstants.Value, value);
            writer.WriteAttributeString(XmlConstants.Wide, XmlConstants.DefaultWide);   // Wtf is this? 
            writer.WriteEndElement();
        }

        public static void WriteStartList(XmlWriter writer)
        {
            writer.WriteStartElement(XmlConstants.ListType);
            writer.WriteAttributeString(XmlConstants.Name, XmlConstants.Object);
            writer.WriteAttributeString(XmlConstants.Type, XmlConstants.Object);
        }

        public static void WriteEndList(XmlWriter writer)
        {
            writer.WriteEndElement();
        }

        public static void WriteStartObject(XmlWriter writer, string class_name, string id, string name)
        {
            writer.WriteStartElement(XmlConstants.Object);
            writer.WriteAttributeString(XmlConstants.Class, class_name);
            if (!String.IsNullOrEmpty(name))
                writer.WriteAttributeString(XmlConstants.Name, name);
            writer.WriteAttributeString(XmlConstants.ID, id);
        }

        public static void WriteStartObject(XmlWriter writer, string class_name, string id)
            => WriteStartObject(writer, class_name, id, string.Empty); 

        public static void WriteEndObject(XmlWriter writer)
        {
            writer.WriteEndElement();
        }

        public static void WriteStartMember(XmlWriter writer, string name, int ownership)
        {
            writer.WriteStartElement(XmlConstants.Member);
            writer.WriteAttributeString(XmlConstants.Name, name);
            if (ownership >= 0)
                WriteVariable(writer, XmlConstants.Ownership, ownership);
        }

        public static void WriteEndMember(XmlWriter writer)
        {
            writer.WriteEndElement();
        }

        public static void WriteArticulation(XmlWriter writer, Articulation art)
        {
            WriteStartObject(writer, XmlConstants.Articulation.XmlClass, GenerateID());
            WriteVariable(writer, XmlConstants.Articulation.DisplayType, (int)art.DisplayType);
            WriteVariable(writer, XmlConstants.Articulation.ArticulationType, (int)art.ArticulationType);
            WriteVariable(writer, XmlConstants.Articulation.Symbol, art.Symbol);
            WriteVariable(writer, XmlConstants.Articulation.Text, art.Text);
            WriteVariable(writer, XmlConstants.Articulation.Description, art.Description);
            WriteVariable(writer, XmlConstants.Articulation.Group, art.Group);
            WriteEndObject(writer);
        }

        public static void WriteOutputEvent(XmlWriter writer, OutputEvent oe)
        {
            WriteStartObject(writer, XmlConstants.OutputEvent.XmlClass, GenerateID());
            WriteVariable(writer, XmlConstants.OutputEvent.XmlEventType, oe.EventType);
            WriteVariable(writer, XmlConstants.OutputEvent.XmlData1, oe.Data1);
            WriteVariable(writer, XmlConstants.OutputEvent.XmlData2, oe.Data2);
            WriteEndObject(writer);
        }


        // Slot Attributes
        private static void WriteSlotAttributes(XmlWriter writer, SoundSlot slot)
        {
            WriteVariable(writer, XmlConstants.SoundSlot.Channel, slot.Channel);
            WriteVariable(writer, XmlConstants.SoundSlot.VelocityFactor, slot.VelocityFactor);
            WriteVariable(writer, XmlConstants.SoundSlot.LengthFactor, slot.LengthFactor);
            WriteVariable(writer, XmlConstants.SoundSlot.MinVelocity, slot.MinVelocity);
            WriteVariable(writer, XmlConstants.SoundSlot.MaxVelocity, slot.MaxVelocity);
            WriteVariable(writer, XmlConstants.SoundSlot.Transpose, slot.Transpose);
            WriteVariable(writer, XmlConstants.SoundSlot.MinPitch, slot.MinPitch);
            WriteVariable(writer, XmlConstants.SoundSlot.MaxPitch, slot.MaxPitch);
        }
        public static void WriteSoundSlot(XmlWriter writer, SoundSlot slot)
        {
            WriteStartObject(writer, "PSoundSlot", GenerateID());

            // Remote Keys: 
            WriteStartObject(writer, "PSlotThruTrigger", GenerateID(), "remote");
            WriteVariable(writer, "status", 144);
            WriteVariable(writer, "data1", slot.RemoteKey);
            WriteEndObject(writer); // End of remote keys

            // PSlot Midi Action: 
            WriteStartObject(writer, "PSlotMidiAction", GenerateID(), "action");
            WriteVariable(writer, "version", slot.Version); 
            WriteStartMember(writer, "noteChanger", 1);
            WriteStartList(writer);
            WriteStartObject(writer, "PSlotNoteChanger", GenerateID());
            WriteSlotAttributes(writer, slot);
            WriteEndObject(writer);
            WriteEndList(writer);
            WriteEndMember(writer);

            // Midi Messages: 
            WriteStartMember(writer, "midiMessages", 1);
            if (slot.OutputEvents.Count != 0)
            {
                WriteStartList(writer);
                foreach (var output_event in slot.OutputEvents)
                    WriteOutputEvent(writer, output_event);
                WriteEndList(writer);
            }
            WriteEndMember(writer);

            WriteSlotAttributes(writer, slot); // WRITE ALL ATTRIBUTES AGAIN (for some reason?) 
            WriteVariable(writer, "key", -1); // Wtf is this? 
            WriteEndObject(writer); // End of PSlotMidiAction

            WriteStartMember(writer, "sv", 2);


            int art_count = 0;
            foreach (Articulation art in slot.Articulations)
                if (!Articulation.IsBlank(art))
                    ++art_count;

            if (art_count != 0)
            {
                WriteStartList(writer);
                foreach (var art in slot.Articulations)
                    if (!Articulation.IsBlank(art))
                        WriteArticulation(writer, art);
                WriteEndList(writer);
            }
            WriteEndMember(writer);


            WriteStartMember(writer, "name", -1);
            WriteVariable(writer, "s", slot.Name);
            WriteEndMember(writer);

            WriteVariable(writer, "color", slot.Color);

            WriteEndObject(writer); // End object 
        }

        public static void WriteExpressionMap(XmlWriter writer, ExpressionMap expmap)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement(XmlConstants.ExpressionMap.StartElement);
            WriteVariable(writer, XmlConstants.ExpressionMap.Name, expmap.Name);

            // Write Articulations
            WriteStartMember(writer, XmlConstants.Articulation.MemberName, 1);
            WriteStartList(writer);
            // --- Iterate Articulations --- 
            foreach (Articulation art in expmap.Articulations)
                WriteArticulation(writer, art);
            WriteEndList(writer);
            WriteEndMember(writer);


            WriteStartMember(writer, XmlConstants.SoundSlot.MemberName, 1);
            WriteStartList(writer);
            // --- Iterate Sound Slots ---
            foreach (SoundSlot slot in expmap.SoundSlots)
                WriteSoundSlot(writer, slot);  
            WriteEndList(writer);
            WriteEndMember(writer);


            WriteStartMember(writer, XmlConstants.ExpressionMap.Controller, 1);
            // wtf is this? 
            WriteEndMember(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush(); 
        }

        public static string GenerateID()
        {
            return new Random().Next().ToString();
        }
    }

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
