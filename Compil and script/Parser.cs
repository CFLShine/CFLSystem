
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections;
using System.Collections.Generic;

namespace CFL_1.CFL_System.Compil
{
    public class Parser
    {
        public Parser() { }

        public Parser(string _code)
        {
            __code = _code;
            parse();
        }

        public void parse(string _code)
        {
            __code = _code;
            parse();
        }

        public void parse()
        {
            __syntaxTree = CSharpSyntaxTree.ParseText(__code);
            init();
        }

        public SyntaxTree syntaxTree
        { get {  return __syntaxTree ; } }

        public IEnumerable<SyntaxToken> tokens
        { get { return __syntaxTree.GetRoot().DescendantTokens() ; } }

        public IEnumerable<SyntaxTrivia> trivias
        { get { return __syntaxTree.GetRoot().DescendantTrivia() ;} }

        #region init
        private void init()
        {}

        #endregion init

        private string __code;
        private SyntaxTree __syntaxTree;
       
    }
}
