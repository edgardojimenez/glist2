using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace GroceriesList.Helpers {

    public static class Extensions {

        public static string TrimEnd(this string buffer, string trim) {
            return buffer.TrimEnd(trim.ToCharArray());
        }

        public static string SafeReplace(this string input, string find, string replace) {
            return string.IsNullOrEmpty(input) ? input : input.Replace(find, replace);
        }

        public static string[] SafeSplit(this string input, string delimiter) {
            if (input.IsNullOrEmpty()) {
                return new string[0];
            } else {
                return input.Split(delimiter.ToCharArray());
            }
        }

        public static string Truncate(this string buffer, int maxLength) {
            if (buffer.IsNullOrEmpty()) {
                return string.Empty;
            }

            if (buffer.Length > maxLength) {
                return buffer.Substring(0, maxLength) + "...";
            } else {
                return buffer;
            }
        }

        public static string SafeTrim(this string value) {
            if (!value.IsNullOrEmpty()) {
                return value.Trim();
            }

            return string.Empty;
        }

        public static string NullIfEmpty(this string value) {
            if (!string.IsNullOrWhiteSpace(value)) {
                return value.Trim();
            }

            return null;
        }

        public static string EmptyIfNull(this string s) {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            else
                return s;
        }

        public static bool MatchesAnyString(this string container, params string[] values) {
            if (values == null || container.IsNullOrEmpty()) {
                return false;
            } else {
                container = container.Trim();
                return values.Any(t => string.Compare(container, t.Trim(), true) == 0);
            }
        }

        public static bool MatchesAny<T>(this T container, params T[] values) {
            if (values == null || container == null) {
                return false;
            }

            return values.Contains(container);
        }

        public static bool IsNullOrEmpty(this string buffer) {
            return string.IsNullOrEmpty(buffer);
        }

        public static bool IsNullOrWhiteSpace(this string buffer) {
            return string.IsNullOrWhiteSpace(buffer);
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
            foreach (T item in enumerable) {
                action(item);
            }
        }

        public static T Clone<T>(this T entity) where T : class {
            if (entity == null) {
                return null;
            }

            using (var memStream = new MemoryStream()) {
                var bf = new BinaryFormatter();
                bf.Serialize(memStream, entity);
                memStream.Flush();
                memStream.Position = 0;
                return ((T)bf.Deserialize(memStream));
            }
        }

        public static string FormatWith(this string format, params object[] args) {
            return string.Format(format, args);
        }

        public static T ShallowCopy<T, U>(this U entity) where T : new() {
            T clone = new T();

            // copy base class properties.
            foreach (var prop in typeof(U).GetProperties()) {
                var prop2 = typeof(T).GetProperty(prop.Name);
                if (prop2 != null && prop.CanWrite && prop.CanRead)
                    prop2.SetValue(clone, prop.GetValue(entity, null), null);
            }

            return clone;
        }

        public static void AddParameter<T>(this SqlCommand command, string sqlParameterName, T obj, Expression<Func<T, object>> expression) {
            object finalValue = DBNull.Value;
            if (obj != null) {

                var propertyName = string.Empty;

                var body = expression.Body as MemberExpression;
                if (body == null) {
                    var ubody = expression.Body as UnaryExpression;
                    if (ubody != null) {
                        body = ubody.Operand as MemberExpression;
                    }
                    if (body != null) {
                        propertyName = body.Member.Name;
                    }
                } else {
                    propertyName = body.Member.Name;
                }

                var type = obj.GetType();
                var name = type.GetProperty(propertyName);
                if (name != null) {
                    var val = name.GetValue(obj, null);
                    if (val != null) {
                        finalValue = val;
                    }
                }
            }

            command.Parameters.AddWithValue(sqlParameterName, finalValue);
        }

        private static object ChangeType(object value, Type conversionType) {
            if (conversionType == null) {
                throw new ArgumentNullException("conversionType");
            }

            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                if (value == null) {
                    return null;
                }

                var nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }

            return Convert.ChangeType(value, conversionType);
        }

        public static string UserName(this IIdentity identity) {
            if (identity.Name.IndexOf("\\") > 0) {
                return identity.Name.Substring(identity.Name.IndexOf("\\") + 1);
            }
            return identity.Name;
        }

        public static string ToYesNo(this bool value) {
            return value ? "Yes" : "No";
        }

        public static T ConvertObject<T>(this object obj) {
            if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value)) {
                return default(T);
            }

            return (T)obj;
        }

        public static void ThrowOnNull(this object myself) {
            var message = string.Format("Parameter of type '{0}' cannot be null.", myself.GetType().Name);

            myself.ThrowOnNull(message);
        }

        public static void ThrowOnNull(this object myself, string message) {
            if (null == myself) {
                throw new ArgumentNullException(message ?? "Argument cannot be null.");
            }
        }

        public static void ThrowOnNullOrWhitespace(this string myself) {
            myself.ThrowOnNullOrWhitespace(null);
        }

        public static void ThrowOnNullOrWhitespace(this string myself, string message) {
            if (null == myself) {
                throw new ArgumentNullException(message ?? "Argument cannot be null.");
            } else if (myself.Trim().Length == 0) {
                throw new ArgumentException(message ?? "Argument cannot contain only whitespace.");
            }
        }

        public static void ThrowOnTypeNotDerivedFrom(this Type myType, Type supertype) {
            ThrowOnTypeNotDerivedFrom(myType, supertype, null);
        }

        public static void ThrowOnTypeNotDerivedFrom(this Type myType, Type supertype, string message) {
            if (!supertype.IsAssignableFrom(myType)) {
                throw new InvalidCastException(message ?? string.Format("{0} is not derived from {1}", myType.Name, supertype.Name));
            }
        }

        public static bool IsNullOrWhitespace(this string myself) {
            return null == myself || myself.Trim().Length == 0;
        }

        public static string TrimOrEmptyOnNull(this string myString) {
            return (myString ?? string.Empty).Trim();
        }

        public static T DefaultIfDbNull<T>(this object mySelf, T defaultValue) {
            var returnValue = defaultValue;

            if (!Convert.IsDBNull(mySelf)) {
                if (typeof(T).IsAssignableFrom(mySelf.GetType())) {
                    returnValue = (T)mySelf;
                } else {
                    returnValue = (T)Convert.ChangeType(mySelf, typeof(T));
                }
            }

            return returnValue;
        }

        public static string RemoveWhitespace(this string thisString) {
            var whitespaceChars = "\n\r\t ";

            var newChars = thisString.ToCharArray().Where(c => whitespaceChars.IndexOf(c) < 0);

            return new String(newChars.ToArray());
        }

        public static string RemoveNonNumeric(this string thisString) {
            var newChars = thisString.ToCharArray().Where(c => char.IsDigit(c));

            return new string(newChars.ToArray());
        }

        public static T GreaterOfTheTwo<T>(this T thisObject, T compareValue) where T : IComparable {
            return ((IComparable)thisObject).CompareTo(compareValue) > 0 ? thisObject : compareValue;
        }

        public static string RemoveDomainFromName(this string thisString) {
            var name = (thisString ?? string.Empty).Trim();
            var index = name.IndexOf('\\');

            if (index + 1 >= name.Length) {
                name = string.Empty;
            } else if (index > 0 && index < name.Length) {
                name = name.Substring(index + 1);
            }

            return name;
        }

        public static string AppendRandomUpperCase(this string thisString, int charCount) {
            var rnd = new Random();
            string returnString = null;

            if (charCount > 0) {
                var chars = new char[charCount];

                for (var count = 0; count < charCount; count++) {
                    chars[count] = Convert.ToChar(rnd.Next(65, 90));
                }

                returnString = (thisString ?? string.Empty) + new string(chars);
            }

            return returnString ?? thisString ?? string.Empty;
        }

        public static string ToFirstLetterCapitalized(this string thisString) {
            var stringParts = (thisString ?? string.Empty).Split(' ');
            var builder = new StringBuilder();

            foreach (var stringPart in stringParts) {
                var trimmedString = stringPart.Trim();

                if (trimmedString.Length > 0) {
                    builder.Append(trimmedString.Substring(0, 1).ToUpper());

                    if (trimmedString.Length > 1) {
                        builder.Append(trimmedString.Substring(1).ToLower());
                    }

                    builder.Append(' ');
                }
            }

            return builder.ToString().Trim();
        }

    }
}
