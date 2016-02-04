using Microsoft.CSharp;

namespace Pharmatechnik.Nav.Language {
    static class CSharp {

        static readonly CSharpCodeProvider CodeProvider =new CSharpCodeProvider();

        public static bool IsValidIdentifier(string value) {
            return CodeProvider.IsValidIdentifier(value);
        }
    }
}