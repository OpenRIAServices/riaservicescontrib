﻿using System.ComponentModel;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using EntityGraph.RIA;
using EntityGraphTest.Web;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class IEditableObjectTests : EntityGraphTest
    {
        [TestMethod]
        public void IEditableObjectScalarPropertyTest() {
            string oldNameForA = a.name;
            string oldNameForB = b.name;
            string oldNameForC = c.name;
            string oldNameForD = d.name;

            var gr = a.EntityGraph();
            gr.BeginEdit();
            a.name = "NewNameForA";
            b.name = "NewNameForB";
            c.name = "NewNameForC";
            d.name = "NewNameForD";
            gr.CancelEdit();

            Assert.IsTrue(a.name == oldNameForA, "Change of A's name is not correctly canceled");
            Assert.IsTrue(b.name == oldNameForB, "Change of B's name is not correctly canceled");
            Assert.IsTrue(c.name == oldNameForC, "Change of C's name is not correctly canceled");
            Assert.IsTrue(d.name == oldNameForD, "Change of D's name is not correctly canceled");
        }

        [Asynchronous]
        [TestMethod]
        public void IEditableObjectAssociationSetTest() {
            EntityGraphTestDomainContext ctx = new EntityGraphTestDomainContext();
            LoadOperation<B> loadOp = ctx.Load(ctx.GetBSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    B existingB = loadOp.Entities.SingleOrDefault();
                    B b = new B();
                    var gr = a.EntityGraph();

                    gr.BeginEdit();
                    // Adding a new entity (b) will fail, because this will not correctly raise the proper 
                    // collection changed events when CancelEdit is called. (http://forums.silverlight.net/forums/p/204333/478862.aspx#478862)
                    //
                    // Adding to a.BSet directly does change existingB.AId prior to the invocation
                    // of existingB.BeginEdit(). Consequently, existingB.CancelEdit() will not restore existingB.AId.
                    // Calling Add on the entity graph calls existingB.BEginEdit() prior to adding
                    // existingB to a.BSet.
                    ((IEditableObject)existingB).BeginEdit();
                    a.BSet.Add(existingB);
                    Assert.IsTrue(a.BSet.Count() == 1, "a.BSet has incorrect number of elements");
                    gr.CancelEdit();
                    Assert.IsTrue(a.BSet.Count() == 0, "a.BSet has incorrect number of elements");
                });
            EnqueueTestComplete();
        }
        [Asynchronous]
        [TestMethod]
        public void IEditableObjectAssociationSetAndRemoveTest() {
            EntityGraphTestDomainContext ctx = new EntityGraphTestDomainContext();
            LoadOperation<B> loadOp = ctx.Load(ctx.GetBSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    B existingB = loadOp.Entities.SingleOrDefault();
                    B b = new B();
                    var gr = a.EntityGraph();

                    gr.BeginEdit();
                    ((IEditableObject)b).BeginEdit();
                    a.BSet.Add(b);
                    a.BSet.Remove(b);
                    Assert.IsTrue(a.BSet.Count() == 0, "a.BSet has incorrect number of elements");
                    gr.CancelEdit();
                    Assert.IsTrue(a.BSet.Count() == 0, "a.BSet has incorrect number of elements");
                });
            EnqueueTestComplete();
        }

        [Asynchronous]
        [TestMethod]
        public void IEditableObjectAssociationTest() {
            EntityGraphTestDomainContext ctx = new EntityGraphTestDomainContext();
            LoadOperation<B> loadOp = ctx.Load(ctx.GetBSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    ctx.As.Add(a);
                    B existingB = loadOp.Entities.SingleOrDefault();
                    B newB = new B { Id = -1 };
                    var gr = a.EntityGraph();

                    a.B = existingB;
                    gr.BeginEdit();
                    a.B = newB;
                    gr.CancelEdit();
                    Assert.IsTrue(a.BId == existingB.Id); // Succeeds 
                    Assert.IsTrue(a.B == existingB); // Succeeds

                    a.B = newB;
                    gr.BeginEdit();
                    a.B = existingB;
                    gr.CancelEdit();

                    // Here we have a bug in IEditableObject (http://forums.silverlight.net/forums/p/204333/478308.aspx#478308)
                    Assert.IsTrue(a.BId == newB.Id); // Succeeds 
                    Assert.IsTrue(a.B != newB); // Potential RIA bug, (should be equal to newB)
                    Assert.IsTrue(a.B == null); // Potential RIA bug, (should be equal to newB)
                });
            EnqueueTestComplete();
        }
    }
}