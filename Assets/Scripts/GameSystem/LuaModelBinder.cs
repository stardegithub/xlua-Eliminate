using System;
using System.ComponentModel;
using Foundation.Databinding;
using UnityEngine;
using XLua;

namespace GameSystem
{
    public class LuaModelBinder : IObservableModel, IDisposable
    {
        ObservableMessage _bindingMessage = new ObservableMessage();
        protected LuaTable _luaTable;
        object _instance;
        MonoBehaviour _insanceBehaviour;
        IObservableModel _bindableInstance;
        INotifyPropertyChanged _notifyInstance;


        public LuaModelBinder(object instance, LuaTable luaTable)
        {
            _luaTable = luaTable;
            _instance = instance;
            _insanceBehaviour = instance as MonoBehaviour;
            _bindableInstance = instance as IObservableModel;
            _notifyInstance = instance as INotifyPropertyChanged;

            if (_bindableInstance != null)
                _bindableInstance.OnBindingUpdate += _bindableInstance_OnBindingUpdate;
            else if (_notifyInstance != null)
                _notifyInstance.PropertyChanged += _notifyInstance_PropertyChanged;
        }

        void _notifyInstance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_onBindingEvent != null)
            {
                _bindingMessage.Name = e.PropertyName;
                _bindingMessage.Value = GetValue(e.PropertyName);
                _onBindingEvent(_bindingMessage);
            }
        }

        void _bindableInstance_OnBindingUpdate(ObservableMessage obj)
        {
            if (_onBindingEvent != null)
            {
                _onBindingEvent(obj);
            }
        }

        /// <summary>
        /// Raises property changed on all listeners
        /// </summary>
        /// <param name="propertyName">property to change</param>
        /// <param name="propValue">value to pass</param>
        public virtual void NotifyProperty(string propertyName, object propValue)
        {
            RaiseBindingUpdate(propertyName, propValue);
        }

        #region interface
        private Action<ObservableMessage> _onBindingEvent = delegate { };
        public event Action<ObservableMessage> OnBindingUpdate
        {
            add
            {
                _onBindingEvent = (Action<ObservableMessage>)Delegate.Combine(_onBindingEvent, value);
            }
            remove
            {
                _onBindingEvent = (Action<ObservableMessage>)Delegate.Remove(_onBindingEvent, value);
            }
        }

        public void RaiseBindingUpdate(string memberName, object paramater)
        {
            if (_onBindingEvent != null)
            {
                _bindingMessage.Name = memberName;
                _bindingMessage.Sender = this;
                _bindingMessage.Value = paramater;

                _onBindingEvent(_bindingMessage);
            }
        }

        [HideInInspector]
        public object GetValue(string memberName)
        {
            object member;
            if (!_luaTable.TryGet(memberName, out member))
            {
                Debug.LogError("Member not found ! " + memberName + " " + _luaTable);
                return null;
            }

            return member;
        }

        public object GetValue(string memberName, object paramater)
        {
            object member;
            if (!_luaTable.TryGet(memberName, out member))
            {
                Debug.LogError("Member not found ! " + memberName + " " + _luaTable);
                return null;
            }

            if (member is LuaFunction)
            {
                var meth = (member as LuaFunction);

                if (paramater != null)
                {

                    meth.Call(paramater);
                }

                return meth.Call();
            }

            return member;
        }

        [HideInInspector]
        public void Command(string memberName)
        {
            Command(memberName, null);
        }

        public void Command(string memberName, object paramater)
        {
            object member;
            if (!_luaTable.TryGet(memberName, out member))
            {
                Debug.LogError("Member not found ! " + memberName + " " + _luaTable);
                return;
            }

            if (member is LuaFunction)
            {
                var method = member as LuaFunction;
                method.Call(paramater);
            }
            else
            {
                _luaTable.Set(memberName, paramater);
            }
        }

        [HideInInspector]
        public void Dispose()
        {
            _bindingMessage.Dispose();

            if (_bindableInstance != null)
            {
                _bindableInstance.OnBindingUpdate -= _bindableInstance_OnBindingUpdate;
            }
            if (_notifyInstance != null)
            {
                _notifyInstance.PropertyChanged -= _notifyInstance_PropertyChanged;
            }
            _luaTable.Dispose();
            _luaTable = null;
            _instance = null;
            _insanceBehaviour = null;
            _bindableInstance = null;
            _bindingMessage = null;
            _notifyInstance = null;
        }
        #endregion
    }
}
