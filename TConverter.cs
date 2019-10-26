using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MyLittleTeamspeakServerQuery
{
    public static class TConverter
    {
        public static T ChangeType<T>(string value)
        {
            return (T)ChangeType(typeof(T), value);
        }
        public static object ChangeType(Type t, string value)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(t);
            if (t == typeof(double))
                return tc.ConvertFromInvariantString(value);
            else if (t == typeof(string))
                value = Parser.UnescapeString(value);
            else if (t == typeof(bool))
                value = ConvertToBool(value).ToString();
            else if (value.Contains(","))
                return value.Split(',').Select(x => int.Parse(x)).ToArray();
            else if (t == typeof(int[]) && !value.Contains(","))
                return new int[] { int.Parse(value) };
            else if (t == typeof(DateTime))
                return new DateTime(1970, 1, 1).AddSeconds(Convert.ToDouble(value)).ToUniversalTime();
            else if (t == typeof(UInt32))
                if (!uint.TryParse(value, out uint resul))
                    value = uint.MinValue.ToString();

            if (string.IsNullOrEmpty(value))
                return tc.ConvertFromString("0");
            else
                return tc.ConvertFromString(value);
            //return tc.ConvertFrom(value, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
        {

            TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(typeof(TC)));
        }
        private static bool ConvertToBool(string value)
        {
            return (int.TryParse(value, out int val) && val == 0) ? false : true;
        }
    }
}
