using System;
using System.Collections.Generic;
using System.Linq;
using Expedition0.Tasks;
using NUnit.Framework;

namespace Expedition0.Test
{
    public class TritLogicTest
    {
        private static IEnumerable<Trit> AllTrits => Enum.GetValues(typeof(Trit)).Cast<Trit>();

        private static readonly (Trit a, Trit expected)[] TestCasesNot =
        {
            (Trit.False, Trit.True),
            (Trit.Neutral, Trit.Neutral),
            (Trit.True, Trit.False)
        };

        private static readonly (Trit a, Trit b, Trit expected)[] TestCasesAnd =
        {
            (Trit.False, Trit.False, Trit.False),
            (Trit.False, Trit.Neutral, Trit.False),
            (Trit.False, Trit.True, Trit.False),
            (Trit.Neutral, Trit.False, Trit.False),
            (Trit.Neutral, Trit.Neutral, Trit.Neutral),
            (Trit.Neutral, Trit.True, Trit.Neutral),
            (Trit.True, Trit.False, Trit.False),
            (Trit.True, Trit.Neutral, Trit.Neutral),
            (Trit.True, Trit.True, Trit.True)
        };
        
        private static readonly (Trit a, Trit b, Trit expected)[] TestCasesOr =
        {
            (Trit.False, Trit.False, Trit.False),
            (Trit.False, Trit.Neutral, Trit.Neutral),
            (Trit.False, Trit.True, Trit.True),
            (Trit.Neutral, Trit.False, Trit.Neutral),
            (Trit.Neutral, Trit.Neutral, Trit.Neutral),
            (Trit.Neutral, Trit.True, Trit.True),
            (Trit.True, Trit.False, Trit.True),
            (Trit.True, Trit.Neutral, Trit.True),
            (Trit.True, Trit.True, Trit.True)
        };
        
        private static readonly (Trit a, Trit b, Trit expected)[] TestCasesXor =
        {
            (Trit.False, Trit.False, Trit.False),
            (Trit.False, Trit.Neutral, Trit.Neutral),
            (Trit.False, Trit.True, Trit.True),
            (Trit.Neutral, Trit.False, Trit.Neutral),
            (Trit.Neutral, Trit.Neutral, Trit.Neutral),
            (Trit.Neutral, Trit.True, Trit.Neutral),
            (Trit.True, Trit.False, Trit.True),
            (Trit.True, Trit.Neutral, Trit.Neutral),
            (Trit.True, Trit.True, Trit.False)
        };
        
        [TestCaseSource(nameof(TestCasesNot))]
        [Test]
        public void TestNot((Trit a, Trit expected) testCase)
        {
            Assert.AreEqual(testCase.a.Not(), testCase.expected);
        }

        [TestCaseSource(nameof(TestCasesAnd))]
        [Test]
        public void TestAnd((Trit a, Trit b, Trit expected) testCase)
        {
            Assert.AreEqual(testCase.a.And(testCase.b), testCase.expected);
        }

        [TestCaseSource(nameof(TestCasesOr))]
        [Test]
        public void TestOr((Trit a, Trit b, Trit expected) testCase)
        {
            Assert.AreEqual(testCase.a.Or(testCase.b), testCase.expected);
        }

        [TestCaseSource(nameof(TestCasesXor))]
        [Test]
        public void TestXor((Trit a, Trit b, Trit expected) testCase)
        {
            Assert.AreEqual(testCase.a.Xor(testCase.b), testCase.expected);
        }

        [TestCaseSource(nameof(TestCasesOr))]
        [Test]
        public void TestImplyKleene((Trit a, Trit b, Trit expected) testCase)
        {
            Assert.AreEqual(testCase.a.ImplyKleene(testCase.b), testCase.a.Not().Or(testCase.b));
            Assert.AreEqual(testCase.a.Not().ImplyKleene(testCase.b), testCase.a.Or(testCase.b));
        }
        
        [TestCaseSource(nameof(TestCasesOr))]
        public void TestImplyLukasiewicz((Trit a, Trit b, Trit expected) testCase)
        {
            if (testCase.a == Trit.Neutral && testCase.b == Trit.Neutral)
            {
                Assert.AreEqual(testCase.a.ImplyLukasiewicz(testCase.b), Trit.True);
            }
            else
            {
                Assert.AreEqual(testCase.a.ImplyKleene(testCase.b), testCase.a.Not().Or(testCase.b));
                Assert.AreEqual(testCase.a.Not().ImplyKleene(testCase.b), testCase.a.Or(testCase.b));
            }
        }
    }
}