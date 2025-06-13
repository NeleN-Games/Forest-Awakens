using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace Services
{
    public static class ServiceLocator
    {
        private static readonly List<IInitializable> InitializablesInOrder = new();

        private static readonly Dictionary<Type, IInitializable> InitializablesByType = new();
        public static void Register<T>(T service) where T : IInitializable
        {
            var type = typeof(T);
            if (InitializablesByType.ContainsKey(type))
                throw new Exception($"Service of type {type.Name} already registered");

            InitializablesInOrder.Add(service);
            InitializablesByType[type] = service;
        }
        public static void InitializeAll()
        {          

            foreach (var service in InitializablesInOrder)
            {
                try
                {
                    service.Initialize();
                }
                catch (Exception e)
                {
                    Debug.LogError($"service initialization error: one or more initialized service of type are missing");
                    Console.WriteLine(e);
                    throw;
                }

            }
        }
    
        public static T Get<T>() where T : class, IInitializable
        {
            if (InitializablesByType.TryGetValue(typeof(T), out var service))
                return service as T;

            throw new Exception($"Service of type {typeof(T).Name} not registered");
        }


        public static void Clear()
        {
            InitializablesInOrder.Clear();
        }
    }
}