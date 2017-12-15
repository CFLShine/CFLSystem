
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq.Expressions;
using RuntimeExec;
using MSTD.ShBase;

namespace CFL_1.CFL_System
{
    public abstract class ControlData
    {
        public void connect(REMemberExpression _memberExpression)
        {
            __memberExpression = _memberExpression;
            __primaryValue = _memberExpression.CValue;
            initControlValue();
        }

        public delegate void MemberChangedByUser(REMemberExpression _memberExpression, object _component);
        public MemberChangedByUser OnMemberChangedByUser;

        //protected:
        protected REMemberExpression __memberExpression;
        protected object __primaryValue;

        //protected:
        /// <summary>
        /// Donne au control la valeur initiale __primaryValue
        /// </summary>
        protected abstract void initControlValue();
    }

    public class TextBoxData : ControlData
    {
        public TextBoxData(TextBox _textBox)
        {
            __ctrl = _textBox;
            __ctrl.KeyUp += keyUp;
        }

        protected override void initControlValue()
        {
            if(__ctrl != null)
            {
                if(__primaryValue == null)
                    __ctrl.Text = "" ;
                else
                    __ctrl.Text = __primaryValue.ToString();
            }
        }

        private void keyUp(object sender, RoutedEventArgs e)
        { 
            e.Handled = false;
            if(__memberExpression.Parent == null)
                __ctrl.Text = "" ;
            else
            {
                __memberExpression.CValue = __ctrl.Text ;

                if(OnMemberChangedByUser != null)
                    OnMemberChangedByUser.Invoke(__memberExpression, ((REClassObject)(__memberExpression.Parent)).CValue);
            }
        }

        //private:
        private TextBox __ctrl;
    }

    public class DatePickerData : ControlData
    {
        public DatePickerData(DatePicker _ctrl)
        {
            __ctrl = _ctrl;
            __ctrl.KeyUp += keyUp;
        }

        protected override void initControlValue()
        {
            if(__ctrl != null)
            {
                if(__primaryValue == null)
                    __ctrl.Text = "" ;
                else
                    __ctrl.Text = __primaryValue.ToString();
            }
        }

        private void keyUp(object sender, RoutedEventArgs e)
        {
            e.Handled = false;
            if(__memberExpression.Parent == null)
                __ctrl.Text = "" ;
            else
            {
                __memberExpression.CValue = __ctrl.Text ;

                if(OnMemberChangedByUser != null)
                    OnMemberChangedByUser.Invoke(__memberExpression, __memberExpression.Parent.CValue);
            }
        }

        private DatePicker __ctrl;
    }

    public class CheckBoxData : ControlData
    {
        public CheckBoxData(CheckBox __checkBox)
        {
            __ctrl = __checkBox;
            __ctrl.Click += mouseup; 
        }

        protected override void initControlValue()
        {
            if (__ctrl != null)
            {
                if(__primaryValue == null)
                    __ctrl.IsChecked = false;
                else
                {
                    __ctrl.IsChecked = ((bool)__primaryValue == true);
                }
            }
        }

        //private:

        private CheckBox __ctrl;

        private void mouseup(object sender, RoutedEventArgs e)
        {
            e.Handled = false;

            if(__memberExpression.Parent == null)
            {
                __ctrl.IsChecked = false ;
            }
            else
            {
                //__ctrl.IsChecked retourne un bool? qui ne peut pas être casté en bool, d'où ce if.
                if(__ctrl.IsChecked == true)
                    __memberExpression.CValue = true;
                else
                    __memberExpression.CValue = false;

                if(OnMemberChangedByUser != null)
                    OnMemberChangedByUser.Invoke(__memberExpression, (__memberExpression.Parent.CValue));
            }
        }
    }

    public class ComboBoxData : ControlData
    {
        public ComboBoxData(ComboBox _comboBox)
        {
            __ctrl = _comboBox;
            __ctrl.KeyUp += keyUp;
        }

        //protected:
        protected override void initControlValue()
        {
            if(__primaryValue == null)
            {
                __ctrl.SelectedIndex = 0 ;
            }
            else
            if (__ctrl != null)
            {
                __ctrl.SelectedValue = (string)__primaryValue;
            }
        }

        //private:
        private ComboBox __ctrl ;

        private void keyUp(object sender, RoutedEventArgs e)
        {
            e.Handled = false ;
            if(__memberExpression.Parent == null)
                __ctrl.SelectedIndex = 0 ;
            else
            {
                __memberExpression.CValue = __ctrl.SelectedValue ;
            }
        }
    }

    public class InterControlsData
    {
        public InterControlsData() { }

        public delegate void MemberChangedByUser(REMemberExpression _memberExpression, object _object);
        public MemberChangedByUser OnMemberChangedByUser;
        private void onMemberChangedByUser(REMemberExpression _memberExpression, object _object)
        {
            if(OnMemberChangedByUser != null)
                OnMemberChangedByUser.Invoke(_memberExpression, _object);
        }

        public void connect(TextBox _ctrl, Base _component, string _propertyName) 
        {
            ControlData _ctrlData = controlData(_ctrl);
            _ctrlData.connect(new REMemberExpression(new REClassObject(_component), _propertyName));
            _ctrlData.OnMemberChangedByUser += onMemberChangedByUser;
        }

        public void connect(DatePicker _ctrl, Base _component, string _propertyName)
        {
            ControlData _ctrlData = controlData(_ctrl);
            _ctrlData.connect(new REMemberExpression(new REClassObject(_component), _propertyName));
            _ctrlData.OnMemberChangedByUser += onMemberChangedByUser;
        }

        public void connect(CheckBox _ctrl, Base _component, string _propertyName)
        {
            ControlData _ctrlData = controlData(_ctrl);
            _ctrlData.connect(new REMemberExpression(new REClassObject(_component), _propertyName));
            _ctrlData.OnMemberChangedByUser += onMemberChangedByUser;
        }

        public void connect(ComboBox _ctrl, Base _component, string _propertyName)
        {
            ControlData _ctrlData = controlData(_ctrl);
            _ctrlData.connect(new REMemberExpression(new REClassObject(_component), _propertyName));
            _ctrlData.OnMemberChangedByUser += onMemberChangedByUser;
        }

        public void connect(TextBox _ctrl, Base _component, Expression<Func<object>> _property)
        {
            ControlData _ctrlData = controlData(_ctrl);
            _ctrlData.connect(new REMemberExpression(new REClassObject(_component), _property));
            _ctrlData.OnMemberChangedByUser += onMemberChangedByUser;
        }

        public void connect(DatePicker _ctrl, Base _component, Expression<Func<object>> _property)
        {
            ControlData _ctrlData = controlData(_ctrl);
            _ctrlData.connect(new REMemberExpression(new REClassObject(_component), _property));
            _ctrlData.OnMemberChangedByUser += onMemberChangedByUser;
        }

        public void connect(CheckBox _ctrl, Base _component, Expression<Func<object>> _property) 
        {
            ControlData _ctrlData = controlData(_ctrl);
            _ctrlData.connect(new REMemberExpression(new REClassObject(_component), _property));
            _ctrlData.OnMemberChangedByUser += onMemberChangedByUser;
        }

        public void connect(ComboBox _ctrl, Base _component, Expression<Func<object>> _property)
        {
            ControlData _ctrlData = controlData(_ctrl);
            _ctrlData.connect(new REMemberExpression(new REClassObject(_component), _property));
            _ctrlData.OnMemberChangedByUser += onMemberChangedByUser;
        }

        public Dictionary<Control, ControlData>.ValueCollection ControlsData()
        {
            return __controlDataMap.Values;
        }

        //private:

        ControlData controlData(TextBox _control)
        {
            if (__controlDataMap.ContainsKey(_control))
                return __controlDataMap[_control];
            ControlData _controlData = newControlData(_control);
            __controlDataMap[_control] = _controlData;
            return _controlData;
        }

        ControlData controlData(DatePicker _control)
        {
            if (__controlDataMap.ContainsKey(_control))
                return __controlDataMap[_control];
            ControlData _controlData = newControlData(_control);
            __controlDataMap[_control] = _controlData;
            return _controlData;
        }
        
        ControlData controlData(CheckBox _control) 
        {
            if (__controlDataMap.ContainsKey(_control))
                return __controlDataMap[_control];
            ControlData _controlData = newControlData(_control);
            __controlDataMap[_control] = _controlData;
            return _controlData;
        }

        ControlData controlData(ComboBox _control)
        {
            if (__controlDataMap.ContainsKey(_control))
                return __controlDataMap[_control];
            ControlData _controlData = newControlData(_control);
            __controlDataMap[_control] = _controlData;
            return _controlData;
        }

        Dictionary<Control, ControlData> __controlDataMap = new Dictionary<Control, ControlData>();

        ControlData newControlData(TextBox _textBox)
        {
            return new TextBoxData(_textBox);
        }

        ControlData newControlData(DatePicker _datePicker)
        {
            return new DatePickerData(_datePicker);
        }

        ControlData newControlData(CheckBox _checkBox)
        {
            return new CheckBoxData(_checkBox);
        }

        ControlData newControlData(ComboBox _comboBox)
        {
            return new ComboBoxData(_comboBox);
        }
    }
}
