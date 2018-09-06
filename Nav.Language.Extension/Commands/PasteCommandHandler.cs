#region Using Directives

using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Nav.Language.Extension.DropHandler;
using Pharmatechnik.Nav.Language.Text;
using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    [Export(typeof(ICommandHandler))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [Name(CommandHandlerNames.PasteCommandHandler)]
    class PasteCommandHandler: ICommandHandler<PasteCommandArgs> {

        public IEditorOperationsFactoryService EditorOperationsFactoryService { get; }

        [ImportingConstructor]
        public PasteCommandHandler(IEditorOperationsFactoryService editorOperationsFactoryService) {
            EditorOperationsFactoryService = editorOperationsFactoryService;

        }

        public string DisplayName => "Paste taskref";

        public bool ExecuteCommand(PasteCommandArgs args, CommandExecutionContext executionContext) {

            var navFile      = TryGetNavFile(Clipboard.GetDataObject());
            var navDirectory = TryGetDirectory(args.TextView);

            if (navFile == null || navDirectory == null) {
                return false;
            }

            var    directoryName    = navDirectory.FullName + Path.DirectorySeparatorChar;
            var    relativeFileName = PathHelper.GetRelativePath(fromPath: directoryName, toPath: navFile.FullName);
            string taskrefStatement = $"{SyntaxFacts.TaskrefKeyword} \"{relativeFileName}\"{SyntaxFacts.Semicolon}";

            var editorOperations = EditorOperationsFactoryService.GetEditorOperations(args.TextView);

            var selStart     = args.TextView.Selection.Start.Position;
            var position     = selStart.Position;
            var line         = selStart.GetContainingLine();
            var lineText     = line.GetText();
            var linePosition = position - line.Start;

            if (lineText.IsInQuotation(linePosition)) {
                return false;
            }

            editorOperations.ReplaceSelection(taskrefStatement);
            editorOperations.InsertNewLine();

            return true;
        }

        public CommandState GetCommandState(PasteCommandArgs args) {

            bool canExeceute = TryGetNavFile(Clipboard.GetDataObject()) != null;

            return canExeceute ? CommandState.Available : CommandState.Unavailable;
        }

        [CanBeNull]
        public static FileInfo TryGetNavFile(IDataObject dataObject) {

            var fileInfo = TryGetFile(dataObject);

            return fileInfo?.Extension == NavLanguageContentDefinitions.FileExtension ? fileInfo : null;
        }

        [CanBeNull]
        public static FileInfo TryGetFile(IDataObject dataObject) {

            var    data     = new DataObject(dataObject);
            string fileName = null;

            if (data.GetDataPresent(ClipBoardFormats.FileDrop)) {
                // The drag and drop operation came from the file system
                StringCollection files = data.GetFileDropList();

                if (files.Count == 1) {
                    fileName = files[0];
                }
            } else if (data.GetDataPresent(ClipBoardFormats.VsProjectItems)) {
                // The drag and drop operation came from the VS solution explorer
                fileName = data.GetText();
            } else if (data.GetDataPresent(typeof(string))) {
                fileName = data.GetText();
            }

            return TryGetFileInfo(fileName);
        }

        static CodeGenerationUnit GetCodeGenerationUnit(ITextView textView) {

            var semanticModelService = SemanticModelService.GetOrCreateSingelton(textView.TextBuffer);

            var generationUnitAndSnapshot = semanticModelService.UpdateSynchronously();
            var codeGenerationUnit        = generationUnitAndSnapshot.CodeGenerationUnit;

            return codeGenerationUnit;
        }

        [CanBeNull]
        static DirectoryInfo TryGetDirectory(ITextView textView) {
            return GetCodeGenerationUnit(textView)?.Syntax.SyntaxTree.SourceText.FileInfo?.Directory;
        }

        [CanBeNull]
        static FileInfo TryGetFileInfo(string candidate) {

            if (String.IsNullOrEmpty(candidate)) {
                return null;
            }

            candidate = candidate.Trim('"', '\'');

            try {
                var fileInfo = new FileInfo(candidate);
                return fileInfo.Exists ? fileInfo : null;
            } catch (ArgumentException) {
            } catch (IOException) {
            }

            return null;
        }

    }

}