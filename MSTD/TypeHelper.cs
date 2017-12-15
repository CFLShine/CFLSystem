using System;
using System.Collections.Generic;
using System.Reflection;
using MSTD.ShBase;

namespace MSTD
{
    /// <summary>
    /// Représente un type qu'il soit nullable ou pas.
    /// Prend en compte le type <see cref="Base"/>.
    /// LIST pour tout type de List, DICTIONARY pour tout type de Dictionary,
    /// ARRAY pour tout type de Array.
    /// </summary>
    public enum TypeEnum
    {
        UNKNOWN,
        STRING,
        BASE,
        ENUM,
        BOOL,
        INT,
        CHAR,
        LONG,
        DOUBLE,
        DECIMAL,
        DATETIME,
        TIMESPAN
    }

    public static class TypeHelper
    {
        #region Factory

        public static object NewInstance(object o)
        {
            return NewInstance(o.GetType());
        }

        public static object NewInstance(Type T)
        {
            return (Activator.CreateInstance(T));
        }

        public static T NewInstance<T>()
        {
            return (T) NewInstance(typeof(T));
        }

        /// <summary>
        /// Retourne une instance de T si T est non nullable, sinon null.
        /// </summary>
        public static object GetDefaultValue(Type t)
        {
            if(t != null)
            {
                if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                    return Activator.CreateInstance(t);
            }
            return null;
        }

        #endregion Instance

        public static IEnumerable<PropertyInfo> MappableProperties(Type t)
        {
            foreach(PropertyInfo _prInfo in t.GetProperties())
            {
                if(PropertyHelper.IsMappableProperty(_prInfo))
                    yield return _prInfo;
            }
        }

        /// <summary>
        /// Retourne true si le type est primitif : 
        /// Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single,
        /// ou string, Guid, DateTime, TimeSpan, Decimal (nullables ou non).
        /// Inclus les types nullable.
        /// </summary>
        public static bool IsPrimitiveOrAlike(Type type)
        {
            return type.IsPrimitive
                || type == typeof(string)
                || type == typeof(Guid)
                || type == typeof(Guid?)
                || type == typeof(DateTime)
                || type == typeof(TimeSpan)
                || type == typeof(Decimal?)
                || type == typeof(DateTime?)
                || type == typeof(TimeSpan?)
                || type == typeof(Decimal);
        }

        public static bool IsNullable(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static bool IsBaseOrSub(Type type)
        {
            return type == typeof(Base) || type.IsSubclassOf(typeof(Base));
        }

        public static bool IsGenericList(Type _type)
        {
            return(_type.IsGenericType && _type.GetGenericTypeDefinition() == typeof(List<>));
        }

        /// <summary>
        /// Retourne true si objectType est une liste dont les items
        /// sont de type ou sous-type de itemsType.
        /// </summary>
        public static bool IsListOf(Type objectType, Type itemsType)
        {
            return IsGenericList(objectType) && 
            (ListItemsType(objectType) == itemsType || ListItemsType(objectType).IsSubclassOf(itemsType));
        }

        /// <summary>
        /// Retourne le type des items d'une liste.
        /// </summary>
        public static Type ListItemsType(Type _ListType)
        {
            return _ListType.GetGenericArguments()[0];
        }

        public static bool IsDictionary(Type type)
        {
            return(type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(Dictionary<,>));
        }

        /// <summary>
        /// Retourne un tuple du type des clés et du type des valeurs du dictionnaire si 
        /// type est un type de dictionnaire, sinon retourne null.
        /// </summary>
        public static Tuple<Type, Type> DictionaryKeysValuesTypes(Type type)
        {
            if(!IsDictionary(type))
                return null;
            return new Tuple<Type, Type>(type.GetGenericArguments()[0], type.GetGenericArguments()[1]);
        }

        #region Get property

        /// <summary>
        /// Retourne la propriété publique membre de la classe de type _classType dont le nom est _propertyName,
        /// ou null si non trouvé.
        /// N'est pas sensible à la casse.
        /// </summary>
        public static PropertyInfo Property(Type _classType, string _propertyName)
        {
            string _name = _propertyName.ToLower();
            foreach(PropertyInfo _prInfo in _classType.GetProperties())
            {
                if(_prInfo.PropertyType.IsPublic && _prInfo.Name.ToLower() == _name)
                    return _prInfo;
            }
            return null;
        }

        /// <summary>
        /// Retourne la première propriété publique de type propertyType trouvée dans la class de type classType,
        /// ou null si non trouvée.
        /// </summary>
        public static PropertyInfo Property(Type classType, Type propertyType)
        {
            foreach(PropertyInfo _prInfo in classType.GetProperties())
            {
                if(_prInfo.PropertyType.IsPublic && _prInfo.PropertyType == propertyType)
                    return _prInfo;
            }
            return null;
        }

        #endregion
    }
}
