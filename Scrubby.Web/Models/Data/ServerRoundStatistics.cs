using System;

namespace Scrubby.Web.Models.Data;

public struct ServerRoundStatistic
{
    public string Server { get; set; }
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public float Hours { get; set; }
}