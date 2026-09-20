using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUAcceleratedNormalGenerator
{
    public static class Extensions
    {
        public static bool ExecuteIf(this bool condition, Action action)
        {
            if (condition) action();
            return condition;
        }
        public static bool ExecuteIf<T>(this bool condition, Func<T> action)
        {
            if (condition) action();
            return condition;
        }

        // --- Execute If Not Null ---

        public static bool ExecuteIfNotNull<T>(this T obj, Action action)
        {
            bool isNotNull = obj != null;
            if (isNotNull) action();
            return isNotNull;
        }

        public static bool ExecuteIfNotNull<T, TResult>(this T obj, Func<TResult> action)
        {
            bool isNotNull = obj != null;
            if (isNotNull) action();
            return isNotNull;
        }

        // Overload: Passes the object into the action so you can use it directly
        public static bool ExecuteIfNotNull<T>(this T obj, Action<T> action)
        {
            bool isNotNull = obj != null;
            if (isNotNull) action(obj);
            return isNotNull;
        }

        public static bool ExecuteIfNotNull<T, TResult>(this T obj, Func<T, TResult> action)
        {
            bool isNotNull = obj != null;
            if (isNotNull) action(obj);
            return isNotNull;
        }

        // --- Execute If Null ---

        public static bool ExecuteIfNull<T>(this T obj, Action action)
        {
            bool isNull = obj == null;
            if (isNull) action();
            return isNull;
        }

        public static bool ExecuteIfNull<T, TResult>(this T obj, Func<TResult> action)
        {
            bool isNull = obj == null;
            if (isNull) action();
            return isNull;
        }
    }
}
