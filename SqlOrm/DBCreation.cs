
using System;
using System.Reflection;
using MSTD.ShBase;
using Npgsql;

namespace SqlOrm
{
    public class DBCreation
    {
        public DBCreation(DBConnection _connection,
                          ShContext _context)
        {
            __connection = _connection;
            __context = _context;
        }

        /// <summary>
        /// Crée la db si elle n'existe pas,
        /// puis chaque table représentée par un <see cref="DBSet"/> du <see cref="DBContext"/> si elle n'existe pas.
        /// La connection est retournée fermée.
        /// </summary>
        public bool CreateOrCompleteDataBase()
        {
            return CreateDbIfNotExists() && CreateTablesIfNotExist();
        }

        private bool CreateDbIfNotExists()
        {
            DBConnection _connection = new DBConnection()
            {
                Server = __connection.Server,
                DataBase = "postgres",
                UserId = __connection.UserId,
                Password = __connection.Password,
            };

            if(!_connection.Connect())
                return false;

            NpgsqlCommand _command = new NpgsqlCommand()
            {
                CommandText = string.Format("CREATE DATABASE {0} " +
                                            "WITH OWNER = postgres " +
                                            "ENCODING = 'UTF8' " +
                                            "CONNECTION LIMIT = -1 "  +
                                            "TABLESPACE = pg_default " +         
                                            "LC_COLLATE = 'French_France.1252' " +
                                            "LC_CTYPE = 'French_France.1252' ;", 
                                            __connection.DataBase),
                Connection = _connection.Connection
            };

            try
            {
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if(ex is PostgresException _pgex)
                {
                    if(_pgex.SqlState != "42P04")
                        throw ex;
                }
            }
            
            _connection.Close();

            // si l'execution n'a pas levé d'erreur : 
            return ConnectToDb();
        }

        #region Create tables

        private bool CreateTablesIfNotExist()
        {
            foreach(Set _set in __context.GetSets())
            {
                if(!CreateTableIfNotExists(_set))
                    return false;
            }
            return true;
        }

        private bool CreateTableIfNotExists(Set _set)
        {
            string _tableName = _set.Type.Name.ToLower();
            string _columnNames_types = ColumnNames_types(_set.Type);

            if(!string.IsNullOrWhiteSpace(_columnNames_types))
                _columnNames_types = "," + _columnNames_types;
            _columnNames_types = "objectrepresentation TEXT, id UUID PRIMARY KEY" + _columnNames_types;

            string _query = "CREATE TABLE IF NOT EXISTS " + _tableName + " " +
                              "(" + _columnNames_types + ");";

            if(!__connection.Connect())
                return false;

            NpgsqlCommand _command = new NpgsqlCommand()
            {
                CommandText = _query,
            };

            __connection.ExecuteNonQuery(_command);

            return __connection.ExecuteNonQuery(_command);
        }

        /// <summary>
        /// Retourne une chaine comme 'col1 int2,col2 text,col3 uuid ...',
        /// Utile pour la requete 'CREATE TABLE IF NOT EXISTS tablename (col1 int2,col2 text,col3 uuid...);.
        /// </summary>
        private string ColumnNames_types(Type _type)
        {
            string _columns = "";
            foreach(PropertyInfo _prInfo in _type.GetProperties())
            {
                string _columnName = SqlCSharp.ColumnName(_prInfo).ToLower();
                if(_columnName == "id")
                    continue;

                //ne répond pas aux conditions pour être mappée
                if(_columnName == "") 
                    continue;

                string _typestr = "";
                SqlType _sqlType = SqlCSharp.GetSqlType(_prInfo.PropertyType);

                if(_sqlType == SqlType.CLASS)
                    _typestr = SqlCSharp.GetSqlTypeStr(SqlCSharp.GetSqlType(typeof(string)));
                else
                if(_sqlType == SqlType.ENUM)
                    _typestr = SqlCSharp.GetSqlTypeStr(SqlCSharp.GetSqlType(typeof(int)));
                else
                if(_sqlType == SqlType.LIST)
                    _typestr = SqlCSharp.GetSqlTypeStr(SqlCSharp.GetSqlType(typeof(string)));
                else
                    _typestr = SqlCSharp.GetSqlTypeStr(_sqlType);

                if(_typestr == "")
                    throw new Exception("Un type de propriété n'a pas sa correspondance sql. \n" +
                                        "propriété " + _prInfo.Name + " de type " + _prInfo.PropertyType.Name + "\n" +
                                        "dans l'objet de type " + _type.Name);

                if(_columns != "")
                    _columns += ",";

                _columns += _columnName + " " + _typestr;

            }
            return _columns;
        }

        #endregion Create tables
       
        /// <summary>
        /// Connecte __connection à la db nouvellement crée ou existante.
        /// </summary>
        /// <returns></returns>
        private bool ConnectToDb()
        {
            bool _ok = __connection.Connect();
            __connection.Close();
            return _ok;
        }

        private DBConnection __connection = null;
        private ShContext __context = null;
    }
}
