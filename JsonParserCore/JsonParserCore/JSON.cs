﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONParser
{
    public interface Content
    {
        public string ToJSON();
    }

    struct BoolLiteral : Content
    {
        public bool Value
        {
            get; set;
        }

        public static implicit operator bool(BoolLiteral literal)
        {
            return literal.Value;
        }
        public static implicit operator BoolLiteral(bool literal)
        {
            return new BoolLiteral(literal);
        }

        public string ToJSON()
        {
            return Value ? "true" : "false";
        }

        public BoolLiteral(bool value)
        {
            Value = value;
        }
    }

    struct IntLiteral : Content
    {
        public int Value
        {
            get; set;
        }

        public static implicit operator int(IntLiteral literal)
        {
            return literal.Value;
        }
        public static implicit operator IntLiteral(int literal)
        {
            return new IntLiteral(literal);
        }

        public string ToJSON()
        {
            return Value.ToString();
        }

        public IntLiteral(int value)
        {
            Value = value;
        }
    }

    struct StringLiteral : Content
    {
        public string Value
        {
            get; set;
        }

        public static implicit operator string(StringLiteral literal)
        {
            return literal.Value;
        }
        public static implicit operator StringLiteral(string literal)
        {
            return new StringLiteral(literal);
        }

        public string ToJSON()
        {
            return "\"" + Value.ToString() + "\"";
        }

        public StringLiteral(string value)
        {
            Value = value;
        }
    }

    class Token
    {
        public string Value
        {
            get; set;
        }
        public TokenType Type
        {
            get; set;
        }
        public readonly int Position;

        public Token(string token, TokenType type, int position)
        {
            Value = token;
            Type = type;
            Position = position;
        }
    }

    class JSONArray : Content
    {
        private List<Content> Values
        {
            get; set;
        }

        public void Add(Content literal)
        {
            Values.Add(literal);
        }

        public string ToJSON()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append('[');

            for (int index = 0; index < Values.Count; index++)
            {
                stringBuilder.Append(Values[index].ToJSON());

                if (index != Values.Count - 1)
                {
                    stringBuilder.Append(',');
                }
            }

            stringBuilder.Append(']');

            return stringBuilder.ToString();
        }

        public JSONArray()
        {
            Values = new List<Content>();
        }
    }

    class Object : Content
    {
        private Dictionary<StringLiteral, Content> Fields
        {
            get; set;
        }

        public Content[] Values
        {
            get
            {
                return Fields.Values.ToArray();
            }
        }
        public StringLiteral[] Keys
        {
            get
            {
                return Fields.Keys.ToArray();
            }
        }
        public int Count
        {
            get
            {
                return Fields.Count;
            }
        }

        public void DefineProperty(string key, Content value)
        {
            if (Fields.ContainsKey(key))
            {
                Fields[key] = value;
            }
            else
            {
                Fields.Add(key, value);
            }
        }

        public string ToJSON()
        {
            var stringBuilder = new StringBuilder();
            var fields = new List<(StringLiteral key, Content value)>();

            foreach (var field in Fields)
            {
                fields.Add((field.Key, field.Value));
            }

            stringBuilder.Append('{');

            for (int index = 0; index < fields.Count; index++)
            {
                var (key, value) = fields[index];

                stringBuilder.Append(key.ToJSON());
                stringBuilder.Append(':');
                stringBuilder.Append(value.ToJSON());

                if (index != fields.Count - 1)
                {
                    stringBuilder.Append(',');
                }
            }

            stringBuilder.Append('}');

            return stringBuilder.ToString();
        }

        public Object()
        {
            Fields = new();
        }
    }

    enum TokenType
    {
        none,
        number,
        boolean,
        text,
        start,
        end,
        arrayStart,
        arrayEnd,
        quotStart,
        quotEnd,
        splitter,
        equal,
        docEnd
    }

    enum Context
    {
        objectElement, arrayElement
    }

    public class JSON
    {
        private static readonly char[] ControlCharacters = new char[] { '{', '}', '[', ']', ',', '"', ':' };
        private static IReadOnlyDictionary<char, string> EscapeSymbols = new Dictionary<char, string>(new KeyValuePair<char, string>[] { new KeyValuePair<char, string>('"', "\\\"") });

        private static Token[] Tokenize(string input)
        {
            bool openedString = false;
            (int index, char value, TokenType type) currentControlToken = (-1, '\0', TokenType.none);
            (int index, char value, TokenType type) nextControlToken = (-1, '\0', TokenType.none);

            List<Token> tokens = new();

            for (int index = 0; index < input.Length; index++)
            {
                var currentChar = input[index];
                var isQuot = currentChar == '"';

                if (IsControlSymbol(input, index) && (!openedString || isQuot))
                {
                    nextControlToken = (index, currentChar, isQuot ? (openedString ? TokenType.quotEnd : TokenType.quotStart) : GetControlSymbolType(currentChar));

                    int length = index - currentControlToken.index - 1;
                    int literalStart = currentControlToken.index + 1;

                    if (currentControlToken.index != -1 && (length > 0 || openedString))
                    {
                        string literal = input.Substring(literalStart, length);
                        TokenType literalTokenType;

                        if (openedString)
                        {
                            literalTokenType = TokenType.text;
                        }
                        else
                        {
                            TryGetLiteralType(literal, out literalTokenType);
                        }

                        tokens.Add(new Token(literal, literalTokenType, literalStart));
                    }

                    currentControlToken.index = nextControlToken.index;
                    currentControlToken.value = nextControlToken.value;
                    currentControlToken.type = nextControlToken.type;

                    if (isQuot)
                    {
                        openedString = !openedString;
                    }

                    tokens.Add(new Token(currentControlToken.value.ToString(), currentControlToken.type, currentControlToken.index));
                }
            }

            if (openedString)
            {
                throw new Exception("Unterminated string");
            }

            return tokens.ToArray();
        }
        private static bool IsControlSymbol(string input, int index)
        {
            char selectedChar = input[index];

            if (EscapeSymbols.ContainsKey(selectedChar))
            {
                string escapeSymbol = EscapeSymbols[selectedChar];
                int indexInString = escapeSymbol.IndexOf(selectedChar);
                string stringSample = input.Substring(index - indexInString, escapeSymbol.Length);

                if (stringSample == escapeSymbol)
                {
                    return false;
                }
            }

            return Array.FindIndex(ControlCharacters, (char value) => { return value == selectedChar; }) != -1;
        }
        private static TokenType GetControlSymbolType(char token)
        {
            var tokenType = token switch
            {
                '{' => TokenType.start,
                '}' => TokenType.end,
                '[' => TokenType.arrayStart,
                ']' => TokenType.arrayEnd,
                ',' => TokenType.splitter,
                ':' => TokenType.equal,
                _ => throw new Exception("Unhandled symbol"),
            };
            return tokenType;
        }
        private static bool TryGetLiteralType(string token, out TokenType type)
        {
            if (int.TryParse(token, out _))
            {
                type = TokenType.number;
                return true;
            }
            else if (bool.TryParse(token, out _))
            {
                type = TokenType.boolean;
                return true;
            }
            else
            {
                type = TokenType.none;
                return false;
            }
        }
        private static TokenType[] GetValidTokenTypes(TokenType type, Context context, bool? fieldName)
        {
            var contentTokenTypes = new TokenType[] { TokenType.start, TokenType.boolean, TokenType.number, TokenType.quotStart, TokenType.arrayStart };

            switch (type)
            {
                case TokenType.text:
                    return new TokenType[] { TokenType.quotEnd };
                case TokenType.quotStart:
                    return new TokenType[] { TokenType.text };
                default:
                    switch (context)
                    {
                        case Context.objectElement:
                            switch (type)
                            {
                                case TokenType.number:
                                case TokenType.boolean:
                                    return new TokenType[] { TokenType.splitter, TokenType.end };
                                case TokenType.start:
                                    return new TokenType[] { TokenType.quotStart, TokenType.end };
                                case TokenType.end:
                                    return new TokenType[] { TokenType.splitter, TokenType.arrayEnd, TokenType.end, TokenType.docEnd };
                                case TokenType.quotEnd:
                                    if (fieldName.HasValue && fieldName.Value)
                                    {
                                        return new TokenType[] { TokenType.equal };
                                    }
                                    else
                                    {
                                        return new TokenType[] { TokenType.splitter, TokenType.end };
                                    }
                                case TokenType.splitter:
                                    return new TokenType[] { TokenType.quotStart };
                                case TokenType.equal:
                                    return contentTokenTypes;
                            }
                            break;
                        case Context.arrayElement:
                            switch (type)
                            {
                                case TokenType.number:
                                case TokenType.boolean:
                                    return new TokenType[] { TokenType.splitter, TokenType.arrayEnd };
                                case TokenType.arrayStart:
                                    return contentTokenTypes.Concat(new TokenType[] { TokenType.arrayEnd }).ToArray();
                                case TokenType.arrayEnd:
                                    return new TokenType[] { TokenType.splitter, TokenType.arrayEnd, TokenType.end, TokenType.docEnd };
                                case TokenType.quotEnd:
                                    if (!fieldName.HasValue)
                                    {
                                        return new TokenType[] { TokenType.splitter, TokenType.arrayEnd };
                                    }
                                    break;
                                case TokenType.splitter:
                                    return contentTokenTypes;
                            }
                            break;
                        default:
                            throw new Exception("Unhandled case");
                    }
                    break;
            }

            throw new Exception("Incorrect input");
        }

        private static (JSONArray array, int end) ParseArray(Token[] tokens, int startIndex)
        {
            JSONArray array = new();
            int end = -1;

            if (tokens[startIndex].Type == TokenType.arrayStart)
            {
                for (int index = startIndex + 1; index < tokens.Length && end == -1; index++)
                {
                    bool validate = true;

                    switch (tokens[index].Type)
                    {
                        case TokenType.number:
                            array.Add((IntLiteral)int.Parse(tokens[index].Value));
                            break;
                        case TokenType.boolean:
                            array.Add((BoolLiteral)bool.Parse(tokens[index].Value));
                            break;
                        case TokenType.text:
                            array.Add((StringLiteral)tokens[index].Value);
                            break;
                        case TokenType.arrayStart:
                            var parsedArray = JSON.ParseArray(tokens, index);

                            array.Add(parsedArray.array);
                            index = parsedArray.end;
                            validate = false;
                            break;
                        case TokenType.start:
                            var parsedObject = JSON.ParseObject(tokens, index);

                            array.Add(parsedObject.objectElement);
                            index = parsedObject.end;
                            validate = false;
                            break;
                        case TokenType.arrayEnd:
                            end = index;
                            break;
                        default:
                            break;
                    }

                    if (validate)
                    {
                        var validTokens = GetValidTokenTypes(tokens[index].Type, Context.arrayElement, null);

                        if (!validTokens.Contains(index + 1 < tokens.Length ? tokens[index + 1].Type : TokenType.docEnd))
                        {
                            throw new Exception($"Expected on of those tokens: {String.Join(", ", validTokens)}");
                        }
                    }
                }

                if (end == -1)
                {
                    throw new Exception("Unterminated array");
                }
            }
            else
            {
                throw new Exception("StartIndex doesn't represent array start");
            }

            return (array, end);
        }
        private static (Object objectElement, int end) ParseObject(Token[] tokens, int startIndex)
        {
            Object objectElement = new();
            int end = -1;
            string? openedFieldName = null;

            if (tokens[startIndex].Type == TokenType.start)
            {
                for (int index = startIndex + 1; index < tokens.Length && end == -1; index++)
                {
                    bool validate = true;

                    switch (tokens[index].Type)
                    {
                        case TokenType.number:
                            objectElement.DefineProperty(openedFieldName!, (IntLiteral)int.Parse(tokens[index].Value));
                            openedFieldName = null;
                            break;
                        case TokenType.boolean:
                            objectElement.DefineProperty(openedFieldName!, (BoolLiteral)bool.Parse(tokens[index].Value));
                            openedFieldName = null;
                            break;
                        case TokenType.text:
                            if (openedFieldName != null)
                            {
                                objectElement.DefineProperty(openedFieldName, (StringLiteral)tokens[index].Value);
                                openedFieldName = null;
                            }
                            else
                            {
                                openedFieldName = tokens[index].Value;
                            }
                            break;
                        case TokenType.arrayStart:
                            var parsedArray = JSON.ParseArray(tokens, index);

                            objectElement.DefineProperty(openedFieldName!, parsedArray.array);
                            index = parsedArray.end;
                            openedFieldName = null;
                            validate = false;
                            break;
                        case TokenType.start:
                            var parsedObject = JSON.ParseObject(tokens, index);

                            objectElement.DefineProperty(openedFieldName!, parsedObject.objectElement);
                            index = parsedObject.end;
                            openedFieldName = null;
                            validate = false;
                            break;
                        case TokenType.end:
                            end = index;
                            break;
                        default:
                            break;
                    }

                    if (validate)
                    {
                        var validTokens = GetValidTokenTypes(tokens[index].Type, Context.objectElement, openedFieldName != null);

                        if (!validTokens.Contains(index + 1 < tokens.Length ? tokens[index + 1].Type : TokenType.docEnd))
                        {
                            throw new Exception($"Expected on of those tokens: {String.Join(", ", validTokens)}");
                        }
                    }
                }

                if (end == -1)
                {
                    throw new Exception("Unterminated object");
                }
            }
            else
            {
                throw new Exception("StartIndex doesn't represent object start");
            }

            return (objectElement, end);
        }
        private static Content[] ParseTokens(Token[] tokens)
        {
            List<Content> result = new();
            Token startToken = tokens[0];

            if (startToken.Type == TokenType.start)
            {
                result.Add(ParseObject(tokens, 0).objectElement);
            }
            else if (startToken.Type == TokenType.arrayStart)
            {
                result.Add(ParseArray(tokens, 0).array);
            }
            else
            {
                throw new Exception("Incorect open token");
            }

            return result.ToArray();
        }
        public static Content[] Parse(string json)
        {
            return ParseTokens(Tokenize(json.Replace("\n", "").Replace(" ", "")));
        }
    }
}