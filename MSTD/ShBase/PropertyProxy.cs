using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SqlOrm;

namespace MSTD.ShBase
{
    public abstract class PropertyProxy : Proxy
    {
        public PropertyProxy(ShContext context)
            :base(context)
        { }

        public PropertyProxy(ShContext context, PropertyInfo prInfo, ClassProxy parent)
            :base(context)
        {
            PropertyInfo = prInfo;
            Parent = parent;
        }

        public ClassProxy Parent { get; set; }

        public abstract bool IsPrimitive    { get; }
        public abstract bool IsObject       { get; }
        public abstract bool IsList         { get; }
        public abstract bool IsDictionary   { get; }
        public abstract bool IsEnum         { get; }

        /// <summary>
        /// Chaque classe fille surcharge PropertyInfo pour renseigner
        /// <see cref="Name"/> et 
        /// <see cref="TypeName"/> et événtuèlement d'autre propriétés de type.
        /// <see cref="PropertyPrimitiveProxy"/> et <see cref="PropertyObjectProxy"/>: <see cref="TypeName"/> = le type c#,
        /// <see cref="PropertyListProxy"/> et <see cref="PropertyDictionaryProxy"/> renseignent aussi
        /// le type des items, ou des clés et valeurs.
        /// </summary>
        public abstract PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Le nom qui devra être affiché à l'utilisateur final
        /// pour représenter cette propriété.
        /// ex. string Name : Label = "Nom" si un DisplayAttribute est attaché à
        /// la propriété avec Name = "Nom", sinon, "Name", le nom de la propriété.
        /// </summary>
        public string Label 
        { 
            get => PropertyHelper.GetNameToDisplay(__prInfo);
        }

        /// <summary>
        /// Le type c# du membre représenté par ce <see cref="PropertyProxy"/>.
        /// Il est automatiquement renseigné lorsque PropertyInfo est donné dans une classe fille.
        /// Dans le cas d'une liste, <see cref="TypeName"/> == "List", un dictionnaire <see cref="TypeName"/> == "Dictionary",
        /// un DateTime ou DateTime?, un TimeSpan ou TimeSpan?, "Date" et "Time",
        /// pour un type primitif ou object (<see cref="Base"/>), le type c#.
        /// </summary>
        public string TypeName { get; protected set; }

        /// <summary>
        /// Le type c# du membre représenté par ce <see cref="PropertyProxy"/>.
        /// </summary>
        public virtual Type Type
        {
            get
            {
                if(__type == null)
                {
                    if(PropertyInfo != null)
                        __type = PropertyInfo.PropertyType;
                    else if(!string.IsNullOrWhiteSpace(TypeName))
                    {
                        foreach(Type t in Assembly.GetExecutingAssembly().GetTypes())
                        {
                            if(t.Name == TypeName)
                            {
                                __type = t;
                                break;
                            }
                        }
                    }
                }
                return __type;
            }
        }

        /// <summary>
        /// Le nom du membre, dans la classe CSharp.
        /// </summary>
        public string Name
        {
            get => __name;
            set
            {
                __name = value;
                if(Parent is ClassProxy _parent 
                && _parent.Entity != null)
                {
                    PropertyInfo _prInfo = TypeHelper.Property(_parent.Entity.GetType(), value);
                    PropertyInfo = _prInfo??throw new Exception("La propriété " + value + " n'existe pas dans le type " + _parent.Entity.GetType().Name);
                }
            }
        }

        /// <summary>
        /// La valeur est une copie de la valeur du membre de l'objet
        /// représenté par la classe Proxy.
        /// <see cref="PropertyObjectProxy"/> surcharge <see cref="Value"/>
        /// qui est un objet de classe proxy de ce type. Ce proxy sera cherché
        /// dans le Context ou instancié si non trouvé.
        /// </summary>
        public virtual object Value
        {
            get => __value;
            set => __value = value;
        }

        /// <summary>
        /// Liste de valeurs possibles pour <see cref="Value"/>.
        /// Ceci peut être utile, par exemple, si cette <see cref="PropertyProxy"/>
        /// est utilisé par un control visuel, comme un combo, lequel se peuplera de ces valeurs.
        /// </summary>
        public Dictionary<Guid, object> Data { get; set; } = new Dictionary<Guid, object>();

        /// <summary>
        /// Copie la valeur de la propriété représentée par ce
        /// <see cref="PropertyProxy"/> et l'affecte à <see cref="Value"/>.
        /// Provoque une exeption si <see cref="Parent"/> est null 
        /// ou si (<see cref="Parent"/>.)<see cref="ClassProxy.Entity"/> est null.
        /// </summary>
        public abstract void CopyValueFromEntity();

        /// <summary>
        /// Affecte <see cref="Value"/> à la propriété 
        /// </summary>
        public abstract void GiveValueToEntity();

        /// <summary>
        /// Permet de savoir si l'entité a été modifiée depuis que ses valeurs ont été copiées dans un <see cref="ClassProxy"/>"/>.
        /// </summary>
        public abstract bool IsSameAsEntityValue();
        
        protected PropertyInfo __prInfo = null;
        protected string __name = "";

        protected object __value = null;
        protected Type __type = null;
    }

    public class PropertyPrimitiveProxy : PropertyProxy
    {
        public PropertyPrimitiveProxy(ShContext context)
            :base(context)
        { }

        public  PropertyPrimitiveProxy(ShContext context, PropertyInfo prInfo, ClassProxy parent)
            :base(context, prInfo, parent)
        { }

        public override bool IsPrimitive    => true;
        public override bool IsObject       => false;
        public override bool IsList         => false;
        public override bool IsDictionary   => false;
        public override bool IsEnum         => false;

        public override PropertyInfo PropertyInfo
        {
            get => __prInfo;
            set
            {
                __prInfo = value;
                Name = __prInfo.Name;

                if(__prInfo.PropertyType == typeof(DateTime) || __prInfo.PropertyType == typeof(DateTime?))
                    TypeName = "DateTime";
                else
                if(__prInfo.PropertyType == typeof(TimeSpan) || __prInfo.PropertyType == typeof(TimeSpan?))
                    TypeName = "TimeSpan";
                else
                    TypeName = __prInfo.PropertyType.Name;
            }
        }

        public override void GiveValueToEntity()
        {
            if(Parent == null)
                throw new Exception("Parent ne peut pas être null");
            if(Parent.Entity == null)
                throw new Exception("Parent.Entity ne peut pas être null");
            if(PropertyInfo == null)
                throw new Exception("PropertyInfo ne peut pas être null.");
            
            if(Value == null)
                Value = TypeHelper.GetDefaultValue(PropertyInfo.PropertyType);

            PropertyInfo.SetValue(Parent.Entity, Value);
        }

        public override void CopyValueFromEntity()
        {
            if(Parent == null)
                throw new Exception("Parent ne peut pas être null");
            if(Parent.Entity == null)
                throw new Exception("Parent.Entity ne peut pas être null");
            if(PropertyInfo == null)
                throw new Exception("PropertyInfo ne peut pas être null.");

            Value = PropertyInfo.GetValue(Parent.Entity);
        }

        public override bool IsSameAsEntityValue()
        {
            if(Parent == null)
                throw new Exception("Parent ne peut pas être null");
            if(Parent.Entity == null)
                throw new Exception("Parent.Entity ne peut pas être null");
            if(PropertyInfo == null)
                throw new Exception("PropertyInfo ne peut pas être null.");
            
            object _entiyValue = PropertyInfo.GetValue(Parent.Entity);
            if(Value == null)
                return _entiyValue == null;
            return Value.Equals(PropertyInfo.GetValue(Parent.Entity));
        }
    }

    public class PropertyObjectProxy : PropertyProxy
    {
        public PropertyObjectProxy(ShContext context)
            :base(context)
        { }

        public PropertyObjectProxy(ShContext context, PropertyInfo prInfo, ClassProxy parent)
            :base(context, prInfo, parent)
        { }

        public override bool IsPrimitive    => false;
        public override bool IsObject       => true;
        public override bool IsList         => false;
        public override bool IsDictionary   => false;
        public override bool IsEnum         => false;

        public override object Value 
        { 
            get => base.Value; 
            set
            {
                if(string.IsNullOrWhiteSpace((string)value) || (string)value == "null")
                {
                    base.Value = "null";
                    ID = Guid.Empty;
                }
                else
                {
                    base.Value = value;
                    Tuple<Type, Guid> _t = BaseObjectRepresentation(Context, (string)value);
                    ID = _t.Item2;
                }
            }
        }

        public Base Entity
        {
            get
            {
                if(Parent == null || Parent.Entity == null)
                    return null;
                return (Base) PropertyInfo.GetValue(Parent.Entity);
            }
        }

        public override PropertyInfo PropertyInfo
        {
            get => __prInfo;
            set
            {
                __prInfo = value;
                __name = __prInfo.Name;
                TypeName = __prInfo.PropertyType.Name;
            }
        }

        public override void GiveValueToEntity()
        {
            if(Parent == null)
                throw new Exception("Parent ne peut pas être null");
            if(Parent.Entity == null)
                throw new Exception("Parent.Entity ne peut pas être null");
            if(PropertyInfo == null)
                throw new Exception("PropertyInfo ne peut pas être null.");

            if(Value != null)
            {
                if((string)Value == "null")
                    PropertyInfo.SetValue(Parent.Entity, null);
                else
                {
                    string _objectrepresentation = (string)Value;
                    ClassProxy _proxy = GetProxy(Context, _objectrepresentation);

                    // _proxy peut être null si lors d'un load, cet objet n'a pas été réclamé.
                    if(_proxy != null)
                        PropertyInfo.SetValue(Parent.Entity, _proxy.Entity);
                }
            }
        }

        public override void CopyValueFromEntity()
        {
            if(Parent == null)
                throw new Exception("Parent ne peut pas être null");
            if(Parent.Entity == null)
                throw new Exception("Parent.Entity ne peut pas être null");
            if(PropertyInfo == null)
                throw new Exception("PropertyInfo ne peut pas être null.");

            Base _childEntity = PropertyInfo.GetValue(Parent.Entity) as Base;
            
            Value = BaseObjectRepresentation(_childEntity);
        }

        public override bool IsSameAsEntityValue()
        {
            if(Parent == null)
                throw new Exception("Parent ne peut pas être null");
            if(Parent.Entity == null)
                throw new Exception("Parent.Entity ne peut pas être null");
            if(PropertyInfo == null)
                throw new Exception("PropertyInfo ne peut pas être null.");

            Base _childEntity = PropertyInfo.GetValue(Parent.Entity) as Base;
            
            return (string)Value == BaseObjectRepresentation(_childEntity);
        }
    }

    public class PropertyListProxy : PropertyProxy
    {
        public PropertyListProxy(ShContext context)
            :base(context)
        { }

        public PropertyListProxy(ShContext context, PropertyInfo prInfo, ClassProxy parent)
            :base(context, prInfo, parent)
        { }

        public override bool IsPrimitive    => false;
        public override bool IsObject       => false;
        public override bool IsList         => true;
        public override bool IsDictionary   => false;
        public override bool IsEnum         => false;

        public Type ItemsType { get; private set; }

        public string ItemsTypeName { get; set; }

        public override PropertyInfo PropertyInfo
        {
            get => __prInfo;
            set
            {
                __prInfo = value;
                __name = __prInfo.Name;
                TypeName = "list";
                ItemsType = TypeHelper.ListItemsType(__prInfo.PropertyType);
                ItemsTypeName = ItemsType.Name;
            }
        }

        public IEnumerable<Base> Entities()
        {
            if(Parent == null || Parent.Entity == null)
                yield  return null;
            IList _list = PropertyInfo.GetValue(Parent.Entity) as IList;
            if(_list == null)
                yield return null;
            foreach(object _o in _list)
                yield return (Base)_o;
        }

        public override void GiveValueToEntity()
        {
            if(Parent == null)
                throw new Exception("Parent ne peut pas être null");
            if(Parent.Entity == null)
                throw new Exception("Parent.Object ne peut pas être null");
            if(PropertyInfo == null)
                throw new Exception("PropertyInfo ne peut pas être null.");
            // inutile de tester ItemsType qui est automatiquement renseigné 
            // lorsque PropertyInfo est donné.

            if(Value != null)
            {
                IList _list = PropertyInfo.GetValue(Parent.Entity) as IList;
                if(_list == null)
                    _list = (IList)TypeHelper.NewInstance(PropertyInfo.PropertyType.MakeGenericType());
                
                List<object> _value = Value as List<object>;
                _list.Clear();

                //list<Base> ou List<BaseDerived>
                if(ItemsType == typeof(Base) 
                || ItemsType.IsSubclassOf(typeof(Base)))
                {
                    foreach(object _o in _value)
                    {
                        ClassProxy _proxy = GetProxy(Context, (string)_o);
                        _list.Add(_proxy.Entity);
                    }
                }
                else
                {
                    foreach(object _o in _value)
                        _list.Add(_o);
                }

                PropertyInfo.SetValue(Parent.Entity, _list);
            }
        }
        
        /// <summary>
        /// Si les items de la liste sont de type primitif, 
        /// ils sont copiés dans la liste proxy,
        /// si ils sont de type <see cref="Base"/> ou dérivé de <see cref="Base"/>,
        /// ils sont enregistrés sous la forme string typename_id.
        /// </summary>
        public override void CopyValueFromEntity()
        {
            if(Parent == null)
                throw new Exception("Parent ne peut pas être null");
            if(Parent.Entity == null)
                throw new Exception("Parent.Object ne peut pas être null");
            if(PropertyInfo == null)
                throw new Exception("PropertyInfo ne peut pas être null.");
            // inutile de tester ItemsType qui est automatiquement renseigné 
            // lorsque PropertyInfo est donné.
            
            List<object> _copy = new List<object>();

            IList _list = PropertyInfo.GetValue(Parent.Entity) as IList;
            if(_list != null)
            {
                if(ItemsType == typeof(Base)
                || ItemsType.IsSubclassOf(typeof(Base)))
                {
                    foreach(object _o in _list)
                    {
                        if(_o != null)
                            _copy.Add(BaseObjectRepresentation((Base)_o));
                    }
                }
                else
                {
                    foreach(object _o in _list)
                        _copy.Add(_o);
                }
            }
            Value = _copy;
        }

        /// <summary>
        /// Construit sa liste depuis une string sous la forme
        /// val1,val2,... ou {val1,val2,...}
        /// N'utiliser que pour les listes d'objets de <see cref="Base"/> ou dérivés,
        /// sinon, provoque une exception.
        /// </summary>
        public void Parse(string s)
        {
            if(ItemsType != typeof(Base) && !ItemsType.IsSubclassOf(typeof(Base)))
                throw new Exception("Cette fonction n'est utilisable que pour les types Base ou dérivés.");

            if(s.StartsWith("{") && s.EndsWith("}"))
            {
                s = s.Substring(1,s.Length - 2);
            }

            Value = new List<object>();
            if(!string.IsNullOrWhiteSpace(s))
            {
                if(s.Contains(","))
                {
                    foreach(string _val in s.Split(','))
                        ((List<object>)Value).Add(_val);
                }
                else
                    ((List<object>)Value).Add(s);
            }
        }

        /// <summary>
        /// Si ce <see cref="PropertyListProxy"/> représente une liste
        /// d'objets de <see cref="Base"/> ou dérivés, retourne une liste
        /// de représentation de ces objets, sinon provoque une exception.
        /// </summary>
        public List<Tuple<Type, Guid>> ObjectsRepresentations()
        {
            List<Tuple<Type, Guid>> _values = new List<Tuple<Type, Guid>>();
            
            if(Value == null)
                return _values;

            List<object> _value = (List<object>)Value;

            string _s = "";

            foreach(object _o in _value)
            {
                _s = (string)_o;
                Tuple<Type, Guid> _objectRepresentation = BaseObjectRepresentation(Context, _s);
                _values.Add(_objectRepresentation);
            }
            return _values;
        }

        public override bool IsSameAsEntityValue()
        {
            List<object> _proxyValue = Value as List<object>;
            IList _entityValue = PropertyInfo.GetValue(Parent.Entity) as IList;

            if((_entityValue == null) != (_proxyValue == null))
                return false;
            if(_entityValue != null)
            {
                if(_entityValue.Count != _proxyValue.Count)
                    return false;
                if(ItemsType == typeof(Base) || ItemsType.IsSubclassOf(typeof(Base)))
                {
                    for(int _i = 0; _i < _entityValue.Count; _i++)
                    {
                        Base _base = _entityValue[_i] as Base;
                        if(BaseObjectRepresentation(_base) != (string)_proxyValue[_i])
                            return false;
                    }
                }
                else
                {
                    for(int _i = 0; _i < _entityValue.Count; _i++)
                    {
                        if(_entityValue[_i] != _proxyValue[_i])
                            return false;
                    }
                }
            }
            return true;
        }
    }

    public class PropertyDictionaryProxy : PropertyProxy
    {
        public PropertyDictionaryProxy(ShContext context)
            :base(context)
        { }

        public PropertyDictionaryProxy(ShContext context, PropertyInfo prInfo, ClassProxy parent)
            :base(context, prInfo, parent)
        { }

        public override bool IsPrimitive    => false;
        public override bool IsObject       => false;
        public override bool IsList         => false;
        public override bool IsDictionary   => true;
        public override bool IsEnum         => false;

        public Type KeysType { get; private set; }
        public Type ValuesType { get; private set; }

        public string KeysTypeName { get; set; }

        public string ValuesTypeName { get; set; }

        public override PropertyInfo PropertyInfo
        {
            get => __prInfo;
            set
            {
                __prInfo = value;
                __name = __prInfo.Name;
                Tuple<Type, Type> _keyValueTypes = TypeHelper.DictionaryKeysValuesTypes(__prInfo.PropertyType);
                KeysType = _keyValueTypes.Item1;
                ValuesType = _keyValueTypes.Item2;
                KeysTypeName = KeysType.Name;
                ValuesTypeName = ValuesType.Name;
            }
        }

        public override void GiveValueToEntity()
        {
            if(Parent == null)
                throw new Exception("Parent ne peut pas être null");
            if(Parent.Entity == null)
                throw new Exception("Parent.Object ne peut pas être null");
            if(PropertyInfo == null)
                throw new Exception("PropertyInfo ne peut pas être null.");
            // inutile de tester KeysType et ValuesType qui sont automatiquement
            // renseignés lorsque PropertyType est donné.
            
            if(Value != null)
            {
                IDictionary _dictionary = PropertyInfo.GetValue(Parent.Entity) as IDictionary;
                if(_dictionary == null)
                    _dictionary = (IDictionary)TypeHelper.NewInstance(PropertyInfo.PropertyType.MakeGenericType());
                
                Dictionary<object, object> _proxyValue = Value as Dictionary<object, object>;
                _dictionary.Clear();

                foreach(KeyValuePair<object, object> _kvp in _proxyValue)
                {
                    object _key = null;
                    object _value = null;

                    if(KeysType == typeof(Base)
                    || KeysType.IsSubclassOf(typeof(Base)))
                    { 
                        _key = GetProxy(Context, (string)_kvp.Key).Entity;
                    }
                    else 
                        _key = _kvp.Key;

                    if(ValuesType == typeof(Base)
                    || ValuesType.IsSubclassOf(typeof(Base)))
                    {
                        _value = GetProxy(Context, (string)_kvp.Value).Entity;
                    }
                    else
                        _value = _kvp.Value;

                    _dictionary[_key] = _value;
                }

                PropertyInfo.SetValue(Parent.Entity, _dictionary);
            }
        }

        /// <summary>
        /// Si les clés ou les valeurs sont de type primitif, 
        /// elles sont copiées dans de dictionaire proxy,
        /// si elles sont de type <see cref="Base"/> ou dérivées de <see cref="Base"/>,
        /// elles sont enregistrées sous la forme typename_id.
        /// </summary>
        public override void CopyValueFromEntity()
        {
            if(Parent == null)
                throw new Exception("Parent ne peut pas être null");
            if(Parent.Entity == null)
                throw new Exception("Parent.Object ne peut pas être null");
            if(PropertyInfo == null)
                throw new Exception("PropertyInfo ne peut pas être null.");
            // inutile de tester KeysType et ValuesType qui sont automatiquement
            // renseignés lorsque PropertyType est donné.

            Dictionary<object, object> _copy = new Dictionary<object, object>();

            IDictionary _dictionary = PropertyInfo.GetValue(Parent.Entity) as IDictionary;
            if(_dictionary != null)
            {
                object _key = null;
                object _value = null;

                foreach(KeyValuePair<object, object> _kvp in _dictionary)
                {
                    _key = _kvp.Key as Base;
                    if(_key != null)
                        _key = _key.GetType().Name.ToLower() + "_" + ((Base) _key).ID.ToString();
                    else
                        _key = _kvp.Key;

                    _value = _kvp.Value as Base;
                    if(_value != null)
                        _value = _value.GetType().Name.ToLower() + "_" + ((Base) _value).ID.ToString();
                    else
                        _value = _kvp.Value;

                    _copy[_key] = _value;
                }
            }
            Value = _copy;
        }

        public override bool IsSameAsEntityValue()
        {
            throw new NotImplementedException();
        }
    }

    public class PropertyEnumProxy : PropertyPrimitiveProxy
    {
        public PropertyEnumProxy(ShContext context)
            :base(context)
        { }

        public  PropertyEnumProxy(ShContext context, PropertyInfo prInfo, ClassProxy parent)
            :base(context, prInfo, parent)
        { }

        public override bool IsPrimitive    => false;
        public override bool IsObject       => false;
        public override bool IsList         => false;
        public override bool IsDictionary   => false;
        public override bool IsEnum         => true;

        public override PropertyInfo PropertyInfo
        {
            get => __prInfo;
            set
            {
                __prInfo = value;
                __name = __prInfo.Name;
                TypeName = __prInfo.PropertyType.Name;
                
                foreach(var v in Enum.GetNames(__prInfo.PropertyType))
                {
                    Data.Add(Guid.NewGuid(), v);
                }
            }
        }

    }
}
