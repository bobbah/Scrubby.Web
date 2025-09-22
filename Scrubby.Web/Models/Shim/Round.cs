using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrubby.Web.Models.Shim;

public class Round : IEquatable<Round>
{
    public enum FileType
    {
        Manifest,
        Game
    }

    public int ID { get; set; }
    public List<RoundPlayer> Players { get; set; }
    public string BaseURL { get; set; }
    public List<File> Files { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime Ended { get; set; }
    public string GameMode { get; set; }
    public string Server { get; set; }
    public List<ProcessStatus> Status { get; set; }
    public RoundBuildInfo VersionInfo { get; set; }

    public bool Equals(Round other) => ID == other.ID;

    public bool HasFile(string file)
    {
        var search = Files.Where(x => x.FileName == file);
        return search.Count() != 0;
    }

    public ProcessStatus GetProcessStatus(string process)
    {
        if (Status == null || Status.Count == 0) return null;
        return Status.FirstOrDefault(x => x.Process == process);
    }

    public override string ToString() => ID.ToString();

    public class RoundPlayer : IEquatable<RoundPlayer>
    {
        public string Ckey { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public string Role { get; set; }
        public string Jointime { get; set; }
        public int RoundID { get; set; }
        public string CleanKey { get; set; }

        public bool Equals(RoundPlayer other) // this should be changed
            =>
                Name == other.Name;

        public override int GetHashCode() // this should be changed
            =>
                Name.GetHashCode();
    }
}