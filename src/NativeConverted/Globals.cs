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
    //  TokenIn     Check if a token code is in the token list.
    //
    //      tc    : token code
    //      pList : ptr to tcDummy-terminated token list
    //
    //  Return:  true if in list, false if not or empty list
    //--------------------------------------------------------------
    public static bool TokenIn ( TTokenCode tc, IEnumerable<TTokenCode> pList )
    {
        if (pList == null)
            return false; // empty list

        return pList.Any(x => x == tc);
    }

    public static int currentNestingLevel = 0;
    public static int currentLineNumber = 0;

    public static TSymtab globalSymtab = new TSymtab(); // the global symbol table
    public static int cntSymtabs = 0; // symbol table counter
    public static TSymtab pSymtabList; // ptr to head of symtab list
    public static TSymtab[] vpSymtabs; // ptr to vector of symtab ptrs

    //--------------------------------------------------------------
    //  Token lists
    //--------------------------------------------------------------

    //TODO: Move to TTokens
    //--Tokens that can start a procedure or function definition.
    public static readonly TTokenCode[] tlProcFuncStart = new[] {
        TTokenCode.TcPROCEDURE, TTokenCode.TcFUNCTION, TTokenCode.TcDummy
    };

    //--Tokens that can follow a procedure or function definition.
    public static readonly TTokenCode[] tlProcFuncFollow = new[] { TTokenCode.TcSemicolon, TTokenCode.TcDummy };

    //--Tokens that can follow a routine header.
    public static readonly TTokenCode[] tlHeaderFollow = new[] { TTokenCode.TcSemicolon, TTokenCode.TcDummy };

    //--Tokens that can follow a program or procedure id in a header.
    public static readonly TTokenCode[] tlProgProcIdFollow = new[] { TTokenCode.TcLParen, TTokenCode.TcSemicolon, TTokenCode.TcDummy };

    //--Tokens that can follow a function id in a header.
    public static readonly TTokenCode[] tlFuncIdFollow = new[] {
        TTokenCode.TcLParen, TTokenCode.TcColon, TTokenCode.TcSemicolon, TTokenCode.TcDummy
    };

    //--Tokens that can follow an actual variable parameter.
    public static readonly TTokenCode[] tlActualVarParmFollow = new[] { TTokenCode.TcComma, TTokenCode.TcRParen, TTokenCode.TcDummy };

    //--Tokens that can follow a formal parameter list.
    public static readonly TTokenCode[] tlFormalParmsFollow = new[] { TTokenCode.TcRParen, TTokenCode.TcSemicolon, TTokenCode.TcDummy };

    //--Tokens that can start a declaration.
    public static readonly TTokenCode[] tlDeclarationStart = new[] {
       TTokenCode.TcCONST, TTokenCode.TcTYPE, TTokenCode.TcVAR, TTokenCode.TcPROCEDURE, TTokenCode.TcFUNCTION, TTokenCode.TcDummy
    };

    //--Tokens that can follow a declaration.
    public static readonly TTokenCode[] tlDeclarationFollow = new[] { TTokenCode.TcSemicolon, TTokenCode.TcIdentifier, TTokenCode.TcDummy };

    //--Tokens that can start an identifier or field.
    public static readonly TTokenCode[] tlIdentifierStart = new[] { TTokenCode.TcIdentifier, TTokenCode.TcDummy };

    //--Tokens that can follow an identifier or field.
    public static readonly TTokenCode[] tlIdentifierFollow = new[] {
        TTokenCode.TcComma, TTokenCode.TcIdentifier, TTokenCode.TcColon, TTokenCode.TcSemicolon, TTokenCode.TcDummy
    };

    //--Tokens that can follow an identifier or field sublist.
    public static readonly TTokenCode[] tlSublistFollow = new[] { TTokenCode.TcColon, TTokenCode.TcDummy };

    //--Tokens that can follow a field declaration.
    public static readonly TTokenCode[] tlFieldDeclFollow = new[] {
        TTokenCode.TcSemicolon, TTokenCode.TcIdentifier, TTokenCode.TcEND, TTokenCode.TcDummy
    };

    //--Tokens that can start an enumeration constant.
    public static readonly TTokenCode[] tlEnumConstStart = new[] { TTokenCode.TcIdentifier, TTokenCode.TcDummy };

    //--Tokens that can follow an enumeration constant.
    public static readonly TTokenCode[] tlEnumConstFollow = new[] {
        TTokenCode.TcComma, TTokenCode.TcIdentifier, TTokenCode.TcRParen, TTokenCode.TcSemicolon, TTokenCode.TcDummy 
    };

    //--Tokens that can follow a subrange limit.
    public static readonly TTokenCode[] tlSubrangeLimitFollow = new[] {
        TTokenCode.TcDotDot, TTokenCode.TcIdentifier, TTokenCode.TcPlus, TTokenCode.TcMinus, TTokenCode.TcString,
        TTokenCode.TcRBracket, TTokenCode.TcComma, TTokenCode.TcSemicolon, TTokenCode.TcOF, TTokenCode.TcDummy
    };

    //--Tokens that can start an index type.
    public static readonly TTokenCode[] tlIndexStart = new[] {
        TTokenCode.TcIdentifier, TTokenCode.TcNumber, TTokenCode.TcString, TTokenCode.TcLParen, TTokenCode.TcPlus, TTokenCode.TcMinus,
        TTokenCode.TcDummy
    };

    //--Tokens that can follow an index type.
    public static readonly TTokenCode[] tlIndexFollow = new[] {
        TTokenCode.TcComma, TTokenCode.TcRBracket, TTokenCode.TcOF, TTokenCode.TcSemicolon, TTokenCode.TcDummy
    };

    //--Tokens that can follow the index type list.
    public static readonly TTokenCode[] tlIndexListFollow = new[] {
        TTokenCode.TcOF, TTokenCode.TcIdentifier, TTokenCode.TcLParen, TTokenCode.TcARRAY, TTokenCode.TcRECORD,
        TTokenCode.TcPlus, TTokenCode.TcMinus, TTokenCode.TcNumber, TTokenCode.TcString, TTokenCode.TcSemicolon, TTokenCode.TcDummy
    };

    //--Tokens that can start a statement.
    public static readonly TTokenCode[] tlStatementStart = new[] {
        TTokenCode.TcBEGIN, TTokenCode.TcCASE, TTokenCode.TcFOR, TTokenCode.TcIF, TTokenCode.TcREPEAT, TTokenCode.TcWHILE, TTokenCode.TcIdentifier,
        TTokenCode.TcDummy
    };

    //--Tokens that can follow a statement.
    public static readonly TTokenCode[] tlStatementFollow = new[] {
        TTokenCode.TcSemicolon, TTokenCode.TcPeriod, TTokenCode.TcEND, TTokenCode.TcELSE, TTokenCode.TcUNTIL, TTokenCode.TcDummy
    };

    //--Tokens that can start a CASE label.
    public static readonly TTokenCode[] tlCaseLabelStart = new[] {
        TTokenCode.TcIdentifier, TTokenCode.TcNumber, TTokenCode.TcPlus, TTokenCode.TcMinus, TTokenCode.TcString, TTokenCode.TcDummy
    };

    //--Tokens that can start an expression.
    public static readonly TTokenCode[] tlExpressionStart = new[] {
        TTokenCode.TcPlus, TTokenCode.TcMinus, TTokenCode.TcIdentifier, TTokenCode.TcNumber, TTokenCode.TcString,
        TTokenCode.TcNOT, TTokenCode.TcLParen, TTokenCode.TcDummy
    };

    //--Tokens that can follow an expression.
    public static readonly TTokenCode[] tlExpressionFollow = new[] {
        TTokenCode.TcComma, TTokenCode.TcRParen, TTokenCode.TcRBracket, TTokenCode.TcColon, TTokenCode.TcTHEN, TTokenCode.TcTO, TTokenCode.TcDOWNTO,
        TTokenCode.TcDO, TTokenCode.TcOF, TTokenCode.TcDummy
    };

    //--Tokens that can start a subscript or field.
    public static readonly TTokenCode[] tlSubscriptOrFieldStart = new[] {
        TTokenCode.TcLBracket, TTokenCode.TcPeriod, TTokenCode.TcDummy
    };

    //--Relational operators.
    public static readonly TTokenCode[] tlRelOps = new[] {
        TTokenCode.TcEqual, TTokenCode.TcNe, TTokenCode.TcLt, TTokenCode.TcGt, TTokenCode.TcLe, TTokenCode.TcGe, TTokenCode.TcDummy
    };

    //--Unary + and - operators.
    public static readonly TTokenCode[] tlUnaryOps = new[] { TTokenCode.TcPlus, TTokenCode.TcMinus, TTokenCode.TcDummy };

    //--Additive operators.
    public static readonly TTokenCode[] tlAddOps = new[] {
        TTokenCode.TcPlus, TTokenCode.TcMinus, TTokenCode.TcOR, TTokenCode.TcDummy
    };

    //--Multiplicative operators.
    public static readonly TTokenCode[] tlMulOps = new[] {
        TTokenCode.TcStar, TTokenCode.TcSlash, TTokenCode.TcDIV, TTokenCode.TcMOD, TTokenCode.TcAND, TTokenCode.TcDummy
    };

    //--Tokens that can end a program.
    public static readonly TTokenCode[] tlProgramEnd = new[] { TTokenCode.TcPeriod, TTokenCode.TcDummy };

    //--Individual tokens.
    public static readonly TTokenCode[] tlColonEqual = new[] { TTokenCode.TcColonEqual, TTokenCode.TcDummy };
    public static readonly TTokenCode[] tlDO = new[] { TTokenCode.TcDO, TTokenCode.TcDummy };
    public static readonly TTokenCode[] tlTHEN = new[] { TTokenCode.TcTHEN, TTokenCode.TcDummy };
    public static readonly TTokenCode[] tlTODOWNTO = new[] { TTokenCode.TcTO, TTokenCode.TcDOWNTO, TTokenCode.TcDummy };
    public static readonly TTokenCode[] tlOF = new[] { TTokenCode.TcOF, TTokenCode.TcDummy };
    public static readonly TTokenCode[] tlColon = new[] { TTokenCode.TcColon, TTokenCode.TcDummy };
    public static readonly TTokenCode[] tlEND = new[] { TTokenCode.TcEND, TTokenCode.TcDummy };

    //--------------------------------------------------------------
    //  main
    //--------------------------------------------------------------	

    //--------------------------------------------------------------
    //  Registers and instructions
    //--------------------------------------------------------------

    public static string[] registers = new[] { "ax", "ah", "al", "bx", "bh", "bl", "cx", "ch", "cl", "dx", "dh", "dl", "cs", "ds", "es", "ss", "sp", "bp", "si", "di" };

    public static string[] instructions = new[] { "mov", "rep\tmovsb", "lea", "xchg", "cmp", "repe\tcmpsb", "pop", "push", "and", "or", "xor", "neg", "inc", "dec", "add", "sub", "imul", "idiv", "cld", "call", "ret", "jmp", "jl", "jle", "je", "jne", "jge", "jg" };

    //--------------------------------------------------------------
    //  EmitIdComment           Emit an identifier and its
    //                          modifiers as a comment.
    //--------------------------------------------------------------

    //TODO: Move to TTokens
    //--Tokens that can start an identifier modifier.
    public static TTokenCode[] tlIdModStart = new[] { TTokenCode.TcLBracket, TTokenCode.TcLParen, TTokenCode.TcPeriod, TTokenCode.TcDummy };

    //TODO: Move to TTokens
    //--Tokens that can end an identifier modifier.
    public static TTokenCode[] tlIdModEnd = new[] { TTokenCode.TcRBracket, TTokenCode.TcRParen, TTokenCode.TcDummy };

    public static void DisplayError ( string message )
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    //--------------------------------------------------------------
    //  AbortTranslation    A fatal error occurred during the
    //                      translation.  Print the abort code
    //                      to the error file and then exit.
    //
    //      ac : abort code
    //--------------------------------------------------------------
    public static void AbortTranslation ( TAbortCode ac )
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
    public static void Error ( TErrorCode ec )
    {
        const int maxSyntaxErrors = 25;

        var errorPosition = errorArrowOffset + inputPosition - 1;

        //--Print the arrow pointing to the token just scanned.
        if (errorArrowFlag)
        {
            //ORIGINAL LINE: sprintf(list.text, "%*s^", errorPosition, " ");
            list.text = "".PadLeft(errorPosition);
            list.PutLine();
        }

        //--Print the error message.
        list.text = System.String.Format("*** ERROR: {0}", errorMessages[(int)ec]);
        list.PutLine();

        if (++errorCount > maxSyntaxErrors)
        {
            list.PutLine("Too many syntax errors.  Translation aborted.");
            Globals.AbortTranslation(TAbortCode.AbortTooManySyntaxErrors);
        }
    }

    //--------------------------------------------------------------
    //  RuntimeError        Print the runtime error message and then
    //                      abort the program.
    //
    //      ec : error code
    //--------------------------------------------------------------
    public static void RuntimeError ( TRuntimeErrorCode ec )
    {
        Console.WriteLine();
        Console.Write("*** RUNTIME ERROR in line ");
        Console.Write(currentLineNumber);
        Console.Write(": ");
        Console.Write(runtimeErrorMessages[(int)ec]);
        Console.WriteLine();
    }

    public static int errorCount = 0; // count of syntax errors
    public static bool errorArrowFlag = true; // true if print arrows under syntax errors, false if not
    public static int errorArrowOffset = 8; // offset for printing the error arrow

    //--------------------------------------------------------------
    //  Abort messages      Keyed to enumeration type TAbortCode.
    //--------------------------------------------------------------

    public static string[] abortMsg = new[] { null, "Invalid command line arguments", "Failed to open source file", "Failed to open intermediate form file", "Failed to open assembly file", "Too many syntax errors", "Stack overflow", "Code segment overflow", "Nesting too deep", "Runtime error", "Unimplemented feature" };

    //--------------------------------------------------------------
    //  Error messages      Keyed to enumeration type TErrorCode.
    //--------------------------------------------------------------

    public static string[] errorMessages = new[] { "No error", "Unrecognizable input", "Too many syntax errors", "Unexpected end of file", "Invalid number", "Invalid fraction", "Invalid exponent", "Too many digits", "Real literal out of range", "Integer literal out of range", "Missing )", "Invalid expression", "Invalid assignment statement", "Missing identifier", "Missing :=", "Undefined identifier", "Stack overflow", "Invalid statement", "Unexpected token", "Missing ;", "Missing ,", "Missing DO", "Missing UNTIL", "Missing THEN", "Invalid FOR control variable", "Missing OF", "Invalid constant", "Missing constant", "Missing :", "Missing END", "Missing TO or DOWNTO", "Redefined identifier", "Missing =", "Invalid type", "Not a type identifier", "Invalid subrange type", "Not a constant identifier", "Missing ..", "Incompatible types", "Invalid assignment target", "Invalid identifier usage", "Incompatible assignment", "Min limit greater than max limit", "Missing [", "Missing ]", "Invalid index type", "Missing BEGIN", "Missing .", "Too many subscripts", "Invalid field", "Nesting too deep", "Missing PROGRAM", "Already specified in FORWARD", "Wrong number of actual parameters", "Invalid VAR parameter", "Not a record variable", "Missing variable", "Code segment overflow", "Unimplemented feature" };

    //--------------------------------------------------------------
    //  Runtime error messages      Keyed to enumeration type
    //                              TRuntimeErrorCode.
    //--------------------------------------------------------------

    public static string[] runtimeErrorMessages = new[] { "No runtime error", "Runtime stack overflow", "Value out of range", "Invalid CASE expression value", "Division by zero", "Invalid standard function argument", "Invalid user input", "Unimplemented runtime feature" };

    public const TTokenCode mcLineMarker = ((TTokenCode)127);
    public const TTokenCode mcLocationMarker = ((TTokenCode)126);

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

    public static void InitializeStandardRoutines ( TSymtab pSymtab )
    {
        int i = 0;

        do
        {
            TStdRtn pSR = stdRtnList[i];
            TSymtabNode pRoutineId = pSymtab.Enter(pSR.pName, pSR.dc);

            pRoutineId.defn.routine = new TDefn.RoutineDefn() {
                which = pSR.rc,
                parmCount = 0,
                totalParmSize = 0,
                totalLocalSize = 0,
                locals = new TLocalIds(),
                pSymtab = null,
                pIcode = null,
            };        
            TType.SetType(ref pRoutineId.pType, pDummyType);
        } while (stdRtnList[++i].pName != null);
    }

    public static TCharCode[] charCodeMap = new TCharCode[128]; // maps a character to its code

    public static int asmLabelIndex = 0; // assembly label index
    public static bool xrefFlag = false; // true = cross-referencing on, false = off

    //              *************************
    //              *                       *
    //              *  Reserved Word Table  *
    //              *                       *
    //              *************************

    public const int minResWordLen = 2; // min and max reserved
    public const int maxResWordLen = 9; //   word lengths

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

    internal static TResWord[][] rwTable = new[] { null, null, rw2, rw3, rw4, rw5, rw6, rw7, rw8, rw9 };

    //--------------------------------------------------------------
    //  InitializePredefinedTypes   Initialize the predefined
    //                              types by entering their
    //                              identifiers into the symbol
    //                              table.
    //
    //      pSymtab : ptr to symbol table
    //--------------------------------------------------------------
    public static void InitializePredefinedTypes ( TSymtab pSymtab )
    {
        //--Enter the names of the predefined types and of "false"
        //--and "true" into the symbol table.
        var pIntegerId = pSymtab.Enter("integer", TDefnCode.DcType);
        var pRealId = pSymtab.Enter("real", TDefnCode.DcType);
        var pBooleanId = pSymtab.Enter("boolean", TDefnCode.DcType);
        var pCharId = pSymtab.Enter("char", TDefnCode.DcType);
        var pFalseId = pSymtab.Enter("false", TDefnCode.DcConstant);
        var pTrueId = pSymtab.Enter("true", TDefnCode.DcConstant);

        //--Create the predefined type objects.
        if (pIntegerType == null)
            TType.SetType(ref pIntegerType, new TType(TFormCode.FcScalar, sizeof(int), pIntegerId));
        if (pRealType == null)
            TType.SetType(ref pRealType, new TType(TFormCode.FcScalar, sizeof(float), pRealId));
        if (pBooleanType == null)
            TType.SetType(ref pBooleanType, new TType(TFormCode.FcEnum, sizeof(int), pBooleanId));
        if (pCharType == null)
            TType.SetType(ref pCharType, new TType(TFormCode.FcScalar, sizeof(char), pCharId));

        //--Link each predefined type's id node to its type object.
        TType.SetType(ref pIntegerId.pType, pIntegerType);
        TType.SetType(ref pRealId.pType, pRealType);
        TType.SetType(ref pBooleanId.pType, pBooleanType);
        TType.SetType(ref pCharId.pType, pCharType);

        //--More initialization for the boolean type object.
        pBooleanType.enumeration.max = 1; // max value
        pBooleanType.enumeration.pConstIds = pFalseId; // first constant

        //--More initialization for the "false" and "true" id nodes.
        pFalseId.defn.constant = new TDefn.ConstantDefn() { value = new TDataValue() { integer = 0 } };
        pTrueId.defn.constant = new TDefn.ConstantDefn() { value = new TDataValue() { integer = 1 } };        
        TType.SetType(ref pTrueId.pType, pBooleanType);
        TType.SetType(ref pFalseId.pType, pBooleanType);
        pFalseId.next = pTrueId; // "false" node points to "true" node

        //--Initialize the dummy type object that will be used
        //--for erroneous type definitions and for typeless objects.
        TType.SetType(ref pDummyType, new TType(TFormCode.FcNone, 1, null));
    }

    //--------------------------------------------------------------
    //  RemovePredefinedTypes       Remove the predefined types.
    //--------------------------------------------------------------
    public static void RemovePredefinedTypes ()
    {
        TType.RemoveType(ref pIntegerType);
        TType.RemoveType(ref pRealType);
        TType.RemoveType(ref pBooleanType);
        TType.RemoveType(ref pCharType);
        TType.RemoveType(ref pDummyType);
    }   

    internal static string[] formStrings = new[] { "*** Error ***", "Scalar", "Enumeration", "Subrange", "Array", "Record" };

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
