using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MSTD;
using MSTD.ShBase;

namespace SqlOrm
{
    /// <summary>
    /// Exemples :
    /// 
    /// pour exprimer Personne.Age > 20 and Personne.Identite.Nom = "DUPONT" and Personne.Identite.Prenom == "Jean" :
    /// 
    /// passer (new MemberPath(
    /// 
    /// </summary>
    public class DBExpression : DBQueryable
    {
        public DBExpression(params object[] elements)
        {
            Add(elements);
        }

        public DBExpression()
        {}

        public void Add(params object[] elements)
        {
            foreach(object o in elements)
                Elements.Add(o);
        }

        public override string Query()
        {
            string _query = "";

            foreach(object _o in Elements)
            {
                if(_o is MemberPath _path)
                {
                    Type _prType = _path.LastPropertyInfo.PropertyType;
                    if(_prType == typeof(Base) || _prType.IsSubclassOf(typeof(Base)))
                        __state = STATE.MEMBEROBJECT;
                    else
                        __state = STATE.MEMBERVALUE;

                    _query = string.Concat(_query, _path.LastPropertyName.ToLower(), " ");
                }
                    
                else
                if(_o is DBSelect _select)
                {
                    if(__state == STATE.MEMBEROBJECT)
                        _select.Select("objectrepresentation");
                    _query = string.Concat(_query, "(", _select.Query(), ") ");
                }

                else
                if(_o is DBQueryable _queryable)
                    _query = string.Concat(_query, _queryable.Query(), " ");

                else
                if(_o is string s)
                {
                    if(__state == STATE.MEMBEROBJECT)
                    {
                        if(s == "=" || s == "in" || s == "IN")
                        {
                            _query = string.Concat(_query, "IN", " ");
                        }
                        else
                            _query = string.Concat(_query, s, " ");
                    }
                    else
                    {
                        DBExpressionBuilder.IsSymbol(ref s);// remplace == par =, && par and, etc

                        _query = string.Concat(_query, s, " ");
                    }
                        
                }

                else
                    throw new Exception("Type non attendu dans les éléments de cette expression.");

            }
            return _query;
        }

        public List<object> Elements = new List<object>();
        
        #region MemberExpression

        public MemberPath GetLowestLevelMember()
        {
            MemberPath _found = null;

            foreach(object o in Elements)
            {
                if(o is MemberPath path)
                {
                    if(_found == null)
                        _found = path;
                    else
                    {
                        if(_found.CommonMembersWith(path) < _found.Count)
                            _found = path;
                    }
                }
            }
            return _found;
        }

        #endregion MemberExpression

        private STATE __state;

        private enum STATE
        {
            MEMBERVALUE,
            MEMBEROBJECT,
        }

    }
}
