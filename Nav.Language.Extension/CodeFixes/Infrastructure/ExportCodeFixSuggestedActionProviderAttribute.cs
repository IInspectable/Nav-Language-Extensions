#region Using Directives

using System;
using System.ComponentModel.Composition;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    class ExportCodeFixSuggestedActionProviderAttribute : ExportAttribute {
    
        public ExportCodeFixSuggestedActionProviderAttribute(string name): base(typeof(ICodeFixSuggestedActionProvider)) {
            Name = name;
        }

        public string Name { get; }
    }
}