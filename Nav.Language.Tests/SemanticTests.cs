﻿using NUnit.Framework;
using Pharmatechnik.Nav.Language;
using System.Linq;

namespace Nav.Language.Tests {
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

            var taskC=model.TryFindTaskDefinition("C");
            var initTrans = taskC.TryFindNode<IInitNodeSymbol>("I1")?.Outgoings.Single();

            var calls= initTrans.GetReachableCalls();
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

            var taskC = model.TryFindTaskDefinition("C");
            var initTrans = taskC.TryFindNode<IInitNodeSymbol>("I1")?.Outgoings.Single();

            var calls = initTrans.GetReachableCalls();
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
            var initTransition = taskC.TryFindNode<IInitNodeSymbol>("I1")?.Outgoings.Single();          
            var calls = initTransition.GetReachableCalls(); 
            var callNodeNames = calls.Select(call => call.Node.Name).ToList();
            Assert.That(callNodeNames, Is.EquivalentTo(new[] { "V1" }));

            var triggerTrans1 = taskC.TryFindNode<IViewNodeSymbol>("V1")?.Outgoings.First();
            calls = triggerTrans1.GetReachableCalls();
            callNodeNames = calls.Select(call => call.Node.Name).ToList();
            Assert.That(callNodeNames, Is.EquivalentTo(new[] { "V1" }));

            var triggerTrans2 = taskC.TryFindNode<IViewNodeSymbol>("V1")?.Outgoings.Skip(1).First();
            calls = triggerTrans2.GetReachableCalls();
            callNodeNames = calls.Select(call => call.Node.Name).ToList();
            Assert.That(callNodeNames, Is.EquivalentTo(new[] { "A" }));

            var triggerTrans3 = taskC.TryFindNode<IViewNodeSymbol>("V1")?.Outgoings.Skip(2).First();
            calls = triggerTrans3.GetReachableCalls();
            callNodeNames = calls.Select(call => call.Node.Name).ToList();
            Assert.That(callNodeNames, Is.EquivalentTo(new[] { "A","e1" }));
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

        CodeGenerationUnit ParseModel(string source) {
            var syntax=Syntax.ParseCodeGenerationUnit(source);
            var model= CodeGenerationUnit.FromCodeGenerationUnitSyntax(syntax);
            return model;
        }
    }
}
