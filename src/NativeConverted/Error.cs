using System;

//--------------------------------------------------------------
//  Abort codes for fatal translator errors.
//--------------------------------------------------------------

public enum TAbortCode
{
	AbortInvalidCommandLineArgs = -1,
	AbortSourceFileOpenFailed = -2,
	AbortIFormFileOpenFailed = -3,
	AbortAssemblyFileOpenFailed = -4,
	AbortTooManySyntaxErrors = -5,
	AbortStackOverflow = -6,
	AbortCodeSegmentOverflow = -7,
	AbortNestingTooDeep = -8,
	AbortRuntimeError = -9,
	AbortUnimplementedFeature = -10,
}

//--------------------------------------------------------------
//  Error codes for syntax errors.
//--------------------------------------------------------------

public enum TErrorCode
{
	ErrNone,
	ErrUnrecognizable,
	ErrTooMany,
	ErrUnexpectedEndOfFile,
	ErrInvalidNumber,
	ErrInvalidFraction,
	ErrInvalidExponent,
	ErrTooManyDigits,
	ErrRealOutOfRange,
	ErrIntegerOutOfRange,
	ErrMissingRightParen,
	ErrInvalidExpression,
	ErrInvalidAssignment,
	ErrMissingIdentifier,
	ErrMissingColonEqual,
	ErrUndefinedIdentifier,
	ErrStackOverflow,
	ErrInvalidStatement,
	ErrUnexpectedToken,
	ErrMissingSemicolon,
	ErrMissingComma,
	ErrMissingDO,
	ErrMissingUNTIL,
	ErrMissingTHEN,
	ErrInvalidFORControl,
	ErrMissingOF,
	ErrInvalidConstant,
	ErrMissingConstant,
	ErrMissingColon,
	ErrMissingEND,
	ErrMissingTOorDOWNTO,
	ErrRedefinedIdentifier,
	ErrMissingEqual,
	ErrInvalidType,
	ErrNotATypeIdentifier,
	ErrInvalidSubrangeType,
	ErrNotAConstantIdentifier,
	ErrMissingDotDot,
	ErrIncompatibleTypes,
	ErrInvalidTarget,
	ErrInvalidIdentifierUsage,
	ErrIncompatibleAssignment,
	ErrMinGtMax,
	ErrMissingLeftBracket,
	ErrMissingRightBracket,
	ErrInvalidIndexType,
	ErrMissingBEGIN,
	ErrMissingPeriod,
	ErrTooManySubscripts,
	ErrInvalidField,
	ErrNestingTooDeep,
	ErrMissingPROGRAM,
	ErrAlreadyForwarded,
	ErrWrongNumberOfParms,
	ErrInvalidVarParm,
	ErrNotARecordVariable,
	ErrMissingVariable,
	ErrCodeSegmentOverflow,
	ErrUnimplementedFeature,
}

//fig 5-30
//--------------------------------------------------------------
//  Runtime error codes.
//--------------------------------------------------------------

public enum TRuntimeErrorCode
{
   RteNone,
   RteStackOverflow,
   RteValueOutOfRange,
   RteInvalidCaseValue,
   RteDivisionByZero,
   RteInvalidFunctionArgument,
   RteInvalidUserInput,
   RteUnimplementedRuntimeFeature,
}