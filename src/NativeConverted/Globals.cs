public static class Globals
{	
	//              ***********
	//              *         *
	//              *  Input  *
	//              *         *
	//              ***********

	public static readonly int maxInputBufferSize = 256;

	//              ***********************
	//              *                     *
	//              *  Text Input Buffer  *
	//              *                     *
	//              ***********************

	public static char eofChar = (char)0x7F; // special end-of-file character
	public static int inputPosition; // "virtual" position of the current char
				   //   in the input buffer (with tabs expanded)
	public static bool listFlag = true; // true if list source lines, else false

	//              *****************
	//              *               *
	//              *  List Buffer  *
	//              *               *
	//              *****************

	public static readonly int maxPrintLineLength = 80;
	public static readonly int maxLinesPerPage = 50;

	public static TListBuffer list = new TListBuffer(); // the list file buffer

	//--------------------------------------------------------------
	//  Token lists
	//--------------------------------------------------------------

//--------------------------------------------------------------
//  TokenIn     Check if a token code is in the token list.
//
//      tc    : token code
//      pList : ptr to tcDummy-terminated token list
//
//  Return:  true if in list, false if not or empty list
//--------------------------------------------------------------


	public static int TokenIn( TTokenCode tc, TTokenCode pList )
	{
//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
		TTokenCode * pCode; // ptr to token code in list

		if ( ( ( int )pList ) == 0 )
			return false; // empty list

		for ( pCode = pList; * pCode; ++pCode )
		{
		if ( *pCode == tc )
			return true; // in list
		}

		return false; // not in list
	}

	public static int currentNestingLevel = 0;
	public static int currentLineNumber = 0;

	public static TSymtab globalSymtab = new TSymtab(); // the global symbol table
	public static int cntSymtabs = 0; // symbol table counter
	public static TSymtab pSymtabList = null; // ptr to head of symtab list
	public static TSymtab[] vpSymtabs; // ptr to vector of symtab ptrs

	//--------------------------------------------------------------
	//  Token lists
	//--------------------------------------------------------------

	//--------------------------------------------------------------
	//  main
	//--------------------------------------------------------------	
	
	//--------------------------------------------------------------
	//  Registers and instructions
	//--------------------------------------------------------------

	public static string[] registers = new [] { "ax", "ah", "al", "bx", "bh", "bl", "cx", "ch", "cl", "dx", "dh", "dl", "cs", "ds", "es", "ss", "sp", "bp", "si", "di" };

	public static string[] instructions = new [] { "mov", "rep\tmovsb", "lea", "xchg", "cmp", "repe\tcmpsb", "pop", "push", "and", "or", "xor", "neg", "inc", "dec", "add", "sub", "imul", "idiv", "cld", "call", "ret", "jmp", "jl", "jle", "je", "jne", "jge", "jg" };

	//--------------------------------------------------------------
	//  EmitIdComment           Emit an identifier and its
	//                          modifiers as a comment.
	//--------------------------------------------------------------

	//--Tokens that can start an identifier modifier.
	public static TTokenCode[] tlIdModStart = { TTokenCode.TcLBracket, TTokenCode.TcLParen, TTokenCode.TcPeriod, TTokenCode.TcDummy };

	//--Tokens that can end an identifier modifier.
	public static TTokenCode[] tlIdModEnd = { TTokenCode.TcRBracket, TTokenCode.TcRParen, TTokenCode.TcDummy };

//--------------------------------------------------------------
//  AbortTranslation    A fatal error occurred during the
//                      translation.  Print the abort code
//                      to the error file and then exit.
//
//      ac : abort code
//--------------------------------------------------------------

    public static void DisplayError ( string message )
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

	public static void AbortTranslation( TAbortCode ac )
	{
        var code = (int)ac;
        DisplayError($"*** Fatal translator error: {abortMsg[-code]}");
        Environment.Exit(code);
	}

//--------------------------------------------------------------
//  Error       Print an arrow under the error and then
//              print the error message.
//
//      ec : error code
//--------------------------------------------------------------


	public static void Error( TErrorCode ec )
	{
		const int maxSyntaxErrors = 25;

		int errorPosition = errorArrowOffset + inputPosition - 1;

		//--Print the arrow pointing to the token just scanned.
		if ( errorArrowFlag != 0 )
		{
//C++ TO C# CONVERTER TODO TASK: The following line has a C format specifier which cannot be directly translated to C#:
//ORIGINAL LINE: sprintf(list.text, "%*s^", errorPosition, " ");
		list.text = String.Format( "%*s^", errorPosition, " " );
		list.PutLine();
		}

		//--Print the error message.
		list.text = String.Format( "*** ERROR: {0}", errorMessages[( int )ec] );
		list.PutLine();

		if ( ++errorCount > maxSyntaxErrors )
		{
		list.PutLine( "Too many syntax errors.  Translation aborted." );
		AbortTranslation( TAbortCode.AbortTooManySyntaxErrors );
		}
	}

//--------------------------------------------------------------
//  RuntimeError        Print the runtime error message and then
//                      abort the program.
//
//      ec : error code
//--------------------------------------------------------------


	public static void RuntimeError( TRuntimeErrorCode ec )
	{
		Console.WriteLine();
		Console.Write("*** RUNTIME ERROR in line ");
		Console.Write(currentLineNumber);
		Console.Write(": ");
		Console.Write(runtimeErrorMessages[(int)ec]);
		Console.WriteLine();
	}

	public static int errorCount = 0; // count of syntax errors
	public static bool errorArrowFlag = true; // true if print arrows under syntax
					  //   errors, false if not
	public static int errorArrowOffset = 8; // offset for printing the error arrow

	//--------------------------------------------------------------
	//  Abort messages      Keyed to enumeration type TAbortCode.
	//--------------------------------------------------------------

//C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent to pointers to value types:
//ORIGINAL LINE: char *abortMsg[] = { null, "Invalid command line arguments", "Failed to open source file", "Failed to open intermediate form file", "Failed to open assembly file", "Too many syntax errors", "Stack overflow", "Code segment overflow", "Nesting too deep", "Runtime error", "Unimplemented feature"};
	public static string[] abortMsg = new [] { null, "Invalid command line arguments", "Failed to open source file", "Failed to open intermediate form file", "Failed to open assembly file", "Too many syntax errors", "Stack overflow", "Code segment overflow", "Nesting too deep", "Runtime error", "Unimplemented feature" };

	//--------------------------------------------------------------
	//  Error messages      Keyed to enumeration type TErrorCode.
	//--------------------------------------------------------------

	public static string[] errorMessages = new[] { "No error", "Unrecognizable input", "Too many syntax errors", "Unexpected end of file", "Invalid number", "Invalid fraction", "Invalid exponent", "Too many digits", "Real literal out of range", "Integer literal out of range", "Missing )", "Invalid expression", "Invalid assignment statement", "Missing identifier", "Missing :=", "Undefined identifier", "Stack overflow", "Invalid statement", "Unexpected token", "Missing ;", "Missing ,", "Missing DO", "Missing UNTIL", "Missing THEN", "Invalid FOR control variable", "Missing OF", "Invalid constant", "Missing constant", "Missing :", "Missing END", "Missing TO or DOWNTO", "Redefined identifier", "Missing =", "Invalid type", "Not a type identifier", "Invalid subrange type", "Not a constant identifier", "Missing ..", "Incompatible types", "Invalid assignment target", "Invalid identifier usage", "Incompatible assignment", "Min limit greater than max limit", "Missing [", "Missing ]", "Invalid index type", "Missing BEGIN", "Missing .", "Too many subscripts", "Invalid field", "Nesting too deep", "Missing PROGRAM", "Already specified in FORWARD", "Wrong number of actual parameters", "Invalid VAR parameter", "Not a record variable", "Missing variable", "Code segment overflow", "Unimplemented feature" };

	//--------------------------------------------------------------
	//  Runtime error messages      Keyed to enumeration type
	//                              TRuntimeErrorCode.
	//--------------------------------------------------------------

	public static string[] runtimeErrorMessages = new[] { "No runtime error", "Runtime stack overflow", "Value out of range", "Invalid CASE expression value", "Division by zero", "Invalid standard function argument", "Invalid user input", "Unimplemented runtime feature" };

	public static readonly TTokenCode mcLineMarker = ( ( TTokenCode ) 127 );
	public static readonly TTokenCode mcLocationMarker = ( ( TTokenCode ) 126 );

	//--------------------------------------------------------------
	//  TIcode      Intermediate code subclass of TScanner.
	//--------------------------------------------------------------

	//--Vector of special symbol and reserved word strings.
	public static readonly string[] symbolStrings = new[] { null, null, null, null, null, null, "^", "*", "(", ")", "-", "+", "=", "[", "]", ":", ";", "<", ">", ",", ".", "/", ":=", "<=", ">=", "<>", "..", "and", "array", "begin", "case", "const", "div", "do", "downto", "else", "end", "file", "for", "function", "goto", "if", "in", "label", "mod", "nil", "not", "of", "or", "packed", "procedure", "program", "record", "repeat", "set", "then", "to", "type", "until", "var", "while", "with" };

	public static bool execFlag = true; // true for executor back end,

	public static TStdRtn[] stdRtnList = new[]
	{
		new TStdRtn() { pName = "read", rc = TRoutineCode.RcRead, dc = TDefnCode.DcProcedure },
		new TStdRtn() { pName = "readln", rc = TRoutineCode.RcReadln, dc = TDefnCode.DcProcedure },
		new TStdRtn() { pName = "write", rc = TRoutineCode.RcWrite, dc = TDefnCode.DcProcedure },
		new TStdRtn() { pName = "writeln", rc = TRoutineCode.RcWriteln, dc = TDefnCode.DcProcedure },
		new TStdRtn() { pName = "abs", rc = TRoutineCode.RcAbs, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "arctan", rc = TRoutineCode.RcArctan, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "chr", rc = TRoutineCode.RcChr, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "cos", rc = TRoutineCode.RcCos, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "eof", rc = TRoutineCode.RcEof, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "eoln", rc = TRoutineCode.RcEoln, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "exp", rc = TRoutineCode.RcExp, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "ln", rc = TRoutineCode.RcLn, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "odd", rc = TRoutineCode.RcOdd, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "ord", rc = TRoutineCode.RcOrd, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "pred", rc = TRoutineCode.RcPred, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "round", rc = TRoutineCode.RcRound, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "sin", rc = TRoutineCode.RcSin, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "sqr", rc = TRoutineCode.RcSqr, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "sqrt", rc = TRoutineCode.RcSqrt, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "succ", rc = TRoutineCode.RcSucc, dc = TDefnCode.DcFunction },
		new TStdRtn() { pName = "trunc", rc = TRoutineCode.RcTrunc, dc = TDefnCode.DcFunction }
	};

	public static void InitializeStandardRoutines( TSymtab pSymtab )
	{
		int i = 0;

		do
		{
		TStdRtn pSR = stdRtnList[i];
		TSymtabNode pRoutineId = pSymtab.Enter( pSR.pName, pSR.dc );

		pRoutineId.defn.routine.which = pSR.rc;
		pRoutineId.defn.routine.parmCount = 0;
		pRoutineId.defn.routine.totalParmSize = 0;
		pRoutineId.defn.routine.totalLocalSize = 0;
		pRoutineId.defn.routine.locals.pParmIds = null;
		pRoutineId.defn.routine.locals.pConstantIds = null;
		pRoutineId.defn.routine.locals.pTypeIds = null;
		pRoutineId.defn.routine.locals.pVariableIds = null;
		pRoutineId.defn.routine.locals.pRoutineIds = null;
		pRoutineId.defn.routine.pSymtab = null;
		pRoutineId.defn.routine.pIcode = null;
		SetType( pRoutineId.pType, pDummyType );
		} while ( stdRtnList[++i].pName != null );
	}

	public static TCharCode[] charCodeMap = new TCharCode[128]; // maps a character to its code

	public static int asmLabelIndex = 0; // assembly label index
	public static bool xrefFlag = false; // true = cross-referencing on, false = off

	//              *************************
	//              *                       *
	//              *  Reserved Word Table  *
	//              *                       *
	//              *************************

	public static readonly int minResWordLen = 2; // min and max reserved
	public static readonly int maxResWordLen = 9; //   word lengths

	internal static TResWord[] rw2 =
	{
		new TResWord() { pString = "do", code = TTokenCode.TcDO },
		new TResWord() { pString = "if", code = TTokenCode.TcIF },
		new TResWord() { pString = "in", code = TTokenCode.TcIN },
		new TResWord() { pString = "of", code = TTokenCode.TcOF },
		new TResWord() { pString = "or", code = TTokenCode.TcOR },
		new TResWord() { pString = "to", code = TTokenCode.TcTO }
	};

	internal static TResWord[] rw3 =
	{
		new TResWord() { pString = "and", code = TTokenCode.TcAND },
		new TResWord() { pString = "div", code = TTokenCode.TcDIV },
		new TResWord() { pString = "end", code = TTokenCode.TcEND },
		new TResWord() { pString = "for", code = TTokenCode.TcFOR },
		new TResWord() { pString = "mod", code = TTokenCode.TcMOD },
		new TResWord() { pString = "nil", code = TTokenCode.TcNIL },
		new TResWord() { pString = "not", code = TTokenCode.TcNOT },
		new TResWord() { pString = "set", code = TTokenCode.TcSET },
		new TResWord() { pString = "var", code = TTokenCode.TcVAR }
	};

	internal static TResWord[] rw4 =
	{
		new TResWord() { pString = "case", code = TTokenCode.TcCASE },
		new TResWord() { pString = "else", code = TTokenCode.TcELSE },
		new TResWord() { pString = "file", code = TTokenCode.TcFILE },
		new TResWord() { pString = "goto", code = TTokenCode.TcGOTO },
		new TResWord() { pString = "then", code = TTokenCode.TcTHEN },
		new TResWord() { pString = "type", code = TTokenCode.TcTYPE },
		new TResWord() { pString = "with", code = TTokenCode.TcWITH }
	};

	internal static TResWord[] rw5 =
	{
		new TResWord() { pString = "array", code = TTokenCode.TcARRAY },
		new TResWord() { pString = "begin", code = TTokenCode.TcBEGIN },
		new TResWord() { pString = "const", code = TTokenCode.TcCONST },
		new TResWord() { pString = "label", code = TTokenCode.TcLABEL },
		new TResWord() { pString = "until", code = TTokenCode.TcUNTIL },
		new TResWord() { pString = "while", code = TTokenCode.TcWHILE }
	};

	internal static TResWord[] rw6 =
	{
		new TResWord() { pString = "downto", code = TTokenCode.TcDOWNTO },
		new TResWord() { pString = "packed", code = TTokenCode.TcPACKED },
		new TResWord() { pString = "record", code = TTokenCode.TcRECORD },
		new TResWord() { pString = "repeat", code = TTokenCode.TcREPEAT }
	};

	internal static TResWord[] rw7 =
	{
		new TResWord() { pString = "program", code = TTokenCode.TcPROGRAM }
	};

	internal static TResWord[] rw8 =
	{
		new TResWord() { pString = "function", code = TTokenCode.TcFUNCTION }
	};

	internal static TResWord[] rw9 =
	{
		new TResWord() { pString = "procedure", code = TTokenCode.TcPROCEDURE }
	};

	//--------------------------------------------------------------
	//  The reserved word table
	//--------------------------------------------------------------

	internal static TResWord[] rwTable =
	{
		new TResWord( null, null ),
		new TResWord( rw2, rw3 ),
		new TResWord( rw4, rw5 ),
		new TResWord( rw6, rw7 ),
		new TResWord( rw8, rw9 )
	};

//--------------------------------------------------------------
//  InitializePredefinedTypes   Initialize the predefined
//                              types by entering their
//                              identifiers into the symbol
//                              table.
//
//      pSymtab : ptr to symbol table
//--------------------------------------------------------------


	public static void InitializePredefinedTypes( TSymtab pSymtab )
	{
		//--Enter the names of the predefined types and of "false"
		//--and "true" into the symbol table.
		TSymtabNode pIntegerId = pSymtab.Enter( "integer", TDefnCode.DcType );
		TSymtabNode pRealId = pSymtab.Enter( "real", TDefnCode.DcType );
		TSymtabNode pBooleanId = pSymtab.Enter( "boolean", TDefnCode.DcType );
		TSymtabNode pCharId = pSymtab.Enter( "char", TDefnCode.DcType );
		TSymtabNode pFalseId = pSymtab.Enter( "false", TDefnCode.DcConstant );
		TSymtabNode pTrueId = pSymtab.Enter( "true", TDefnCode.DcConstant );

		//--Create the predefined type objects.
		if ( pIntegerType == null )
		SetType( pIntegerType, new TType( TFormCode.FcScalar, sizeof( int ), pIntegerId ) );
		if ( pRealType == null )
		SetType( pRealType, new TType( TFormCode.FcScalar, sizeof( float ), pRealId ) );
		if ( pBooleanType == null )
		SetType( pBooleanType, new TType( TFormCode.FcEnum, sizeof( int ), pBooleanId ) );
		if ( pCharType == null )
		SetType( pCharType, new TType( TFormCode.FcScalar, sizeof( char ), pCharId ) );

		//--Link each predefined type's id node to its type object.
		SetType( pIntegerId.pType, pIntegerType );
		SetType( pRealId.pType, pRealType );
		SetType( pBooleanId.pType, pBooleanType );
		SetType( pCharId.pType, pCharType );

		//--More initialization for the boolean type object.
		pBooleanType.enumeration.max = 1; // max value
		pBooleanType.enumeration.pConstIds = pFalseId; // first constant

		//--More initialization for the "false" and "true" id nodes.
		pFalseId.defn.constant.value.integer = 0;
		pTrueId.defn.constant.value.integer = 1;
		SetType( pTrueId.pType, pBooleanType );
		SetType( pFalseId.pType, pBooleanType );
		pFalseId.next = pTrueId; // "false" node points to "true" node

		//--Initialize the dummy type object that will be used
		//--for erroneous type definitions and for typeless objects.
		SetType( pDummyType, new TType( TFormCode.FcNone, 1, null ) );
	}

//--------------------------------------------------------------
//  RemovePredefinedTypes       Remove the predefined types.
//--------------------------------------------------------------

	public static void RemovePredefinedTypes()
	{
		RemoveType( pIntegerType );
		RemoveType( pRealType );
		RemoveType( pBooleanType );
		RemoveType( pCharType );
		RemoveType( pDummyType );
	}

	internal static string[] formStrings = new [] { "*** Error ***", "Scalar", "Enumeration", "Subrange", "Array", "Record" };

	//--Pointers to predefined types.
	public static TType pIntegerType = null;
	public static TType pRealType = null;
	public static TType pBooleanType = null;
	public static TType pCharType = null;
	public static TType pDummyType = null;
	
	//--------------------------------------------------------------
	//  Runtime stack frame offsets
	//--------------------------------------------------------------

	public static readonly int procLocalsStackFrameOffset = 0;
	public static readonly int funcLocalsStackFrameOffset = -4;
	public static readonly int parametersStackFrameOffset = +6;
}	
