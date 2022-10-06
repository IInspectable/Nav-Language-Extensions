#region Using Directives

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension; 

sealed class NavLanguageContentDefinitions
{
    public const string ContentType   = "Nav";
    public const string LanguageName  = "Nav";
    public const string FileExtension = ".nav";

    [Export]
    [Name(ContentType)]
    [BaseDefinition("code")]
    internal ContentTypeDefinition GuiModelContentTypeDefinition = null;

    [Export]
    [ContentType(ContentType)]
    [FileExtension(FileExtension)]
    internal FileExtensionToContentTypeDefinition GuiModelFileExtensionDefinition = null;
}