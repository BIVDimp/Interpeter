using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
    public enum TokenType
    {
        Unknown,
        WrongStringLiteral,

        SingleLineComment,
        WhiteSpace,

        Number,
        RegularStringLiteral,
        VerbatimStringLiteral,
        Identifier,
        Plus,
        Minus,
        Multiply,
        Divide,
        Degree,
        Assignment,
        Equal,
        NotEqual,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
        OpenParenthesis,
        CloseParenthesis,
        OpenCurlyBrace,
        CloseCurlyBrace,
        OpenBracket,
        CloseBracket,
        Comma,
        New,
        If,
        Else,
        For,
        While,
        Do,
        Goto,
        Semicolon,
        Colon,
        Return,
        Break,
        EOF
    }

    public struct Token
    {
        public readonly TokenType Type;
        public readonly string Name;
        public readonly Position Position;

        public Token(TokenType type, Position position)
            : this(type, default(string), position)
        {
        }

        public Token(TokenType type, string name, Position position)
        {
            Type = type;
            Name = name;
            Position = position;
        }

        public bool Equals(Token token)
        {
            return Type == token.Type && string.Equals(Name, token.Name);
        }
    }
}