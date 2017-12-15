using System;
using System.Collections.Generic;
using System.Reflection;
using MSTD.ShBase;

namespace MSTD
{
    public class SolutionClasses
    {
        /// <summary>
        /// Retourne le premier Type nommé typename.
        /// Non semsible à la casse.
        /// </summary>
        public static Type Type(string typename)
        {
            foreach(Type _t in Instance.Types)
            {
                if(_t.Name.ToLower() == typename.ToLower())
                    return _t;
            }
            return null;
        }

        public static Base Factory(string _typename)
        {
            Type _type = Type(_typename);
            return (_type != null) ? (Base)(Activator.CreateInstance(_type)) : null;
        }

        public List<Type> Types { get; private set; } = new List<Type>();
        
        private void Init()
        {
            Type[] _types = Assembly.GetExecutingAssembly().GetTypes();
            foreach(Type _type in _types)
            {
                if(_type.IsSubclassOf(typeof(Base)))
                    Types.Add(_type);
            }
        }

        private static SolutionClasses Instance
        {
            get
            {
                if(__instance == null)
                    __instance = new SolutionClasses();
                return __instance;
            }
        }

        private static SolutionClasses __instance;
        private SolutionClasses() 
        {
            Init();    
        }
    }
}
