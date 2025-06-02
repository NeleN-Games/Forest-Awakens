using System;
using System.Collections.Generic;
using Interfaces;

namespace Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T service)
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
                throw new Exception($"Service of type {type.Name} already registered");

            _services[type] = service;
            
            if (service is IInitializable initializable)
            {
                initializable.Initialize();
            }
        }

        public static T Get<T>()
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
                return (T)service;

            throw new Exception($"Service of type {type.Name} not registered");
        }

        public static void Clear()
        {
            _services.Clear();
        }
    }
}