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
    }
}