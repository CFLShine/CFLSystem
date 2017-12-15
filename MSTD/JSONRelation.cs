using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace MSTD
{
    /// <summary>
    /// Expose les méthodes statiques <seealso cref="string serialize(T _object, Type[] _knownTypes = null)"/>
    /// et static T deserialize(string _s, Type[] _knownTypes = null)
    /// </summary>
    public class JSONRelation <T> where T : new()
    {
        /// <summary>
        /// Préciser _knownTypes si l'objet à serialiser contient des membres objets de classe
        /// ou des membres collections d'objet de classe.
        /// Ce sont alors ces types de classe à passer dans _knownTypes.
        /// </summary>
        public static string serialize(T _object, params Type[] knownTypes)
        {
            List<Type> _knownTypes = new List<Type>();
            foreach(Type _t in knownTypes)
            {
                _knownTypes.Add(_t);
            }
            DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(T), _knownTypes);
            MemoryStream _ms = new MemoryStream();
            
            
            _serializer.WriteObject(_ms, _object);
            byte[] _json = _ms.ToArray();
            _ms.Close();
            string _s = Encoding.UTF8.GetString(_json, 0, _json.Length);
            return _s;
        }

        /// <summary>
        /// Préciser _knownTypes si l'objet à déserialiser contient des membres objets de classe
        /// ou des membres collections d'objet de classe.
        /// Ce sont alors ces types de classe à passer dans _knownTypes
        /// </summary>
        public static T deserialize(string _s, Type[] knownTypes = null)
        {
            List<Type> _knownTypes = new List<Type>();
            foreach(Type _t in knownTypes)
            {
                _knownTypes.Add(_t);
            }

            T _object = default(T);
            MemoryStream _ms = new MemoryStream(Encoding.UTF8.GetBytes(_s));
            DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(T));
            _object = (T)_serializer.ReadObject(_ms);
            _ms.Close();
            return _object;
        }
    }
}
