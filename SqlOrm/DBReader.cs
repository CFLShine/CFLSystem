using System;
using System.Collections.Generic;
using Npgsql;

namespace SqlOrm
{
    public class DBField
    {
        public string Name
        {
            get;
            set;
        } = "";

        /// <summary>
        /// La valeur retournée par la DB, récupérée dans le NpgsqlDataReader.
        /// </summary>
        public object Value
        {
            get;
            set;
        } = null;

        /// <summary>
        /// Retourne le nom de la propriété CSharp (en lowercase) représentée 
        /// par ce DBField.
        /// Si la propriété est un objet ou une list,
        /// retourne le dernier élément de class_typename_membername, ou de list_typename_membername,
        /// sinon, Name lui-même.
        /// Il n'est pas garanti que cette propriété existe dans l'objet csharp, car ce champ
        /// peut être un champ ajouté pour le fonctionnement de l'orm.
        /// </summary>
        public string PropertyName
        {
            get
            {
                if(Name.Contains("_"))
                {
                    string[] _elements = Name.Split('_');
                    return _elements[_elements.Length -1];
                }
                return Name;
            }
        }
    }

    public class DBRow : List<DBField>
    {
        public DBField GetField(int index)
        {
            return this[index];
        }

        public DBField GetField(string propertyName)
        {
            foreach(DBField _field in this)
            {
                if(_field.PropertyName == propertyName.ToLower())
                    return _field;
            }
            return null;
        }

        public string GetFieldName(int index)
        {
            return this[index].Name;
        }

        public string GetPropertyName(int index)
        {
            return this[index].PropertyName;
        }

        public Guid GetId()
        {
            return (Guid)GetField("id").Value;
        }

        public string GetObjectType()
        {
            string _objectrepresentation = (string)GetField("objectrepresentation").Value;
            return _objectrepresentation.Split('_')[0];
        }
    }

    public class DBReader
    {
        public DBReader(DBConnection _connection, string _query)
        {
            NpgsqlCommand _command = new NpgsqlCommand(_query);
            Init(_connection.ExecuteQuery(_command));
        }

        private void Init(NpgsqlDataReader _reader)
        {
            if(_reader == null)
                return;

            __rows = new List<DBRow>();
            
            DBRow _row = null;

            using(_reader)
            {
                while(_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        if(_reader.FieldCount > 0)
                        {
                            string _firstColumn = _reader.GetName(0);

                            for (int _i = 0; _i < _reader.FieldCount; _i++)
                            {
                                DBField _field = new DBField();
                                _field.Name = _reader.GetName(_i);
                                _field.Value = _reader.GetValue(_i);

                                if (_field.Name == _firstColumn)
                                {
                                    _row = new DBRow();
                                    __rows.Add(_row);
                                }
                                _row.Add(_field);
                            }
                        }
                    }
                    _reader.NextResult();
                }
            }
        }

        /// <summary>
        /// Se positionne avant la première ligne.
        /// </summary>
        public void GotoStart()
        {
            __currentRowIndex = -1;
            __currentRow = null;
        }

        /// <summary>
        /// S'il reste au moins une ligne à lire, se positionne dessus et retourne true, sinon false.
        /// </summary>
        public bool Read()
        {
            if (__rows == null || ++__currentRowIndex >= __rows.Count)
                return false;
            __currentRow = __rows[__currentRowIndex];
            return true;
        }

        /// <summary>
        /// Permet de s'assurer qu'au moins un Read a été fait et qu'il y a une ligne en cours à lire.
        /// </summary>
        public bool OnRow
        {
            get
            {
                return __currentRow != null;
            }
        }

        /// <summary>
        /// Retourne le nombre de lignes.
        /// </summary>
        /// <returns></returns>
        public int RowCount
        { 
            get 
            { 
                return __rows.Count; 
            } 
        }

        public DBRow CurrentRow
        {
            get
            {
                return __currentRow;
            }
        }

        private DBRow __currentRow = null;
        private int __currentRowIndex = -1;

        private List<DBRow> __rows = null;
    }
}
