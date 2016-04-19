using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Esri.Services.WebAuthnz.DataStructures;
using Esri.Services.WebAuthnz.Logic;

namespace Esri.Services.WebAuthnz.Test
{
    [TestClass]
    public class LogicTests
    {
        public static Dictionary<string, string[]> Person
        {
            get
            {
                Dictionary<string, string[]> properties = new Dictionary<string, string[]>();
                properties.Add("employer", new string[] { "Esri" });

                return properties;
            }
        }

        public static ReadOnlyDictionary<string, string[]> ReadOnlyPerson
        {
            get
            {
                return new ReadOnlyDictionary<string, string[]>(Person);
            }
        }

        public static Proposition Match
        {
            get
            {
                return new Proposition()
                {
                    Name = "employer",
                    Value = "Esri"
                };
            }
        }

        public static Proposition Mismatch
        {
            get
            {
                return new Proposition()
                {
                    Name = "employer",
                    Value = "Boundless"
                };
            }
        }

        public static Proposition Missing
        {
            get
            {
                return new Proposition()
                {
                    Name = "asdfjkl;",
                    Value = "asdfjkl;"
                };
            }
        }

        public static Proposition MatchNegate
        {
            get
            {
                return new Proposition()
                {
                    Name = "employer",
                    Value = "Esri",
                    Negate = true
                };
            }
        }

        public static Proposition MismatchNegate
        {
            get
            {
                return new Proposition()
                {
                    Name = "employer",
                    Value = "Boundless",
                    Negate = true
                };
            }
        }

        public static Proposition MissingNegate
        {
            get
            {
                return new Proposition()
                {
                    Name = "asdfjkl;",
                    Value = "asdfjkl;",
                    Negate = true
                };
            }
        }

        #region Proposition Tests

        [TestMethod]
        public void TestPropositionMatch()
        {
            Assert.IsTrue(Match.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestPropositionMismatch()
        {
            Assert.IsFalse(Mismatch.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestPropositionMissing()
        {
            Assert.IsFalse(Missing.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestPropositionMatchNegate()
        {
            Assert.IsFalse(MatchNegate.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestPropositionMismatchNegate()
        {
            Assert.IsTrue(MismatchNegate.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestPropositionMissingNegate()
        {
            Assert.IsFalse(MissingNegate.Evaluate(ReadOnlyPerson));
        }

        #endregion

        #region Conjunction Tests

        [TestMethod]
        public void TestConjunctionMissingMissing()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    Missing,
                    Missing
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMissingMissingNegate()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    Missing,
                    MissingNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMissingMatchNegate()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    Missing,
                    MatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMissingMismatchNegate()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    Missing,
                    MismatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMatchMissing()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    Match,
                    Missing
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMatchMatch()
        {
            Assert.IsTrue(new Conjunction()
            {
                Propositions = new Proposition[] {
                    Match,
                    Match
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMatchMissingNegate()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    Match,
                    MissingNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMatchMatchNegate()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    Match,
                    MatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMatchMismatchNegate()
        {
            Assert.IsTrue(new Conjunction()
            {
                Propositions = new Proposition[] {
                    Match,
                    MismatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMismatchMissing()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    Mismatch,
                    Missing
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMismatchMatch()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    Mismatch,
                    Match
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMismatchMismatch()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    Mismatch,
                    Mismatch
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMismatchMissingNegate()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    Mismatch,
                    MissingNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMismatchMatchNegate()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    Mismatch,
                    MatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMismatchMismatchNegate()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    Mismatch,
                    MismatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMissingNegateMissingNegate()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    MissingNegate,
                    MissingNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMatchNegateMissingNegate()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    MatchNegate,
                    MissingNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMatchNegateMatchNegate()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    MatchNegate,
                    MatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMismatchNegateMissingNegate()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    MismatchNegate,
                    MissingNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMismatchNegateMatchNegate()
        {
            Assert.IsFalse(new Conjunction()
            {
                Propositions = new Proposition[] {
                    MismatchNegate,
                    MatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestConjunctionMismatchNegateMismatchNegate()
        {
            Assert.IsTrue(new Conjunction()
            {
                Propositions = new Proposition[] {
                    MismatchNegate,
                    MismatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        #endregion

        #region Disjunction Tests

        [TestMethod]
        public void TestDisjunctionMissingMissing()
        {
            Assert.IsFalse(new Disjunction()
            {
                Propositions = new Proposition[] {
                    Missing,
                    Missing
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMissingMissingNegate()
        {
            Assert.IsFalse(new Disjunction()
            {
                Propositions = new Proposition[] {
                    Missing,
                    MissingNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMissingMatchNegate()
        {
            Assert.IsFalse(new Disjunction()
            {
                Propositions = new Proposition[] {
                    Missing,
                    MatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMissingMismatchNegate()
        {
            Assert.IsTrue(new Disjunction()
            {
                Propositions = new Proposition[] {
                    Missing,
                    MismatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMatchMissing()
        {
            Assert.IsTrue(new Disjunction()
            {
                Propositions = new Proposition[] {
                    Match,
                    Missing
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMatchMatch()
        {
            Assert.IsTrue(new Disjunction()
            {
                Propositions = new Proposition[] {
                    Match,
                    Match
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMatchMissingNegate()
        {
            Assert.IsTrue(new Disjunction()
            {
                Propositions = new Proposition[] {
                    Match,
                    MissingNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMatchMatchNegate()
        {
            Assert.IsTrue(new Disjunction()
            {
                Propositions = new Proposition[] {
                    Match,
                    MatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMatchMismatchNegate()
        {
            Assert.IsTrue(new Disjunction()
            {
                Propositions = new Proposition[] {
                    Match,
                    MismatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMismatchMissing()
        {
            Assert.IsFalse(new Disjunction()
            {
                Propositions = new Proposition[] {
                    Mismatch,
                    Missing
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMismatchMatch()
        {
            Assert.IsTrue(new Disjunction()
            {
                Propositions = new Proposition[] {
                    Mismatch,
                    Match
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMismatchMismatch()
        {
            Assert.IsFalse(new Disjunction()
            {
                Propositions = new Proposition[] {
                    Mismatch,
                    Mismatch
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMismatchMissingNegate()
        {
            Assert.IsFalse(new Disjunction()
            {
                Propositions = new Proposition[] {
                    Mismatch,
                    MissingNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMismatchMatchNegate()
        {
            Assert.IsFalse(new Disjunction()
            {
                Propositions = new Proposition[] {
                    Mismatch,
                    MatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMismatchMismatchNegate()
        {
            Assert.IsTrue(new Disjunction()
            {
                Propositions = new Proposition[] {
                    Mismatch,
                    MismatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMissingNegateMissingNegate()
        {
            Assert.IsFalse(new Disjunction()
            {
                Propositions = new Proposition[] {
                    MissingNegate,
                    MissingNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMatchNegateMissingNegate()
        {
            Assert.IsFalse(new Disjunction()
            {
                Propositions = new Proposition[] {
                    MatchNegate,
                    MissingNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMatchNegateMatchNegate()
        {
            Assert.IsFalse(new Disjunction()
            {
                Propositions = new Proposition[] {
                    MatchNegate,
                    MatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMismatchNegateMissingNegate()
        {
            Assert.IsTrue(new Disjunction()
            {
                Propositions = new Proposition[] {
                    MismatchNegate,
                    MissingNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMismatchNegateMatchNegate()
        {
            Assert.IsTrue(new Disjunction()
            {
                Propositions = new Proposition[] {
                    MismatchNegate,
                    MatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        [TestMethod]
        public void TestDisjunctionMismatchNegateMismatchNegate()
        {
            Assert.IsTrue(new Disjunction()
            {
                Propositions = new Proposition[] {
                    MismatchNegate,
                    MismatchNegate
                }
            }.Evaluate(ReadOnlyPerson));
        }

        #endregion
    }
}
