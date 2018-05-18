// --------------------------------------
//  Unity3d Mvvm Toolkit and Lobby
//  PropertyBinderEditor.cs
//  copyright (c) 2014 Nicholas Ventimiglia, http://avariceonline.com
//  All rights reserved.
//  -------------------------------------
// 

using System;
using System.Linq;
using System.Reflection;
using Foundation.Databinding;
using UnityEditor;
using UnityEngine;
using XLua;

namespace Foundation.Editor
{
    /// <summary>
    ///     Handles the finding of the Context
    /// </summary>
    [CustomEditor(typeof(BindingBase), true)]
    public class BinderEditor : UnityEditor.Editor
    {
        LuaEnv luaEnv;
        LuaTable luaTable;

        void OnEnable()
        {
            if (luaTable == null)
            {
                luaEnv = new LuaEnv();
                luaTable = luaEnv.NewTable();
                LuaTable metaTable = luaEnv.NewTable();
                metaTable.Set("__index", luaEnv.Global);
                luaTable.SetMetaTable(metaTable);
                metaTable.Dispose();
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            if (luaTable != null)
            {
                luaTable.Dispose();
                luaEnv.Dispose();
            }
        }

        protected BindingBase Target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Target = target as BindingBase;

            if (!Application.isPlaying)
                Target.FindContext();

            if (Target.Context == null)
            {
                EditorGUILayout.LabelField("BindingContext not found.");
                PropertyTextInputs();
            }
            else if (Target.Context.DataType == null)
            {
                EditorGUILayout.LabelField("BindingContext.DataType not found.");
                PropertyTextInputs();
            }
            else
            {

                Target.Init();

                foreach (var binding in Target.GetBindingInfos())
                {
                    if (binding.ShouldShow != null && !binding.ShouldShow())
                        continue;

                    PropertyDropDown(binding);
                }
            }
        }

        private void PropertyDropDown(BindingBase.BindingInfo info)
        {
            var type = Target.Context.DataType;

            var labels = new System.Collections.Generic.List<string>();
            var names = new System.Collections.Generic.List<string>();

            if (!Target.Context.DataType.IsAssignableFrom(typeof(GameSystem.LuaObservableBehaviour)))
            {

                var members = new MemberInfo[0];

                // filter
                switch (info.Filters)
                {
                    case BindingBase.BindingFilter.Commands:
                        members = EditorMembersHelper.GetMethods(type);
                        break;
                    case BindingBase.BindingFilter.Properties:
                        members = EditorMembersHelper.GetProperties(type);
                        break;
                }

                // filter
                if (info.FilterTypes != null)
                {
                    members = members.Where(o => info.FilterTypes.Any(t => ValidType(t, o.GetParamaterType()))).ToArray();
                }

                labels = members.Select(o => string.Format("{0} : {1}", o.Name, o.GetParamaterType())).ToList();
                names = members.Select(o => o.Name).ToList();
            }
            else
            {
                var luaObervableBehaviour = Target.Context.DataInstance as GameSystem.LuaObservableBehaviour;
                string luaScript = luaObervableBehaviour.GetLuaScript();
                if (!string.IsNullOrEmpty(luaScript))
                {
                    luaEnv.DoString(luaScript, GetType().Name, luaTable);
                    var keys = luaTable.GetKeys<string>().OrderBy(c => c);
                    foreach (var k in keys)
                    {
                        if (k == "set" || k == "get") continue;

                        object value;
                        if (info.FilterTypes != null)
                        {
                            foreach (var t in info.FilterTypes)
                            {
                                if (luaTable.TryGet(k, t, out value))
                                {
                                    labels.Add(string.Format("{0} : {1}", k, value.GetType()));
                                    names.Add(k);
                                }
                            }
                        }
                        else
                        {
                            if (luaTable.TryGet(k, out value))
                            {
                                Type t = value.GetType();
                                if (info.Filters == BindingBase.BindingFilter.Commands && t == typeof(LuaFunction)
                                || info.Filters == BindingBase.BindingFilter.Properties && t != typeof(LuaFunction))
                                {
                                    labels.Add(string.Format("{0} : {1}", k, value.GetType()));
                                    names.Add(k);
                                }
                            }
                        }
                    }
                    luaEnv.Tick();
                }
            }

            if (labels.Count == 0)
            {
                EditorGUILayout.LabelField(string.Format("{0}->{1} has no valid members.", info.BindingName, type.Name));
                return;
            }

            labels.Insert(0, "Null");
            names.Insert(0, "");

            var index = names.FindIndex(c => c == info.MemberName);
            var i = EditorGUILayout.Popup(info.BindingName, index, labels.ToArray());

            if (i < 0)
            {
                return;
            }
            if (i != index || info.MemberName != names[i])
            {
                info.MemberName = names[i];
                EditorUtility.SetDirty(target);
            }
        }

        private void PropertyTextInputs()
        {
            Target.Init();

            foreach (var binding in Target.GetBindingInfos())
            {
                var p = EditorGUILayout.TextField(binding.BindingName, binding.MemberName);

                if (p != binding.MemberName)
                {
                    binding.MemberName = p;
                    EditorUtility.SetDirty(target);
                }
            }
        }

        private bool ValidType(Type filteredType, Type memberType)
        {
            return filteredType.IsAssignableFrom(memberType);
        }
    }
}