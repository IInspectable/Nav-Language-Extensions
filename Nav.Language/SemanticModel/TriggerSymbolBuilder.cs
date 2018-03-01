using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language {

    sealed class TriggerSymbolBuilderResult {

        public TriggerSymbolBuilderResult(SymbolCollection<TriggerSymbol> triggers, IReadOnlyList<Diagnostic> diagnostics) {
            Triggers    = triggers    ?? new SymbolCollection<TriggerSymbol>();
            Diagnostics = diagnostics ?? new List<Diagnostic>();
        }

        public SymbolCollection<TriggerSymbol> Triggers    { get; }
        public IReadOnlyList<Diagnostic>       Diagnostics { get; }

    }

    sealed class TriggerSymbolBuilder: SyntaxNodeVisitor {

        readonly List<TriggerSymbol> _triggers;
        readonly List<Diagnostic>    _diagnostics;

        public TriggerSymbolBuilder(List<Diagnostic> diagnostics) {
            _diagnostics = diagnostics ?? new List<Diagnostic>();
            _triggers    = new List<TriggerSymbol>();
        }

        public static TriggerSymbolBuilderResult Build(TransitionDefinitionSyntax transitionDefinitionSyntax) {

            var diagnostics = new List<Diagnostic>();
            var builder     = new TriggerSymbolBuilder(diagnostics);

            var triggers = builder.GetTriggers(transitionDefinitionSyntax);

            return new TriggerSymbolBuilderResult(triggers, diagnostics);
        }

        public SymbolCollection<TriggerSymbol> GetTriggers(TransitionDefinitionSyntax transitionDefinitionSyntax) {

            if (transitionDefinitionSyntax.Trigger != null) {
                Visit(transitionDefinitionSyntax.Trigger);
            }

            var result = new SymbolCollection<TriggerSymbol>();
            foreach (var trigger in _triggers) {
                var existing = result.TryFindSymbol(trigger.Name);
                if (existing != null) {

                    _diagnostics.Add(new Diagnostic(
                                         trigger.Location,
                                         existing.Location,
                                         DiagnosticDescriptors.Semantic.Nav0026TriggerWithName0AlreadyDeclared,
                                         existing.Name));

                } else {
                    result.Add(trigger);
                }
            }

            return result;
        }

        public override void VisitSpontaneousTrigger(SpontaneousTriggerSyntax spontaneousTriggerSyntax) {
            var location = spontaneousTriggerSyntax.GetLocation();
            if (location != null) {
                var trigger = new SpontaneousTriggerSymbol(location, spontaneousTriggerSyntax);
                _triggers.Add(trigger);
            }
        }

        public override void VisitSignalTrigger(SignalTriggerSyntax signalTriggerSyntax) {

            if (signalTriggerSyntax.IdentifierOrStringList == null) {
                return;
            }

            foreach (var signal in signalTriggerSyntax.IdentifierOrStringList) {
                var location = signal.GetLocation();
                if (location != null) {
                    var trigger = new SignalTriggerSymbol(signal.Text, location, signal);
                    _triggers.Add(trigger);
                }
            }
        }

    }

}