﻿using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using VrPlayer.Helpers;
using VrPlayer.Helpers.Mvvm;

namespace VrPlayer.Contracts
{
    public abstract class PluginBase<T>: ViewModelBase, IPlugin<T> where T: ILoadable
    {
        public string Name { get; set; }
        public T Content { get; set; }
        public FrameworkElement Panel { get; set; }
        
        public void InjectConfig(PluginConfig config)
        {
            foreach (var val in config.Data)
            {
                var prop = Content.GetType().GetProperty(val.Key, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null || !prop.CanWrite) continue;
                try
                {
                    var obj = Convert.ChangeType(val.Value, prop.PropertyType);
                    prop.SetValue(Content, obj, null);
                }
                catch (Exception exc)
                {
                    Logger.Instance.Error(
                        string.Format("Error while injecting config for '{0}'. Could not assign value '{1}' of type '{2}' to key '{3}'.", 
                        this.GetType().FullName,
                        val.Value,
                        prop.PropertyType.FullName,
                        val.Key), 
                        exc);
                }
            }
        }

        public PluginConfig ExtractConfig()
        {
            var config = new PluginConfig();
            config.Type = GetType().FullName;

            if (Content != null)
            {    
                foreach (var prop in Content.GetType().GetProperties())
                {
                    var attribute = (DataMemberAttribute)prop.GetCustomAttributes(typeof(DataMemberAttribute), false).FirstOrDefault(); 
                    if (attribute != null) 
                        config.Data.Add(new DataItem(prop.Name, prop.GetValue(Content, null).ToString()));
                } 
            }
            
            return config;
        }

        public void Load()
        {
            if (Content != null)
                Content.Load();
        }

        public void Unload()
        {
            if (Content != null)
                Content.Unload();
        }
    }
}