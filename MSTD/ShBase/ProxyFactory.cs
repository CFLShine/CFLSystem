using System;
using System.Reflection;

namespace MSTD.ShBase
{
    public static class ProxyFactory
    {
        public static ClassProxy ClassProxyFactory(ShContext context, ref ClassProxy proxy, Type t, Guid id)
        {
            if(proxy == null)
                return new ClassProxy(context, t, id);// appellera cette fonction à nouveau.
           
            proxy.Context = context;
            proxy.Type = t;

            if(proxy.ID == Guid.Empty)
                proxy.ID = id;

            foreach(PropertyInfo _pr in t.GetProperties())
            {
                if(PropertyHelper.IsMappableProperty(_pr))
                {
                    proxy.SetProperty(_pr.Name, PropertyProxyFactory(context, _pr, proxy));
                }
            }

            return proxy;
        }

        public static PropertyProxy PropertyProxyFactory(ShContext context, PropertyInfo prInfo, ClassProxy parent)
        {
            if(TypeHelper.IsPrimitiveOrAlike(prInfo.PropertyType))
                return new PropertyPrimitiveProxy(context, prInfo, parent);
            if(prInfo.PropertyType.IsSubclassOf(typeof(Base)))
                return new PropertyObjectProxy(context, prInfo, parent);
            if(TypeHelper.IsGenericList(prInfo.PropertyType))
                return new PropertyListProxy(context, prInfo, parent);
            if(TypeHelper.IsDictionary(prInfo.PropertyType))
                return new PropertyDictionaryProxy(context, prInfo, parent);
            if(prInfo.PropertyType.IsEnum)
                return new PropertyEnumProxy(context, prInfo, parent);
            
            throw new Exception("Le type " + prInfo.PropertyType.Name + " n'est pas pris en compte.");
        }

    }
}
