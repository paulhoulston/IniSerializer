using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IniSerializer
{
    public static class IniSerializer
    {
        private const string SectionHeadingFormat = "[{0}]";

        public interface IHaveASectionName
        {
            string SectionName { get; }
        }

        public static string Serialize<T>(T objToSerialize)
        {
            // If the type is enumerable, iterate through the children
            if (objToSerialize is IEnumerable<object>)
            {
                return aggregateLines(((IEnumerable<object>)objToSerialize).Select(Serialize));
            }
            
            var iniFileLines = new List<string>();
            iniFileLines.Add(getSectionName(objToSerialize));
            iniFileLines.AddRange(getSectionValues(objToSerialize));
            return aggregateLines(iniFileLines);
        }
        
        private static string aggregateLines(IEnumerable<string> lines)
        {
            return string.Join(Environment.NewLine, lines);
        }

        private static string getSectionName<T>(T objToSerialize)
        {
            var name = objToSerialize as IHaveASectionName;
            return string.Format(SectionHeadingFormat, name == null ? objToSerialize.GetType().Name : name.SectionName);
        }

        private static IEnumerable<string> getSectionValues<T>(T objToSerialize)
        {
            return from pi in objToSerialize.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                let iniValue = pi.GetCustomAttribute(typeof (IniValueAttribute)) as IniValueAttribute
                where iniValue != null
                orderby iniValue.Position
                select string.Format("{0}={1}",
                    iniValue.Key,
                    pi.GetValue(objToSerialize));
        }
    }
}