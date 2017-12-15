

using System;
using MSTD;

namespace SqlOrm
{
    /// <summary>
    /// exemple d'utilisation :
    /// 
    /// pour
    /// 
    /// MyClass.Identite.nom == "DUPONT" 
    /// and (MyClass.Personne.Identite.prenom == "Jean" or
    ///      MyClass.Personne.Identite.prenom = "Jeanne")
    /// 
    /// passer
    /// 
    /// new MemberPath(()=>((Myclass)null).Personne.Identite.nom), "=", "DUPONT", 
    /// and, "(", new MemberPath(()=>((Myclass)null).Personne.Identite.prenom), "=", "Jean", "or",
    ///           new MemberPath(()=>((Myclass)null).Personne.Identite.prenom), "=", "Jeannne", ")"
    /// 
    /// donne
    /// 
    /// personne = (SELECT objectrepresentation FROM personne 
    /// WHERE identite = (SELECT objectrepresentaion FROM indentte
    ///       WHERE nom = 'DUPONT' and (prenom = 'Jean' or prenom = 'Jeanne') ))
    /// </summary>
    public class DBMemberExpressionComparison : DBQueryable
    {
        public DBMemberExpressionComparison(params object[] expression)
        {
            __expression = expression;
            GetPath();
        }

        public override string Query()
        {
            BuildQuery();
            return __query;
        }

        public MemberPath Path { get; set; } = null;

        private void BuildQuery()
        {
            if(Path != null)
                __query = MemberExpression(Path);

            foreach(object _element in __expression)
            {
                if(_element is DBMemberExpressionComparison other)
                {
                    if(Path != null && Path.Count > 1)
                    {
                        if(__lastOperatorLogicPos != -1)
                        {
                            if(other.Path != null)
                            {
                                int _parenthesis = Path.Count - Path.CommonMembersWith(other.Path) - 1;

                                if(_parenthesis > 0) // dans le cas ou Path et other.Path expriment le mêmes objets, _parenthesis == -1
                                {
                                    __query = __query.Insert(__lastOperatorLogicPos, new string(')', _parenthesis));
                                    __openedParenthesis -= _parenthesis;
                                }
                            }
                            __lastOperatorLogicPos = -1;
                        }
                        other.Path = RemoveCommonMembers(other.Path);
                    }
                    __query += other.Query();
                }
                else
                if(_element is MemberPath _path)
                    __query += _path.LastPropertyName.ToLower() + " ";
                else
                if(_element is string s)
                {
                    if(DBExpressionBuilder.IsOperatorLogic(ref s))
                    {
                        __lastOperatorLogicPos = __query.Length;
                        __query += s + " ";
                    }
                    else
                    if(DBExpressionBuilder.IsSymbol(ref s))
                    {
                        __query += s + " ";
                        if(s == "(")
                            ++__openedParenthesis;
                        else
                        if(s == ")")
                            --__openedParenthesis;
                    }
                    else
                    __query += SqlCSharp.SqlValue(_element) + " ";
                }
                
                else
                    __query += SqlCSharp.SqlValue(_element) + " ";
            }
            __query += new string(')', __openedParenthesis);
        }

        private string MemberExpression(MemberPath path)
        {
            if(path.Count == 1)
                return "";
            
            string _select = path.Names[0].ToLower() + " = (SELECT objectrepresentation FROM " + 
                path.Properties[0].PropertyType.Name.ToLower() +
                " WHERE ";

            ++__openedParenthesis;

            if(path.Properties.Count > 2)
                _select += MemberExpression(path.Shift()) ;
            return _select ;
        }

        private void GetPath()
        {
            foreach(object _element in __expression)
            {
                if(_element is MemberPath _path)
                {
                    Path = _path;
                    return;
                }
            }
        }

        /// <summary>
        /// Path :  personne.coordonnees (.adreesse)
        /// other : personne.coordonnees.commune (.nom)
        /// ne doit rester que : commune
        /// Est assumé que jusqu'à commune les membres sont les mêmes et que other a autant de noms ou plus que Path.
        /// </summary>
        private MemberPath RemoveCommonMembers(MemberPath other)
        {
            MemberPath _new = new MemberPath();
            for(int _i = 0; _i < other.Count; _i ++)
            {
                if(_i >= Path.Count || Path.Names[_i] != other.Names[_i])
                    _new.AddProperty(other.Properties[_i]);
            }
            return _new;
        }
        
        private object[] __expression { get; set; }
        
        private string __query = "";

        private int __openedParenthesis = 0;
        private int __lastOperatorLogicPos = -1;
    }
}
