#region Using Directives

using System;
using System.ComponentModel.Composition;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    class ExportCodeFixActionProviderAttribute : ExportAttribute {
    
        public ExportCodeFixActionProviderAttribute(string name): base(typeof(ICodeFixActionProvider)) {
            Name = name;
        }

        public string Name { get; }
    }
}