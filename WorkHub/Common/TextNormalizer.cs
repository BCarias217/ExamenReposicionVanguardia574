using System.Globalization;
using System.Text.RegularExpressions;

namespace WorkHub.Common
{
    public static class TextNormalizer
    {
        public static string Normalizar(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            var recortado = texto.Trim();
            var sinEspaciosDobles = Regex.Replace(recortado, @"\s+", " ");

            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(sinEspaciosDobles.ToLower());
        }
    }
}