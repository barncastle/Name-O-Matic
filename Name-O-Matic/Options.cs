using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

namespace NameOMatic
{
    class Options
    {
        [Option("path", Required = true, HelpText = "TACT/CAS repository location")]
        public string Path { get; set; }

        [Option("prod", SetName = "CASCLib", HelpText = "CAS product for multi-game installaytions")]
        public string Product { get; set; } = "wowt";

        [Option("bc", SetName = "TACTNet", HelpText = "Build Config hash")]
        public string BuildConfig { get; set; }

        [Option("cc", SetName = "TACTNet", HelpText = "CDN Config hash")]
        public string CDNConfig { get; set; }

        [Option("diff", HelpText = "Root EKey of another build to diff against")]
        public string DiffHash { get; set; }

        [Option("type", HelpText = "", Separator = ',')]
        public IEnumerable<string> Formats { get; set; }


        public bool IsValidFormat(string format)
        {
            return !Formats.Any() || Formats.Contains(format, StringComparer.OrdinalIgnoreCase);
        }
    }
}
