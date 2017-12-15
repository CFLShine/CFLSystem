
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MSTD
{
    /// <summary>
    /// <see cref="MemberPath"/> retient une expression de membre sous
    /// forme d'une liste de string, les noms de chaque membre de l'expression
    /// et d'une liste de PropertyInfo, les propriétés invoquées par l'expression.
    /// </summary>
    public class MemberPath
    {
        public MemberPath(){ }

        /// <summary>
        /// Expression (exemple): ()=>((MyClass)null).a.b
        /// </summary>
        public MemberPath(Expression<Func<object>> _expression)
        {
            extractMembersTypesAndNames(_expression);
        }

        public MemberPath(MemberPath other)
        {
            __rootType = other.RootType;
            foreach(PropertyInfo _prInfo in other.Properties)
                AddProperty(_prInfo);
        }

        public MemberPath(Type rootType, params string[] _names)
        {
            __rootType = rootType;
            foreach(string _name in _names)
                Names.Add(_name);
            extractMembersTypes();
        }

        public MemberPath(Type rootType, string _path)
        {
            __rootType = rootType;
            string[] _names = _path.Split('.');
            foreach(string _name in _names)
                Names.Add(_name);
            extractMembersTypes();
        }

        /// <summary>
        /// Ajoute une propriété à cette expression.
        /// Si cette propriété est en position 0,
        /// RootType devient le type déclarant cette propriété.
        /// </summary>
        public void AddProperty(PropertyInfo property)
        {
            Properties.Add(property);
            Names.Add(property.Name);
            if(Properties.Count == 1)
                __rootType = property.GetMethod.ReflectedType;
        }

        /// <summary>
        /// Retourne true si other est exactement la même expression que this.
        /// </summary>
        public bool IsSameAs(MemberPath other)
        {
            if(other == null)
                return false;
            if(other.RootType != RootType)
                return false;
            if(other.Names.Count != Names.Count)
                return false;
            for(int _i = 0; _i < Names.Count; _i++)
            {
                if(Names[_i] != other.Names[_i])
                    return false;
            }
            return true;
        }

        // retourne le nombre de membres en commun entre Path et other.
        public int CommonMembersWith(MemberPath other)
        {
            if(other == null || RootType != other.RootType)
                return 0;
            for(int _i = 0; _i < Names.Count; _i++)
            {
                if(_i >= Names.Count || Names[_i] != other.Names[_i])
                    return _i;
            }
            return Names.Count;
        }

        public Type RootType => __rootType;

        /// <summary>
        /// Retourne le nom de propriétés exprimées.
        /// Ne compte pas <see cref="RootType"/>;
        /// </summary>
        public int Count
        {
            get => Properties.Count;
        }

        /// <summary>
        /// Retourne un nouveau <see cref="MemberPath"/> extrait de celui-ci
        /// à partir du membre d'index first et jusqu'au membre d'index last.
        /// ex : expression b.c.d.e
        /// Extract(1, 2) retourne un <see cref="MemberPath"/> expression c.d
        /// Le RootType du nouveau MemberPath est le type déclarant sa propriété
        /// en position 0.
        /// </summary>
        public MemberPath Extract(int first, int last)
        {
            if(first < 0 || first  >= Properties.Count)
                throw new IndexOutOfRangeException("first == " + first.ToString() +  ",Properties compte " + Properties.Count.ToString() + " éléments.");
            if(last < first)
                throw new Exception("last doit être plu grand ou égal à first. first : " + first.ToString() + ", last : " + last.ToString() + ".");
            if(last > Properties.Count)
                throw new IndexOutOfRangeException("first == " + first.ToString() + ", Properties compte " + Properties.Count.ToString() + " éléments.");
            
            MemberPath _newPath = new MemberPath();

            for(int _i = first; _i <= last; _i++)
            {
                _newPath.AddProperty(Properties[_i]);
            }
            return _newPath;
        }

        /// <summary>
        /// Retourne un nouveau <see cref="MemberPath"/> copie de celui-ci avec les éléments
        /// décalés de levels items à gauche.
        /// Le type racine devient le type déclarant le membre de niveau 0 après le décalage.
        /// </summary>
        public MemberPath Shift(int levels = 1)
        {
            if(Properties.Count < levels)
                throw new Exception("levels trop grand: levels == " + levels.ToString() + ", nombre de membre == ");
            
            MemberPath _newPath = new MemberPath();

            if(Properties.Count > levels) // snon retourne un MemberPath vide et sans RooType.
            {
                for(int _i = levels; _i < Properties.Count; _i++)
                    _newPath.AddProperty(Properties[_i]);
            }

            return _newPath;
        }

        /// <summary>
        /// _expression (exemple) : ()=>((MyClass)null).a.b
        /// </summary>
        public void SetExpression(Expression<Func<object>> _expression)
        {
            extractMembersTypesAndNames(_expression);
        }

        public List<string> Names { get; private set; } = new List<string>();

        /// <summary>
        /// Retourne une chaine représentant l'expression du membre, incluant RootType si includeRootType == true.
        /// ex RootType true, retourne "MyClass.Member1.Member2.Member3"
        /// </summary>
        public string ExpressionString(bool includeRootType = false)
        {
            string _path = "";
            if(Names != null)
            {
                foreach(string _name in Names)
                {
                    if(_path != "")
                        _path += ".";
                    _path += _name;
                }
            }
            return _path;
        }

        /// <summary>
        /// Retourne une chaine représentant l'expression du membre indiqué par level, en incluant RootType
        /// si includeRootType == true.
        /// Le niveau 0 est le premier membre après RootType.
        /// ex : Myclass.a.b.c, level 1, RootType true retorourne MyClass.a.b
        /// </summary>
        public string ExpressionString(int level, bool includeRootType = false)
        {
            if(level >= Count)
                throw new IndexOutOfRangeException("level est plus grand que Count-1");

            string _path = "";

            if(includeRootType)
                _path = RootType.Name;

            if(level < 0)
                return _path;

            for(int _i = 0; _i < level + 1; _i++)
            {
                if(_path != "")
                    _path = string.Concat(_path, ".", Names[_i]);
                else
                    _path = Names[_i];
            }
            return _path;
        }

        /// <summary>
        /// Retourne les PropertyInfo des propriétés de l'expression.
        /// </summary>
        public List<PropertyInfo> Properties { get; private set; } = new List<PropertyInfo>();

        /// <summary>
        /// Retourne le nom de la dernière propriété de l'expression.
        /// Ex : expression a.b.c, retourne "c".
        /// </summary>
        public string LastPropertyName
        {
            get
            {
                if(Names.Count == 0)
                    return "";
                return Names[Names.Count - 1];
            }
        }

        /// <summary>
        /// Retourne la PropertyInfo de la dernière propriété de l'expression.
        /// Ex : expression a.b.c, retourne la PropertyInfo de c.
        /// </summary>
        public PropertyInfo LastPropertyInfo
        {
            get
            {
                if(Properties.Count == 0)
                    return null;
                return Properties[Properties.Count - 1];
            }
        }

        private void extractMembersTypesAndNames(Expression<Func<object>> _expression)
        {
            Names.Clear();
            Properties.Clear();
            if(_expression.Body is UnaryExpression _unary)
                extractMembersTypeAndNames((MemberExpression)_unary.Operand);
            else
                extractMembersTypeAndNames((MemberExpression)_expression.Body);
        }

        private void extractMembersTypeAndNames(MemberExpression _memberExpression)
        {
            if(_memberExpression.Member is PropertyInfo _prInfo)
            {
                Properties.Insert(0, _prInfo);
                Names.Insert(0, _memberExpression.Member.Name);

                // appele recursif
                if(_memberExpression.Expression != null && _memberExpression.Expression is MemberExpression _memberExpr)
                    extractMembersTypeAndNames(_memberExpr);
            }
            else
                __rootType = _memberExpression.Type;
        }

        // utile depuis le constructeur REMemberPath(Type _classType, string _path)
        // Names déja construit.
        private void extractMembersTypes()
        {
            Properties.Clear();
            Type _current = RootType;

            foreach(string _name in Names)
            {
                PropertyInfo _prInfo = TypeHelper.Property(_current, _name);
                if(_prInfo == null)
                {
                    throw new Exception("La propriété " + _name + " du chemin " + ExpressionString() + " est introuvable dans le type " + _current.Name);
                }
                _current = _prInfo.PropertyType;    
                Properties.Add(_prInfo);
            }
        }

        private Type __rootType = null;
    }
}
