using System;
using System.Globalization;
using System.Xml;

namespace EME_Expression_Map_Editor.Model
{
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

        public static void WriteExpressionMap(XmlWriter writer, ExpressionMap expmap)
        {
            // Write document header and ExpressionMap name
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

        private static string GenerateID()
        {
            return new Random().Next().ToString();
        }

        private static void WriteVariable(XmlWriter writer, string name, int value)
        {
            writer.WriteStartElement(XmlConstants.IntegerTypename);
            writer.WriteAttributeString(XmlConstants.Name, name);
            writer.WriteAttributeString(XmlConstants.Value, value.ToString());
            writer.WriteEndElement();
        }

        private static void WriteVariable(XmlWriter writer, string name, double value)
        {
            writer.WriteStartElement(XmlConstants.FloatTypename);
            writer.WriteAttributeString(XmlConstants.Name, name);
            writer.WriteAttributeString(XmlConstants.Value, value.ToString(new CultureInfo(XmlConstants.DefaultCultureInfo)));
            writer.WriteEndElement();
        }

        private static void WriteVariable(XmlWriter writer, string name, string value)
        {
            writer.WriteStartElement(XmlConstants.StringTypename);
            writer.WriteAttributeString(XmlConstants.Name, name);
            writer.WriteAttributeString(XmlConstants.Value, value);
            writer.WriteAttributeString(XmlConstants.Wide, XmlConstants.DefaultWide);   // Wtf is this? 
            writer.WriteEndElement();
        }

        private static void WriteStartList(XmlWriter writer)
        {
            writer.WriteStartElement(XmlConstants.ListType);
            writer.WriteAttributeString(XmlConstants.Name, XmlConstants.Object);
            writer.WriteAttributeString(XmlConstants.Type, XmlConstants.Object);
        }

        private static void WriteEndList(XmlWriter writer)
        {
            writer.WriteEndElement();
        }

        private static void WriteStartObject(XmlWriter writer, string class_name, string id, string name)
        {
            writer.WriteStartElement(XmlConstants.Object);
            writer.WriteAttributeString(XmlConstants.Class, class_name);
            if (!String.IsNullOrEmpty(name))
                writer.WriteAttributeString(XmlConstants.Name, name);
            writer.WriteAttributeString(XmlConstants.ID, id);
        }

        private static void WriteStartObject(XmlWriter writer, string class_name, string id)
            => WriteStartObject(writer, class_name, id, string.Empty);

        private static void WriteEndObject(XmlWriter writer)
        {
            writer.WriteEndElement();
        }

        private static void WriteStartMember(XmlWriter writer, string name, int ownership)
        {
            writer.WriteStartElement(XmlConstants.Member);
            writer.WriteAttributeString(XmlConstants.Name, name);
            if (ownership >= 0)
                WriteVariable(writer, XmlConstants.Ownership, ownership);
        }

        private static void WriteEndMember(XmlWriter writer)
        {
            writer.WriteEndElement();
        }

        private static void WriteArticulation(XmlWriter writer, Articulation art)
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

        private static void WriteOutputEvent(XmlWriter writer, OutputEvent oe)
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
        private static void WriteSoundSlot(XmlWriter writer, SoundSlot slot)
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

            WriteSlotAttributes(writer, slot); // WRITE ALL ATTRIBUTES AGAIN (data duplication for unknown reasons; see documentation) 
            WriteVariable(writer, "key", -1); // Unknown parameter
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
    }
}
