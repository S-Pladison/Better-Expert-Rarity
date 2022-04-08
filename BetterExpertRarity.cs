using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace BetterExpertRarity
{
    public class BetterExpertRarity : Mod
    {
        public static readonly FieldInfo MouseTextCacheInfo = typeof(Main).GetField("_mouseTextCache", BindingFlags.NonPublic | BindingFlags.Instance);
        public static readonly Type MouseTextCacheType = MouseTextCacheInfo?.GetValue(Main.instance).GetType();

        public static readonly FieldInfo MouseCursorTextInfo = MouseTextCacheType?.GetField("cursorText", BindingFlags.Public | BindingFlags.Instance);
        public static readonly FieldInfo MouseRareInfo = MouseTextCacheType?.GetField("rare", BindingFlags.Public | BindingFlags.Instance);
        public static readonly FieldInfo MouseIsValidInfo = MouseTextCacheType?.GetField("isValid", BindingFlags.Public | BindingFlags.Instance);
    }
}