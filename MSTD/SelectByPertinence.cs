using System;
using System.Collections.Generic;
using MSTD.ShBase;
using RuntimeExec;

namespace MSTD
{
    /// <summary>
    /// <see cref="NONE"/> aucune correspondence,
    /// <see cref="PERTINENCE1"/> tout correspond,
    /// <see cref="PERTINENCE2"/> un ou plusieurs membres correspondent,
    /// <see cref="PERTINENCE3"/> tous les membres commencent par l'un des mots,
    /// <see cref="PERTINENCE4"/> un ou plusieurs membres commencent par l'un des mots,
    /// <see cref="PERTINENCE5"/> un ou plusieurs membres contiennent un des mots
    /// </summary>
    public enum PERTINENCE
    {
        /// <summary>
        /// Aucune correspondance
        /// </summary>
        NONE,

        /// <summary>
        /// tout correspond
        /// </summary>
        PERTINENCE1,

        /// <summary>
        /// un ou plusieurs membres correspondent, les autres commencent par l'un des mots
        /// </summary>
        PERTINENCE2,
        
        /// <summary>
        /// un ou plusieurs membres correspondent
        /// </summary>
        PERTINENCE3,
        
        /// <summary>
        /// tous les membres commencent par l'un des mots
        /// </summary>
        PERTINENCE4,
        
        /// <summary>
        /// un ou plusieurs membres commencent par l'un des mots,
        /// </summary>
        PERTINENCE5,
        
        /// <summary>
        /// un ou plusieurs membres contiennet un des mots
        /// </summary>
        PERTINENCE6
    }

    /// <summary>
    /// <see cref="SelectByPertinence"/> effectue une recherche dans une liste d'objets 
    /// d'après une string, argument de recherche et une liste de <see cref="REMemberExpression"/>
    /// qui désignent les propriétés sur lesquelles baser la recherche.
    /// Cette string peut contenir plusieurs mots.
    /// Le résultat de la recherche est une série de listes d'objets selectionnés dans la liste initiale fournie.
    /// Ces listes correspondent à des niveaux de pertinence décrits par l'énumeration <see cref="PERTINENCE"/>.
    /// </summary>
    public class SelectByPertinence
    {
        public bool CasseSensitive { get; set; }

        /// <summary>
        /// Peuple les listes d'objets de <see cref="Base"/> correspondant chacune à un degré de pertinance d'après les critères de recherche.
        /// _argument : chaine de caractères contenant des mots recherchés.
        /// _objects : liste des objets dans laquelle effectuer la recherche.
        /// _members : propriétés ou champs publiques des objets sur lesquels baser la recherche.
        /// </summary>
        public void Seach(string _argument, List<Base> _objects, List<REMemberExpression> _members)
        {
            Clear();
            List<string> _combinations = Combinations(_argument);

            foreach(Base _object in _objects)
            {
                switch (Pertinence(_combinations, _object, _members))
                {
                    case MSTD.PERTINENCE.NONE:
                        ListPertinence0.Add(_object);
                        break;
                    case MSTD.PERTINENCE.PERTINENCE1:
                        ListPertinence1.Add(_object);
                        break;
                    case MSTD.PERTINENCE.PERTINENCE2:
                        ListPertinence2.Add(_object);
                        break;
                    case MSTD.PERTINENCE.PERTINENCE3:
                        ListPertinence3.Add(_object);
                        break;
                    case MSTD.PERTINENCE.PERTINENCE4:
                        ListPertinence4.Add(_object);
                        break;
                    case MSTD.PERTINENCE.PERTINENCE5:
                        ListPertinence5.Add(_object);
                        break;
                    case MSTD.PERTINENCE.PERTINENCE6:
                        ListPertinence6.Add(_object);
                        break;
                    default:
                        break;
                }
            }
        }

        public PERTINENCE Pertinence(List<string> _words, Base _object, List<REMemberExpression> _members)
        {
            if(_object == null)
                throw new ArgumentNullException();
            if(_words == null || _words.Count == 0 || _members == null || _members.Count == 0)
                return MSTD.PERTINENCE.NONE;
            List<string> _values = new List<string>();

            if(CasseSensitive == false)
            {
                for(int _i = 0; _i < _words.Count; _i++)
                {
                    _words[_i] = _words[_i].ToLower();
                }
            }

            foreach(REMemberExpression _expr in _members)
            {
                object _value = _expr.Update(_object).Invoke().CValue;
                if(_value != null)
                {
                    if(CasseSensitive == false)
                        _values.Add(_value.ToString().ToLower());
                    else
                        _values.Add(_value.ToString());
                }
                    
                else
                    _values.Add("");
            }

            int _membersMatch = 0;
            int _membersStartsWhith = 0;
            int _membersContains = 0;

            foreach(string _value in _values)
            {
                foreach(string _word in _words)
                {
                    if(_value == _word)
                        ++_membersMatch;
                    else
                    if(_value.StartsWith(_word))
                        ++_membersStartsWhith;
                    else
                    if(_value.Contains(_word))
                        ++_membersContains;
                }
            }

            if(_membersMatch == _values.Count)
                return MSTD.PERTINENCE.PERTINENCE1;

            if(_membersMatch + _membersStartsWhith == _values.Count)
                return MSTD.PERTINENCE.PERTINENCE2;

            if(_membersMatch > 0)
                return MSTD.PERTINENCE.PERTINENCE3;

            if(_membersStartsWhith == _values.Count)
                return MSTD.PERTINENCE.PERTINENCE4;

            if(_membersStartsWhith > 0)
                return MSTD.PERTINENCE.PERTINENCE5;

            if(_membersContains > 0)
                return MSTD.PERTINENCE.PERTINENCE6;

            return MSTD.PERTINENCE.NONE;
        }

        /// <summary>
        /// a b c => a, a b, a b c, b, b c, c
        /// </summary>
        public List<string> Combinations(string _argument)
        {
            List<string> _combinations = new List<string>();

            if(!string.IsNullOrWhiteSpace(_argument))
            {
                string[] _elements = _argument.Split(' ');
                List<string> _correctElements = new List<string>();

                foreach(string _element in _elements)
                {
                    if(!string.IsNullOrWhiteSpace(_element))
                        _correctElements.Add(_element);
                }

                if(_correctElements.Count == 1)
                {
                    _combinations.Add(_correctElements[0]);
                }
                else
                {
                    for(int _i = 0; _i < _correctElements.Count; _i++)
                    {
                        string _str = _correctElements[_i];
                        
                        _combinations.Add(_str);

                        for(int _j = _i + 1; _j < _correctElements.Count; _j++)
                        {
                            _str += " " + _correctElements[_j];
                            _combinations.Add(_str);
                        }
                    }
                }
            }
            return _combinations;
        }

        private void Clear()
        {
            ListPertinence0.Clear();
            ListPertinence1.Clear();
            ListPertinence2.Clear();
            ListPertinence3.Clear();
            ListPertinence4.Clear();
            ListPertinence5.Clear();
            ListPertinence6.Clear();
        }

        public  List<Base> ListPertinence0 { get; private set; } = new List<Base>();                              
        public  List<Base> ListPertinence1 { get; private set; } = new List<Base>(); 
        public  List<Base> ListPertinence2 { get; private set; } = new List<Base>(); 
        public  List<Base> ListPertinence3 { get; private set; } = new List<Base>(); 
        public  List<Base> ListPertinence4 { get; private set; } = new List<Base>(); 
        public  List<Base> ListPertinence5 { get; private set; } = new List<Base>();
        public  List<Base> ListPertinence6 { get; private set; } = new List<Base>();
    }
}
