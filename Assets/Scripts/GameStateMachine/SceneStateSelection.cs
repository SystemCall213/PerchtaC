using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreLoop.Interfaces;
using UnityEngine;

namespace CoreLoop
{
    public enum SceneStateType
    {
        None = 0,
        MainMenu,
        Cinematic,
        Combat,
        Room
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SceneStateAttribute : Attribute
    {
        public SceneStateType Type { get; }

        public SceneStateAttribute(SceneStateType type) => Type = type;
    }

    public sealed class SceneStateDescriptor
    {
        public SceneStateType Type { get; }
        public Type StateType { get; }
        public Type PayloadType { get; }
        public bool HasPayload => PayloadType != null;

        public SceneStateDescriptor(SceneStateType type, Type stateType, Type payloadType)
        {
            Type = type;
            StateType = stateType;
            PayloadType = payloadType;
        }
    }

    [Serializable]
    public struct SceneStateSelection
    {
        public SceneStateType stateType;
        [SerializeReference] public IStatePayload payload;

        public void ValidateAndInitialize()
        {
            if (stateType == SceneStateType.None)
            {
                payload = null;
                return;
            }

            if (!SceneStateRegistry.TryGetDescriptor(stateType, out var descriptor))
            {
                payload = null;
                return;
            }

            if (!descriptor.HasPayload)
            {
                payload = null;
                return;
            }

            if (!PayloadMatchesState(payload, stateType))
            {
                payload = CreatePayload(stateType);
            }
        }

        public static bool PayloadMatchesState(IStatePayload payload, SceneStateType stateType)
        {
            if (payload == null) return false;
            if (!SceneStateRegistry.TryGetDescriptor(stateType, out var descriptor)) return false;

            return descriptor.PayloadType != null && descriptor.PayloadType.IsInstanceOfType(payload);
        }

        public static IStatePayload CreatePayload(SceneStateType stateType)
        {
            if (!SceneStateRegistry.TryGetDescriptor(stateType, out var descriptor) || !descriptor.HasPayload)
            {
                return null;
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(descriptor.PayloadType))
            {
                Debug.LogError($"Payload type '{descriptor.PayloadType.Name}' for scene state '{stateType}' is a UnityEngine.Object and cannot be created as a SerializeReference payload.");
                return null;
            }

            try
            {
                return (IStatePayload)Activator.CreateInstance(descriptor.PayloadType);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to create payload '{descriptor.PayloadType.Name}' for scene state '{stateType}': {exception.Message}");
                return null;
            }
        }
    }

    public static class SceneStateRegistry
    {
        private static Dictionary<SceneStateType, SceneStateDescriptor> stateDescriptors;

        public static bool TryGetDescriptor(SceneStateType stateType, out SceneStateDescriptor descriptor)
        {
            EnsureInitialized();

            if (stateType == SceneStateType.None)
            {
                descriptor = null;
                return false;
            }

            if (stateDescriptors.TryGetValue(stateType, out descriptor))
            {
                return true;
            }

            Debug.LogError($"No state class registered for scene state '{stateType}'. Add a SceneStateAttribute to the matching State class.");
            return false;
        }

        private static void EnsureInitialized()
        {
            if (stateDescriptors != null) return;

            stateDescriptors = new Dictionary<SceneStateType, SceneStateDescriptor>();
            var stateTypes = typeof(SceneStateRegistry).Assembly.GetTypes()
                .Where(t => typeof(State).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => new { StateType = t, Attr = t.GetCustomAttribute<SceneStateAttribute>() })
                .Where(x => x.Attr != null);

            foreach (var stateType in stateTypes)
            {
                if (stateDescriptors.ContainsKey(stateType.Attr.Type))
                {
                    Debug.LogError($"Duplicate scene state registration for '{stateType.Attr.Type}' on '{stateType.StateType.Name}'.");
                    continue;
                }

                stateDescriptors.Add(
                    stateType.Attr.Type,
                    new SceneStateDescriptor(stateType.Attr.Type, stateType.StateType, FindPayloadType(stateType.StateType)));
            }
        }

        private static Type FindPayloadType(Type stateType)
        {
            Type type = stateType;

            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(State<>))
                {
                    return type.GetGenericArguments()[0];
                }

                type = type.BaseType;
            }

            return null;
        }
    }
}