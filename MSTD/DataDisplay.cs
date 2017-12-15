using System;
using System.Collections.Generic;
using System.Linq;
using RuntimeExec;

namespace MSTD
{
    public enum DecimalSeparator
    {
        POINT,
        COMA
    }

    /// <summary>
    /// <see cref="DataDisplay"/> est un constructeur de chaîne <see cref="string"/>.
    /// La chaîne retournée par <see cref="ReValue"/> est constituée de toutes les éléments de <see cref="Elements"/>
    /// en tenant compte des formats pourvus.
    /// 
    /// Exemple d'utilisation :
    /// <code>
    /// int i = 2;
    /// <see cref="DataDisplay"/> _d = new <see cref="DataDisplay"/>(new REValue("Il y a "),new REAdd(3, 5), new REValue(" objects dans ce tableau."), 
    ///                                                              new REValue(" C'est plus que "), new REValue(i), new REValue("..."));
    /// string _s = d.Value; // _s vaut "Il y a 8 objets dans ce tableau. Cest plus que 2..."
    /// </code>
    /// 
    /// Un élément est un <see cref="RuntimeExec.REExpression"/> et ce sera le résultat
    /// de <see cref="RuntimeExec.REExpression.ReValue"/> qui sera affiché,
    /// par exemple, un <see cref="RuntimeExec.REIfStatement"/> qui contient un <see cref="DataDisplay"/>
    /// comme <see cref="RuntimeExec.REIfStatement.ThenStatement"/> et un autre <see cref="DataDisplay"/>
    /// comme <see cref="RuntimeExec.REIfStatement.ElseStatement"/> si sa condition n'est pas satisfaite.
    /// Un élément peur aussi être un autre <see cref="DataDisplay"/>.
    /// </summary>
    public class DataDisplay : REExpression
    {
        public DataDisplay(){}

        public DataDisplay(params REExpression[] _elements)
        {
            if(_elements != null)
            {
                Elements = _elements.ToList();
            }
        }

        public override REBase Copy()
        {
            DataDisplay _dataDisplay = new DataDisplay();
            foreach(REExpression _expr in Elements)
                _dataDisplay.Elements.Add((REExpression)_expr.Copy());
            return _dataDisplay;
        }

        public List<REExpression> Elements { get; set; }

        /// <summary>
        /// Cette fonction va rechercher dans chaque élément si un <see cref="REClassObject"/>
        /// du même type que _object est utilisé par l'expression et le remplacer par _object.
        /// </summary>
        /// <param name="_object"></param>
        public override REExpression Update(REClassObject _object)
        {
            foreach(REExpression _expr in Elements)
            {
                if(_expr != null)
                {
                    _expr.Update(_object);

                    List<REExpression> _children = new List<REExpression>();
                    _expr.ChildrenAll(_children);
                    foreach(REExpression _child in _children)
                    {
                        if(_child != null)
                            _child.Update(_object);
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Retourne un tableau des <see cref="Elements"/>.
        /// </summary>
        public override REBase[] Children => Elements.ToArray();

        public string DatesFormat { get; set; } = "dd/MM/yyyy";
        public string TimesFormat { get; set; } = "hh:mm";

        /// <summary>
        /// Format donné pour les types entiers.
        /// ex : value = 1234, value.ToString("D8") affiche 00001234
        /// </summary>
        public string IntegralsFormat { get; set; } = "D";

        /// <summary>
        /// Format donné pour les types de nombre à virgule.
        /// ex : value = 12.34, value.ToString("F4") affiche 12.3400
        /// </summary>
        public string FloatsFormat{ get; set; } = "F2";

        /// <summary>
        /// Indique que le séparateur décimal pour les type double et float
        /// sera un point <see cref="DecimalSeparator.POINT"/> 
        /// ou une virgule <see cref="DecimalSeparator.COMA"/>.
        /// </summary>
        public DecimalSeparator DecimalSeparator { get; set; } = DecimalSeparator.POINT;

        public override REExpression Invoke()
        {
            string _str = "";
            if(Elements != null)
            {
                foreach(REExpression _expr in Elements)
                    _str += ValueString(_expr);
            }
            __revalue = new REValue(_str);
            return this;
        }

        /// <summary>
        /// Appelle <see cref="Invoke"/> puis
        /// retourne (string)<see cref="CValue"/> une string, concaténation des éléments tenant compte des formats pourvus.
        /// </summary>
        public override string Display()
        {
            Invoke();
            return (string)CValue;
        }

        public override REBase ReValue 
        { 
            get => __revalue; 
            set => throw new NotImplementedException(); 
        }

        private REBase __revalue;

        public override object CValue 
        { 
            get
            {
                if(ReValue is REExpression _expr)
                {
                    return _expr.CValue;
                }
                return null;
            }
            set => throw new NotImplementedException(); 
        }

        private string ValueString(REExpression _expr)
        {
            if(_expr == null)
                return "";

            _expr.Invoke();
            object _value = _expr.CValue;
            
            if(_value is DateTime _dateTime)
                return(_dateTime.ToString(DatesFormat));
            else
            if(_value is TimeSpan _time)
                return(_time.ToString(TimesFormat));
            else
            if(_value is int || _value is long)
                return(((long)_value).ToString(IntegralsFormat));
            else
            if(_value is uint || _value is ulong)
                return(((ulong)_value).ToString(IntegralsFormat));
            else
            if(_value is float _float)
                return(_float.ToString(FloatsFormat));
            else
            if(_value is double _double)
                return(_double.ToString(FloatsFormat));
            else
            return(_value.ToString());
        }
    }
}
