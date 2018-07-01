#region Using Directives

using System;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell;

using Pharmatechnik.Nav.Language.CodeFixes;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Images {

    public static class ImageMonikers {

        public static ImageMoniker ProjectNode => KnownMonikers.CSProjectNode;

        static readonly Guid CustomMonikerGuid = new Guid("{11e9628b-b9e6-45d6-ae8d-b4440be46fa6}");

        #region CodeFixImpact

        public static ImageMoniker FromCodeFixImpact(CodeFixImpact impact) {
            switch (impact) {
                case CodeFixImpact.None:
                    return default;
                case CodeFixImpact.Medium:
                    return KnownMonikers.StatusWarningOutline;
                case CodeFixImpact.High:
                    return KnownMonikers.StatusInvalidOutline;
                default:
                    return default;
            }
        }

        #endregion

        #region Analysis

        public static ImageMoniker WaitingForAnalysis => KnownMonikers.Loading;
        public static ImageMoniker AnalysisOK         => KnownMonikers.StatusOK;
        public static ImageMoniker AnalysisWarning    => KnownMonikers.StatusWarning;
        public static ImageMoniker AnalysisError      => KnownMonikers.StatusError;

        #endregion

        #region GoTo

        /// <summary>
        /// Nav file --> C# file
        /// </summary>
        public static ImageMoniker GoToDeclaration => KnownMonikers.GoToDefinition;

        /// <summary>
        /// C# file --> Nav file
        /// </summary>
        public static ImageMoniker GoToDefinition => KnownMonikers.GoToDeclaration;

        public static ImageMoniker Include             => KnownMonikers.ClassFile;
        public static ImageMoniker GoToNodeDeclaration => KnownMonikers.GoToReference;
        public static ImageMoniker GoToMethodPublic    => KnownMonikers.MethodPublic;
        public static ImageMoniker GoToClassPublic     => KnownMonikers.ClassPublic;
        public static ImageMoniker GoToInterfacePublic => KnownMonikers.InterfacePublic;
        public static ImageMoniker CSharpFile          => KnownMonikers.CSFileNode;

        #endregion

        #region Symbols

        static ImageMoniker? _taskDeclarationImageMoniker;

        public static ImageMoniker TaskDeclaration {
            get {
                #pragma warning disable VSTHRD010
                if (_taskDeclarationImageMoniker.HasValue) {
                    return _taskDeclarationImageMoniker.Value;
                }

                if (ThreadHelper.CheckAccess()) {
                    // Darf nur im GUI Thread erstellt werden
                    _taskDeclarationImageMoniker = CreateTaskDeclarationImageHandle();
                }

                // Natürlich kann es erste Aufrufer geben, die nicht vom GUI Thrad kommen
                // - dann gibt es keinen Moniker. So what, das Problem heilt sich selbst,
                // sobals der erste GUI Call kommt.
                // Bisweilen kein praxisrelevantes Problem..

                return _taskDeclarationImageMoniker.GetValueOrDefault();

                ImageMoniker CreateTaskDeclarationImageHandle() {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    return GetCompositedImageHandle(
                        CreateLayer(TaskDefinition),
                        CreateLayer(KnownMonikers.ReferencedElement)).Moniker;
                }

                #pragma warning restore VSTHRD010
            }
        }

        public static ImageMoniker InitConnectionPoint => KnownMonikers.InputPin;
        public static ImageMoniker ExitConnectionPoint => KnownMonikers.OutputPin;
        public static ImageMoniker EndConnectionPoint  => KnownMonikers.ActivityFinalNode;
        public static ImageMoniker TaskDefinition      => KnownMonikers.ActivityDiagram;
        public static ImageMoniker InitNode            => KnownMonikers.InputPin;
        public static ImageMoniker ExitNode            => KnownMonikers.OutputPin;
        public static ImageMoniker EndNode             => KnownMonikers.ActivityFinalNode;
        public static ImageMoniker TaskNode            => KnownMonikers.ActivityDiagram;
        public static ImageMoniker ChoiceNode          => KnownMonikers.DecisionNode;
        public static ImageMoniker ViewNode            => KnownMonikers.WindowsForm;
        public static ImageMoniker DialogNode          => KnownMonikers.Dialog;
        public static ImageMoniker SignalTrigger       => KnownMonikers.EventTrigger;
        public static ImageMoniker Edge                => KnownMonikers.AssociationRelationship;
        public static ImageMoniker ModalEdge           => new ImageMoniker {Guid = CustomMonikerGuid, Id = 1};
        public static ImageMoniker NonModalEdge        => new ImageMoniker {Guid = CustomMonikerGuid, Id = 2};
        public static ImageMoniker GoToEdge            => new ImageMoniker {Guid = CustomMonikerGuid, Id = 3};

        public static ImageMoniker FromSymbol(ISymbol symbol) {
            return SymbolImageMonikerFinder.FindImageMoniker(symbol);
        }

        sealed class SymbolImageMonikerFinder: SymbolVisitor<ImageMoniker> {

            public static ImageMoniker FindImageMoniker(ISymbol symbol) {
                var finder = new SymbolImageMonikerFinder();
                return finder.Visit(symbol);
            }

            #pragma warning disable VSTHRD010
            public override ImageMoniker VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {
                return TaskDeclaration;
            }
            #pragma warning restore VSTHRD010

            public override ImageMoniker VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {
                return TaskDefinition;
            }

            public override ImageMoniker VisitIncludeSymbol(IIncludeSymbol includeSymbol) {
                return Include;
            }

            public override ImageMoniker VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {
                return SignalTrigger;
            }

            public override ImageMoniker VisitEdgeModeSymbol(IEdgeModeSymbol edgeModeSymbol) {
                switch (edgeModeSymbol.EdgeMode) {

                    case EdgeMode.Modal:
                        return ModalEdge;
                    case EdgeMode.NonModal:
                        return NonModalEdge;
                    case EdgeMode.Goto:
                        return GoToEdge;
                    default:
                        return Edge;
                }
            }

            #region ConnectionPoints

            public override ImageMoniker VisitConnectionPointReferenceSymbol(IConnectionPointReferenceSymbol connectionPointReferenceSymbol) {
                if (connectionPointReferenceSymbol.Declaration == null) {
                    return DefaultVisit(connectionPointReferenceSymbol);
                }

                return Visit(connectionPointReferenceSymbol.Declaration);
            }

            public override ImageMoniker VisitInitConnectionPointSymbol(IInitConnectionPointSymbol initConnectionPointSymbol) {
                return InitConnectionPoint;
            }

            public override ImageMoniker VisitExitConnectionPointSymbol(IExitConnectionPointSymbol exitConnectionPointSymbol) {
                return ExitConnectionPoint;
            }

            public override ImageMoniker VisitEndConnectionPointSymbol(IEndConnectionPointSymbol endConnectionPointSymbol) {
                return EndConnectionPoint;
            }

            #endregion

            #region Nodes

            public override ImageMoniker VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
                if (nodeReferenceSymbol.Declaration == null) {
                    return DefaultVisit(nodeReferenceSymbol);
                }

                return Visit(nodeReferenceSymbol.Declaration);
            }

            public override ImageMoniker VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {
                return InitNode;
            }

            public override ImageMoniker VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
                return Visit(initNodeAliasSymbol.InitNode);
            }

            public override ImageMoniker VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {
                return ExitNode;
            }

            public override ImageMoniker VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {
                return EndNode;
            }

            public override ImageMoniker VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {
                return TaskNode;
            }

            public override ImageMoniker VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAlias) {
                return Visit(taskNodeAlias.TaskNode);
            }

            public override ImageMoniker VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {
                return ChoiceNode;
            }

            public override ImageMoniker VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {
                return ViewNode;
            }

            public override ImageMoniker VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {
                return DialogNode;
            }

            #endregion

        }

        #endregion

        #region Actions

        public static ImageMoniker AddEdge              => KnownMonikers.AddAssociation;
        public static ImageMoniker RenameNode           => KnownMonikers.Rename;
        public static ImageMoniker InsertNode           => KnownMonikers.InsertClause;
        public static ImageMoniker DeleteQuotationMarks => KnownMonikers.PendingDeleteNode;
        public static ImageMoniker RemoveUnusedSymbol   => KnownMonikers.PendingDeleteNode;
        public static ImageMoniker AddSemicolon         => KnownMonikers.PendingAddNode;

        #endregion

        #region Helper

        static ImageCompositionLayer CreateLayer(
            ImageMoniker imageMoniker,
            int virtualWidth = 16,
            int virtualYOffset = 0,
            int virtualXOffset = 0) {

            return new ImageCompositionLayer {
                VirtualWidth        = virtualWidth,
                VirtualHeight       = 16,
                ImageMoniker        = imageMoniker,
                HorizontalAlignment = (uint) _UIImageHorizontalAlignment.IHA_Left,
                VerticalAlignment   = (uint) _UIImageVerticalAlignment.IVA_Top,
                VirtualXOffset      = virtualXOffset,
                VirtualYOffset      = virtualYOffset,
            };
        }

        static IImageHandle GetCompositedImageHandle(params ImageCompositionLayer[] layers) {

            ThreadHelper.ThrowIfNotOnUIThread();

            var imageService = NavLanguagePackage.GetGlobalService<SVsImageService, IVsImageService2>();

            var imageHandle = imageService.AddCustomCompositeImage(
                virtualWidth: 16,
                virtualHeight: 16,
                layerCount: layers.Length,
                layers: layers);

            return imageHandle;
        }

        #endregion

    }

}