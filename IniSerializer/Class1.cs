using NUnit.Framework;

namespace IniSerializer
{
    public class Given_I_want_to_serialize_an_object_with_no_values_to_an_ini_file
    {
        class When_the_object_is_serialized
        {
            [Test]
            public void Then_the_section_name_is_output_on_the_first_line()
            {
                var objToSerialize = new ObjectToSerialize();
                var serializer = new IniSerializer();
                Assert.AreEqual("[Test Section Heading]", serializer.Serialize(objToSerialize).Split('\r', '\n')[0]);
            }
        }
    }

    class ObjectToSerialize
    {
    }

    class IniSerializer
    {
        public string Serialize(ObjectToSerialize objToSerialize)
        {
            return "[Test Section Heading]";
        }
    }

}
