#region Using Directives

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Classification {

    // ReSharper disable UnassignedField.Local
    #pragma warning disable 0169
    static class ClassificationTypeDefinitions {

        //======================================
        //      Die Farben sollen derzeit nicht 
        //      anpassbar sein.
        //======================================
        static class Is {
            public const bool UserVisible = false;
        }
  
        #region Keyword

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.Keyword)]
        [BaseDefinition(PredefinedClassificationTypeNames.Keyword)]
        static ClassificationTypeDefinition _keyword;
      
        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.Keyword)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        internal sealed class KeywordClassificationFormatDefinition : ClassificationFormatDefinition {

            public KeywordClassificationFormatDefinition() {
                DisplayName = "Nav Keyword";
            }
        }

        #endregion

        #region Comment

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.Comment)]
        [BaseDefinition(PredefinedClassificationTypeNames.Comment)]
        static ClassificationTypeDefinition _comment;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.Comment)]
        [UserVisible(Is.UserVisible)] // This should be visible to the end user
        [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
        internal sealed class CommentClassificationFormatDefinition : ClassificationFormatDefinition {

            public CommentClassificationFormatDefinition() {
                DisplayName = "Nav Comment"; // Human readable version of the name
            }
        }

        #endregion

        #region Identifier

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.Identifier)]
        [BaseDefinition(PredefinedClassificationTypeNames.Identifier)]
        static ClassificationTypeDefinition _identifier;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.Identifier)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        internal sealed class IdentifierClassificationFormatDefinition : ClassificationFormatDefinition {

            public IdentifierClassificationFormatDefinition() {
                DisplayName = "Nav Identifier";
            }
        }

        #endregion

        #region String

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.StringLiteral)]
        [BaseDefinition(PredefinedClassificationTypeNames.String)]
        static ClassificationTypeDefinition _string;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.StringLiteral)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        internal sealed class StringClassificationFormatDefinition : ClassificationFormatDefinition {

            public StringClassificationFormatDefinition() {
                DisplayName = "Nav String";
            }
        }

        #endregion

        #region FormName

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.FormName)]
        [BaseDefinition("class name")]
        static ClassificationTypeDefinition _type;
      
        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.FormName)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        internal sealed class FormNameClassificationFormatDefinition : ClassificationFormatDefinition {

            public FormNameClassificationFormatDefinition() {
                DisplayName = "Nav Form Name";
            }
        }

        #endregion

        #region TaskName

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.TaskName)]
        [BaseDefinition("class name")]
        static ClassificationTypeDefinition _taskName;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.TaskName)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        internal sealed class TaskNameClassificationFormatDefinition : ClassificationFormatDefinition {

            public TaskNameClassificationFormatDefinition() {
                DisplayName = "Nav Task Name";
            }
        }

        #endregion

        #region TypeName

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.TypeName)]
        [BaseDefinition("class name")]
        static ClassificationTypeDefinition _typeName;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.TypeName)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        internal sealed class TypeNameClassificationFormatDefinition : ClassificationFormatDefinition {

            public TypeNameClassificationFormatDefinition() {
                DisplayName = "Nav Type Name";
            }
        }

        #endregion

        #region Punctuation

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.Punctuation)]
        [BaseDefinition("Punctuation")]
        static ClassificationTypeDefinition _punctuation;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.Punctuation)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        internal sealed class PunctuationClassificationFormatDefinition : ClassificationFormatDefinition {

            public PunctuationClassificationFormatDefinition() {
                DisplayName = "Nav Punctuation";
            }
        }

        #endregion

        #region Unknown

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.Unknown)]
        [BaseDefinition("Syntax Error")]
        static ClassificationTypeDefinition _unknown;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.Unknown)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        internal sealed class UnknownClassificationFormatDefinition : ClassificationFormatDefinition {

            public UnknownClassificationFormatDefinition() {
                DisplayName = "Nav Unknown";
            }
        }

        #endregion

        #region DeadCode

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.DeadCode)]
        [BaseDefinition("formal language")]
        static ClassificationTypeDefinition _deadCode;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.DeadCode)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        internal sealed class DeadCodeClassificationFormatDefinition : ClassificationFormatDefinition {

            public DeadCodeClassificationFormatDefinition() {
                DisplayName       = "Nav Dead Code";
                ForegroundOpacity = 0.5;               
            }
        }

        #endregion

        #region Underline

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.Underline)]
        [BaseDefinition("formal language")]
        static ClassificationTypeDefinition _underline;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.Underline)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        internal sealed class UnderlineClassificationFormatDefinition : ClassificationFormatDefinition {

            public UnderlineClassificationFormatDefinition() {
                DisplayName = "Nav Underline";

                var underline = new System.Windows.TextDecoration {
                    PenThicknessUnit = System.Windows.TextDecorationUnit.FontRecommended
                };
                if (TextDecorations == null) {
                    TextDecorations = new System.Windows.TextDecorationCollection();
                }
                TextDecorations.Add(underline);
            }
        }

        #endregion

        public static Dictionary<SyntaxTokenClassification, IClassificationType> GetSyntaxTokenClassificationMap(IClassificationTypeRegistryService registry) {

            var classificationMap = new Dictionary<SyntaxTokenClassification, IClassificationType> {
                {SyntaxTokenClassification.Skiped        , registry.GetClassificationType(ClassificationTypeNames.Unknown)},
                {SyntaxTokenClassification.Unknown       , registry.GetClassificationType(ClassificationTypeNames.Unknown)},
                {SyntaxTokenClassification.Comment       , registry.GetClassificationType(ClassificationTypeNames.Comment)},
                {SyntaxTokenClassification.Keyword       , registry.GetClassificationType(ClassificationTypeNames.Keyword)},
                {SyntaxTokenClassification.Identifier    , registry.GetClassificationType(ClassificationTypeNames.Identifier)},
                {SyntaxTokenClassification.Punctuation   , registry.GetClassificationType(ClassificationTypeNames.Punctuation)},
                {SyntaxTokenClassification.StringLiteral , registry.GetClassificationType(ClassificationTypeNames.StringLiteral)},
                {SyntaxTokenClassification.FormName      , registry.GetClassificationType(ClassificationTypeNames.FormName)},
                {SyntaxTokenClassification.TypeName      , registry.GetClassificationType(ClassificationTypeNames.TypeName)},
                {SyntaxTokenClassification.TaskName      , registry.GetClassificationType(ClassificationTypeNames.TaskName)},
            };

            return classificationMap;
        }
    }
}
