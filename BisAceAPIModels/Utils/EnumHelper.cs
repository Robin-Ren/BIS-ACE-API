using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace BisAceAPIModels.Utils
{
    public class EnumHelper
    {
        public static IEnumerable<KeyValuePair<int, string>> GetList<T>()
        {
            var arrayValues = (int[])(Enum.GetValues(typeof(T)).Cast<int>());
            List<KeyValuePair<int, string>> lst = null;
            for (int i = 0; i < arrayValues.Length; i++)
            {
                if (lst == null)
                    lst = new List<KeyValuePair<int, string>>();
                int value = arrayValues[i];
                string name = GetEnumDescription<T>((value));
                lst.Add(new KeyValuePair<int, string>(value, name));
            }
            return lst;
        }

        public static string GetEnumDescription<TEnum>(int value)
        {
            return GetEnumDescription((Enum)(object)((TEnum)(object)value));
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;

            return value.ToString();
        }

        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "description");
        }

        public static string GetNameFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return field.Name;
                }
                else
                {
                    if (field.Name == description)
                        return field.Name;
                }
            }
            throw new ArgumentException("Not found.", "description");
        }


        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

    }
}