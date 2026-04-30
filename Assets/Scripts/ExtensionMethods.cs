using System;
using System.Collections.Generic;
using System.Linq;

namespace DefaultNamespace
{
    public static class ExtensionMethods
    {
        public static List<T> Shuffle<T>(this List<T> list)
        {
            return list.OrderBy(_ => Guid.NewGuid()).ToList();
        }
    }
}