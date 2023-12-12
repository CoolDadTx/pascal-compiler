//fig 12-7
//  *************************************************************
//  *                                                           *
//  *   C O D E   G E N E R A T O R   (Header)                  *
//  *                                                           *
//  *   CLASSES: TAssemblyBuffer, TCodeGenerator                *
//  *                                                           *
//  *   FILE:    prog13-1/codegen.h                             *
//  *                                                           *
//  *   MODULE:  Code generator                                 *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************



//--------------------------------------------------------------
//  Assembly label prefixes
//--------------------------------------------------------------


//--------------------------------------------------------------
//  Runtime stack frame items
//--------------------------------------------------------------


//--------------------------------------------------------------
//  Names of library routines
//--------------------------------------------------------------





//--------------------------------------------------------------
//  Emit0               Emit a no-operand instruction.
//--------------------------------------------------------------

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Emit0(opcode) { Operator(opcode); pAsmBuffer->PutLine(); }

//--------------------------------------------------------------
//  Emit1               Emit a one-operand instruction.
//--------------------------------------------------------------

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Emit1(opcode, operand1) { Operator(opcode); pAsmBuffer->Put('\t'); operand1; pAsmBuffer->PutLine(); }

//--------------------------------------------------------------
//  Emit2               Emit a two-operand instruction.
//--------------------------------------------------------------

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Emit2(opcode, operand1, operand2) { Operator(opcode); pAsmBuffer->Put('\t'); operand1; pAsmBuffer->Put(','); operand2; pAsmBuffer->PutLine(); }

//--------------------------------------------------------------
//  TRegister           Machine registers.
//--------------------------------------------------------------

public enum TRegister
{
	Ax,
	Ah,
	Al,
	Bx,
	Bh,
	Bl,
	Cx,
	Ch,
	Cl,
	Dx,
	Dh,
	Dl,
	Cs,
	Ds,
	Es,
	Ss,
	Sp,
	Bp,
	Si,
	Di,
}

//--------------------------------------------------------------
//  TInstruction        Assembly instructions.
//--------------------------------------------------------------

public enum TInstruction
{
	Mov,
	RepMovsb,
	Lea,
	Xchg,
	Cmp,
	RepeCmpsb,
	Pop,
	Push,
	And,
	Or,
	Xor,
	Neg,
	Incr,
	Decr,
	Add,
	Sub,
	Imul,
	Idiv,
	Cld,
	Call,
	Ret,
	Jmp,
	Jl,
	Jle,
	Je,
	Jne,
	Jge,
	Jg,
}

//--------------------------------------------------------------
//  TAssemblyBuffer     Assembly language buffer subclass of
//                      TTextOutBuffer.
//--------------------------------------------------------------

public class TAssemblyBuffer : TTextOutBuffer
{
//C++ TO C# CONVERTER NOTE: Enums must be named in C#, so the following enum has been named AnonymousEnum3:
	private enum AnonymousEnum3
	{
	MaxLength = 72,
	}

	private std::fstream file = new std::fstream(); // assembly output file
	private string pText; // assembly buffer pointer
	private int textLength; // length of assembly comment

	public TAssemblyBuffer( string pAssemblyFileName, TAbortCode ac )
	{
		//--Open the assembly output file.  Abort if failed.
		file.open( pAssemblyFileName, ios.@out );
		if ( !file.good() )
			AbortTranslation( ac );
    
		Reset();
	}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: char *Text() const
	public string Text()
	{
		return pText;
	}

	public void Reset()
	{
	pText = text;
	text = StringFunctions.ChangeCharacter( text, 0, '\0' );
	textLength = 0;
	}

	public void Put( char ch )
	{
		pText ++= ch;
		pText = '\0';
		++textLength;
	}
	public override void PutLine()
	{
		file << text << std::endl;
		Reset();
	}

	public new void PutLine( string pText )
	{
	base.PutLine( pText );
	}

	public void Advance()
	{
		while ( *pText )
		{
		++pText;
		++textLength;
		}
	}

	public void Put( string pString )
	{
	pText = pString;
	Advance();
	}

	public void Reset( string pString )
	{
		Reset();
		Put( pString );
	}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: int Fit(int length) const
	public int Fit( int length )
	{
	return textLength + length < AnonymousEnum3.MaxLength;
	}
}

//--------------------------------------------------------------
//  TCodeGenerator      Code generator subclass of TBackend.
//--------------------------------------------------------------

public class TCodeGenerator : TBackend
{
	private readonly TAssemblyBuffer pAsmBuffer;

	//--Pointers to the list of all the float and string literals
	//--used in the source program.
	private TSymtabNode pFloatLitList;
	private TSymtabNode pStringLitList;

	public void Reg( TRegister r )
	{
		Put( registers[( int )r] );
	}
	public void Operator( TInstruction opcode )
	{
		Put( '\t' );
		Put( instructions[( int )opcode] );
	}
	public void Label( string pPrefix, int index )
	{
		AsmText() = string.Format("{0}_{1:D3}", pPrefix, index);
		Advance();
	}
	public void WordLabel( string pPrefix, int index )
	{
		AsmText() = string.Format("WORD PTR {0}_{1:D3}", pPrefix, index);
		Advance();
	}
	public void HighDWordLabel( string pPrefix, int index )
	{
		AsmText() = string.Format("WORD PTR {0}_{1:D3}+2", pPrefix, index);
		Advance();
	}
	public void Byte( TSymtabNode pId )
	{
		AsmText() = string.Format("BYTE PTR {0}_{1:D3}", pId.String(), pId.labelIndex);
		Advance();
	}
	public void Word( TSymtabNode pId )
	{
		AsmText() = string.Format("WORD PTR {0}_{1:D3}", pId.String(), pId.labelIndex);
		Advance();
	}
	public void HighDWord( TSymtabNode pId )
	{
		AsmText() = string.Format("WORD PTR {0}_{1:D3}+2", pId.String(), pId.labelIndex);
		Advance();
	}
	public void ByteIndirect( TRegister r )
	{
		AsmText() = string.Format("BYTE PTR [{0}]", registers[(int)r]);
		Advance();
	}
	public void WordIndirect( TRegister r )
	{
		AsmText() = string.Format("WORD PTR [{0}]", registers[(int)r]);
		Advance();
	}
	public void HighDWordIndirect( TRegister r )
	{
		AsmText() = string.Format("WORD PTR [{0}+2]", registers[(int)r]);
		Advance();
	}
	public void TaggedName( TSymtabNode pId )
	{
		AsmText() = string.Format("{0}_{1:D3}", pId.String(), pId.labelIndex);
		Advance();
	}
	public void NameLit( string pName )
	{
		AsmText() = string.Format("{0}", pName);
		Advance();
	}
	public void IntegerLit( int n )
	{
		AsmText() = string.Format("{0:D}", n);
		Advance();
	}
	public void CharLit( char ch )
	{
		AsmText() = string.Format("'{0}'", ch);
		Advance();
	}

	public void EmitStatementLabel( int index )
	{
		AsmText() = string.Format("{0}_{1:D3}:", DefineConstants.StmtLabelPrefix, index);
		PutLine();
	}

	//--Program
	public void EmitProgramPrologue()
	{
		PutLine( "\tDOSSEG" );
		PutLine( "\t.MODEL  small" );
		PutLine( "\t.STACK  1024" );
		PutLine();
		PutLine( "\t.CODE" );
		PutLine();
		PutLine( "\tPUBLIC\t_PascalMain" );
		PutLine( "\tINCLUDE\tpasextrn.inc" );
		PutLine();
    
		//--Equates for stack frame components.
		AsmText() = string.Format("{0}\t\tEQU\t<WORD PTR [bp+4]>", DefineConstants.StaticLink);
		PutLine();
		AsmText() = string.Format("{0}\t\tEQU\t<WORD PTR [bp-4]>", DefineConstants.ReturnValue);
		PutLine();
		AsmText() = string.Format("{0}\tEQU\t<WORD PTR [bp-2]>", DefineConstants.HighReturnValue);
		PutLine();
	}
	public void EmitProgramEpilogue( TSymtabNode pProgramId )
	{
		TSymtabNode pId;
		TType pType;
    
		PutLine();
		PutLine( "\t.DATA" );
		PutLine();
    
		//--Emit declarations for the program's global variables.
		for ( pId = pProgramId.defn.routine.locals.pVariableIds; pId != null; pId = pId.next )
		{
		AsmText() = string.Format("{0}_{1:D3}\t", pId.String(), pId.labelIndex);
		Advance();
    
		pType = pId.pType;
		if ( pType == pCharType )
			AsmText() = "DB\t0";
		else if ( pType == pRealType )
			AsmText() = "DD\t0.0";
		else if ( pType.IsScalar() == 0 )
			AsmText() = string.Format("DB\t{0:D} DUP(0)", pType.size);
		else
			AsmText() = "DW\t0";
    
		PutLine();
		}
    
		//--Emit declarations for the program's floating point literals.
		for ( pId = pFloatLitList; pId != null; pId = pId.next )
		{
		AsmText() = string.Format("{0}_{1:D3}\tDD\t{2:e}", DefineConstants.FloatLabelPrefix, pId.labelIndex, pId.defn.constant.value.real);
		PutLine();
		}
    
		//--Emit declarations for the program's string literals.
		for ( pId = pStringLitList; pId != null; pId = pId.next )
		{
		int i;
		char[] pString = pId.String();
		int length = pString.Length - 2; // don't count quotes
    
		AsmText() = string.Format("{0}_{1:D3}\tDB\t\"", DefineConstants.StringLabelPrefix, pId.labelIndex);
		Advance();
    
		for ( i = 1; i <= length; ++i )
			Put( pString[i] );
		Put( '\"' );
		PutLine();
		}
    
		PutLine();
		AsmText() = "\tEND";
		PutLine();
	}
	public void EmitMain( TSymtabNode pMainId )
	{
		TSymtabNode pRtnId;
    
		EmitProgramHeaderComment( pMainId );
		EmitVarDeclComment( pMainId.defn.routine.locals.pVariableIds );
    
		//--Emit code for nested subroutines.
		for ( pRtnId = pMainId.defn.routine.locals.pRoutineIds; pRtnId != null; pRtnId = pRtnId.next )
		EmitRoutine( pRtnId );
    
		//--Switch to main's intermediate code and emit code
		//--for its compound statement.
		pIcode = pMainId.defn.routine.pIcode;
		currentNestingLevel = 1;
		EmitMainPrologue();
		EmitCompound();
		EmitMainEpilogue();
	}
	public void EmitMainPrologue()
	{
		PutLine();
		PutLine( "_PascalMain\tPROC" );
		PutLine();
    
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Bp );
			pAsmBuffer.PutLine();
		}; // dynamic link
		{
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Bp );
			pAsmBuffer.Put( ',' );
			Reg( TRegister.Sp );
			pAsmBuffer.PutLine();
		}; // new stack frame base
	}
	public void EmitMainEpilogue()
	{
		PutLine();
    
		{
			Operator( TInstruction.Pop );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Bp );
			pAsmBuffer.PutLine();
		}; // restore caller's stack frame
		{
			Operator( TInstruction.Ret );
			pAsmBuffer.PutLine();
		}; // return
    
		PutLine();
		PutLine( "_PascalMain\tENDP" );
	}

	//--Routines
	public void EmitRoutine( TSymtabNode pRoutineId )
	{
		TSymtabNode pRtnId;
    
		EmitSubroutineHeaderComment( pRoutineId );
    
		//--Emit code for the parameters and local variables.
		EmitDeclarations( pRoutineId );
    
		//--Emit code for nested subroutines.
		for ( pRtnId = pRoutineId.defn.routine.locals.pRoutineIds; pRtnId != null; pRtnId = pRtnId.next )
		EmitRoutine( pRtnId );
    
		//--Switch to the routine's intermediate code and emit code
		//--for its compound statement.
		pIcode = pRoutineId.defn.routine.pIcode;
		currentNestingLevel = pRoutineId.level + 1; // level of locals
		EmitRoutinePrologue( pRoutineId );
		EmitCompound();
		EmitRoutineEpilogue( pRoutineId );
	}
	public void EmitRoutinePrologue( TSymtabNode pRoutineId )
	{
		PutLine();
		AsmText() = string.Format("{0}_{1:D3}\tPROC", pRoutineId.String(), pRoutineId.labelIndex);
		PutLine();
		PutLine();
    
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Bp );
			pAsmBuffer.PutLine();
		}; // dynamic link
		{
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Bp );
			pAsmBuffer.Put( ',' );
			Reg( TRegister.Sp );
			pAsmBuffer.PutLine();
		}; // new stack frame base
    
		//--Allocate stack space for a function's return value.
		if ( pRoutineId.defn.how == TDefnCode.DcFunction )
		{
		{
			Operator( TInstruction.Sub );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Sp );
			pAsmBuffer.Put( ',' );
			IntegerLit( 4 );
			pAsmBuffer.PutLine();
		};
		}
    
		//--Allocate stack space for the local variables.
		if ( pRoutineId.defn.routine.totalLocalSize > 0 )
		{
		{
			Operator( TInstruction.Sub );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Sp );
			pAsmBuffer.Put( ',' );
			IntegerLit( pRoutineId.defn.routine.totalLocalSize );
			pAsmBuffer.PutLine();
		};
		}
	}
	public void EmitRoutineEpilogue( TSymtabNode pRoutineId )
	{
		PutLine();
    
		//--Load a function's return value into the ax or dx:ax registers.
		if ( pRoutineId.defn.how == TDefnCode.DcFunction )
		{
		{
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			NameLit( DefineConstants.ReturnValue );
			pAsmBuffer.PutLine();
		};
		if ( pRoutineId.pType == pRealType )
		{
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.Put( ',' );
				NameLit( DefineConstants.HighReturnValue );
				pAsmBuffer.PutLine();
			};
		}
		}
    
		{
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Sp );
			pAsmBuffer.Put( ',' );
			Reg( TRegister.Bp );
			pAsmBuffer.PutLine();
		}; // cut back to caller's stack
		{
			Operator( TInstruction.Pop );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Bp );
			pAsmBuffer.PutLine();
		}; // restore caller's stack frame
    
		{
			Operator( TInstruction.Ret );
			pAsmBuffer.Put( '\t' );
			IntegerLit( pRoutineId.defn.routine.totalParmSize + 2 );
			pAsmBuffer.PutLine();
		};
						// return and cut back stack
    
		PutLine();
		AsmText() = string.Format("{0}_{1:D3}\tENDP", pRoutineId.String(), pRoutineId.labelIndex);
		PutLine();
	}
	public TType * EmitSubroutineCall( TSymtabNode pRoutineId )
	{
		return pRoutineId.defn.routine.which == ( ( int )TRoutineCode.RcDeclared ) != 0 ? EmitDeclaredSubroutineCall( pRoutineId ) : EmitStandardSubroutineCall( pRoutineId );
	}
	public TType * EmitDeclaredSubroutineCall( TSymtabNode pRoutineId )
	{
		int oldLevel = currentNestingLevel; // level of caller
		int newLevel = pRoutineId.level + 1; // level of callee's locals
    
		//--Emit code to push the actual parameter values onto the stack.
		GetToken();
		if ( token == TTokenCode.TcLParen )
		{
		EmitActualParameters( pRoutineId );
		GetToken();
		}
    
		//--Push the static link onto the stack.
		if ( newLevel == oldLevel + 1 )
		{
    
		//--Calling a routine nested within the caller:
		//--Push pointer to caller's stack frame.
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Bp );
			pAsmBuffer.PutLine();
		};
		} else if ( newLevel == oldLevel )
		{
    
		//--Calling another routine at the same level:
		//--Push pointer to stack frame of common parent.
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			NameLit( DefineConstants.StaticLink );
			pAsmBuffer.PutLine();
		};
		} else
		{ // newLevel < oldLevel
    
		//--Calling a routine at a lesser level (nested less deeply):
		//--Push pointer to stack frame of nearest common ancestor
		//--(the callee's parent).
		EmitAdjustBP( newLevel - 1 );
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Bp );
			pAsmBuffer.PutLine();
		};
		EmitRestoreBP( newLevel - 1 );
		}
    
		{
			Operator( TInstruction.Call );
			pAsmBuffer.Put( '\t' );
			TaggedName( pRoutineId );
			pAsmBuffer.PutLine();
		};
    
		return pRoutineId.pType;
	}
	public TType * EmitStandardSubroutineCall( TSymtabNode pRoutineId )
	{
		switch ( pRoutineId.defn.routine.which )
		{
    
		case TRoutineCode.RcRead:
		case TRoutineCode.RcReadln:
			return EmitReadReadlnCall( pRoutineId );
    
		case TRoutineCode.RcWrite:
		case TRoutineCode.RcWriteln:
			return EmitWriteWritelnCall( pRoutineId );
    
		case TRoutineCode.RcEof:
		case TRoutineCode.RcEoln:
			return EmitEofEolnCall( pRoutineId );
    
		case TRoutineCode.RcAbs:
		case TRoutineCode.RcSqr:
			return EmitAbsSqrCall( pRoutineId );
    
		case TRoutineCode.RcArctan:
		case TRoutineCode.RcCos:
		case TRoutineCode.RcExp:
		case TRoutineCode.RcLn:
		case TRoutineCode.RcSin:
		case TRoutineCode.RcSqrt:
			return EmitArctanCosExpLnSinSqrtCall( pRoutineId );
    
		case TRoutineCode.RcPred:
		case TRoutineCode.RcSucc:
			return EmitPredSuccCall( pRoutineId );
    
		case TRoutineCode.RcChr:
			return EmitChrCall();
		case TRoutineCode.RcOdd:
			return EmitOddCall();
		case TRoutineCode.RcOrd:
			return EmitOrdCall();
    
		case TRoutineCode.RcRound:
		case TRoutineCode.RcTrunc:
			return EmitRoundTruncCall( pRoutineId );
    
		default:
			return pDummyType;
		}
	}
	public void EmitActualParameters( TSymtabNode pRoutineId )
	{
		TSymtabNode pFormalId; // ptr to formal parm's symtab node
    
		//--Loop to emit code for each actual parameter.
		for ( pFormalId = pRoutineId.defn.routine.locals.pParmIds; pFormalId != null; pFormalId = pFormalId.next )
		{
    
		TType pFormalType = pFormalId.pType;
		GetToken();
    
		//--VAR parameter: EmitVariable will leave the actual
		//--               parameter's addresss on top of the stack.
		if ( pFormalId.defn.how == TDefnCode.DcVarParm )
			EmitVariable( pNode, true );
    
		//--Value parameter: Emit code to load a scalar value into
		//--                 ax or dx:ax, or push an array or record
		//--                 address onto the stack.
		else
		{
			TType pActualType = EmitExpression();
    
			if ( pFormalType == pRealType )
			{
    
			//--Real formal parm
			if ( pActualType == pIntegerType )
			{
				{
					Operator( TInstruction.Push );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Ax );
					pAsmBuffer.PutLine();
				};
				{
					Operator( TInstruction.Call );
					pAsmBuffer.Put( '\t' );
					NameLit( DefineConstants.FloatConvert );
					pAsmBuffer.PutLine();
				};
				{
					Operator( TInstruction.Add );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Sp );
					pAsmBuffer.Put( ',' );
					IntegerLit( 2 );
					pAsmBuffer.PutLine();
				};
			}
			{
				Operator( TInstruction.Push );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Push );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			} else if ( pActualType.IsScalar() == 0 )
			{
    
			//--Block move onto the stack.  Round the next offset
			//--up to an even number.
			int size = pActualType.size;
			int offset = ( size & 1 ) != 0 ? size + 1 : size;
    
			{
				Operator( TInstruction.Cld );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Si );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Sub );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Sp );
				pAsmBuffer.Put( ',' );
				IntegerLit( offset );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Di );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Sp );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Cx );
				pAsmBuffer.Put( ',' );
				IntegerLit( size );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ds );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Es );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.RepMovsb );
				pAsmBuffer.PutLine();
			};
			} else
			{
			{
				Operator( TInstruction.Push );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			}
		}
		}
	}

	//--Standard routines
	public TType * EmitReadReadlnCall( TSymtabNode pRoutineId )
	{
		//--Actual parameters are optional for readln.
		GetToken();
		if ( token == TTokenCode.TcLParen )
		{
    
		//--Loop to emit code to read each parameter value.
		do
		{
			//--Variable
			GetToken();
			TSymtabNode pVarId = pNode;
			TType pVarType = EmitVariable( pVarId, true ).Base();
    
			//--Read the value.
			if ( pVarType == pIntegerType )
			{
			{
				Operator( TInstruction.Call );
				pAsmBuffer.Put( '\t' );
				NameLit( DefineConstants.ReadInteger );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Bx );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				WordIndirect( TRegister.Bx );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			} else if ( pVarType == pRealType )
			{
			{
				Operator( TInstruction.Call );
				pAsmBuffer.Put( '\t' );
				NameLit( DefineConstants.ReadReal );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Bx );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				WordIndirect( TRegister.Bx );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				HighDWordIndirect( TRegister.Bx );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
			} else if ( pVarType == pCharType )
			{
			{
				Operator( TInstruction.Call );
				pAsmBuffer.Put( '\t' );
				NameLit( DefineConstants.ReadChar );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Bx );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				ByteIndirect( TRegister.Bx );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Al );
				pAsmBuffer.PutLine();
			};
			}
		} while ( token == TTokenCode.TcComma );
    
		GetToken(); // token after )
		}
    
		//--Skip the rest of the input line if readln.
		if ( pRoutineId.defn.routine.which == TRoutineCode.RcReadln )
		{
		{
			Operator( TInstruction.Call );
			pAsmBuffer.Put( '\t' );
			NameLit( DefineConstants.ReadLine );
			pAsmBuffer.PutLine();
		};
		}
    
		return pDummyType;
	}
	public TType * EmitWriteWritelnCall( TSymtabNode pRoutineId )
	{
		const int defaultFieldWidth = 10;
		const int defaultPrecision = 2;
    
		//--Actual parameters are optional for writeln.
		GetToken();
		if ( token == TTokenCode.TcLParen )
		{
    
		//--Loop to emit code for each parameter value.
		do
		{
			//--<expr-1>
			GetToken();
			TType pExprType = EmitExpression().Base();
    
			//--Push the scalar value to be written onto the stack.
			//--A string value is already on the stack.
			if ( pExprType.form != TFormCode.FcArray )
			EmitPushOperand( pExprType );
    
			if ( token == TTokenCode.TcColon )
			{
    
			//--Field width <expr-2>
			//--Push its value onto the stack.
			GetToken();
			EmitExpression();
			{
				Operator( TInstruction.Push );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
    
			if ( token == TTokenCode.TcColon )
			{
    
				//--Precision <expr-3>
				//--Push its value onto the stack.
				GetToken();
				EmitExpression();
				{
					Operator( TInstruction.Push );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Ax );
					pAsmBuffer.PutLine();
				};
			} else if ( pExprType == pRealType )
			{
    
				{
				//--No precision: Push the default precision.
					Operator( TInstruction.Mov );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Ax );
					pAsmBuffer.Put( ',' );
					IntegerLit( defaultPrecision );
					pAsmBuffer.PutLine();
				};
				{
					Operator( TInstruction.Push );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Ax );
					pAsmBuffer.PutLine();
				};
			}
			} else
			{
    
			//--No field width: Push the default field width and
			//--                the default precision.
			if ( pExprType == pIntegerType )
			{
				{
					Operator( TInstruction.Mov );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Ax );
					pAsmBuffer.Put( ',' );
					IntegerLit( defaultFieldWidth );
					pAsmBuffer.PutLine();
				};
				{
					Operator( TInstruction.Push );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Ax );
					pAsmBuffer.PutLine();
				};
			} else if ( pExprType == pRealType )
			{
				{
					Operator( TInstruction.Mov );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Ax );
					pAsmBuffer.Put( ',' );
					IntegerLit( defaultFieldWidth );
					pAsmBuffer.PutLine();
				};
				{
					Operator( TInstruction.Push );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Ax );
					pAsmBuffer.PutLine();
				};
				{
					Operator( TInstruction.Mov );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Ax );
					pAsmBuffer.Put( ',' );
					IntegerLit( defaultPrecision );
					pAsmBuffer.PutLine();
				};
				{
					Operator( TInstruction.Push );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Ax );
					pAsmBuffer.PutLine();
				};
			} else
			{
				{
					Operator( TInstruction.Mov );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Ax );
					pAsmBuffer.Put( ',' );
					IntegerLit( 0 );
					pAsmBuffer.PutLine();
				};
				{
					Operator( TInstruction.Push );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Ax );
					pAsmBuffer.PutLine();
				};
			}
			}
    
			//--Emit the code to write the value.
			if ( pExprType == pIntegerType )
			{
			{
				Operator( TInstruction.Call );
				pAsmBuffer.Put( '\t' );
				NameLit( DefineConstants.WriteInteger );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Add );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Sp );
				pAsmBuffer.Put( ',' );
				IntegerLit( 4 );
				pAsmBuffer.PutLine();
			};
			} else if ( pExprType == pRealType )
			{
			{
				Operator( TInstruction.Call );
				pAsmBuffer.Put( '\t' );
				NameLit( DefineConstants.WriteReal );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Add );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Sp );
				pAsmBuffer.Put( ',' );
				IntegerLit( 8 );
				pAsmBuffer.PutLine();
			};
			} else if ( pExprType == pBooleanType )
			{
			{
				Operator( TInstruction.Call );
				pAsmBuffer.Put( '\t' );
				NameLit( DefineConstants.WriteBoolean );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Add );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Sp );
				pAsmBuffer.Put( ',' );
				IntegerLit( 4 );
				pAsmBuffer.PutLine();
			};
			} else if ( pExprType == pCharType )
			{
			{
				Operator( TInstruction.Call );
				pAsmBuffer.Put( '\t' );
				NameLit( DefineConstants.WriteChar );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Add );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Sp );
				pAsmBuffer.Put( ',' );
				IntegerLit( 4 );
				pAsmBuffer.PutLine();
			};
			} else
			{ // string
    
			{
			//--Push the string length onto the stack.
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				IntegerLit( pExprType.array.elmtCount );
				pAsmBuffer.PutLine();
			};
    
			{
				Operator( TInstruction.Push );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Call );
				pAsmBuffer.Put( '\t' );
				NameLit( DefineConstants.WriteString );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Add );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Sp );
				pAsmBuffer.Put( ',' );
				IntegerLit( 6 );
				pAsmBuffer.PutLine();
			};
			}
    
		} while ( token == TTokenCode.TcComma );
    
		GetToken(); // token after )
		}
    
		//--End the line if writeln.
		if ( pRoutineId.defn.routine.which == TRoutineCode.RcWriteln )
		{
		{
			Operator( TInstruction.Call );
			pAsmBuffer.Put( '\t' );
			NameLit( DefineConstants.WriteLine );
			pAsmBuffer.PutLine();
		};
		}
    
		return pDummyType;
	}
	public TType * EmitEofEolnCall( TSymtabNode pRoutineId )
	{
		{
			Operator( TInstruction.Call );
			pAsmBuffer.Put( '\t' );
			NameLit( pRoutineId.defn.routine.which == ( ( int )TRoutineCode.RcEof ) != 0 ? DefineConstants.StdEof : DefineConstants.StdEoln );
			pAsmBuffer.PutLine();
		};
    
		GetToken(); // token after function name
		return pBooleanType;
	}
	public TType * EmitAbsSqrCall( TSymtabNode pRoutineId )
	{
		GetToken(); // (
		GetToken();
    
		TType pParmType = EmitExpression().Base();
    
		switch ( pRoutineId.defn.routine.which )
		{
    
		case TRoutineCode.RcAbs:
			if ( pParmType == pIntegerType )
			{
			int nonNegativeLabelIndex = ++asmLabelIndex;
    
			{
				Operator( TInstruction.Cmp );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				IntegerLit( 0 );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Jge );
				pAsmBuffer.Put( '\t' );
				Label( DefineConstants.StmtLabelPrefix, nonNegativeLabelIndex );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Neg );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
    
			EmitStatementLabel( nonNegativeLabelIndex );
			} else
			{
			EmitPushOperand( pParmType );
			{
				Operator( TInstruction.Call );
				pAsmBuffer.Put( '\t' );
				NameLit( DefineConstants.StdAbs );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Add );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Sp );
				pAsmBuffer.Put( ',' );
				IntegerLit( 4 );
				pAsmBuffer.PutLine();
			};
			}
			break;
    
		case TRoutineCode.RcSqr:
			if ( pParmType == pIntegerType )
			{
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Imul );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
			} else
			{
			EmitPushOperand( pParmType );
			EmitPushOperand( pParmType );
			{
				Operator( TInstruction.Call );
				pAsmBuffer.Put( '\t' );
				NameLit( DefineConstants.FloatMultiply );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Add );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Sp );
				pAsmBuffer.Put( ',' );
				IntegerLit( 8 );
				pAsmBuffer.PutLine();
			};
			}
			break;
		}
    
		GetToken(); // token after )
		return pParmType;
	}
	public TType * EmitArctanCosExpLnSinSqrtCall( TSymtabNode pRoutineId )
	{
		string stdFuncName;
    
		GetToken(); // (
		GetToken();
    
		//--Evaluate the parameter, and convert an integer value to
		//--real if necessary.
		TType pParmType = EmitExpression().Base();
		if ( pParmType == pIntegerType )
		{
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Call );
			pAsmBuffer.Put( '\t' );
			NameLit( DefineConstants.FloatConvert );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Add );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Sp );
			pAsmBuffer.Put( ',' );
			IntegerLit( 2 );
			pAsmBuffer.PutLine();
		};
		}
    
		EmitPushOperand( pRealType );
    
		switch ( pRoutineId.defn.routine.which )
		{
		case TRoutineCode.RcArctan:
			stdFuncName = DefineConstants.StdArctan;
			break;
		case TRoutineCode.RcCos:
			stdFuncName = DefineConstants.StdCos;
			break;
		case TRoutineCode.RcExp:
			stdFuncName = DefineConstants.StdExp;
			break;
		case TRoutineCode.RcLn:
			stdFuncName = DefineConstants.StdLn;
			break;
		case TRoutineCode.RcSin:
			stdFuncName = DefineConstants.StdSin;
			break;
		case TRoutineCode.RcSqrt:
			stdFuncName = DefineConstants.StdSqrt;
			break;
		}
    
		{
			Operator( TInstruction.Call );
			pAsmBuffer.Put( '\t' );
			NameLit( stdFuncName );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Add );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Sp );
			pAsmBuffer.Put( ',' );
			IntegerLit( 4 );
			pAsmBuffer.PutLine();
		};
    
		GetToken(); // token after )
		return pRealType;
	}
	public TType * EmitPredSuccCall( TSymtabNode pRoutineId )
	{
		GetToken(); // (
		GetToken();
    
		TType pParmType = EmitExpression();
    
		{
			Operator( pRoutineId.defn.routine.which == ( ( int )TRoutineCode.RcPred ) != 0 ? TInstruction.Decr : TInstruction.Incr );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		};
    
		GetToken(); // token after )
		return pParmType;
	}
	public TType * EmitChrCall()
	{
		GetToken(); // (
		GetToken();
		EmitExpression();
    
		GetToken(); // token after )
		return pCharType;
	}
	public TType * EmitOddCall()
	{
		GetToken(); // (
		GetToken();
		EmitExpression();
    
		{
			Operator( TInstruction.And );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			IntegerLit( 1 );
			pAsmBuffer.PutLine();
		};
    
		GetToken(); // token after )
		return pBooleanType;
	}
	public TType * EmitOrdCall()
	{
		GetToken(); // (
		GetToken();
		EmitExpression();
    
		GetToken(); // token after )
		return pIntegerType;
	}
	public TType * EmitRoundTruncCall( TSymtabNode pRoutineId )
	{
		GetToken(); // (
		GetToken();
		EmitExpression();
    
		EmitPushOperand( pRealType );
		{
			Operator( TInstruction.Call );
			pAsmBuffer.Put( '\t' );
			NameLit( pRoutineId.defn.routine.which == ( ( int )TRoutineCode.RcRound ) != 0 ? DefineConstants.StdRound : DefineConstants.StdTrunc );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Add );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Sp );
			pAsmBuffer.Put( ',' );
			IntegerLit( 4 );
			pAsmBuffer.PutLine();
		};
    
		GetToken(); // token after )
		return pIntegerType;
	}

	//--Declarations
	public void EmitDeclarations( TSymtabNode pRoutineId )
	{
		TSymtabNode pParmId = pRoutineId.defn.routine.locals.pParmIds;
		TSymtabNode pVarId = pRoutineId.defn.routine.locals.pVariableIds;
    
		EmitVarDeclComment( pRoutineId.defn.routine.locals.pVariableIds );
		PutLine();
    
		//--Subroutine parameters
		while ( pParmId != null )
		{
		EmitStackOffsetEquate( pParmId );
		pParmId = pParmId.next;
		}
    
		//--Variables
		while ( pVarId != null )
		{
		EmitStackOffsetEquate( pVarId );
		pVarId = pVarId.next;
		}
	}
	public void EmitStackOffsetEquate( TSymtabNode pId )
	{
	//C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent to pointers to value types:
	//ORIGINAL LINE: char *pName = pId->String();
		char pName = pId.String();
		int labelIndex = pId.labelIndex;
		int offset = pId.defn.data.offset;
		TType pType = pId.pType;
    
		if ( pType == pCharType )
	//C++ TO C# CONVERTER TODO TASK: The following line has a C format specifier which cannot be directly translated to C#:
	//ORIGINAL LINE: sprintf(AsmText(), "%s_%03d\tEQU\t<BYTE PTR [bp%+d]>", pName, labelIndex, offset);
		AsmText() = string.Format("{0}_{1:D3}\tEQU\t<BYTE PTR [bp%+d]>", pName, labelIndex, offset);
		else
	//C++ TO C# CONVERTER TODO TASK: The following line has a C format specifier which cannot be directly translated to C#:
	//ORIGINAL LINE: sprintf(AsmText(), "%s_%03d\tEQU\t<WORD PTR [bp%+d]>", pName, labelIndex, offset);
		AsmText() = string.Format("{0}_{1:D3}\tEQU\t<WORD PTR [bp%+d]>", pName, labelIndex, offset);
    
		PutLine();
	}

	//--Loads and pushes
	public void EmitAdjustBP( int level )
	{
		//--Don't do anything if local or global.
		if ( ( level == currentNestingLevel ) || ( level == 1 ) )
			return;
    
			{
		//--Emit code to chase static links.
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Cx );
			pAsmBuffer.Put( ',' );
			Reg( TRegister.Bp );
			pAsmBuffer.PutLine();
			}; // save bp in cx
		do
		{
		{
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Bp );
			pAsmBuffer.Put( ',' );
			NameLit( DefineConstants.StaticLink );
			pAsmBuffer.PutLine();
		}; // chase
		} while ( ++level < currentNestingLevel );
	}
	public void EmitRestoreBP( int level )
	{
		//--Don't do anything if local or global.
		if ( ( level == currentNestingLevel ) || ( level == 1 ) )
			return;
    
			{
		//--Emit code to restore bp.
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Bp );
			pAsmBuffer.Put( ',' );
			Reg( TRegister.Cx );
			pAsmBuffer.PutLine();
			};
	}
	public void EmitLoadValue( TSymtabNode pId )
	{
		TType pType = pId.pType;
    
		EmitAdjustBP( pId.level );
    
		if ( pId.defn.how == TDefnCode.DcVarParm )
		{
		//--VAR formal parameter.
		//--ax or dx:ax = value the address points to
		{
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Bx );
			pAsmBuffer.Put( ',' );
			Word( pId );
			pAsmBuffer.PutLine();
		};
		if ( pType == pCharType )
		{
    
			{
			//--Character:  al = value
				Operator( TInstruction.Sub );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Al );
				pAsmBuffer.Put( ',' );
				ByteIndirect( TRegister.Bx );
				pAsmBuffer.PutLine();
			};
		} else if ( pType == pRealType )
		{
    
			{
			//--Real: dx:ax = value
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				WordIndirect( TRegister.Bx );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				HighDWordIndirect( TRegister.Bx );
				pAsmBuffer.PutLine();
			};
		} else
		{
    
			{
			//--Integer or enumeration: ax = value
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				WordIndirect( TRegister.Bx );
				pAsmBuffer.PutLine();
			};
		}
		} else
		{
    
		//--Load the value into ax or dx:ax.
		if ( pType == pCharType )
		{
    
			{
			//--Character:  al = value
				Operator( TInstruction.Sub );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Al );
				pAsmBuffer.Put( ',' );
				Byte( pId );
				pAsmBuffer.PutLine();
			};
		} else if ( pType == pRealType )
		{
    
			{
			//--Real: dx:ax = value
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				Word( pId );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.Put( ',' );
				HighDWord( pId );
				pAsmBuffer.PutLine();
			};
		} else
		{
    
			{
			//--Integer or enumeration: ax = value
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				Word( pId );
				pAsmBuffer.PutLine();
			};
		}
		}
    
		EmitRestoreBP( pId.level );
	}
	public void EmitLoadFloatLit( TSymtabNode pNode )
	{
		TSymtabNode pf;
    
		{
		//--dx:ax = value
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			WordLabel( DefineConstants.FloatLabelPrefix, pNode.labelIndex );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Dx );
			pAsmBuffer.Put( ',' );
			HighDWordLabel( DefineConstants.FloatLabelPrefix, pNode.labelIndex );
			pAsmBuffer.PutLine();
		};
    
		//--Check if the float is already in the float literal list.
		for ( pf = pFloatLitList; pf != null; pf = pf.next )
		{
		if ( pf == pNode )
			return;
		}
    
		//--Append it to the list if it isn't already there.
		pNode.next = pFloatLitList;
		pFloatLitList = pNode;
	}
	public void EmitPushStringLit( TSymtabNode pNode )
	{
		TSymtabNode ps;
    
		{
		//--ax = addresss of string
			Operator( TInstruction.Lea );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			WordLabel( DefineConstants.StringLabelPrefix, pNode.labelIndex );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		};
    
		//--Check if the string is already in the string literal list.
		for ( ps = pStringLitList; ps != null; ps = ps.next )
		{
		if ( ps == pNode )
			return;
		}
    
		//--Append it to the list if it isn't already there.
		pNode.next = pStringLitList;
		pStringLitList = pNode;
	}
	public void EmitPushOperand( TType pType )
	{
		if ( pType.IsScalar() == 0 )
			return;
    
		if ( pType == pRealType )
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Dx );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		};
	}
	public void EmitPushAddress( TSymtabNode pId )
	{
		int varLevel = pId.level;
		int isVarParm = ( int )pId.defn.how == ( int )TDefnCode.DcVarParm;
    
		EmitAdjustBP( varLevel );
    
		{
			Operator( isVarParm != 0 ? TInstruction.Mov : TInstruction.Lea );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			Word( pId );
			pAsmBuffer.PutLine();
		}
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		};
    
		EmitRestoreBP( varLevel );
	}
	public void EmitPushReturnValueAddress( TSymtabNode pId )
	{
		EmitAdjustBP( pId.level + 1 );
    
		{
			Operator( TInstruction.Lea );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			NameLit( DefineConstants.ReturnValue );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		};
    
		EmitRestoreBP( pId.level + 1 );
	}
	public void EmitPromoteToReal( TType pType1, TType pType2 )
	{
		if ( pType2 == pIntegerType )
		{ // xxx_1 integer_2
		{
			Operator( TInstruction.Call );
			pAsmBuffer.Put( '\t' );
			NameLit( DefineConstants.FloatConvert );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Add );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Sp );
			pAsmBuffer.Put( ',' );
			IntegerLit( 2 );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Dx );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		}; // xxx_1 real_2
		}
    
		if ( pType1 == pIntegerType )
		{ // integer_1 real_2
		{
			Operator( TInstruction.Pop );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Pop );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Dx );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Pop );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Bx );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Dx );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Bx );
			pAsmBuffer.PutLine();
		}; // real_2 integer_1
    
		{
			Operator( TInstruction.Call );
			pAsmBuffer.Put( '\t' );
			NameLit( DefineConstants.FloatConvert );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Add );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Sp );
			pAsmBuffer.Put( ',' );
			IntegerLit( 2 );
			pAsmBuffer.PutLine();
		}; // real_2 real_1
    
		{
			Operator( TInstruction.Pop );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Bx );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Pop );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Cx );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Dx );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Cx );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Bx );
			pAsmBuffer.PutLine();
		}; // real_1 real_2
		}
	}

	//--Statements
	public void EmitStatement()
	{
		//--Emit the current statement as a comment.
		EmitStmtComment();
    
		switch ( token )
		{
    
		case TTokenCode.TcIdentifier:
		{
			if ( pNode.defn.how == TDefnCode.DcProcedure )
			EmitSubroutineCall( pNode );
			else
			EmitAssignment( pNode );
			break;
		}
    
		case TTokenCode.TcREPEAT:
			EmitREPEAT();
			break;
		case TTokenCode.TcWHILE:
			EmitWHILE();
			break;
		case TTokenCode.TcFOR:
			EmitFOR();
			break;
		case TTokenCode.TcIF:
			EmitIF();
			break;
		case TTokenCode.TcCASE:
			EmitCASE();
			break;
		case TTokenCode.TcBEGIN:
			EmitCompound();
			break;
		}
	}
	public void EmitStatementList( TTokenCode terminator )
	{
		//--Loop to emit code for statements and skip semicolons.
		do
		{
		EmitStatement();
		while ( token == TTokenCode.TcSemicolon )
			GetToken();
		} while ( token != terminator );
	}
	public void EmitAssignment( TSymtabNode pTargetId )
	{
		TType pTargetType = pTargetId.pType;
					// ptr to target type object
		TType pExprType; // ptr to expression type object
		int addressOnStack; // true if target address has been pushed
					//   onto the runtime stack
    
		//--Assignment to a function name.
		if ( pTargetId.defn.how == TDefnCode.DcFunction )
		{
		EmitPushReturnValueAddress( pTargetId );
		addressOnStack = true;
		GetToken();
		}
    
		//--Assignment to a nonscalar, a formal VAR parameter, or to
		//--a nonglobal and nonlocal variable. EmitVariable emits code
		//--that leaves the target address on top of the runtime stack.
		else if ( ( pTargetId.defn.how == TDefnCode.DcVarParm ) || ( pTargetType.IsScalar() == 0 ) || ((pTargetId.level > 1) && (pTargetId.level < currentNestingLevel)) )
		{
		pTargetType = EmitVariable( pTargetId, true );
		addressOnStack = true;
		}
    
		//--Assignment to a global or local scalar. A mov will be emitted
		//--after the code for the expression.
		else
		{
		GetToken();
		pTargetType = pTargetId.pType;
		addressOnStack = false;
		}
    
		//--Emit code for the expression.
		GetToken();
		pExprType = EmitExpression();
    
		//--Emit code to do the assignment.
		if ( ( pTargetType.Base() == pIntegerType ) || (pTargetType.Base().form == TFormCode.FcEnum) )
		{
		if ( addressOnStack != 0 )
		{
			{
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Bx );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				WordIndirect( TRegister.Bx );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
		} else
		{
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Word( pTargetId );
			pAsmBuffer.Put( ',' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		};
		} else if ( pTargetType.Base() == pCharType )
		{
    
		//--char := char
		if ( addressOnStack != 0 )
		{
			{
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Bx );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				ByteIndirect( TRegister.Bx );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Al );
				pAsmBuffer.PutLine();
			};
		} else
		{
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Byte( pTargetId );
			pAsmBuffer.Put( ',' );
			Reg( TRegister.Al );
			pAsmBuffer.PutLine();
		};
		} else if ( pTargetType == pRealType )
		{
    
		//--real := ...
		if ( pExprType == pIntegerType )
		{
    
			{
			//--Convert an integer value to real.
				Operator( TInstruction.Push );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Call );
				pAsmBuffer.Put( '\t' );
				NameLit( DefineConstants.FloatConvert );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Add );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Sp );
				pAsmBuffer.Put( ',' );
				IntegerLit( 2 );
				pAsmBuffer.PutLine();
			};
		}
    
		//--... real
		if ( addressOnStack != 0 )
		{
			{
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Bx );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				WordIndirect( TRegister.Bx );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				HighDWordIndirect( TRegister.Bx );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
		} else
		{
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Word( pTargetId );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				HighDWord( pTargetId );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
		}
		} else
		{
    
		//--array  := array
		//--record := record
		{
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Cx );
			pAsmBuffer.Put( ',' );
			IntegerLit( pTargetType.size );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Pop );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Si );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Pop );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Di );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			Reg( TRegister.Ds );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Es );
			pAsmBuffer.Put( ',' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Cld );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.RepMovsb );
			pAsmBuffer.PutLine();
		};
		}
	}
	public void EmitREPEAT()
	{
		int stmtListLabelIndex = ++asmLabelIndex;
		int followLabelIndex = ++asmLabelIndex;
    
		EmitStatementLabel( stmtListLabelIndex );
    
		//--<stmt-list> UNTIL
		GetToken();
		EmitStatementList( TTokenCode.TcUNTIL );
    
		EmitStmtComment();
    
		//--<expr>
		GetToken();
		EmitExpression();
    
		{
		//--Decide whether or not to branch back to the loop start.
			Operator( TInstruction.Cmp );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			IntegerLit( 1 );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Je );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, followLabelIndex );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Jmp );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, stmtListLabelIndex );
			pAsmBuffer.PutLine();
		};
    
		EmitStatementLabel( followLabelIndex );
	}
	public void EmitWHILE()
	{
		int exprLabelIndex = ++asmLabelIndex;
		int stmtLabelIndex = ++asmLabelIndex;
		int followLabelIndex = ++asmLabelIndex;
    
		GetToken();
		GetLocationMarker(); // ignored
    
		EmitStatementLabel( exprLabelIndex );
    
		//--<expr>
		GetToken();
		EmitExpression();
    
		{
			Operator( TInstruction.Cmp );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			IntegerLit( 1 );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Je );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, stmtLabelIndex );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Jmp );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, followLabelIndex );
			pAsmBuffer.PutLine();
		};
    
		EmitStatementLabel( stmtLabelIndex );
    
		//--DO <stmt>
		GetToken();
		EmitStatement();
    
		{
			Operator( TInstruction.Jmp );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, exprLabelIndex );
			pAsmBuffer.PutLine();
		};
		EmitStatementLabel( followLabelIndex );
	}
	public void EmitIF()
	{
		int trueLabelIndex = ++asmLabelIndex;
		int falseLabelIndex = ++asmLabelIndex;
    
		GetToken();
		GetLocationMarker(); // ignored
    
		//--<expr>
		GetToken();
		EmitExpression();
    
		{
			Operator( TInstruction.Cmp );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			IntegerLit( 1 );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Je );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, trueLabelIndex );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Jmp );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, falseLabelIndex );
			pAsmBuffer.PutLine();
		};
    
		EmitStatementLabel( trueLabelIndex );
    
		StartComment( "THEN" );
		PutLine();
    
		//--THEN <stmt-1>
		GetToken();
		EmitStatement();
    
		if ( token == TTokenCode.TcELSE )
		{
		GetToken();
		GetLocationMarker(); // ignored
    
		int followLabelIndex = ++asmLabelIndex;
		{
			Operator( TInstruction.Jmp );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, followLabelIndex );
			pAsmBuffer.PutLine();
		};
    
		StartComment( "ELSE" );
		PutLine();
    
		EmitStatementLabel( falseLabelIndex );
    
		GetToken();
		EmitStatement();
    
		EmitStatementLabel( followLabelIndex );
		} else
		{
		EmitStatementLabel( falseLabelIndex );
		}
	}
	public void EmitFOR()
	{
		int testLabelIndex = ++asmLabelIndex;
		int stmtLabelIndex = ++asmLabelIndex;
		int terminateLabelIndex = ++asmLabelIndex;
    
		GetToken();
		GetLocationMarker(); // ignored
    
		//--Get pointers to the control variable and to its type object.
		GetToken();
		TSymtabNode pControlId = pNode;
		TType pControlType = pNode.pType;
    
		int charFlag = ( pControlType.Base() == pCharType );
    
		//-- <id> := <expr-1>
		EmitAssignment( pControlId );
    
		//--TO or DOWNTO
		int toFlag = token == TTokenCode.TcTO;
    
		EmitStatementLabel( testLabelIndex );
    
		//--<expr-2>
		GetToken();
		EmitExpression();
    
		if ( charFlag != 0 )
		{
			Operator( TInstruction.Cmp );
			pAsmBuffer.Put( '\t' );
			Byte( pControlId );
			pAsmBuffer.Put( ',' );
			Reg( TRegister.Al );
			pAsmBuffer.PutLine();
		} else
		{
			Operator( TInstruction.Cmp );
			pAsmBuffer.Put( '\t' );
			Word( pControlId );
			pAsmBuffer.Put( ',' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		}
		{
			Operator( toFlag != 0 ? TInstruction.Jle : TInstruction.Jge );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, stmtLabelIndex );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Jmp );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, terminateLabelIndex );
			pAsmBuffer.PutLine();
		};
    
		EmitStatementLabel( stmtLabelIndex );
    
		//--DO <stmt>
		GetToken();
		EmitStatement();
    
		{
			Operator( toFlag != 0 ? TInstruction.Incr : TInstruction.Decr );
			pAsmBuffer.Put( '\t' );
			charFlag != 0 ? Byte( pControlId ) : Word( pControlId );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Jmp );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, testLabelIndex );
			pAsmBuffer.PutLine();
		};
    
		EmitStatementLabel( terminateLabelIndex );
    
		{
			Operator( toFlag != 0 ? TInstruction.Decr : TInstruction.Incr );
			pAsmBuffer.Put( '\t' );
			charFlag != 0 ? Byte( pControlId ) : Word( pControlId );
			pAsmBuffer.PutLine();
		};
	}
	public void EmitCASE()
	{
		int i;
		int j;
		int followLabelIndex = ++asmLabelIndex;
    
	//C++ TO C# CONVERTER TODO TASK: C# does not allow declaring types within methods:
	//	struct TBranchEntry
	//	{
	//	int labelValue;
	//	int branchLocation;
	//	int labelIndex;
	//	}
		*pBranchTable;
    
		//--Get the locations of the token that follows the
		//--CASE statement and of the branch table.
		GetToken();
		int atFollow = GetLocationMarker();
		GetToken();
		int atBranchTable = GetLocationMarker();
    
		//--<epxr>
		GetToken();
		TType pExprType = EmitExpression();
    
		int labelValue;
		int branchLocation;
		int charFlag = pExprType.Base() == pCharType;
    
		//--Loop through the branch table in the icode
		//--to count the number of entries.
		int count = 0;
		GoTo( atBranchTable + 1 );
		for ( ;; )
		{
		GetCaseItem( labelValue, branchLocation );
		if ( branchLocation == 0 )
			break;
		else
			++count;
		}
    
		//--Make a copy of the branch table.
		pBranchTable = Arrays.InitializeWithDefaultInstances<TBranchEntry>( count );
		GoTo( atBranchTable + 1 );
		for ( i = 0; i < count; ++i )
		{
		GetCaseItem( labelValue, branchLocation );
		pBranchTable[i].labelValue = labelValue;
		pBranchTable[i].branchLocation = branchLocation;
		}
    
		//--Loop through the branch table copy to emit test code.
		for ( i = 0; i < count; ++i )
		{
		int testLabelIndex = ++asmLabelIndex;
		int branchLabelIndex = ++asmLabelIndex;
    
		{
			Operator( TInstruction.Cmp );
			pAsmBuffer.Put( '\t' );
			charFlag != 0 ? Reg( TRegister.Al ) : Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			IntegerLit( pBranchTable[i].labelValue );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Jne );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, testLabelIndex );
			pAsmBuffer.PutLine();
		};
    
		//--See if the branch location is already in the branch table
		//--copy. If so, reuse the branch label index.
		for ( j = 0; j < i; ++j )
		{
			if ( pBranchTable[j].branchLocation == pBranchTable[i].branchLocation )
			{
			branchLabelIndex = pBranchTable[j].labelIndex;
			break;
			}
		}
    
		{
			Operator( TInstruction.Jmp );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, branchLabelIndex );
			pAsmBuffer.PutLine();
		};
		EmitStatementLabel( testLabelIndex );
    
		//--Enter the branch label index into the branch table copy
		//--only if it is new; otherwise, enter 0.
		pBranchTable[i].labelIndex = j < i != 0 ? 0 : branchLabelIndex;
		}
		{
			Operator( TInstruction.Jmp );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, followLabelIndex );
			pAsmBuffer.PutLine();
		};
    
		//--Loop through the branch table copy again to emit
		//--branch statement code that hasn't already been emitted.
		for ( i = 0; i < count; ++i )
		{
			if ( pBranchTable[i].labelIndex )
			{
		GoTo( pBranchTable[i].branchLocation );
		EmitStatementLabel( pBranchTable[i].labelIndex );
    
		GetToken();
		EmitStatement();
		{
			Operator( TInstruction.Jmp );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, followLabelIndex );
			pAsmBuffer.PutLine();
		};
			}
		}
    
		pBranchTable = null;
    
		GoTo( atFollow );
		GetToken();
    
		StartComment( "END" );
		PutLine();
    
		EmitStatementLabel( followLabelIndex );
	}
	public void EmitCompound()
	{
		StartComment( "BEGIN" );
		PutLine();
    
		//--<stmt-list> END
		GetToken();
		EmitStatementList( TTokenCode.TcEND );
    
		GetToken();
    
		StartComment( "END" );
		PutLine();
	}

	//--Expressions
	public TType * EmitExpression()
	{
		TType pOperand1Type; // ptr to first  operand's type
		TType pOperand2Type; // ptr to second operand's type
		TType pResultType; // ptr to result type
		TTokenCode op; // operator
		TInstruction jumpOpcode; // jump instruction opcode
		int jumpLabelIndex; // assembly jump label index
    
		//--Emit code for the first simple expression.
		pResultType = EmitSimpleExpression();
    
		//--If we now see a relational operator,
		//--emit code for the second simple expression.
		if ( TokenIn( token, tlRelOps ) != 0 )
		{
		EmitPushOperand( pResultType );
		op = token;
		pOperand1Type = pResultType.Base();
    
		GetToken();
		pOperand2Type = EmitSimpleExpression().Base();
    
		//--Perform the operation, and push the resulting value
		//--onto the stack.
		if ( ( ( pOperand1Type == pIntegerType ) && ( pOperand2Type == pIntegerType ) ) || ( ( pOperand1Type == pCharType ) && ( pOperand2Type == pCharType ) ) || ( pOperand1Type.form == TFormCode.FcEnum ) )
		{
    
			//--integer <op> integer
			//--boolean <op> boolean
			//--char    <op> char
			//--enum    <op> enum
			//--Compare dx (operand 1) to ax (operand 2).
			{
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Cmp );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
		} else if ( ( pOperand1Type == pRealType ) || ( pOperand2Type == pRealType ) )
		{
    
			//--real    <op> real
			//--real    <op> integer
			//--integer <op> real
			//--Convert the integer operand to real.
			//--Call _FloatCompare to do the comparison, which
			//--returns -1 (less), 0 (equal), or +1 (greater).
			EmitPushOperand( pOperand2Type );
			EmitPromoteToReal( pOperand1Type, pOperand2Type );
    
			{
				Operator( TInstruction.Call );
				pAsmBuffer.Put( '\t' );
				NameLit( DefineConstants.FloatCompare );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Add );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Sp );
				pAsmBuffer.Put( ',' );
				IntegerLit( 8 );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Cmp );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				IntegerLit( 0 );
				pAsmBuffer.PutLine();
			};
		} else
		{
    
			//--string <op> string
			//--Compare the string pointed to by si (operand 1)
			//--to the string pointed to by di (operand 2).
			{
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Di );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Si );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ds );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Es );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Cld );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Cx );
				pAsmBuffer.Put( ',' );
				IntegerLit( pOperand1Type.array.elmtCount );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.RepeCmpsb );
				pAsmBuffer.PutLine();
			};
		}
    
		{
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			IntegerLit( 1 );
			pAsmBuffer.PutLine();
		}; // default: load 1
    
		switch ( op )
		{
			case TTokenCode.TcLt:
				jumpOpcode = TInstruction.Jl;
				break;
			case TTokenCode.TcLe:
				jumpOpcode = TInstruction.Jle;
				break;
			case TTokenCode.TcEqual:
				jumpOpcode = TInstruction.Je;
				break;
			case TTokenCode.TcNe:
				jumpOpcode = TInstruction.Jne;
				break;
			case TTokenCode.TcGe:
				jumpOpcode = TInstruction.Jge;
				break;
			case TTokenCode.TcGt:
				jumpOpcode = TInstruction.Jg;
				break;
		}
    
		jumpLabelIndex = ++asmLabelIndex;
		{
			Operator( jumpOpcode );
			pAsmBuffer.Put( '\t' );
			Label( DefineConstants.StmtLabelPrefix, jumpLabelIndex );
			pAsmBuffer.PutLine();
		};
    
		{
			Operator( TInstruction.Sub );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		}; // load 0 if false
		EmitStatementLabel( jumpLabelIndex );
    
		pResultType = pBooleanType;
		}
    
		return pResultType;
	}
	public TType * EmitSimpleExpression()
	{
		TType pOperandType; // ptr to operand's type
		TType pResultType; // ptr to result type
		TTokenCode op; // operator
		TTokenCode unaryOp = TTokenCode.TcPlus; // unary operator
    
		//--Unary + or -
		if ( TokenIn( token, tlUnaryOps ) != 0 )
		{
		unaryOp = token;
		GetToken();
		}
    
		//--Emit code for the first term.
		pResultType = EmitTerm();
    
		//--If there was a unary operator, negate in integer value in ax
		//--with the neg instruction, or negate a real value in dx:ax
		//--by calling _FloatNegate.
		if ( unaryOp == TTokenCode.TcMinus )
		{
		if ( pResultType.Base() == pIntegerType )
		{
			Operator( TInstruction.Neg );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		} else if ( pResultType == pRealType )
		{
			EmitPushOperand( pResultType );
			{
				Operator( TInstruction.Call );
				pAsmBuffer.Put( '\t' );
				NameLit( DefineConstants.FloatNegate );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Add );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Sp );
				pAsmBuffer.Put( ',' );
				IntegerLit( 4 );
				pAsmBuffer.PutLine();
			};
		}
		}
    
		//--Loop to execute subsequent additive operators and terms.
		while ( TokenIn( token, tlAddOps ) != 0 )
		{
		op = token;
		pResultType = pResultType.Base();
		EmitPushOperand( pResultType );
    
		GetToken();
		pOperandType = EmitTerm().Base();
    
		//--Perform the operation, and push the resulting value
		//--onto the stack.
		if ( op == TTokenCode.TcOR )
		{
    
			//--boolean OR boolean => boolean
			//--ax = ax OR dx
			{
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Or );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
			pResultType = pBooleanType;
		} else if ( ( pResultType == pIntegerType ) && ( pOperandType == pIntegerType ) )
		{
    
			{
			//--integer +|- integer => integer
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
			if ( op == TTokenCode.TcPlus )
			{
				Operator( TInstruction.Add );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			} else
			{
			{
				Operator( TInstruction.Sub );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
			}
			pResultType = pIntegerType;
		} else
		{
    
			//--real    +|- real    => real
			//--real    +|- integer => real
			//--integer +|- real    => real
			//--Convert the integer operand to real and then
			//--call _FloatAdd or _FloatSubtract.
			EmitPushOperand( pOperandType );
			EmitPromoteToReal( pResultType, pOperandType );
    
			{
				Operator( TInstruction.Call );
				pAsmBuffer.Put( '\t' );
				NameLit( ( int )op == ( ( int )TTokenCode.TcPlus ) != 0 ? DefineConstants.FloatAdd : DefineConstants.FloatSubtract );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Add );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Sp );
				pAsmBuffer.Put( ',' );
				IntegerLit( 8 );
				pAsmBuffer.PutLine();
			};
			pResultType = pRealType;
		}
		}
    
		return pResultType;
	}
	public TType * EmitTerm()
	{
		TType pOperandType; // ptr to operand's type
		TType pResultType; // ptr to result type
		TTokenCode op; // operator
    
		//--Emit code for the first factor.
		pResultType = EmitFactor();
    
		//--Loop to execute subsequent multiplicative operators and factors.
		while ( TokenIn( token, tlMulOps ) != 0 )
		{
		op = token;
		pResultType = pResultType.Base();
		EmitPushOperand( pResultType );
    
		GetToken();
		pOperandType = EmitFactor().Base();
    
		//--Perform the operation, and push the resulting value
		//--onto the stack.
		switch ( op )
		{
    
			case TTokenCode.TcAND:
			{
    
			{
			//--boolean AND boolean => boolean
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.And );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
			pResultType = pBooleanType;
			break;
			}
    
			case TTokenCode.TcStar:
    
			if ( ( pResultType == pIntegerType ) && ( pOperandType == pIntegerType ) )
			{
    
				//--integer * integer => integer
				//--ax = ax*dx
				{
					Operator( TInstruction.Pop );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Dx );
					pAsmBuffer.PutLine();
				};
				{
					Operator( TInstruction.Imul );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Dx );
					pAsmBuffer.PutLine();
				};
				pResultType = pIntegerType;
			} else
			{
    
				//--real    * real    => real
				//--real    * integer => real
				//--integer * real    => real
				//--Convert the integer operand to real
				//--and then call _FloatMultiply, which
				//--leaves the value in dx:ax.
				EmitPushOperand( pOperandType );
				EmitPromoteToReal( pResultType, pOperandType );
    
				{
					Operator( TInstruction.Call );
					pAsmBuffer.Put( '\t' );
					NameLit( DefineConstants.FloatMultiply );
					pAsmBuffer.PutLine();
				};
				{
					Operator( TInstruction.Add );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Sp );
					pAsmBuffer.Put( ',' );
					IntegerLit( 8 );
					pAsmBuffer.PutLine();
				};
				pResultType = pRealType;
			}
			break;
    
			case TTokenCode.TcSlash:
			{
    
			//--real    / real    => real
			//--real    / integer => real
			//--integer / real    => real
			//--integer / integer => real
			//--Convert any integer operand to real
			//--and then call _FloatDivide, which
				//--leaves the value in dx:ax.
			EmitPushOperand( pOperandType );
			EmitPromoteToReal( pResultType, pOperandType );
    
			{
				Operator( TInstruction.Call );
				pAsmBuffer.Put( '\t' );
				NameLit( DefineConstants.FloatDivide );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Add );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Sp );
				pAsmBuffer.Put( ',' );
				IntegerLit( 8 );
				pAsmBuffer.PutLine();
			};
			pResultType = pRealType;
			break;
			}
    
			case TTokenCode.TcDIV:
			case TTokenCode.TcMOD:
			{
    
			//--integer DIV|MOD integer => integer
			//--ax = ax IDIV cx
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Cx );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Sub );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Idiv );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Cx );
				pAsmBuffer.PutLine();
			};
			if ( op == TTokenCode.TcMOD )
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
			pResultType = pIntegerType;
			break;
			}
		}
		}
    
		return pResultType;
	}
	public TType * EmitFactor()
	{
		TType pResultType; // ptr to result type
    
		switch ( token )
		{
    
		case TTokenCode.TcIdentifier:
		{
			switch ( pNode.defn.how )
			{
    
			case TDefnCode.DcFunction:
				pResultType = EmitSubroutineCall( pNode );
				break;
    
			case TDefnCode.DcConstant:
				pResultType = EmitConstant( pNode );
				break;
    
			default:
				pResultType = EmitVariable( pNode, false );
				break;
			}
			break;
		}
    
		case TTokenCode.TcNumber:
		{
    
			//--Push the number's integer or real value onto the stack.
			if ( pNode.pType == pIntegerType )
			{
    
			{
			//--ax = value
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				IntegerLit( pNode.defn.constant.value.integer );
				pAsmBuffer.PutLine();
			};
			pResultType = pIntegerType;
			} else
			{
    
			//--dx:ax = value
			EmitLoadFloatLit( pNode );
			pResultType = pRealType;
			}
    
			GetToken();
			break;
		}
    
		case TTokenCode.TcString:
		{
    
			//--Push either a character or a string address onto the
			//--runtime stack, depending on the string length.
			int length = Convert.ToString( pNode.String() ).Length - 2; // skip quotes
			if ( length == 1 )
			{
    
			//--Character
			//--ah = 0
			//--al = value
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				CharLit( pNode.defn.constant.value.character );
				pAsmBuffer.PutLine();
			};
			pResultType = pCharType;
			} else
			{
    
			//--String
			//--ax = string address
			EmitPushStringLit( pNode );
			pResultType = pNode.pType;
			}
    
			GetToken();
			break;
		}
    
		case TTokenCode.TcNOT:
    
			//--Emit code for boolean factor and invert its value.
			//--ax = NOT ax
			GetToken();
			EmitFactor();
			{
				Operator( TInstruction.Xor );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				IntegerLit( 1 );
				pAsmBuffer.PutLine();
			};
			pResultType = pBooleanType;
			break;
    
		case TTokenCode.TcLParen:
		{
    
			//--Parenthesized subexpression:  Call EmitExpression
			//--                              recursively.
			GetToken(); // first token after (
			pResultType = EmitExpression();
			GetToken(); // first token after )
			break;
		}
		}
    
		return pResultType;
	}
	public TType * EmitConstant( TSymtabNode pId )
	{
		TType pType = pId.pType;
    
		if ( pType == pRealType )
    
		//--Real: dx:ax = value
		EmitLoadFloatLit( pId );
		else if ( pType == pCharType )
		{
    
		{
		//--Character: ax = value
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			CharLit( pId.defn.constant.value.character );
			pAsmBuffer.PutLine();
		};
		} else if ( pType.form == TFormCode.FcArray )
		{
    
		//--String constant: push string address
		EmitPushStringLit( pId );
		} else
		{
    
		{
		//--Integer or enumeration: ax = value
			Operator( TInstruction.Mov );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			IntegerLit( pId.defn.constant.value.integer );
			pAsmBuffer.PutLine();
		};
		}
    
		GetToken();
		return pType;
	}
	public TType * EmitVariable( TSymtabNode pId, int addressFlag )
	{
		TType pType = pId.pType;
    
		//--It's not a scalar, or addressFlag is true, push the
		//--data address onto the stack. Otherwise, load the
		//--data value into ax or dx:ax.
		if ( addressFlag != 0 || ( pType.IsScalar() == 0 ) )
			EmitPushAddress( pId );
		else
			EmitLoadValue( pId );
    
		GetToken();
    
		//--If there are any subscripts and field designators,
		//--emit code to evaluate them and modify the address.
		if ( ( token == TTokenCode.TcLBracket ) || ( token == TTokenCode.TcPeriod ) )
		{
		int doneFlag = false;
    
		do
		{
			switch ( token )
			{
    
			case TTokenCode.TcLBracket:
				pType = EmitSubscripts( pType );
				break;
    
			case TTokenCode.TcPeriod:
				pType = EmitField();
				break;
    
			default:
				doneFlag = true;
				break;
			}
		} while ( doneFlag == 0 );
    
		//--If addresssFlag is false and the variable is scalar,
		//--pop the address off the top of the stack and use it
		//--to load the value into ax or dx:ax.
		if ( ( addressFlag == 0 ) && ( pType.IsScalar() ) != 0 )
		{
			{
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Bx );
				pAsmBuffer.PutLine();
			};
			if ( pType == pRealType )
			{
    
			{
			//--Read: dx:ax = value
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				WordIndirect( TRegister.Bx );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.Put( ',' );
				HighDWordIndirect( TRegister.Bx );
				pAsmBuffer.PutLine();
			};
			} else if ( pType.Base() == pCharType )
			{
    
			{
			//--Character: al = value
				Operator( TInstruction.Sub );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Al );
				pAsmBuffer.Put( ',' );
				ByteIndirect( TRegister.Bx );
				pAsmBuffer.PutLine();
			};
			} else
			{
    
			{
			//--Integer or enumeration: ax = value
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Ax );
				pAsmBuffer.Put( ',' );
				WordIndirect( TRegister.Bx );
				pAsmBuffer.PutLine();
			};
			}
		}
		}
    
		return pType;
	}
	public TType * EmitSubscripts( TType pType )
	{
		int minIndex;
		int elmtSize;
    
		//--Loop to executed subscript lists enclosed in brackets.
		while ( token == TTokenCode.TcLBracket )
		{
    
		//--Loop to execute comma-separated subscript expressions
		//--within a subscript list.
		do
		{
			GetToken();
			EmitExpression();
    
			minIndex = pType.array.minIndex;
			elmtSize = pType.array.pElmtType.size;
    
			//--Convert the subscript into an offset by subracting
			//--the mininum index from it and then multiplying the
			//--result by the element size.   Add the offset to the
			//--address at the top of the stack.
			if ( minIndex != 0 )
			{
					Operator( TInstruction.Sub );
					pAsmBuffer.Put( '\t' );
					Reg( TRegister.Ax );
					pAsmBuffer.Put( ',' );
					IntegerLit( minIndex );
					pAsmBuffer.PutLine();
			};
			if ( elmtSize > 1 )
			{
			{
				Operator( TInstruction.Mov );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.Put( ',' );
				IntegerLit( elmtSize );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Imul );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
			}
			{
				Operator( TInstruction.Pop );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Add );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.Put( ',' );
				Reg( TRegister.Ax );
				pAsmBuffer.PutLine();
			};
			{
				Operator( TInstruction.Push );
				pAsmBuffer.Put( '\t' );
				Reg( TRegister.Dx );
				pAsmBuffer.PutLine();
			};
    
			//--Prepare for another subscript in this list.
			if ( token == TTokenCode.TcComma )
				pType = pType.array.pElmtType;
    
		} while ( token == TTokenCode.TcComma );
    
		//--Prepare for another subscript list.
		GetToken();
		if ( token == TTokenCode.TcLBracket )
			pType = pType.array.pElmtType;
		}
    
		return pType.array.pElmtType;
	}
	public TType * EmitField()
	{
		GetToken();
		TSymtabNode pFieldId = pNode;
		int offset = pFieldId.defn.data.offset;
    
		//--Add the field's offset to the data address
		//--if the offset is greater than 0.
		if ( offset > 0 )
		{
		{
			Operator( TInstruction.Pop );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Add );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.Put( ',' );
			IntegerLit( offset );
			pAsmBuffer.PutLine();
		};
		{
			Operator( TInstruction.Push );
			pAsmBuffer.Put( '\t' );
			Reg( TRegister.Ax );
			pAsmBuffer.PutLine();
		};
		}
    
		GetToken();
		return pFieldId.pType;
	}

	//--Assembly buffer
	private string AsmText()
	{
		return pAsmBuffer.Text();
	}
	private void Reset()
	{
		pAsmBuffer.Reset();
	}
	private void Put( char ch )
	{
		pAsmBuffer.Put( ch );
	}
	private void Put( ref string pString )
	{
		pAsmBuffer.Put( pString );
	}
	private void PutLine()
	{
		pAsmBuffer.PutLine();
	}
	private void PutLine( ref string pText )
	{
		pAsmBuffer.PutLine( pText );
	}
	private void Advance()
	{
		pAsmBuffer.Advance();
	}

	//--Comments
	public void PutComment( string pString )
	{
		int length = pString.Length;
    
		//--Start a new comment if the current one is full.
		if ( !pAsmBuffer.Fit( length ) )
		{
		PutLine();
		StartComment();
		}
    
		AsmText() = pString;
		Advance();
	}
	public void StartComment( int n )
	{
		Reset();
		AsmText() = string.Format("; {{{0:D}}} ", n);
		Advance();
	}

	private void StartComment()
	{
		Reset();
		PutComment( "; " );
	}

	private void StartComment( string pString )
	{
	StartComment();
	PutComment( pString );
	}

	public void EmitProgramHeaderComment( TSymtabNode pProgramId )
	{
		PutLine();
		StartComment( "PROGRAM " );
		PutComment( pProgramId.String() ); // program name
    
		//--Emit the program's parameter list.
		TSymtabNode pParmId = pProgramId.defn.routine.locals.pParmIds;
		if ( pParmId != null )
		{
		PutComment( " (" );
    
		//--Loop to emit each parameter.
		do
		{
			PutComment( pParmId.String() );
			pParmId = pParmId.next;
			if ( pParmId != null )
				PutComment( ", " );
		} while ( pParmId != null );
    
		PutComment( ")" );
		}
    
		PutLine();
	}
	public void EmitSubroutineHeaderComment( TSymtabNode pRoutineId )
	{
		PutLine();
		StartComment( pRoutineId.defn.how == ( ( int )TDefnCode.DcProcedure ) != 0 ? "PROCEDURE " : "FUNCTION " );
		//--Emit the procedure or function name
		//--followed by the formal parameter list.
		PutComment( pRoutineId.String() );
		EmitSubroutineFormalsComment( pRoutineId.defn.routine.locals.pParmIds );
    
		//--Emit a function's return type.
		if ( pRoutineId.defn.how == TDefnCode.DcFunction )
		{
		PutComment( " : " );
		PutComment( pRoutineId.pType.pTypeId.String() );
		}
    
		PutLine();
	}
	public void EmitSubroutineFormalsComment( TSymtabNode pParmId )
	{
		if ( pParmId == null )
			return;
    
		PutComment( " (" );
    
		//--Loop to emit each sublist of parameters with
		//--common definition and type.
		do
		{
		TDefnCode commonDefn = pParmId.defn.how; // common defn
		TType pCommonType = pParmId.pType; // common type
		int doneFlag; // true if sublist done, false if not
    
		if ( commonDefn == TDefnCode.DcVarParm )
			PutComment( "VAR " );
    
		//--Loop to emit the parms in the sublist.
		do
		{
			PutComment( pParmId.String() );
    
			pParmId = pParmId.next;
			doneFlag = ( pParmId == null ) || ( commonDefn != pParmId.defn.how ) || ( pCommonType != pParmId.pType );
			if ( doneFlag == 0 )
				PutComment( ", " );
		} while ( doneFlag == 0 );
    
		//--Print the sublist's common type.
		PutComment( " : " );
		PutComment( pCommonType.pTypeId.String() );
    
		if ( pParmId != null )
			PutComment( "; " );
		} while ( pParmId != null );
    
		PutComment( ")" );
	}
	public void EmitVarDeclComment( TSymtabNode pVarId )
	{
		TType pCommonType; // ptr to common type
    
		if ( pVarId == null )
			return;
		pCommonType = pVarId.pType;
    
		PutLine();
		StartComment( "VAR" );
		PutLine();
		StartComment();
    
		//--Loop to print sublists of variables with a common type.
		do
		{
		PutComment( pVarId.String() );
		pVarId = pVarId.next;
    
		if ( pVarId != null && ( pVarId.pType == pCommonType ) )
			PutComment( ", " );
		else
		{
    
			//--End of sublist:  Print the common type and begin
			//--                 a new sublist.
			PutComment( " : " );
			EmitTypeSpecComment( pCommonType );
			PutLine();
    
			if ( pVarId != null )
			{
			pCommonType = pVarId.pType;
			StartComment();
			}
		}
		} while ( pVarId != null );
	}
	public void EmitTypeSpecComment( TType pType )
	{
		//--If named type, emit the name, else emit "..."
		PutComment( pType.pTypeId != null ? pType.pTypeId.String() : "..." );
	}
	public void EmitStmtComment()
	{
		SaveState(); // save icode state
		StartComment( currentLineNumber );
    
		switch ( token )
		{
		case TTokenCode.TcIdentifier:
			EmitAsgnOrCallComment();
			break;
		case TTokenCode.TcREPEAT:
			EmitREPEATComment();
			break;
		case TTokenCode.TcUNTIL:
			EmitUNTILComment();
			break;
		case TTokenCode.TcWHILE:
			EmitWHILEComment();
			break;
		case TTokenCode.TcIF:
			EmitIFComment();
			break;
		case TTokenCode.TcFOR:
			EmitFORComment();
			break;
		case TTokenCode.TcCASE:
			EmitCASEComment();
			break;
		}
    
		RestoreState(); // restore icode state
	}
	public void EmitAsgnOrCallComment()
	{
		EmitIdComment();
    
		if ( token == TTokenCode.TcColonEqual )
		{
		PutComment( " := " );
    
		GetToken();
		EmitExprComment();
		}
    
		PutLine();
	}
	public void EmitREPEATComment()
	{
		PutComment( "REPEAT" );
		PutLine();
	}
	public void EmitUNTILComment()
	{
		PutComment( "UNTIL " );
    
		GetToken();
		EmitExprComment();
    
		PutLine();
	}
	public void EmitWHILEComment()
	{
		PutComment( "WHILE " );
    
		GetToken();
		GetLocationMarker();
    
		GetToken();
		EmitExprComment();
    
		PutComment( " DO" );
		PutLine();
	}
	public void EmitIFComment()
	{
		PutComment( "IF " );
    
		GetToken();
		GetLocationMarker();
    
		GetToken();
		EmitExprComment();
    
		PutLine();
	}
	public void EmitFORComment()
	{
		PutComment( "FOR " );
    
		GetToken();
		GetLocationMarker();
    
		GetToken();
		EmitIdComment();
		PutComment( " := " );
    
		GetToken();
		EmitExprComment();
		PutComment( token == ( ( int )TTokenCode.TcTO ) != 0 ? " TO " : " DOWNTO " );
    
		GetToken();
		EmitExprComment();
    
		PutComment( " DO" );
		PutLine();
	}
	public void EmitCASEComment()
	{
		PutComment( "CASE " );
    
		GetToken();
		GetLocationMarker();
		GetToken();
		GetLocationMarker();
    
		GetToken();
		EmitExprComment();
    
		PutComment( " OF " );
		PutLine();
	}
	public void EmitExprComment()
	{
		int doneFlag = false; // true if done with expression, false if not
    
		//--Loop over the entire expression.
		do
		{
		switch ( token )
		{
			case TTokenCode.TcIdentifier:
				EmitIdComment();
				break;
    
			case TTokenCode.TcNumber:
				PutComment( pToken.String() );
				GetToken();
					break;
    
			case TTokenCode.TcString:
				PutComment( pToken.String() );
				GetToken();
					break;
    
			case TTokenCode.TcPlus:
				PutComment( " + " );
				GetToken();
				break;
			case TTokenCode.TcMinus:
				PutComment( " - " );
				GetToken();
				break;
			case TTokenCode.TcStar:
				PutComment( "*" );
				GetToken();
				break;
			case TTokenCode.TcSlash:
				PutComment( "/" );
				GetToken();
				break;
			case TTokenCode.TcDIV:
				PutComment( " DIV " );
				GetToken();
				break;
			case TTokenCode.TcMOD:
				PutComment( " MOD " );
				GetToken();
				break;
			case TTokenCode.TcAND:
				PutComment( " AND " );
				GetToken();
				break;
			case TTokenCode.TcOR:
				PutComment( " OR " );
				GetToken();
				break;
			case TTokenCode.TcEqual:
				PutComment( " = " );
				GetToken();
				break;
			case TTokenCode.TcNe:
				PutComment( " <> " );
				GetToken();
				break;
			case TTokenCode.TcLt:
				PutComment( " < " );
				GetToken();
				break;
			case TTokenCode.TcLe:
				PutComment( " <= " );
				GetToken();
				break;
			case TTokenCode.TcGt:
				PutComment( " > " );
				GetToken();
				break;
			case TTokenCode.TcGe:
				PutComment( " >= " );
				GetToken();
				break;
			case TTokenCode.TcNOT:
				PutComment( "NOT " );
				GetToken();
				break;
    
			case TTokenCode.TcLParen:
			PutComment( "(" );
			GetToken();
			EmitExprComment();
			PutComment( ")" );
			GetToken();
			break;
    
			default:
			doneFlag = true;
			break;
		}
		} while ( doneFlag == 0 );
	}
	public void EmitIdComment()
	{
		PutComment( pToken.String() );
		GetToken();
    
		//--Loop to print any modifiers (subscripts, record fields,
		//--or actual parameter lists).
		while ( TokenIn( token, tlIdModStart ) != 0 )
		{
    
		//--Record field.
		if ( token == TTokenCode.TcPeriod )
		{
			PutComment( "." );
			GetToken();
			EmitIdComment();
		}
    
		//--Subscripts or actual parameters.
		else
		{
    
			//--( or [
			PutComment( token == ( ( int )TTokenCode.TcLParen ) != 0 ? "(" : "[" );
			GetToken();
    
			while ( TokenIn( token, tlIdModEnd ) == 0 )
			{
			EmitExprComment();
    
			//--Write and writeln field width and precision.
			while ( token == TTokenCode.TcColon )
			{
				PutComment( ":" );
				GetToken();
				EmitExprComment();
			}
    
			if ( token == TTokenCode.TcComma )
			{
				PutComment( ", " );
				GetToken();
			}
			}
    
			//--) or ]
			PutComment( token == ( ( int )TTokenCode.TcRParen ) != 0 ? ")" : "]" );
			GetToken();
		}
		}
	}

	public TCodeGenerator( string pAsmName )
	{
		this.pAsmBuffer = new TAssemblyBuffer( pAsmName, TAbortCode.AbortAssemblyFileOpenFailed );
	pFloatLitList = pStringLitList = null;
	}

	public void Go( TSymtabNode pProgramId )
	{
		EmitProgramPrologue();
    
		//--Emit code for the program.
		currentNestingLevel = 1;
		EmitMain( pProgramId );
    
		EmitProgramEpilogue( pProgramId );
	}
}

//endfig
