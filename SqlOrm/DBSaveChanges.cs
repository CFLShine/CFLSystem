using System;
using System.Collections.Generic;
using CFL_1.CFL_System.SqlServerOrm;
using MSTD;
using MSTD.ShBase;
using Npgsql;

namespace SqlOrm
{
    public class DBSaveChanges
    {
        public DBSaveChanges(ShContext context, DBConnection connection)
        {
            __context = context?? throw new ArgumentNullException("context");
            __connection = connection?? throw new ArgumentNullException("connection");
        }
        
        /// <summary>
        /// Effectue la sauvegarde des proipriétés modifiées des objets représentés
        /// dans le <see cref="ShContext"/>.
        /// </summary>
        public bool Exe()
        {
            string _saveChangesQuery = SaveChangesQuery();

            if(__connection.ExecuteNonQuery(new NpgsqlCommand(_saveChangesQuery)))
            {
                foreach(KeyValuePair<Guid, ClassProxy> _kvp in __inserteds)
                    _kvp.Value.IsNew = false;
                foreach(KeyValuePair<Guid, ClassProxy> _kvp in __updateds)
                    _kvp.Value.UpdateProxyValues();// suprime le fait qu'un PropertyProxy se dise changé.

                if(__connection.NotifyChanges &&
                  (__inserteds.Count != 0 || __updateds.Count != 0))
                {
                    DBNotification _notification = new DBNotification();
                    foreach(KeyValuePair<Guid, ClassProxy> _kvp in __inserteds)
                        _notification.AddEntity(_kvp.Value.TypeName.ToLower(), _kvp.Key);
                    foreach(KeyValuePair<Guid, ClassProxy> _kvp in __updateds)
                        _notification.AddEntity(_kvp.Value.TypeName.ToLower(), _kvp.Key);
                    __connection.Notify(_notification);
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Construit la requête de sauvegarde et classe les proxy
        /// dans __inserteds ou __updateds.
        /// </summary>
        private string SaveChangesQuery()
        {
            string _saveChangesQuery = "";
            
            foreach(ClassProxy _proxy in __context.GetProxies())
            {
                if(_proxy.IsNew)
                {
                    _saveChangesQuery += InsertQuery(_proxy);
                    __inserteds[_proxy.ID] = _proxy;
                }
                else
                {
                    List<PropertyProxy> _changes = _proxy.ChangedProperties();
                    if(_changes.Count != 0)
                    {
                        _saveChangesQuery += UpdateQuery(_changes, _proxy);
                        __updateds[_proxy.ID] = _proxy;
                    }
                }
            }
            
            return _saveChangesQuery;
        }

        private string InsertQuery(ClassProxy _proxy)
        {
            List<FieldValue> _fieldsValues = FieldsValues(_proxy.Properties(), _proxy);
            string _query = "INSERT INTO " + _proxy.TypeName.ToLower() +
                              "(objectrepresentation," + Fields(_fieldsValues) + ") " + 
                              "VALUES (" + "'" + SqlCSharp.ObjectRepresentaition(_proxy.Entity) + "'" + "," + Values(_fieldsValues) + ");";
            return _query;
        }

        private string UpdateQuery(IEnumerable<PropertyProxy> _changes, ClassProxy _classAndProxy)
        {
            List<FieldValue> _fieldsValues = FieldsValues(_changes, _classAndProxy);
            return "UPDATE " + _classAndProxy.TypeName.ToLower() + 
                            " SET " + FieldsEqualValues(_fieldsValues, _classAndProxy) +
                             " WHERE " + _classAndProxy.TypeName.ToLower() + ".id = " + 
                             SqlCSharp.SqlValue(_classAndProxy.Entity, TypeHelper.Property(_classAndProxy.Entity.GetType(), "ID")) +
                             ";";
        }

        private List<FieldValue> FieldsValues(IEnumerable<PropertyProxy> _changes, ClassProxy _classAndProxy)
        {
            List<FieldValue> _fieldsValues = new List<FieldValue>();
            
            foreach(PropertyProxy _prProxy in _changes)
            {
                _fieldsValues.Add(new FieldValue(_classAndProxy.Entity, _prProxy.PropertyInfo));
            }
            return _fieldsValues;
        }

        /// <summary>
        /// Retourne une chaine comme : field1,field2,...
        /// </summary>
        private string Fields(List<FieldValue> _changes)
        {
            string _fields = "";
            foreach(FieldValue _fieldValue in _changes)
            {
                if(_fields != "")
                    _fields += ",";
                _fields += _fieldValue.ColumnName;
            }
            return _fields;
        }

        /// <summary>
        /// Retourne une chaine comme : val1,val2,...
        /// </summary>
        private string Values(List<FieldValue> _changes)
        {
            string _values = "";
            foreach(FieldValue _fieldValue in _changes)
            {
                if(_values != "")
                    _values += ",";
                _values += _fieldValue.SqlValue;
            }
            return _values;
        }

        /// <summary>
        /// Retourne une chaine comme : field1=val1,field2=val2,...
        /// </summary>
        private string FieldsEqualValues(List<FieldValue> _changes, ClassProxy _classAndProxy)
        {
            string _fieldsEqualValues = "";
            foreach(FieldValue _fieldValue in _changes)
            {
                if(_fieldsEqualValues != "")
                    _fieldsEqualValues += ",";
                _fieldsEqualValues += _fieldValue.ColumnName +
                                      "=" +
                                      _fieldValue.SqlValue;
            }
            return _fieldsEqualValues;
        }

        private ShContext __context = null;
        private DBConnection __connection = null;
        private Dictionary<Guid, ClassProxy> __inserteds = new Dictionary<Guid, ClassProxy>();
        private Dictionary<Guid, ClassProxy> __updateds = new Dictionary<Guid, ClassProxy>();

    }
}
