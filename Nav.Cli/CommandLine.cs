#region Using Directives

using System;
using System.Collections.Generic;
using NDesk.Options;

#endregion

namespace Pharmatechnik.Nav.Language; 

sealed record CommandLine {
   
    public CommandLine() {
        Sources = new List<string>();
    }

    public string       Directory            { get; private set; }
    public List<string> Sources              { get; }
    public bool         Force                { get; private set; }
    public bool         GenerateToClasses    { get; private set; }
    public bool         UseSyntaxCache       { get; private set; }
    public bool         Verbose              { get; private set; }
    public bool         FullPaths            { get; private set; }
    public bool         NoWarnings           { get; private set; }
    public string       ProjectRootDirectory { get; private set; }
    public string       IwflRootDirectory    { get; private set; }

    public bool   Analyze { get; set; }
    public string Pattern { get; set; }
    
    public static CommandLine Parse(string[] commandline) {

        bool        showHelp = false;
        CommandLine cla      = new CommandLine();
        var p = new OptionSet {
            { "d=|directory="       , "Alle .nav-Dateien im Verzeichnis und allen Unterverzeichnissen sind Eingabedateien.", v => cla.Directory = v },
            { "s=|sources="         , ".nav-Eingabedatei.", v => cla.Sources.Add(v) },
            { "f|force"             , "Überschreibt die Ausgabedatei(en) auch wenn sich diese nicht geändert haben.", v => cla.Force = v           != null },
            { "t|generatetoclasses" , "Erzeugt eine leere TO Klasse, wenn diese noch nicht exisitert.", v => cla.GenerateToClasses = v != null },
            { "c|useSyntaxCache"    , "Cached Syntaxen an statt sie immer wieder neu zu parsen.", v => cla.UseSyntaxCache = v             != null },
            { "nowarnings"          , "Unterdrückt Warnmeldungen in der Logausgabe.", v => cla.NoWarnings = v                                   != null },
            { "v|verbose"           , "Schreibt ausführliche Meldungen in die Logausgabe.", v => cla.Verbose = v                                 != null },
            { "fullpaths"           , "Wenn angegeben, werden in die Logausgaben ganze Pfade geschrieben.", v => cla.FullPaths = v               != null },
            { "i=|iwflroot"         , "Gibt ein alternatives IWFL Wurzelverzeichnis an.", v => cla.IwflRootDirectory = v },
            { "r=|projectroot"      , "Gibt das Project Wurzelverzeichnis an.", v => cla.ProjectRootDirectory = v },
            { "h|?|help"            , "Zeigt diese Hilfe an.", v => showHelp = v != null },

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

    static void ShowHelp(OptionSet p) {
        Console.WriteLine("Usage: nav.exe [OPTIONS]+");            
        Console.WriteLine();
        Console.WriteLine("Options:");
        p.WriteOptionDescriptions(Console.Out);
    }
}