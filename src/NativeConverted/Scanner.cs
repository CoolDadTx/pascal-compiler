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

//fig 3-5
//  *************************************************************
//  *                                                           *
//  *   S C A N N E R   (Header)                                *
//  *                                                           *
//  *   CLASSES: TScanner, TTextScanner                         *
//  *                                                           *
//  *   FILE:    prog3-1/scanner.h                              *
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

public abstract class TScanner : System.IDisposable
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


	//fig 3-16
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
		if ( GlobalMembers.charCodeMap[ch] == TCharCode.CcWhiteSpace )

			//--Saw a whitespace character:  fetch the next character.
			ch = pTextInBuffer.GetChar();
		else if ( ch == '{' )
		{

			//--Skip over a comment, then fetch the next character.
			do
			ch = pTextInBuffer.GetChar();
			while ( ( ch != '}' ) && ( ch != eofChar ) );
			if ( ch != eofChar )
				ch = pTextInBuffer.GetChar();
			else
				Error( TErrorCode.ErrUnexpectedEndOfFile );
		}
		} while ( ( GlobalMembers.charCodeMap[ch] == TCharCode.CcWhiteSpace ) || ( ch == '{' ) );
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
			GlobalMembers.charCodeMap[i] = TCharCode.CcError;
		for ( i = 'a'; i <= 'z'; ++i )
			GlobalMembers.charCodeMap[i] = TCharCode.CcLetter;
		for ( i = 'A'; i <= 'Z'; ++i )
			GlobalMembers.charCodeMap[i] = TCharCode.CcLetter;
		for ( i = '0'; i <= '9'; ++i )
			GlobalMembers.charCodeMap[i] = TCharCode.CcDigit;
		GlobalMembers.charCodeMap['+'] = GlobalMembers.charCodeMap['-'] = TCharCode.CcSpecial;
		GlobalMembers.charCodeMap['*'] = GlobalMembers.charCodeMap['/'] = TCharCode.CcSpecial;
		GlobalMembers.charCodeMap['='] = GlobalMembers.charCodeMap['^'] = TCharCode.CcSpecial;
		GlobalMembers.charCodeMap['.'] = GlobalMembers.charCodeMap[','] = TCharCode.CcSpecial;
		GlobalMembers.charCodeMap['<'] = GlobalMembers.charCodeMap['>'] = TCharCode.CcSpecial;
		GlobalMembers.charCodeMap['('] = GlobalMembers.charCodeMap[')'] = TCharCode.CcSpecial;
		GlobalMembers.charCodeMap['['] = GlobalMembers.charCodeMap[']'] = TCharCode.CcSpecial;
		GlobalMembers.charCodeMap['{'] = GlobalMembers.charCodeMap['}'] = TCharCode.CcSpecial;
		GlobalMembers.charCodeMap[':'] = GlobalMembers.charCodeMap[';'] = TCharCode.CcSpecial;
		GlobalMembers.charCodeMap[' '] = GlobalMembers.charCodeMap['\t'] = TCharCode.CcWhiteSpace;
		GlobalMembers.charCodeMap['\n'] = GlobalMembers.charCodeMap['\0'] = TCharCode.CcWhiteSpace;
		GlobalMembers.charCodeMap['\''] = TCharCode.CcQuote;
		GlobalMembers.charCodeMap[eofChar] = TCharCode.CcEndOfFile;
	}
	public override void Dispose()
	{
		if ( pTextInBuffer != null )
			pTextInBuffer.Dispose();
		base.Dispose();
	}

	//endfig

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
		switch ( GlobalMembers.charCodeMap[pTextInBuffer.Char()] )
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