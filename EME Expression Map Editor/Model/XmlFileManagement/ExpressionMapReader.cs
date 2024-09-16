using System;
using static EME_Expression_Map_Editor.Model.Articulation;
using System.Xml;
using System.Globalization;

namespace EME_Expression_Map_Editor.Model
{
    public static class ExpressionMapReader
    {
        public static XmlReader CreateStandardXmlReader(string filename)
        {
            return XmlReader.Create(filename);
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
                        expmap.Articulations.Add(art);
                        reader.Read();
                    } while (reader.Name.Equals("obj"));
                }
                else if (reader[XmlConstants.Class] == XmlConstants.SoundSlot.XmlClass)
                {
                    do
                    {
                        SoundSlot slot = new SoundSlot();
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

        private static string NextString(XmlReader reader)
        {
            reader.ReadToFollowing(XmlConstants.StringTypename);
            string? val = reader.GetAttribute(XmlConstants.Value);
            reader.Read();
            return val != null ? val : string.Empty;
        }

        private static int NextInteger(XmlReader reader)
        {
            reader.ReadToFollowing(XmlConstants.IntegerTypename);
            Int32.TryParse(reader.GetAttribute(XmlConstants.Value), out int val);
            reader.Read();
            return val;
        }

        private static double NextFloat(XmlReader reader)
        {

            reader.ReadToFollowing(XmlConstants.FloatTypename);
            Double.TryParse(reader.GetAttribute(XmlConstants.Value), NumberStyles.Float, new CultureInfo(XmlConstants.DefaultCultureInfo), out double val); 
            reader.Read();
            return val;
        }

        private static void ReadArticulation(XmlReader reader, Articulation art)
        {
            art.DisplayType = (Display)NextInteger(reader);
            art.ArticulationType = (ArtType)NextInteger(reader);
            art.Symbol = NextInteger(reader);
            art.Text = NextString(reader);
            art.Description = NextString(reader);
            art.Group = NextInteger(reader);
            reader.ReadEndElement();
        }

        private static void ReadOutputEvent(XmlReader reader, OutputEvent oe)
        {
            oe.EventType = NextInteger(reader);
            oe.Data1 = NextInteger(reader);
            oe.Data2 = NextInteger(reader);
            reader.ReadEndElement();
        }

        private static void ReadSoundSlot(XmlReader reader, SoundSlot slot)
        {
            bool ReadUntilAttributeFound(XmlReader r, string attr_name, string tgt_value)
            {
                string? str = r[attr_name];
                if (str != null)
                    return !str.Equals(tgt_value);
                else
                    return true;
            }


            NextInteger(reader); // Throw away 'Status' element; its function is unknown, and is never used
            slot.RemoteKey = NextInteger(reader);

            do
            {
                reader.ReadToFollowing("obj");
            } while (ReadUntilAttributeFound(reader, "class", "PSlotNoteChanger"));

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

            NextInteger(reader); // Throw away version number; unknown function, never used

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
                    slot.OutputEvents.Add(output_event); 
                    reader.Read();
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
                } while (reader.Name.Equals("obj"));
            }

            // Read Name Here 
            reader.ReadToFollowing("member");
            slot.Name = NextString(reader);
            slot.Color = NextInteger(reader);
            reader.ReadEndElement();
        }


    }
}
