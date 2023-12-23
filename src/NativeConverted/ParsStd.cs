//  *************************************************************
//  *                                                           *
//  *   P A R S E R   (Standard Routines)                       *
//  *                                                           *
//  *   Parse calls to standard procedures and functions.       *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog8-1/parsstd.cpp                            *
//  *                                                           *
//  *   MODULE:  Parser                                         *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************
partial class TParser
{
    //--------------------------------------------------------------
    //  ParseStandardSubroutineCall     Parse a call to a standard
    //                                  procedure or function.
    //
    //      pRoutineId : ptr to the subroutine id's
    //                   symbol table node
    //
    //  Return: ptr to the type object of the call
    //--------------------------------------------------------------
    public TType ParseStandardSubroutineCall ( TSymtabNode pRoutineId )
    {
        switch (pRoutineId.defn.routine.which)
        {

            case TRoutineCode.RcRead:
            case TRoutineCode.RcReadln:
            return ParseReadReadlnCall(pRoutineId);

            case TRoutineCode.RcWrite:
            case TRoutineCode.RcWriteln:
            return ParseWriteWritelnCall(pRoutineId);

            case TRoutineCode.RcEof:
            case TRoutineCode.RcEoln:
            return ParseEofEolnCall();

            case TRoutineCode.RcAbs:
            case TRoutineCode.RcSqr:
            return ParseAbsSqrCall();

            case TRoutineCode.RcArctan:
            case TRoutineCode.RcCos:
            case TRoutineCode.RcExp:
            case TRoutineCode.RcLn:
            case TRoutineCode.RcSin:
            case TRoutineCode.RcSqrt:
            return ParseArctanCosExpLnSinSqrtCall();

            case TRoutineCode.RcPred:
            case TRoutineCode.RcSucc:
            return ParsePredSuccCall();

            case TRoutineCode.RcChr:
            return ParseChrCall();
            case TRoutineCode.RcOdd:
            return ParseOddCall();
            case TRoutineCode.RcOrd:
            return ParseOrdCall();

            case TRoutineCode.RcRound:
            case TRoutineCode.RcTrunc:
            return ParseRoundTruncCall();

            default:
            return Globals.pDummyType;
        }
    }

    //--------------------------------------------------------------
    //  ParseReadReadlnCall     Parse a call to read or readln.
    //                          Each actual parameter must be a
    //                          scalar variable.
    //
    //      pRoutineId : ptr to the routine id's symbol table node
    //
    //  Return: ptr to the dummy type object
    //--------------------------------------------------------------
    public TType ParseReadReadlnCall ( TSymtabNode pRoutineId )
    {
        //--Actual parameters are optional for readln.
        if (token != TTokenCode.TcLParen)
        {
            if (pRoutineId.defn.routine.which == TRoutineCode.RcRead)
                Globals.Error(TErrorCode.ErrWrongNumberOfParms);
            return Globals.pDummyType;
        }

        //--Loop to parse comma-separated list of actual parameters.
        do
        {
            //-- ( or ,
            GetTokenAppend();

            //--Each actual parameter must be a scalar variable,
            //--but parse an expression anyway for error recovery.
            if (token == TTokenCode.TcIdentifier)
            {
                TSymtabNode pParmId = Find(pToken.String);
                icode.Put(pParmId);

                if (ParseVariable(pParmId).Base().form != TFormCode.FcScalar)
                    Globals.Error(TErrorCode.ErrIncompatibleTypes);
            } else
            {
                ParseExpression();
                Globals.Error(TErrorCode.ErrInvalidVarParm);
            }

            //-- , or )
            Resync(tlActualVarParmFollow, tlStatementFollow, tlStatementStart);
        } while (token == TTokenCode.TcComma);

        //-- )
        CondGetTokenAppend(TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen);

        return Globals.pDummyType;
    }

    //--------------------------------------------------------------
    //  ParseWriteWritelnCall   Parse a call to write or writeln.
    //                          Each actual parameter can be in any
    //                          one of the following forms:
    //
    //                              <expr>
    //                              <expr> : <expr>
    //                              <expr> : <expr> : <expr>
    //
    //      pRoutineId : ptr to the routine id's symbol table node
    //
    //  Return: ptr to the dummy type object
    //--------------------------------------------------------------
    public TType ParseWriteWritelnCall ( TSymtabNode pRoutineId )
    {
        //--Actual parameters are optional only for writeln.
        if (token != TTokenCode.TcLParen)
        {
            if (pRoutineId.defn.routine.which == TRoutineCode.RcWrite)
                Globals.Error(TErrorCode.ErrWrongNumberOfParms);
            return Globals.pDummyType;
        }

        //--Loop to parse comma-separated list of actual parameters.
        do
        {
            //-- ( or ,
            GetTokenAppend();

            //--Value <expr> : The type must be either a non-Boolean
            //--               scalar or a string.
            TType pActualType = ParseExpression().Base();
            if (((pActualType.form != TFormCode.FcScalar) || (pActualType == Globals.pBooleanType)) && ((pActualType.form != TFormCode.FcArray) || (pActualType.array.pElmtType != Globals.pCharType)))
                Globals.Error(TErrorCode.ErrIncompatibleTypes);

            //--Optional field width <expr>
            if (token == TTokenCode.TcColon)
            {
                GetTokenAppend();
                if (ParseExpression().Base() != Globals.pIntegerType)
                    Globals.Error(TErrorCode.ErrIncompatibleTypes);

                //--Optional precision <expr>
                if (token == TTokenCode.TcColon)
                {
                    GetTokenAppend();
                    if (ParseExpression().Base() != Globals.pIntegerType)
                        Globals.Error(TErrorCode.ErrIncompatibleTypes);
                }
            }
        } while (token == TTokenCode.TcComma);

        //-- )
        CondGetTokenAppend(TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen);

        return Globals.pDummyType;
    }

    //--------------------------------------------------------------
    //  ParseEofEolnCall    Parse a call to eof or eoln.
    //                      No parameters => boolean result
    //
    //  Return: ptr to the boolean type object
    //--------------------------------------------------------------
    public TType ParseEofEolnCall ()
    {
        //--There should be no actual parameters, but parse
        //--them anyway for error recovery.
        if (token == TTokenCode.TcLParen)
        {
            Globals.Error(TErrorCode.ErrWrongNumberOfParms);
            ParseActualParmList(null, false);
        }

        return Globals.pBooleanType;
    }

    //--------------------------------------------------------------
    //  ParseAbsSqrCall     Parse a call to abs or sqr.
    //                      Integer parm => integer result
    //                      Real parm    => real result
    //
    //  Return: ptr to the result's type object
    //--------------------------------------------------------------
    public TType ParseAbsSqrCall ()
    {
        TType pResultType = null; // ptr to result type object

        //--There should be one integer or real parameter.
        if (token == TTokenCode.TcLParen)
        {
            GetTokenAppend();

            TType pParmType = ParseExpression().Base();
            if ((pParmType != Globals.pIntegerType) && (pParmType != Globals.pRealType))
            {
                Globals.Error(TErrorCode.ErrIncompatibleTypes);
                pResultType = Globals.pIntegerType;
            } else
            {
                pResultType = pParmType;
            }

            //--There better not be any more parameters.
            if (token != TTokenCode.TcRParen)
                SkipExtraParms();

            //-- )
            CondGetTokenAppend(TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen);
        } else
        {
            Globals.Error(TErrorCode.ErrWrongNumberOfParms);
        }

        return pResultType;
    }

    //--------------------------------------------------------------
    //  ParseArctanCosExpLnSinSqrtCall  Parse a call to arctan, cos,
    //                                  exp, ln, sin, or sqrt.
    //                                  Integer parm => real result
    //                                  Real parm    => real result
    //
    //  Return: ptr to the real type object
    //--------------------------------------------------------------
    public TType ParseArctanCosExpLnSinSqrtCall ()
    {
        //--There should be one integer or real parameter.
        if (token == TTokenCode.TcLParen)
        {
            GetTokenAppend();

            TType pParmType = ParseExpression().Base();
            if ((pParmType != Globals.pIntegerType) && (pParmType != Globals.pRealType))
                Globals.Error(TErrorCode.ErrIncompatibleTypes);

            //--There better not be any more parameters.
            if (token != TTokenCode.TcRParen)
                SkipExtraParms();

            //-- )
            CondGetTokenAppend(TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen);
        } else
        {
            Globals.Error(TErrorCode.ErrWrongNumberOfParms);
        }

        return Globals.pRealType;
    }

    //--------------------------------------------------------------
    //  ParsePredSuccCall   Parse a call to pred or succ.
    //                      Integer parm => integer result
    //                      Enum parm    => enum result
    //
    //  Return: ptr to the result's type object
    //--------------------------------------------------------------
    public TType ParsePredSuccCall ()
    {
        TType pResultType = null; // ptr to result type object

        //--There should be one integer or enumeration parameter.
        if (token == TTokenCode.TcLParen)
        {
            GetTokenAppend();

            TType pParmType = ParseExpression().Base();
            if ((pParmType != Globals.pIntegerType) && (pParmType.form != TFormCode.FcEnum))
            {
                Globals.Error(TErrorCode.ErrIncompatibleTypes);
                pResultType = Globals.pIntegerType;
            } else
            {
                pResultType = pParmType;
            }

            //--There better not be any more parameters.
            if (token != TTokenCode.TcRParen)
                SkipExtraParms();

            //-- )
            CondGetTokenAppend(TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen);
        } else
        {
            Globals.Error(TErrorCode.ErrWrongNumberOfParms);
        }

        return pResultType;
    }

    //--------------------------------------------------------------
    //  ParseChrCall        Parse a call to chr.
    //                      Integer parm => character result
    //
    //  Return: ptr to the character type object
    //--------------------------------------------------------------
    public TType ParseChrCall ()
    {
        //--There should be one character parameter.
        if (token == TTokenCode.TcLParen)
        {
            GetTokenAppend();

            TType pParmType = ParseExpression().Base();
            if (pParmType != Globals.pIntegerType)
                Globals.Error(TErrorCode.ErrIncompatibleTypes);

            //--There better not be any more parameters.
            if (token != TTokenCode.TcRParen)
                SkipExtraParms();

            //-- )
            CondGetTokenAppend(TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen);
        } else
        {
            Globals.Error(TErrorCode.ErrWrongNumberOfParms);
        }

        return  Globals.pCharType;
    }

    //--------------------------------------------------------------
    //  ParseOddCall        Parse a call to odd.
    //                      Integer parm => boolean result
    //
    //  Return: ptr to the boolean type object
    //--------------------------------------------------------------
    public TType ParseOddCall ()
    {
        //--There should be one integer parameter.
        if (token == TTokenCode.TcLParen)
        {
            GetTokenAppend();

            TType pParmType = ParseExpression().Base();
            if (pParmType != Globals.pIntegerType)
                Globals.Error(TErrorCode.ErrIncompatibleTypes);

            //--There better not be any more parameters.
            if (token != TTokenCode.TcRParen)
                SkipExtraParms();

            //-- )
            CondGetTokenAppend(TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen);
        } else
        {
            Globals.Error(TErrorCode.ErrWrongNumberOfParms);
        }

        return Globals.pBooleanType;
    }

    //--------------------------------------------------------------
    //  ParseOrdCall        Parse a call to ord.
    //                      Character parm => integer result
    //                      Enum parm      => integer result
    //
    //  Return: ptr to the integer type object
    //--------------------------------------------------------------
    public TType ParseOrdCall ()
    {
        //--There should be one character or enumeration parameter.
        if (token == TTokenCode.TcLParen)
        {
            GetTokenAppend();

            TType pParmType = ParseExpression().Base();
            if ((pParmType != Globals.pCharType) && (pParmType.form != TFormCode.FcEnum))
                Globals.Error(TErrorCode.ErrIncompatibleTypes);

            //--There better not be any more parameters.
            if (token != TTokenCode.TcRParen)
                SkipExtraParms();

            //-- )
            CondGetTokenAppend(TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen);
        } else
        {
            Globals.Error(TErrorCode.ErrWrongNumberOfParms);
        }

        return Globals.pIntegerType;
    }

    //--------------------------------------------------------------
    //  ParseRoundTruncCall     Parse a call to round or trunc.
    //                          Real parm => integer result
    //
    //  Return: ptr to the integer type object
    //--------------------------------------------------------------
    public TType ParseRoundTruncCall ()
    {
        //--There should be one real parameter.
        if (token == TTokenCode.TcLParen)
        {
            GetTokenAppend();

            TType pParmType = ParseExpression().Base();
            if (pParmType != Globals.pRealType)
                Globals.Error(TErrorCode.ErrIncompatibleTypes);

            //--There better not be any more parameters.
            if (token != TTokenCode.TcRParen)
                SkipExtraParms();

            //-- )
            CondGetTokenAppend(TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen);
        } else
        {
            Globals.Error(TErrorCode.ErrWrongNumberOfParms);
        }

        return Globals.pIntegerType;
    }

    //--------------------------------------------------------------
    //  SkipExtraParms      Skip extra parameters in a call to a
    //                      standard procedure or function.
    //
    //      pSymtab : ptr to symbol table
    //--------------------------------------------------------------
    public void SkipExtraParms ()
    {
        Globals.Error(TErrorCode.ErrWrongNumberOfParms);

        while (token == TTokenCode.TcComma)
        {
            GetTokenAppend();
            ParseExpression();
        }
    }

    //--------------------------------------------------------------
    //  InitializeStandardRoutines  Initialize the standard
    //                              routines by entering their
    //                              identifiers into the symbol
    //                              table.
    //
    //      pSymtab : ptr to symbol table
    //--------------------------------------------------------------
}

public class TStdRtn
{
	public string pName;
	public TRoutineCode rc;
	public TDefnCode dc;
}
