﻿using System.Linq;
using RiaServicesContrib.DomainServices.Client;
using EntityGraphTests.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityGraphTests.Tests
{
    [TestClass]
    public class IEnumerableTests : EntityGraphTest{
        [TestMethod]
        public void IEnumerableCountGraphTest() {
            EntityGraph gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            a.BSet.Add(new B());
            Assert.IsTrue(gr.Count() == 5, "Graph contains unexpected number of elements");
        }
        [TestMethod]
        public void IEnumerableCountNamedGraphTest() {
            EntityGraph gr = a.EntityGraph(EntityGraphs.CircularGraphShape1);
            a.BSet.Add(new B());
            Assert.IsTrue(gr.Count() == 4, "Graph contains unexpected number of elements");
        }

        [TestMethod]
        public void IEnumerableTest() {
            B newB = new B();
            a.BSet.Add(newB);
            EntityGraph eg = a.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.AreEqual(a, eg.OfType<A>().Single());
            Assert.AreEqual(c, eg.OfType<C>().Single());
            Assert.AreEqual(d, eg.OfType<D>().Single());

            Assert.IsTrue(eg.OfType<B>().Contains(b));
            Assert.IsTrue(eg.OfType<B>().Contains(newB));

            Assert.IsTrue(eg.Count() == 5);
        }

        [TestMethod]
        public void IEnumerableNamedGraphTest() {
            B newB = new B();
            a.BSet.Add(newB);
            EntityGraph eg = a.EntityGraph(EntityGraphs.CircularGraphShape1);
            Assert.AreEqual(a, eg.OfType<A>().Single());
            Assert.AreEqual(b, eg.OfType<B>().Single());
            Assert.AreEqual(c, eg.OfType<C>().Single());
            Assert.AreEqual(d, eg.OfType<D>().Single());

            Assert.IsTrue(eg.Count() == 4);
        }
    }
}
