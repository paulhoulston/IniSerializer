using System;
using System.Linq;
using NUnit.Framework;

namespace IniSerializer
{
    public class Given_I_want_to_serialize_an_object_with_no_properties
    {
        private class When_the_serialized_object_does_not_implement_the_IHaveASectionName_interface
        {
            private class ObjectToSerialize
            {
            }

            [Test]
            public void Then_the_name_of_the_class_is_used_as_the_section_heading()
            {
                var serializedOutput =
                    new IniSerializer<ObjectToSerialize>()
                        .Serialize(
                            new ObjectToSerialize());

                Assert.AreEqual("[ObjectToSerialize]", serializedOutput);
            }
        }

        private class When_the_object_is_serialized
        {
            private readonly string[] _serializedOutput;

            private class ObjectToSerialize : IHaveASectionName
            {
                public ObjectToSerialize(string sectionName)
                {
                    SectionName = sectionName;
                }

                public string SectionName { get; private set; }
            }

            public When_the_object_is_serialized()
            {
                _serializedOutput
                    = new IniSerializer<ObjectToSerialize>()
                        .Serialize(new ObjectToSerialize("Test Section Heading"))
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
            private class ObjectToSerialize : IHaveASectionName
            {
                public ObjectToSerialize(string sectionName)
                {
                    SectionName = sectionName;
                }

                [IniValue("TheItem")]
                public string Item1 { get; set; }

                public string SectionName { get; private set; }
            }

            private readonly string[] _serializedOutput;

            public When_the_object_is_serialized()
            {
                _serializedOutput =
                    new IniSerializer<ObjectToSerialize>()
                        .Serialize(
                            new ObjectToSerialize("Section Heading for section with one property")
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
            private class ObjectToSerializeWithOneProperty : IHaveASectionName
            {
                public ObjectToSerializeWithOneProperty(string sectionName)
                {
                    SectionName = sectionName;
                }

                [IniValue("AStringValue")]
                public string StringValue { get; set; }

                [IniValue("AnIntValue")]
                public int IntegerValue { get; set; }

                public string PropertyToIgnore { get; set; }

                public string SectionName { get; private set; }
            }

            private readonly string[] _serializedOutput;

            public When_the_object_is_serialized()
            {
                _serializedOutput =
                    new IniSerializer<ObjectToSerializeWithOneProperty>()
                        .Serialize(
                            new ObjectToSerializeWithOneProperty("Section Heading for section with multiple properties")
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

        private class When_the_properties_on_the_object_to_be_serialized_have_the_order_set
        {
            private readonly string[] _serializedOutput;

            private class ObjectToSerialize : IHaveASectionName
            {
                public ObjectToSerialize(string sectionName)
                {
                    SectionName = sectionName;
                }

                [IniValue("Item1", Position = 3)]
                public string Item1 { get; set; }

                [IniValue("Item2", Position = 1)]
                public string Item2 { get; set; }

                [IniValue("Item3", Position = 2)]
                public string Item3 { get; set; }

                public string SectionName { get; private set; }
            }

            public When_the_properties_on_the_object_to_be_serialized_have_the_order_set()
            {
                _serializedOutput =
                    new IniSerializer<ObjectToSerialize>().Serialize(
                        new ObjectToSerialize("Section Heading for section with ordered properties")
                        {
                            Item1 = "Position 3",
                            Item2 = "Position 1",
                            Item3 = "Position 2",
                        }).Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            }

            [Test]
            public void Then_the_section_name_is_output_on_the_first_line()
            {
                Assert.AreEqual("[Section Heading for section with ordered properties]", _serializedOutput[0]);
            }

            [Test]
            public void Then_the_objects_are_ordered_correctly()
            {
                Assert.AreEqual("Item2=Position 1", _serializedOutput[1]);
                Assert.AreEqual("Item3=Position 2", _serializedOutput[2]);
                Assert.AreEqual("Item1=Position 3", _serializedOutput[3]);
            }
        }
    }

    public class Given_I_want_to_serialize_an_enumerable_of_objects
    {
        private class ObjectToSerialize : IHaveASectionName
        {
            public ObjectToSerialize(string sectionName)
            {
                SectionName = sectionName;
            }

            [IniValue("Item1", Position = 1)]
            public string Item1 { get; set; }

            [IniValue("Item2", Position = 2)]
            public string Item2 { get; set; }

            [IniValue("Item3", Position = 3)]
            public string Item3 { get; set; }

            public string SectionName { get; private set; }
        }

        public class When_there_is_one_item
        {
            [Test]
            public void Then_the_output_is_created_correctly()
            {
                const string expected =
                    "[Section]\r\n" +
                    "Item1=Value_1_1\r\n" +
                    "Item2=Value_1_2\r\n" +
                    "Item3=Value_1_3";

                var serializedOutput =
                    new IniSerializer<ObjectToSerialize>().Serialize(
                    new[]
                    {
                        new ObjectToSerialize("Section")
                        {
                            Item1 = "Value_1_1",
                            Item2 = "Value_1_2",
                            Item3 = "Value_1_3",
                        }
                    });
                Assert.AreEqual(expected, serializedOutput);
            }
        }

        public class When_there_are_multiple_items
        {
            [Test]
            public void Then_the_output_is_created_correctly()
            {
                const string expected =
                    "[Section_1]\r\n" +
                    "Item1=Value_1_1\r\n" +
                    "Item2=Value_1_2\r\n" +
                    "Item3=Value_1_3\r\n" +
                    "[Section_2]\r\n" +
                    "Item1=Value_2_1\r\n" +
                    "Item2=Value_2_2\r\n" +
                    "Item3=Value_2_3";

                var serializedOutput =
                    new IniSerializer<ObjectToSerialize>().Serialize(
                    new[]
                    {
                        new ObjectToSerialize("Section_1")
                        {
                            Item1 = "Value_1_1",
                            Item2 = "Value_1_2",
                            Item3 = "Value_1_3",
                        },
                        new ObjectToSerialize("Section_2")
                        {
                            Item1 = "Value_2_1",
                            Item2 = "Value_2_2",
                            Item3 = "Value_2_3",
                        }
                    });
                Assert.AreEqual(expected, serializedOutput);
            }
        }
    }
}