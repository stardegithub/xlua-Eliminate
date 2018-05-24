using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using Facade.Core;

namespace Facade
{
    public class FacadeScriptObserver : ObserverBehaviour
    {
        [SerializeField]
        protected string scriptName;
        protected object scriptInstance;
        protected Dictionary<string, MethodInfo> messageMethodMap = new Dictionary<string, MethodInfo>();
        protected MethodInfo methodAwake, methodStart, methodOnEnable, methodOnDisable, methodOnDestroy, methodUpdate;

        void Awake()
        {

            ReflectMethod();
            ObserverMessages = new List<string>(messageMethodMap.Keys);
            Facade.Instance.RemoveObserver(this, ObserverMessages);
            Facade.Instance.RegisterObserver(this, ObserverMessages);

            if (methodAwake != null)
            {
                methodAwake.Invoke(scriptInstance, null);
            }
        }

        void Start()
        {
            if (methodStart != null)
            {
                methodStart.Invoke(scriptInstance, null);
            }
        }

        void OnEnable()
        {
            if (methodOnEnable != null)
            {
                methodOnEnable.Invoke(scriptInstance, null);
            }
        }

        void OnDisable()
        {
            if (methodOnDisable != null)
            {
                methodOnDisable.Invoke(scriptInstance, null);
            }
        }

        void OnDestroy()
        {
            if (methodOnDestroy != null)
            {
                methodOnDestroy.Invoke(scriptInstance, null);
            }

            Facade.Instance.RemoveObserver(this, ObserverMessages);
            scriptInstance = null;
            messageMethodMap = null;

            methodAwake = null;
            methodStart = null;
            methodOnEnable = null;
            methodOnDisable = null;
            methodOnDestroy = null;
            methodUpdate = null;
        }

        void Update()
        {
            if (methodUpdate != null)
            {
                methodUpdate.Invoke(scriptInstance, null);
            }
        }

        protected void ReflectMethod()
        {
            Type type = Type.GetType(scriptName);
            if (type == null)
            {
                return;
            }

            scriptInstance = Activator.CreateInstance(type);

            MethodInfo[] mis = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            foreach (var mi in mis)
            {
                switch (mi.Name)
                {
                    case "Init":
                        {
                            mi.Invoke(scriptInstance, new object[] { this });
                            break;
                        }
                    case "Awake":
                        {
                            methodAwake = mi;
                            break;
                        }
                    case "Start":
                        {
                            methodStart = mi;
                            break;
                        }
                    case "OnEnable":
                        {
                            methodOnEnable = mi;
                            break;
                        }
                    case "OnDisable":
                        {
                            methodOnDisable = mi;
                            break;
                        }
                    case "OnDestroy":
                        {
                            methodOnDestroy = mi;
                            break;
                        }
                    case "Update":
                        {
                            methodUpdate = mi;
                            break;
                        }
                    default:
                        {
                            var attributes = mi.GetCustomAttributes(typeof(CustomViewObserverMethodAttribute), false);
                            if (attributes.Length > 0)
                            {
                                var attribute = attributes[0] as CustomViewObserverMethodAttribute;
                                messageMethodMap.Add(attribute.messageName, mi);
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 处理View消息
        /// </summary>
        /// <param name="message"></param>
        public override void OnMessage(IMessage message)
        {
            var method = messageMethodMap[message.Name];
            if (method != null)
            {
                method.Invoke(scriptInstance, new object[] { message });
            }
        }
    }

    public class CustomViewObserverMethodAttribute : Attribute
    {
        public string messageName;

        public CustomViewObserverMethodAttribute(string messageName)
        {
            this.messageName = messageName;
        }
    }
}
