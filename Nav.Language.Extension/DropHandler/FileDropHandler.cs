#region Using Directives

using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.DragDrop;

using System;
using System.Collections.Specialized;
using System.IO;
using System.Windows;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.DropHandler {

    class FileDropHandler: IDropHandler {

        readonly IWpfTextView _textView;

        public FileDropHandler(IWpfTextView textView) {
            _textView = textView;
        }

        public DragDropPointerEffects HandleDataDropped(DragDropInfo dragDropInfo) {

            var dragedFile   = GetDragedFile(dragDropInfo);
            var navDirectory = GetNavDirectory();

            if (dragedFile == null || navDirectory == null) {
                return DragDropPointerEffects.None;
            }

            var directoryName = navDirectory.FullName + Path.DirectorySeparatorChar;
            var    relativeFileName = PathHelper.GetRelativePath(fromPath: directoryName, toPath: dragedFile.FullName);
            int    position         = dragDropInfo.VirtualBufferPosition.Position.Position;
            string taskrefStatement = $"{SyntaxFacts.TaskrefKeyword} \"{relativeFileName}\"{SyntaxFacts.Semicolon}{Environment.NewLine}";

            _textView.TextBuffer.Insert(position, taskrefStatement);

            return DragDropPointerEffects.Copy;
        }

        public void HandleDragCanceled() {
        }

        public DragDropPointerEffects HandleDragStarted(DragDropInfo dragDropInfo) {
            return DragDropPointerEffects.Link;
        }

        public DragDropPointerEffects HandleDraggingOver(DragDropInfo dragDropInfo) {
            return DragDropPointerEffects.Link;
        }

        public bool IsDropEnabled(DragDropInfo dragDropInfo) {

            var fileInfo = GetDragedFile(dragDropInfo);

            return fileInfo?.Extension == NavLanguageContentDefinitions.FileExtension && GetNavDirectory() != null;
        }

        [CanBeNull]
        private static FileInfo GetDragedFile(DragDropInfo info) {

            var    data     = new DataObject(info.Data);
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
            }

            if (!String.IsNullOrEmpty(fileName) &&
                File.Exists(fileName)) {
                return new FileInfo(fileName);
            }

            return null;
        }

        CodeGenerationUnit GetCodeGenerationUnit() {

            var semanticModelService = SemanticModelService.GetOrCreateSingelton(_textView.TextBuffer);

            var generationUnitAndSnapshot = semanticModelService.UpdateSynchronously();
            var codeGenerationUnit        = generationUnitAndSnapshot.CodeGenerationUnit;

            return codeGenerationUnit;
        }

        [CanBeNull]
        DirectoryInfo GetNavDirectory() {
            return GetCodeGenerationUnit()?.Syntax.SyntaxTree.SourceText.FileInfo?.Directory;
        }

    }

}