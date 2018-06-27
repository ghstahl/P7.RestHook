using System;

namespace RestHookHost
{
    static class Unique
    {
        public static string S => Guid.NewGuid().ToString("N");
        public static string G => Guid.NewGuid().ToString();
    }
}