
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MSTD
{
    public static class PropertyHelper
    {
        /// <summary>
        /// Verifie que cette propriété est publique et qu'elle pocède
        /// les accèsseurs get et set.
        /// </summary>
        public static bool IsExposedProperty(PropertyInfo prInfo)
        {
            Type _t = prInfo.PropertyType;

            return _t.IsPublic
                && prInfo.CanRead
                && prInfo.CanWrite;
        }

        /// <summary>
        /// Vérifie que cette propriété est publique,
        /// qu'elle n'a pas l'attribut [NotMapped] et
        /// qu'elle pocède les les accesseurs get et set.
        /// </summary>
        public static bool IsMappableProperty(PropertyInfo prInfo)
        {
            return IsExposedProperty(prInfo)
                && prInfo.GetCustomAttribute<NotMappedAttribute>() == null;
        }

        /// <summary>
        /// Si le propriété a l'atribut DisplayAttribute, retourne DisplayAttribute.Name,
        /// sinon, retourne le nom de la propriété (PropertyInfo.Name).
        /// </summary>
        public static string GetNameToDisplay(PropertyInfo _prInfo)
        {
            DisplayAttribute _attribute = _prInfo.GetCustomAttribute<DisplayAttribute>();
            return (_attribute != null && !string.IsNullOrWhiteSpace(_attribute.Name))?
                    _attribute.Name : _prInfo.Name;
        }

        public static T GetAttribute<T>(PropertyInfo prInfo) where T : Attribute
        {
            return prInfo.GetCustomAttribute<T>();
        }


    }
}
