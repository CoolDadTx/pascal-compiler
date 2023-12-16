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
[GenerateOneOf]
public partial class TDataValue : OneOfBase<int, float, char, string>
{
    protected TDataValue ( OneOf<int, float, char, string> input ) : base(input)
    {
    }
}

