using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace RuntimeExec
{
    /// <summary>
    /// Exemple :
    /// Soit un object ObjectA de classe A, 
    /// une propriété PrB de ObjectA retournant un objet ObjectB de class B, 
    /// une propriété PrInt de objectB retournant un int,
    /// l'expression est : 
    /// 
    ///           ObjectA.PrB.PrInt
    ///           
    /// Le <see cref="REMemberExpression"/> sera alors fait comme suit :
    /// - un <see cref="REMemberExpression"/> correspondant à PrB, <see cref="REMemberExpression.TreeAncestor"/>  <see cref="REClassObject"/>(ObjectA)
    /// - un <see cref="REMemberExpression"/> correspondant à PrInt, <see cref="REMemberExpression.TreeAncestor"/>  <see cref="REClassObject"/>(ObjectB),
    /// qui contient la <see cref="REProperty"/> PrInt.
    /// 
    /// <see cref="REMemberExpression.ReValue"/> donne ou retourne la valeur <see cref="REExpression"/>
    /// de la <see cref="REExpression"/> contenue dans dernier membre de l'expression.
    /// Le premier objet de l'expression peut être changé en assignant <see cref="REMemberExpression.TreeAncestor"/>
    /// ce qui provoque la reconstruction de l'expression.
    /// </summary>
    public class REMemberExpression : REMember
    {
        public REMemberExpression(){ }

        /// <summary>
        /// Pour une expression A.b.c, passer en argument la <see cref="REClassObject"/> du type A,
        /// et ()=>((A)null).b.c
        /// </summary>
        public REMemberExpression(REClassObject parent, Expression<Func<object>> _expression)
        {
            MembersNames(_expression);
            Parent = parent??throw new Exception
                              (@"_parent ne doit pas être null. 
                                 Si _parent ne peut être aquis au moment de cet instanciation, 
                                 utiliser un constructeur réclamant string _parentTypeName, ou Type _parentType");
        }

        /// <summary>
        /// Pour une expression A.b.c, passer en argument la <see cref="REClassObject"/> du type A,
        /// et "b.c".
        /// </summary>
        public REMemberExpression(REClassObject parent, string expression)
        {
            string[] _names = expression.Split('.');
            Names = new List<string>();
            foreach(string _name in _names)
                Names.Add(_name);
            
            Parent = parent??throw new Exception
                              (@"parent ne doit pas être null. 
                                 Si parent ne peut être aquis au moment de cet instanciation, 
                                 utiliser un constructeur réclamant string parentTypeName, ou Type parentType");
        }

        /// <summary>
        /// Pour une expression A.b.c, passer en argument la <see cref="REClassObject"/> du type A,
        /// et "b", "c".
        /// </summary>
        public REMemberExpression(REClassObject parent, params string[] expression)
        {
            if(expression == null)
                throw new ArgumentNullException("_expression");
            Names = new List<string>();
            foreach(string _name in expression)
                Names.Add(_name);
            
            Parent = parent??throw new Exception
                              (@"parent ne doit pas être null. 
                                 Si _parent ne peut être aquis au moment de cet instanciation, 
                                 utiliser un constructeur réclamant string parentTypeName, ou Type parentType");
        }

        public REMemberExpression(Type parentType, Expression<Func<object>> expression)
        {
            if(expression == null)
                throw new ArgumentNullException("expression");
            if(parentType == null)
                throw new ArgumentNullException("parentType");
            MembersNames(expression);
            ParentTypeName = parentType.Name;
        }

        public REMemberExpression(string parentTypeName, string expression)
        {
            ParentTypeName = parentTypeName;
            string[] _names = expression.Split('.');
            Names = new List<string>();
            foreach(string _name in _names)
                Names.Add(_name);
        }

        public REMemberExpression(string parentTypeName, params string[] expression)
        {
            if(expression == null)
                throw new ArgumentNullException("_expression");
            ParentTypeName = parentTypeName;
            Names = new List<string>();
            foreach(string _name in expression)
                Names.Add(_name);
        }

        public REMemberExpression(string parentTypeName, List<string> expression)
        {
            Names = expression??throw new ArgumentNullException("_expression");
            ParentTypeName = parentTypeName;
        }

        public override REBase Copy()
        {
            if(Parent != null)
                return new REMemberExpression(Parent, Names.ToArray());   
            return new REMemberExpression(ParentTypeName, Names.ToArray());
        }

        /// <summary>
        /// <see cref="Parent"/> est le <see cref="REClassObject"/> dont fait partie le membre
        /// exprimé par ce <see cref="REMemberExpression"/>.
        /// set retient le <see cref="REClassObject"/> et invoque <see cref="Build"/> 
        /// pour construire ce <see cref="REMemberExpression"/>.
        /// </summary>
        public override REClassObject Parent 
        { 
            get => base.Parent; 
            set
            {
                base.Parent = value; 
                Build();
            }
        }

        public override REBase ReValue 
        { 
            get
            {
                if(Parent == null || Expression == null)
                    return NULL;
                return Expression.ReValue;
            }

            set
            {
                if(ReValue is REValue _revalue && value is REExpression _expr)
                {
                    _revalue.CValue = _expr.CValue;
                }
                else
                    throw new NotImplementedException();
            }
        }

        public override object CValue 
        { 
            get
            {
                if(ReValue is REExpression _expr)
                    return _expr.CValue;
                throw new NotImplementedException(); 
            }
            set
            {
                if(ReValue is REExpression _expr)
                    _expr.CValue = value;
                else
                throw new NotImplementedException();  
            }
        }

        public override REExpression Invoke()
        {
            if(Expression != null)
                Expression.Invoke();
            return this;
        }

        /// <summary>
        /// Retourne l'expression représentée par une string ( ex. "a.b.c")
        /// </summary>
        public override string Display()
        {
            string _expression = "";
            if(Names != null)
            {
                foreach(string _name in Names)
                {
                    if(_expression != "")
                        _expression += ".";
                    _expression += _name;
                }
            }
            return _expression;
        }

        public REExpression Expression { get; set; } = null;

        public override REBase[] Children => new REBase[1]{ Expression };

        public string LastMemberName()
        {
            if(Names != null && Names.Count > 0)
                return Names[Children.Length - 1];
            return "";
        }

        private List<string> Names { get; set; } = null;

        #region Build

        private void Build()
        {
            if(Names == null)
                throw new Exception("Names null.");

            if(Parent != null && Names.Count != 0)
            {
                REMember _member = Parent.GetMember(Names[0]);

                if(Names.Count > 1) // nous sommes sur un objet de classe
                {
                    // copie de Names sauf le premier élément
                    String[] _names = new string[Names.Count - 1];
                    Names.CopyTo(1, _names, 0, _names.Length);

                    Expression = new REMemberExpression((REClassObject)(_member.ReValue), _names);
                }
                else
                {
                    Expression = _member;
                }
            }
        }

        private void MembersNames(Expression<Func<object>> _expression)
        {
            Names = new List<string>();

            string[] _elements = _expression.ToString().Split('.');

            for(int _i = _elements.Length - 1; _i >= 0; _i--)
            {
                if(_elements[_i].EndsWith(")"))
                    break;
                Names.Insert(0, _elements[_i]);
            }

            Names.RemoveAt(0);
        }

        #endregion Build
    }
}
