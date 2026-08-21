using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace KuzuStudios.Kutils
{
    public static class StringUtils
    {
        public static string GetUniqueName(string baseName, List<string> strings)
        {
            int maxIndex = -1;

            foreach (var item in strings)
            {
                if (string.IsNullOrEmpty(item)) continue;

                if (item == baseName)
                {
                    maxIndex = Mathf.Max(maxIndex, 0);
                    continue;
                }

                var match = Regex.Match(item, $@"^{baseName} (\d+)$");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int index))
                {
                    if (index > maxIndex)
                    {
                        maxIndex = index;
                    }
                }
            }

            return maxIndex < 0 ? baseName : $"{baseName} {maxIndex + 1}";
        }
    }
}