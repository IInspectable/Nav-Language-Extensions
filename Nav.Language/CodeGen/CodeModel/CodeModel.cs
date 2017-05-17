namespace Pharmatechnik.Nav.Language.CodeGen {

    public abstract class CodeModel {
        /// <summary>
        /// IWFService
        /// </summary>
        protected const string DefaultIwfsBaseType = "IWFService";
        /// <summary>
        /// IBeginWFService
        /// </summary>
        protected const string DefaultIBeginWfsBaseType = "IBeginWFService";
        /// <summary>
        /// Logic
        /// </summary>
        protected const string LogicMethodSuffix = "Logic";
        /// <summary>
        /// TO
        /// </summary>
        protected const string ToClassNameSuffix = "TO";
        /// <summary>
        /// WFL
        /// </summary>
        protected const string WflNamespaceSuffix = "WFL";
        /// <summary>
        /// IWFL
        /// </summary>
        protected const string IwflNamespaceSuffix = "IWFL";
        /// <summary>
        /// WFSBase
        /// </summary>
        protected const string WfsBaseClassSuffix = "WFSBase";
        /// <summary>
        /// WFS
        /// </summary>
        protected const string WfsClassSuffix = "WFS";       
        /// <summary>
        /// Begin
        /// </summary>
        protected const string BeginMethodPrefix = "Begin";
        /// <summary>
        /// After
        /// </summary>
        protected const string ExitMethodSuffix = "After";
        /// <summary>
        /// IBegin
        /// </summary>
        protected const string BeginInterfacePrefix = "IBegin";
        /// <summary>
        /// Pharmatechnik.Apotheke.XTplus.Framework.NavigationEngine.IWF
        /// </summary>
        protected const string NavigationEngineIwflNamespace="Pharmatechnik.Apotheke.XTplus.Framework.NavigationEngine.IWFL";
        /// <summary>
        /// Pharmatechnik.Apotheke.XTplus.Framework.NavigationEngine.WFL
        /// </summary>
        protected const string NavigationEngineWflNamespace = "Pharmatechnik.Apotheke.XTplus.Framework.NavigationEngine.WFL";
        /// <summary>
        /// __UNKNOWN__NAMESPACE__
        /// </summary>
        protected const string UnknownNamespace = "__UNKNOWN__NAMESPACE__";
        /// <summary>
        /// par
        /// </summary>
        protected const string DefaultParamterName = "par";
        /// <summary>
        /// bool
        /// </summary>
        protected const string DefaultTaskResultType = "bool";
        /// <summary>
        /// BaseWFService
        /// </summary>
        protected const string DefaultWfsBaseClass = "BaseWFService";
    }
}