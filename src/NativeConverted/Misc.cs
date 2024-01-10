//--------------------------------------------------------------
//  TCharCode           Character codes.
//--------------------------------------------------------------
public enum TCharCode
{
	CcLetter,
	CcDigit,
	CcSpecial,
	CcQuote,
	CcWhiteSpace,
	CcEndOfFile,
	CcError,
}

//--------------------------------------------------------------
//  TTokenCode          Token codes.
//--------------------------------------------------------------

public enum TTokenCode
{
	TcDummy,
	TcIdentifier,
	TcNumber,
	TcString,
	TcEndOfFile,
	TcError,

	TcUpArrow,
	TcStar,
	TcLParen,
	TcRParen,
	TcMinus,
	TcPlus,
	TcEqual,
	TcLBracket,
	TcRBracket,
	TcColon,
	TcSemicolon,
	TcLt,
	TcGt,
	TcComma,
	TcPeriod,
	TcSlash,
	TcColonEqual,
	TcLe,
	TcGe,
	TcNe,
	TcDotDot,

	TcAND,
	TcARRAY,
	TcBEGIN,
	TcCASE,
	TcCONST,
	TcDIV,
	TcDO,
	TcDOWNTO,
	TcELSE,
	TcEND,
	TcFILE,
	TcFOR,
	TcFUNCTION,
	TcGOTO,
	TcIF,
	TcIN,
	TcLABEL,
	TcMOD,
	TcNIL,
	TcNOT,
	TcOF,
	TcOR,
	TcPACKED,
	TcPROCEDURE,
	TcPROGRAM,
	TcRECORD,
	TcREPEAT,
	TcSET,
	TcTHEN,
	TcTO,
	TcTYPE,
	TcUNTIL,
	TcVAR,
	TcWHILE,
	TcWITH,
}

//--------------------------------------------------------------
//  TDataType           Data type.
//--------------------------------------------------------------

public enum TDataType
{
	TyDummy,
	TyInteger,
	TyReal,
	TyCharacter,
	TyString,
}

//--------------------------------------------------------------
//  TDataValue          Data value.
//--------------------------------------------------------------

//union TDataValue
//{
//	int integer;
//	float real;
//	char character;
//	char *pString;
//};
public struct TDataValue
{
    public static TDataValue FromInteger ( int value ) => new TDataValue() { integer = value };

    public int integer
    {
        get => _value.AsT0;
        set => _value = OneOf<int, float, char, string>.FromT0(value);
    }

    public static TDataValue FromReal ( float value ) => new TDataValue() { real = value };

    public float real
    {
        get => _value.AsT1;
        set => _value = OneOf<int, float, char, string>.FromT1(value);
    }

    public static TDataValue FromCharacter ( char value ) => new TDataValue() { character = value };

    public char character
    {
        get => _value.AsT2;
        set => _value = OneOf<int, float, char, string>.FromT2(value);
    }

    public static TDataValue FromString ( string value ) => new TDataValue() { pString = value };

    public string pString
    {
        get => _value.AsT3;
        set => _value = OneOf<int, float, char, string>.FromT3(value);
    }

    private OneOf<int, float, char, string> _value;   
}

