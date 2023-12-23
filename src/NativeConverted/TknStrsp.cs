//  *************************************************************
//  *                                                           *
//  *   T O K E N S   (Strings and Specials)                    *
//  *                                                           *
//  *   Routines to extract string and special symbol tokens    *
//  *   from the source file.                                   *
//  *                                                           *
//  *   CLASSES: TStringToken, TSpecialToken, TErrorToken       *
//  *                                                           *
//  *   FILE:    prog3-2/tknstrsp.cpp                           *
//  *                                                           *
//  *   MODULE:  Scanner                                        *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************

//--------------------------------------------------------------
//  TStringToken        String token subclass of TToken.
//--------------------------------------------------------------
public class TStringToken : TToken
{

    public TStringToken ()
    {
        code = TTokenCode.TcString;
    }

    //--------------------------------------------------------------
    //  Get     Get a string token from the source.
    //
    //      pBuffer : ptr to text input buffer
    //--------------------------------------------------------------
    public override void Get ( TTextInBuffer buffer )
    {
        char ch; // current character

        var ps = new StringBuilder();
        ps.Append('\''); // opening quote

        //--Get the string.
        ch = buffer.GetChar(); // first char after opening quote
        while (ch != Globals.eofChar)
        {
            if (ch == '\'')
            { // look for another quote

                //--Fetched a quote.  Now check for an adjacent quote,
                //--since two consecutive quotes represent a single
                //--quote in the string.
                ch = buffer.GetChar();
                if (ch != '\'')
                    break; // not another quote, so previous
                           //   quote ended the string
            }

            //--Replace the end of line character with a blank.
            else if (ch == '\0')
                ch = ' ';

            //--Append current char to string, then get the next char.
            ps.Append(ch);
            ch = buffer.GetChar();
        }

        if (ch == Globals.eofChar)
            Globals.Error(TErrorCode.ErrUnexpectedEndOfFile);

        ps.Append('\''); // closing quote

        this.String = ps.ToString();
    }
    public override bool IsDelimiter () => true;

    //--------------------------------------------------------------
    //  Print       Print the token to the list file.
    //--------------------------------------------------------------
    public override void Print ()
    {
        Globals.list.text = String.Format("\t{0,-18} {1}", ">> string:", this.String);
        Globals.list.PutLine();
    }
}

//--------------------------------------------------------------
//  TSpecialToken       Special token subclass of TToken.
//--------------------------------------------------------------

public class TSpecialToken : TToken
{
    //--------------------------------------------------------------
    //  Get         Extract a one- or two-character special symbol
    //              token from the source.
    //
    //      pBuffer : ptr to text input buffer
    //--------------------------------------------------------------


    public override void Get ( TTextInBuffer buffer )
    {
        char ch = buffer.Char();

        var ps = new StringBuilder();

        ps.Append(ch);

        switch (ch)
        {
            case '^':
            code = TTokenCode.TcUpArrow;
            buffer.GetChar();
            break;
            case '*':
            code = TTokenCode.TcStar;
            buffer.GetChar();
            break;
            case '(':
            code = TTokenCode.TcLParen;
            buffer.GetChar();
            break;
            case ')':
            code = TTokenCode.TcRParen;
            buffer.GetChar();
            break;
            case '-':
            code = TTokenCode.TcMinus;
            buffer.GetChar();
            break;
            case '+':
            code = TTokenCode.TcPlus;
            buffer.GetChar();
            break;
            case '=':
            code = TTokenCode.TcEqual;
            buffer.GetChar();
            break;
            case '[':
            code = TTokenCode.TcLBracket;
            buffer.GetChar();
            break;
            case ']':
            code = TTokenCode.TcRBracket;
            buffer.GetChar();
            break;
            case ';':
            code = TTokenCode.TcSemicolon;
            buffer.GetChar();
            break;
            case ',':
            code = TTokenCode.TcComma;
            buffer.GetChar();
            break;
            case '/':
            code = TTokenCode.TcSlash;
            buffer.GetChar();
            break;

            case ':':
            ch = buffer.GetChar(); // : or :=
            if (ch == '=')
            {
                ps.Append('=');
                code = TTokenCode.TcColonEqual;
                buffer.GetChar();
            } else
            {
                code = TTokenCode.TcColon;
            }
            break;

            case '<':
            ch = buffer.GetChar(); // < or <= or <>
            if (ch == '=')
            {
                ps.Append('=');
                code = TTokenCode.TcLe;
                buffer.GetChar();
            } else if (ch == '>')
            {
                ps.Append(">");                
                code = TTokenCode.TcNe;
                buffer.GetChar();
            } else
            {
                code = TTokenCode.TcLt;
            }
            break;

            case '>':
            ch = buffer.GetChar(); // > or >=
            if (ch == '=')
            {
                ps.Append('=');
                code = TTokenCode.TcGe;
                buffer.GetChar();
            } else
            {
                code = TTokenCode.TcGt;
            }
            break;

            case '.':
            ch = buffer.GetChar(); // . or ..
            if (ch == '.')
            {
                ps.Append('.');
                code = TTokenCode.TcDotDot;
                buffer.GetChar();
            } else
            {
                code = TTokenCode.TcPeriod;
            }
            break;

            default:
            code = TTokenCode.TcError; // error
            buffer.GetChar();
            Globals.Error(TErrorCode.ErrUnrecognizable);
            break;
        }

        this.String = ps.ToString();
    }
    public override bool IsDelimiter () => true;

    //--------------------------------------------------------------
    //  Print       Print the token to the list file.
    //--------------------------------------------------------------
    public override void Print ()
    {
        Globals.list.text = String.Format("\t{0,-18} {1}", ">> special:", this.String);
        Globals.list.PutLine();
    }
}

//--------------------------------------------------------------
//  TErrorToken         Error token subclass of TToken.
//--------------------------------------------------------------

public class TErrorToken : TToken
{

    public TErrorToken ()
    {
        code = TTokenCode.TcError;
    }

    //--------------------------------------------------------------
    //  Get         Extract an invalid character from the source.
    //
    //      pBuffer : ptr to text input buffer
    //--------------------------------------------------------------
    public override void Get ( TTextInBuffer buffer )
    {
        this.String = buffer.Char().ToString();

        buffer.GetChar();
        Globals.Error(TErrorCode.ErrUnrecognizable);
    }
    public override bool IsDelimiter () => false;
    public override void Print ()
    {
    }
}