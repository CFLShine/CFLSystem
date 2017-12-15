using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using CFL_1.CFL_Data;
using Microsoft.CodeAnalysis.CSharp;

namespace CFL_1.CFL_System.Compil_and_script
{
    public class Scripter : CodeCompiler
    {
        /// <summary>
        /// <see cref="CodeCompiler.CodeCompiler(bool)"/>
        /// </summary>
        public Scripter(bool _addApplicationReferences = false)
            : base(_addApplicationReferences)
        { 
            init() ; 
        }

        /// <summary>
        /// <see cref="CodeCompiler.CodeCompiler(Type[])"/>
        /// </summary>
        public Scripter(Type[] _types)
            : base(_types)
        { 
            init(); 
        }

        public object eval(string _code)
        {
            object _result = null;
            CSharpScript.EvaluateAsync<object>(_code,
                                               __scriptOptions)
                                               .ContinueWith(s=> _result = s.Result).Wait();
            return _result;
        }

        
        public override void compil(string _code)
        {
            __script = CSharpScript.Create(_code,
                                           __scriptOptions);
            __syntaxTree = SyntaxFactory.ParseSyntaxTree(__script.Code);
            __compilation = __script.GetCompilation();
        }

        private void init()
        {
            __scriptOptions = ScriptOptions.Default;
            __scriptOptions.AddReferences(__references);
        }

        Script __script = null;
        ScriptOptions __scriptOptions;
    }
}
