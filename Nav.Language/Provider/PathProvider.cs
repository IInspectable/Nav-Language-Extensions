#region Using Directives

using System;
using System.IO;
using System.Linq;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Text;
using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Pharmatechnik.Nav.Language; 

public class PathProvider: IPathProvider {

    /// <summary>
    /// generated
    /// </summary>
    public const string GeneratedFolderName = "generated";

    /// <summary>
    /// generated
    /// </summary>
    public const string GeneratedFileNameSuffix = "generated";

    /// <summary>
    /// cs
    /// </summary>
    public const string CSharpFileExtension = "cs";

    public PathProvider(string syntaxFileName, string taskName, string generateTo = null, GenerationOptions options = null) {

        if (String.IsNullOrEmpty(syntaxFileName)) {
            throw new ArgumentException("Missing syntax filename", nameof(syntaxFileName));
        }

        if (String.IsNullOrEmpty(taskName)) {
            throw new ArgumentException("Missing taskName", nameof(taskName));
        }

        options ??= GenerationOptions.Default;

        var syntaxFileDirectoryName = Path.GetDirectoryName(syntaxFileName);

        var iwflDirectory = syntaxFileDirectoryName;
        if (!options.IwflRootDirectory.IsNullOrEmpty() &&
            !options.ProjectRootDirectory.IsNullOrEmpty()) {

            var projectRootDirectory = options.ProjectRootDirectory.EndsWith("\\")
                ? options.ProjectRootDirectory
                : options.ProjectRootDirectory + "\\";

            var relativeFileName = GetRelativePath(projectRootDirectory, syntaxFileName);
            var newFileName      = CombinePath(options.IwflRootDirectory, relativeFileName);
            iwflDirectory = Path.GetDirectoryName(newFileName);
        }

        TaskName       = taskName;
        SyntaxFileName = syntaxFileName;

        IwflGeneratedDirectory = CombinePath(iwflDirectory,           CodeGenFacts.IwflNamespaceSuffix, generateTo, GeneratedFolderName);
        WflGeneratedDirectory  = CombinePath(syntaxFileDirectoryName, CodeGenFacts.WflNamespaceSuffix,  generateTo, GeneratedFolderName);
        WflDirectory           = CombinePath(syntaxFileDirectoryName, CodeGenFacts.WflNamespaceSuffix,  generateTo);
    }

    public string TaskName               { get; }
    public string WflDirectory           { get; }
    public string WflGeneratedDirectory  { get; }
    public string IwflGeneratedDirectory { get; }

    public virtual string SyntaxFileName    { get; }
    public virtual string WfsBaseFileName   => CombinePath(WflGeneratedDirectory,  $"{TaskName}{CodeGenFacts.WfsBaseClassSuffix}.{GeneratedFileNameSuffix}.{CSharpFileExtension}");
    public virtual string IWfsFileName      => CombinePath(IwflGeneratedDirectory, $"{CodeGenFacts.InterfacePrefix}{TaskName}{CodeGenFacts.WfsClassSuffix}.{GeneratedFileNameSuffix}.{CSharpFileExtension}");
    public virtual string IBeginWfsFileName => CombinePath(WflGeneratedDirectory,  $"{CodeGenFacts.BeginInterfacePrefix}{TaskName}{CodeGenFacts.WfsClassSuffix}.{GeneratedFileNameSuffix}.{CSharpFileExtension}");
    public virtual string WfsFileName       => CombinePath(WflDirectory,           $"{TaskName}{CodeGenFacts.WfsClassSuffix}.{CSharpFileExtension}");

    public string GetRelativePath(string fromPath, string toPath) {
        return PathHelper.GetRelativePath(fromPath, toPath);
    }

    public string GetToFileName(string toClassName) {
        return CombinePath(IwflGeneratedDirectory, $"{toClassName}.{GeneratedFileNameSuffix}.{CSharpFileExtension}");
    }

    static string CombinePath(string first, params string[] parts) {
        return parts.Where(part => !String.IsNullOrEmpty(part)).Aggregate(first, Path.Combine);
    }

}