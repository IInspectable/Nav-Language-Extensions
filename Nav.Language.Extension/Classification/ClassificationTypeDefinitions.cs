﻿#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Nav.Language.Text;

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

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.Keyword)] [BaseDefinition(PredefinedClassificationTypeNames.Keyword)]
        public static ClassificationTypeDefinition Keyword;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.Keyword)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        public sealed class KeywordClassificationFormatDefinition: ClassificationFormatDefinition {

            public KeywordClassificationFormatDefinition() {
                DisplayName = "Nav Keyword";
            }

        }

        #endregion

        #region Comment

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.Comment)] [BaseDefinition(PredefinedClassificationTypeNames.Comment)]
        public static ClassificationTypeDefinition Comment;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.Comment)]
        [UserVisible(Is.UserVisible)] // This should be visible to the end user
        [Order(Before = Priority.Default)]
        // Set the priority to be after the default classifiers
        public sealed class CommentClassificationFormatDefinition: ClassificationFormatDefinition {

            public CommentClassificationFormatDefinition() {
                DisplayName = "Nav Comment"; // Human readable version of the name
            }

        }

        #endregion

        #region Identifier

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.Identifier)] [BaseDefinition(PredefinedClassificationTypeNames.Identifier)]
        public static ClassificationTypeDefinition Identifier;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.Identifier)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        public sealed class IdentifierClassificationFormatDefinition: ClassificationFormatDefinition {

            public IdentifierClassificationFormatDefinition() {
                DisplayName = "Nav Identifier";
            }

        }

        #endregion

        #region String

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.StringLiteral)] [BaseDefinition(PredefinedClassificationTypeNames.String)]
        public static ClassificationTypeDefinition String;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.StringLiteral)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        public sealed class StringClassificationFormatDefinition: ClassificationFormatDefinition {

            public StringClassificationFormatDefinition() {
                DisplayName = "Nav String";
            }

        }

        #endregion

        #region FormName

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.FormName)] [BaseDefinition("class name")]
        public static ClassificationTypeDefinition Type;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.FormName)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        public sealed class FormNameClassificationFormatDefinition: ClassificationFormatDefinition {

            public FormNameClassificationFormatDefinition() {
                DisplayName = "Nav Form Name";
            }

        }

        #endregion

        #region TaskName

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.TaskName)] [BaseDefinition("class name")]
        public static ClassificationTypeDefinition TaskName;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.TaskName)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        public sealed class TaskNameClassificationFormatDefinition: ClassificationFormatDefinition {

            public TaskNameClassificationFormatDefinition() {
                DisplayName = "Nav Task Name";
            }

        }

        #endregion

        #region TypeName

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.TypeName)] [BaseDefinition("class name")]
        public static ClassificationTypeDefinition TypeName;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.TypeName)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        public sealed class TypeNameClassificationFormatDefinition: ClassificationFormatDefinition {

            public TypeNameClassificationFormatDefinition() {
                DisplayName = "Nav Type Name";
            }

        }

        #endregion

        #region Punctuation

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.Punctuation)] [BaseDefinition("Punctuation")]
        public static ClassificationTypeDefinition Punctuation;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.Punctuation)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        public sealed class PunctuationClassificationFormatDefinition: ClassificationFormatDefinition {

            public PunctuationClassificationFormatDefinition() {
                DisplayName = "Nav Punctuation";
            }

        }

        #endregion

        #region Unknown

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.Unknown)] [BaseDefinition("Syntax Error")]
        public static ClassificationTypeDefinition Unknown;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.Unknown)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        public sealed class UnknownClassificationFormatDefinition: ClassificationFormatDefinition {

            public UnknownClassificationFormatDefinition() {
                DisplayName = "Nav Unknown";
            }

        }

        #endregion

        #region DeadCode

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.DeadCode)] [BaseDefinition("formal language")]
        public static ClassificationTypeDefinition DeadCode;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.DeadCode)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.High)]
        public sealed class DeadCodeClassificationFormatDefinition: ClassificationFormatDefinition {

            public DeadCodeClassificationFormatDefinition() {
                DisplayName       = "Nav Dead Code";
                ForegroundOpacity = 0.5;
            }

        }

        #endregion

        #region ChoiceNode

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.ChoiceNode)] [BaseDefinition(ClassificationTypeNames.Identifier)]
        public static ClassificationTypeDefinition ChoiceNode;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.ChoiceNode)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Low)]
        public sealed class ChoiceNodeClassificationFormatDefinition: ClassificationFormatDefinition {

            public ChoiceNodeClassificationFormatDefinition() {
                IsItalic = true;
            }

        }

        #endregion

        #region ConnectionPoint

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.ConnectionPoint)] [BaseDefinition(ClassificationTypeNames.Identifier)]
        public static ClassificationTypeDefinition ConnectionPoint;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.ConnectionPoint)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Low)]
        public sealed class ConnectionPointClassificationFormatDefinition: ClassificationFormatDefinition {

            public ConnectionPointClassificationFormatDefinition() {
                // IsItalic = true;
                IsBold = true;
            }

        }

        #endregion

        #region Underline

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.Underline)] [BaseDefinition("formal language")]
        public static ClassificationTypeDefinition Underline;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.Underline)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        public sealed class UnderlineClassificationFormatDefinition: ClassificationFormatDefinition {

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

        #region PreprocessorKeyword

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.PreprocessorKeyword)] [BaseDefinition("preprocessor keyword")]
        public static ClassificationTypeDefinition PreprocessorKeyword;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.PreprocessorKeyword)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        public sealed class PreprocessorKeywordClassificationFormatDefinition: ClassificationFormatDefinition {

            public PreprocessorKeywordClassificationFormatDefinition() {
                DisplayName = "Nav Preprocessor Keyword";
            }

        }

        #endregion

        #region PreprocessorKeyword

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.PreprocessorText)] [BaseDefinition("preprocessor text")]
        public static ClassificationTypeDefinition PreprocessorText;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.PreprocessorText)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        public sealed class PreprocessorTextClassificationFormatDefinition: ClassificationFormatDefinition {

            public PreprocessorTextClassificationFormatDefinition() {
                DisplayName = "Nav Preprocessor Text";
            }

        }

        #endregion

        public static ImmutableDictionary<TextClassification, IClassificationType> GetSyntaxTokenClassificationMap(IClassificationTypeRegistryService registry) {

            var classificationMap = new Dictionary<TextClassification, IClassificationType> {
                {TextClassification.Skiped             , registry.GetClassificationType(ClassificationTypeNames.Unknown)},
                {TextClassification.Unknown            , registry.GetClassificationType(ClassificationTypeNames.Unknown)},
                {TextClassification.Comment            , registry.GetClassificationType(ClassificationTypeNames.Comment)},
                {TextClassification.Keyword            , registry.GetClassificationType(ClassificationTypeNames.Keyword)},
                {TextClassification.Identifier         , registry.GetClassificationType(ClassificationTypeNames.Identifier)},
                {TextClassification.Punctuation        , registry.GetClassificationType(ClassificationTypeNames.Punctuation)},
                {TextClassification.StringLiteral      , registry.GetClassificationType(ClassificationTypeNames.StringLiteral)},
                {TextClassification.FormName           , registry.GetClassificationType(ClassificationTypeNames.FormName)},
                {TextClassification.TypeName           , registry.GetClassificationType(ClassificationTypeNames.TypeName)},
                {TextClassification.TaskName           , registry.GetClassificationType(ClassificationTypeNames.TaskName)},
                {TextClassification.PreprocessorKeyword, registry.GetClassificationType(ClassificationTypeNames.PreprocessorKeyword)},
                {TextClassification.PreprocessorText   , registry.GetClassificationType(ClassificationTypeNames.PreprocessorText)},
            };

            return classificationMap.ToImmutableDictionary();
        }

    }

}