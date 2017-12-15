using System;
using System.Collections.Generic;

namespace MSTD.ShBase
{
    public class Set
    {
        public Set(Type type)
        {
            Type = type?? throw new ArgumentNullException("type");
        }

        /// <summary>
        /// Type des entités
        /// </summary>
        public Type Type
        {
            get;
            private set;
        }

        public ShContext Context { get; set; } = null;

        public ClassProxy Factory()
        {
            Base _entity = (Activator.CreateInstance(Type)) as Base;
            ClassProxy _proxy = new ClassProxy(Context, _entity)
            {
                Context = Context
            };
            return _proxy;
        }

        /// <summary>
        /// Trouve ou ajoute au <see cref="Set"/> le <see cref="ClassProxy"/> construit
        /// sur entity.
        /// </summary>
        public ClassProxy GetOrAttachProxy(Base entity)
        {
            ClassProxy _proxy = GetProxy(entity);
            if(_proxy == null)
            {
                _proxy = new ClassProxy(Context, entity)
                {
                    Context = Context
                };
                AddProxy(_proxy);
            }

            return _proxy;
        }

        /// <summary>
        /// Ajoute un <see cref="ClassProxy"/> à ce <see cref="Set"/>.
        /// Ne vérifie pas qu'il n'y soit déja.
        /// </summary>
        public void AddProxy(ClassProxy proxy)
        {
            __proxies[proxy.ID] = proxy;
        }

        public bool IsAttached(Base entity)
        {
            return GetProxy(entity) != null;
        }

        /// <summary>
        /// Retourne la <see cref="ClassProxy"/> de entity, 
        /// ou null si non trouvé.
        /// </summary>
        public ClassProxy GetProxy(Base entity)
        {
            if(entity == null)
                return null;
            return GetProxy(entity.ID);
        }

        public ClassProxy GetProxy(Guid id)
        {
            ClassProxy _classProxy = null;
            __proxies.TryGetValue(id, out _classProxy);
            return _classProxy;
        }

        public IEnumerable<ClassProxy> GetProxies()
        {
            foreach(ClassProxy _proxy in __proxies.Values)
            {
                yield return _proxy;
            }
        }

        public void UpdateEntities()
        {
            foreach(ClassProxy _proxy in __proxies.Values)
            {
                if(_proxy != null)
                    _proxy.UpdateEntityValues();
            }
        }

        private Dictionary<Guid, ClassProxy> __proxies = new Dictionary<Guid, ClassProxy>();
    }

    public class Set<T> : Set where T : class
    {
        public Set()
            :base(typeof(T))
        { }
    }
}
