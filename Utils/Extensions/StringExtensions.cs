using Microsoft.Xna.Framework;
using Terraria.UI.Chat;

namespace BetterExpertRarity.Utils.Extensions
{
    public static class StringExtensions
    {
        public static TextSnippet[] ToSnippets(this string text)
        {
            var snippets = ChatManager.ParseMessage(text ?? " ", Color.White).ToArray();
            ChatManager.ConvertNormalSnippets(snippets);
            return snippets;
        }
    }
}