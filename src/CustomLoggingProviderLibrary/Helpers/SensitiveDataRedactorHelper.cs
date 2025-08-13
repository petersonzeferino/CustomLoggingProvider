using System.Text.RegularExpressions;

namespace CustomLoggingProviderLibrary.Helpers
{
    internal static class SensitiveDataRedactorHelper
    {
        /// <summary>
        /// Redacts common sensitive data from a string: emails, passwords, credit cards, and API keys.
        /// </summary>
        /// <param name="input">The input string that may contain sensitive data.</param>
        /// <returns>A string with sensitive data masked.</returns>
        public static string Redact(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            string output = input;

            // Mask email addresses
            string emailPattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b";
            output = Regex.Replace(output, emailPattern, "[REDACTED_EMAIL]");

            // Mask passwords in query strings or key-value pairs (password=...)
            string passwordPattern = @"(?i)(password\s*=\s*)[^&\s]+";
            output = Regex.Replace(output, passwordPattern, "$1[REDACTED]");

            // Mask API keys (alphanumeric strings of length 20+)
            string apiKeyPattern = @"\b[A-Za-z0-9]{20,}\b";
            output = Regex.Replace(output, apiKeyPattern, "[REDACTED_KEY]");

            return output;
        }
    }
}