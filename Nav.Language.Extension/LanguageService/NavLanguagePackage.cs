#region Using Directives

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using EnvDTE;
using JetBrains.Annotations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using Pharmatechnik.Nav.Language.Extension.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.LanguageService {

    #region Documentation
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    #endregion
    [ProvideLanguageService(typeof(NavLanguageInfo),
                            NavLanguageContentDefinitions.LanguageName,
                            101,
                            AutoOutlining         = true,        
                            MatchBraces           = true,
                            ShowSmartIndent       = false,
                            DefaultToInsertSpaces = true,
                            MatchBracesAtCaret    = true,
                            RequestStockColors    = true,
                            ShowDropDownOptions   = false)]
    [InstalledProductRegistration("#110", "#112", ThisAssembly.ProductVersion, IconResourceID = 400)]
    [ProvideLanguageExtension(typeof(NavLanguageInfo), NavLanguageContentDefinitions.FileExtension)]
    [PackageRegistration(UseManagedResourcesOnly = true)]   
    [Guid(GuidList.NavPackageGuid)]
    [ProvideAutoLoad("{adfc4e64-0397-11d1-9f4e-00a0c911004f}")] // VSConstants.UICONTEXT_NoSolution
    [ProvideAutoLoad("{f1536ef8-92ec-443c-9ed7-fdadf150da82}")] // VSConstants.UICONTEXT_SolutionExists
    sealed partial class NavLanguagePackage : Package {

        // ReSharper disable once EmptyConstructor
        public NavLanguagePackage() {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Documentation
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        #endregion
        protected override void Initialize() {

            var langService = new NavLanguageInfo(this);
            ((IServiceContainer)this).AddService(langService.GetType(), langService, true);

            ((IServiceContainer)this).AddService(GetType(), this, true);

            base.Initialize();
        }

        public static object GetGlobalService<TService>() where TService : class {
            return GetGlobalService(typeof(TService));
        }

        public static TInterface GetGlobalService<TService, TInterface>() where TInterface : class {
            return GetGlobalService(typeof(TService)) as TInterface;
        }

        static IServiceProvider GetServiceProvider() {
            var serviceProvider = GetGlobalService<NavLanguagePackage, IServiceProvider>();
            return serviceProvider;
        }

        public static VisualStudioWorkspace Workspace {
            get {
                var componentModel = GetGlobalService<SComponentModel, IComponentModel>();
                var workspace = componentModel.GetService<VisualStudioWorkspace>();
                return workspace;
            }
        }

        [CanBeNull]
        public static IWpfTextView GoToLocationInPreviewTab(Location location) {

            if(location == null) {
                return null;
            }

            IWpfTextView wpfTextView = null;
            if (location.FilePath != null) {
                wpfTextView=OpenFileInPreviewTab(location.FilePath);
            }

            var selection = DTE?.ActiveDocument.Selection as TextSelection;
            selection?.MoveToLineAndOffset(Line: location.StartLine + 1, Offset: location.StartCharacter + 1);
            selection?.MoveToLineAndOffset(Line: location.EndLine   + 1, Offset: location.EndCharacter   + 1, Extend: true);

            return wpfTextView;
        }

        [CanBeNull]
        internal static IWpfTextView GoToLocationInPreviewTabWithWaitIndicator(IWaitIndicator waitIndicator, Func<CancellationToken, Task<Location>> getLocationTask) {
            // TODO Titel etc. zentralisieren
            // TODO Evtl. überarbeiten
            using (var wait = waitIndicator.StartWait(title: "Nav Language Extensions", message: "Searching Location...", allowCancel: true)) {

                var task = getLocationTask(wait.CancellationToken);
                task.Wait(wait.CancellationToken);
                if (task.IsCanceled) {
                    return null;
                }

                wait.AllowCancel = false;
                wait.Message     = "Opening file...";

                var location     = task.Result;
                return GoToLocationInPreviewTab(location);
            }
        }

        [CanBeNull]
        public static IWpfTextView OpenFile(string file) {

            var serviceProvider = GetServiceProvider();

            Guid logicalView = Guid.Empty;
            IVsUIHierarchy hierarchy;
            uint itemID;
            IVsWindowFrame windowFrame;
            VsShellUtilities.OpenDocument(serviceProvider, file, logicalView, out hierarchy, out itemID, out windowFrame);

            return GetWpfTextViewFromFrame(windowFrame);
        }
        
        [CanBeNull]
        public static IWpfTextView OpenFileInPreviewTab(string file) {
            IVsNewDocumentStateContext newDocumentStateContext = null;

            try {
                var openDoc3 = GetGlobalService<SVsUIShellOpenDocument, IVsUIShellOpenDocument3>();

                Guid reason = VSConstants.NewDocumentStateReason.Navigation;
                newDocumentStateContext = openDoc3?.SetNewDocumentState((uint)__VSNEWDOCUMENTSTATE.NDS_Provisional, ref reason);

                return OpenFile(file);

            } finally {
                newDocumentStateContext?.Restore();
            }
        }      

        [CanBeNull]
        public static ITextBuffer GetOpenTextBufferForFile(string filePath) { 

            var package = GetGlobalService<NavLanguagePackage, NavLanguagePackage>();

            var componentModel = (IComponentModel)GetGlobalService(typeof(SComponentModel));
            var editorAdapterFactoryService = componentModel.GetService<IVsEditorAdaptersFactoryService>();

            IVsUIHierarchy uiHierarchy;
            uint itemId;
            IVsWindowFrame windowFrame;
            if (VsShellUtilities.IsDocumentOpen(
              package,
              filePath,
              Guid.Empty,
              out uiHierarchy,
              out itemId,
              out windowFrame)) {
                IVsTextView view = VsShellUtilities.GetTextView(windowFrame);
                IVsTextLines lines;
                if (view.GetBuffer(out lines) == 0) {
                    var buffer = lines as IVsTextBuffer;
                    if (buffer != null)
                        return editorAdapterFactoryService.GetDataBuffer(buffer);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the current IWpfTextView that is the active document.
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public static IWpfTextView GetActiveTextView() {
            var monitorSelection = (IVsMonitorSelection)GetGlobalService(typeof(SVsShellMonitorSelection));
            if (monitorSelection == null) {
                return null;
            }

            object curDocument;
            if (ErrorHandler.Failed(monitorSelection.GetCurrentElementValue((uint)VSConstants.VSSELELEMID.SEID_DocumentFrame, out curDocument))) {
                // TODO: Report error
                return null;
            }
            var frame = curDocument as IVsWindowFrame;
            if (frame == null) {
                // TODO: Report error
                return null;
            }

            return GetWpfTextViewFromFrame(frame);
        }

        [CanBeNull]
        static IWpfTextView GetWpfTextViewFromFrame(IVsWindowFrame frame) {
           
            object docView;
            if (ErrorHandler.Failed(frame.GetProperty((int) __VSFPROPID.VSFPROPID_DocView, out docView))) {
                // TODO: Report error
                return null;
            }

            if (docView is IVsCodeWindow) {
                IVsTextView textView;
                if (ErrorHandler.Failed(((IVsCodeWindow) docView).GetPrimaryView(out textView))) {
                    // TODO: Report error
                    return null;
                }

                var model = (IComponentModel) Package.GetGlobalService(typeof(SComponentModel));
                var adapterFactory = model.GetService<IVsEditorAdaptersFactoryService>();
                var wpfTextView = adapterFactory.GetWpfTextView(textView);
                return wpfTextView;
            }

            return null;
        }

        public static _DTE DTE {
            get {
                _DTE dte = GetGlobalService<_DTE, _DTE>();
                return dte;
            }
        }

        public static BitmapSource GetImage(ImageMoniker moniker) {

            var imageService = GetGlobalService<SVsImageService, IVsImageService2>();

            ImageAttributes imageAttributes = new ImageAttributes {
                StructSize    = Marshal.SizeOf(typeof(ImageAttributes)),
                Flags         = (uint) _ImageAttributesFlags.IAF_RequiredFlags,
                ImageType     = (uint) _UIImageType.IT_Bitmap,
                Format        = (uint) _UIDataFormat.DF_WPF,
                LogicalHeight = 16,
                LogicalWidth  = 16
            };

            IVsUIObject result = imageService?.GetImage(moniker, imageAttributes);

            object data =null;
            result?.get_Data(out data);

            return data as BitmapSource;
        }
    }
}
