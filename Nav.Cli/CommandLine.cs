#region Using Directives

using System;
using System.Collections.Generic;
using NDesk.Options;

#endregion

namespace Pharmatechnik.Nav.Language; 

sealed class CommandLine {
    public CommandLine() {
        Sources = new List<string>();
    }

    public string       Directory         { get; set; }
    public List<string> Sources           { get; }
    public bool         Force             { get; set; }
    public bool         GenerateToClasses { get; set; }
    public bool         UseSyntaxCache    { get; set; }
    public bool         Verbose           { get; set; }
    public bool         FullPaths         { get; set; }
    public bool         NoWarnings        { get; set; }

    public bool   Analyze { get; set; }
    public string Pattern { get; set; }

    public string ProjectRootDirectory { get; set; }
    public string IwflRootDirectory    { get; set; }

    public static CommandLine Parse(string[] commandline) {

        bool        showHelp = false;
        CommandLine cla      = new CommandLine();
        var p = new OptionSet {
            { "d=|directory="       , "Help for dir", v => cla.Directory = v  },
            { "s=|sources="         , v => cla.Sources.Add(v)  },
            { "f|force"             , v => cla.Force   = v           != null  },
            { "t|generatetoclasses" , v => cla.GenerateToClasses = v != null },
            { "c|useSyntaxCache"    , v => cla.UseSyntaxCache    = v != null },
            { "nowarnings"          , v => cla.NoWarnings = v        != null },
            { "v|verbose"           , v => cla.Verbose    = v        != null },
            { "fullpaths"           , v => cla.FullPaths  = v        != null },
            { "i=|iwflroot"         , v => cla.IwflRootDirectory = v  },
            { "r=|projectroot"      , v => cla.ProjectRootDirectory = v  },
            { "h|?|help"            , v => showHelp = v != null },

        };

        try {
            p.Parse(commandline);
        } catch (OptionException e) {
            Console.Error.WriteLine("nav.exe: ");
            Console.Error.WriteLine(e.Message);
            Console.Error.WriteLine("Try 'nav.exe --help' for more information.");
            return null;
        }

        if (showHelp) {
            ShowHelp(p);
            return null;
        }

        return cla;          
    }
    // TODO Helptext
    static void ShowHelp(OptionSet p) {
        Console.WriteLine("Usage: nav.exe [OPTIONS]+");            
        Console.WriteLine();
        Console.WriteLine("Options:");
        p.WriteOptionDescriptions(Console.Out);
    }
}