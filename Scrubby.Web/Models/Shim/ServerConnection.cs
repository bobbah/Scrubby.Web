using System;

namespace Scrubby.Web.Models.Shim;

public class ServerConnection
{
    public int RoundID { get; set; }
    public CKey CKey { get; set; }
    public DateTime ConnectTime { get; set; }
    public DateTime DisconnectTime { get; set; }
    public string BYONDVersion { get; set; }
    public string Server { get; set; }
    public bool Played { get; set; }
}

public class ServerJoin
{
    public CKey CKey { get; set; }
    public int RelativeIndex { get; set; }
    public DateTime Time { get; set; }
    public string BYONDVersion { get; set; }
}

public class ServerLeave
{
    public CKey CKey { get; set; }
    public int RelativeIndex { get; set; }
    public DateTime Time { get; set; }
}