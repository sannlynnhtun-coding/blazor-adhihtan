namespace Adhihtan.Services;

public static class BurmeseDateFormatter
{
    private static readonly string[] Days =
        ["တနင်္ဂနွေ", "တနင်္လာ", "အင်္ဂါ", "ဗုဒ္ဓဟူး", "ကြာသပတေး", "သောကြာ", "စနေ"];

    private static readonly string[] Months =
        ["ဇန်နဝါရီ", "ဖေဖော်ဝါရီ", "မတ်", "ဧပြီ", "မေ", "ဇွန်", "ဇူလိုင်", "ဩဂုတ်", "စက်တင်ဘာ", "အောက်တိုဘာ", "နိုဝင်ဘာ", "ဒီဇင်ဘာ"];

    public static string Long(DateTime date) =>
        $"{Days[(int)date.DayOfWeek]}နေ့၊ {AppStateService.ToMyanmarNumber(date.Day)} {Months[date.Month - 1]} {AppStateService.ToMyanmarNumber(date.Year)}";

    public static string Short(DateTime date) =>
        $"{AppStateService.ToMyanmarNumber(date.Day)}/{AppStateService.ToMyanmarNumber(date.Month)}";
}
