

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace CFL_1.CFL_System.Compil
{
    public static class SyntaxAnalyzeHelper
    {

        #region text and position

        public static string text(SyntaxToken _token)
        { return _token.ToString(); }

        public static string text(SyntaxTrivia _trivia)
        { return _trivia.ToString(); }

        public static int line(SyntaxToken _token)
        { return _token.GetLocation().GetLineSpan().StartLinePosition.Line ; }

        public static int line(SyntaxTrivia _trivia)
        { return _trivia.GetLocation().GetLineSpan().StartLinePosition.Line ; }

        public static int positionInLine(SyntaxToken _token)
        { return _token.GetLocation().GetLineSpan().StartLinePosition.Character; }

        public static int positionInLine(SyntaxTrivia _trivia)
        { return _trivia.GetLocation().GetLineSpan().StartLinePosition.Character; }

        public static int positionInText(SyntaxToken _token)
        { return _token.Span.Start; }

        public static int positionInText(SyntaxTrivia _trivia)
        { return _trivia.Span.Start; }

        #endregion text and position

        #region analyse

        public static bool isKeyWord(SyntaxToken _token)
        { return _token.IsKeyword(); }

        public static bool isIdentifier(SyntaxToken _token)
        { return _token.IsKind(SyntaxKind.IdentifierToken); }

        public static bool isString(SyntaxToken _token)
        { return _token.IsKind(SyntaxKind.StringLiteralToken); }

        public static bool isComment(SyntaxTrivia _trivia)
        { return _trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) || _trivia.IsKind(SyntaxKind.MultiLineCommentTrivia); }

        public static bool isNumeric(SyntaxToken _token)
        { return _token.IsKind(SyntaxKind.NumericLiteralToken) ; }

        public static bool isMember(SyntaxToken _token)
        {
            if(!isIdentifier(_token))
                return false;
            SyntaxNode _parent = _token.Parent;
            while (_parent != null)
            {
                if(_parent.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                    return _parent.GetFirstToken() != _token;
                _parent = _parent.Parent;
            }
            return false;
        }

        public static bool isClassName_declaration(SyntaxToken _token)
        {
            if(_token.Parent == null || !isIdentifier(_token))
                return false;
            return ( _token.Parent.IsKind(SyntaxKind.ClassDeclaration));
        }

        public static IEnumerable<INamedTypeSymbol> classesInCompilation(Compilation _compilation)
        {
            return _compilation.GlobalNamespace.GetNamespaceMembers().SelectMany(x=>x.GetTypeMembers());
        }

        public static bool isClassName(SyntaxToken _token, SemanticModel _model)
        {
            if(!isIdentifier(_token))
                return false;

            try
            {
                TypeInfo _typeInfo = _model.GetTypeInfo(_token.Parent);
                
                ITypeSymbol _type = _typeInfo.Type;
            
                if(_type != null)
                {
                    return _type.TypeKind == TypeKind.Class && _token.Text == _type.Name;
                }

                SymbolInfo _symbolInfo = _model.GetSymbolInfo(_token.Parent);

                return _symbolInfo.Symbol.Kind == SymbolKind.NamedType;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        #endregion analyse

        public static string kindStr(SyntaxKind _kind)
        {
            switch (_kind)
            {
                case SyntaxKind.None:
                    return "None";
                case SyntaxKind.List:
                    return "List";
                case SyntaxKind.TildeToken:
                    return "TildeToken";
                case SyntaxKind.ExclamationToken:
                    return "ExclamationToken";
                case SyntaxKind.DollarToken:
                    return "DollarToken";
                case SyntaxKind.PercentToken:
                    return "PercentToken";
                case SyntaxKind.CaretToken:
                    return "CaretToken";
                case SyntaxKind.AmpersandToken:
                    return "AmpersandToken";
                case SyntaxKind.AsteriskToken:
                    return "AsteriskToken";
                case SyntaxKind.OpenParenToken:
                    return "OpenParenToken";
                case SyntaxKind.CloseParenToken:
                    return "CloseParenToken";
                case SyntaxKind.MinusToken:
                    return "MinusToken";
                case SyntaxKind.PlusToken:
                    return "PlusToken";
                case SyntaxKind.EqualsToken:
                    return "EqualsToken";
                case SyntaxKind.OpenBraceToken:
                    return "OpenBraceToken";
                case SyntaxKind.CloseBraceToken:
                    return "CloseBraceToken";
                case SyntaxKind.OpenBracketToken:
                    return "OpenBracketToken";
                case SyntaxKind.CloseBracketToken:
                    return "CloseBracketToken";
                case SyntaxKind.BarToken:
                    return "BarToken";
                case SyntaxKind.BackslashToken:
                    return "BackslashToken";
                case SyntaxKind.ColonToken:
                    return "ColonToken";
                case SyntaxKind.SemicolonToken:
                    return "SemicolonToken";
                case SyntaxKind.DoubleQuoteToken:
                    return "DoubleQuoteToken";
                case SyntaxKind.SingleQuoteToken:
                    return "SingleQuoteToken";
                case SyntaxKind.LessThanToken:
                    return "LessThanToken";
                case SyntaxKind.CommaToken:
                    return "CommaToken";
                case SyntaxKind.GreaterThanToken:
                    return "GreaterThanToken";
                case SyntaxKind.DotToken:
                    return "DotToken";
                case SyntaxKind.QuestionToken:
                    return "QuestionToken";
                case SyntaxKind.HashToken:
                    return "HashToken";
                case SyntaxKind.SlashToken:
                    return "SlashToken";
                case SyntaxKind.SlashGreaterThanToken:
                    return "SlashGreaterThanToken";
                case SyntaxKind.LessThanSlashToken:
                    return "LessThanSlashToken";
                case SyntaxKind.XmlCommentStartToken:
                    return "XmlCommentStartToken";
                case SyntaxKind.XmlCommentEndToken:
                    return "XmlCommentEndToken";
                case SyntaxKind.XmlCDataStartToken:
                    return "XmlCDataStartToken";
                case SyntaxKind.XmlCDataEndToken:
                    return "XmlCDataEndToken";
                case SyntaxKind.XmlProcessingInstructionStartToken:
                    return "XmlProcessingInstructionStartToken";
                case SyntaxKind.XmlProcessingInstructionEndToken:
                    return "XmlProcessingInstructionEndToken";
                case SyntaxKind.BarBarToken:
                    return "BarBarToken";
                case SyntaxKind.AmpersandAmpersandToken:
                    return "AmpersandAmpersandToken";
                case SyntaxKind.MinusMinusToken:
                    return "MinusMinusToken";
                case SyntaxKind.PlusPlusToken:
                    return "PlusPlusToken";
                case SyntaxKind.ColonColonToken:
                    return "ColonColonToken";
                case SyntaxKind.QuestionQuestionToken:
                    return "QuestionQuestionToken";
                case SyntaxKind.MinusGreaterThanToken:
                    return "MinusGreaterThanToken";
                case SyntaxKind.ExclamationEqualsToken:
                    return "ExclamationEqualsToken";
                case SyntaxKind.EqualsEqualsToken:
                    return "EqualsEqualsToken";
                case SyntaxKind.EqualsGreaterThanToken:
                    return "EqualsGreaterThanToken";
                case SyntaxKind.LessThanEqualsToken:
                    return "LessThanEqualsToken";
                case SyntaxKind.LessThanLessThanToken:
                    return "LessThanLessThanToken";
                case SyntaxKind.LessThanLessThanEqualsToken:
                    return "LessThanLessThanEqualsToken";
                case SyntaxKind.GreaterThanEqualsToken:
                    return "GreaterThanEqualsToken";
                case SyntaxKind.GreaterThanGreaterThanToken:
                    return "GreaterThanGreaterThanToken";
                case SyntaxKind.GreaterThanGreaterThanEqualsToken:
                    return "GreaterThanGreaterThanEqualsToken";
                case SyntaxKind.SlashEqualsToken:
                    return "SlashEqualsToken";
                case SyntaxKind.AsteriskEqualsToken:
                    return "AsteriskEqualsToken";
                case SyntaxKind.BarEqualsToken:
                    return "BarEqualsToken";
                case SyntaxKind.AmpersandEqualsToken:
                    return "AmpersandEqualsToken";
                case SyntaxKind.PlusEqualsToken:
                    return "PlusEqualsToken";
                case SyntaxKind.MinusEqualsToken:
                    return "MinusEqualsToken";
                case SyntaxKind.CaretEqualsToken:
                    return "CaretEqualsToken";
                case SyntaxKind.PercentEqualsToken:
                    return "PercentEqualsToken";
                case SyntaxKind.BoolKeyword:
                    return "BoolKeyword";
                case SyntaxKind.ByteKeyword:
                    return "ByteKeyword";
                case SyntaxKind.SByteKeyword:
                    return "SByteKeyword";
                case SyntaxKind.ShortKeyword:
                    return "ShortKeyword";
                case SyntaxKind.UShortKeyword:
                    return "UShortKeyword";
                case SyntaxKind.IntKeyword:
                    return "IntKeyword";
                case SyntaxKind.UIntKeyword:
                    return "UIntKeyword";
                case SyntaxKind.LongKeyword:
                    return "LongKeyword";
                case SyntaxKind.ULongKeyword:
                    return "ULongKeyword";
                case SyntaxKind.DoubleKeyword:
                    return "DoubleKeyword";
                case SyntaxKind.FloatKeyword:
                    return "FloatKeyword";
                case SyntaxKind.DecimalKeyword:
                    return "DecimalKeyword";
                case SyntaxKind.StringKeyword:
                    return "StringKeyword";
                case SyntaxKind.CharKeyword:
                    return "CharKeyword";
                case SyntaxKind.VoidKeyword:
                    return "VoidKeyword";
                case SyntaxKind.ObjectKeyword:
                    return "ObjectKeyword";
                case SyntaxKind.TypeOfKeyword:
                    return "TypeOfKeyword";
                case SyntaxKind.SizeOfKeyword:
                    return "SizeOfKeyword";
                case SyntaxKind.NullKeyword:
                    return "NullKeyword";
                case SyntaxKind.TrueKeyword:
                    return "TrueKeyword";
                case SyntaxKind.FalseKeyword:
                    return "FalseKeyword";
                case SyntaxKind.IfKeyword:
                    return "IfKeyword";
                case SyntaxKind.ElseKeyword:
                    return "ElseKeyword";
                case SyntaxKind.WhileKeyword:
                    return "WhileKeyword";
                case SyntaxKind.ForKeyword:
                    return "ForKeyword";
                case SyntaxKind.ForEachKeyword:
                    return "ForEachKeyword";
                case SyntaxKind.DoKeyword:
                    return "DoKeyword";
                case SyntaxKind.SwitchKeyword:
                    return "SwitchKeyword";
                case SyntaxKind.CaseKeyword:
                    return "CaseKeyword";
                case SyntaxKind.DefaultKeyword:
                    return "DefaultKeyword";
                case SyntaxKind.TryKeyword:
                    return "TryKeyword";
                case SyntaxKind.CatchKeyword:
                    return "CatchKeyword";
                case SyntaxKind.FinallyKeyword:
                    return "FinallyKeyword";
                case SyntaxKind.LockKeyword:
                    return "LockKeyword";
                case SyntaxKind.GotoKeyword:
                    return "GotoKeyword";
                case SyntaxKind.BreakKeyword:
                    return "BreakKeyword";
                case SyntaxKind.ContinueKeyword:
                    return "ContinueKeyword";
                case SyntaxKind.ReturnKeyword:
                    return "ReturnKeyword";
                case SyntaxKind.ThrowKeyword:
                    return "ThrowKeyword";
                case SyntaxKind.PublicKeyword:
                    return "PublicKeyword";
                case SyntaxKind.PrivateKeyword:
                    return "PrivateKeyword";
                case SyntaxKind.InternalKeyword:
                    return "InternalKeyword";
                case SyntaxKind.ProtectedKeyword:
                    return "ProtectedKeyword";
                case SyntaxKind.StaticKeyword:
                    return "StaticKeyword";
                case SyntaxKind.ReadOnlyKeyword:
                    return "ReadOnlyKeyword";
                case SyntaxKind.SealedKeyword:
                    return "SealedKeyword";
                case SyntaxKind.ConstKeyword:
                    return "ConstKeyword";
                case SyntaxKind.FixedKeyword:
                    return "FixedKeyword";
                case SyntaxKind.StackAllocKeyword:
                    return "StackAllocKeyword";
                case SyntaxKind.VolatileKeyword:
                    return "VolatileKeyword";
                case SyntaxKind.NewKeyword:
                    return "NewKeyword";
                case SyntaxKind.OverrideKeyword:
                    return "OverrideKeyword";
                case SyntaxKind.AbstractKeyword:
                    return "AbstractKeyword";
                case SyntaxKind.VirtualKeyword:
                    return "VirtualKeyword";
                case SyntaxKind.EventKeyword:
                    return "EventKeyword";
                case SyntaxKind.ExternKeyword:
                    return "ExternKeyword";
                case SyntaxKind.RefKeyword:
                    return "RefKeyword";
                case SyntaxKind.OutKeyword:
                    return "OutKeyword";
                case SyntaxKind.InKeyword:
                    return "InKeyword";
                case SyntaxKind.IsKeyword:
                    return "IsKeyword";
                case SyntaxKind.AsKeyword:
                    return "AsKeyword";
                case SyntaxKind.ParamsKeyword:
                    return "ParamsKeyword";
                case SyntaxKind.ArgListKeyword:
                    return "ArgListKeyword";
                case SyntaxKind.MakeRefKeyword:
                    return "MakeRefKeyword";
                case SyntaxKind.RefTypeKeyword:
                    return "RefTypeKeyword";
                case SyntaxKind.RefValueKeyword:
                    return "RefValueKeyword";
                case SyntaxKind.ThisKeyword:
                    return "ThisKeyword";
                case SyntaxKind.BaseKeyword:
                    return "BaseKeyword";
                case SyntaxKind.NamespaceKeyword:
                    return "NamespaceKeyword";
                case SyntaxKind.UsingKeyword:
                    return "UsingKeyword";
                case SyntaxKind.ClassKeyword:
                    return "ClassKeyword";
                case SyntaxKind.StructKeyword:
                    return "StructKeyword";
                case SyntaxKind.InterfaceKeyword:
                    return "InterfaceKeyword";
                case SyntaxKind.EnumKeyword:
                    return "EnumKeyword";
                case SyntaxKind.DelegateKeyword:
                    return "DelegateKeyword";
                case SyntaxKind.CheckedKeyword:
                    return "CheckedKeyword";
                case SyntaxKind.UncheckedKeyword:
                    return "UncheckedKeyword";
                case SyntaxKind.UnsafeKeyword:
                    return "UnsafeKeyword";
                case SyntaxKind.OperatorKeyword:
                    return "OperatorKeyword";
                case SyntaxKind.ExplicitKeyword:
                    return "ExplicitKeyword";
                case SyntaxKind.ImplicitKeyword:
                    return "ImplicitKeyword";
                case SyntaxKind.YieldKeyword:
                    return "YieldKeyword";
                case SyntaxKind.PartialKeyword:
                    return "PartialKeyword";
                case SyntaxKind.AliasKeyword:
                    return "AliasKeyword";
                case SyntaxKind.GlobalKeyword:
                    return "GlobalKeyword";
                case SyntaxKind.AssemblyKeyword:
                    return "AssemblyKeyword";
                case SyntaxKind.ModuleKeyword:
                    return "ModuleKeyword";
                case SyntaxKind.TypeKeyword:
                    return "TypeKeyword";
                case SyntaxKind.FieldKeyword:
                    return "FieldKeyword";
                case SyntaxKind.MethodKeyword:
                    return "MethodKeyword";
                case SyntaxKind.ParamKeyword:
                    return "ParamKeyword";
                case SyntaxKind.PropertyKeyword:
                    return "PropertyKeyword";
                case SyntaxKind.TypeVarKeyword:
                    return "TypeVarKeyword";
                case SyntaxKind.GetKeyword:
                    return "GetKeyword";
                case SyntaxKind.SetKeyword:
                    return "SetKeyword";
                case SyntaxKind.AddKeyword:
                    return "AddKeyword";
                case SyntaxKind.RemoveKeyword:
                    return "RemoveKeyword";
                case SyntaxKind.WhereKeyword:
                    return "WhereKeyword";
                case SyntaxKind.FromKeyword:
                    return "FromKeyword";
                case SyntaxKind.GroupKeyword:
                    return "GroupKeyword";
                case SyntaxKind.JoinKeyword:
                    return "JoinKeyword";
                case SyntaxKind.IntoKeyword:
                    return "IntoKeyword";
                case SyntaxKind.LetKeyword:
                    return "LetKeyword";
                case SyntaxKind.ByKeyword:
                    return "ByKeyword";
                case SyntaxKind.SelectKeyword:
                    return "SelectKeyword";
                case SyntaxKind.OrderByKeyword:
                    return "OrderByKeyword";
                case SyntaxKind.OnKeyword:
                    return "OnKeyword";
                case SyntaxKind.EqualsKeyword:
                    return "EqualsKeyword";
                case SyntaxKind.AscendingKeyword:
                    return "AscendingKeyword";
                case SyntaxKind.DescendingKeyword:
                    return "DescendingKeyword";
                case SyntaxKind.NameOfKeyword:
                    return "NameOfKeyword";
                case SyntaxKind.AsyncKeyword:
                    return "AsyncKeyword";
                case SyntaxKind.AwaitKeyword:
                    return "AwaitKeyword";
                case SyntaxKind.WhenKeyword:
                    return "WhenKeyword";
                case SyntaxKind.ElifKeyword:
                    return "ElifKeyword";
                case SyntaxKind.EndIfKeyword:
                    return "EndIfKeyword";
                case SyntaxKind.RegionKeyword:
                    return "RegionKeyword";
                case SyntaxKind.EndRegionKeyword:
                    return "EndRegionKeyword";
                case SyntaxKind.DefineKeyword:
                    return "DefineKeyword";
                case SyntaxKind.UndefKeyword:
                    return "UndefKeyword";
                case SyntaxKind.WarningKeyword:
                    return "WarningKeyword";
                case SyntaxKind.ErrorKeyword:
                    return "ErrorKeyword";
                case SyntaxKind.LineKeyword:
                    return "LineKeyword";
                case SyntaxKind.PragmaKeyword:
                    return "PragmaKeyword";
                case SyntaxKind.HiddenKeyword:
                    return "HiddenKeyword";
                case SyntaxKind.ChecksumKeyword:
                    return "ChecksumKeyword";
                case SyntaxKind.DisableKeyword:
                    return "DisableKeyword";
                case SyntaxKind.RestoreKeyword:
                    return "RestoreKeyword";
                case SyntaxKind.ReferenceKeyword:
                    return "ReferenceKeyword";
                case SyntaxKind.LoadKeyword:
                    return "LoadKeyword";
                case SyntaxKind.InterpolatedStringStartToken:
                    return "InterpolatedStringStartToken";
                case SyntaxKind.InterpolatedStringEndToken:
                    return "InterpolatedStringEndToken";
                case SyntaxKind.InterpolatedVerbatimStringStartToken:
                    return "InterpolatedVerbatimStringStartToken";
                case SyntaxKind.UnderscoreToken:
                    return "UnderscoreToken";
                case SyntaxKind.OmittedTypeArgumentToken:
                    return "OmittedTypeArgumentToken";
                case SyntaxKind.OmittedArraySizeExpressionToken:
                    return "OmittedArraySizeExpressionToken";
                case SyntaxKind.EndOfDirectiveToken:
                    return "EndOfDirectiveToken";
                case SyntaxKind.EndOfDocumentationCommentToken:
                    return "EndOfDirectiveToken";
                case SyntaxKind.EndOfFileToken:
                    return "EndOfFileToken";
                case SyntaxKind.BadToken:
                    return "BadToken";
                case SyntaxKind.IdentifierToken:
                    return "IdentifierToken";
                case SyntaxKind.NumericLiteralToken:
                    return "NumericLiteralToken";
                case SyntaxKind.CharacterLiteralToken:
                    return "CharacterLiteralToken";
                case SyntaxKind.StringLiteralToken:
                    return "StringLiteralToken";
                case SyntaxKind.XmlEntityLiteralToken:
                    return "XmlEntityLiteralToken";
                case SyntaxKind.XmlTextLiteralToken:
                    return "XmlTextLiteralToken";
                case SyntaxKind.XmlTextLiteralNewLineToken:
                    return "XmlTextLiteralNewLineToken";
                case SyntaxKind.InterpolatedStringToken:
                    return "InterpolatedStringToken";
                case SyntaxKind.InterpolatedStringTextToken:
                    return "InterpolatedStringTextToken";
                case SyntaxKind.EndOfLineTrivia:
                    return "EndOfLineTrivia";
                case SyntaxKind.WhitespaceTrivia:
                    return "WhitespaceTrivia";

                case SyntaxKind.SingleLineCommentTrivia:
                    return "SingleLineCommentTrivia";
                case SyntaxKind.MultiLineCommentTrivia:
                    return "MultiLineCommentTrivia";
                case SyntaxKind.DocumentationCommentExteriorTrivia:
                    return "DocumentationCommentExteriorTrivia";
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                    return "SingleLineDocumentationCommentTrivia";
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    return "MultiLineDocumentationCommentTrivia";
                case SyntaxKind.DisabledTextTrivia:
                    return "DisabledTextTrivia";

                case SyntaxKind.PreprocessingMessageTrivia:
                    return "PreprocessingMessageTrivia";
                case SyntaxKind.IfDirectiveTrivia:
                    return "IfDirectiveTrivia";
                case SyntaxKind.ElifDirectiveTrivia:
                    return "ElifDirectiveTrivia";
                case SyntaxKind.ElseDirectiveTrivia:
                    return "ElseDirectiveTrivia";
                case SyntaxKind.EndIfDirectiveTrivia:
                    return "EndIfDirectiveTrivia";
                case SyntaxKind.RegionDirectiveTrivia:
                    return "RegionDirectiveTrivia";
                case SyntaxKind.EndRegionDirectiveTrivia:
                    return "EndRegionDirectiveTrivia";
                case SyntaxKind.DefineDirectiveTrivia:
                    return "DefineDirectiveTrivia";
                case SyntaxKind.UndefDirectiveTrivia:
                    return "UndefDirectiveTrivia";
                case SyntaxKind.ErrorDirectiveTrivia:
                    return "ErrorDirectiveTrivia";
                case SyntaxKind.WarningDirectiveTrivia:
                    return "WarningDirectiveTrivia";
                case SyntaxKind.LineDirectiveTrivia:
                    return "LineDirectiveTrivia";
                case SyntaxKind.PragmaWarningDirectiveTrivia:
                    return "PragmaWarningDirectiveTrivia";
                case SyntaxKind.PragmaChecksumDirectiveTrivia:
                    return "PragmaChecksumDirectiveTrivia";
                case SyntaxKind.ReferenceDirectiveTrivia:
                    return "ReferenceDirectiveTrivia";
                case SyntaxKind.BadDirectiveTrivia:
                    return "BadDirectiveTrivia";
                case SyntaxKind.SkippedTokensTrivia:
                    return "SkippedTokensTrivia";

                case SyntaxKind.XmlElement:
                    return "XmlElement";
                case SyntaxKind.XmlElementStartTag:
                    return "XmlElementStartTag";
                case SyntaxKind.XmlElementEndTag:
                    return "XmlElementEndTag";
                case SyntaxKind.XmlEmptyElement:
                    return "XmlEmptyElement";
                case SyntaxKind.XmlTextAttribute:
                    return "XmlTextAttribute";
                case SyntaxKind.XmlCrefAttribute:
                    return "XmlCrefAttribute";
                case SyntaxKind.XmlNameAttribute:
                    return "XmlNameAttribute";
                case SyntaxKind.XmlName:
                    return "XmlName";
                case SyntaxKind.XmlPrefix:
                    return "XmlPrefix";
                case SyntaxKind.XmlText:
                    return "XmlText";
                case SyntaxKind.XmlCDataSection:
                    return "XmlCDataSection";
                case SyntaxKind.XmlComment:
                    return "XmlComment";
                case SyntaxKind.XmlProcessingInstruction:
                    return "XmlProcessingInstruction";

                case SyntaxKind.TypeCref:
                    return "TypeCref";
                case SyntaxKind.QualifiedCref:
                    return "QualifiedCref";
                case SyntaxKind.NameMemberCref:
                    return "NameMemberCref";
                case SyntaxKind.IndexerMemberCref:
                    return "IndexerMemberCref";
                case SyntaxKind.OperatorMemberCref:
                    return "OperatorMemberCref";
                case SyntaxKind.ConversionOperatorMemberCref:
                    return "ConversionOperatorMemberCref";
                case SyntaxKind.CrefParameterList:
                    return "CrefParameterList";
                case SyntaxKind.CrefBracketedParameterList:
                    return "CrefBracketedParameterList";
                case SyntaxKind.CrefParameter:
                    return "CrefParameter";
                case SyntaxKind.IdentifierName:
                    return "IdentifierName";
                case SyntaxKind.QualifiedName:
                    return "QualifiedName";
                case SyntaxKind.GenericName:
                    return "GenericName";
                case SyntaxKind.TypeArgumentList:
                    return "TypeArgumentList";
                case SyntaxKind.AliasQualifiedName:
                    return "AliasQualifiedName";
                case SyntaxKind.PredefinedType:
                    return "PredefinedType";
                case SyntaxKind.ArrayType:
                    return "ArrayType";
                case SyntaxKind.ArrayRankSpecifier:
                    return "ArrayRankSpecifier";
                case SyntaxKind.PointerType:
                    return "PointerType";
                case SyntaxKind.NullableType:
                    return "NullableType";
                case SyntaxKind.OmittedTypeArgument:
                    return "OmittedTypeArgument";

                case SyntaxKind.ParenthesizedExpression:
                    return "ParenthesizedExpression";
                case SyntaxKind.ConditionalExpression:
                    return "ConditionalExpression";
                case SyntaxKind.InvocationExpression:
                    return "InvocationExpression";
                case SyntaxKind.ElementAccessExpression:
                    return "ElementAccessExpression";
                case SyntaxKind.ArgumentList:
                    return "ArgumentList";
                case SyntaxKind.BracketedArgumentList:
                    return "BracketedArgumentList";
                case SyntaxKind.Argument:
                    return "Argument";
                case SyntaxKind.NameColon:
                    return "NameColon";
                case SyntaxKind.CastExpression:
                    return "CastExpression";
                case SyntaxKind.AnonymousMethodExpression:
                    return "AnonymousMethodExpression";
                case SyntaxKind.SimpleLambdaExpression:
                    return "SimpleLambdaExpression";
                case SyntaxKind.ParenthesizedLambdaExpression:
                    return "ParenthesizedLambdaExpression";
                case SyntaxKind.ObjectInitializerExpression:
                    return "ObjectInitializerExpression";
                case SyntaxKind.CollectionInitializerExpression:
                    return "CollectionInitializerExpression";
                case SyntaxKind.ArrayInitializerExpression:
                    return "ArrayInitializerExpression";
                case SyntaxKind.AnonymousObjectMemberDeclarator:
                    return "AnonymousObjectMemberDeclarator";
                case SyntaxKind.ComplexElementInitializerExpression:
                    return "ComplexElementInitializerExpression";
                case SyntaxKind.ObjectCreationExpression:
                    return "ObjectCreationExpression";
                case SyntaxKind.AnonymousObjectCreationExpression:
                    return "AnonymousObjectCreationExpression";
                case SyntaxKind.ArrayCreationExpression:
                    return "ArrayCreationExpression";
                case SyntaxKind.ImplicitArrayCreationExpression:
                    return "ImplicitArrayCreationExpression";
                case SyntaxKind.StackAllocArrayCreationExpression:
                    return "StackAllocArrayCreationExpression";
                case SyntaxKind.OmittedArraySizeExpression:
                    return "OmittedArraySizeExpression";
                case SyntaxKind.InterpolatedStringExpression:
                    return "InterpolatedStringExpression";
                case SyntaxKind.ImplicitElementAccess:
                    return "ImplicitElementAccess";
                case SyntaxKind.IsPatternExpression:
                    return "IsPatternExpression";
                case SyntaxKind.AddExpression:
                    return "AddExpression";
                case SyntaxKind.SubtractExpression:
                    return "SubtractExpression";
                case SyntaxKind.MultiplyExpression:
                    return "MultiplyExpression";
                case SyntaxKind.DivideExpression:
                    return "DivideExpression";
                case SyntaxKind.ModuloExpression:
                    return "ModuloExpression";
                case SyntaxKind.LeftShiftExpression:
                    return "LeftShiftExpression";
                case SyntaxKind.RightShiftExpression:
                    return "RightShiftExpression";
                case SyntaxKind.LogicalOrExpression:
                    return "LogicalOrExpression";
                case SyntaxKind.LogicalAndExpression:
                    return "LogicalAndExpression";
                case SyntaxKind.BitwiseOrExpression:
                    return "BitwiseOrExpression";
                case SyntaxKind.BitwiseAndExpression:
                    return "BitwiseAndExpression";
                case SyntaxKind.ExclusiveOrExpression:
                    return "ExclusiveOrExpression";
                case SyntaxKind.EqualsExpression:
                    return "EqualsExpression";
                case SyntaxKind.NotEqualsExpression:
                    return "NotEqualsExpression";
                case SyntaxKind.LessThanExpression:
                    return "LessThanExpression";
                case SyntaxKind.LessThanOrEqualExpression:
                    return "LessThanOrEqualExpression";
                case SyntaxKind.GreaterThanExpression:
                    return "GreaterThanExpression";
                case SyntaxKind.GreaterThanOrEqualExpression:
                    return "GreaterThanOrEqualExpression";
                case SyntaxKind.IsExpression:
                    return "IsExpression";
                case SyntaxKind.AsExpression:
                    return "AsExpression";
                case SyntaxKind.CoalesceExpression:
                    return "CoalesceExpression";
                case SyntaxKind.SimpleMemberAccessExpression:
                    return "SimpleMemberAccessExpression";
                case SyntaxKind.PointerMemberAccessExpression:
                    return "PointerMemberAccessExpression";
                case SyntaxKind.ConditionalAccessExpression:
                    return "ConditionalAccessExpression";
                case SyntaxKind.MemberBindingExpression:
                    return "MemberBindingExpression";
                case SyntaxKind.ElementBindingExpression:
                    return "ElementBindingExpression";
                case SyntaxKind.SimpleAssignmentExpression:
                    return "SimpleAssignmentExpression";
                case SyntaxKind.AddAssignmentExpression:
                    return "AddAssignmentExpression";
                case SyntaxKind.SubtractAssignmentExpression:
                    return "SubtractAssignmentExpression";
                case SyntaxKind.MultiplyAssignmentExpression:
                    return "MultiplyAssignmentExpression";
                case SyntaxKind.DivideAssignmentExpression:
                    return "DivideAssignmentExpression";
                case SyntaxKind.ModuloAssignmentExpression:
                    return "ModuloAssignmentExpression";
                case SyntaxKind.AndAssignmentExpression:
                    return "AndAssignmentExpression";
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                    return "ExclusiveOrAssignmentExpression";
                case SyntaxKind.OrAssignmentExpression:
                    return "OrAssignmentExpression";
                case SyntaxKind.LeftShiftAssignmentExpression:
                    return "LeftShiftAssignmentExpression";
                case SyntaxKind.RightShiftAssignmentExpression:
                    return "RightShiftAssignmentExpression";
                case SyntaxKind.UnaryPlusExpression:
                    return "UnaryPlusExpression";
                case SyntaxKind.UnaryMinusExpression:
                    return "UnaryMinusExpression";
                case SyntaxKind.BitwiseNotExpression:
                    return "BitwiseNotExpression";
                case SyntaxKind.LogicalNotExpression:
                    return "LogicalNotExpression";
                case SyntaxKind.PreIncrementExpression:
                    return "PreIncrementExpression";
                case SyntaxKind.PreDecrementExpression:
                    return "PreDecrementExpression";
                case SyntaxKind.PointerIndirectionExpression:
                    return "PointerIndirectionExpression";
                case SyntaxKind.AddressOfExpression:
                    return "AddressOfExpression";
                case SyntaxKind.PostIncrementExpression:
                    return "PostIncrementExpression";
                case SyntaxKind.PostDecrementExpression:
                    return "PostDecrementExpression";
                case SyntaxKind.AwaitExpression:
                    return "AwaitExpression";
                case SyntaxKind.ThisExpression:
                    return "ThisExpression";
                case SyntaxKind.BaseExpression:
                    return "BaseExpression";
                case SyntaxKind.ArgListExpression:
                    return "ArgListExpression";
                case SyntaxKind.NumericLiteralExpression:
                    return "NumericLiteralExpression";
                case SyntaxKind.StringLiteralExpression:
                    return "StringLiteralExpression";
                case SyntaxKind.CharacterLiteralExpression:
                    return "CharacterLiteralExpression";
                case SyntaxKind.TrueLiteralExpression:
                    return "TrueLiteralExpression";
                case SyntaxKind.FalseLiteralExpression:
                    return "FalseLiteralExpression";
                case SyntaxKind.NullLiteralExpression:
                    return "NullLiteralExpression";
                case SyntaxKind.TypeOfExpression:
                    return "TypeOfExpression";
                case SyntaxKind.SizeOfExpression:
                    return "SizeOfExpression";
                case SyntaxKind.CheckedExpression:
                    return "CheckedExpression";
                case SyntaxKind.UncheckedExpression:
                    return "UncheckedExpression";
                case SyntaxKind.DefaultExpression:
                    return "DefaultExpression";
                case SyntaxKind.MakeRefExpression:
                    return "MakeRefExpression";
                case SyntaxKind.RefValueExpression:
                    return "RefValueExpression";
                case SyntaxKind.RefTypeExpression:
                    return "RefTypeExpression";
                case SyntaxKind.QueryExpression:
                    return "QueryExpression";
                case SyntaxKind.QueryBody:
                    return "QueryBody";
                case SyntaxKind.FromClause:
                    return "FromClause";
                case SyntaxKind.LetClause:
                    return "LetClause";
                case SyntaxKind.JoinClause:
                    return "JoinClause";
                case SyntaxKind.JoinIntoClause:
                    return "JoinIntoClause";
                case SyntaxKind.WhereClause:
                    return "WhereClause";
                case SyntaxKind.OrderByClause:
                    return "OrderByClause";
                case SyntaxKind.AscendingOrdering:
                    return "AscendingOrdering";
                case SyntaxKind.DescendingOrdering:
                    return "DescendingOrdering";
                case SyntaxKind.SelectClause:
                    return "SelectClause";
                case SyntaxKind.GroupClause:
                    return "GroupClause";
                case SyntaxKind.QueryContinuation:
                    return "QueryContinuation";
                case SyntaxKind.Block:
                    return "Block";
                case SyntaxKind.LocalDeclarationStatement:
                    return "LocalDeclarationStatement";
                case SyntaxKind.VariableDeclaration:
                    return "VariableDeclaration";
                case SyntaxKind.VariableDeclarator:
                    return "VariableDeclarator";
                case SyntaxKind.EqualsValueClause:
                    return "EqualsValueClause";
                case SyntaxKind.ExpressionStatement:
                    return "ExpressionStatement";
                case SyntaxKind.EmptyStatement:
                    return "EmptyStatement";
                case SyntaxKind.LabeledStatement:
                    return "LabeledStatement";
                case SyntaxKind.GotoStatement:
                    return "GotoStatement";
                case SyntaxKind.GotoCaseStatement:
                    return "GotoCaseStatement";
                case SyntaxKind.GotoDefaultStatement:
                    return "GotoDefaultStatement";
                case SyntaxKind.BreakStatement:
                    return "BreakStatement";
                case SyntaxKind.ContinueStatement:
                    return "ContinueStatement";
                case SyntaxKind.ReturnStatement:
                    return "ReturnStatement";
                case SyntaxKind.YieldReturnStatement:
                    return "YieldReturnStatement";
                case SyntaxKind.YieldBreakStatement:
                    return "YieldBreakStatement";
                case SyntaxKind.ThrowStatement:
                    return "ThrowStatement";
                case SyntaxKind.WhileStatement:
                    return "WhileStatement";
                case SyntaxKind.DoStatement:
                    return "DoStatement";
                case SyntaxKind.ForStatement:
                    return "ForStatement";
                case SyntaxKind.ForEachStatement:
                    return "ForEachStatement";
                case SyntaxKind.UsingStatement:
                    return "UsingStatement";
                case SyntaxKind.FixedStatement:
                    return "FixedStatement";
                case SyntaxKind.CheckedStatement:
                    return "CheckedStatement";
                case SyntaxKind.UncheckedStatement:
                    return "UncheckedStatement";
                case SyntaxKind.UnsafeStatement:
                    return "UnsafeStatement";
                case SyntaxKind.LockStatement:
                    return "LockStatement";
                case SyntaxKind.IfStatement:
                    return "IfStatement";
                case SyntaxKind.ElseClause:
                    return "ElseClause";
                case SyntaxKind.SwitchStatement:
                    return "SwitchStatement";
                case SyntaxKind.SwitchSection:
                    return "SwitchSection";
                case SyntaxKind.CaseSwitchLabel:
                    return "CaseSwitchLabel";
                case SyntaxKind.DefaultSwitchLabel:
                    return "DefaultSwitchLabel";
                case SyntaxKind.TryStatement:
                    return "TryStatement";
                case SyntaxKind.CatchClause:
                    return "CatchClause";
                case SyntaxKind.CatchDeclaration:
                    return "CatchDeclaration";
                case SyntaxKind.CatchFilterClause:
                    return "CatchFilterClause";
                case SyntaxKind.FinallyClause:
                    return "FinallyClause";
                case SyntaxKind.LocalFunctionStatement:
                    return "LocalFunctionStatement";
                case SyntaxKind.CompilationUnit:
                    return "CompilationUnit";
                case SyntaxKind.GlobalStatement:
                    return "GlobalStatement";
                case SyntaxKind.NamespaceDeclaration:
                    return "NamespaceDeclaration";
                case SyntaxKind.UsingDirective:
                    return "UsingDirective";
                case SyntaxKind.ExternAliasDirective:
                    return "ExternAliasDirective";
                case SyntaxKind.AttributeList:
                    return "AttributeList";
                case SyntaxKind.AttributeTargetSpecifier:
                    return "AttributeTargetSpecifier";
                case SyntaxKind.Attribute:
                    return "Attribute";
                case SyntaxKind.AttributeArgumentList:
                    return "AttributeArgumentList";
                case SyntaxKind.AttributeArgument:
                    return "AttributeArgument";
                case SyntaxKind.NameEquals:
                    return "NameEquals";
                case SyntaxKind.ClassDeclaration:
                    return "ClassDeclaration";
                case SyntaxKind.StructDeclaration:
                    return "StructDeclaration";
                case SyntaxKind.InterfaceDeclaration:
                    return "InterfaceDeclaration";
                case SyntaxKind.EnumDeclaration:
                    return "EnumDeclaration";
                case SyntaxKind.DelegateDeclaration:
                    return "DelegateDeclaration";
                case SyntaxKind.BaseList:
                    return "BaseList";
                case SyntaxKind.SimpleBaseType:
                    return "SimpleBaseType";
                case SyntaxKind.TypeParameterConstraintClause:
                    return "TypeParameterConstraintClause";
                case SyntaxKind.ConstructorConstraint:
                    return "ConstructorConstraint";
                case SyntaxKind.ClassConstraint:
                    return "ClassConstraint";
                case SyntaxKind.StructConstraint:
                    return "StructConstraint";
                case SyntaxKind.TypeConstraint:
                    return "TypeConstraint";
                case SyntaxKind.ExplicitInterfaceSpecifier:
                    return "ExplicitInterfaceSpecifier";
                case SyntaxKind.EnumMemberDeclaration:
                    return "EnumMemberDeclaration";
                case SyntaxKind.FieldDeclaration:
                    return "FieldDeclaration";
                case SyntaxKind.EventFieldDeclaration:
                    return "EventFieldDeclaration";
                case SyntaxKind.MethodDeclaration:
                    return "MethodDeclaration";
                case SyntaxKind.OperatorDeclaration:
                    return "OperatorDeclaration";
                case SyntaxKind.ConversionOperatorDeclaration:
                    return "ConversionOperatorDeclaration";
                case SyntaxKind.ConstructorDeclaration:
                    return "ConstructorDeclaration";
                case SyntaxKind.BaseConstructorInitializer:
                    return "BaseConstructorInitializer";
                case SyntaxKind.ThisConstructorInitializer:
                    return "ThisConstructorInitializer";
                case SyntaxKind.DestructorDeclaration:
                    return "DestructorDeclaration";
                case SyntaxKind.PropertyDeclaration:
                    return "PropertyDeclaration";
                case SyntaxKind.EventDeclaration:
                    return "EventDeclaration";
                case SyntaxKind.IndexerDeclaration:
                    return "IndexerDeclaration";
                case SyntaxKind.AccessorList:
                    return "AccessorList";
                case SyntaxKind.GetAccessorDeclaration:
                    return "GetAccessorDeclaration";
                case SyntaxKind.SetAccessorDeclaration:
                    return "SetAccessorDeclaration";
                case SyntaxKind.AddAccessorDeclaration:
                    return "AddAccessorDeclaration";
                case SyntaxKind.RemoveAccessorDeclaration:
                    return "RemoveAccessorDeclaration";
                case SyntaxKind.UnknownAccessorDeclaration:
                    return "UnknownAccessorDeclaration";
                case SyntaxKind.ParameterList:
                    return "ParameterList";
                case SyntaxKind.BracketedParameterList:
                    return "BracketedParameterList";
                case SyntaxKind.Parameter:
                    return "Parameter";
                case SyntaxKind.TypeParameterList:
                    return "TypeParameterList";
                case SyntaxKind.TypeParameter:
                    return "TypeParameter";
                case SyntaxKind.IncompleteMember:
                    return "IncompleteMember";
                case SyntaxKind.ArrowExpressionClause:
                    return "ArrowExpressionClause";
                case SyntaxKind.Interpolation:
                    return "Interpolation";
                case SyntaxKind.InterpolatedStringText:
                    return "InterpolatedStringText";
                case SyntaxKind.InterpolationAlignmentClause:
                    return "InterpolationAlignmentClause";
                case SyntaxKind.InterpolationFormatClause:
                    return "InterpolationFormatClause";
                case SyntaxKind.ShebangDirectiveTrivia:
                    return "ShebangDirectiveTrivia";
                case SyntaxKind.LoadDirectiveTrivia:
                    return "LoadDirectiveTrivia";
                case SyntaxKind.TupleType:
                    return "TupleType";
                case SyntaxKind.TupleElement:
                    return "TupleElement";
                case SyntaxKind.TupleExpression:
                    return "TupleExpression";
                case SyntaxKind.SingleVariableDesignation:
                    return "SingleVariableDesignation";
                case SyntaxKind.ParenthesizedVariableDesignation:
                    return "ParenthesizedVariableDesignation";
                case SyntaxKind.ForEachVariableStatement:
                    return "ForEachVariableStatement";
                case SyntaxKind.DeclarationPattern:
                    return "DeclarationPattern";
                case SyntaxKind.ConstantPattern:
                    return "ConstantPattern";
                case SyntaxKind.CasePatternSwitchLabel:
                    return "CasePatternSwitchLabel";
                case SyntaxKind.WhenClause:
                    return "WhenClause";
                case SyntaxKind.DiscardDesignation:
                    return "DiscardDesignation";
                case SyntaxKind.DeclarationExpression:
                    return "DeclarationExpression";
                case SyntaxKind.RefExpression:
                    return "RefExpression";
                case SyntaxKind.RefType:
                    return "RefType";
                case SyntaxKind.ThrowExpression:
                    return "ThrowExpression";
            }
            return "";
        }

        public static string kindStr(SymbolKind _kind)
        {
            switch (_kind)
            {
                case SymbolKind.Alias:
                    return "Alias";
                case SymbolKind.ArrayType:
                    return "ArrayType";
                case SymbolKind.Assembly:
                    return "Assembly";
                case SymbolKind.DynamicType:
                    return "DynamicType";
                case SymbolKind.ErrorType:
                    return "ErrorType";
                case SymbolKind.Event:
                    return "Event";
                case SymbolKind.Field:
                    return "Field";
                case SymbolKind.Label:
                    return "Label";
                case SymbolKind.Local:
                    return "Local";
                case SymbolKind.Method:
                    return "Method";
                case SymbolKind.NetModule:
                    return "NetModule";
                case SymbolKind.NamedType:
                    return "NamedType";
                case SymbolKind.Namespace:
                    return "Namespace";
                case SymbolKind.Parameter:
                    return "Parameter";
                case SymbolKind.PointerType:
                    return "PointerType";
                case SymbolKind.Property:
                    return "Property";
                case SymbolKind.RangeVariable:
                    return "RangeVariable";
                case SymbolKind.TypeParameter:
                    return "TypeParameter";
                case SymbolKind.Preprocessing:
                    return "Preprocessing";
                case SymbolKind.Discard:
                    return "Discard";
                default:
                    return "";
            }
        }
    }
}
