#region Using Directives

using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

    public class AnalyzerContext {

    }

    public interface ITaskDefinitionAnalyzer {

        DiagnosticDescriptor Descriptor { get; }
        IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context);

    }

    static class Analyzer {

        private static readonly Lazy<IList<ITaskDefinitionAnalyzer>> TaskDefinitionAnalyzer = new Lazy<IList<ITaskDefinitionAnalyzer>>(
            () => GetInterfaceImplementationsFromAssembly<ITaskDefinitionAnalyzer>().ToList(),
            LazyThreadSafetyMode.PublicationOnly);

        public static IEnumerable<ITaskDefinitionAnalyzer> GetTaskDefinitionAnalyzer() {
            return TaskDefinitionAnalyzer.Value;
        }

        private static IEnumerable<T> GetInterfaceImplementationsFromAssembly<T>() where T : class {

            var dll   = typeof(Analyzer).GetTypeInfo().Assembly;
            var rules = new List<T>();

            foreach (var type in dll.ExportedTypes) {
                var typeInfo = type.GetTypeInfo();
                if (!typeInfo.IsInterface
                 && !typeInfo.IsAbstract
                 && typeInfo.ImplementedInterfaces.Contains(typeof(T))) {

                    var ruleObj = Activator.CreateInstance(type);
                    if (!(ruleObj is T rule)) {

                        continue;
                    }

                    rules.Add(rule);
                }
            }

            return rules;
        }

    }

}