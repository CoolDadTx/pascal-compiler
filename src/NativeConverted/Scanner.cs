//  *************************************************************
//  *                                                           *
//  *   S C A N N E R                                           *
//  *                                                           *
//  *   Scan the text input file.                               *
//  *                                                           *
//  *   CLASSES: TTextScanner                                   *
//  *                                                           *
//  *   FILE:    prog3-2/scanner.cpp                            *
//  *                                                           *
//  *   MODULE:  Scanner                                        *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************

//--------------------------------------------------------------
//  TScanner            Abstract scanner class.
//--------------------------------------------------------------

public abstract class TScanner : IDisposable
{
	//--Tokens extracted and returned by the scanner.
	protected TWordToken wordToken = new TWordToken();
	protected TNumberToken numberToken = new TNumberToken();
	protected TStringToken stringToken = new TStringToken();
	protected TSpecialToken specialToken = new TSpecialToken();
	protected TEOFToken eofToken = new TEOFToken();
	protected TErrorToken errorToken = new TErrorToken();

	public virtual void Dispose()
	{
	}

	public abstract TToken Get();
}

//--------------------------------------------------------------
//  TTextScanner        Text scanner subclass of TScanner.
//--------------------------------------------------------------

public class TTextScanner : TScanner
{
	private readonly TTextInBuffer pTextInBuffer; // ptr to input text buffer
					 //   to scan


	//--------------------------------------------------------------
	//  SkipWhiteSpace      Repeatedly fetch characters from the
	//                      text input as long as they're
	//                      whitespace. Each comment is a whitespace
	//                      character.
	//--------------------------------------------------------------

	private void SkipWhiteSpace()
	{
		char ch = pTextInBuffer.Char();

		do
		{
		if ( Globals.charCodeMap[ch] == TCharCode.CcWhiteSpace )

			//--Saw a whitespace character:  fetch the next character.
			ch = pTextInBuffer.GetChar();
		else if ( ch == '{' )
		{

			//--Skip over a comment, then fetch the next character.
			do
			ch = pTextInBuffer.GetChar();
			while ( ( ch != '}' ) && ( ch != Globals.eofChar ) );
			if ( ch != Globals.eofChar )
				ch = pTextInBuffer.GetChar();
			else
				Globals.Error( TErrorCode.ErrUnexpectedEndOfFile );
		}
		} while ( ( Globals.charCodeMap[ch] == TCharCode.CcWhiteSpace ) || ( ch == '{' ) );
	}


	//--------------------------------------------------------------
	//  Constructor     Construct a scanner by constructing the
	//                  text input file buffer and initializing the
	//                  character code map.
	//
	//      pBuffer : ptr to text input buffer to scan
	//--------------------------------------------------------------

	public TTextScanner( TTextInBuffer pBuffer )
	{
		this.pTextInBuffer = pBuffer;
		int i;

		//--Initialize the character code map.
		for ( i = 0; i < 128; ++i )
			Globals.charCodeMap[i] = TCharCode.CcError;
		for ( i = 'a'; i <= 'z'; ++i )
			Globals.charCodeMap[i] = TCharCode.CcLetter;
		for ( i = 'A'; i <= 'Z'; ++i )
			Globals.charCodeMap[i] = TCharCode.CcLetter;
		for ( i = '0'; i <= '9'; ++i )
			Globals.charCodeMap[i] = TCharCode.CcDigit;
		Globals.charCodeMap['+'] = Globals.charCodeMap['-'] = TCharCode.CcSpecial;
		Globals.charCodeMap['*'] = Globals.charCodeMap['/'] = TCharCode.CcSpecial;
		Globals.charCodeMap['='] = Globals.charCodeMap['^'] = TCharCode.CcSpecial;
		Globals.charCodeMap['.'] = Globals.charCodeMap[','] = TCharCode.CcSpecial;
		Globals.charCodeMap['<'] = Globals.charCodeMap['>'] = TCharCode.CcSpecial;
		Globals.charCodeMap['('] = Globals.charCodeMap[')'] = TCharCode.CcSpecial;
		Globals.charCodeMap['['] = Globals.charCodeMap[']'] = TCharCode.CcSpecial;
		Globals.charCodeMap['{'] = Globals.charCodeMap['}'] = TCharCode.CcSpecial;
		Globals.charCodeMap[':'] = Globals.charCodeMap[';'] = TCharCode.CcSpecial;
		Globals.charCodeMap[' '] = Globals.charCodeMap['\t'] = TCharCode.CcWhiteSpace;
		Globals.charCodeMap['\n'] = Globals.charCodeMap['\0'] = TCharCode.CcWhiteSpace;
		Globals.charCodeMap['\''] = TCharCode.CcQuote;
		Globals.charCodeMap[Globals.eofChar] = TCharCode.CcEndOfFile;
	}
	public override void Dispose()
	{
		if ( pTextInBuffer != null )
			pTextInBuffer.Dispose();
		base.Dispose();
	}
    
    //--------------------------------------------------------------
	//  Get         Extract the next token from the text input,
	//              based on the current character.
	//
	//  Return: pointer to the extracted token
	//--------------------------------------------------------------

	public override TToken Get()
	{
		TToken pToken; // ptr to token to return

		SkipWhiteSpace();

		//--Determine the token class, based on the current character.
		switch ( Globals.charCodeMap[pTextInBuffer.Char()] )
		{
		case TCharCode.CcLetter:
			pToken = wordToken;
			break;
		case TCharCode.CcDigit:
			pToken = numberToken;
			break;
		case TCharCode.CcQuote:
			pToken = stringToken;
			break;
		case TCharCode.CcSpecial:
			pToken = specialToken;
			break;
		case TCharCode.CcEndOfFile:
			pToken = eofToken;
			break;
		default:
			pToken = errorToken;
			break;
		}

		//--Extract a token of that class, and return a pointer to it.
		pToken.Get( pTextInBuffer );
		return pToken;
	}
}