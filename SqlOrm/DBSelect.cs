
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using MSTD;
using MSTD.ShBase;

namespace SqlOrm
{
    public class DBSelect : DBQueryable
    {
        public DBSelect(){}

        public DBSelect(params string[] members)
        {
            Select(members);
        }

        public DBSelect(params MemberPath[] members)
        {
            Select(members);
        }

        public string TableName { get; private set; }

        public DBSelect Select(params string[] members)
        {
            if(members != null && members.Length > 0)
            {
                __selectedMembers.Clear();
                if(members[0] == "*" || members[0] == "all" || members[0] == "ALL")
                {
                    __selectedMembers.Add("*");
                }
                else
                {
                    foreach(string _member in members)
                        __selectedMembers.Add(_member);
                }
            }
            else
                __selectedMembers.Clear();
            return this;
        }

        public DBSelect Select(params MemberPath[] members)
        {
            __selectedMembers.Clear();
            foreach(MemberPath _path in members)
            {
                __selectedMembers.Add(_path.LastPropertyName.ToLower());
            }
            return this;
        }

        /// <summary>
        /// Ajoute des noms de membres à selectionner en vérifiant qu'ils ne le soient déja.
        /// </summary>
        public DBSelect AddSelect(params string[] members)
        {
            if(!__selectedMembers.Contains("*"))
            {
                foreach(string _member in members)
                {
                    string _m = _member.ToLower();
                    if(!__selectedMembers.Contains(_m))
                        __selectedMembers.Add(_m);
                }
            }
            return this;
        }

        public DBSelect From(Type type)
        {
            TableName = type.Name.ToLower();
            return this;
        }

        public DBSelect From(string tableName)
        {
            TableName = tableName.ToLower();
            return this;
        }

        /// <summary>
        /// elements : expression formée d'éléments acceptés par <see cref="DBExpression"/>
        /// <see cref="DBExpression"/>.
        /// </summary>
        public DBSelect Where(params object[] elements)
        {
            if(__where == null)
                __where = new DBExpression();
            __where.Add(elements);
            return this;
        }

        /// <summary>
        /// Retourne l'expression WHERE de ce select, jamais null.
        /// </summary>
        public DBExpression GetWhere()
        {
            if(__where == null)
                __where = new DBExpression();
            return __where;
        }

        public override string Query()
        {
            string _members = __selectedMembers_listFormat();
                
            string _sqlSelect =  "SELECT " + _members +
                       " FROM " + TableName + " ";
            if(__where != null)
                    _sqlSelect += " WHERE " + __where.Query();
            
            return _sqlSelect;
        }

        public List<string> SelectedMembers => __selectedMembers;

        private string __selectedMembers_listFormat()
        {
            if(__selectedMembers.Count > 0 && __selectedMembers[0] == "all" || __selectedMembers[0] == "*")
                    return "*";
            string _members = "";
            foreach(string _member in __selectedMembers)
            {
                if(_members != "")
                    _members += ",";
                _members += _member;
            }
            return _members;
        }

        protected List<string> __selectedMembers = new List<string>();

        protected DBExpression __where = null;
    }

    public class DBSelect<T> :DBSelect where T: Base, new()
    {
        public DBSelect()
            :base()
        {
            From(typeof(T));
        }

        public DBSelect(params string[] _members)
        {
            From(typeof(T));
            Select(_members);
        }
    }

}
