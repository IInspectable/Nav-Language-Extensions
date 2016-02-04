using System;
using System.Text;

namespace Nav.Language.Tests {

    static class TestHelper {
        private static readonly Random Random = new Random();

        public static string RandomString(int length) {

            StringBuilder builder = new StringBuilder(length);
            for (int i = 0; i < length; i++) {
                if (i % 20 == 0) {
                    builder.Append(" ");
                }
                else if (i%80 == 0) {
                    builder.Append(Environment.NewLine);
                }
                else {
                    var ch = Convert.ToChar(Random.Next(Char.MaxValue));
                    builder.Append(ch);
                }
            }

            return builder.ToString();
        }
    }
}
