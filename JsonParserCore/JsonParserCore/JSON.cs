using System;
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

    public struct BoolLiteral : Content
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

    public struct DoubleLiteral : Content
    {
        public double Value
        {
            get; set;
        }

        public static implicit operator double(DoubleLiteral literal)
        {
            return literal.Value;
        }
        public static implicit operator DoubleLiteral(double literal)
        {
            return new DoubleLiteral(literal);
        }

        public string ToJSON()
        {
            return Value.ToString();
        }

        public DoubleLiteral(double value)
        {
            Value = value;
        }
    }

    public struct StringLiteral : Content
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

    public class JSONArray : Content
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

        public JSONArray(Content[]? values = null)
        {
            Values = new List<Content>();

            if (values != null)
            {
                Values.AddRange(values);
            }
        }
    }

    public class JSONObject : Content
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

        public JSONObject()
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

    public static class JSON
    {
        private static readonly char[] ControlCharacters = new char[] { '{', '}', '[', ']', ',', '"', ':' };
        private static readonly char[] IgnoreCharacters = new char[] { ' ' };
        private static IReadOnlyDictionary<char, string> EscapeSymbols = new Dictionary<char, string>(new KeyValuePair<char, string>[] { new KeyValuePair<char, string>('"', "\\\"") });

        private static Token[] Tokenize(string input)
        {
            bool openedString = false;
            StringBuilder literal = new();

            //init control tokens
            (int index, char value, TokenType type) currentControlToken = (-1, '\0', TokenType.none);
            (int index, char value, TokenType type) nextControlToken = (-1, '\0', TokenType.none);

            List<Token> tokens = new();

            for (int index = 0; index < input.Length; index++)
            {
                var currentChar = input[index];
                var isQuot = IsQuot(currentChar);

                //if the symbol is control token (not an escape) and string isn't opened or the symbol is control token and it's quotation mark then current char is added to tokens list
                if (IsControlSymbol(input, index) && (!openedString || isQuot))
                {
                    var newControlToken = (index, currentChar, isQuot ? (openedString ? TokenType.quotEnd : TokenType.quotStart) : GetControlSymbolType(currentChar));

                    nextControlToken = newControlToken;

                    //if literal isn't empty or it's in quotes it is added to tokens list
                    if (currentControlToken.index != -1 && (literal.Length > 0 || openedString))
                    {
                        //function gets literal type
                        tokens.Add(LiteralToToken(literal.ToString(), currentControlToken.index + 1, openedString));

                        literal.Clear();
                    }

                    currentControlToken = newControlToken;

                    //if control token is quotation mark then string opened flag switches to opposite state
                    if (isQuot)
                    {
                        openedString = !openedString;
                    }

                    tokens.Add(new Token(currentControlToken.value.ToString(), currentControlToken.type, currentControlToken.index));
                }
                else
                {
                    //if current char is included to ignore symbols list it is skipped
                    if (!IsIgnoreSymbol(currentChar) || openedString)
                    {
                        literal.Append(currentChar);
                    }
                }
            }

            //string hasn't closing quotation mark
            if (openedString)
            {
                throw new Exception("Unterminated string");
            }

            return tokens.ToArray();
        }
        private static bool IsIgnoreSymbol(char symbol)
        {
            return Array.IndexOf(IgnoreCharacters, symbol) != -1;
        }
        private static bool IsQuot(char symbol)
        {
            return symbol == '"';
        }
        private static bool IsEscapeSymbol(string input, int index)
        {
            char selectedChar = input[index];

            if (EscapeSymbols.ContainsKey(selectedChar))
            {
                //code snippet gets control symbol context (is it escaping or not)
                string escapeSymbol = EscapeSymbols[selectedChar];
                int indexInString = escapeSymbol.IndexOf(selectedChar);
                string stringSample = input.Substring(index - indexInString, escapeSymbol.Length);

                if (stringSample == escapeSymbol)
                {
                    return true;
                }
            }

            return false;
        }
        private static bool IsControlSymbol(string input, int index)
        {
            char selectedChar = input[index];

            return Array.FindIndex(ControlCharacters, (char value) => { return value == selectedChar; }) != -1 && !IsEscapeSymbol(input, index);
        }
        private static Token LiteralToToken(string literal, int literalStartIndex, bool openedString)
        {
            TokenType literalTokenType;

            if (openedString)
            {
                literalTokenType = TokenType.text;
            }
            else
            {
                TryGetLiteralType(literal, out literalTokenType);
            }

            return new Token(literal, literalTokenType, literalStartIndex);
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
            if (double.TryParse(token, out _))
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
                            throw new Exception("Unhandled case (internal error)");
                    }
                    break;
            }

            throw new Exception("Incorrect input (internal error)");
        }

        private static (JSONArray array, int end) ParseArray(Token[] tokens, int startIndex)
        {
            JSONArray array = new();
            int end = -1;

            Action<int> validateInput = (int index) =>
            {
                var validTokens = GetValidTokenTypes(tokens[index].Type, Context.arrayElement, null);

                if (!validTokens.Contains(index + 1 < tokens.Length ? tokens[index + 1].Type : TokenType.docEnd))
                {
                    throw new Exception($"Expected on of those tokens: {String.Join(", ", validTokens)}");
                }
            };

            if (tokens[startIndex].Type == TokenType.arrayStart)
            {
                validateInput(startIndex);

                for (int index = startIndex + 1; index < tokens.Length && end == -1; index++)
                {
                    int offset = 0;

                    switch (tokens[index].Type)
                    {
                        case TokenType.number:
                            array.Add((DoubleLiteral)double.Parse(tokens[index].Value));
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
                            offset = 1;
                            break;
                        case TokenType.start:
                            var parsedObject = JSON.ParseObject(tokens, index);

                            array.Add(parsedObject.objectElement);
                            index = parsedObject.end;
                            offset = 1;
                            break;
                        case TokenType.arrayEnd:
                            end = index;
                            break;
                    }

                    validateInput(index + offset);
                }

                if (end == -1)
                {
                    throw new Exception("Unterminated array (internal error)");
                }
            }
            else
            {
                throw new Exception("StartIndex doesn't represent array start");
            }

            return (array, end);
        }
        private static (JSONObject objectElement, int end) ParseObject(Token[] tokens, int startIndex)
        {
            JSONObject objectElement = new();
            int end = -1;
            string? openedFieldName = null;

            Action<int> validateInput = (int index) =>
            {
                var validTokens = GetValidTokenTypes(tokens[index].Type, Context.objectElement, openedFieldName != null);

                if (!validTokens.Contains(index + 1 < tokens.Length ? tokens[index + 1].Type : TokenType.docEnd))
                {
                    throw new Exception($"Expected on of those tokens: {String.Join(", ", validTokens)}");
                }
            };

            if (tokens[startIndex].Type == TokenType.start)
            {
                validateInput(startIndex);

                for (int index = startIndex + 1; index < tokens.Length && end == -1; index++)
                {
                    int offset = 0;

                    switch (tokens[index].Type)
                    {
                        case TokenType.number:
                            objectElement.DefineProperty(openedFieldName!, (DoubleLiteral)double.Parse(tokens[index].Value));
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
                            offset = 1;
                            break;
                        case TokenType.start:
                            var parsedObject = JSON.ParseObject(tokens, index);

                            objectElement.DefineProperty(openedFieldName!, parsedObject.objectElement);
                            index = parsedObject.end;
                            openedFieldName = null;
                            offset = 1;
                            break;
                        case TokenType.end:
                            end = index;
                            break;
                    }

                    validateInput(index + offset);
                }

                if (end == -1)
                {
                    throw new Exception("Unterminated object");
                }
            }
            else
            {
                throw new Exception("StartIndex doesn't represent object start (internal error)");
            }

            return (objectElement, end);
        }
        private static Content ParseTokens(Token[] tokens)
        {
            Token startToken = tokens[0];

            if (startToken.Type == TokenType.start)
            {
                return ParseObject(tokens, 0).objectElement;
            }
            else if (startToken.Type == TokenType.arrayStart)
            {
                return ParseArray(tokens, 0).array;
            }
            else
            {
                throw new Exception("Incorect open token");
            }
        }
        public static Content Parse(string json)
        {
            return ParseTokens(Tokenize(json.Replace("\n", "")));
        }

        private static bool IsNumber(this object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }
        private static Content? AsLiteral(object obj)
        {
            if (obj.IsNumber())
            {
                return (DoubleLiteral)Convert.ToDouble(obj);
            }
            else
            {
                switch (obj)
                {
                    case bool boolLiteral:
                        return (BoolLiteral)boolLiteral;
                    case string stringLiteral:
                        return (StringLiteral)stringLiteral;
                    case char charLiteral:
                        return (StringLiteral)charLiteral.ToString();
                    default:
                        return null;
                }
            }
        }
        private static JSONObject DOMObject(object obj)
        {
            var objectElement = new JSONObject();
            var type = obj.GetType();

            foreach (var field in type.GetFields())
            {
                var value = field.GetValue(obj);

                if (value != null)
                {
                    var asLiteral = AsLiteral(value);

                    if (asLiteral != null)
                    {
                        objectElement.DefineProperty(field.Name, asLiteral);
                    }
                    else if (value is Array)
                    {
                        objectElement.DefineProperty(field.Name, DOMArray((Array)value));
                    }
                    else
                    {
                        objectElement.DefineProperty(field.Name, DOMObject(value));
                    }
                }
            }

            return objectElement;
        }
        private static JSONArray DOMArray(Array objects)
        {
            var array = new JSONArray();

            foreach (var obj in objects)
            {
                if (obj != null)
                {
                    var asLiteral = AsLiteral(obj);

                    if (asLiteral != null)
                    {
                        array.Add(asLiteral);
                    }
                    else if (obj is Array)
                    {
                        array.Add(DOMArray((Array)obj));
                    }
                    else
                    {
                        array.Add(DOMObject(obj));
                    }
                }
            }

            return array;
        }
        public static string Stringify(object obj)
        {
            return DOMObject(obj).ToJSON();
        }

        static JSON()
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
        }
    }
}
