#region Using Directives

using System.Collections.Specialized;
using System.IO;
using System.Windows;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;

using Pharmatechnik.Nav.Language.Extension.DropHandler;
using Pharmatechnik.Nav.Language.Text;
using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    class PasteNavFileCommand {

        readonly IEditorOperationsFactoryService _editorOperationsFactoryService;

        public PasteNavFileCommand(ITextView textView, IEditorOperationsFactoryService editorOperationsFactoryService) {
            TextView                        = textView;
            _editorOperationsFactoryService = editorOperationsFactoryService;

        }

        public ITextView TextView { get; }

        public bool Execute(IDataObject dataObject) {

            ThreadHelper.ThrowIfNotOnUIThread();

            var navFileToReference = TryGetNavFile(dataObject);
            var navDirectory       = TryGetDirectory();

            if (navFileToReference == null || navDirectory == null) {
                return false;
            }

            var    directoryName    = navDirectory.FullName + Path.DirectorySeparatorChar;
            var    relativeFileName = PathHelper.GetRelativePath(fromPath: directoryName, toPath: navFileToReference.FullName);
            string taskrefStatement = $"{SyntaxFacts.TaskrefKeyword} \"{relativeFileName}\"{SyntaxFacts.Semicolon}";

            var editorOperations = _editorOperationsFactoryService.GetEditorOperations(TextView);

            var selStart     = TextView.Selection.Start.Position;
            var position     = selStart.Position;
            var line         = selStart.GetContainingLine();
            var lineText     = line.GetText();
            var linePosition = position - line.Start;

            if (lineText.IsInQuotation(linePosition)) {
                return false;
            }

            editorOperations.InsertText(taskrefStatement);

            return true;
        }

        public bool CanExecute(IDataObject dataObject) {

            bool canExecute = TryGetNavFile(dataObject) != null;

            return canExecute;
        }

        [CanBeNull]
        static FileInfo TryGetNavFile(IDataObject dataObject) {

            var fileInfo = TryGetFile(dataObject);

            return fileInfo?.Extension == NavLanguageContentDefinitions.FileExtension ? fileInfo : null;
        }

        [CanBeNull]
        static FileInfo TryGetFile(IDataObject dataObject) {

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

        CodeGenerationUnit GetCodeGenerationUnit() {

            ThreadHelper.ThrowIfNotOnUIThread();

            var semanticModelService = SemanticModelService.GetOrCreateSingelton(TextView.TextBuffer);

            var generationUnitAndSnapshot = semanticModelService.UpdateSynchronously();
            var codeGenerationUnit        = generationUnitAndSnapshot.CodeGenerationUnit;

            return codeGenerationUnit;
        }

        [CanBeNull]
        DirectoryInfo TryGetDirectory() {
            ThreadHelper.ThrowIfNotOnUIThread();
            return GetCodeGenerationUnit()?.Syntax.SyntaxTree.SourceText.FileInfo?.Directory;
        }

        [CanBeNull]
        static FileInfo TryGetFileInfo(string candidate) {

            PathHelper.TryGetFileInfo(candidate, out var fileInfo);
            return fileInfo;

        }

    }

}