using System.Collections.Generic;
using System.Threading;

using Pharmatechnik.Nav.Language;

namespace Nav.Language.Tests {

    public class TestCaseFile {

        public string Content  { get; set; }
        public string FilePath { get; set; }

    }

    class TestSyntaxProvider: SyntaxProvider {

        readonly Dictionary<string, string> _files;

        public TestSyntaxProvider() {
            _files = new Dictionary<string, string>();
        }

        public void RegisterFile(TestCaseFile file) {
            _files[file.FilePath] = file.Content;
        }

        public override CodeGenerationUnitSyntax FromFile(string filePath, CancellationToken cancellationToken = default(CancellationToken)) {

            if (!_files.TryGetValue(filePath, out var content)) {
                return null;
            }

            return Syntax.ParseCodeGenerationUnit(text: content, filePath: filePath);
        }

    }

}