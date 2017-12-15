using CFL_1.CFL_System.Compil;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace CFL_1.CFL_System.Compil_and_script
{
    public enum HilightType
    {
        COMMENT,
        KEYWORD,
        CLASSNAME,
        MEMBER,
        STRING,
        NUMBER,
        NOTHING
    }
    public struct Hilight
    {
        public Hilight(int _pos, int _length, HilightType _type)
        {
           pos = _pos;
           length = _length;
           type = _type;
        }
        public int pos;
        public int length;
        public HilightType type;
    }

    public class Hilighter
    {
        public Hilighter(CodeCompiler _compiler, HilightFunction _hilightFunction)
        { 
            hilightFunction = _hilightFunction;
            __compiler = _compiler;
        }

        public delegate void HilightFunction(List<Hilight> _list);
        public HilightFunction hilightFunction;

        public void highlight()
        {
            highlightTrivia();
            highlightTokens();
        }

        public void highlightTrivia()
        {
            IEnumerator<SyntaxTrivia> _trivias = __compiler.trivias.GetEnumerator();

            while(_trivias.MoveNext())
            {
                SyntaxTrivia _trivia = _trivias.Current;
                string _text = SyntaxAnalyzeHelper.text(_trivia);
                if(_text == "")
                    continue;
                
                if(SyntaxAnalyzeHelper.isComment(_trivia))
                    __hilights.Add(new Hilight(SyntaxAnalyzeHelper.positionInText(_trivia), _text.Length, HilightType.COMMENT));
                else
                    __hilights.Add(new Hilight(SyntaxAnalyzeHelper.positionInText(_trivia), _text.Length, HilightType.NOTHING));
            }
            hilightFunction.Invoke(__hilights);
        }

        public void highlightTokens()
        {
            IEnumerator<SyntaxToken> _tokens = __compiler.tokens.GetEnumerator();
            IEnumerable<INamedTypeSymbol> _classes = SyntaxAnalyzeHelper.classesInCompilation(__compiler.compilation);
            
            while(_tokens.MoveNext())
            {
                SyntaxToken _token= _tokens.Current;
                string _text = SyntaxAnalyzeHelper.text(_token);
                if(_text == "")
                    continue;

                if(SyntaxAnalyzeHelper.isKeyWord(_token))
                    __hilights.Add(new Hilight(SyntaxAnalyzeHelper.positionInText(_token), _text.Length, HilightType.KEYWORD));
                else
                if(SyntaxAnalyzeHelper.isString(_token))
                    __hilights.Add(new Hilight(SyntaxAnalyzeHelper.positionInText(_token), _text.Length, HilightType.STRING));
                else
                if(SyntaxAnalyzeHelper.isNumeric(_token))
                    __hilights.Add(new Hilight(SyntaxAnalyzeHelper.positionInText(_token), _text.Length, HilightType.NUMBER));
                else
                if(SyntaxAnalyzeHelper.isMember(_token))
                    __hilights.Add(new Hilight(SyntaxAnalyzeHelper.positionInText(_token), _text.Length, HilightType.MEMBER));
                else
                if(SyntaxAnalyzeHelper.isClassName_declaration(_token) 
                || SyntaxAnalyzeHelper.isClassName(_token, __compiler.semanticModel))
                    __hilights.Add(new Hilight(SyntaxAnalyzeHelper.positionInText(_token), _text.Length, HilightType.CLASSNAME));
                
                

                else
                    __hilights.Add(new Hilight(SyntaxAnalyzeHelper.positionInText(_token), _text.Length, HilightType.NOTHING));
            }
            hilightFunction.Invoke(__hilights);
        }



        private CodeCompiler __compiler;

        private List<Hilight> __hilights = new List<Hilight>();
    }
}
