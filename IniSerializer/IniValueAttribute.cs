using System;

namespace IniSerializer
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IniValueAttribute : Attribute
    {
        public readonly string Key;

        public IniValueAttribute(string key)
        {
            Key = key;
        }

        public int Position { get; set; }
    }
}