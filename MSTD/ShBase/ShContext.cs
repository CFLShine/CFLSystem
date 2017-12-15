using System;
using System.Collections.Generic;
using System.Reflection;
using CFL_1.CFL_System.MSTD.ShBase;

namespace MSTD.ShBase
{
    public class ShContext
    {
        public ShContext()
        {
            BuildSetList();
            Relations = new ContextRelations(this);
        }

        #region Attach

        /// <summary>
        /// Trouve et retourne le <see cref="ClassProxy"/> de entity s'il existe, ou le crée et l'attache, ainsi
        /// que les <see cref="ClassProxy"/> de ces membres objets de<see cref="Base"/>, 
        /// puis le retourne.
        /// Provoque une exception si entity est null ou non représenté
        /// par un <see cref="Set"/> dans ce <see cref="ShContext"/>.
        /// </summary>
        public ClassProxy GetOrAttach(Base entity)
        {
            if(entity == null)
                throw new ArgumentNullException("entity");
            
            Set _set = GetSet(entity.GetType());
            if(_set == null)
                throw new Exception
                    ("Le type d'entity " + entity.GetType().Name + " n'a pas été représenté dans le Context");
            
            ClassProxy _proxy = _set.GetProxy(entity);
            if(_proxy == null)
            {
                _proxy = new ClassProxy(this, entity);
                _set.AddProxy(_proxy);
            }

            Set _attacheds = new Set<Base>();
            _attacheds.AddProxy(_proxy);
            AttachProxyChildren(_proxy, _attacheds);

            return _proxy;
        }

        /// <summary>
        /// Trouve et retourne le proxy de type typename et ID id s'il existe, 
        /// ou le crée, lui donnant son id, l'attache et le retourne.
        /// 
        /// Cette fonction est utile lors d'un chargement de données, par exemple
        /// lors de la lecture des données retournées par une DB.
        /// Les proxys sont alors crées à partir de ces données, leur id leur est donné,
        /// ainsi que leurs valeurs.
        /// Les id et données de leur propriétés object de <see cref="Base"/>,
        /// ou des objets contenus dans des propriétés collection, ne sont
        /// pas connus à ce moment, cette fonction n'attache donc pas en cascade
        /// ces propriétés. C'est code qui lit les données qui est chargé
        /// de générér chaque proxy et entité, ouis l'appelle à la fonction
        /// <see cref="UpdateEntitiesValues"/> recrée les liens.
        /// 
        /// Provoque une exception si typename n'est pas le nom d'un type représenté 
        /// par un <see cref="Set"/> dans ce context, ou si id est un <see cref="Guid.Empty"/>.
        /// </summary>
        public ClassProxy GetOrAttach(string typename, Guid id)
        {
            if(string.IsNullOrWhiteSpace(typename))
                throw new Exception("typename ne peut pas être null ou vide.");

            Set _set = GetSet(typename);

            if(_set == null)
                throw new Exception("Le type " + typename + " n'est pas représenté dans ce context.");

            ClassProxy _proxy = _set.GetProxy(id);

            if(_proxy == null)
            {
                _proxy = new ClassProxy(this, _set.Type, id);
                _set.AddProxy(_proxy);
            }

            return _proxy;
        }

        private void AttachProxyChildren(ClassProxy proxy, Set attacheds)
        {
            foreach(PropertyObjectProxy _prProxy in proxy.PropertiesObjects())
            {
                Base _entity = _prProxy.Entity;
                Attach(_entity, attacheds);
            }

            foreach(PropertyListProxy _prListProxy in proxy.PropertiesList())
            {
                foreach(Base _entity in _prListProxy.Entities())
                {
                    Attach(_entity, attacheds);
                }
            }
        }

        private void Attach(Base entity, Set attacheds)
        {
            if(entity != null && !attacheds.IsAttached(entity))
            {
                Set _set = GetSet(entity.GetType());
                if(_set != null)
                {
                    ClassProxy _childProxy = _set.GetProxy(entity);
                    if(_childProxy == null)
                    {
                        _childProxy = new ClassProxy(this, entity);
                        _set.AddProxy(_childProxy);
                    }
                    attacheds.AddProxy(_childProxy);
                    AttachProxyChildren(_childProxy, attacheds);
                }
            }
        }

        #endregion Attach

        public ContextRelations Relations { get; private set; }

        #region get Set or ClassProxy

        public IEnumerable<Set> GetSets(int fromIndex = 0)
        {
            for(int _i = fromIndex;  _i < __sets.Count; _i++)
            {
                yield return __sets[_i];
            }
        }

        public int SetsCount
        {
            get => __sets.Count;
        }

        /// <summary>
        /// Retourne le <see cref="Set"/> correspondant au type t,
        /// ou null si non trouvé.
        /// </summary>
        public Set GetSet(Type t)
        {
            foreach(Set _set in __sets)
            {
                if(_set.Type == t)
                    return _set;
            }
            return null;
        }

        /// <summary>
        /// Retourne le <see cref="Set"/> dont le nom de type est égal
        /// à typename, ou null si non trouvé.
        /// Non sensible à la casse.
        /// </summary>
        public Set GetSet(string typename)
        {
            typename = typename.ToLower();
            foreach(Set _set in __sets)
            {
                if(_set.Type.Name.ToLower() == typename)
                    return _set;
            }
            return null;
        }

        /// <summary>
        /// Parcour les <see cref="Set"/>  pour retourner le <see cref="ClassProxy"/>
        /// d'ID égal à id.
        /// Retourne null si non trouvé.
        /// </summary>
        public ClassProxy GetProxy(Guid id)
        {
            ClassProxy _proxy = null;
            foreach(Set _set in __sets)
            {
                _proxy = _set.GetProxy(id);
                if(_proxy != null)
                    return _proxy;
            }
            return null;
        }

        public IEnumerable<ClassProxy> GetProxies()
        {
            foreach(Set _set in __sets)
            {
                foreach(ClassProxy _proxy in _set.GetProxies())
                    yield return _proxy;
            }
        }

        #endregion get Set or ClassProxy    

        #region Proceededs

        /// <summary>
        /// Ajoute _guid.ToString() à la liste des objets pour lesquels une requète
        /// a été préparée ou envoyée à la DB,
        /// évitant de traiter plusieurs fois le même objet.
        /// Lève une exception si <see cref="StartProcess"/> n'a pas été invoqué.
        /// </summary>
        public void Process(ClassProxy _proxy)
        {
            if(__procededs == null)
                throw new Exception("StartSelect n'a pas été invoqué");
            __procededs[_proxy.ID.ToString()] = _proxy;
        }

        /// <summary>
        /// Ajoute _guidstr à la liste des objets pour lesquels une requète
        /// a été préparée ou envoyée à la DB,
        /// évitant de traiter plusieurs fois le même objet.
        /// Lève une exeption si <see cref="StartProcess"/> n'a pas été invoqué.
        /// </summary>
        public void Process(string _guidstr)
        {
            if(__procededs == null)
                throw new Exception("StartSelect n'a pas été invoqué");
            if(__procededs.ContainsKey(_guidstr) == false)
                __procededs[_guidstr] = null;
        }

        public bool IsProceeded(Guid _id)
        {
            return IsProceeded(_id.ToString());
        }

        public bool IsProceeded(string _guidstr)
        {
            return __procededs.ContainsKey(_guidstr);
        }

        public bool IsStartedProcessing
        {
            get
            {
                return __procededs != null;
            }
        }

        public void StartProcess()
        {
            __procededs = new Dictionary<string, ClassProxy>();
        }

        public void EndProcess()
        {
            __procededs.Clear();
            __procededs = null;
        }

                //guid toString(), proxy
        private Dictionary<string, ClassProxy> __procededs = null;

        #endregion Proceededs

        /// <summary>
        /// Met à jour les valeurs des entités en conformité avec celles de leurs proxys.
        /// </summary>
        public void UpdateEntitiesValues()
        {
            foreach(ClassProxy _proxy in GetProxies())
            {
                _proxy.UpdateEntityValues();
            }
        }

        private void BuildSetList()
        {
            foreach(PropertyInfo _prInfo in this.GetType().GetProperties())
            {
                if(PropertyHelper.IsExposedProperty(_prInfo) && _prInfo.PropertyType.IsSubclassOf(typeof(Set)))
                {
                    Set _set = (Set)(Activator.CreateInstance(_prInfo.PropertyType));
                    _set.Context = this;
                    _prInfo.SetValue(this, _set);
                    __sets.Add(_set);
                }
            }
        }

        private List<Set> __sets = new List<Set>();
        
    }
}
