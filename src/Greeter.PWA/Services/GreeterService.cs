using Sysinfocus.AspNetCore.Components;
using System.Text.Json;

namespace Greeter.PWA.Services;

internal class GreeterService(BrowserExtensions browserExtensions)
{
    internal async Task<ICollection<EventModel>> LoadEvents()
    {
        var json = await browserExtensions.GetFromLocalStorage("events");
        if (string.IsNullOrWhiteSpace(json)) return [];
        var models = JsonSerializer.Deserialize<ICollection<EventModel>>(json) ?? [];
        return [..models.OrderBy(x => x.Date.Month).ThenBy(x => x.Date.Day)];
    }

    internal async Task SaveEvents(ICollection<EventModel> events)
    {
        var json = JsonSerializer.Serialize(events);
        await browserExtensions.SetToLocalStorage("events", json);
    }    

    internal async Task ShareApp()
    {
        var data = new
        {
            text = "Check out this amazing Greeter app @ https://sysinfocus.github.io/greeter",
            title = "Sysinfocus Greeter"
        };
        await browserExtensions.InvokeShare(data);
    }

    internal void SendSMS(EventModel evt)
    {
        var message = Uri.EscapeDataString(evt.Message);
        var url = $"sms:{evt.Mobile}?body={message}";
        browserExtensions.Goto(url);
    }

    internal void SendWhatsApp(EventModel evt)
    {
        var message = Uri.EscapeDataString(evt.Message);
        var url = $"https://wa.me/{evt.Mobile}?text={message}";
        browserExtensions.Goto(url);
    }
}

public static class GreeterExtensions
{
    internal static readonly int[] firstNumbers = [1, 21, 31, 41, 51, 61, 71, 81, 91, 101, 201, 301, 401];
    internal static readonly int[] secondNumbers = [2, 22, 32, 42, 52, 62, 72, 82, 92, 102, 202, 302, 402];
    internal static readonly int[] thirdNumbers = [3, 23, 33, 43, 53, 63, 73, 83, 93, 103, 203, 303, 403];
    
    public static string ShowCustomDate(DateTime date)
    {
        if (firstNumbers.Contains(date.Day)) return date.ToString("d'<sup>st</sup>' MMM");
        if (secondNumbers.Contains(date.Day)) return date.ToString("d'<sup>nd</sup>' MMM");
        if (thirdNumbers.Contains(date.Day)) return date.ToString("d'<sup>rd</sup>' MMM");
        return date.ToString("d'<sup>th</sup>' MMM");
    }

    public static string FormattedNumber(DateTime date)
    {
        var years = DateTime.Now.Year - date.Year;
        if (years <= 0) return string.Empty;
        if (firstNumbers.Contains(years)) return years.ToString("(0'<sup>st</sup>')");
        if (secondNumbers.Contains(years)) return years.ToString("(0'<sup>nd</sup>')");
        if (thirdNumbers.Contains(years)) return years.ToString("(0'<sup>rd</sup>')");
        return years.ToString("(0'<sup>th</sup>')");
    }
}