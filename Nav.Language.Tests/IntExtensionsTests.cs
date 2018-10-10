using System;
using System.Collections.Generic;

using NUnit.Framework;

using Pharmatechnik.Nav.Language;

namespace Nav.Language.Tests {

    [TestFixture]
    public class IntExtensionsTests {

        public class TestCase {

            public int Value    { get; set; }
            public int Start    { get; set; }
            public int End      { get; set; }
            public int Expected { get; set; }

            public override string ToString() {
                return $"[{Start}, {End}] Value: {Value}, Expected: {Expected}";
            }

        }

        [Test, TestCaseSource(nameof(GetTestCases))]
        public void TestRange(TestCase testCase) {

            Assert.That(testCase.Value.Trim(testCase.Start, testCase.End), Is.EqualTo(testCase.Expected));
        }

        [Test]
        public void TestOutOfRange() {

            Assert.Throws<ArgumentOutOfRangeException>(() => 1.Trim(4, 1));
        }

        public static IEnumerable<TestCase> GetTestCases() {

            yield return new TestCase {Start = 5, Value = 6, End  = 10, Expected = 6};
            yield return new TestCase {Start = 5, Value = 1, End  = 10, Expected = 5};
            yield return new TestCase {Start = 5, Value = 11, End = 10, Expected = 10};

            yield return new TestCase {Start = 5, Value = 5, End = 5, Expected = 5};
            yield return new TestCase {Start = 5, Value = 4, End = 5, Expected = 5};
            yield return new TestCase {Start = 5, Value = 6, End = 5, Expected = 5};
        }

    }

}