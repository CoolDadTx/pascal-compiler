public static class GlobalMembers
{
public static class GlobalMembers
{
public static class GlobalMembers
{
public static class GlobalMembers
{
public static class GlobalMembers
{
public static class GlobalMembers
{
using System;

public static class GlobalMembers
{
public static class GlobalMembers
{
public static class GlobalMembers
{
public static class GlobalMembers
{
using System;

public static class GlobalMembers
{
using System;

public static class GlobalMembers
{
public static class GlobalMembers
{
public static class GlobalMembers
{
public static class GlobalMembers
{
public static class GlobalMembers
{
using System;

public static class GlobalMembers
{
using System;

public static class GlobalMembers
{
public static class GlobalMembers
{
using System;

public static class GlobalMembers
{
public static class GlobalMembers
{
public static class GlobalMembers
{
public static class GlobalMembers
{
public static class GlobalMembers
{
using System;

public static class GlobalMembers
{
public static class GlobalMembers
{
public static class GlobalMembers
{
public static class GlobalMembers
{
public static class GlobalMembers
{
using System;

public static class GlobalMembers
{
	//fig 2-4
	//  *************************************************************
	//  *                                                           *
	//  *   I / O   B U F F E R S                                   *
	//  *                                                           *
	//  *   Process text I/O files.  Included are member functions  *
	//  *   to read the source file and write to the list file.     *
	//  *                                                           *
	//  *   CLASSES: TTextInBuffer,  TSourceBuffer                  *
	//  *            TTextOutBuffer, TListBuffer                    *
	//  *                                                           *
	//  *   FILE:    prog2-1/buffer.cpp                             *
	//  *                                                           *
	//  *   MODULE:  Buffer                                         *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************

	//fig 2-3
	//  *************************************************************
	//  *                                                           *
	//  *   I / O   B U F F E R S   (Header)                        *
	//  *                                                           *
	//  *   CLASSES: TTextInBuffer,  TSourceBuffer                  *
	//  *            TTextOutBuffer, TListBuffer                    *
	//  *                                                           *
	//  *   FILE:    prog2-1/buffer.h                               *
	//  *                                                           *
	//  *   MODULE:  Buffer                                         *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************



	//              ***********
	//              *         *
	//              *  Input  *
	//              *         *
	//              ***********

//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern char eofChar;
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern int inputPosition;
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern int listFlag;
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern int level;

	public static readonly int maxInputBufferSize = 256;

//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern TListBuffer list;

	//endfig




	//              ***********************
	//              *                     *
	//              *  Text Input Buffer  *
	//              *                     *
	//              ***********************

	public static char eofChar = 0x7F; // special end-of-file character
	public static int inputPosition; // "virtual" position of the current char
				   //   in the input buffer (with tabs expanded)
	public static int listFlag = true; // true if list source lines, else false

	//              *****************
	//              *               *
	//              *  List Buffer  *
	//              *               *
	//              *****************

	public static readonly int maxPrintLineLength = 80;
	public static readonly int maxLinesPerPage = 50;

	public static TListBuffer list = new TListBuffer(); // the list file buffer
	//endfig


}
	//  *************************************************************
	//  *                                                           *
	//  *   C O M M O N                                             *
	//  *                                                           *
	//  *   FILE:    prog8-1/common.cpp                             *
	//  *                                                           *
	//  *   MODULE:  Common                                         *
	//  *                                                           *
	//  *   Data and routines common to the front and back ends.    *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************

	//  *************************************************************
	//  *                                                           *
	//  *   C O M M O N   (Header)                                  *
	//  *                                                           *
	//  *   FILE:    prog8-1/common.h                               *
	//  *                                                           *
	//  *   MODULE:  Common                                         *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************



//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern int currentLineNumber;
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern int currentNestingLevel;

//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern TSymtab globalSymtab;
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern int cntSymtabs;
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern TSymtab *pSymtabList;
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern TSymtab **vpSymtabs;

	//--------------------------------------------------------------
	//  Token lists
	//--------------------------------------------------------------

//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlProcFuncStart[], tlProcFuncFollow[], tlHeaderFollow[];

//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlProgProcIdFollow[], tlFuncIdFollow[], tlActualVarParmFollow[], tlFormalParmsFollow[];

//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlDeclarationStart[], tlDeclarationFollow[], tlIdentifierStart[], tlIdentifierFollow[], tlSublistFollow[], tlFieldDeclFollow[];

//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlEnumConstStart[], tlEnumConstFollow[], tlSubrangeLimitFollow[];

//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlIndexStart[], tlIndexFollow[], tlIndexListFollow[];

//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlStatementStart[], tlStatementFollow[];
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlCaseLabelStart[];

//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlExpressionStart[], tlExpressionFollow[];
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlRelOps[], tlUnaryOps[], tlAddOps[], tlMulOps[];

//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlProgramEnd[];

//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlColonEqual[];
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlDO[];
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlTHEN[];
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlTODOWNTO[];
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlOF[];
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlColon[];
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlEND[];

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

	//fig 8-9
	//--Tokens that can start a procedure or function definition.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlProcFuncStart[] = { tcPROCEDURE, tcFUNCTION, tcDummy };

	//--Tokens that can follow a procedure or function definition.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlProcFuncFollow[] = { tcSemicolon, tcDummy };

	//--Tokens that can follow a routine header.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlHeaderFollow[] = { tcSemicolon, tcDummy };

	//--Tokens that can follow a program or procedure id in a header.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlProgProcIdFollow[] = { tcLParen, tcSemicolon, tcDummy };

	//--Tokens that can follow a function id in a header.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlFuncIdFollow[] = { tcLParen, tcColon, tcSemicolon, tcDummy };

	//--Tokens that can follow an actual variable parameter.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlActualVarParmFollow[] = { tcComma, tcRParen, tcDummy };

	//--Tokens that can follow a formal parameter list.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlFormalParmsFollow[] = { tcRParen, tcSemicolon, tcDummy };
	//endfig

	//--Tokens that can start a declaration.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlDeclarationStart[] = { tcCONST, tcTYPE, tcVAR, tcPROCEDURE, tcFUNCTION, tcDummy };

	//--Tokens that can follow a declaration.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlDeclarationFollow[] = { tcSemicolon, tcIdentifier, tcDummy };

	//--Tokens that can start an identifier or field.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlIdentifierStart[] = { tcIdentifier, tcDummy };

	//--Tokens that can follow an identifier or field.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlIdentifierFollow[] = { tcComma, tcIdentifier, tcColon, tcSemicolon, tcDummy };

	//--Tokens that can follow an identifier or field sublist.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlSublistFollow[] = { tcColon, tcDummy };

	//--Tokens that can follow a field declaration.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlFieldDeclFollow[] = { tcSemicolon, tcIdentifier, tcEND, tcDummy };

	//--Tokens that can start an enumeration constant.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlEnumConstStart[] = { tcIdentifier, tcDummy };

	//--Tokens that can follow an enumeration constant.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlEnumConstFollow[] = { tcComma, tcIdentifier, tcRParen, tcSemicolon, tcDummy };

	//--Tokens that can follow a subrange limit.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlSubrangeLimitFollow[] = { tcDotDot, tcIdentifier, tcPlus, tcMinus, tcString, tcRBracket, tcComma, tcSemicolon, tcOF, tcDummy };

	//--Tokens that can start an index type.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlIndexStart[] = { tcIdentifier, tcNumber, tcString, tcLParen, tcPlus, tcMinus, tcDummy };

	//--Tokens that can follow an index type.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlIndexFollow[] = { tcComma, tcRBracket, tcOF, tcSemicolon, tcDummy };

	//--Tokens that can follow the index type list.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlIndexListFollow[] = { tcOF, tcIdentifier, tcLParen, tcARRAY, tcRECORD, tcPlus, tcMinus, tcNumber, tcString, tcSemicolon, tcDummy };

	//--Tokens that can start a statement.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlStatementStart[] = { tcBEGIN, tcCASE, tcFOR, tcIF, tcREPEAT, tcWHILE, tcIdentifier, tcDummy };

	//--Tokens that can follow a statement.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlStatementFollow[] = { tcSemicolon, tcPeriod, tcEND, tcELSE, tcUNTIL, tcDummy };

	//--Tokens that can start a CASE label.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlCaseLabelStart[] = { tcIdentifier, tcNumber, tcPlus, tcMinus, tcString, tcDummy };

	//--Tokens that can start an expression.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlExpressionStart[] = { tcPlus, tcMinus, tcIdentifier, tcNumber, tcString, tcNOT, tcLParen, tcDummy };

	//--Tokens that can follow an expression.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlExpressionFollow[] = { tcComma, tcRParen, tcRBracket, tcColon, tcTHEN, tcTO, tcDOWNTO, tcDO, tcOF, tcDummy };

	//--Tokens that can start a subscript or field.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlSubscriptOrFieldStart[] = { tcLBracket, tcPeriod, tcDummy };

	//--Relational operators.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlRelOps[] = { tcEqual, tcNe, tcLt, tcGt, tcLe, tcGe, tcDummy };

	//--Unary + and - operators.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlUnaryOps[] = { tcPlus, tcMinus, tcDummy };

	//--Additive operators.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlAddOps[] = { tcPlus, tcMinus, tcOR, tcDummy };

	//--Multiplicative operators.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlMulOps[] = { tcStar, tcSlash, tcDIV, tcMOD, tcAND, tcDummy };

	//--Tokens that can end a program.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlProgramEnd[] = { tcPeriod, tcDummy };

	//--Individual tokens.
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlColonEqual[] = {tcColonEqual, tcDummy};
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlDO[] = {tcDO, tcDummy};
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlTHEN[] = {tcTHEN, tcDummy};
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlTODOWNTO[] = {tcTO, tcDOWNTO, tcDummy};
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlOF[] = {tcOF, tcDummy};
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlColon[] = {tcColon, tcDummy};
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern const TTokenCode tlEND[] = {tcEND, tcDummy};


}
	//fig 14-21
	//  *************************************************************
	//  *                                                           *
	//  *   Program 14-1: Compiler                                  *
	//  *                                                           *
	//  *   Compile a Pascal program into assembly code.            *
	//  *                                                           *
	//  *   FILE:   prog14-1/compile.cpp                            *
	//  *                                                           *
	//  *   USAGE:  compile <source file> <assembly file>           *
	//  *                                                           *
	//  *               <source file>    name of the source file    *
	//  *               <assembly file>  name of the assembly       *
	//  *                                  language output file     *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************

	//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
	//ORIGINAL LINE: #define Emit0(opcode) { Operator(opcode); pAsmBuffer->PutLine(); }
	//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
	//ORIGINAL LINE: #define Emit1(opcode, operand1) { Operator(opcode); pAsmBuffer->Put('\t'); operand1; pAsmBuffer->PutLine(); }
	//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
	//ORIGINAL LINE: #define Emit2(opcode, operand1, operand2) { Operator(opcode); pAsmBuffer->Put('\t'); operand1; pAsmBuffer->Put(','); operand2; pAsmBuffer->PutLine(); }


	//--------------------------------------------------------------
	//  main
	//--------------------------------------------------------------

	static void Main( void argc, string[] args )
	{
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//	extern int execFlag;

		//--Check the command line arguments.
		if ( argc != 3 )
		{
		cerr << "Usage: compile <source file> <asssembly file>" << "\n";
		AbortTranslation( TAbortCode.AbortInvalidCommandLineArgs );
		}

		execFlag = false;

		//--Create the parser for the source file,
		//--and then parse the file.
		TParser pParser = new TParser( new TSourceBuffer( args[1] ) );
		TSymtabNode pProgramId = pParser.Parse();
		if ( pParser != null )
		pParser.Dispose();

		//--If there were no syntax errors, convert the symbol tables,
		//--and create and invoke the backend code generator.
		if ( errorCount == 0 )
		{
		vpSymtabs = new TSymtab[cntSymtabs];
		for ( TSymtab * pSt = pSymtabList; pSt != null; pSt = pSt.Next() )
			pSt.Convert( vpSymtabs );

		TBackend pBackend = new TCodeGenerator( args[2] );
		pBackend.Go( pProgramId );

		vpSymtabs = null;
		if ( pBackend != null )
		pBackend.Dispose();
		}
	}
	//endfig

}
	//fig 12-8
	//  *************************************************************
	//  *                                                           *
	//  *   E M I T   A S S E M B L Y   S T A T E M E N T S         *
	//  *                                                           *
	//  *   Routines for generating and emitting                    *
	//  *   language statements.                                    *
	//  *                                                           *
	//  *   CLASSES: TAssemblyBuffer, TCodeGenerator                *
	//  *                                                           *
	//  *   FILE:    prog13-1/emitasm.cpp                           *
	//  *                                                           *
	//  *   MODULE:  Code generator                                 *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************

	//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
	//ORIGINAL LINE: #define Emit0(opcode) { Operator(opcode); pAsmBuffer->PutLine(); }
	//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
	//ORIGINAL LINE: #define Emit1(opcode, operand1) { Operator(opcode); pAsmBuffer->Put('\t'); operand1; pAsmBuffer->PutLine(); }
	//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
	//ORIGINAL LINE: #define Emit2(opcode, operand1, operand2) { Operator(opcode); pAsmBuffer->Put('\t'); operand1; pAsmBuffer->Put(','); operand2; pAsmBuffer->PutLine(); }


	//--------------------------------------------------------------
	//  Registers and instructions
	//--------------------------------------------------------------

	public static string[] registers = { "ax", "ah", "al", "bx", "bh", "bl", "cx", "ch", "cl", "dx", "dh", "dl", "cs", "ds", "es", "ss", "sp", "bp", "si", "di" };

	public static string[] instructions = { "mov", "rep\tmovsb", "lea", "xchg", "cmp", "repe\tcmpsb", "pop", "push", "and", "or", "xor", "neg", "inc", "dec", "add", "sub", "imul", "idiv", "cld", "call", "ret", "jmp", "jl", "jle", "je", "jne", "jge", "jg" };
}
}
}
}

	//--------------------------------------------------------------
	//  EmitIdComment           Emit an identifier and its
	//                          modifiers as a comment.
	//--------------------------------------------------------------

	//--Tokens that can start an identifier modifier.
	public static TTokenCode[] tlIdModStart = { TTokenCode.TcLBracket, TTokenCode.TcLParen, TTokenCode.TcPeriod, TTokenCode.TcDummy };

	//--Tokens that can end an identifier modifier.
	public static TTokenCode[] tlIdModEnd = { TTokenCode.TcRBracket, TTokenCode.TcRParen, TTokenCode.TcDummy };
}
}
}
	//  *************************************************************
	//  *                                                           *
	//  *   E R R O R S                                             *
	//  *                                                           *
	//  *   Error routines to print error messages and to abort     *
	//  *   the translation.                                        *
	//  *                                                           *
	//  *   FILE:    prog11-1/error.cpp                             *
	//  *                                                           *
	//  *   MODULE:  Error                                          *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************

	//  *************************************************************
	//  *                                                           *
	//  *   E R R O R S   (Header)                                  *
	//  *                                                           *
	//  *   FILE:    prog5-2/error.h                                *
	//  *                                                           *
	//  *   MODULE:  Error                                          *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************


//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern int errorCount;
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern int errorArrowFlag;
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern int errorArrowOffset;

//--------------------------------------------------------------
//  AbortTranslation    A fatal error occurred during the
//                      translation.  Print the abort code
//                      to the error file and then exit.
//
//      ac : abort code
//--------------------------------------------------------------


	public static void AbortTranslation( TAbortCode ac )
	{
		cerr << "*** Fatal translator error: " << abortMsg[-ac] << "\n";
		Environment.Exit( ac );
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
		list.text = string.Format( "%*s^", errorPosition, " " );
		list.PutLine();
		}

		//--Print the error message.
		list.text = string.Format( "*** ERROR: {0}", errorMessages[( int )ec] );
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
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//	extern int currentLineNumber;

		Console.Write( "\n" );
		Console.Write( "*** RUNTIME ERROR in line " );
		Console.Write( currentLineNumber );
		Console.Write( ": " );
		Console.Write( runtimeErrorMessages[( int )ec] );
		Console.Write( "\n" );
	}
	//endfig




	public static int errorCount = 0; // count of syntax errors
	public static int errorArrowFlag = true; // true if print arrows under syntax
					  //   errors, false if not
	public static int errorArrowOffset = 8; // offset for printing the error arrow

	//--------------------------------------------------------------
	//  Abort messages      Keyed to enumeration type TAbortCode.
	//--------------------------------------------------------------

//C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent to pointers to value types:
//ORIGINAL LINE: char *abortMsg[] = { null, "Invalid command line arguments", "Failed to open source file", "Failed to open intermediate form file", "Failed to open assembly file", "Too many syntax errors", "Stack overflow", "Code segment overflow", "Nesting too deep", "Runtime error", "Unimplemented feature"};
	public static char[] abortMsg = { null, "Invalid command line arguments", "Failed to open source file", "Failed to open intermediate form file", "Failed to open assembly file", "Too many syntax errors", "Stack overflow", "Code segment overflow", "Nesting too deep", "Runtime error", "Unimplemented feature" };

	//--------------------------------------------------------------
	//  Error messages      Keyed to enumeration type TErrorCode.
	//--------------------------------------------------------------

	public static string[] errorMessages = { "No error", "Unrecognizable input", "Too many syntax errors", "Unexpected end of file", "Invalid number", "Invalid fraction", "Invalid exponent", "Too many digits", "Real literal out of range", "Integer literal out of range", "Missing )", "Invalid expression", "Invalid assignment statement", "Missing identifier", "Missing :=", "Undefined identifier", "Stack overflow", "Invalid statement", "Unexpected token", "Missing ;", "Missing ,", "Missing DO", "Missing UNTIL", "Missing THEN", "Invalid FOR control variable", "Missing OF", "Invalid constant", "Missing constant", "Missing :", "Missing END", "Missing TO or DOWNTO", "Redefined identifier", "Missing =", "Invalid type", "Not a type identifier", "Invalid subrange type", "Not a constant identifier", "Missing ..", "Incompatible types", "Invalid assignment target", "Invalid identifier usage", "Incompatible assignment", "Min limit greater than max limit", "Missing [", "Missing ]", "Invalid index type", "Missing BEGIN", "Missing .", "Too many subscripts", "Invalid field", "Nesting too deep", "Missing PROGRAM", "Already specified in FORWARD", "Wrong number of actual parameters", "Invalid VAR parameter", "Not a record variable", "Missing variable", "Code segment overflow", "Unimplemented feature" };

	//--------------------------------------------------------------
	//  Runtime error messages      Keyed to enumeration type
	//                              TRuntimeErrorCode.
	//--------------------------------------------------------------

	public static string[] runtimeErrorMessages = { "No runtime error", "Runtime stack overflow", "Value out of range", "Invalid CASE expression value", "Division by zero", "Invalid standard function argument", "Invalid user input", "Unimplemented runtime feature" };

}
	//  *************************************************************
	//  *                                                           *
	//  *   I N T E R M E D I A T E   C O D E                       *
	//  *                                                           *
	//  *   Create and access the intermediate code implemented in  *
	//  *   memory.                                                 *
	//  *                                                           *
	//  *   CLASSES: icode                                          *
	//  *                                                           *
	//  *   FILE:    prog10-1/icode.cpp                             *
	//  *                                                           *
	//  *   MODULE:  Intermediate code                              *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************

	//  *************************************************************
	//  *                                                           *
	//  *   I N T E R M E D I A T E   C O D E   (Header)            *
	//  *                                                           *
	//  *   CLASSES: TIcode                                         *
	//  *                                                           *
	//  *   FILE:    prog10-1/icode.h                               *
	//  *                                                           *
	//  *   MODULE:  Intermediate code                              *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************



	public static readonly TTokenCode mcLineMarker = ( ( TTokenCode ) 127 );
	public static readonly TTokenCode mcLocationMarker = ( ( TTokenCode ) 126 );

	//--------------------------------------------------------------
	//  TIcode      Intermediate code subclass of TScanner.
	//--------------------------------------------------------------

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
	//class TSymtabNode;




	//--Vector of special symbol and reserved word strings.
	public const string symbolStrings = { null, null, null, null, null, null, "^", "*", "(", ")", "-", "+", "=", "[", "]", ":", ";", "<", ">", ",", ".", "/", ":=", "<=", ">=", "<>", "..", "and", "array", "begin", "case", "const", "div", "do", "downto", "else", "end", "file", "for", "function", "goto", "if", "in", "label", "mod", "nil", "not", "of", "or", "packed", "procedure", "program", "record", "repeat", "set", "then", "to", "type", "until", "var", "while", "with" };
	//endfig


}
	//  *************************************************************
	//  *                                                           *
	//  *   P A R S E R   (Declarations)                            *
	//  *                                                           *
	//  *   Parse constant definitions and variable and record      *
	//  *   field declarations.                                     *
	//  *                                                           *
	//  *   CLASSES: TParser                                        *
	//  *                                                           *
	//  *   FILE:    prog13-1/parsdecl.cpp                          *
	//  *                                                           *
	//  *   MODULE:  Parser                                         *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************



	public static int execFlag = true; // true for executor back end,
}
}
}
}
}
	public static TStdRtn[] stdRtnList =
	{
		{ "read", TRoutineCode.RcRead, TDefnCode.DcProcedure },
		{ "readln", TRoutineCode.RcReadln, TDefnCode.DcProcedure },
		{ "write", TRoutineCode.RcWrite, TDefnCode.DcProcedure },
		{ "writeln", TRoutineCode.RcWriteln, TDefnCode.DcProcedure },
		{ "abs", TRoutineCode.RcAbs, TDefnCode.DcFunction },
		{ "arctan", TRoutineCode.RcArctan, TDefnCode.DcFunction },
		{ "chr", TRoutineCode.RcChr, TDefnCode.DcFunction },
		{ "cos", TRoutineCode.RcCos, TDefnCode.DcFunction },
		{ "eof", TRoutineCode.RcEof, TDefnCode.DcFunction },
		{ "eoln", TRoutineCode.RcEoln, TDefnCode.DcFunction },
		{ "exp", TRoutineCode.RcExp, TDefnCode.DcFunction },
		{ "ln", TRoutineCode.RcLn, TDefnCode.DcFunction },
		{ "odd", TRoutineCode.RcOdd, TDefnCode.DcFunction },
		{ "ord", TRoutineCode.RcOrd, TDefnCode.DcFunction },
		{ "pred", TRoutineCode.RcPred, TDefnCode.DcFunction },
		{ "round", TRoutineCode.RcRound, TDefnCode.DcFunction },
		{ "sin", TRoutineCode.RcSin, TDefnCode.DcFunction },
		{ "sqr", TRoutineCode.RcSqr, TDefnCode.DcFunction },
		{ "sqrt", TRoutineCode.RcSqrt, TDefnCode.DcFunction },
		{ "succ", TRoutineCode.RcSucc, TDefnCode.DcFunction },
		{ "trunc", TRoutineCode.RcTrunc, TDefnCode.DcFunction },
		{ null }
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
	//endfig

}
}
}
}




	public static TCharCode[] charCodeMap = new TCharCode[128]; // maps a character to its code


}
	//  *************************************************************
	//  *                                                           *
	//  *   S Y M B O L   T A B L E                                 *
	//  *                                                           *
	//  *   Manage a symbol table.                      		*
	//  *                                                           *
	//  *	CLASSES: TDefn, TSymtabNode, TSymtab, TSymtabStack	*
	//  *		 TLineNumNode, TLineNumList			*
	//  *                                                           *
	//  *   FILE:    prog8-1/symtab.cpp                             *
	//  *                                                           *
	//  *   MODULE:  Symbol table                                   *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************

	//  *************************************************************
	//  *                                                           *
	//  *   S Y M B O L   T A B L E   (Header)                      *
	//  *                                                           *
	//  *   CLASSES: TDefn, TSymtabNode, TSymtab, TSymtabStack      *
	//  *            TLineNumNode, TLineNumList                     *
	//  *                                                           *
	//  *   FILE:    prog8-1/symtab.h                               *
	//  *                                                           *
	//  *   MODULE:  Symbol table                                   *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************



//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern int currentNestingLevel;
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern int currentLineNumber;
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern int asmLabelIndex;
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern int xrefFlag;

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
	//class TSymtab;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
	//class TSymtabNode;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
	//class TIcode;




	public static int asmLabelIndex = 0; // assembly label index
	public static int xrefFlag = false; // true = cross-referencing on, false = off


}
}
}
	//fig 3-17
	//  *************************************************************
	//  *                                                           *
	//  *   T O K E N S   (Words)                                   *
	//  *                                                           *
	//  *   Extract word tokens from the source file.               *
	//  *                                                           *
	//  *   CLASSES: TWordToken                                     *
	//  *                                                           *
	//  *   FILE:    prog3-2/tknword.cpp                            *
	//  *                                                           *
	//  *   MODULE:  Scanner                                        *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************



	//              *************************
	//              *                       *
	//              *  Reserved Word Table  *
	//              *                       *
	//              *************************

	public static readonly int minResWordLen = 2; // min and max reserved
	public static readonly int maxResWordLen = 9; //   word lengths

	internal static TResWord[] rw2 =
	{
		{ "do", TTokenCode.TcDO },
		{ "if", TTokenCode.TcIF },
		{ "in", TTokenCode.TcIN },
		{ "of", TTokenCode.TcOF },
		{ "or", TTokenCode.TcOR },
		{ "to", TTokenCode.TcTO },
		{ null }
	};

	internal static TResWord[] rw3 =
	{
		{ "and", TTokenCode.TcAND },
		{ "div", TTokenCode.TcDIV },
		{ "end", TTokenCode.TcEND },
		{ "for", TTokenCode.TcFOR },
		{ "mod", TTokenCode.TcMOD },
		{ "nil", TTokenCode.TcNIL },
		{ "not", TTokenCode.TcNOT },
		{ "set", TTokenCode.TcSET },
		{ "var", TTokenCode.TcVAR },
		{ null }
	};

	internal static TResWord[] rw4 =
	{
		{ "case", TTokenCode.TcCASE },
		{ "else", TTokenCode.TcELSE },
		{ "file", TTokenCode.TcFILE },
		{ "goto", TTokenCode.TcGOTO },
		{ "then", TTokenCode.TcTHEN },
		{ "type", TTokenCode.TcTYPE },
		{ "with", TTokenCode.TcWITH },
		{ null }
	};

	internal static TResWord[] rw5 =
	{
		{ "array", TTokenCode.TcARRAY },
		{ "begin", TTokenCode.TcBEGIN },
		{ "const", TTokenCode.TcCONST },
		{ "label", TTokenCode.TcLABEL },
		{ "until", TTokenCode.TcUNTIL },
		{ "while", TTokenCode.TcWHILE },
		{ null }
	};

	internal static TResWord[] rw6 =
	{
		{ "downto", TTokenCode.TcDOWNTO },
		{ "packed", TTokenCode.TcPACKED },
		{ "record", TTokenCode.TcRECORD },
		{ "repeat", TTokenCode.TcREPEAT },
		{ null }
	};

	internal static TResWord[] rw7 =
	{
		{ "program", TTokenCode.TcPROGRAM },
		{ null }
	};

	internal static TResWord[] rw8 =
	{
		{ "function", TTokenCode.TcFUNCTION },
		{ null }
	};

	internal static TResWord[] rw9 =
	{
		{ "procedure", TTokenCode.TcPROCEDURE },
		{ null }
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
}
	//fig 7-8
	//  *************************************************************
	//  *                                                           *
	//  *   T Y P E S                                               *
	//  *                                                           *
	//  *   CLASSES: TType                                          *
	//  *                                                           *
	//  *   FILE:    prog7-1/types.cpp                              *
	//  *                                                           *
	//  *   MODULE:  Symbol table                                   *
	//  *                                                           *
	//  *   Routines to manage type objects.                        *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************

	//fig 7-5
	//  *************************************************************
	//  *                                                           *
	//  *   T Y P E S   (Header)                                    *
	//  *                                                           *
	//  *	CLASSES: TType                               		*
	//  *                                                           *
	//  *   FILE:    prog7-1/types.h                                *
	//  *                                                           *
	//  *   MODULE:  Symbol table                                   *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************



//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern TType *pIntegerType, *pRealType, *pBooleanType, *pCharType, *pDummyType;

//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern char *formStrings[];

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




	internal static string[] formStrings = { "*** Error ***", "Scalar", "Enumeration", "Subrange", "Array", "Record" };

	//--Pointers to predefined types.
	public static TType pIntegerType = null;
	public static TType pRealType = null;
	public static TType pBooleanType = null;
	public static TType pCharType = null;
	public static TType pDummyType = null;
	//endfig


}
}
	//  *************************************************************
	//  *                                                           *
	//  *   M I S C E L L A N E O U S   (Header)			*
	//  *                                                           *
	//  *   FILE:    prog13-1/misc.h				*
	//  *                                                           *
	//  *   MODULE:  Common                                         *
	//  *                                                           *
	//  *   Copyright (c) 1996 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************


	//--------------------------------------------------------------
	//  Runtime stack frame offsets
	//--------------------------------------------------------------

	public static readonly int procLocalsStackFrameOffset = 0;
	public static readonly int funcLocalsStackFrameOffset = -4;
	public static readonly int parametersStackFrameOffset = +6;
}
	//  *************************************************************
	//  *                                                           *
	//  *   T O K E N S   (Header)                                  *
	//  *                                                           *
	//  *	CLASSES: TToken, TWordToken, TNumberToken, 		*
	//  *		 TStringToken, TSpecialToken, TEOFToken,	*
	//  *		 TErrorToken                                    *
	//  *                                                           *
	//  *   FILE:    prog4-2/token.h                                *
	//  *                                                           *
	//  *   MODULE:  Scanner                                        *
	//  *                                                           *
	//  *   Copyright (c) 1995 by Ronald Mak                        *
	//  *   For instructional purposes only.  No warranties.        *
	//  *                                                           *
	//  *************************************************************



//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//extern TCharCode charCodeMap[];
}