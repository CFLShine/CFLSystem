using System;
using System.Collections.Generic;
using System.Reflection;
using MSTD;
using MSTD.ShBase;
using Newtonsoft.Json;

namespace CFL_1.CFL_System.MSTD
{
    /*
{
 "Défunt": 
 {
  "Etat civil":
  {
   "Identite": 
   {
    "properties": 
    {
     "GUID": 
     {
      "label": "Nom",
      "type": "input/text",
     }
    }
   },
   "properties": 
   {
    "GUID2": 
    {
     "type":"input/numeric",
     "label":"Taille du sexe"
    },
    "GUID3": 
    {
     "type": "input/checkbox",
     "label": "Est mort"
    },
    "GUID4": 
    {
     "type": "list",
     "label": "Commune du décès",
     "data": 
     {
      "GUID5": "Aix-les-Bains",
      "GUID6": "Pugny-Chatenod"
     }
    }
   }
  }
 }
}
    retour de l'interface :

{
 "GUID": "valeur",
 "GUID2": "valeur"
}

https://www.w3schools.com/html/html_form_input_types.asp

    */

    public static class BaseProxyToJson
    {
        public static string ProduceJSon(ClassProxy proxy)
        {
            if(proxy == null)
                throw new ArgumentNullException("_proxy");

            Dictionary<string, object> _properties = ClassProxyRepresentation(proxy, 0);
            Dictionary<string, object> _object = new Dictionary<string, object>()
            {
                { proxy.TypeName, _properties }
            };
            
            string _json = JsonConvert.SerializeObject(_object, Formatting.Indented);
            return _json;
        }

        private static Dictionary<string, object> ClassProxyRepresentation(ClassProxy _proxy, int level = -1)
        {
            Dictionary<string, object> _dic = new Dictionary<string, object>();

            if(level != -1)
                _dic.Add("Level", level);

            // Properties
            Dictionary<string, object> _properties = new Dictionary<string, object>();

            foreach(PropertyProxy _prProxy in _proxy.PropertiesNoObject())
            {
                Dictionary<string, object> _prRepesentation = PropertyRepresentation(_prProxy);
                _properties.Add(_prProxy.ID.ToString(), _prRepesentation);
            }

            // Objects
            foreach(PropertyObjectProxy _prProxy in _proxy.PropertiesObjects())
            {
                AttributeSelected _attr = _prProxy.PropertyInfo.GetCustomAttribute<AttributeSelected>();
                if(_attr != null)
                {
                    Dictionary<string, object> _property = new Dictionary<string, object>();
                    Dictionary<string, object> _data = new Dictionary<string, object>();

                    _property.Add("Label", _prProxy.Label);

                    if(_prProxy.Value != null)
                    {
                        _property.Add("Selected", ((ClassProxy)_prProxy.Value).ID.ToString());
                    }

                    foreach(object _o in _attr.DataSource())
                    {
                        Base _base = _o as Base;
                        _data.Add(_base.ID.ToString(), ClassProxyRepresentation(_proxy.Context.GetOrAttach(_base)));
                    }
                    _property.Add("data", _data);
                    _properties.Add(_prProxy.ID.ToString(), _property);
                }
                else
                    _dic.Add(_prProxy.Name, ClassProxyRepresentation((ClassProxy)_prProxy.Value, level + 1));
            }
            
            if(_properties.Count > 0)
                _dic.Add("Properties", _properties);

            return _dic;
        }

        private static Dictionary<string, object> PropertyRepresentation(PropertyProxy _prProxy)
        {
            Dictionary<string, object> _dic = new Dictionary<string, object>();
            _dic.Add("Label", _prProxy.Label);
            _dic.Add("Type", _prProxy.TypeName);
            _dic.Add("Value", _prProxy.Value);
            return _dic;
        }
        
    }
}
