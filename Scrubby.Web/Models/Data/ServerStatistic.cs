using System;

namespace Scrubby.Web.Models.Data;

public class ServerStatistic
{
    public string Server { get; set; }

    public int Played { get; set; }
    public int Connected { get; set; }
    public long PlayedMillisec { get; set; }
    public long ConnectedMillisec { get; set; }
    public TimeSpan PlayedTime => TimeSpan.FromMilliseconds(PlayedMillisec);
    public TimeSpan ConnectedTime => TimeSpan.FromMilliseconds(ConnectedMillisec);
    public int TotalConnections => Played + Connected;
    public TimeSpan TotalTimeConnected => PlayedTime + ConnectedTime;
    public string TotalTimeConnectedString => TimeToString(TotalTimeConnected);
    public string PlayedTimeString => TimeToString(PlayedTime);
    public string ConnectedTimeString => TimeToString(ConnectedTime);

    public static string TimeToString(TimeSpan time)
    {
        return $"{(time.Days > 0 ? $"{time.Days} days, " : "")}" +
               $"{(time.Hours > 0 ? $"{time.Hours} hours, " : "")}" +
               $"{time.Minutes} minutes";
    }
}