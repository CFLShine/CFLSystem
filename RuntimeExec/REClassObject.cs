using System;
using System.Collections.Generic;
using System.Reflection;
using MSTD.ShBase;

namespace RuntimeExec
{
    /// <summary>
    /// <see cref="REClassObject"/> est soit un objet de classe
    /// construit sur le model de <see cref="REClass"/>,
    /// soit sur le model d'un objet CSharp.
    /// La propriété <see cref="CValue"/> prend ou expose l'objet CSharp.
    /// </summary>
    public class REClassObject : REExpression
    {
        public REClassObject(){ }

        /// <summary>
        /// Génère un <see cref="REClassObject"/> qui encapsule
        /// les membres d'un objet CSharp, le contient et 
        /// l'expose par la propriété <see cref="CValue"/>.
        /// </summary>
        public REClassObject(object _objectCSharp) => CValue = _objectCSharp; // Appèle BuildFromCSharp()

        /// <summary>
        /// Assigne _typeName à <see cref="TypeName"/>.
        /// </summary>
        public REClassObject(string parentTypeName)
        {
            ObjectTypeName = parentTypeName;
        }

        /// <summary>
        /// Assigne type.Name à <see cref="TypeName"/>.
        /// </summary>
        public REClassObject(Type type)
        {
            ObjectTypeName = type.Name;
        }

        public string ObjectTypeName { get; set; }

        public override REBase Copy()
        {
            return new REClassObject(ObjectTypeName);
        }

        /// <summary>
        /// Ajoute la propriété à cet objet, lui donne cet objet comme <see cref="REMember.Parent"/> et <see cref="REBase.TreeAncestor"/>
        /// Provoque une exception si _pr est null.
        /// Provoque une exeption la propriété n'est pas nommée ou si cet objet contient déja un membre avec le même nom.
        /// </summary>
        /// <param name="_pr"></param>
        public void AddProperty(REProperty _pr)
        {
            if(string.IsNullOrWhiteSpace(_pr.MemberName))
                throw new Exception("Un membre doit être nommé avant d'être ajouté à un objet.");
            REBase _member = GetMember(_pr.MemberName);
            if(_member != null)
                throw new Exception("Un membre " + _member.GetType().Name + " nomé " + _pr.MemberName + " existe déja dans cet objet.");

            _pr.Parent = this;
            _pr.TreeAncestor = this;

            Properties.Add(_pr);
        }

        /// <summary>
        /// Ajoute le champs à cet objet, lui donne cet objet comme <see cref="REMember.Parent"/> et <see cref="REBase.TreeAncestor"/>.
        /// Provoque une exception si _fld est null.
        /// Provoque une exeption le champs n'est pas nommé ou si cet objet contient déja un membre avec le même nom.
        /// </summary>
        /// <param name="_pr"></param>
        public void AddField(REField _fld)
        {
            if(string.IsNullOrWhiteSpace(_fld.MemberName))
                throw new Exception("Un membre doit être nommé avant d'être ajouté à un objet.");
            REBase _member = GetMember(_fld.MemberName);
            if(_member != null)
                throw new Exception("Un membre " + _member.GetType().Name + " nomé " + _fld.MemberName + " existe déja dans cet objet.");

            _fld.Parent = this;
            _fld.TreeAncestor = this;

            Fields.Add(_fld);
        }

        public List<REProperty> Properties { get; set; } = new List<REProperty>();
        public List<REField> Fields { get; set; } = new List<REField>();

        public REMember GetMember(string _name)
        {
            foreach(REProperty _pr in Properties)
            {
                if(_pr.MemberName == _name)
                    return _pr;
            }

            foreach(REField _fld in Fields)
            {
                if(_fld.MemberName == _name)
                    return _fld;
            }
            
            return null;
        }

        /// <summary>
        /// get :
        /// retourne this.
        /// set :
        /// Provoque une erreur.
        /// </summary>
        public override REBase ReValue 
        { 
            get
            {
                return this;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// get : si ce <see cref="REClassObject"/> a été construit depuis un objet CSharp,
        /// retourne cet objet, sinon retourne null.
        /// set : Retient un objet CSharp et se construit d'après cet objet.
        /// </summary>
        public override object CValue
        {
            get
            {
                if(__value is REClass)
                    return null;
                return __value;
            }
            set
            {
                __value = value;
                BuildFromCSharp();
            }
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        public override REExpression Invoke()
        {
            //do nothing
            return this;
        }

        public override REBase[] Children
        {
            get
            {
                // TODO ajouter les méthodes
                REBase[] _children = new REBase[Properties.Count + Fields.Count];
                int _i = 0;
                foreach(REProperty _pr in Properties)
                {
                    _children[_i] = _pr;
                    ++_i;
                }
                foreach(REField _fld in Fields)
                {
                    _children[_i] = _fld;
                    ++_i;
                }
                return _children;
            }
        }

        public override REExpression Update(REClassObject _object)
        {
            if(_object != null && _object.GetType().Name == ObjectTypeName)
                CValue = _object;
            return this;
        }

        private void BuildFromCSharp()
        {
            if(__value == null)
                return;
            
            Type _typeCSharp = __value.GetType();
            ObjectTypeName = _typeCSharp.Name;

            foreach(PropertyInfo _prInfo in _typeCSharp.GetProperties())
            {
                if(_prInfo.PropertyType.IsPublic)
                {
                    if(_prInfo.PropertyType.IsSubclassOf(typeof(Base)))
                    {
                        object _object = _prInfo.GetValue(CValue);
                        if(_object != null)
                            AddProperty(new REProperty(new REClassObject(_object), _prInfo.Name));
                        else
                            AddProperty(new REProperty(new REClassObject(_prInfo.PropertyType.Name), _prInfo.Name));
                    }
                    else
                    AddProperty(new REProperty(new RERefCSharp(this, _prInfo.Name), _prInfo.Name));
                }
            }

            foreach(FieldInfo _fldInfo in _typeCSharp.GetFields())
            {
                if(_fldInfo.IsPublic)
                {
                    if(_fldInfo.FieldType.IsSubclassOf(typeof(Base)))
                    {
                        object _object = _fldInfo.GetValue(CValue);
                        if(_object != null)
                            AddField(new REField(new REClassObject(_object), _fldInfo.Name));
                        else
                            AddField(new REField(new REClassObject(_fldInfo.FieldType.Name), _fldInfo.Name));
                    }
                    else
                    AddField(new REField(new RERefCSharp(this, _fldInfo.Name), _fldInfo.Name));
                }
            }
        }

        private object __value = null;
    }
}
