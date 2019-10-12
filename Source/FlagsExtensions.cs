using System;

namespace Xplorer
{
    public static class FlagsExtensions
    {
        public static T ToggleFlag<T>(this Enum flags, Enum flag) where T : Enum
        {
            var value = Convert.ToInt32(flags);
            var result = value ^= Convert.ToInt32(flag);
            return (T)Enum.ToObject(typeof(T), result);
        }

        public static T SetFlag<T>(this Enum flags, Enum flag) where T : Enum
        {
            var value = Convert.ToInt32(flags);
            var result = value |= Convert.ToInt32(flag);
            return (T)Enum.ToObject(typeof(T), result);
        }

        public static T UnsetFlag<T>(this Enum flags, Enum flag) where T : Enum
        {
            var value = Convert.ToInt32(flags);
            var result = value &= ~Convert.ToInt32(flag);
            return (T)Enum.ToObject(typeof(T), result);
        }
    }
}