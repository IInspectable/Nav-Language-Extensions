using System;

namespace Pharmatechnik.Nav.Language {

    public class UnitTestDiagnosticFormatter: DiagnosticFormatter {

        UnitTestDiagnosticFormatter():
            base(displayEndLocations: true, workingDirectory: null) {
        }

        public new static readonly DiagnosticFormatter Instance = new UnitTestDiagnosticFormatter();

        public String LinePrefix => "//==>> ";

        public override string Format(Diagnostic diagnostic, IFormatProvider formatter = null) {
            return LinePrefix + base.Format(diagnostic, formatter);
        }

        protected override string FormatFilePath(Diagnostic diagnostic, IFormatProvider formatter) {
            return String.Empty;
        }

    }

}