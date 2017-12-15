using System;
using System.Collections.Generic;
using System.Reflection;
using MSTD;
using MSTD.ShBase;

namespace SqlOrm
{
    public class DBLoader<T> where T : Base, new()
    {
        public DBLoader(DBConnection connection, ShContext context)
        {
            Connection = connection;
            Context = context;
        }

        public DBConnection Connection { get; private set; }

        public ShContext Context { get; private set; }

        #region Load

        public List<T> ToList(DBSelect select) 
        {
            __select = select;

            Context.StartProcess();

            List<T> _list = new List<T>();
            string _initialQuery = InitialQuery();

            List<ClassProxy> _initials = Load(_initialQuery);

            foreach(ClassProxy _proxy in _initials)
                _list.Add((T)_proxy.Entity);
            
            // procède au chargement des membres objets ou listes d'objets.
            Include(_initials);

            Context.UpdateEntitiesValues();
            Context.EndProcess();

            return _list;
        }

        public T First(DBSelect select) 
        {
            List<T> _list = ToList(select);
            if(_list.Count > 0)
                return _list[0];
            return null;
        }

        private List<ClassProxy> Load(string _query)
        {
            List<ClassProxy> _proxies = new List<ClassProxy>();

            DBReader _reader = new DBReader(Connection, _query);
            while(_reader.Read())
            {
                ClassProxy _classProxy = Context.GetOrAttach(_reader.CurrentRow.GetObjectType(), _reader.CurrentRow.GetId());

                if(_classProxy.Entity == null)
                    _classProxy.CreateNewEntity();

                _classProxy.IsNew = false;
                SetValuesToClassProxy(_classProxy, _reader.CurrentRow);
                _proxies.Add(_classProxy);
                Context.Process(_classProxy);
            }
            return _proxies;
        }

        #endregion Load

        #region initial query

        private string InitialQuery()
        {
            __select.AddSelect("id", "objectrepresentation");

            return __select.Query() + ";";
        }

        #endregion initial query

        #region Include

        public DBLoader<T> IncludeCascade()
        {
            __includeCascade = true;
            return this;
        }
        private bool __includeCascade = false;

        private void Include(List<ClassProxy> _initials)
        {
            if(__select.SelectedMembers.Contains("*"))
            {
                IncludeCascade(_initials);
                return;
            }

            string _includeQuery = "";

            PropertyProxy _prProxy = null;
            foreach(ClassProxy _initial in _initials)
            {
                foreach(string _s in __select.SelectedMembers)
                {
                    _prProxy = _initial.GetPropertyProxy(_s);
                    if(_prProxy != null && _prProxy.IsObject)
                        _includeQuery += IncludeQuery(_initial, _s);
                }
            }

            if(_includeQuery == "")
                return;
            List<ClassProxy> _includeds = Load(_includeQuery);

            if(__includeCascade)
                IncludeCascade(_includeds);
        }

        private void IncludeCascade(List<ClassProxy> _proxies)
        {
            string _includeQuery = "";
            foreach(ClassProxy _proxy in _proxies)
            {
                _includeQuery += IncludeAllQuery(_proxy);
            }

            if(_includeQuery != "")
            {
                _proxies = Load(_includeQuery);
                IncludeCascade(_proxies);
            }
        }

        private string IncludeQuery(ClassProxy _classProxy, string _memberName)
        {
            PropertyProxy _prProxy = _classProxy.GetPropertyProxy(_memberName);
            if(_prProxy is PropertyObjectProxy _prObjectProxy)
                return IncludeClassQuery(_prObjectProxy);
            else
            if(_prProxy is PropertyListProxy _prListProxy)
                return IncludeListQuery(_prListProxy);
            else 
                throw new Exception("Le type " + _prProxy.GetType().Name + " n'est pas prévu pour Include.");
        }
        
        private string IncludeAllQuery(ClassProxy _classProxy)
        {
            string _query = "";
            foreach(PropertyProxy _memberProxy in _classProxy.Properties())
            {
                if(_memberProxy.IsObject)
                    _query += IncludeClassQuery((PropertyObjectProxy)_memberProxy);
                else
                if(_memberProxy.IsList)
                {
                    Type _itemsType = ((PropertyListProxy)_memberProxy).ItemsType;
                    if(_itemsType == typeof(Base) || _itemsType.IsSubclassOf(typeof(Base)))
                        _query += IncludeListQuery((PropertyListProxy)_memberProxy);
                }
            }
            return _query;
        }

        private string IncludeClassQuery(PropertyObjectProxy _memberProxy)
        {
            // si le membre est null
            if(_memberProxy.ID == Guid.Empty)
                return "";

            string _memberGuid = _memberProxy.ID.ToString();

            if(Context.IsProceeded(_memberGuid))
                return "";

            return "SELECT * FROM " + _memberProxy.TypeName.ToLower() + 
                   " WHERE id = " + "'" + _memberGuid + "'" + ";";
        }

        private string IncludeListQuery(PropertyListProxy _listProxy)
        {
            string _query = "";

            foreach(Tuple<Type, Guid> _tuple in _listProxy.ObjectsRepresentations())
            {
                string _tableName = _tuple.Item1.Name.ToLower();
                string _guidstr = _tuple.Item2.ToString();
                if(!Context.IsProceeded(_guidstr))
                {
                    _query +=
                    "SELECT * FROM " + _tableName + 
                    " WHERE id = " + "'" + _guidstr + "'" + ";";
                }
            }
            return _query;
        }

        #endregion Include

        #region give values to object

        /// <summary>
        /// Les données sont extraites de la ligne en cours du reader.
        /// </summary>
        public void SetValuesToClassProxy(ClassProxy _proxy, DBRow _row)
        {
            _proxy.ID = _row.GetId();

            for (int _i = 0; _i < _row.Count; _i++)
            {
                DBField _field = _row.GetField(_i);
                string _propertyName = _field.PropertyName;

                PropertyProxy _prProxy = _proxy.GetPropertyProxy(_propertyName);

                if(_prProxy != null)
                {
                    SetValueToProperty(_prProxy, _field.Value);
                }
            }
        }

        private void SetValueToProperty(PropertyProxy _prProxy, object _value)
        {
            Type _valueType = _value.GetType();
            Type _prType = _prProxy.Type;

            if(_value is DBNull)
            {
                if (TypeHelper.IsNullable(_prType))
                    _prProxy.Value = null;
                
                else 
                if(_prProxy is PropertyObjectProxy _prObjectProxy)
                    _prObjectProxy.Value = "";
                
                else
                if(_prProxy is PropertyListProxy _prListProxy)
                    _prListProxy.Parse("");
                
                else
                if(_prType == typeof(bool))
                    _prProxy.Value = false;
                
                else
                if(_prType == typeof(int)
                || _prType == typeof(long)
                || _prType == typeof(double)
                || _prType == typeof(decimal))
                    _prProxy.Value = 0;
                
                else
                if(_prType == typeof(string))
                    _prProxy.Value = "";
            }
            
            else
            if(_prProxy is PropertyObjectProxy _prObjectProxy)
                _prObjectProxy.Value = _value;
            
            else
            if(_prProxy is PropertyListProxy _prListProxy)
                _prListProxy.Parse((string)_value);

            else 
            if(_prProxy is PropertyEnumProxy _enumProxy)
                _enumProxy.Value = _value;

            else
            if(_valueType != _prType)
            {
                if(_valueType == typeof(decimal) && _prType == typeof(double))
                    _prProxy.Value = Convert.ToDouble(_value);
                else 

                if (TypeHelper.IsNullable(_prType))
                {
                    if(_prType == typeof(bool?))
                        _prProxy.Value =  (bool?)_value;
                    else
                    if(_prType == typeof(int?))
                        _prProxy.Value = (int?)_value;
                    else
                    if(_prType == typeof(long?))
                        _prProxy.Value = (long?)_value;
                    else
                    if(_prType == typeof(double?))
                        _prProxy.Value = (double?)_value;
                    else
                        if(_prType == typeof(decimal?))
                        _prProxy.Value = (decimal?)_value;
                    else
                        if(_prType == typeof(DateTime?))
                        _prProxy.Value = (DateTime?)_value;
                    else
                    if(_prType == typeof(TimeSpan?))
                        _prProxy.Value = (TimeSpan?)_value;
                }

                else throw new Exception("_propertyType = " + _prType.Name + Environment.NewLine + 
                                         "_valueType = " + _valueType.Name + Environment.NewLine +
                                         "Cast de ces types non prévu dans cette fonction.");
            }
            else
                _prProxy.Value = _value;
        }

        #endregion

        private DBSelect __select = null;
    }
}
