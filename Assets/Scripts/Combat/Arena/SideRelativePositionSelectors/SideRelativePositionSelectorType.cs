using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Combat.Arena.SideRelativePositionSelectors
{
    public enum SideRelativePositionSelectorType
    {
        Random,
        Incremental,
        BalancedRandom,
        PredifienedSideRandom
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SideRelativePositionSelectorAttribute : Attribute
    {
        public SideRelativePositionSelectorType Type { get; }
        public SideRelativePositionSelectorAttribute(SideRelativePositionSelectorType type) => Type = type;
    }

    [Serializable]
    public struct SideRelativePositionSelectorData
    {
        public SideRelativePositionSelectorType type;
        [SerializeReference] public ISideRelativePositionSelector selector;

        public void ValidateAndInitialize()
        {
            if (selector == null || !SelectorMatchesType(selector, type))
            {
                selector = CreateSelector(type);
            }
        }

        public static bool SelectorMatchesType(ISideRelativePositionSelector selector, SideRelativePositionSelectorType type)
        {
            if (selector == null) return false;
            var attr = selector.GetType().GetCustomAttribute<SideRelativePositionSelectorAttribute>();
            return attr != null && attr.Type == type;
        }

        private static Dictionary<SideRelativePositionSelectorType, Type> _selectorTypes;

        public static ISideRelativePositionSelector CreateSelector(SideRelativePositionSelectorType type)
        {
            if (_selectorTypes == null)
            {
                _selectorTypes = typeof(ISideRelativePositionSelector).Assembly.GetTypes()
                    .Where(t => typeof(ISideRelativePositionSelector).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                    .Select(t => new { Type = t, Attr = t.GetCustomAttribute<SideRelativePositionSelectorAttribute>() })
                    .Where(x => x.Attr != null)
                    .ToDictionary(x => x.Attr.Type, x => x.Type);
            }

            if (_selectorTypes.TryGetValue(type, out var selectorType))
            {
                return (ISideRelativePositionSelector)Activator.CreateInstance(selectorType);
            }

            return null;
        }
    }
}