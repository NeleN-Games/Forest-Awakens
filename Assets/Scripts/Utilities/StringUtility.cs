using UnityEngine;

namespace Utilities
{
    public class StringUtility : MonoBehaviour
    {
        public static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            if (input.Length == 1)
                return input.ToUpper();
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
