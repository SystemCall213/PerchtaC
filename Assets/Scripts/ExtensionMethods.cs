using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMOD;

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