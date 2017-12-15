

using System;
using System.Collections;
using MSTD.ShBase;

namespace SqlOrm
{
    /// <see cref="SqlType.BOOL"/>     : bool.
        /// <see cref="SqlType.INTEGER"/>  : short, int.
        /// <see cref="SqlType.BIGINT"/>   : long.
        /// <see cref="SqlType.NUMERIC"/>  : decimal, double.
        /// <see cref="SqlType.TEXT"/>     : string.
        /// <see cref="SqlType.UUID"/>     : Guid.
        /// <see cref="SqlType.DATE"/>     : DateTime ou DateTime?.
        /// <see cref="SqlType.TIME"/>     : TimeSpan ou TimeSpan?.
        /// 
        /// Non sql, utiles pour identifier le type d'une propriété :
        /// <see cref="SqlType.CLASS"/> propriété de type <see cref="Base"/> ou dérivé.
        /// <see cref="SqlType.ENUM"/>
        /// <see cref="SqlType.LIST"/>
        
    public class DBValue : DBQueryable
    {
        public DBValue(string value)
        {
            Value = value;
            SqlType = SqlType.TEXT;
        }

        public DBValue(Guid value)
        {
            Value = value;
            SqlType = SqlType.UUID;
        }

        public DBValue(bool value)
        {
            Value = value;
            SqlType = SqlType.BOOL;
        }

        public DBValue(int value)
        {
            Value = value;
            SqlType = SqlType.INTEGER;
        }

        public DBValue(short value)
        {
            Value = value;
            SqlType = SqlType.INTEGER;
        }

        public DBValue(long value)
        {
            Value = value;
            SqlType = SqlType.BIGINT;
        }

        public DBValue(double value)
        {
            Value = value;
            SqlType = SqlType.NUMERIC;
        }

        public DBValue(decimal value)
        {
            Value = value;
            SqlType = SqlType.NUMERIC;
        }

        public DBValue(DateTime value)
        {
            Value = value;
            SqlType = SqlType.DATE;
        }

        public DBValue(DateTime? value)
        {
            Value = value;
            SqlType = SqlType.DATE;
        }

        public DBValue(TimeSpan value)
        {
            Value = value;
            SqlType = SqlType.TIME;
        }

        public DBValue(TimeSpan? value)
        {
            Value = value;
            SqlType = SqlType.TIME;
        }

        public DBValue(Base value)
        {
            Value = value;
            SqlType = SqlType.CLASS;
        }

        public DBValue(IList value)
        {
            Value = value;
            SqlType = SqlType.LIST;
        }

        public DBValue(object value)
        {
            if(value != null)
            {
                if(value.GetType().IsEnum)
                {
                    SqlType = SqlType.ENUM;
                }
                else
                    throw new NotSupportedException("Le type " + value.GetType().Name + " n'est pas supporté.");
            }
            Value = value;
        }

        public override string Query()
        {
            if(Value == null)
                return "NULL";
            return SqlCSharp.SqlValue(Value, SqlType, InList);
        }

        public bool InList{ get; set; } = false;

        public object Value{ get; private set; }

        public SqlType SqlType{ get; private set; } = SqlType.NOTMAPPED;
    }

    
}
