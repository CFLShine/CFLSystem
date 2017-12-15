using System;
using System.Reflection;

namespace SqlOrm
{
    /// <summary>
    /// <see cref="FieldValue"/> est utilisé par <see cref="DBSaveChanges"/> pour 
    /// faciliter la représentation d'une propriété CSharp en champs d'une table et constituer une liste
    /// des champs à update.
    /// </summary>
    public class FieldValue
    {
        /// <summary>
        /// <see cref="FieldValue"/> est utilisé par <see cref="DBSaveChanges"/> pour 
        /// faciliter la représentation d'une propriété CSharp en champs d'une table et constituer une liste
        /// des champs à update.
        /// </summary>
        public FieldValue(object _class, PropertyInfo _prInfo)
        {
            Class = _class;
            PrInfo = _prInfo;
            SqlValueType = SqlCSharp.GetSqlType(_prInfo.PropertyType);
        }

        public object Class 
        {
            get;
            private set;
        } = null;

        public PropertyInfo PrInfo
        {
            get ;
            private set ;
        }

        public string ColumnName
        {
            get { return SqlCSharp.ColumnName(PrInfo) ; }
        } 

        public SqlType SqlValueType
        {
            get;
            private set;
        }

        public object Value
        {
            get
            {
                 return PrInfo.GetValue(Class);
            }
        } 
        
        public string SqlValue
        {
            get
            {
                string _sqlValue = SqlCSharp.SqlValue(Value, SqlValueType, SqlValueType == SqlType.LIST);
                if(string.IsNullOrWhiteSpace(_sqlValue))
                    throw new Exception("Aucune valeur Sql n'a été retournée.");
                return _sqlValue;
            }
        }

    }
}
