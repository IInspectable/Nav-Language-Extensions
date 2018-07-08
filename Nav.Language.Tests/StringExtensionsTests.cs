using System.Collections.Generic;

using NUnit.Framework;

using Pharmatechnik.Nav.Language.Text;

namespace Nav.Language.Tests {

    [TestFixture]
    public class StringExtensionsTests {

        public static IEnumerable<TestCase> GetTestCases() {
            yield return new TestCase("FooBarBaz", "Foo", "Bar", "Baz");
            yield return new TestCase("fooBarBaz", "foo", "Bar", "Baz");
            yield return new TestCase("FBZ", "F", "B", "Z");
            yield return new TestCase("fBZ", "f", "B", "Z");
            yield return new TestCase("fbZ", "fb",  "Z");
            yield return new TestCase("fbz", "fbz");
            yield return new TestCase("");
        }

        [Test, TestCaseSource(nameof(GetTestCases))]
        public void TestCamelHumps(TestCase testCase) {

            var result = testCase.Input.CamelHumpSplit();

            Assert.That(result, Is.EqualTo(testCase.Parts));
        }

        public class TestCase {

            public string   Input { get; }
            public string[] Parts { get; }

            public TestCase(string input, params string[] parts) {
                Input = input;
                Parts = parts;

            }

            public override string ToString() {
                return Input;
            }

        }

    }

}