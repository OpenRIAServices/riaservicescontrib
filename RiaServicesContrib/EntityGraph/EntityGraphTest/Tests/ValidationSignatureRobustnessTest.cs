﻿using System.ComponentModel.DataAnnotations;
using System.Linq;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RIA.EntityValidator;

namespace EntityGraphTest.Tests
{
    public class ValidationSignatureRobustnessValidation : ValidationRule<A, ValidationResult>
    {
        public override ValidationRuleDependencies<A> Signature
        {
            get
            {
                return new ValidationRuleDependencies<A>
                {
                    A => A.name,
                    A => A.B,
                    A => A.B.C
                };
            }
        }
        public void Validate(C c)
        {
        }
    }

    [TestClass]
    public class ValidationSignatureRobustnessTest : EntityGraphTest
    {
        public override void TestSetup()
        {
            base.TestSetup();
            MEFValidationRules.RegisterType(typeof(ValidationSignatureRobustnessValidation));
        }
        public override void TestCleanup()
        {
            base.TestCleanup();
            MEFValidationRules.UnregisterType(typeof(ValidationSignatureRobustnessValidation));
        }
        [TestMethod]
        public void TestSignatureWithIncompleteEntityGraph()
        {
            a.name = null;
            a.B = null;
            var engine = new ValidationEngine<A, ValidationResult>(new MEFValidationRulesProvider<A, ValidationResult>(), a);
            Assert.IsTrue(engine.ObjectsInvolved().Count() == 0);
            a.B = new B();
            engine.Refresh();
            Assert.IsTrue(engine.ObjectsInvolved().Count() == 2);
        }
    }
}