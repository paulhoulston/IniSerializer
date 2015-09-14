using System;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace IniSerializer
{
    public class Given_I_want_to_serialize_an_object_with_no_values
    {
        class When_the_object_is_serialized
        {
            private readonly string[] _serializedOutput;

            public When_the_object_is_serialized()
            {
                var objToSerialize = new ObjectToSerialize();
                var serializer = new IniSerializer();
                _serializedOutput = serializer.Serialize(objToSerialize).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            }

            [Test]
            public void Then_the_section_name_is_output_on_the_first_line()
            {
                Assert.AreEqual("[Test Section Heading]", _serializedOutput[0]);
            }

            [Test]
            public void And_no_other_lines_are_added()
            {
                Assert.AreEqual(1, _serializedOutput.Length);
            }
        }
    }

    public class Given_I_want_to_serialize_an_object_with_one_property
    {
        class When_the_object_is_serialized
        {
            private readonly string[] _serializedOutput;

            public When_the_object_is_serialized()
            {
                var objToSerialize = new ObjectToSerializeWithOneProperty { Item1 = "Value Of Item 1" };
                var serializer = new IniSerializer();
                _serializedOutput = serializer.Serialize(objToSerialize).Split(new []{'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            }

            [Test]
            public void Then_the_section_name_is_output_on_the_first_line()
            {
                Assert.AreEqual("[Section Heading for section with properties]", _serializedOutput[0]);
            }

            [Test]
            public void And_the_property_is_added_to_the_second_line_as_a_key_value_pair_seperated_by_and_equals_sign()
            {
                Assert.AreEqual("TheItem=Value Of Item 1", _serializedOutput[1]);
            }
        }
    }

    class IniSectionAttribute : Attribute
    {
        public readonly string SectionName;

        public IniSectionAttribute(string sectionName)
        {
            SectionName = sectionName;
        }
    }

    [IniSection("[Test Section Heading]")]
    class ObjectToSerialize
    {
    }

    [IniSection("[Section Heading for section with properties]")]
    class ObjectToSerializeWithOneProperty
    {
        public string Item1 { get; set; }
    }

    class IniSerializer
    {
        public string Serialize<T>(T objToSerialize)
        {
            var tType = typeof(T);
            var output = new StringBuilder();

            var iniSection = (IniSectionAttribute)tType.GetCustomAttribute(typeof(IniSectionAttribute));
            output.AppendLine(iniSection.SectionName);

            foreach (var propertyInfo in tType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                output.AppendLine(string.Format("{0}={1}", propertyInfo.Name, propertyInfo.GetValue(objToSerialize)));
            }
            return output.ToString();
        }
    }

}
