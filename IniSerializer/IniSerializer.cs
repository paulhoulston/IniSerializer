using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IniSerializer
{
    public interface IHaveASectionName
    {
        string SectionName { get; }
    }

    public class IniSerializer<T>
    {
        private readonly Type _tType;

        public IniSerializer()
        {
            _tType = typeof (T);
        }

        public string Serialize(IEnumerable<T> objToSerialize)
        {
            return string.Join(Environment.NewLine, objToSerialize.Select(Serialize));
        }

        public string Serialize(T objToSerialize)
        {
            var iniFileLines = new List<string>
            {
                getSectionName(objToSerialize)
            };
            iniFileLines.AddRange(getSectionValues(objToSerialize));

            return string.Join(Environment.NewLine, iniFileLines);
        }

        private string getSectionName(T objToSerialize)
        {
            string sectionName;
            if (objToSerialize is IHaveASectionName)
            {
                sectionName = ((IHaveASectionName) objToSerialize).SectionName;
            }
            else
            {
                sectionName = _tType.Name;
            }
            return string.Format("[{0}]", sectionName);
        }

        private IEnumerable<string> getSectionValues(T objToSerialize)
        {
            return from pi in _tType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                let iniValue = pi.GetCustomAttribute(typeof (IniValueAttribute)) as IniValueAttribute
                where iniValue != null
                orderby iniValue.Position
                select string.Format("{0}={1}",
                    iniValue.Key,
                    pi.GetValue(objToSerialize));
        }
    }
}