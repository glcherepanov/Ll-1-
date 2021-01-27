using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Lexer
{
    public enum Tokens : int
    {
        Null,
        Private,
        Public,
        Void,
        Var,
        Class,
        [StringValue("int")]
        Int,
        Double,
        Bool,
        Char,
        String,
        If,
        Else,
        While,
        For,
        Read,
        Write,
        [StringValue("id")]
        Identificator,
        StringParameter,
        Float,
        Binary,
        Hex,
        NotEqual,
        Error,
        Assignment,
        Comparison,
        [StringValue("+")]
        Add,
        [StringValue("-")]
        Sub,
        [StringValue("*")]
        Mult,
        [StringValue("/")]
        Div,
        LineComment,
        Comment,
        Pow,
        [StringValue("(")]
        OpenParenthesis,
        [StringValue(")")]
        CloseParenthesis,
        OpenBrace,
        CloseBrace,
        OpenBrackets,
        CloseBrackets,
        LogicalNegation,
        LogicalAnd,
        LogicalOr,
        ConditionalAnd,
        ConditionalOr,
        Dot,
        Comma,
        Colon,
        Semicolon,
        SmallOrEqual,
        Smaller,
        MoreOrEqual,
        More,
        Space,
        [StringValue("")]
        EndOfFile
    }

    public class StringValueAttribute : Attribute
    {
        public string StringValue { get; protected set; }
        public StringValueAttribute(string value)
        {
            this.StringValue = value;
        }

    }
    public class Functions
    {
        public static string GetPositions(string currLine, int pos)
        {
            char curr = currLine[pos];
            char next = Char.MinValue;
            if (pos < currLine.Length - 1)
            {
                next = currLine[pos + 1];
            }
            else
            {
                next = curr;
            }
            return curr.ToString() + next.ToString();
        }

        public static bool IsDigitPlus(char next)
        {
            return (Char.IsDigit(next) || next == 'b' || next == 'B'
                    || next == 'x' || next == 'X' || next == 'A' || next == 'C'
                    || next == 'D' || next == 'E' || next == 'F');
        }

        public static bool IsDigitPlusSecond(char next)
        {
            return (Char.IsDigit(next) || next == '.' || next == 'E' || next == 'e' ||
                    next == '-');
        }

        public static bool IsLetterPlus(char next)
        {
            return (Char.IsLetter(next) || next == '_');
        }

        public static bool CheckNumbersLength(Tokens token, string num)
        {
            if (token.Equals(Tokens.Int))
            {
                if ((Convert.ToInt64(num)) > Math.Pow(2, 31) || (Convert.ToInt64(num)) < -Math.Pow(2, 31))
                {
                    return false;
                }
            }
            if (token.Equals(Tokens.Hex))
            {
                long intValue;
                if (Int64.TryParse(num.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out intValue))
                {
                    if (intValue > Math.Pow(2,31))
                    {
                        return false;
                    }
                }
            }
            if (token.Equals(Tokens.Binary))
            {
                if (Convert.ToInt64(num.Substring(2), 2) > Math.Pow(2, 31))
                {
                    return false;
                }
            }
            if (token.Equals(Tokens.Double) || token.Equals(Tokens.Float))
            {
                if (double.Parse(num, System.Globalization.CultureInfo.InvariantCulture) > Math.Pow(2, 31) || double.Parse(num, System.Globalization.CultureInfo.InvariantCulture) < -Math.Pow(2, 31))
                {
                    return false;
                }
            }

            return true;
        }

        public static Tokens ReservedWords(string identificator)
        {
            Tokens tokens = Tokens.Null;
            switch (identificator)
            {
                case "if":
                    tokens = Tokens.If;
                    break;
                case "else":
                    tokens = Tokens.Else;
                    break;
                case "while":
                    tokens = Tokens.While;
                    break;
                case "for":
                    tokens = Tokens.For;
                    break;
                case "read":
                    tokens = Tokens.Read;
                    break;
                case "write":
                    tokens = Tokens.Write;
                    break;
                case "private":
                    tokens = Tokens.Private;
                    break;
                case "public":
                    tokens = Tokens.Public;
                    break;
                case "void":
                    tokens = Tokens.Void;
                    break;
                case "var":
                    tokens = Tokens.Var;
                    break;
                case "class":
                    tokens = Tokens.Class;
                    break;
                case "int":
                    tokens = Tokens.Int;
                    break;
                case "double":
                    tokens = Tokens.Double;
                    break;
                case "bool":
                    tokens = Tokens.Bool;
                    break;
                case "char":
                    tokens = Tokens.Char;
                    break;
                case "String":
                    tokens = Tokens.String;
                    break;
                default:
                    tokens = Tokens.Identificator;
                    break;
            }

            return tokens;
        }

        public static void PrintResultMessage(Tokens token, string output, int numStr, int factPos)
        {
            using (StreamWriter sw = new StreamWriter("output.txt", true, Encoding.Default))
            {
                sw.WriteLine(token.ToString() + ' ' + output + ' ' + numStr + ' ' + factPos);
            }
        }

        public static void PrintMessageForLL(Tokens token)
        {
            using (StreamWriter sw = new StreamWriter("../../../tokenizedInputLine.txt", true, Encoding.Default))
            {
                sw.Write(token.GetStringValue() + ' ');
            }
        }
    }

    static class Program
    {
        public static string GetStringValue(this Enum value)
        {
            Type type = value.GetType();

            FieldInfo fieldInfo = type.GetField(value.ToString());

            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            return attribs.Length > 0 ? attribs[0].StringValue : null;
        }

        private static void Lexer(string[] argv)
        {
            if (argv.Length < 1)
            {
                Console.WriteLine("Error arguments");
            }
            else
            {
                using (StreamWriter sw = new StreamWriter("output.txt", false, Encoding.Default))
                {
                    sw.WriteLine("Token, Value, Line, Position");
                }
                try
                {
                    Processing(argv);
                }
                catch (IOException ex)
                {
                    using (StreamWriter sw = new StreamWriter("output.txt", true, Encoding.Default))
                    {
                        sw.WriteLine(ex.Message);
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    using (StreamWriter sw = new StreamWriter("output.txt", true, Encoding.Default))
                    {
                        sw.WriteLine(Tokens.Error.ToString() + " Index out of range");
                    }
                }
                catch (Exception ex)
                {
                    using (StreamWriter sw = new StreamWriter("output.txt", true, Encoding.Default))
                    {
                        sw.WriteLine(Tokens.Error.ToString() + " " + ex.Message);
                    }
                }
            }
        }

        public static void Tokenize(string input)
        {
            string[] stringArr = new string[1];
            stringArr[0] = input;
            using (StreamWriter sw = new StreamWriter("../../../tokenizedInputLine.txt", false, Encoding.Default))
            {
                sw.Close();
            }
            Processing(stringArr);
        }

        public static void Processing(string[] arguments)
        {
            string line;
            int counter = 1;
            StreamReader file = new StreamReader(arguments[0]);
            if (file.Peek() < 0)
            {
                Tokens tokens = Tokens.EndOfFile;
                Functions.PrintResultMessage(tokens, "", 1, 1);
                Functions.PrintMessageForLL(tokens);

            }
            while ((line = file.ReadLine()) != null)
            {
                string outputString = "";
                int position = 0;
                int strNumber = counter;
                while (position < line.Length)
                {
                    char currChar = line[position];
                    char nextChar = Char.MinValue;
                    char prevChar = Char.MinValue;

                    if (position < (line.Length - 1))
                    {
                        nextChar = line[position + 1];
                    }

                    if (position > 0)
                    {
                        prevChar = line[position - 1];
                    }

                    Tokens tokens = Tokens.Null;

                    switch (currChar)
                    {
                        case ' ':
                            {
                                outputString = " ";
                                tokens = Tokens.Space;
                                break;
                            }
                        case '+':
                            {
                                outputString = "+";
                                tokens = Tokens.Add;
                                break;
                            }
                        case '-':
                            {
                                if (Char.IsDigit(nextChar))
                                {
                                    outputString = Char.ToString(currChar);

                                    bool isInt = true;
                                    bool isDouble = false;
                                    bool isFloat = false;

                                    while (Functions.IsDigitPlusSecond(currChar))
                                    {
                                        if (currChar == '.')
                                        {
                                            isInt = false;
                                            isDouble = true;
                                        }
                                        if (currChar == 'E' || currChar == 'e')
                                        {
                                            isInt = false;
                                            isDouble = false;
                                            isFloat = true;
                                        }
                                        position++;
                                        string currChars = Functions.GetPositions(line, position);
                                        currChar = currChars[0];
                                        nextChar = currChars[1];
                                        if (Functions.IsDigitPlusSecond(currChar))
                                        {
                                            outputString += currChar;
                                        }
                                    }

                                    if (!Char.IsDigit(currChar))
                                    {
                                        position--;
                                    }
                                
                                    if (isInt)
                                    {
                                        tokens = Tokens.Int;
                                    }
                                    else if (isDouble)
                                    {
                                        tokens = Tokens.Double;
                                    }
                                    else if (isFloat)
                                    {
                                        tokens = Tokens.Float;
                                    }
                                    break;
                                }
                                else
                                {
                                    outputString = "-";
                                    tokens = Tokens.Sub;
                                    break;
                                }
                            }
                        case '*':
                            {
                                outputString = "*";
                                tokens = Tokens.Mult;
                                break;
                            }
                        case '^':
                            {
                                outputString = "^";
                                tokens = Tokens.Pow;
                                break;
                            }
                        case '(':
                            {
                                outputString = "(";
                                tokens = Tokens.OpenParenthesis;
                                break;
                            }
                        case ')':
                            {
                                outputString = ")";
                                tokens = Tokens.CloseParenthesis;
                                break;
                            }
                        case '{':
                            {
                                outputString = "{";
                                tokens = Tokens.OpenBrace;
                                break;
                            }
                        case '}':
                            {
                                outputString = "}";
                                tokens = Tokens.CloseBrace;
                                break;
                            }
                        case '.':
                            {
                                outputString = ".";
                                tokens = Tokens.Dot;
                                break;
                            }
                        case ',':
                            {
                                outputString = ",";
                                tokens = Tokens.Comma;
                                break;
                            }
                        case ':':
                            {
                                outputString = ":";
                                tokens = Tokens.Colon;
                                break;
                            }
                        case ';':
                            {
                                outputString = ";";
                                tokens = Tokens.Semicolon;
                                break;
                            }
                        case '[':
                            {
                                outputString = "[";
                                tokens = Tokens.OpenBrackets;
                                break;
                            }
                        case ']':
                            {
                                outputString = "]";
                                tokens = Tokens.CloseBrackets;
                                break;
                            }
                        case '!':
                            {
                                if (nextChar == '=')
                                {
                                    outputString = "!=";
                                    tokens = Tokens.NotEqual;
                                    position++;
                                }
                                else
                                {
                                    outputString = "!";
                                    tokens = Tokens.LogicalNegation;
                                }
                                break;
                            }
                        case '&':
                            {
                                if (nextChar == '&')
                                {
                                    outputString = "&&";
                                    tokens = Tokens.ConditionalAnd;
                                    position++;
                                }
                                else
                                {
                                    outputString = "&";
                                    tokens = Tokens.LogicalAnd;
                                    position++;
                                }
                                break;
                            }
                        case '|':
                            {
                                if (nextChar == '|')
                                {
                                    outputString = "||";
                                    tokens = Tokens.ConditionalOr;
                                    position++;
                                }
                                else
                                {
                                    outputString = "|";
                                    tokens = Tokens.LogicalOr;
                                    position++;
                                }
                                break;
                            }
                        case '=':
                            {
                                if (nextChar == '=')
                                {
                                    outputString = "==";
                                    tokens = Tokens.Comparison;
                                    position++;
                                }
                                else
                                {
                                    outputString = "=";
                                    tokens = Tokens.Assignment;
                                }
                                break;
                            }
                        case '<':
                            {
                                if (nextChar == '=')
                                {
                                    outputString = "<=";
                                    tokens = Tokens.SmallOrEqual;
                                    position++;
                                }
                                else
                                {
                                    outputString = "<";
                                    tokens = Tokens.Smaller;
                                }
                                break;
                            }
                        case '>':
                            {
                                if (nextChar == '=')
                                {
                                    outputString = ">=";
                                    tokens = Tokens.MoreOrEqual;
                                    position++;
                                }
                                else
                                {
                                    outputString = ">";
                                    tokens = Tokens.More;
                                }
                                break;
                            }
                        case '/':
                            {
                                if (nextChar == '/')
                                {
                                    outputString = "//";
                                    tokens = Tokens.LineComment;
                                }
                                else if (nextChar == '*')
                                {
                                    outputString = "comment";
                                    tokens = Tokens.Comment;
                                }
                                else if ((prevChar != '*') && (nextChar != Char.MinValue))
                                {
                                    outputString = "/";
                                    tokens = Tokens.Div;
                                }
                                break;
                            }
                        case '"':
                            {
                                tokens = Tokens.StringParameter;
                                String str = Char.ToString(currChar);
                                while (nextChar != '"')
                                {
                                    position++;
                                    string currChars = Functions.GetPositions(line, position);
                                    currChar = currChars[0];
                                    nextChar = currChars[1];
                                    str += Char.ToString(currChar);
                                    if (position == line.Length - 1)
                                    {
                                        tokens = Tokens.Error;
                                        break;
                                    }
                                }
                                if (tokens.Equals(Tokens.StringParameter))
                                {
                                    outputString = str + '"';                              
                                }
                                else
                                {
                                    outputString = "string parameter";
                                }
                                position++;
                                break;
                            }
                        default:
                            {
                                if (Char.IsDigit(currChar))
                                {
                                    outputString = Char.ToString(currChar);

                                    bool isInt = true;
                                    bool isDouble = false;
                                    bool isFloat = false;
                                    bool isHex = false;
                                    bool isBinary = false;

                                    if (currChar == '0' && Functions.IsDigitPlus(nextChar))
                                    {
                                        while (Functions.IsDigitPlus(currChar) || Char.IsLetter(currChar))
                                        {
                                            if (currChar == 'X' || currChar == 'x')
                                            {
                                                isInt = false;
                                                isHex = true;
                                            }

                                            if ((currChar == 'B' || currChar == 'b') && !isHex)
                                            {
                                                isInt = false;
                                                isBinary = true;
                                            }

                                            if (isBinary && currChar != '0' && currChar != '1' && currChar != 'b' && currChar != 'B')
                                            {
                                                tokens = Tokens.Error;
                                                isBinary = false;                                                
                                            }

                                            if (isHex && currChar != 'x' && currChar != 'X' && currChar != 'B' && currChar != 'A' && currChar != 'C'
                                                && currChar != 'D' && currChar != 'E' && currChar != 'F' && !Char.IsDigit(currChar))
                                            {
                                                tokens = Tokens.Error;
                                                isHex = false;
                                            }

                                            position++;
                                            if (position >= line.Length)
                                            {
                                                break;
                                            }

                                            string currChars = Functions.GetPositions(line, position);
                                            currChar = currChars[0];
                                            nextChar = currChars[1];

                                            if (Functions.IsDigitPlus(currChar) || Char.IsLetter(currChar))
                                            {
                                                outputString += currChar;
                                            }
                                        }

                                        if (Functions.IsLetterPlus(currChar))
                                        {
                                            tokens = Tokens.Error;
                                            outputString += currChar;
                                            while (Functions.IsLetterPlus(currChar))
                                            {
                                                position++;
                                                if (position >= line.Length)
                                                {
                                                    break;
                                                }

                                                string currChars = Functions.GetPositions(line, position);
                                                currChar = currChars[0];
                                                nextChar = currChars[1];

                                                if (Functions.IsLetterPlus(currChar))
                                                {
                                                    outputString += currChar;
                                                }
                                                else
                                                {
                                                    position--;
                                                }
                                                
                                            }
                                            break;
                                        }

                                        if (!Char.IsDigit(currChar))
                                        {
                                            position--;
                                        }
                                        if (isBinary)
                                        {
                                            tokens = Tokens.Binary;
                                        }
                                        else if (isHex)
                                        {
                                            tokens = Tokens.Hex;
                                        }
                                    }
                                    else
                                    {
                                        while (Functions.IsDigitPlusSecond(currChar))
                                        {
                                            if (currChar == '.')
                                            {
                                                isInt = false;
                                                isDouble = true;
                                            }
                                            if (currChar == 'E' || currChar == 'e')
                                            {
                                                isInt = false;
                                                isDouble = false;
                                                isFloat = true;
                                            }
                                            position++;
                                            string currChars = Functions.GetPositions(line, position);
                                            currChar = currChars[0];
                                            nextChar = currChars[1];
                                            if (Functions.IsDigitPlusSecond(currChar))
                                            {
                                                outputString += currChar;
                                            }
                                        }

                                        if (Functions.IsLetterPlus(currChar))
                                        {
                                            tokens = Tokens.Error;
                                            outputString += currChar;
                                            while (Functions.IsLetterPlus(currChar))
                                            {
                                                position++;
                                                if (position >= line.Length)
                                                {
                                                    break;
                                                }

                                                string currChars = Functions.GetPositions(line, position);
                                                currChar = currChars[0];
                                                nextChar = currChars[1];

                                                if (Functions.IsLetterPlus(currChar))
                                                {
                                                    outputString += currChar;
                                                }
                                                else
                                                {
                                                    position--;
                                                }

                                            }
                                            break;
                                        }

                                        if (!Char.IsDigit(currChar))
                                        {
                                            position--;
                                        }                                     
                                    }
                                    if (isInt)
                                    {
                                        tokens = Tokens.Int;
                                    }
                                    else if (isDouble)
                                    {
                                        tokens = Tokens.Double;
                                    }
                                    else if (isFloat)
                                    {
                                        tokens = Tokens.Float;
                                    }
                                }
                                else if (Functions.IsLetterPlus(currChar))
                                {
                                    String ident = "";
                                    while (Functions.IsLetterPlus(currChar) || Char.IsDigit(currChar))
                                    {
                                        ident += currChar;
                                        position++;
                                        if (position >= line.Length)
                                        {
                                            break;
                                        }
                                        string currChars = Functions.GetPositions(line, position);
                                        currChar = currChars[0];
                                        nextChar = currChars[1];  
                                    }
                                    if (!Char.IsLetter(currChar) && currChar != '_' && !Char.IsDigit(currChar))
                                    {
                                        position--;
                                    }
                                    outputString = ident;
                                    if (outputString.Length > 255)
                                    {
                                        tokens = Tokens.Error;
                                    }
                                    else
                                    {
                                        tokens = Functions.ReservedWords(ident);
                                    }
                                }

                                break;
                            }
                    }

                    if (tokens.Equals(Tokens.Comment))
                    {
                        while (currChar != '*' || nextChar != '/')
                        {
                            position++;
                            if (position == line.Length - 1)
                            {
                                line = file.ReadLine();
                                position = 0;
                                counter++;
                            }

                            string currChars = Functions.GetPositions(line, position);
                            currChar = currChars[0];
                            nextChar = currChars[1];

                            if (file.Peek() < 0)
                            {
                                tokens = Tokens.Error;
                                break;
                            }
                            
                        }
                    }

                    if (!Functions.CheckNumbersLength(tokens, outputString))
                    {
                        tokens = Tokens.Error;
                    }

                    if (tokens.Equals(Tokens.LineComment))
                    {
                        position = line.Length;
                    }

                    int factPosition = position + 1;

                    if (!tokens.Equals(Tokens.Space) && !tokens.Equals(Tokens.Null))
                    {
                        Functions.PrintResultMessage(tokens, outputString, strNumber, factPosition);
                        Functions.PrintMessageForLL(tokens);
                    }

                    position++;
                    if (currChar != ' ' && file.Peek() < 0)
                    {
                        tokens = Tokens.EndOfFile;
                        Functions.PrintResultMessage(tokens, "", counter, factPosition);
                        Functions.PrintMessageForLL(tokens);

                    }
                }

                counter++;
            }
        }
    }
}