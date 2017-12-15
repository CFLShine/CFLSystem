
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CFL_1.CFL_System.Compil_and_script
{
    public abstract class CodeCompiler
    {
        /// <summary>
        /// Si _addApplicationReferences == true, la compilation se fera avec les références 
        /// de toutes les classes de cette sollution.
        /// </summary>
        protected CodeCompiler(bool _addApplicationReferences = false)
        {
            Type[] _types = null;
            if(_addApplicationReferences)
            {
                try
                {
                    _types = Assembly.GetExecutingAssembly().GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    _types = ex.Types;
                }
            }
            addReferences(_types);
        }

        /// <summary>
        /// _types : Types des classes dont les références seront ajoutées
        /// à la compilation.
        /// </summary>
        protected CodeCompiler(Type[] _types)
        {
            addReferences(_types);
        }

        public SyntaxTree syntaxTree
        {
            get { return __syntaxTree; }
            protected set { __syntaxTree = value; }
        }

        public abstract void compil(string _code);


        public SemanticModel semanticModel
        { get { return compilation.GetSemanticModel(__syntaxTree); } }

        public IEnumerable<SyntaxToken> tokens
        { get { return syntaxTree.GetRoot().DescendantTokens() ; } }

        public IEnumerable<SyntaxTrivia> trivias
        { get { return syntaxTree.GetRoot().DescendantTrivia() ;} }

        public Compilation compilation
        { get { return __compilation ; } }

        protected void addReferences(Type[] _types = null)
        {
            __references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            if(_types != null)
            {
                foreach(Type _type in _types)
                __references.Add(MetadataReference.CreateFromFile(_type.Assembly.Location));
            }
        }

        protected SyntaxTree __syntaxTree ;
        protected Compilation __compilation;
        protected List<MetadataReference> __references = new List<MetadataReference>();
    }
}
