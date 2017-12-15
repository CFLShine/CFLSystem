using System;
using System.Collections.Generic;
using System.Reflection;
using MSTD;
using MSTD.ShBase;

namespace CFL_1.CFL_System.MSTD.ShBase
{
    public enum RelationKind
    {
        ISMEMBEROF,
        ISINLIST,
        HASMEMBER,
        HASINLIST
    }

    public struct BaseRelation
    {
        public BaseRelation(Type type, RelationKind kind, Type relationWith)
        {
            Type = type;
            Kind = kind;
            RelationWith = relationWith;
        }

        /// <summary>
        /// Le type de la class qui déclare cette relation.
        /// </summary>
        public Type Type { get; set; }

        public RelationKind Kind { get; set; }

        public Type RelationWith { get; set; }

    }

    /// <summary>
    /// <see cref="BaseRelations"/> définit les relations entre un type
    /// dérivées de <see cref="Base"/> et d'autres types dérivés de <see cref="Base"/>.
    /// </summary>
    public class BaseRelations
    {
        /// <summary>
        /// Le type de la classe qui déclare ces relations.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// type est le type de la classe qui déclare les relations
        /// avec les autres types connus par le <see cref="ShContext"/> context.
        /// </summary>
        public BaseRelations(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Trouve ou ajoute une liste de <see cref="BaseRelation"/> pour le type
        /// <see cref="BaseRelation.RelationWith"/> et lui ajoute la relation.
        /// </summary>
        public void AddRelation(BaseRelation relation)
        {
            List<BaseRelation> _relations = null;
            if(!__relations.TryGetValue(relation.RelationWith, out _relations))
            {
                _relations = new List<BaseRelation>();
                __relations[relation.RelationWith] = _relations;
            }
            _relations.Add(relation);
        }

        private Dictionary<Type, List<BaseRelation>> __relations = new Dictionary<Type, List<BaseRelation>>();
    }

    /// <summary>
    /// <see cref="ContextRelations"/> identifie toutes les relations
    /// entre tous les types connus d'un <see cref="ShContext"/>.
    /// </summary>
    public class ContextRelations
    {
        /// <summary>
        /// Se construit en identifiant toutes les relations entre tous les types
        /// représentés dans le <see cref="ShContext"/> context.
        /// </summary>
        public ContextRelations(ShContext context)
        {
            Init(context);
        }

        /// <summary>
        /// Trouve ou ajoute la <see cref="BaseRelations"/> pour le Type type
        /// et lui ajoute la <see cref="BaseRelation"/> relation.
        /// </summary>
        public void AddRelation(Type type, BaseRelation relation)
        {
            BaseRelations _relations = null;
            if(!__relations.TryGetValue(type, out _relations))
            {
                _relations = new BaseRelations(type);
                __relations[type] = _relations;
            }
            _relations.AddRelation(relation);
        }

        private void Init(ShContext context)
        {
            int _i = 1;
            foreach(Set _set in context.GetSets())
            {

                if(_i < context.SetsCount)
                {
                    foreach(Set _other in context.GetSets(_i++))
                    {
                        FindRelationsFromT1ToT2(_set.Type, _other.Type);
                        FindRelationsFromT1ToT2(_other.Type, _set.Type);
                    }
                }
            }
        }

        private void FindRelationsFromT1ToT2(Type t1, Type t2)
        {
            foreach(PropertyInfo _prInfo in t1.GetProperties())
            {
                if(PropertyHelper.IsMappableProperty(_prInfo))
                {
                    if(_prInfo.PropertyType == t2)
                    {
                        AddRelation(t1, new BaseRelation(t1, RelationKind.HASMEMBER, t2));
                        AddRelation(t2, new BaseRelation(t2, RelationKind.ISMEMBEROF, t1));
                    }
                    else
                    {
                        if(TypeHelper.IsListOf(_prInfo.PropertyType, t2))
                        {
                            AddRelation(t1, new BaseRelation(t1, RelationKind.HASINLIST, t2));
                            AddRelation(t2, new BaseRelation(t2, RelationKind.ISINLIST, t1));
                        }
                    }
                }
            }
        }

        private Dictionary<Type, BaseRelations> __relations = new Dictionary<Type, BaseRelations>();
    }
    
}
