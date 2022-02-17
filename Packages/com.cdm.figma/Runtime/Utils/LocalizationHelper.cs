using UnityEngine.Localization.Tables;

namespace Cdm.Figma.Utils
{
    public class LocalizationHelper
    {
        public static bool TryGetTableAndEntryReference(string localizationKey, 
            out TableReference tableReference, out TableEntryReference tableEntryReference)
        {
            const char delimiter = '/';

            var tokens = localizationKey.Split(delimiter);

            if (tokens.Length != 2 || string.IsNullOrWhiteSpace(tokens[0]) || string.IsNullOrWhiteSpace(tokens[1]))
            {
                tableReference = default;
                tableEntryReference = default;
                return false;
            }

            tableReference = tokens[0];
            tableEntryReference = tokens[1];
            return true;
        }
    }
}