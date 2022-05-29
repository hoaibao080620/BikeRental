using System.Text;
using System.Text.RegularExpressions;

namespace BikeService.Sonic.Extensions;

public static class StringExtension
{
    public static string ConvertToUnSign(this string source)
    {
        var regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
        var temp = source.Normalize(NormalizationForm.FormD);
        return regex.Replace(temp, string.Empty)
            .Replace('\u0111', 'd').Replace('\u0110', 'D');
    }
}