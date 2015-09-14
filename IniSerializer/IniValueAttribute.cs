using System;

namespace IniSerializer
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IniValueAttribute : Attribute
    {
        public string Key { get; set; }

        public int Position { get; set; }
    }
}