namespace Cdm.Figma.Utils
{
    public static class FontHelpers
    {
        public static string GetFontDescriptor(string family, int weight, bool italic)
        {
            return $"{family}-{(FontWeight)weight}{(italic ? "-Italic" : "")}";
        }
    }
}