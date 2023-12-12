public class TParser
{
//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseExpression()
	{
		TType pResultType; // ptr to result type
		TType pOperandType; // ptr to operand type
    
		//--Parse the first simple expression.
		pResultType = ParseSimpleExpression();
    
		//--If we now see a relational operator,
		//--parse the second simple expression.
		if ( TokenIn( token, tlRelOps ) != 0 )
		{
		GetTokenAppend();
		pOperandType = ParseSimpleExpression();
    
		//--Check the operand types and return the boolean type.
		CheckRelOpOperands( pResultType, pOperandType );
		pResultType = pBooleanType;
		}
    
		//--Make sure the expression ended properly.
		Resync( tlExpressionFollow, tlStatementFollow, tlStatementStart );
    
		return pResultType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseSimpleExpression()
	{
		TType pResultType; // ptr to result type
		TType pOperandType; // ptr to operand type
		TTokenCode op; // operator
		int unaryOpFlag = false; // true if unary op, else false
    
		//--Unary + or -
		if ( TokenIn( token, tlUnaryOps ) != 0 )
		{
		unaryOpFlag = true;
		GetTokenAppend();
		}
    
		//--Parse the first term.
		pResultType = ParseTerm();
    
		//--If there was a unary sign, check the term's type.
		if ( unaryOpFlag != 0 )
			CheckIntegerOrReal( pResultType );
    
		//--Loop to parse subsequent additive operators and terms.
		while ( TokenIn( token, tlAddOps ) != 0 )
		{
    
		//--Remember the operator and parse the subsequent term.
		op = token;
		GetTokenAppend();
		pOperandType = ParseTerm();
    
		//--Check the operand types to determine the result type.
		switch ( op )
		{
    
			case TTokenCode.TcPlus:
			case TTokenCode.TcMinus:
    
			//--integer <op> integer => integer
			if ( IntegerOperands( pResultType, pOperandType ) )
				pResultType = pIntegerType;
    
			//--real    <op> real    => real
			//--real    <op> integer => real
			//--integer <op> real    => real
			else if ( RealOperands( pResultType, pOperandType ) )
				pResultType = pRealType;
    
			else
				Error( TErrorCode.ErrIncompatibleTypes );
			break;
    
			case TTokenCode.TcOR:
    
			//--boolean OR boolean => boolean
			CheckBoolean( pResultType, pOperandType );
			pResultType = pBooleanType;
			break;
		}
    
		}
    
		return pResultType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseTerm()
	{
		TType pResultType; // ptr to result type
		TType pOperandType; // ptr to operand type
		TTokenCode op; // operator
    
		//--Parse the first factor.
		pResultType = ParseFactor();
    
		//--Loop to parse subsequent multiplicative operators and factors.
		while ( TokenIn( token, tlMulOps ) != 0 )
		{
    
		//--Remember the operator and parse the subsequent factor.
		op = token;
		GetTokenAppend();
		pOperandType = ParseFactor();
    
		//--Check the operand types to determine the result type.
		switch ( op )
		{
    
			case TTokenCode.TcStar:
    
			//--integer * integer => integer
			if ( IntegerOperands( pResultType, pOperandType ) )
				pResultType = pIntegerType;
    
			//--real    * real    => real
			//--real    * integer => real
			//--integer * real    => real
			else if ( RealOperands( pResultType, pOperandType ) )
				pResultType = pRealType;
    
			else
				Error( TErrorCode.ErrIncompatibleTypes );
			break;
    
			case TTokenCode.TcSlash:
    
			//--integer / integer => real
			//--real    / real    => real
			//--real    / integer => real
			//--integer / real    => real
			if ( IntegerOperands( pResultType, pOperandType ) || RealOperands( pResultType, pOperandType ) )
				pResultType = pRealType;
			else
				Error( TErrorCode.ErrIncompatibleTypes );
			break;
    
			case TTokenCode.TcDIV:
			case TTokenCode.TcMOD:
    
			//--integer <op> integer => integer
			if ( IntegerOperands( pResultType, pOperandType ) )
				pResultType = pIntegerType;
			else
				Error( TErrorCode.ErrIncompatibleTypes );
			break;
    
			case TTokenCode.TcAND:
    
			//--boolean AND boolean => boolean
			CheckBoolean( pResultType, pOperandType );
			pResultType = pBooleanType;
			break;
		}
		}
    
		return pResultType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseFactor()
	{
		TType pResultType; // ptr to result type
    
		switch ( token )
		{
    
	//fig 8-19
		case TTokenCode.TcIdentifier:
		{
    
			//--Search for the identifier and enter it if
			//--necessary.  Append the symbol table node handle
			//--to the icode.
			TSymtabNode pNode = Find( pToken.String() );
			icode.Put( pNode );
    
			if ( pNode.defn.how == TDefnCode.DcUndefined )
			{
			pNode.defn.how = TDefnCode.DcVariable;
			SetType( pNode.pType, pDummyType );
			}
    
			//--Based on how the identifier is defined,
			//--parse a constant, function call, or variable.
			switch ( pNode.defn.how )
			{
    
			case TDefnCode.DcFunction:
				pResultType = ParseSubroutineCall( pNode, true );
				break;
    
			case TDefnCode.DcProcedure:
				Error( TErrorCode.ErrInvalidIdentifierUsage );
				pResultType = ParseSubroutineCall( pNode, false );
				break;
    
			case TDefnCode.DcConstant:
				GetTokenAppend();
				pResultType = pNode.pType;
				break;
    
			default:
				pResultType = ParseVariable( pNode );
				break;
			}
    
			break;
		}
	//endfig
    
		case TTokenCode.TcNumber:
		{
    
			//--Search for the number and enter it if necessary.
			TSymtabNode pNode = SearchAll( pToken.String() );
			if ( pNode == null )
			{
			pNode = EnterLocal( pToken.String() );
    
			//--Determine the number's type, and set its value into
			//--the symbol table node.
			if ( pToken.Type() == TDataType.TyInteger )
			{
				pResultType = pIntegerType;
				pNode.defn.constant.value.integer = pToken.Value().integer;
			} else
			{
				pResultType = pRealType;
				pNode.defn.constant.value.real = pToken.Value().real;
			}
			SetType( pNode.pType, pResultType );
			}
    
			//--Append the symbol table node handle to the icode.
			icode.Put( pNode );
    
			pResultType = pNode.pType;
			GetTokenAppend();
			break;
		}
    
		case TTokenCode.TcString:
		{
    
			//--Search for the string and enter it if necessary.
			char[] pString = pToken.String();
			TSymtabNode pNode = SearchAll( pString );
			if ( pNode == null )
			{
			pNode = EnterLocal( pString );
			pString = pNode.String();
    
			//--Compute the string length (without the quotes).
			//--If the length is 1, the result type is character,
			//--else create a new string type.
			int length = pString.Length - 2;
			pResultType = length == 1 ? pCharType : new TType( length );
			SetType( pNode.pType, pResultType );
    
			//--Set the character value or string pointer into the
			//--symbol table node.
			if ( length == 1 )
				pNode.defn.constant.value.character = pString[1];
			else
				pNode.defn.constant.value.pString = pString[1];
			}
    
			//--Append the symbol table node handle to the icode.
			icode.Put( pNode );
    
			pResultType = pNode.pType;
			GetTokenAppend();
			break;
		}
    
		case TTokenCode.TcNOT:
    
			//--The operand type must be boolean.
			GetTokenAppend();
			CheckBoolean( ParseFactor() );
			pResultType = pBooleanType;
    
			break;
    
		case TTokenCode.TcLParen:
    
			//--Parenthesized subexpression:  Call ParseExpression
			//--                              recursively ...
			GetTokenAppend();
			pResultType = ParseExpression();
    
			//-- ... and check for the closing right parenthesis.
			if ( token == TTokenCode.TcRParen )
				GetTokenAppend();
			else
				Error( TErrorCode.ErrMissingRightParen );
    
			break;
    
		default:
    
			Error( TErrorCode.ErrInvalidExpression );
			pResultType = pDummyType;
    
			break;
		}
    
		return pResultType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseVariable( TSymtabNode pId )
	{
		TType pResultType = pId.pType; // ptr to result type
    
		//--Check how the variable identifier was defined.
		switch ( pId.defn.how )
		{
		case TDefnCode.DcVariable:
		case TDefnCode.DcValueParm:
		case TDefnCode.DcVarParm:
		case TDefnCode.DcFunction:
		case TDefnCode.DcUndefined:
			break; // OK
    
	//C++ TO C# CONVERTER TODO TASK: C# does not allow fall-through from a non-empty 'case':
		default:
			pResultType = pDummyType;
			Error( TErrorCode.ErrInvalidIdentifierUsage );
			break;
		}
    
		GetTokenAppend();
    
		//-- [ or . : Loop to parse any subscripts and fields.
		int doneFlag = false;
		do
		{
		switch ( token )
		{
    
			case TTokenCode.TcLBracket:
			pResultType = ParseSubscripts( pResultType );
			break;
    
			case TTokenCode.TcPeriod:
			pResultType = ParseField( pResultType );
			break;
    
			default:
				doneFlag = true;
				break;
		}
		} while ( doneFlag == 0 );
    
		return pResultType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseSubscripts( TType pType )
	{
		//--Loop to parse a list of subscripts separated by commas.
		do
		{
		//-- [ (first) or , (subsequent)
		GetTokenAppend();
    
		//-- The current variable is an array type.
		if ( pType.form == TFormCode.FcArray )
		{
    
			//--The subscript expression must be assignment type
			//--compatible with the corresponding subscript type.
			CheckAssignmentTypeCompatible( pType.array.pIndexType, ParseExpression(), TErrorCode.ErrIncompatibleTypes );
    
			//--Update the variable's type.
			pType = pType.array.pElmtType;
		}
    
		//--No longer an array type, so too many subscripts.
		//--Parse the extra subscripts anyway for error recovery.
		else
		{
			Error( TErrorCode.ErrTooManySubscripts );
			ParseExpression();
		}
    
		} while ( token == TTokenCode.TcComma );
    
		//-- ]
		CondGetTokenAppend( TTokenCode.TcRBracket, TErrorCode.ErrMissingRightBracket );
    
		return ( TType ) pType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseField( TType pType )
	{
		GetTokenAppend();
    
		if ( ( token == TTokenCode.TcIdentifier ) && ( pType.form == TFormCode.FcRecord ) )
		{
		TSymtabNode pFieldId = pType.record.pSymtab.Search( pToken.String() );
		if ( pFieldId == null )
			Error( TErrorCode.ErrInvalidField );
		icode.Put( pFieldId );
    
		GetTokenAppend();
		return pFieldId != null ? pFieldId.pType : pDummyType;
		} else
		{
		Error( TErrorCode.ErrInvalidField );
		GetTokenAppend();
		return pDummyType;
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TSymtabNode * ParseProgram()
	{
		//--<program-header>
		TSymtabNode pProgramId = ParseProgramHeader();
    
		//-- ;
		Resync( tlHeaderFollow, tlDeclarationStart, tlStatementStart );
		if ( token == TTokenCode.TcSemicolon )
			GetToken();
		else if ( TokenIn( token, tlDeclarationStart ) != 0 || TokenIn( token, tlStatementStart ) != 0 )
		Error( TErrorCode.ErrMissingSemicolon );
    
		//--<block>
		ParseBlock( pProgramId );
		pProgramId.defn.routine.pSymtab = symtabStack.ExitScope();
    
		//-- .
		Resync( tlProgramEnd );
		CondGetTokenAppend( TTokenCode.TcPeriod, TErrorCode.ErrMissingPeriod );
    
		return pProgramId;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TSymtabNode * ParseProgramHeader()
	{
		TSymtabNode pProgramId; // ptr to program id node
    
		//--PROGRAM
		CondGetToken( TTokenCode.TcPROGRAM, TErrorCode.ErrMissingPROGRAM );
    
		//--<id>
		if ( token == TTokenCode.TcIdentifier )
		{
		pProgramId = EnterNewLocal( pToken.String(), TDefnCode.DcProgram );
		pProgramId.defn.routine.which = TRoutineCode.RcDeclared;
		pProgramId.defn.routine.parmCount = 0;
		pProgramId.defn.routine.totalParmSize = 0;
		pProgramId.defn.routine.totalLocalSize = 0;
		pProgramId.defn.routine.locals.pParmIds = null;
		pProgramId.defn.routine.locals.pConstantIds = null;
		pProgramId.defn.routine.locals.pTypeIds = null;
		pProgramId.defn.routine.locals.pVariableIds = null;
		pProgramId.defn.routine.locals.pRoutineIds = null;
		pProgramId.defn.routine.pSymtab = null;
		pProgramId.defn.routine.pIcode = null;
		SetType( pProgramId.pType, pDummyType );
		GetToken();
		} else
		{
			Error( TErrorCode.ErrMissingIdentifier );
		}
    
		//-- ( or ;
		Resync( tlProgProcIdFollow, tlDeclarationStart, tlStatementStart );
    
		//--Enter the nesting level 1 and open a new scope for the program.
		symtabStack.EnterScope();
    
		//--Optional ( <id-list> )
		if ( token == TTokenCode.TcLParen )
		{
		TSymtabNode pPrevParmId = null;
    
		//--Loop to parse a comma-separated identifier list.
		do
		{
			GetToken();
			if ( token == TTokenCode.TcIdentifier )
			{
			TSymtabNode pParmId = EnterNewLocal( pToken.String(), TDefnCode.DcVarParm );
			SetType( pParmId.pType, pDummyType );
			GetToken();
    
			//--Link program parm id nodes together.
			if ( pPrevParmId == null )
				pProgramId.defn.routine.locals.pParmIds = pPrevParmId = pParmId;
			else
			{
				pPrevParmId.next = pParmId;
				pPrevParmId = pParmId;
			}
			} else
			{
				Error( TErrorCode.ErrMissingIdentifier );
			}
		} while ( token == TTokenCode.TcComma );
    
		//-- )
		Resync( tlFormalParmsFollow, tlDeclarationStart, tlStatementStart );
		CondGetToken( TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen );
		}
    
		return pProgramId;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseSubroutineDeclarations( TSymtabNode pRoutineId )
	{
		TSymtabNode pLastId = null; // ptr to last routine id node
					  //   in local list
    
		//--Loop to parse procedure and function definitions.
		while ( TokenIn( token, tlProcFuncStart ) != 0 )
		{
		TSymtabNode pRtnId = ParseSubroutine();
    
		//--Link the routine's local (nested) routine id nodes together.
		if ( !pRoutineId.defn.routine.locals.pRoutineIds )
			pRoutineId.defn.routine.locals.pRoutineIds = pRtnId;
		else
			pLastId.next = pRtnId;
		pLastId = pRtnId;
    
		//-- ;
		Resync( tlDeclarationFollow, tlProcFuncStart, tlStatementStart );
		if ( token == TTokenCode.TcSemicolon )
			GetToken();
		else if ( TokenIn( token, tlProcFuncStart ) != 0 || TokenIn( token, tlStatementStart ) != 0 )
			Error( TErrorCode.ErrMissingSemicolon );
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TSymtabNode * ParseSubroutine()
	{
		//--<routine-header>
		TSymtabNode pRoutineId = ( token == TTokenCode.TcPROCEDURE ) ? ParseProcedureHeader() : ParseFunctionHeader();
    
		//-- ;
		Resync( tlHeaderFollow, tlDeclarationStart, tlStatementStart );
		if ( token == TTokenCode.TcSemicolon )
			GetToken();
		else if ( TokenIn( token, tlDeclarationStart ) != 0 || TokenIn( token, tlStatementStart ) != 0 )
		Error( TErrorCode.ErrMissingSemicolon );
    
		//--<block> or forward
		if ( string.Compare( pToken.String(), "forward", true ) != 0 )
		{
		pRoutineId.defn.routine.which = TRoutineCode.RcDeclared;
		ParseBlock( pRoutineId );
		} else
		{
		GetToken();
		pRoutineId.defn.routine.which = TRoutineCode.RcForward;
		}
    
		pRoutineId.defn.routine.pSymtab = symtabStack.ExitScope();
		return pRoutineId;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TSymtabNode * ParseProcedureHeader()
	{
		TSymtabNode pProcId; // ptr to procedure id node
		int forwardFlag = false; // true if forwarded, false if not
    
		GetToken();
    
		//--<id> : If the procedure id has already been declared in
		//--       this scope, it must have been a forward declaration.
		if ( token == TTokenCode.TcIdentifier )
		{
		pProcId = SearchLocal( pToken.String() );
		if ( pProcId == null )
		{
    
			//--Not already declared.
			pProcId = EnterLocal( pToken.String(), TDefnCode.DcProcedure );
			pProcId.defn.routine.totalLocalSize = 0;
			SetType( pProcId.pType, pDummyType );
		} else if ( ( pProcId.defn.how == TDefnCode.DcProcedure ) && ( pProcId.defn.routine.which == TRoutineCode.RcForward ) )
		{
    
			//--Forwarded.
			forwardFlag = true;
		} else
		{
			Error( TErrorCode.ErrRedefinedIdentifier );
		}
    
		GetToken();
		} else
		{
			Error( TErrorCode.ErrMissingIdentifier );
		}
    
		//-- ( or ;
		Resync( tlProgProcIdFollow, tlDeclarationStart, tlStatementStart );
    
		//--Enter the next nesting level and open a new scope
		//--for the procedure.
		symtabStack.EnterScope();
    
		//--Optional ( <id-list> ) : If there was a forward declaration,
		//--                         there must not be a parameter list,
		//--                         but if there is, parse it anyway
		//--                         for error recovery.
		if ( token == TTokenCode.TcLParen )
		{
		int parmCount; // count of formal parms
		int totalParmSize; // total byte size of all parms
		TSymtabNode pParmList = ParseFormalParmList( parmCount, totalParmSize );
    
		if ( forwardFlag != 0 )
			Error( TErrorCode.ErrAlreadyForwarded );
		else
		{
    
			//--Not forwarded.
			pProcId.defn.routine.parmCount = parmCount;
			pProcId.defn.routine.totalParmSize = totalParmSize;
			pProcId.defn.routine.locals.pParmIds = pParmList;
		}
		} else if ( forwardFlag == 0 )
		{
    
		//--No parameters and no forward declaration.
		pProcId.defn.routine.parmCount = 0;
		pProcId.defn.routine.totalParmSize = 0;
		pProcId.defn.routine.locals.pParmIds = null;
		}
    
		pProcId.defn.routine.locals.pConstantIds = null;
		pProcId.defn.routine.locals.pTypeIds = null;
		pProcId.defn.routine.locals.pVariableIds = null;
		pProcId.defn.routine.locals.pRoutineIds = null;
    
		SetType( pProcId.pType, pDummyType );
		return pProcId;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TSymtabNode * ParseFunctionHeader()
	{
		TSymtabNode pFuncId; // ptr to function id node
		int forwardFlag = false; // true if forwarded, false if not
    
		GetToken();
    
		//--<id> : If the function id has already been declared in
		//--       this scope, it must have been a forward declaration.
		if ( token == TTokenCode.TcIdentifier )
		{
		pFuncId = SearchLocal( pToken.String() );
		if ( pFuncId == null )
		{
    
			//--Not already declared.
			pFuncId = EnterLocal( pToken.String(), TDefnCode.DcFunction );
			pFuncId.defn.routine.totalLocalSize = 0;
		} else if ( ( pFuncId.defn.how == TDefnCode.DcFunction ) && ( pFuncId.defn.routine.which == TRoutineCode.RcForward ) )
		{
    
			//--Forwarded.
			forwardFlag = true;
		} else
		{
			Error( TErrorCode.ErrRedefinedIdentifier );
		}
    
		GetToken();
		} else
		{
			Error( TErrorCode.ErrMissingIdentifier );
		}
    
		//-- ( or : or ;
		Resync( tlFuncIdFollow, tlDeclarationStart, tlStatementStart );
    
		//--Enter the next nesting level and open a new scope
		//--for the function.
		symtabStack.EnterScope();
    
		//--Optional ( <id-list> ) : If there was a forward declaration,
		//--                         there must not be a parameter list,
		//--                         but if there is, parse it anyway
		//--                         for error recovery.
		if ( token == TTokenCode.TcLParen )
		{
		int parmCount; // count of formal parms
		int totalParmSize; // total byte size of all parms
		TSymtabNode pParmList = ParseFormalParmList( parmCount, totalParmSize );
    
		if ( forwardFlag != 0 )
			Error( TErrorCode.ErrAlreadyForwarded );
		else
		{
    
			//--Not forwarded.
			pFuncId.defn.routine.parmCount = parmCount;
			pFuncId.defn.routine.totalParmSize = totalParmSize;
			pFuncId.defn.routine.locals.pParmIds = pParmList;
		}
		} else if ( forwardFlag == 0 )
		{
    
		//--No parameters and no forward declaration.
		pFuncId.defn.routine.parmCount = 0;
		pFuncId.defn.routine.totalParmSize = 0;
		pFuncId.defn.routine.locals.pParmIds = null;
		}
    
		pFuncId.defn.routine.locals.pConstantIds = null;
		pFuncId.defn.routine.locals.pTypeIds = null;
		pFuncId.defn.routine.locals.pVariableIds = null;
		pFuncId.defn.routine.locals.pRoutineIds = null;
    
		//--Optional <type-id> : If there was a forward declaration,
		//--                     there must not be a type id, but if
		//--                     there is, parse it anyway for error
		//--                     recovery.
		if ( forwardFlag == 0 || ( token == TTokenCode.TcColon ) )
		{
		CondGetToken( TTokenCode.TcColon, TErrorCode.ErrMissingColon );
		if ( token == TTokenCode.TcIdentifier )
		{
			TSymtabNode pTypeId = Find( pToken.String() );
			if ( pTypeId.defn.how != TDefnCode.DcType )
				Error( TErrorCode.ErrInvalidType );
    
			if ( forwardFlag != 0 )
				Error( TErrorCode.ErrAlreadyForwarded );
			else
				SetType( pFuncId.pType, pTypeId.pType );
    
			GetToken();
		} else
		{
			Error( TErrorCode.ErrMissingIdentifier );
			SetType( pFuncId.pType, pDummyType );
		}
		}
    
		return pFuncId;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseBlock( TSymtabNode pRoutineId )
	{
		//--<declarations>
		ParseDeclarations( pRoutineId );
    
		//--<compound-statement> : Reset the icode and append BEGIN to it,
		//--                       and then parse the compound statement.
		Resync( tlStatementStart );
		if ( token != TTokenCode.TcBEGIN )
			Error( TErrorCode.ErrMissingBEGIN );
		icode.Reset();
		ParseCompound();
    
		//--Set the program's or routine's icode.
		pRoutineId.defn.routine.pIcode = new TIcode( icode );
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TSymtabNode * ParseFormalParmList( ref int count, ref int totalSize )
	{
	//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
	//	extern int execFlag;
    
		TSymtabNode pParmId; // ptrs to parm symtab nodes
		TSymtabNode pFirstId;
		TSymtabNode pLastId;
		TSymtabNode pPrevSublistLastId = null;
		TSymtabNode pParmList = null; // ptr to list of parm nodes
		TDefnCode parmDefn; // how a parm is defined
		int offset = parametersStackFrameOffset;
    
		count = totalSize = 0;
		GetToken();
    
		//--Loop to parse a parameter declarations separated by semicolons.
		while ( ( token == TTokenCode.TcIdentifier ) || ( token == TTokenCode.TcVAR ) )
		{
		TType pParmType; // ptr to parm's type object
    
		pFirstId = null;
    
		//--VAR or value parameter?
		if ( token == TTokenCode.TcVAR )
		{
			parmDefn = TDefnCode.DcVarParm;
			GetToken();
		} else
		{
			parmDefn = TDefnCode.DcValueParm;
		}
    
		//--Loop to parse the comma-separated sublist of parameter ids.
		while ( token == TTokenCode.TcIdentifier )
		{
			pParmId = EnterNewLocal( pToken.String(), parmDefn );
			++count;
			if ( pParmList == null )
				pParmList = pParmId;
    
			//--Link the parm id nodes together.
			if ( pFirstId == null )
				pFirstId = pLastId = pParmId;
			else
			{
			pLastId.next = pParmId;
			pLastId = pParmId;
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
    
		//-- :
		Resync( tlSublistFollow, tlDeclarationFollow );
		CondGetToken( TTokenCode.TcColon, TErrorCode.ErrMissingColon );
    
		//--<type-id>
		if ( token == TTokenCode.TcIdentifier )
		{
			TSymtabNode pTypeId = Find( pToken.String() );
			if ( pTypeId.defn.how != TDefnCode.DcType )
				Error( TErrorCode.ErrInvalidType );
			pParmType = pTypeId.pType;
			GetToken();
		} else
		{
			Error( TErrorCode.ErrMissingIdentifier );
			pParmType = pDummyType;
		}
    
		if ( execFlag )
		{
			//--Loop to assign the offset and type to each
			//--parm id in the sublist.
			for ( pParmId = pFirstId; pParmId != null; pParmId = pParmId.next )
			{
			pParmId.defn.data.offset = totalSize++;
			SetType( pParmId.pType, pParmType );
			}
		} else
		{
			//--Loop to assign the type to each parm id in the sublist.
			for ( pParmId = pFirstId; pParmId != null; pParmId = pParmId.next )
			SetType( pParmId.pType, pParmType );
		}
    
		//--Link this sublist to the previous sublist.
		if ( pPrevSublistLastId != null )
			pPrevSublistLastId.next = pFirstId;
		pPrevSublistLastId = pLastId;
    
		//-- ; or )
		Resync( tlFormalParmsFollow, tlDeclarationFollow );
		if ( ( token == TTokenCode.TcIdentifier ) || ( token == TTokenCode.TcVAR ) )
			Error( TErrorCode.ErrMissingSemicolon );
		else
		{
			while ( token == TTokenCode.TcSemicolon )
				GetToken();
		}
		}
    
		if ( !execFlag )
		{
    
		//--Assign the offset to each parm id in the entire
		//--formal parameter list in reverse order.
		ReverseNodeList( pParmList );
		for ( pParmId = pParmList; pParmId != null; pParmId = pParmId.next )
		{
			pParmId.defn.data.offset = offset;
			offset += pParmId.defn.how == ( ( int )TDefnCode.DcValueParm ) != 0 ? pParmId.pType.size : sizeof( object* ); // VAR pointer -  data value
			if ( ( offset & 1 ) != 0 )
				++offset; // round up to even
		}
		ReverseNodeList( pParmList );
    
		totalSize = offset - parametersStackFrameOffset;
		}
    
		//-- )
		CondGetToken( TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen );
    
		return pParmList;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseSubroutineCall( TSymtabNode pRoutineId, int parmCheckFlag )
	{
		GetTokenAppend();
    
		return ( pRoutineId.defn.routine.which == TRoutineCode.RcDeclared ) || ( pRoutineId.defn.routine.which == TRoutineCode.RcForward ) || parmCheckFlag == 0 ? ParseDeclaredSubroutineCall( pRoutineId, parmCheckFlag ) : ParseStandardSubroutineCall( pRoutineId );
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseDeclaredSubroutineCall( TSymtabNode pRoutineId, int parmCheckFlag )
	{
		ParseActualParmList( pRoutineId, parmCheckFlag );
		return pRoutineId.pType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseActualParmList( TSymtabNode pRoutineId, int parmCheckFlag )
	{
		TSymtabNode pFormalId = pRoutineId != null ? pRoutineId.defn.routine.locals.pParmIds : null;
    
		//--If there are no actual parameters, there better not be
		//--any formal parameters either.
		if ( token != TTokenCode.TcLParen )
		{
		if ( parmCheckFlag != 0 && pFormalId != null )
			Error( TErrorCode.ErrWrongNumberOfParms );
		return;
		}
    
		//--Loop to parse actual parameter expressions
		//--separated by commas.
		do
		{
		//-- ( or ,
		GetTokenAppend();
    
		ParseActualParm( pFormalId, parmCheckFlag );
		if ( pFormalId != null )
			pFormalId = pFormalId.next;
		} while ( token == TTokenCode.TcComma );
    
		//-- )
		CondGetTokenAppend( TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen );
    
		//--There better not be any more formal parameters.
		if ( parmCheckFlag != 0 && pFormalId != null )
			Error( TErrorCode.ErrWrongNumberOfParms );
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseActualParm( TSymtabNode pFormalId, int parmCheckFlag )
	{
		//--If we're not checking the actual parameters against
		//--the corresponding formal parameters (as during error
		//--recovery), just parse the actual parameter.
		if ( parmCheckFlag == 0 )
		{
		ParseExpression();
		return;
		}
    
		//--If we've already run out of formal parameter,
		//--we have an error.  Go into error recovery mode and
		//--parse the actual parameter anyway.
		if ( pFormalId == null )
		{
		Error( TErrorCode.ErrWrongNumberOfParms );
		ParseExpression();
		return;
		}
    
		//--Formal value parameter: The actual parameter can be an
		//--                        arbitrary expression that is
		//--                        assignment type compatible with
		//--                        the formal parameter.
		if ( pFormalId.defn.how == TDefnCode.DcValueParm )
		CheckAssignmentTypeCompatible( pFormalId.pType, ParseExpression(), TErrorCode.ErrIncompatibleTypes );
    
		//--Formal VAR parameter: The actual parameter must be a
		//--                      variable of the same type as the
		//--                      formal parameter.
		else if ( token == TTokenCode.TcIdentifier )
		{
		TSymtabNode pActualId = Find( pToken.String() );
		icode.Put( pActualId );
    
		if ( pFormalId.pType != ParseVariable( pActualId ) )
			Error( TErrorCode.ErrIncompatibleTypes );
		Resync( tlExpressionFollow, tlStatementFollow, tlStatementStart );
		}
    
		//--Error: Parse the actual parameter anyway for error recovery.
		else
		{
		ParseExpression();
		Error( TErrorCode.ErrInvalidVarParm );
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ReverseNodeList( ref TSymtabNode head )
	{
		TSymtabNode prev = null;
		TSymtabNode curr = head;
		TSymtabNode next;
    
		//--Reverse the list in place.
		while ( curr != null )
		{
			next = curr.next;
			curr.next = prev;
			prev = curr;
			curr = next;
		}
    
		//--Now point to the new head of the list,
		//--which was formerly its tail.
		head = prev;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseStandardSubroutineCall( TSymtabNode pRoutineId )
	{
		switch ( pRoutineId.defn.routine.which )
		{
    
		case TRoutineCode.RcRead:
		case TRoutineCode.RcReadln:
			return ( ParseReadReadlnCall( pRoutineId ) );
    
		case TRoutineCode.RcWrite:
		case TRoutineCode.RcWriteln:
			return ( ParseWriteWritelnCall( pRoutineId ) );
    
		case TRoutineCode.RcEof:
		case TRoutineCode.RcEoln:
			return ( ParseEofEolnCall() );
    
		case TRoutineCode.RcAbs:
		case TRoutineCode.RcSqr:
			return ( ParseAbsSqrCall() );
    
		case TRoutineCode.RcArctan:
		case TRoutineCode.RcCos:
		case TRoutineCode.RcExp:
		case TRoutineCode.RcLn:
		case TRoutineCode.RcSin:
		case TRoutineCode.RcSqrt:
			return ( ParseArctanCosExpLnSinSqrtCall() );
    
		case TRoutineCode.RcPred:
		case TRoutineCode.RcSucc:
			return ( ParsePredSuccCall() );
    
		case TRoutineCode.RcChr:
			return ( ParseChrCall() );
		case TRoutineCode.RcOdd:
			return ( ParseOddCall() );
		case TRoutineCode.RcOrd:
			return ( ParseOrdCall() );
    
		case TRoutineCode.RcRound:
		case TRoutineCode.RcTrunc:
			return ( ParseRoundTruncCall() );
    
		default:
			return ( pDummyType );
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseReadReadlnCall( TSymtabNode pRoutineId )
	{
		//--Actual parameters are optional for readln.
		if ( token != TTokenCode.TcLParen )
		{
		if ( pRoutineId.defn.routine.which == TRoutineCode.RcRead )
			Error( TErrorCode.ErrWrongNumberOfParms );
		return pDummyType;
		}
    
		//--Loop to parse comma-separated list of actual parameters.
		do
		{
		//-- ( or ,
		GetTokenAppend();
    
		//--Each actual parameter must be a scalar variable,
		//--but parse an expression anyway for error recovery.
		if ( token == TTokenCode.TcIdentifier )
		{
			TSymtabNode pParmId = Find( pToken.String() );
			icode.Put( pParmId );
    
			if ( ParseVariable( pParmId ).Base().form != TFormCode.FcScalar )
				Error( TErrorCode.ErrIncompatibleTypes );
		} else
		{
			ParseExpression();
			Error( TErrorCode.ErrInvalidVarParm );
		}
    
		//-- , or )
		Resync( tlActualVarParmFollow, tlStatementFollow, tlStatementStart );
		} while ( token == TTokenCode.TcComma );
    
		//-- )
		CondGetTokenAppend( TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen );
    
		return pDummyType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseWriteWritelnCall( TSymtabNode pRoutineId )
	{
		//--Actual parameters are optional only for writeln.
		if ( token != TTokenCode.TcLParen )
		{
		if ( pRoutineId.defn.routine.which == TRoutineCode.RcWrite )
			Error( TErrorCode.ErrWrongNumberOfParms );
		return pDummyType;
		}
    
		//--Loop to parse comma-separated list of actual parameters.
		do
		{
		//-- ( or ,
		GetTokenAppend();
    
		//--Value <expr> : The type must be either a non-Boolean
		//--               scalar or a string.
		TType pActualType = ParseExpression().Base();
		if ( ( ( pActualType.form != TFormCode.FcScalar ) || ( pActualType == pBooleanType ) ) && ( ( pActualType.form != TFormCode.FcArray ) || ( pActualType.array.pElmtType != pCharType ) ) )
			Error( TErrorCode.ErrIncompatibleTypes );
    
		//--Optional field width <expr>
		if ( token == TTokenCode.TcColon )
		{
			GetTokenAppend();
			if ( ParseExpression().Base() != pIntegerType )
			Error( TErrorCode.ErrIncompatibleTypes );
    
			//--Optional precision <expr>
			if ( token == TTokenCode.TcColon )
			{
			GetTokenAppend();
			if ( ParseExpression().Base() != pIntegerType )
				Error( TErrorCode.ErrIncompatibleTypes );
			}
		}
		} while ( token == TTokenCode.TcComma );
    
		//-- )
		CondGetTokenAppend( TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen );
    
		return pDummyType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseEofEolnCall()
	{
		//--There should be no actual parameters, but parse
		//--them anyway for error recovery.
		if ( token == TTokenCode.TcLParen )
		{
		Error( TErrorCode.ErrWrongNumberOfParms );
		ParseActualParmList( null, 0 );
		}
    
		return pBooleanType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseAbsSqrCall()
	{
		TType pResultType; // ptr to result type object
    
		//--There should be one integer or real parameter.
		if ( token == TTokenCode.TcLParen )
		{
		GetTokenAppend();
    
		TType pParmType = ParseExpression().Base();
		if ( ( pParmType != pIntegerType ) && ( pParmType != pRealType ) )
		{
			Error( TErrorCode.ErrIncompatibleTypes );
			pResultType = pIntegerType;
		} else
		{
			pResultType = pParmType;
		}
    
		//--There better not be any more parameters.
		if ( token != TTokenCode.TcRParen )
			SkipExtraParms();
    
		//-- )
		CondGetTokenAppend( TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen );
		} else
		{
			Error( TErrorCode.ErrWrongNumberOfParms );
		}
    
		return pResultType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseArctanCosExpLnSinSqrtCall()
	{
		//--There should be one integer or real parameter.
		if ( token == TTokenCode.TcLParen )
		{
		GetTokenAppend();
    
		TType pParmType = ParseExpression().Base();
		if ( ( pParmType != pIntegerType ) && ( pParmType != pRealType ) )
			Error( TErrorCode.ErrIncompatibleTypes );
    
		//--There better not be any more parameters.
		if ( token != TTokenCode.TcRParen )
			SkipExtraParms();
    
		//-- )
		CondGetTokenAppend( TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen );
		} else
		{
			Error( TErrorCode.ErrWrongNumberOfParms );
		}
    
		return pRealType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParsePredSuccCall()
	{
		TType pResultType; // ptr to result type object
    
		//--There should be one integer or enumeration parameter.
		if ( token == TTokenCode.TcLParen )
		{
		GetTokenAppend();
    
		TType pParmType = ParseExpression().Base();
		if ( ( pParmType != pIntegerType ) && ( pParmType.form != TFormCode.FcEnum ) )
		{
			Error( TErrorCode.ErrIncompatibleTypes );
			pResultType = pIntegerType;
		} else
		{
			pResultType = pParmType;
		}
    
		//--There better not be any more parameters.
		if ( token != TTokenCode.TcRParen )
			SkipExtraParms();
    
		//-- )
		CondGetTokenAppend( TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen );
		} else
		{
			Error( TErrorCode.ErrWrongNumberOfParms );
		}
    
		return pResultType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseChrCall()
	{
		//--There should be one character parameter.
		if ( token == TTokenCode.TcLParen )
		{
		GetTokenAppend();
    
		TType pParmType = ParseExpression().Base();
		if ( pParmType != pIntegerType )
			Error( TErrorCode.ErrIncompatibleTypes );
    
		//--There better not be any more parameters.
		if ( token != TTokenCode.TcRParen )
			SkipExtraParms();
    
		//-- )
		CondGetTokenAppend( TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen );
		} else
		{
			Error( TErrorCode.ErrWrongNumberOfParms );
		}
    
		return pCharType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseOddCall()
	{
		//--There should be one integer parameter.
		if ( token == TTokenCode.TcLParen )
		{
		GetTokenAppend();
    
		TType pParmType = ParseExpression().Base();
		if ( pParmType != pIntegerType )
			Error( TErrorCode.ErrIncompatibleTypes );
    
		//--There better not be any more parameters.
		if ( token != TTokenCode.TcRParen )
			SkipExtraParms();
    
		//-- )
		CondGetTokenAppend( TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen );
		} else
		{
			Error( TErrorCode.ErrWrongNumberOfParms );
		}
    
		return pBooleanType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseOrdCall()
	{
		//--There should be one character or enumeration parameter.
		if ( token == TTokenCode.TcLParen )
		{
		GetTokenAppend();
    
		TType pParmType = ParseExpression().Base();
		if ( ( pParmType != pCharType ) && ( pParmType.form != TFormCode.FcEnum ) )
			Error( TErrorCode.ErrIncompatibleTypes );
    
		//--There better not be any more parameters.
		if ( token != TTokenCode.TcRParen )
			SkipExtraParms();
    
		//-- )
		CondGetTokenAppend( TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen );
		} else
		{
			Error( TErrorCode.ErrWrongNumberOfParms );
		}
    
		return pIntegerType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseRoundTruncCall()
	{
		//--There should be one real parameter.
		if ( token == TTokenCode.TcLParen )
		{
		GetTokenAppend();
    
		TType pParmType = ParseExpression().Base();
		if ( pParmType != pRealType )
			Error( TErrorCode.ErrIncompatibleTypes );
    
		//--There better not be any more parameters.
		if ( token != TTokenCode.TcRParen )
			SkipExtraParms();
    
		//-- )
		CondGetTokenAppend( TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen );
		} else
		{
			Error( TErrorCode.ErrWrongNumberOfParms );
		}
    
		return pIntegerType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void SkipExtraParms()
	{
		Error( TErrorCode.ErrWrongNumberOfParms );
    
		while ( token == TTokenCode.TcComma )
		{
		GetTokenAppend();
		ParseExpression();
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseStatement()
	{
		InsertLineMarker();
    
		//--Call the appropriate parsing function based on
		//--the statement's first token.
		switch ( token )
		{
    
		case TTokenCode.TcIdentifier:
		{
    
			//--Search for the identifier and enter it if
			//--necessary.  Append the symbol table node handle
			//--to the icode.
			TSymtabNode pNode = Find( pToken.String() );
			icode.Put( pNode );
    
			//--Based on how the identifier is defined,
			//--parse an assignment statement or a procedure call.
			if ( pNode.defn.how == TDefnCode.DcUndefined )
			{
			pNode.defn.how = TDefnCode.DcVariable;
			SetType( pNode.pType, pDummyType );
			ParseAssignment( pNode );
			} else if ( pNode.defn.how == TDefnCode.DcProcedure )
			{
			ParseSubroutineCall( pNode, true );
			} else
			{
				ParseAssignment( pNode );
			}
    
			break;
		}
    
		case TTokenCode.TcREPEAT:
			ParseREPEAT();
			break;
		case TTokenCode.TcWHILE:
			ParseWHILE();
			break;
		case TTokenCode.TcIF:
			ParseIF();
			break;
		case TTokenCode.TcFOR:
			ParseFOR();
			break;
		case TTokenCode.TcCASE:
			ParseCASE();
			break;
		case TTokenCode.TcBEGIN:
			ParseCompound();
			break;
		}
    
		//--Resynchronize at a proper statement ending.
		if ( token != TTokenCode.TcEndOfFile )
		Resync( tlStatementFollow, tlStatementStart );
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseStatementList( TTokenCode terminator )
	{
		//--Loop to parse statements and to check for and skip semicolons.
		do
		{
		ParseStatement();
    
		if ( TokenIn( token, tlStatementStart ) != 0 )
			Error( TErrorCode.ErrMissingSemicolon );
		else
		{
			while ( token == TTokenCode.TcSemicolon )
				GetTokenAppend();
		}
		} while ( ( token != terminator ) && ( token != TTokenCode.TcEndOfFile ) );
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseAssignment( TSymtabNode pTargetId )
	{
		TType pTargetType = ParseVariable( pTargetId );
    
		//-- :=
		Resync( tlColonEqual, tlExpressionStart );
		CondGetTokenAppend( TTokenCode.TcColonEqual, TErrorCode.ErrMissingColonEqual );
    
		//--<expr>
		TType pExprType = ParseExpression();
    
		//--Check for assignment compatibility.
		CheckAssignmentTypeCompatible( pTargetType, pExprType, TErrorCode.ErrIncompatibleAssignment );
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseREPEAT()
	{
		GetTokenAppend();
    
		//--<stmt-list>
		ParseStatementList( TTokenCode.TcUNTIL );
    
		//--UNTIL
		CondGetTokenAppend( TTokenCode.TcUNTIL, TErrorCode.ErrMissingUNTIL );
    
		//--<expr> : must be boolean
		InsertLineMarker();
		CheckBoolean( ParseExpression() );
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseWHILE()
	{
		//--Append a placeholder location marker for the token that
		//--follows the WHILE statement.  Remember the location of this
		//--placeholder so it can be fixed up below.
		int atFollowLocationMarker = PutLocationMarker();
    
		//--<expr> : must be boolean
		GetTokenAppend();
		CheckBoolean( ParseExpression() );
    
		//--DO
		Resync( tlDO, tlStatementStart );
		CondGetTokenAppend( TTokenCode.TcDO, TErrorCode.ErrMissingDO );
    
		//--<stmt>
		ParseStatement();
		FixupLocationMarker( atFollowLocationMarker );
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseIF()
	{
		//--Append a placeholder location marker for where to go to if
		//--<expr> is false.  Remember the location of this placeholder
		//--so it can be fixed up below.
		int atFalseLocationMarker = PutLocationMarker();
    
		//--<expr> : must be boolean
		GetTokenAppend();
		CheckBoolean( ParseExpression() );
    
		//--THEN
		Resync( tlTHEN, tlStatementStart );
		CondGetTokenAppend( TTokenCode.TcTHEN, TErrorCode.ErrMissingTHEN );
    
		//--<stmt-1>
		ParseStatement();
		FixupLocationMarker( atFalseLocationMarker );
    
		if ( token == TTokenCode.TcELSE )
		{
    
		//--Append a placeholder location marker for the token that
		//--follows the IF statement.  Remember the location of this
		//--placeholder so it can be fixed up below.
		int atFollowLocationMarker = PutLocationMarker();
    
		//--ELSE <stmt-2>
		GetTokenAppend();
		ParseStatement();
		FixupLocationMarker( atFollowLocationMarker );
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseFOR()
	{
		TType pControlType; // ptr to the control id's type object
    
		//--Append a placeholder for the location of the token that
		//--follows the FOR statement.  Remember the location of this
		//--placeholder.
		int atFollowLocationMarker = PutLocationMarker();
    
		//--<id>
		GetTokenAppend();
		if ( token == TTokenCode.TcIdentifier )
		{
    
		//--Verify the definition and type of the control id.
		TSymtabNode pControlId = Find( pToken.String() );
		if ( pControlId.defn.how != TDefnCode.DcUndefined )
			pControlType = pControlId.pType.Base();
		else
		{
			pControlId.defn.how = TDefnCode.DcVariable;
			pControlType = pControlId.pType = pIntegerType;
		}
		if ( ( pControlType != pIntegerType ) && ( pControlType != pCharType ) && ( pControlType.form != TFormCode.FcEnum ) )
		{
			Error( TErrorCode.ErrIncompatibleTypes );
			pControlType = pIntegerType;
		}
    
		icode.Put( pControlId );
		GetTokenAppend();
		} else
		{
			Error( TErrorCode.ErrMissingIdentifier );
		}
    
		//-- :=
		Resync( tlColonEqual, tlExpressionStart );
		CondGetTokenAppend( TTokenCode.TcColonEqual, TErrorCode.ErrMissingColonEqual );
    
		//--<expr-1>
		CheckAssignmentTypeCompatible( pControlType, ParseExpression(), TErrorCode.ErrIncompatibleTypes );
    
		//--TO or DOWNTO
		Resync( tlTODOWNTO, tlExpressionStart );
		if ( TokenIn( token, tlTODOWNTO ) != 0 )
			GetTokenAppend();
		else
			Error( TErrorCode.ErrMissingTOorDOWNTO );
    
		//--<expr-2>
		CheckAssignmentTypeCompatible( pControlType, ParseExpression(), TErrorCode.ErrIncompatibleTypes );
    
		//--DO
		Resync( tlDO, tlStatementStart );
		CondGetTokenAppend( TTokenCode.TcDO, TErrorCode.ErrMissingDO );
    
		//--<stmt>
		ParseStatement();
		FixupLocationMarker( atFollowLocationMarker );
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseCASE()
	{
		TCaseItem pCaseItemList; // ptr to list of CASE items
		int caseBranchFlag; // true if another CASE branch,
					//   else false
    
		pCaseItemList = null;
    
		//--Append placeholders for the location of the token that
		//--follows the CASE statement and of the CASE branch table.
		//--Remember the locations of these placeholders.
		int atFollowLocationMarker = PutLocationMarker();
		int atBranchTableLocationMarker = PutLocationMarker();
    
		//--<expr>
		GetTokenAppend();
		TType pExprType = ParseExpression().Base();
    
		//--Verify the type of the CASE expression.
		if ( ( pExprType != pIntegerType ) && ( pExprType != pCharType ) && ( pExprType.form != TFormCode.FcEnum ) )
		Error( TErrorCode.ErrIncompatibleTypes );
    
		//--OF
		Resync( tlOF, tlCaseLabelStart );
		CondGetTokenAppend( TTokenCode.TcOF, TErrorCode.ErrMissingOF );
    
		//--Loop to parse CASE branches.
		caseBranchFlag = TokenIn( token, tlCaseLabelStart );
		while ( caseBranchFlag != 0 )
		{
		if ( TokenIn( token, tlCaseLabelStart ) != 0 )
			ParseCaseBranch( pExprType, pCaseItemList );
    
		if ( token == TTokenCode.TcSemicolon )
		{
			GetTokenAppend();
			caseBranchFlag = true;
		} else if ( TokenIn( token, tlCaseLabelStart ) )
		{
			Error( TErrorCode.ErrMissingSemicolon );
			caseBranchFlag = true;
		} else
		{
			caseBranchFlag = false;
		}
		}
    
		//--Append the branch table to the intermediate code.
		FixupLocationMarker( atBranchTableLocationMarker );
		TCaseItem pItem = pCaseItemList;
		TCaseItem pNext;
		do
		{
		PutCaseItem( pItem.labelValue, pItem.atBranchStmt );
		pNext = pItem.next;
		pItem = null;
		pItem = pNext;
		} while ( pItem != null );
		PutCaseItem( 0, 0 ); // end of table
    
		//--END
		Resync( tlEND, tlStatementStart );
		CondGetTokenAppend( TTokenCode.TcEND, TErrorCode.ErrMissingEND );
		FixupLocationMarker( atFollowLocationMarker );
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseCaseBranch( TType pExprType, ref TCaseItem pCaseItemList )
	{
		int caseLabelFlag; // true if another CASE label, else false
    
		//--<case-label-list>
		do
		{
		ParseCaseLabel( pExprType, pCaseItemList );
		if ( token == TTokenCode.TcComma )
		{
    
			//--Saw comma, look for another CASE label.
			GetTokenAppend();
			if ( TokenIn( token, tlCaseLabelStart ) != 0 )
				caseLabelFlag = true;
			else
			{
			Error( TErrorCode.ErrMissingConstant );
			caseLabelFlag = false;
			}
		} else
		{
			caseLabelFlag = false;
		}
    
		} while ( caseLabelFlag != 0 );
    
		//-- :
		Resync( tlColon, tlStatementStart );
		CondGetTokenAppend( TTokenCode.TcColon, TErrorCode.ErrMissingColon );
    
		//--Loop to set the branch statement location into each CASE item
		//--for this branch.
		for ( TCaseItem * pItem = pCaseItemList; pItem && ( pItem.atBranchStmt == 0 ); pItem = pItem.next )
		pItem.atBranchStmt = icode.CurrentLocation() - 1;
    
		//--<stmt>
		ParseStatement();
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseCaseLabel( TType pExprType, ref TCaseItem pCaseItemList )
	{
		TType pLabelType; // ptr to the CASE label's type object
		int signFlag = false; // true if unary sign, else false
    
		//--Allocate a new CASE item and insert it at the head of the list.
		TCaseItem pCaseItem = new TCaseItem( ref pCaseItemList );
    
		//--Unary + or -
		if ( TokenIn( token, tlUnaryOps ) != 0 )
		{
		signFlag = true;
		GetTokenAppend();
		}
    
		switch ( token )
		{
    
		//--Identifier:  Must be a constant whose type matches that
		//--             of the CASE expression.
		case TTokenCode.TcIdentifier:
		{
    
			TSymtabNode pLabelId = Find( pToken.String() );
			icode.Put( pLabelId );
    
			if ( pLabelId.defn.how != TDefnCode.DcUndefined )
			pLabelType = pLabelId.pType.Base();
			else
			{
			pLabelId.defn.how = TDefnCode.DcConstant;
			SetType( pLabelId.pType, pDummyType );
			pLabelType = pDummyType;
			}
			if ( pExprType != pLabelType )
				Error( TErrorCode.ErrIncompatibleTypes );
    
			//--Only an integer constant can have a unary sign.
			if ( signFlag != 0 && ( pLabelType != pIntegerType ) )
			Error( TErrorCode.ErrInvalidConstant );
    
			//--Set the label value into the CASE item.
			if ( ( pLabelType == pIntegerType ) || ( pLabelType.form == TFormCode.FcEnum ) )
			pCaseItem.labelValue = signFlag != 0 ? -pLabelId.defn.constant.value.integer : pLabelId.defn.constant.value.integer;
			else
			pCaseItem.labelValue = pLabelId.defn.constant.value.character;
    
			GetTokenAppend();
			break;
		}
    
		//--Number:  Both the label and the CASE expression
		//--         must be integer.
		case TTokenCode.TcNumber:
		{
    
			if ( pToken.Type() != TDataType.TyInteger )
				Error( TErrorCode.ErrInvalidConstant );
			if ( pExprType != pIntegerType )
				Error( TErrorCode.ErrIncompatibleTypes );
    
			TSymtabNode pNode = SearchAll( pToken.String() );
			if ( pNode == null )
			{
			pNode = EnterLocal( pToken.String() );
			pNode.pType = pIntegerType;
			pNode.defn.constant.value.integer = pToken.Value().integer;
			}
			icode.Put( pNode );
    
			//--Set the label value into the CASE item.
			pCaseItem.labelValue = signFlag != 0 ? -pNode.defn.constant.value.integer : pNode.defn.constant.value.integer;
    
			GetTokenAppend();
			break;
		}
    
		//--String:  Must be a single character without a unary sign.
		//--         (Note that the string length includes the quotes.)
		//--         The CASE expression type must be character.
		case TTokenCode.TcString:
		{
    
			if ( signFlag != 0 || ( Convert.ToString( pToken.String() ).Length != 3 ) )
			Error( TErrorCode.ErrInvalidConstant );
			if ( pExprType != pCharType )
				Error( TErrorCode.ErrIncompatibleTypes );
    
			TSymtabNode pNode = SearchAll( pToken.String() );
			if ( pNode == null )
			{
			pNode = EnterLocal( pToken.String() );
			pNode.pType = pCharType;
			pNode.defn.constant.value.character = pToken.String()[1];
			}
			icode.Put( pNode );
    
			//--Set the label value into the CASE item.
			pCaseItem.labelValue = pToken.String()[1];
    
			GetTokenAppend();
			break;
		}
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseCompound()
	{
		GetTokenAppend();
    
		//--<stmt-list>
		ParseStatementList( TTokenCode.TcEND );
    
		//--END
		CondGetTokenAppend( TTokenCode.TcEND, TErrorCode.ErrMissingEND );
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseTypeDefinitions( TSymtabNode pRoutineId )
	{
		TSymtabNode pLastId = null; // ptr to last type id node
					  //   in local list
    
		//--Loop to parse a list of type definitions
		//--seperated by semicolons.
		while ( token == TTokenCode.TcIdentifier )
		{
    
		//--<id>
		TSymtabNode pTypeId = EnterNewLocal( pToken.String() );
    
		//--Link the routine's local type id nodes together.
		if ( !pRoutineId.defn.routine.locals.pTypeIds )
			pRoutineId.defn.routine.locals.pTypeIds = pTypeId;
		else
			pLastId.next = pTypeId;
		pLastId = pTypeId;
    
		//-- =
		GetToken();
		CondGetToken( TTokenCode.TcEqual, TErrorCode.ErrMissingEqual );
    
		//--<type>
		SetType( pTypeId.pType, ParseTypeSpec() );
		pTypeId.defn.how = TDefnCode.DcType;
    
		//--If the type object doesn't have a name yet,
		//--point it to the type id.
		if ( pTypeId.pType.pTypeId == null )
			pTypeId.pType.pTypeId = pTypeId;
    
		//-- ;
		Resync( tlDeclarationFollow, tlDeclarationStart, tlStatementStart );
		CondGetToken( TTokenCode.TcSemicolon, TErrorCode.ErrMissingSemicolon );
    
		//--Skip extra semicolons.
		while ( token == TTokenCode.TcSemicolon )
			GetToken();
		Resync( tlDeclarationFollow, tlDeclarationStart, tlStatementStart );
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseTypeSpec()
	{
		switch ( token )
		{
    
		//--Type identifier
		case TTokenCode.TcIdentifier:
		{
			TSymtabNode pId = Find( pToken.String() );
    
			switch ( pId.defn.how )
			{
			case TDefnCode.DcType:
				return ParseIdentifierType( pId );
			case TDefnCode.DcConstant:
				return ParseSubrangeType( pId );
    
			default:
				Error( TErrorCode.ErrNotATypeIdentifier );
				GetToken();
				return ( pDummyType );
			}
		}
    
	//C++ TO C# CONVERTER TODO TASK: C# does not allow fall-through from a non-empty 'case':
		case TTokenCode.TcLParen:
			return ParseEnumerationType();
		case TTokenCode.TcARRAY:
			return ParseArrayType();
		case TTokenCode.TcRECORD:
			return ParseRecordType();
    
		case TTokenCode.TcPlus:
		case TTokenCode.TcMinus:
		case TTokenCode.TcNumber:
		case TTokenCode.TcString:
			return ParseSubrangeType( null );
    
		default:
			Error( TErrorCode.ErrInvalidType );
			return ( pDummyType );
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseIdentifierType( TSymtabNode pId2 )
	{
		GetToken();
		return pId2.pType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseEnumerationType()
	{
		TType pType = new TType( TFormCode.FcEnum, sizeof( int ), null );
		TSymtabNode pLastId = null;
		int constValue = -1; // enumeration constant value
    
		GetToken();
		Resync( tlEnumConstStart );
    
		//--Loop to parse list of constant identifiers separated by commas.
		while ( token == TTokenCode.TcIdentifier )
		{
		TSymtabNode pConstId = EnterNewLocal( pToken.String() );
		++constValue;
    
		if ( pConstId.defn.how == TDefnCode.DcUndefined )
		{
			pConstId.defn.how = TDefnCode.DcConstant;
			pConstId.defn.constant.value.integer = constValue;
			SetType( pConstId.pType, pType );
    
			//--Link constant identifier symbol table nodes together.
			if ( pLastId == null )
			pType.enumeration.pConstIds = pLastId = pConstId;
			else
			{
			pLastId.next = pConstId;
			pLastId = pConstId;
			}
		}
    
		//-- ,
		GetToken();
		Resync( tlEnumConstFollow );
		if ( token == TTokenCode.TcComma )
		{
    
			//--Saw comma.  Skip extra commas and look for
			//--            an identifier.
			do
			{
			GetToken();
			Resync( tlEnumConstStart, tlEnumConstFollow );
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
    
		//-- )
		CondGetToken( TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen );
    
		pType.enumeration.max = constValue;
		return pType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseSubrangeType( TSymtabNode pMinId )
	{
		TType pType = new TType( TFormCode.FcSubrange, 0, null );
    
		//--<min-const>
		SetType( pType.subrange.pBaseType, ParseSubrangeLimit( pMinId, pType.subrange.min ) );
    
		//-- ..
		Resync( tlSubrangeLimitFollow, tlDeclarationStart );
		CondGetToken( TTokenCode.TcDotDot, TErrorCode.ErrMissingDotDot );
    
		//--<max-const>
		TType pMaxType = ParseSubrangeLimit( null, pType.subrange.max );
    
		//--Check limits.
		if ( pMaxType != pType.subrange.pBaseType )
		{
		Error( TErrorCode.ErrIncompatibleTypes );
		pType.subrange.min = pType.subrange.max = 0;
		} else if ( pType.subrange.min > pType.subrange.max )
		{
		Error( TErrorCode.ErrMinGtMax );
    
		int temp = pType.subrange.min;
		pType.subrange.min = pType.subrange.max;
		pType.subrange.max = temp;
		}
    
		pType.size = pType.subrange.pBaseType.size;
		return pType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseSubrangeLimit( TSymtabNode pLimitId, ref int limit )
	{
		TType pType = pDummyType; // type to return
		TTokenCode sign = TTokenCode.TcDummy; // unary + or - sign, or none
    
		limit = 0;
    
		//--Unary + or -
		if ( TokenIn( token, tlUnaryOps ) != 0 )
		{
		if ( token == TTokenCode.TcMinus )
			sign = TTokenCode.TcMinus;
		GetToken();
		}
    
		switch ( token )
		{
    
		case TTokenCode.TcNumber:
    
			//--Numeric constant:  Integer type only.
			if ( pToken.Type() == TDataType.TyInteger )
			{
			limit = ( int )sign == ( ( int )TTokenCode.TcMinus ) != 0 ? -pToken.Value().integer : pToken.Value().integer;
			pType = pIntegerType;
			} else
			{
				Error( TErrorCode.ErrInvalidSubrangeType );
			}
			break;
    
		case TTokenCode.TcIdentifier:
    
			//--Identifier limit:  Must be integer, character, or
			//--                   enumeration type.
			if ( pLimitId == null )
				pLimitId = Find( pToken.String() );
    
			if ( pLimitId.defn.how == TDefnCode.DcUndefined )
			{
			pLimitId.defn.how = TDefnCode.DcConstant;
			pType = SetType( pLimitId.pType, pDummyType );
			break;
			}
    
			else if ( ( pLimitId.pType == pRealType ) || ( pLimitId.pType == pDummyType ) || ( pLimitId.pType.form == TFormCode.FcArray ) )
			Error( TErrorCode.ErrInvalidSubrangeType );
			else if ( pLimitId.defn.how == TDefnCode.DcConstant )
			{
    
			//--Use the value of the constant identifier.
			if ( pLimitId.pType == pIntegerType )
				limit = ( int )sign == ( ( int )TTokenCode.TcMinus ) != 0 ? -pLimitId.defn.constant.value.integer : pLimitId.defn.constant.value.integer;
			else if ( pLimitId.pType == pCharType )
			{
				if ( sign != TTokenCode.TcDummy )
					Error( TErrorCode.ErrInvalidConstant );
				limit = pLimitId.defn.constant.value.character;
			} else if ( pLimitId.pType.form == TFormCode.FcEnum )
			{
				if ( sign != TTokenCode.TcDummy )
					Error( TErrorCode.ErrInvalidConstant );
				limit = pLimitId.defn.constant.value.integer;
			}
			pType = pLimitId.pType;
			}
    
			else
				Error( TErrorCode.ErrNotAConstantIdentifier );
			break;
    
		case TTokenCode.TcString:
    
			//--String limit:  Character type only.
			if ( sign != TTokenCode.TcDummy )
				Error( TErrorCode.ErrInvalidConstant );
    
			if ( Convert.ToString( pToken.String() ).Length != 3 )
			Error( TErrorCode.ErrInvalidSubrangeType );
    
			limit = pToken.String()[1];
			pType = pCharType;
			break;
    
		default:
			Error( TErrorCode.ErrMissingConstant );
			return pType; // don't get another token
		}
    
		GetToken();
		return pType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseArrayType()
	{
		TType pArrayType = new TType( TFormCode.FcArray, 0, null );
		TType pElmtType = pArrayType;
		int indexFlag; // true if another array index, false if done
    
		//-- [
		GetToken();
		CondGetToken( TTokenCode.TcLBracket, TErrorCode.ErrMissingLeftBracket );
    
		//--Loop to parse each type spec in the index type list,
		//--seperated by commas.
		do
		{
		ParseIndexType( pElmtType );
    
		//-- ,
		Resync( tlIndexFollow, tlIndexStart );
		if ( ( token == TTokenCode.TcComma ) || TokenIn( token, tlIndexStart ) != 0 )
		{
    
			//--For each type spec after the first, create an
			//--element type object.
			pElmtType = SetType( pElmtType.array.pElmtType, new TType( TFormCode.FcArray, 0, null ) );
			CondGetToken( TTokenCode.TcComma, TErrorCode.ErrMissingComma );
			indexFlag = true;
		} else
		{
			indexFlag = false;
		}
    
		} while ( indexFlag != 0 );
    
		//-- ]
		CondGetToken( TTokenCode.TcRBracket, TErrorCode.ErrMissingRightBracket );
    
		//--OF
		Resync( tlIndexListFollow, tlDeclarationStart, tlStatementStart );
		CondGetToken( TTokenCode.TcOF, TErrorCode.ErrMissingOF );
    
		//--Final element type.
		SetType( pElmtType.array.pElmtType, ParseTypeSpec() );
    
		//--Total byte size of the array.
		if ( pArrayType.form != TFormCode.FcNone )
		pArrayType.size = ArraySize( pArrayType );
    
		return pArrayType;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ParseIndexType( TType pArrayType )
	{
		if ( TokenIn( token, tlIndexStart ) != 0 )
		{
		TType pIndexType = ParseTypeSpec();
		SetType( pArrayType.array.pIndexType, pIndexType );
    
		switch ( pIndexType.form )
		{
    
			//--Subrange index type
			case TFormCode.FcSubrange:
			pArrayType.array.elmtCount = pIndexType.subrange.max - pIndexType.subrange.min + 1;
			pArrayType.array.minIndex = pIndexType.subrange.min;
			pArrayType.array.maxIndex = pIndexType.subrange.max;
			return;
    
			//--Enumeration index type
			case TFormCode.FcEnum:
			pArrayType.array.elmtCount = pIndexType.enumeration.max + 1;
			pArrayType.array.minIndex = 0;
			pArrayType.array.maxIndex = pIndexType.enumeration.max;
			return;
    
			//--Error
			default:
				goto BadIndexType;
		}
		}
    
	BadIndexType:
    
		//--Error
		SetType( pArrayType.array.pIndexType, pDummyType );
		pArrayType.array.elmtCount = 0;
		pArrayType.array.minIndex = pArrayType.array.maxIndex = 0;
		Error( TErrorCode.ErrInvalidIndexType );
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public int ArraySize( TType pArrayType )
	{
		//--Calculate the size of the element type
		//--if it hasn't already been calculated.
		if ( pArrayType.array.pElmtType.size == 0 )
		pArrayType.array.pElmtType.size = ArraySize( pArrayType.array.pElmtType );
    
		return ( pArrayType.array.elmtCount * pArrayType.array.pElmtType.size );
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public TType * ParseRecordType()
	{
		TType pType = new TType( TFormCode.FcRecord, 0, null );
		pType.record.pSymtab = new TSymtab();
    
		//--Parse field declarations.
		GetToken();
		ParseFieldDeclarations( pType, 0 );
    
		//--END
		CondGetToken( TTokenCode.TcEND, TErrorCode.ErrMissingEND );
    
		return pType;
	}
}