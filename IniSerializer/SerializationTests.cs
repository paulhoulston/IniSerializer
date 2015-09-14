using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace IniSerializer
{
    public class Given_I_want_to_serialize_an_object_with_no_properties
    {
        private class When_the_serialized_object_does_not_implement_the_IniSectionAttribute_attribute
        {
            private class ObjectToSerialize
            {
            }

            [Test, ExpectedException(typeof (MustImplementIniSectionAttributeException))]
            public void Then_an_MustImplementIniSectionAttributeException_exception_is_thrown()
            {
                new IniSerializer<ObjectToSerialize>().Serialize(new ObjectToSerialize());
            }
        }

        private class When_the_object_is_serialized
        {
            private readonly string[] _serializedOutput;

            [IniSection("[Test Section Heading]")]
            private class ObjectToSerialize
            {
            }

            public When_the_object_is_serialized()
            {
                _serializedOutput
                    = new IniSerializer<ObjectToSerialize>()
                        .Serialize(new ObjectToSerialize())
                        .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
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
        private class When_the_object_is_serialized
        {
            [IniSection("[Section Heading for section with one property]")]
            private class ObjectToSerializeWithOneProperty
            {
                [IniValue("TheItem")]
                public string Item1 { get; set; }
            }

            private readonly string[] _serializedOutput;

            public When_the_object_is_serialized()
            {
                _serializedOutput =
                    new IniSerializer<ObjectToSerializeWithOneProperty>()
                        .Serialize(
                            new ObjectToSerializeWithOneProperty
                            {
                                Item1 = "Value Of Item 1"
                            })
                        .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            }

            [Test]
            public void Then_the_section_name_is_output_on_the_first_line()
            {
                Assert.AreEqual("[Section Heading for section with one property]", _serializedOutput[0]);
            }

            [Test]
            public void And_the_property_is_added_to_the_second_line_as_a_key_value_pair_seperated_by_and_equals_sign()
            {
                Assert.AreEqual("TheItem=Value Of Item 1", _serializedOutput[1]);
            }
        }
    }

    public class Given_I_want_to_serialize_an_object_with_multiple_properties
    {
        private class When_the_object_is_serialized
        {
            [IniSection("[Section Heading for section with multiple properties]")]
            private class ObjectToSerializeWithOneProperty
            {
                [IniValue("AStringValue")]
                public string StringValue { get; set; }

                [IniValue("AnIntValue")]
                public int IntegerValue { get; set; }

                public string PropertyToIgnore { get; set; }
            }

            private readonly string[] _serializedOutput;

            public When_the_object_is_serialized()
            {
                _serializedOutput =
                    new IniSerializer<ObjectToSerializeWithOneProperty>()
                        .Serialize(
                            new ObjectToSerializeWithOneProperty
                            {
                                StringValue = "Value Of Item 1",
                                IntegerValue = 3,
                                PropertyToIgnore = "StringValueWithNoAttribute"
                            })
                        .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            }

            [Test]
            public void Then_the_section_name_is_output_on_the_first_line()
            {
                Assert.AreEqual("[Section Heading for section with multiple properties]", _serializedOutput[0]);
            }

            [Test]
            public void And_the_string_value_property_is_added_to_as_a_key_value_pair_seperated_by_and_equals_sign()
            {
                Assert.NotNull(_serializedOutput.SingleOrDefault(str => str.Equals("AnIntValue=3")));
            }

            [Test]
            public void And_the_int_value_property_is_added_to_as_a_key_value_pair_seperated_by_and_equals_sign()
            {
                Assert.NotNull(_serializedOutput.SingleOrDefault(str => str.Equals("AStringValue=Value Of Item 1")));
            }

            [Test]
            public void And_the_property_not_decorated_with_the_IniValueAttribute_is_ignored()
            {
                Assert.IsNull(_serializedOutput.SingleOrDefault(str => str.Contains("StringValueWithNoAttribute")));
            }
        }
    }
    public class MustImplementIniSectionAttributeException : Exception
    {
        public MustImplementIniSectionAttributeException()
            : base("The object to serialize must be decorated with the 'IniSectionAttribute' class attribute")
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    internal class IniSectionAttribute : Attribute
    {
        public readonly string SectionName;

        public IniSectionAttribute(string sectionName)
        {
            SectionName = sectionName;
        }
    }

    internal class IniValueAttribute : Attribute
    {
        public readonly string Key;

        public IniValueAttribute(string key)
        {
            Key = key;
        }
    }

    class IniSerializer<T>
    {
        private readonly Type _tType;

        public IniSerializer()
        {
            _tType = typeof (T);
        }

        public string Serialize(T objToSerialize)
        {
            var iniFileLines = new List<string>
            {
                getSectionName()
            };
            iniFileLines.AddRange(getSectionValues(objToSerialize));

            return string.Join(Environment.NewLine, iniFileLines);
        }

        private string getSectionName()
        {
            var iniSection = _tType.GetCustomAttribute(typeof(IniSectionAttribute)) as IniSectionAttribute;
            if (iniSection == null)
                throw new MustImplementIniSectionAttributeException();
            return iniSection.SectionName;
        }

        private IEnumerable<string> getSectionValues(T objToSerialize)
        {
            return from pi in _tType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                let iniValue = pi.GetCustomAttribute(typeof (IniValueAttribute)) as IniValueAttribute
                where iniValue != null
                select string.Format("{0}={1}",
                    iniValue.Key,
                    pi.GetValue(objToSerialize));
        }
    }

}