using System;
using System.Collections.Generic;
using System.Linq;
using Scrubby.Web.Models.Data;
using Scrubby.Web.Models.Shim;

namespace Scrubby.Web.Models;

public class RoundModel
{
    public int LastID { get; set; }
    public int NextID { get; set; }
    public ScrubbyRound CurrentRound { get; set; }
    public List<string> HightlightedCkeys { get; set; }
    public List<CKey> NonPlaying { get; set; }

    public bool CompletedProcess(string name)
    {
        return CurrentRound?.Status?.Any(x =>
                   x.Process.Equals(name, StringComparison.InvariantCultureIgnoreCase) &&
                   x.Status == Status.Complete) ??
               false;
    }

    public Dictionary<string, List<Round.RoundPlayer>> PlayersByDepartment()
    {
        if (CurrentRound.Players == null) return null;

        var toReturn = new Dictionary<string, List<Round.RoundPlayer>>
        {
            { "command", new List<Round.RoundPlayer>() },
            { "synthetic", new List<Round.RoundPlayer>() },
            { "service", new List<Round.RoundPlayer>() },
            { "engineering", new List<Round.RoundPlayer>() },
            { "cargo", new List<Round.RoundPlayer>() },
            { "security", new List<Round.RoundPlayer>() },
            { "medical", new List<Round.RoundPlayer>() },
            { "research", new List<Round.RoundPlayer>() },
            { "assistant", new List<Round.RoundPlayer>() },
            { "civilian", new List<Round.RoundPlayer>() },
            { "other", new List<Round.RoundPlayer>() }
        };
        var heads = new List<Round.RoundPlayer>();

        foreach (var player in CurrentRound.Players)
            switch (player.Job)
            {
                case "assistant":
                    toReturn["assistant"].Add(player);
                    break;
                case "clown":
                case "mime":
                case "curator":
                case "lawyer":
                case "chaplain":
                    toReturn["civilian"].Add(player);
                    break;
                case "head of personnel":
                    toReturn["command"].Add(player);
                    break;
                case "cargo technician":
                case "shaft miner":
                    toReturn["cargo"].Add(player);
                    break;
                case "bartender":
                case "cook":
                case "botanist":
                case "janitor":
                    toReturn["service"].Add(player);
                    break;
                case "station engineer":
                case "atmospheric technician":
                    toReturn["engineering"].Add(player);
                    break;
                case "medical doctor":
                case "chemist":
                case "geneticist":
                case "virologist":
                case "paramedic":
                case "psychologist":
                case "coroner":
                    toReturn["medical"].Add(player);
                    break;
                case "scientist":
                case "roboticist":
                    toReturn["research"].Add(player);
                    break;
                case "warden":
                case "detective":
                case "security officer":
                    toReturn["security"].Add(player);
                    break;
                case "cyborg":
                    toReturn["synthetic"].Add(player);
                    break;
                case "captain":
                case "quartermaster":
                case "chief engineer":
                case "chief medical officer":
                case "research director":
                case "head of security":
                case "ai":
                    heads.Add(player);
                    break;
                default:
                    toReturn["other"].Add(player);
                    break;
            }

        foreach (var department in toReturn)
            department.Value.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));

        foreach (var player in heads)
            switch (player.Job)
            {
                case "captain":
                    toReturn["command"].Insert(0, player);
                    break;
                case "quartermaster":
                    toReturn["cargo"].Insert(0, player);
                    break;
                case "chief engineer":
                    toReturn["engineering"].Insert(0, player);
                    break;
                case "chief medical officer":
                    toReturn["medical"].Insert(0, player);
                    break;
                case "research director":
                    toReturn["research"].Insert(0, player);
                    break;
                case "head of security":
                    toReturn["security"].Insert(0, player);
                    break;
                case "ai":
                    toReturn["synthetic"].Insert(0, player);
                    break;
            }

        return toReturn;
    }
}