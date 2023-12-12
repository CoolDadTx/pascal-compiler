using System;

//  *************************************************************
//  *                                                           *
//  *   P A R S E R                                             *
//  *                                                           *
//  *   Parse a Pascal source program.                          *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog11-1/parser.cpp                            *
//  *                                                           *
//  *   MODULE:  Parser                                         *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************

//  *************************************************************
//  *                                                           *
//  *   P A R S E R   (Header)                                  *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog13-1/parser.h                              *
//  *                                                           *
//  *   MODULE:  Parser                                         *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************



//--------------------------------------------------------------
//  TCaseItem           CASE item class.
//--------------------------------------------------------------

public class TCaseItem
{

	public TCaseItem next;
	public int labelValue;
	public int atBranchStmt;

	public TCaseItem( ref TCaseItem pListHead )
	{
	next = pListHead; // insert at head of list
	pListHead = this;
	atBranchStmt = 0;
	}
}

//--------------------------------------------------------------
//  TParser             Parser class.
//--------------------------------------------------------------

public class TParser : System.IDisposable
{
	private readonly TTextScanner pScanner; // ptr to the scanner
	private TToken pToken; // ptr to the current token
	private TTokenCode token; // code of current token
	private TSymtabStack symtabStack = new TSymtabStack(); // symbol table stack
	private TIcode icode = new TIcode(); // icode buffer

	//--Routines
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TSymtabNode ParseProgram();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TSymtabNode ParseProgramHeader();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseSubroutineDeclarations(TSymtabNode pRoutineId);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TSymtabNode ParseSubroutine();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TSymtabNode ParseProcedureHeader();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TSymtabNode ParseFunctionHeader();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseBlock(TSymtabNode pRoutineId);

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TSymtabNode ParseFormalParmList(ref int count, ref int totalSize);

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseSubroutineCall(TSymtabNode pRoutineId, int parmCheckFlag);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseDeclaredSubroutineCall(TSymtabNode pRoutineId, int parmCheckFlag);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseStandardSubroutineCall(TSymtabNode pRoutineId);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseActualParmList(TSymtabNode pRoutineId, int parmCheckFlag);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseActualParm(TSymtabNode pFormalId, int parmCheckFlag);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ReverseNodeList(ref TSymtabNode head);

	//--Standard subroutines
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseReadReadlnCall(TSymtabNode pRoutineId);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseWriteWritelnCall(TSymtabNode pRoutineId);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseEofEolnCall();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseAbsSqrCall();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseArctanCosExpLnSinSqrtCall();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParsePredSuccCall();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseChrCall();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseOddCall();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseOrdCall();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseRoundTruncCall();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void SkipExtraParms();

	//--Declarations
	public void ParseDeclarations( TSymtabNode pRoutineId )
	{
		if ( token == TTokenCode.TcCONST )
		{
		GetToken();
		ParseConstantDefinitions( pRoutineId );
		}
    
		if ( token == TTokenCode.TcTYPE )
		{
		GetToken();
		ParseTypeDefinitions( pRoutineId );
		}
    
		if ( token == TTokenCode.TcVAR )
		{
		GetToken();
		ParseVariableDeclarations( pRoutineId );
		}
    
		if ( TokenIn( token, tlProcFuncStart ) != 0 )
		ParseSubroutineDeclarations( pRoutineId );
	}
	public void ParseConstantDefinitions( TSymtabNode pRoutineId )
	{
		TSymtabNode pLastId = null; // ptr to last constant id node
					  //   in local list
    
		//--Loop to parse a list of constant definitions
		//--seperated by semicolons.
		while ( token == TTokenCode.TcIdentifier )
		{
    
		//--<id>
		TSymtabNode pConstId = EnterNewLocal( pToken.String() );
    
		//--Link the routine's local constant id nodes together.
		if ( !pRoutineId.defn.routine.locals.pConstantIds )
			pRoutineId.defn.routine.locals.pConstantIds = pConstId;
		else
			pLastId.next = pConstId;
		pLastId = pConstId;
    
		//-- =
		GetToken();
		CondGetToken( TTokenCode.TcEqual, TErrorCode.ErrMissingEqual );
    
		//--<constant>
		ParseConstant( pConstId );
		pConstId.defn.how = TDefnCode.DcConstant;
    
		//-- ;
		Resync( tlDeclarationFollow, tlDeclarationStart, tlStatementStart );
		CondGetToken( TTokenCode.TcSemicolon, TErrorCode.ErrMissingSemicolon );
    
		//--Skip extra semicolons.
		while ( token == TTokenCode.TcSemicolon )
			GetToken();
		Resync( tlDeclarationFollow, tlDeclarationStart, tlStatementStart );
		}
	}
	public void ParseConstant( TSymtabNode pConstId )
	{
		TTokenCode sign = TTokenCode.TcDummy; // unary + or - sign, or none
    
		//--Unary + or -
		if ( TokenIn( token, tlUnaryOps ) != 0 )
		{
		if ( token == TTokenCode.TcMinus )
			sign = TTokenCode.TcMinus;
		GetToken();
		}
    
		switch ( token )
		{
    
		//--Numeric constant:  Integer or real type.
		case TTokenCode.TcNumber:
			if ( pToken.Type() == TDataType.TyInteger )
			{
			pConstId.defn.constant.value.integer = ( int )sign == ( ( int )TTokenCode.TcMinus ) != 0 ? -pToken.Value().integer : pToken.Value().integer;
			SetType( pConstId.pType, pIntegerType );
			} else
			{
			pConstId.defn.constant.value.real = ( int )sign == ( ( int )TTokenCode.TcMinus ) != 0 ? -pToken.Value().real : pToken.Value().real;
			SetType( pConstId.pType, pRealType );
			}
    
			GetToken();
			break;
    
		//--Identifier constant
		case TTokenCode.TcIdentifier:
			ParseIdentifierConstant( pConstId, sign );
			break;
    
		//--String constant:  Character or string
		//--                  (character array) type.
		case TTokenCode.TcString:
			int length = Convert.ToString( pToken.String() ).Length - 2; // skip quotes
    
			if ( sign != TTokenCode.TcDummy )
				Error( TErrorCode.ErrInvalidConstant );
    
			//--Single character
			if ( length == 1 )
			{
			pConstId.defn.constant.value.character = pToken.String()[1];
			SetType( pConstId.pType, pCharType );
			}
    
			//--String (character array):  Create a new unnamed
			//--                           string type.
			else
			{
			string pString = new string( new char[length - 1] );
			CopyQuotedString( pString, pToken.String() );
			pConstId.defn.constant.value.pString = pString;
			SetType( pConstId.pType, new TType( length ) );
			}
    
			GetToken();
			break;
		}
	}
	public void ParseIdentifierConstant( TSymtabNode pId1, TTokenCode sign )
	{
		TSymtabNode pId2 = Find( pToken.String() ); // ptr to <id-2>
    
		if ( pId2.defn.how != TDefnCode.DcConstant )
		{
		Error( TErrorCode.ErrNotAConstantIdentifier );
		SetType( pId1.pType, pDummyType );
		GetToken();
		return;
		}
    
		//--Integer identifier
		if ( pId2.pType == pIntegerType )
		{
		pId1.defn.constant.value.integer = ( int )sign == ( ( int )TTokenCode.TcMinus ) != 0 ? -pId2.defn.constant.value.integer : pId2.defn.constant.value.integer;
		SetType( pId1.pType, pIntegerType );
		}
    
		//--Real identifier
		else if ( pId2.pType == pRealType )
		{
		pId1.defn.constant.value.real = ( int )sign == ( ( int )TTokenCode.TcMinus ) != 0 ? -pId2.defn.constant.value.real : pId2.defn.constant.value.real;
		SetType( pId1.pType, pRealType );
		}
    
		//--Character identifier:  No unary sign allowed.
		else if ( pId2.pType == pCharType )
		{
		if ( sign != TTokenCode.TcDummy )
			Error( TErrorCode.ErrInvalidConstant );
    
		pId1.defn.constant.value.character = pId2.defn.constant.value.character;
		SetType( pId1.pType, pCharType );
		}
    
		//--Enumeration identifier:  No unary sign allowed.
		else if ( pId2.pType.form == TFormCode.FcEnum )
		{
		if ( sign != TTokenCode.TcDummy )
			Error( TErrorCode.ErrInvalidConstant );
    
		pId1.defn.constant.value.integer = pId2.defn.constant.value.integer;
		SetType( pId1.pType, pId2.pType );
		}
    
		//--Array identifier:  Must be character array, and
		//                     no unary sign allowed.
		else if ( pId2.pType.form == TFormCode.FcArray )
		{
		if ( ( sign != TTokenCode.TcDummy ) || ( pId2.pType.array.pElmtType != pCharType ) )
			Error( TErrorCode.ErrInvalidConstant );
    
		pId1.defn.constant.value.pString = pId2.defn.constant.value.pString;
		SetType( pId1.pType, pId2.pType );
		}
    
		GetToken();
	}

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseTypeDefinitions(TSymtabNode pRoutineId);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseTypeSpec();

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseIdentifierType(TSymtabNode pId2);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseEnumerationType();

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseSubrangeType(TSymtabNode pMinId);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseSubrangeLimit(TSymtabNode pLimitId, ref int limit);

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseArrayType();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseIndexType(TType pArrayType);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	int ArraySize(TType pArrayType);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseRecordType();

	public void ParseVariableDeclarations( TSymtabNode pRoutineId )
	{
		if ( execFlag != 0 )
		ParseVarOrFieldDecls( pRoutineId, null, pRoutineId.defn.routine.parmCount );
		else
		ParseVarOrFieldDecls( pRoutineId, null, pRoutineId.defn.how == ( ( int )TDefnCode.DcProcedure ) != 0 ? procLocalsStackFrameOffset : funcLocalsStackFrameOffset );
	}
	public void ParseFieldDeclarations( TType pRecordType, int offset )
	{
		ParseVarOrFieldDecls( null, pRecordType, offset );
	}
	public void ParseVarOrFieldDecls( TSymtabNode pRoutineId, TType pRecordType, int offset )
	{
		TSymtabNode pId; // ptrs to symtab nodes
		TSymtabNode pFirstId;
		TSymtabNode pLastId;
		TSymtabNode pPrevSublistLastId = null; // ptr to last node of
							 //   previous sublist
		int totalSize = 0; // total byte size of
							 //   local variables
    
		//--Loop to parse a list of variable or field declarations
		//--separated by semicolons.
		while ( token == TTokenCode.TcIdentifier )
		{
    
		//--<id-sublist>
		pFirstId = ParseIdSublist( pRoutineId, pRecordType, pLastId );
    
		//-- :
		Resync( tlSublistFollow, tlDeclarationFollow );
		CondGetToken( TTokenCode.TcColon, TErrorCode.ErrMissingColon );
    
		//--<type>
		TType pType = ParseTypeSpec();
    
		//--Now loop to assign the type and offset to each
		//--identifier in the sublist.
		for ( pId = pFirstId; pId != null; pId = pId.next )
		{
			SetType( pId.pType, pType );
    
			if ( pRoutineId != null )
			{
    
			//--Variables
			if ( execFlag != 0 )
				pId.defn.data.offset = offset++;
			else
			{
				offset -= pType.size;
				pId.defn.data.offset = offset;
			}
			totalSize += pType.size;
			} else
			{
    
			//--Record fields
			pId.defn.data.offset = offset;
			offset += pType.size;
			}
		}
    
		if ( pFirstId != null )
		{
    
			//--Set the first sublist into the routine id's symtab node.
			if ( pRoutineId != null && ( !pRoutineId.defn.routine.locals.pVariableIds ) )
			pRoutineId.defn.routine.locals.pVariableIds = pFirstId;
    
			//--Link this list to the previous sublist.
			if ( pPrevSublistLastId != null )
				pPrevSublistLastId.next = pFirstId;
			pPrevSublistLastId = pLastId;
		}
    
		//-- ;   for variable and record field declaration, or
		//-- END for record field declaration
		if ( pRoutineId != null )
		{
			Resync( tlDeclarationFollow, tlStatementStart );
			CondGetToken( TTokenCode.TcSemicolon, TErrorCode.ErrMissingSemicolon );
    
			//--Skip extra semicolons.
			while ( token == TTokenCode.TcSemicolon )
				GetToken();
			Resync( tlDeclarationFollow, tlDeclarationStart, tlStatementStart );
		} else
		{
			Resync( tlFieldDeclFollow );
			if ( token != TTokenCode.TcEND )
			{
			CondGetToken( TTokenCode.TcSemicolon, TErrorCode.ErrMissingSemicolon );
    
			//--Skip extra semicolons.
			while ( token == TTokenCode.TcSemicolon )
				GetToken();
			Resync( tlFieldDeclFollow, tlDeclarationStart, tlStatementStart );
			}
		}
		}
    
		//--Set the routine identifier node or the record type object.
		if ( pRoutineId != null )
		pRoutineId.defn.routine.totalLocalSize = totalSize;
		else
		pRecordType.size = offset;
	}
	public TSymtabNode * ParseIdSublist( TSymtabNode pRoutineId, TType pRecordType, ref TSymtabNode pLastId )
	{
		TSymtabNode pId;
		TSymtabNode pFirstId = null;
    
		pLastId = null;
    
		//--Loop to parse each identifier in the sublist.
		while ( token == TTokenCode.TcIdentifier )
		{
    
		//--Variable:  Enter into local  symbol table.
		//--Field:     Enter into record symbol table.
		pId = pRoutineId != null ? EnterNewLocal( pToken.String() ) : pRecordType.record.pSymtab.EnterNew(pToken.String());
    
		//--Link newly-declared identifier nodes together
		//--into a sublist.
		if ( pId.defn.how == TDefnCode.DcUndefined )
		{
			pId.defn.how = pRoutineId != null ? TDefnCode.DcVariable : TDefnCode.DcField;
			if ( pFirstId == null )
				pFirstId = pLastId = pId;
			else
			{
			pLastId.next = pId;
			pLastId = pId;
			}
		}
    
		//-- ,
		GetToken();
		Resync( tlIdentifierFollow );
		if ( token == TTokenCode.TcComma )
		{
    
			//--Saw comma.
			//--Skip extra commas and look for an identifier.
			do
			{
			GetToken();
			Resync( tlIdentifierStart, tlIdentifierFollow );
			if ( token == TTokenCode.TcComma )
				Error( TErrorCode.ErrMissingIdentifier );
			} while ( token == TTokenCode.TcComma );
			if ( token != TTokenCode.TcIdentifier )
				Error( TErrorCode.ErrMissingIdentifier );
		} else if ( token == TTokenCode.TcIdentifier )
		{
			Error( TErrorCode.ErrMissingComma );
		}
		}
    
		return pFirstId;
	}

	//--Statements
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseStatement();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseStatementList(TTokenCode terminator);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseAssignment(TSymtabNode pTargetId);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseREPEAT();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseWHILE();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseIF();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseFOR();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseCASE();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseCaseBranch(TType pExprType, ref TCaseItem pCaseItemList);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseCaseLabel(TType pExprType, ref TCaseItem pCaseItemList);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ParseCompound();

	//--Expressions
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseExpression();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseSimpleExpression();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseTerm();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseFactor();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseVariable(TSymtabNode pId);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseSubscripts(TType pType);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	TType ParseField(TType pType);

	private void GetToken()
	{
	pToken = pScanner.Get();
	token = pToken.Code();
	}

	private void GetTokenAppend()
	{
	GetToken();
	icode.Put( token ); // append token code to icode buffer
	}

	private void CondGetToken( TTokenCode tc, TErrorCode ec )
	{
	//--Get another token only if the current one matches tc.
	if ( tc == token )
		GetToken();
	else
		Error( ec ); // error if no match
	}

	private void CondGetTokenAppend( TTokenCode tc, TErrorCode ec )
	{
	//--Get another token only if the current one matches tc.
	if ( tc == token )
		GetTokenAppend();
	else
		Error( ec ); // error if no match
	}

	private void InsertLineMarker()
	{
		icode.InsertLineMarker();
	}

	private int PutLocationMarker()
	{
	return icode.PutLocationMarker();
	}

	private void FixupLocationMarker( int location )
	{
	icode.FixupLocationMarker( location );
	}

	private void PutCaseItem( int value, int location )
	{
	icode.PutCaseItem( value, location );
	}

	private TSymtabNode SearchLocal( string pString )
	{
	return symtabStack.SearchLocal( pString );
	}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: TSymtabNode *SearchAll(const char *pString) const
	private TSymtabNode SearchAll( string pString )
	{
	return symtabStack.SearchAll( pString );
	}

	private TSymtabNode EnterLocal( string pString, TDefnCode dc = dcUndefined )
	{
	return symtabStack.EnterLocal( pString, dc );
	}

	private TSymtabNode EnterNewLocal( string pString, TDefnCode dc = dcUndefined )
	{
	return symtabStack.EnterNewLocal( pString, dc );
	}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: TSymtabNode *Find(const char *pString) const
	private TSymtabNode Find( string pString )
	{
	return symtabStack.Find( pString );
	}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: void CopyQuotedString(char *pString, const char *pQuotedString) const
	private void CopyQuotedString( ref string pString, string pQuotedString )
	{
	int length = pQuotedString.Length - 2; // don't count quotes
	pString = Convert.ToString( pQuotedString[1] ).Substring( 0, length );
	pString[length] = '\0';
	}

	//endfig

	//--------------------------------------------------------------
	//  Resync          Resynchronize the parser.  If the current
	//                  token is not in one of the token lists,
	//                  flag it as an error and then skip tokens
	//                  up to one that is in a list or end of file.
	//--------------------------------------------------------------

	private void Resync( TTokenCode pList1, TTokenCode pList2 = null, TTokenCode pList3 = null )
	{
		//--Is the current token in one of the lists?
		int errorFlag = ( !TokenIn( token, pList1 ) ) && ( !TokenIn( token, pList2 ) ) && ( !TokenIn( token, pList3 ) );

		if ( errorFlag != 0 )
		{

		//--Nope.  Flag it as an error.
		TErrorCode errorCode = token == ( ( int )TTokenCode.TcEndOfFile ) != 0 ? TErrorCode.ErrUnexpectedEndOfFile : TErrorCode.ErrUnexpectedToken;
		Error( errorCode );

		//--Skip tokens.
		while ( ( !TokenIn( token, pList1 ) ) && ( !TokenIn( token, pList2 ) ) && ( !TokenIn( token, pList3 ) ) && ( token != TTokenCode.TcPeriod ) && ( token != TTokenCode.TcEndOfFile ) )
			GetToken();

		//--Flag an unexpected end of file (if haven't already).
		if ( ( token == TTokenCode.TcEndOfFile ) && ( errorCode != TErrorCode.ErrUnexpectedEndOfFile ) )
			Error( TErrorCode.ErrUnexpectedEndOfFile );
		}
	}

	public TParser( TTextInBuffer pBuffer )
	{
		this.pScanner = new TTextScanner( pBuffer );
	}

   public void Dispose()
   {
	   if ( pScanner != null )
		   pScanner.Dispose();
   }

	public TToken GetCommandToken()
	{
	GetToken();
	return pToken;
	}


	//fig 11-12
	//--------------------------------------------------------------
	//  ParseCommandExpression      Parse the expression part of the
	//                              debugger "show" command.
	//
	//      pCmdIcode : ref to ptr to command icode
	//      cmdToken  : ref to current command token
	//--------------------------------------------------------------

	public void ParseCommandExpression( ref TIcode pCmdIcode, ref TTokenCode cmdToken )
	{
		icode.Reset();
		GetTokenAppend(); // first token of expression

		//--Parse the expression.
		ParseExpression();
		pCmdIcode = new TIcode( icode ); // copy of expression icode
		cmdToken = token; // transfer token to debugger

		//--Convert the current symbol table again in case new nodes
		//--were inserted by the expression.
		TSymtab pSymtab = symtabStack.GetCurrentSymtab();
		if ( pSymtab.NodeVector() != null )
			pSymtab.NodeVector().Dispose();
		pSymtab.Convert( vpSymtabs );
	}

	//--------------------------------------------------------------
	//  ParseCommandAssignment      Parse the assignment statement
	//                              part of the debugger "assign"
	//                              command.
	//
	//      pCmdIcode : ref to ptr to command icode
	//      cmdToken  : ref to current command token
	//
	//  Return: ptr to the symbol table node of the target variable
	//--------------------------------------------------------------

	public TSymtabNode ParseCommandAssignment( ref TIcode pCmdIcode, ref TTokenCode cmdToken )
	{
		TSymtabNode pTargetId = null;

		icode.Reset();
		GetTokenAppend(); // first token of target variable

		if ( token == TTokenCode.TcIdentifier )
		{
		pTargetId = Find( pToken.String() );
		icode.Put( pTargetId );

		//--Parse the assignment.
		ParseAssignment( pTargetId );
		pCmdIcode = new TIcode( icode ); // copy of statement icode
		cmdToken = token; // transfer token to debugger

		//--Convert the current symbol table again in case new
		//--nodes were inserted by the assignment expression.
		TSymtab pSymtab = symtabStack.GetCurrentSymtab();
		if ( pSymtab.NodeVector() != null )
			pSymtab.NodeVector().Dispose();
		pSymtab.Convert( vpSymtabs );
		} else
		{
			Error( TErrorCode.ErrUnexpectedToken );
		}

		return pTargetId;
	}

	public void DebugSetCurrentSymtab( TSymtab pSymtab )
	{
	symtabStack.SetCurrentSymtab( pSymtab );
	}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: TSymtabNode *DebugSearchAll(const char *pString) const
	public TSymtabNode DebugSearchAll( string pString )
	{
	return SearchAll( pString );
	}


	//--------------------------------------------------------------
	//  Parse       Parse the source file.
	//
	//  Return: ptr to the program id's symbol table node
	//--------------------------------------------------------------

	public TSymtabNode Parse()
	{
		//--Extract the first token and parse the program.
		GetToken();
		TSymtabNode pProgramId = ParseProgram();

		//--Print the parser's summary.
		list.PutLine();
		list.text = string.Format( "{0,20:D} source lines.", currentLineNumber );
		list.PutLine();
		list.text = string.Format( "{0,20:D} syntax errors.", errorCount );
		list.PutLine();

		return pProgramId;
	}
}




