using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MSTD.ShBase
{
    public static class BaseHelper
    {
        #region Components

        /// <summary>
        /// Retourne le premier composant trouvé de type T, qu'il soit contenu par une propriété
        /// ou une liste.
        /// </summary>
        public static Base ComponentOfType(Base _parent, Type T)
        {
            return ComponentOfType(_parent, T.Name);
        }

        /// <summary>
        /// Retourne le premier composant trouvé de type _typeName, qu'il soit contenu par une propriété
        /// ou une liste.
        /// Non sensible à la casse.
        /// </summary>
        public static Base ComponentOfType(Base _parent, string _typeName)
        {
            _typeName = _typeName.ToLower();
            Type _propertyType = null;

            foreach(PropertyInfo _pr in _parent.GetType().GetProperties())
            {
                _propertyType = _pr.PropertyType;
                if(_propertyType.IsPublic)
                {
                    if(_propertyType.Name.ToLower() == _typeName)
                    { 
                        return (Base)(_pr.GetValue(_parent));
                    }

                    if(TypeHelper.IsGenericList(_propertyType))
                    {
                        Type _itemsType = TypeHelper.ListItemsType(_propertyType);
                        if(_itemsType == typeof(Base) || _itemsType.IsSubclassOf(typeof(Base)))
                        {
                            IList _list = (IList)_pr.GetValue(_parent);
                            foreach(object _o in _list)
                            {
                                if(_o != null)
                                {
                                    Type _ot = _o.GetType();
                                    if(_o.GetType().Name.ToLower() == _typeName)
                                    return (Base)_o;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Retourne les composants de type T, qu'ils soient contenus par des propriétés
        /// ou des listes.
        /// </summary>
        public static IEnumerable<Base> ComponentsOfType(Base _parent, Type T)
        {
            return ComponentsOfType(_parent, T.Name.ToLower());
        }

        /// <summary>
        /// Retourne les composants de type _typeName, qu'ils soient contenus par des propriétés
        /// ou des listes.
        /// Non sensible à la casse.
        /// </summary>
        public static IEnumerable<Base> ComponentsOfType(Base _parent, string _typeName)
        {
            _typeName = _typeName.ToLower();
            Type _propertyType = null;

            foreach(PropertyInfo _pr in _parent.GetType().GetProperties())
            {
                _propertyType = _pr.PropertyType;
                if(_propertyType.IsPublic)
                {
                    if(_propertyType.Name.ToLower() == _typeName)
                    { 
                        yield return (Base)(_pr.GetValue(_parent));
                    }

                    if(TypeHelper.IsGenericList(_propertyType) && TypeHelper.ListItemsType(_propertyType).IsSubclassOf(typeof(Base)))
                    {
                        IList _list = (IList)_pr.GetValue(_parent);
                        foreach(object _o in _list)
                        {
                            if(_o != null)
                            {
                                Type _t = _o.GetType();
                                if(_o.GetType().Name.ToLower() == _typeName)
                                yield return (Base)_o;
                            }
                        }
                    }
                }
            }
        }

        public static Base Component(Base _parent, Guid _id)
        {
            foreach(PropertyInfo _pr in _parent.GetType().GetProperties())
            {
                Type _propertyType = _pr.PropertyType;
                if(_propertyType.IsPublic && _pr.CanRead)
                {
                    if(_propertyType.IsSubclassOf(typeof(Base)))
                    { 
                        Base _value = (Base)(_pr.GetValue(_parent));
                        if(_value.ID == _id)
                            return _value;
                    }

                    if(TypeHelper.IsGenericList(_propertyType) && TypeHelper.ListItemsType(_propertyType).IsSubclassOf(typeof(Base)))
                    {
                        IList _list = (IList)_pr.GetValue(_parent);
                        foreach(object _o in _list)
                        {
                            if(_o is Base _component && _component.ID == _id)
                                return _component;
                        }
                    }
                }
            }
            return null;
        }

        public static Base Component(Base parent, string name)
        {
            string _name = name.ToLower();
            foreach(PropertyInfo _pr in parent.GetType().GetProperties())
            {
                if(_pr.PropertyType.IsPublic && _pr.CanRead && _pr.Name.ToLower() == _name)
                {
                    return _pr.GetValue(parent) as Base;
                }
            }
            return null;
        }

        #endregion Components

        #region Property exposition

        public static IEnumerable<PropertyInfo> ExposedProperties(Base _object)
        {
            foreach(PropertyInfo _prInfo in _object.GetType().GetProperties())
            {
                if(PropertyHelper.IsExposedProperty(_prInfo))
                    yield return _prInfo;
            }
        }

        #endregion Property exposition

    }
}
