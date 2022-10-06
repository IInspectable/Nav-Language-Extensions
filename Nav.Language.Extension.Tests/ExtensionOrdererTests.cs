#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Nav.Language.Extension.Tests; 

[TestFixture]
public class ExtensionOrdererTests {

    [Test]
    public void EquivalentTest() {

        var extensions = new[] {
            Create("Foo"),
            Create("Bar"),
            Create("Baz")
        };

        var result =ExtensionOrderer.Order(extensions);

        Assert.That(extensions, Is.EquivalentTo(result));
    }

    [Test]        
    public void DuplicateNameTest() {

        var extensions = new[] {
            Create("Foo"),
            Create("Foo"),
            Create("Baz")
        };

        Assert.Throws<ArgumentException>( ()=> ExtensionOrderer.Order(extensions));
    }

    [Test]
    public void CycleAfterSelfTest() {

        var extensions = new[] {
            Create("Foo").After("Foo"),     
        };

        Assert.Throws<ArgumentException>(() => ExtensionOrderer.Order(extensions));
    }

    [Test]
    public void CycleBeforeSelfTest() {

        var extensions = new[] {
            Create("Foo").Before("Foo"),
        };

        Assert.Throws<ArgumentException>(() => ExtensionOrderer.Order(extensions));
    }

    [Test]
    public void SimpleOrderTest() {

        var extensions = new[] {
            Create("Foo").After("Baz"),
            Create("Bar").After("Baz").Before("Foo"),
            Create("Baz")
        };

        var result = ExtensionOrderer.Order(extensions);

        var expected = new[] {
            "Baz",
            "Bar",
            "Foo"
        };

        Assert.That(result.Select(e => e.Metadata.Name), Is.EqualTo(expected));
    }

    [Test]
    public void SimpleCycleTest() {

        var extensions = new[] {
            Create("Foo").After("Baz"),
            Create("Bar").After("Baz").Before("Foo"),
            Create("Baz").After("Foo")
        };

        Assert.Throws<ArgumentException>(() => ExtensionOrderer.Order(extensions));
    }

    [Test]
    public void OrderTest() {

        var extensions = new[] {
            Create("Foo").After("Baz").After("Bar"),
            Create("Bar").After("Baz").Before("Foo"),
            Create("Baz").Before("Bar").Before("Foo")
        };

        var result = ExtensionOrderer.Order(extensions);

        var expected = new[] {
            "Baz",
            "Bar",
            "Foo"
        };

        Assert.That(result.Select(e => e.Metadata.Name), Is.EqualTo(expected));
    }

    [Test]
    public void CycleTest() {

        var extensions = new[] {
            Create("Foo").After("Baz").After("Bar"),
            Create("Bar").After("Baz").Before("Foo"),
            Create("Baz").Before("Bar").Before("Foo").Before("Bar")
        };

        var result = ExtensionOrderer.Order(extensions);

        var expected = new[] {
            "Baz",
            "Bar",
            "Foo"
        };

        Assert.That(result.Select(e => e.Metadata.Name), Is.EqualTo(expected));
    }
        
    #region Helper

    TestLazy Create(string name) {
        return new(name);
    }
      
    class OrderableMetadata : IOrderableMetadata {

        public OrderableMetadata(string name) {
            Name   = name;
            Before = new List<string>();
            After  = new List<string>();
        }

        public string       Name   { get; }
        public List<string> Before { get; }
        public List<string> After  { get; }
            
        string IOrderableMetadata.               Name   => Name;
        IReadOnlyList<string> IOrderableMetadata.Before => Before;
        IReadOnlyList<string> IOrderableMetadata.After  => After;
    }
        
    class TestLazy: Lazy<string, OrderableMetadata> {
        public TestLazy(string name): base(() => name, new OrderableMetadata(name)) {                
        }

        public TestLazy Before(string before) {
            Metadata.Before.Add(before);
            return this;
        }
        public TestLazy After(string after) {
            Metadata.After.Add(after);
            return this;
        }
    }

    #endregion
}