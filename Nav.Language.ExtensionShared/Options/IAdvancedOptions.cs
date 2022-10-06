namespace Pharmatechnik.Nav.Language.Extension.Options; 

public interface IAdvancedOptions {
    bool SemanticHighlighting            { get; }
    bool HighlightReferencesUnderCursor  { get;  }
    bool HighlightReferencesUnderInclude { get;  }
    bool AutoInsertDelimiters            {get;}
}