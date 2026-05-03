using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public static class ExtensionMethods
    {
        public static List<T> Shuffle<T>(this List<T> list)
        {
            return list.OrderBy(_ => Guid.NewGuid()).ToList();
        }
        
        public static Vector2 ConvertToVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }
    }
}