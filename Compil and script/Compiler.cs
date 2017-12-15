
using System;

//using Microsoft.CSharp;
//using System.CodeDom.Compiler;
//using Microsoft.CodeAnalysis.Emit;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using CFL_1.CFL_System.Compil_and_script;
using System.Reflection;
using System.IO;
using Microsoft.CodeAnalysis.Emit;
using System.Linq;

namespace CFL_1.CFL_System.Compil
{
    // see Roslyn

    public class Compiler : CodeCompiler
    {
        /// <summary>
        /// <see cref="CodeCompiler.CodeCompiler(Type[])"/>
        /// </summary>
        public Compiler(Type[] _types)
            : base(_types)
        {
            init();
        }

        /// <summary>
        /// <see cref="CodeCompiler.CodeCompiler(bool)"/>
        /// </summary>
        public Compiler(bool _addApplicationReferences = false)
            : base(_addApplicationReferences)
        {
            init();
        }

        public override void compil(string _code)
        {
            __code = _code; 
            compil();
        }

        /// <summary>
        /// Lance une compilation.
        /// Si réussite, retourne l'assembly,
        /// sinon, retourne null.
        /// </summary>
        public Assembly assembly()
        {
            using (var ms = new MemoryStream())
            {
                compil();
                if(__compilation == null)
                    return null;

                EmitResult result = compilation.Emit(ms);
            
                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic => 
                        diagnostic.IsWarningAsError || 
                        diagnostic.Severity == DiagnosticSeverity.Error);
            
                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                    return null;
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly _assembly = Assembly.Load(ms.ToArray());
                    return _assembly;
                    /*assembly.EntryPoint.Invoke(null, )

                    Type type = assembly.GetType(_classFullName);
                    object _obj = Activator.CreateInstance(type);
                    type.InvokeMember(_functionName,
                        BindingFlags.Default | BindingFlags.InvokeMethod,
                        null,
                        _obj,
                        new object[] { "Hello World" });
                    return _obj;*/
                }
            }
        }

        public Assembly run<T>(T _parameters)
        {
            Assembly _assembly = assembly();
            if(_assembly == null)
                return null;
            
            MethodInfo _entry = entryPoint(_assembly);
            if(_entry == null)
                return null;
           
            _entry.Invoke(null, new object[] { _parameters } );
            return _assembly;
        }

        private MethodInfo entryPoint(Assembly _assembly)
        {
            foreach(var _type in _assembly.DefinedTypes)
            {
                List<MethodInfo> _list = _type.GetDeclaredMethods("Main").ToList<MethodInfo>();
                if(_list.Count > 0)
                    return _list[0];
            }
            return null;
        }

        private void compil()
        {
            syntaxTree = CSharpSyntaxTree.ParseText(__code);

            __compilation = 
                CSharpCompilation.Create
                (   "Compilation", 
                    new[] {__syntaxTree},
                    __references, 
                    __options
                );
        }

        private void init()
        {
            

            __options = 
                new CSharpCompilationOptions
                (   outputKind : OutputKind.DynamicallyLinkedLibrary );
        }

        string __code = "";
        
        private CSharpCompilationOptions __options;
    }

    

}
