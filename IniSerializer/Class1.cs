using NUnit.Framework;

namespace IniSerializer
{
    public class Given_I_want_to_serialize_an_object_to_an_ini_file
    {
        class When_the_object_is_serialized
        {
            [Test]
            public void Then_the_section_name_is_output_correctly()
            {
                var objToSerialize = new ObjectToSerialize();
                var serializer = new IniSerializer();
                Assert.AreEqual("[Test Section Heading]", serializer.Serialize(objToSerialize));
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
