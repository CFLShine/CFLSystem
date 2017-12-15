using System;
using System.Collections.Generic;
using SqlOrm;

namespace MSTD.ShBase
{
    public class Proxy
    {
        public Proxy(ShContext context)
        {
            Context = context;
        }

        public ShContext Context
        {
            get;
            set;
        }

        public virtual Guid ID { get; set; } = Guid.Empty;

        /// <summary>
        /// La string s représente le type et l'id de l'objet représenté
        /// par le proxy sous la forme typename_id.
        /// Retourne le proxy, ou null si non trouvé.
        /// Provoque une exception si s ne répond pas au format demandé,
        /// ou si le type demandé n'est pas représenté dans le context.
        /// </summary>
        public static ClassProxy GetProxy(ShContext context, string s)
        {
            Tuple<Type, Guid> _representation = BaseObjectRepresentation(context, s);
            Set _set = context.GetSet(_representation.Item1);
            if(_set == null)
                throw new Exception("Le type " + _representation.Item1.Name 
                                               + " n'est pas représenté dans le context.");
            return _set.GetProxy(_representation.Item2);
        }

        #region Base object representation

        /// <summary>
        /// Retourne une chaine représentant l'entité sous la forme
        /// typename_id ou "null" si entity est null.
        /// </summary>
        public static string BaseObjectRepresentation(Base entity)
        {
            if(entity == null)
                return "null";
            return entity.GetType().Name.ToLower() + "_" + entity.ID.ToString();
        }

        /// <summary>
        /// La string s représente le type et l'id de l'objet représenté
        /// par le proxy sous la forme typename_id.
        /// Retourne un Tuple de Set, Guid représeantant l'objet.
        /// Provoque une exception si s ne répond pas au format demandé,
        /// si le type demandé n'est pas représenté dans le context,
        /// si context est null.
        /// </summary>
        public static Tuple<Type, Guid> BaseObjectRepresentation(ShContext context, string s)
        {
            if(context == null)
                throw new ArgumentNullException("context");

            if(string.IsNullOrWhiteSpace(s) || !s.Contains("_"))
                throw new Exception("s ne répond pas au format typename_id");

            string[] _elements = s.Split('_');
            if(_elements.Length != 2 
            || string.IsNullOrWhiteSpace(_elements[0])
            || string.IsNullOrWhiteSpace(_elements[1]))
                throw new Exception("s ne répond pas au format typename_id");

            Set _set = context.GetSet(_elements[0]);
            if(_set == null)
                throw new Exception("Le type " + _elements[0] + " n'est pas représenté dans le context");

            Guid _guid;
            if(!Guid.TryParse(_elements[1], out _guid))
                throw new Exception("La chaine " + _elements[1] + "ne représente pas un Guid valide");

            return new Tuple<Type, Guid>(_set.Type, _guid);
        }

        #endregion Base object representation
        
    }

    public class ClassProxy : Proxy
    {
        #region Constructors

        public ClassProxy(ShContext context, Type classType, Guid id)
            :base(context)
        {
            ClassProxy _proxy = this;
            ProxyFactory.ClassProxyFactory(Context, ref _proxy, classType, id);
            IsBuilt = true;
        }

        public ClassProxy(ShContext context, Base entity)
            :base(context)
        {
            if(entity == null)
                throw new ArgumentNullException("entity");
            ClassProxy _proxy = this;
            ProxyFactory.ClassProxyFactory(Context, ref _proxy, entity.GetType(), entity.ID);
            __entity = entity;
            UpdateProxyValues();
            IsBuilt = true;
        }

        #endregion Constructors

        public Base Entity
        {
            get => __entity;
            set
            {
                __entity = value;
                if(__entity != null)
                {
                    Type = __entity.GetType();

                    if(!IsBuilt)
                    {
                        ClassProxy _proxy = this;
                        ProxyFactory.ClassProxyFactory(Context, ref _proxy, __entity.GetType(), Entity.ID);
                        IsBuilt = true;
                    }
                }

                UpdateProxyValues();
            }
        }

        /// <summary>
        /// Donne pour valeur à <see cref="Entity"/> une nouvelle instance
        /// de <see cref="Type"/>.
        /// </summary>
        public void CreateNewEntity()
        {
            Entity = (Base)TypeHelper.NewInstance(Type);
            Entity.ID = ID;
        }

        /// <summary>
        /// Signifie que l'objet représenté par ce proxy n'a jamais été sauvegardé.
        /// Par défaut, <see cref="IsNew"/> == true.
        /// Une classe de sauvegarde a la responsabilité de le changer à false.
        /// </summary>
        public bool IsNew { get; set; } = true;

        public bool IsBuilt
        {
            get;
            private set;
        }

        /// <summary>
        /// Le Guid de l'objet représenté par ce <see cref="ClassProxy"/>.
        /// set : applique aussi la valeur à <see cref="Entity"/> si
        /// <see cref="Entity"/> n'est pas null.
        /// </summary>
        public override Guid ID
        {
            get => base.ID;
            set
            {
                base.ID = value;
                if(__entity != null)
                {
                    if(ID == Guid.Empty)
                        throw new InvalidOperationException("ID ne peut pas être égale à Guid.Empty si Entity n'est pas null.");
                    __entity.ID = value;
                }
            }
        }

        /// <summary>
        /// Le nom du type de la classe sur laquelle ce <see cref="ClassProxy"/> est construit.
        /// </summary>
        public string TypeName
        {
            get => __typename;
            set
            {
                __typename = value;
                Set _set = Context.GetSet(__typename);
                __type = _set.Type;
            }
        }

        /// <summary>
        /// Le type de l'entité représentée.
        /// </summary>
        public Type Type
        {
            get => __type;
            set
            {
                __type = value;
                __typename = __type.Name;
            }

        }

        public void SetProperty(string name, PropertyProxy property)
        {
            property.Name = name;
            __properties[name] = property;
        }

        public IEnumerable<PropertyProxy> Properties()
        {
            foreach(PropertyProxy _pr in __properties.Values)
                yield return _pr;
        }

        private Dictionary<string, PropertyProxy> __properties
        {
            get;
            set;
        } = new Dictionary<string, PropertyProxy>();

        public IEnumerable<PropertyProxy> PropertiesNoObject()
        {
            foreach(PropertyProxy prProxy in Properties())
            {
                if(prProxy.GetType() != typeof(PropertyObjectProxy))
                    yield return prProxy;
            }
        }

        public IEnumerable<PropertyObjectProxy> PropertiesObjects()
        {
            foreach(PropertyProxy prProxy in Properties())
            {
                if(prProxy.GetType() == typeof(PropertyObjectProxy))
                    yield return (PropertyObjectProxy)prProxy;
            }
        }

        public IEnumerable<PropertyListProxy> PropertiesList()
        {
            foreach(PropertyProxy prProxy in Properties())
            {
                if(prProxy.GetType() == typeof(PropertyListProxy))
                    yield return (PropertyListProxy)prProxy;
            }
        }

        /// <summary>
        /// Retourne la <see cref="PropertyProxy"/> du membre propertyName,
        /// ou null si non trouvé.
        /// Non sensible à la casse.
        /// </summary>
        public PropertyProxy GetPropertyProxy(string propertyName)
        {
            propertyName = propertyName.ToLower();
            foreach(PropertyProxy prProxy in __properties.Values)
            {
                if(prProxy.Name.ToLower() == propertyName)
                    return prProxy;
            }
            return null;
        }

        public void UpdateProxyValues()
        {
            foreach(PropertyProxy _prProxy in Properties())
            {
                _prProxy.CopyValueFromEntity();
            }
        }
        
        public void UpdateEntityValues()
        {
            foreach(PropertyProxy _prProxy in Properties())
            {
                _prProxy.GiveValueToEntity();
            }
        }

        /// <summary>
        /// Retourne une liste des <see cref="PropertyProxy"/> dont la valeur
        /// ne correspond plus à celle de l'entité représentée.
        /// Le choix d'une liste au lieu d'un IEnumerable permet
        /// de tester Liste.Count.
        /// </summary>
        public List<PropertyProxy> ChangedProperties()
        {
            List<PropertyProxy> _changes = new List<PropertyProxy>();
            foreach(PropertyProxy _prProxy in Properties())
            {
                if(_prProxy.IsSameAsEntityValue() != true)
                    _changes.Add(_prProxy);
            }
            return _changes;
        }

        private Base __entity = null;
        private Type __type = null;
        private string __typename = "";
    }
}
