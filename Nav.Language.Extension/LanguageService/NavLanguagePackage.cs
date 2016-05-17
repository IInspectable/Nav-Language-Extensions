#region Using Directives

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using EnvDTE;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

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
    public sealed partial class NavLanguagePackage : Package {

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


        public static void OpenFile(string file) {
            DTE?.ExecuteCommand("File.OpenFile", Quote(file));
        }

        public static void GoToLocationInPreviewTab(Location location) {
            if(location.FilePath != null) {
                OpenFileInPreviewTab(location.FilePath);
            }

            var selection = DTE?.ActiveDocument.Selection as TextSelection;
            selection?.MoveToLineAndOffset(Line: location.StartLine + 1, Offset: location.StartCharacter + 1);
            selection?.MoveToLineAndOffset(Line: location.EndLine   + 1, Offset: location.EndCharacter   + 1, Extend: true);
        }

        static string Quote(string file) {
            return '"' + file?.Trim('"') + '"';
        }

        public static void OpenFileInPreviewTab(string file) {
            IVsNewDocumentStateContext newDocumentStateContext = null;

            try {
                var openDoc3 = GetGlobalService<SVsUIShellOpenDocument, IVsUIShellOpenDocument3>();

                Guid reason = VSConstants.NewDocumentStateReason.Navigation;
                newDocumentStateContext = openDoc3?.SetNewDocumentState((uint)__VSNEWDOCUMENTSTATE.NDS_Provisional, ref reason);

                OpenFile(file);

            } finally {
                newDocumentStateContext?.Restore();
            }
        }

        /// <summary>
        /// Gets the current IWpfTextView that is the active document.
        /// </summary>
        /// <returns></returns>
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

            object docView;
            if (ErrorHandler.Failed(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out docView))) {
                // TODO: Report error
                return null;
            }

            if (docView is IVsCodeWindow) {
                IVsTextView textView;
                if (ErrorHandler.Failed(((IVsCodeWindow)docView).GetPrimaryView(out textView))) {
                    // TODO: Report error
                    return null;
                }

                var model = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
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
