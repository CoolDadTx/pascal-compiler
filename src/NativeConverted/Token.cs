//--------------------------------------------------------------
//  TToken              Abstract token class.
//--------------------------------------------------------------

public abstract class TToken
{

//C++ TO C# CONVERTER TODO TASK: C# has no concept of a 'friend' class:
//	friend class TIcode;

	protected TTokenCode code;
	protected TDataType type;
	protected TDataValue value = new TDataValue();
	protected string string = new string( new char[maxInputBufferSize] );

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: TTokenCode Code() const
	public TTokenCode Code()
	{
		return code;
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: TDataType Type() const
	public TDataType Type()
	{
		return type;
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: TDataValue Value() const
	public TDataValue Value()
	{
		return value;
	}
	public string String()
	{
		return string;
	}

	public abstract void Get( TTextInBuffer buffer );
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual int IsDelimiter() const = 0;
	public abstract int IsDelimiter();
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual void Print() const = 0;
	public abstract void Print();
}

//--------------------------------------------------------------
//  TWordToken          Word token subclass of TToken.
//--------------------------------------------------------------

public class TWordToken : TToken
{
	public void CheckForReservedWord()
	{
		int len = string.Length;
	//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
		TResWord * prw = new TResWord(); // ptr to elmt of reserved word table
    
		code = TTokenCode.TcIdentifier; // first assume it's an identifier
    
		//--Is it the right length?
		if ( ( len >= minResWordLen ) && ( len <= maxResWordLen ) )
		{
    
		//--Yes.  Use the word length to pick the appropriate list
		//--from the reserved word table and check to see if the word
		//--is in there.
		for ( prw = rwTable[len]; prw.pString != null; ++prw )
		{
			if ( string.Compare( string, prw.pString ) == 0 )
			{
			code = prw.code; // yes: set reserved word token code
			break;
			}
		}
		}
	}

	public void Get( TTextInBuffer buffer )
	{
	//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//	extern TCharCode charCodeMap[];
    
		char ch = buffer.Char(); // char fetched from input
	//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
		char * ps = string;
    
		//--Get the word.
		do
		{
		*ps++= ch;
		ch = buffer.GetChar();
		} while ( ( charCodeMap[ch] == TCharCode.CcLetter ) || ( charCodeMap[ch] == TCharCode.CcDigit ) );
    
		*ps = '\0';
		string.ToLower(); // downshift its characters
    
		CheckForReservedWord();
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual int IsDelimiter() const
	public override int IsDelimiter()
	{
		return false;
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual void Print() const;
	public void Print()
	{
		if ( code == TTokenCode.TcIdentifier )
		list.text = string.Format( "\t{0,-18} {1}", ">> identifier:", string );
		else
		list.text = string.Format( "\t{0,-18} {1}", ">> reserved word:", string );
    
		list.PutLine();
	}
}

//--------------------------------------------------------------
//  TNumberToken        Number token subclass of TToken.
//--------------------------------------------------------------

public class TNumberToken : TToken
{
	private char ch; // char fetched from input buffer
	private string ps; // ptr into token string
	private int digitCount; // total no. of digits in number
	private int countErrorFlag; // true if too many digits, else false

	public int AccumulateValue( TTextInBuffer buffer, ref float value, TErrorCode ec )
	{
		const int maxDigitCount = 20;
    
		//--Error if the first character is not a digit.
		if ( charCodeMap[ch] != TCharCode.CcDigit )
		{
		Error( ec );
		return false; // failure
		}
    
		//--Accumulate the value as long as the total allowable
		//--number of digits has not been exceeded.
		do
		{
		*ps++= ch;
    
		if ( ++digitCount <= maxDigitCount )
			value = 10 * value + ( ch - '0' ); // shift left and add
		else
			countErrorFlag = true; // too many digits
    
		ch = buffer.GetChar();
		} while ( charCodeMap[ch] == TCharCode.CcDigit );
    
		return true; // success
	}

	public TNumberToken()
	{
		code = TTokenCode.TcNumber;
	}

	public void Get( TTextInBuffer buffer )
	{
		const int maxInteger = 32767;
		const int maxExponent = 37;
    
		float numValue = 0.0F; // value of number ignoring
					   //    the decimal point
		int wholePlaces = 0; // no. digits before the decimal point
		int decimalPlaces = 0; // no. digits after  the decimal point
		char exponentSign = '+';
		float eValue = 0.0F; // value of number after 'E'
		int exponent = 0; // final value of exponent
		int sawDotDotFlag = false; // true if encountered '..',
					   //   else false
    
		ch = buffer.Char();
		ps = string;
		digitCount = 0;
		countErrorFlag = false;
		code = TTokenCode.TcError; // we don't know what it is yet, but
		type = TDataType.TyInteger; //    assume it'll be an integer
    
		//--Get the whole part of the number by accumulating
		//--the values of its digits into numValue.  wholePlaces keeps
		//--track of the number of digits in this part.
		if ( !AccumulateValue( buffer, numValue, TErrorCode.ErrInvalidNumber ) )
			return;
		wholePlaces = digitCount;
    
		//--If the current character is a dot, then either we have a
		//--fraction part or we are seeing the first character of a '..'
		//--token.  To find out, we must fetch the next character.
		if ( ch == '.' )
		{
		ch = buffer.GetChar();
    
		if ( ch == '.' )
		{
    
			//--We have a .. token.  Back up bufferp so that the
			//--token can be extracted next.
			sawDotDotFlag = true;
			buffer.PutBackChar();
		} else
		{
			type = TDataType.TyReal;
			*ps++= '.';
    
			//--We have a fraction part.  Accumulate it into numValue.
			if ( !AccumulateValue( buffer, numValue, TErrorCode.ErrInvalidFraction ) )
				return;
			decimalPlaces = digitCount - wholePlaces;
		}
		}
    
		//--Get the exponent part, if any. There cannot be an
		//--exponent part if we already saw the '..' token.
		if ( sawDotDotFlag == 0 && ( ( ch == 'E' ) || ( ch == 'e' ) ) )
		{
		type = TDataType.TyReal;
		*ps++= ch;
		ch = buffer.GetChar();
    
		//--Fetch the exponent's sign, if any.
		if ( ( ch == '+' ) || ( ch == '-' ) )
		{
			*ps++= exponentSign = ch;
			ch = buffer.GetChar();
		}
    
		//--Accumulate the value of the number after 'E' into eValue.
		digitCount = 0;
		if ( !AccumulateValue( buffer, eValue, TErrorCode.ErrInvalidExponent ) )
			return;
		if ( exponentSign == '-' )
			eValue = -eValue;
		}
    
		//--Were there too many digits?
		if ( countErrorFlag )
		{
		Error( TErrorCode.ErrTooManyDigits );
		return;
		}
    
		//--Calculate and check the final exponent value,
		//--and then use it to adjust the number's value.
		exponent = ( int )eValue - decimalPlaces;
		if ( ( exponent + wholePlaces < -maxExponent ) || ( exponent + wholePlaces > maxExponent ) )
		{
		Error( TErrorCode.ErrRealOutOfRange );
		return;
		}
		if ( exponent != 0 )
			numValue *= ( float )Math.Pow( 10, exponent );
    
		//--Check and set the numeric value.
		if ( type == TDataType.TyInteger )
		{
		if ( ( numValue < -maxInteger ) || ( numValue > maxInteger ) )
		{
			Error( TErrorCode.ErrIntegerOutOfRange );
			return;
		}
		value.integer = ( int )numValue;
		} else
		{
			value.real = numValue;
		}
    
		*ps = '\0';
		code = TTokenCode.TcNumber;
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual int IsDelimiter() const
	public override int IsDelimiter()
	{
		return false;
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual void Print() const;
	public void Print()
	{
		if ( type == TDataType.TyInteger )
		list.text = string.Format( "\t{0,-18} ={1:D}", ">> integer:", value.integer );
		else
		list.text = string.Format( "\t{0,-18} ={1:g}", ">> real:", value.real );
    
		list.PutLine();
	}
}

//--------------------------------------------------------------
//  TStringToken        String token subclass of TToken.
//--------------------------------------------------------------

public class TStringToken : TToken
{

	public TStringToken()
	{
		code = TTokenCode.TcString;
	}

	public void Get( TTextInBuffer buffer )
	{
		char ch; // current character
	//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
		char * ps = string; // ptr to char in string
    
		*ps++= '\''; // opening quote
    
		//--Get the string.
		ch = buffer.GetChar(); // first char after opening quote
		while ( ch != eofChar )
		{
		if ( ch == '\'' )
		{ // look for another quote
    
			//--Fetched a quote.  Now check for an adjacent quote,
			//--since two consecutive quotes represent a single
			//--quote in the string.
			ch = buffer.GetChar();
			if ( ch != '\'' )
				break; // not another quote, so previous
						//   quote ended the string
		}
    
		//--Replace the end of line character with a blank.
		else if ( ch == '\0' )
			ch = ' ';
    
		//--Append current char to string, then get the next char.
		*ps++= ch;
		ch = buffer.GetChar();
		}
    
		if ( ch == eofChar )
			Error( TErrorCode.ErrUnexpectedEndOfFile );
    
		*ps++= '\''; // closing quote
		*ps = '\0';
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual int IsDelimiter() const
	public override int IsDelimiter()
	{
		return true;
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual void Print() const;
	public void Print()
	{
		list.text = string.Format( "\t{0,-18} {1}", ">> string:", string );
		list.PutLine();
	}
}

//--------------------------------------------------------------
//  TSpecialToken       Special token subclass of TToken.
//--------------------------------------------------------------

public class TSpecialToken : TToken
{

	public void Get( TTextInBuffer buffer )
	{
		char ch = buffer.Char();
	//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
		char * ps = string;
    
		*ps++= ch;
    
		switch ( ch )
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
				if ( ch == '=' )
				{
				*ps++= '=';
				code = TTokenCode.TcColonEqual;
				buffer.GetChar();
				} else
				{
					code = TTokenCode.TcColon;
				}
				break;
    
		case '<':
			ch = buffer.GetChar(); // < or <= or <>
				if ( ch == '=' )
				{
				*ps++= '=';
				code = TTokenCode.TcLe;
				buffer.GetChar();
				} else if ( ch == '>' )
				{
				*ps++= '>';
				code = TTokenCode.TcNe;
				buffer.GetChar();
				} else
				{
					code = TTokenCode.TcLt;
				}
				break;
    
		case '>':
			ch = buffer.GetChar(); // > or >=
				if ( ch == '=' )
				{
				*ps++= '=';
				code = TTokenCode.TcGe;
				buffer.GetChar();
				} else
				{
					code = TTokenCode.TcGt;
				}
				break;
    
		case '.':
			ch = buffer.GetChar(); // . or ..
				if ( ch == '.' )
				{
				*ps++= '.';
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
				Error( TErrorCode.ErrUnrecognizable );
				break;
		}
    
		*ps = '\0';
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual int IsDelimiter() const
	public override int IsDelimiter()
	{
		return true;
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual void Print() const;
	public void Print()
	{
		list.text = string.Format( "\t{0,-18} {1}", ">> special:", string );
		list.PutLine();
	}
}

//--------------------------------------------------------------
//  TEOFToken           End-of-file token subclass of TToken.
//--------------------------------------------------------------

public class TEOFToken : TToken
{

	public TEOFToken()
	{
		code = TTokenCode.TcEndOfFile;
	}

	public override void Get( TTextInBuffer buffer )
	{
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual int IsDelimiter() const
	public override int IsDelimiter()
	{
		return false;
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual void Print() const
	public override void Print()
	{
	}
}

//--------------------------------------------------------------
//  TErrorToken         Error token subclass of TToken.
//--------------------------------------------------------------

public class TErrorToken : TToken
{

	public TErrorToken()
	{
		code = TTokenCode.TcError;
	}

	public void Get( TTextInBuffer buffer )
	{
		string[0] = buffer.Char();
		string[1] = '\0';
    
		buffer.GetChar();
		Error( TErrorCode.ErrUnrecognizable );
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual int IsDelimiter() const
	public override int IsDelimiter()
	{
		return false;
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual void Print() const
	public override void Print()
	{
	}
}

