using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Combat.Arena
{
    public enum RadialPositionSelectorType
    {
        BalancedRandom,
        Random,
        SideRelativeRandom,
        Incremential
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class RadialPositionSelectorAttribute : Attribute
    {
        public RadialPositionSelectorType Type { get; }
        public RadialPositionSelectorAttribute(RadialPositionSelectorType type) => Type = type;
    }

    [Serializable]
    public struct RadialPositionSelectorData
    {
        public RadialPositionSelectorType type;
        [SerializeReference] public IRadialPositionSelector selector;

        public void ValidateAndInitialize()
        {
            if (selector == null || !SelectorMatchesType(selector, type))
            {
                selector = CreateSelector(type);
            }
        }

        public static bool SelectorMatchesType(IRadialPositionSelector selector, RadialPositionSelectorType type)
        {
            if (selector == null) return false;
            var attr = selector.GetType().GetCustomAttribute<RadialPositionSelectorAttribute>();
            return attr != null && attr.Type == type;
        }

        private static Dictionary<RadialPositionSelectorType, Type> _selectorTypes;

        public static IRadialPositionSelector CreateSelector(RadialPositionSelectorType type)
        {
            if (_selectorTypes == null)
            {
                _selectorTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => typeof(IRadialPositionSelector).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                    .Select(t => new { Type = t, Attr = t.GetCustomAttribute<RadialPositionSelectorAttribute>() })
                    .Where(x => x.Attr != null)
                    .ToDictionary(x => x.Attr.Type, x => x.Type);
            }

            if (_selectorTypes.TryGetValue(type, out var selectorType))
            {
                return (IRadialPositionSelector)Activator.CreateInstance(selectorType);
            }

            return null;
        }
    }
}