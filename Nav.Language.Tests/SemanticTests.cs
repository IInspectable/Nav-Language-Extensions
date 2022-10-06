﻿using System.Linq;
using NUnit.Framework;
using Pharmatechnik.Nav.Language;
// ReSharper disable PossibleNullReferenceException

namespace Nav.Language.Tests; 

[TestFixture]
public class SemanticTests {

    [Test]
    public void TestOutgoingCallsWithChoice() {

        var nav = @"
task A
{
    init I1;            
    exit e1;

    I1  --> e1;     
}
task B
{
    init I1;            
    exit e1;

    I1  --> e1;     
}
task C
{
    init I1;
    task A;
    task B;

    exit e1;
    choice C1;

    I1    --> C1;  
    C1    --> A;    
    C1    --> B;

    A:e1  --> e1;
    B:e1  --> e1;   
}
        ";

        var model = ParseModel(nav);

        var taskC     =model.TryFindTaskDefinition("C");
        var initTrans = taskC.TryFindNode<IInitNodeSymbol>("I1")?.Outgoings.Single();

        var calls         = initTrans.GetReachableCalls();
        var callNodeNames = calls.Select(call => call.Node.Name).ToList();

        Assert.That(callNodeNames, Is.EquivalentTo(new []{"A","B"}));
    }

    [Test]
    public void TestOutgoingCallsWith2Choices() {

        var nav = @"
task A
{
    init I1;            
    exit e1;

    I1      -->     e1;     
}
task B
{
    init I1;            
    exit e1;

    I1      -->     e1;     
}
task C
{
    init I1;
    task A;
    task B;

    exit e1;
    choice C1;
    choice C2;

    I1      -->     C1;  
    C1      -->     A;    
    C1      -->     C2;

    C2      -->     B;

    A:e1    -->     e1;
    B:e1    -->     e1;   
}
        ";

        var model = ParseModel(nav);

        var taskC     = model.TryFindTaskDefinition("C");
        var initTrans = taskC.TryFindNode<IInitNodeSymbol>("I1")?.Outgoings.Single();

        var calls         = initTrans.GetReachableCalls();
        var callNodeNames = calls.Select(call => call.Node.Name).ToList();

        Assert.That(callNodeNames, Is.EquivalentTo(new[] { "A", "B" }));
    }

    [Test]
    public void TestOutgoingCallsWithView() {

        var nav = @"
task A
{
    init I1;            
    exit e1;

    I1      -->     e1;     
}
task B
{
    init I1;            
    exit e1;

    I1      -->     e1;     
}
task C
{
    init I1;
    task A;

    view V1;
    choice C1;
    choice C2;
    exit e1;

    I1      -->     V1;  

    V1      --> V1 on s1;
    V1      --> A  on s2;        
    V1      --> C1 on s3;
    C1      --> A;
    C1      --> C2;
    C2      --> e1;

    A:e1    --> V1;
}
        ";
            
        var model = ParseModel(nav);
        var taskC = model.TryFindTaskDefinition("C");

        // I1      -->     V1; 
        var initTransition = taskC.TryFindNode<IInitNodeSymbol>("I1")?.Outgoings.Single();          
        var calls          = initTransition.GetReachableCalls(); 
        var callNodeNames  = calls.Select(call => call.Node.Name).ToList();
        Assert.That(callNodeNames, Is.EquivalentTo(new[] { "V1" }));
        Assert.That(taskC.InitTransitions
                         .First(t => t.SourceReference.Name == "I1" && t.TargetReference.Name == "V1")
                         .GetReachableCalls()
                         .Select(call => call.Node.Name).ToList(), 
                    Is.EquivalentTo(new[] {"V1"}));


        // V1      --> V1 on s1;
        var triggerTrans1 = taskC.TryFindNode<IViewNodeSymbol>("V1")?.Outgoings.First();
        calls         = triggerTrans1.GetReachableCalls();
        callNodeNames = calls.Select(call => call.Node.Name).ToList();
        Assert.That(callNodeNames, Is.EquivalentTo(new[] { "V1" }));
        Assert.That(taskC.TriggerTransitions
                         .First(t => t.SourceReference.Name == "V1" && t.TargetReference.Name == "V1")
                         .GetReachableCalls()
                         .Select(call => call.Node.Name).ToList(), 
                    Is.EquivalentTo(new[] {"V1"}));

        // V1      --> A  on s2;   
        var triggerTrans2 = taskC.TryFindNode<IViewNodeSymbol>("V1")?.Outgoings.Skip(1).First();
        calls         = triggerTrans2.GetReachableCalls();
        callNodeNames = calls.Select(call => call.Node.Name).ToList();
        Assert.That(callNodeNames, Is.EquivalentTo(new[] { "A" }));            
        Assert.That(taskC.TriggerTransitions
                         .First(t => t.SourceReference.Name == "V1" && t.TargetReference.Name == "A")
                         .GetReachableCalls()
                         .Select(call => call.Node.Name).ToList(), 
                    Is.EquivalentTo(new[] {"A"}));
            

        // V1      --> C1 on s3;
        var triggerTrans3 = taskC.TryFindNode<IViewNodeSymbol>("V1")?.Outgoings.Skip(2).First();
        calls         = triggerTrans3.GetReachableCalls();
        callNodeNames = calls.Select(call => call.Node.Name).ToList();
        Assert.That(callNodeNames, Is.EquivalentTo(new[] { "A","e1" }));
        Assert.That(taskC.TriggerTransitions
                         .First(t => t.SourceReference.Name == "V1" && t.TargetReference.Name == "C1")
                         .GetReachableCalls()
                         .Select(call => call.Node.Name).ToList(), 
                    Is.EquivalentTo(new[] {"A","e1"}));

        // C1      --> A;
        Assert.That(taskC.ChoiceTransitions
                         .First(t => t.SourceReference.Name == "C1" && t.TargetReference.Name == "A")
                         .GetReachableCalls()
                         .Select(call => call.Node.Name).ToList(), 
                    Is.EquivalentTo(new[] {"A"}));

        // C1      --> C2;
        Assert.That(taskC.ChoiceTransitions
                         .First(t => t.SourceReference.Name == "C1" && t.TargetReference.Name == "C2")
                         .GetReachableCalls()
                         .Select(call => call.Node.Name).ToList(), 
                    Is.EquivalentTo(new[] {"e1"}));

        // C2      --> e1;
        Assert.That(taskC.ChoiceTransitions
                         .First(t => t.SourceReference.Name == "C2" && t.TargetReference.Name == "e1")
                         .GetReachableCalls()
                         .Select(call => call.Node.Name).ToList(), 
                    Is.EquivalentTo(new[] {"e1"}));
    }

    [Test]
    public void TestTransitionSymbolsPresent() {

        var nav = @"
            task A {
                init i;
                exit e;
                i --> e;
            }

            task B
            {
                init i;
                task A a;               
                choice c;
                view v;
                exit e;

                i      --> c;  

                c      --> a;
                c      --> e;
                c      --> v;

                v --> e on trigger;

                a:e --> e;
            }
            ";

        var model = ParseModel(nav);
        var taskB = model.TryFindTaskDefinition("B");

        Assert.That(model.Diagnostics.HasErrors()                  , Is.False, "Semantic Fehler");
        Assert.That(model.Syntax.SyntaxTree.Diagnostics.HasErrors(), Is.False, "Syntax Fehler");
        Assert.That(taskB                                          , Is.Not.Null);
            
        // Sicherstellen, dass die 4 Arten von Übergängen im Sematic Model wiedergegeben werden
        Assert.That(taskB.InitTransitions.Count   , Is.EqualTo(1));
        Assert.That(taskB.ChoiceTransitions.Count , Is.EqualTo(3));
        Assert.That(taskB.TriggerTransitions.Count, Is.EqualTo(1));
        Assert.That(taskB.ExitTransitions.Count   , Is.EqualTo(1));
    }

    [Test]
    public void ReachableTest() {
        var nav = @"
            task A {
                init i;
                exit e;
                i --> e;
            }

            task B
            {
                init i;
                task A a1;  
                task A a2;  
                task A a3;  
                choice c1;
                choice c2;
                exit e;
                view v1;
                view v2;

                i   --> c1;  

                c1  --> c2;

                c2  --> a1;
                a1:e --> e;
                c2  --> v1;

                v2 --> v2 on t1;
                v2 --> a2 on t3;

                v1 --> a2 on t2;

                a2:e --> e;
                
                a3:e --> e;
            }
            ";

        var model = ParseModel(nav);
        var taskB = model.TryFindTaskDefinition("B");

        var a = taskB.TryFindNode("a1");
        Assert.That(a.IsReachable(), Is.True);

        var v1 = taskB.TryFindNode("v1");
        Assert.That(v1.IsReachable(), Is.True);

        var a2 = taskB.TryFindNode("a2");
        Assert.That(a2.IsReachable(), Is.True);

        var v2 = taskB.TryFindNode("v2");
        Assert.That(v2.IsReachable(), Is.False);    
            
        var a3 = taskB.TryFindNode("a3");
        Assert.That(a3.IsReachable(), Is.False);
    }

    CodeGenerationUnit ParseModel(string source) {
        var syntax =Syntax.ParseCodeGenerationUnit(source);
        var model  = CodeGenerationUnit.FromCodeGenerationUnitSyntax(syntax);
        return model;
    }
}