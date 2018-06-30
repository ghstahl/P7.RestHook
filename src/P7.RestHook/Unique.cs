using System;

namespace P7.RestHook
{
    static public class Unique
    {
        public static string S => Guid.NewGuid().ToString("N");
        public static string G => Guid.NewGuid().ToString();
    }
}