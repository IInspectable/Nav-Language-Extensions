#region Using Directives

using System.Windows;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands; 

[Export(typeof(ICommandHandler))]
[ContentType(NavLanguageContentDefinitions.ContentType)]
[TextViewRole(PredefinedTextViewRoles.Editable)]
[Name(CommandHandlerNames.PasteCommandHandler)]
class PasteCommandHandler: ICommandHandler<PasteCommandArgs> {

    readonly NavEditorOperationsProvider _navEditorOperationsProvider;

    [ImportingConstructor]
    public PasteCommandHandler(NavEditorOperationsProvider navEditorOperationsProvider) {
        _navEditorOperationsProvider = navEditorOperationsProvider;

    }

    public string DisplayName => "Paste";

    public bool ExecuteCommand(PasteCommandArgs args, CommandExecutionContext executionContext) {
            
        ThreadHelper.ThrowIfNotOnUIThread();

        var pasteCommand = GetPasteNavFileCommand(args);
        return pasteCommand.Execute(Clipboard.GetDataObject());

    }

    public CommandState GetCommandState(PasteCommandArgs args) {

        var pasteCommand = GetPasteNavFileCommand(args);
        return pasteCommand.CanExecute(Clipboard.GetDataObject()) ? CommandState.Available : CommandState.Unavailable;
    }

    PasteNavFileCommand GetPasteNavFileCommand(PasteCommandArgs args) {

        var pasteCommand = _navEditorOperationsProvider.CreatePasteNavFileCommand(args.TextView);
        return pasteCommand;
    }

}