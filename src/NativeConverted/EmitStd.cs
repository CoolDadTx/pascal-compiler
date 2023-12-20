//  *************************************************************
//  *                                                           *
//  *   C O D E   G E N E R A T O R   (Standard Routines)       *
//  *                                                           *
//  *   Generating and emit assembly code for calls to the      *
//  *   standard procedures and functions.                      *
//  *                                                           *
//  *   CLASSES: TCodeGenerator                                 *
//  *                                                           *
//  *   FILE:    prog14-1/emitstd.cpp                           *
//  *                                                           *
//  *   MODULE:  Code generator                                 *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************

partial class TCodeGenerator
{
    //--------------------------------------------------------------
    //  EmitStandardSubroutineCall  Emit code for a call to a
    //                              standard procedure or function.
    //
    //      pRoutineId : ptr to the subroutine name's symtab node
    //
    //  Return: ptr to the call's type object
    //--------------------------------------------------------------
    public TType EmitStandardSubroutineCall ( TSymtabNode pRoutineId )
    {
        switch (pRoutineId.defn.routine.which)
        {

            case TRoutineCode.RcRead:
            case TRoutineCode.RcReadln:
            return EmitReadReadlnCall(pRoutineId);

            case TRoutineCode.RcWrite:
            case TRoutineCode.RcWriteln:
            return EmitWriteWritelnCall(pRoutineId);

            case TRoutineCode.RcEof:
            case TRoutineCode.RcEoln:
            return EmitEofEolnCall(pRoutineId);

            case TRoutineCode.RcAbs:
            case TRoutineCode.RcSqr:
            return EmitAbsSqrCall(pRoutineId);

            case TRoutineCode.RcArctan:
            case TRoutineCode.RcCos:
            case TRoutineCode.RcExp:
            case TRoutineCode.RcLn:
            case TRoutineCode.RcSin:
            case TRoutineCode.RcSqrt:
            return EmitArctanCosExpLnSinSqrtCall(pRoutineId);

            case TRoutineCode.RcPred:
            case TRoutineCode.RcSucc:
            return EmitPredSuccCall(pRoutineId);

            case TRoutineCode.RcChr:
            return EmitChrCall();
            case TRoutineCode.RcOdd:
            return EmitOddCall();
            case TRoutineCode.RcOrd:
            return EmitOrdCall();

            case TRoutineCode.RcRound:
            case TRoutineCode.RcTrunc:
            return EmitRoundTruncCall(pRoutineId);

            default:
            return Globals.pDummyType;
        }
    }

    //--------------------------------------------------------------
    //  EmitReadReadlnCall          Emit code for a call to read or
    //                              readln.
    //
    //  Return: ptr to the dummy type object
    //--------------------------------------------------------------
    public TType EmitReadReadlnCall ( TSymtabNode pRoutineId )
    {
        //--Actual parameters are optional for readln.
        GetToken();
        if (token == TTokenCode.TcLParen)
        {

            //--Loop to emit code to read each parameter value.
            do
            {
                //--Variable
                GetToken();
                TSymtabNode pVarId = pNode;
                TType pVarType = EmitVariable(pVarId, true).Base();

                //--Read the value.
                if (pVarType == Globals.pIntegerType)
                {
                    {
                        Operator(TInstruction.Call);
                        pAsmBuffer.Put('\t');
                        NameLit(ReadInteger);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Pop);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Bx);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Mov);
                        pAsmBuffer.Put('\t');
                        WordIndirect(TRegister.Bx);
                        pAsmBuffer.Put(',');
                        Reg(TRegister.Ax);
                        pAsmBuffer.PutLine();
                    };
                } else if (pVarType == Globals.pRealType)
                {
                    {
                        Operator(TInstruction.Call);
                        pAsmBuffer.Put('\t');
                        NameLit(ReadReal);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Pop);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Bx);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Mov);
                        pAsmBuffer.Put('\t');
                        WordIndirect(TRegister.Bx);
                        pAsmBuffer.Put(',');
                        Reg(TRegister.Ax);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Mov);
                        pAsmBuffer.Put('\t');
                        HighDWordIndirect(TRegister.Bx);
                        pAsmBuffer.Put(',');
                        Reg(TRegister.Dx);
                        pAsmBuffer.PutLine();
                    };
                } else if (pVarType == Globals.pCharType)
                {
                    {
                        Operator(TInstruction.Call);
                        pAsmBuffer.Put('\t');
                        NameLit(ReadChar);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Pop);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Bx);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Mov);
                        pAsmBuffer.Put('\t');
                        ByteIndirect(TRegister.Bx);
                        pAsmBuffer.Put(',');
                        Reg(TRegister.Al);
                        pAsmBuffer.PutLine();
                    };
                }
            } while (token == TTokenCode.TcComma);

            GetToken(); // token after )
        }

        //--Skip the rest of the input line if readln.
        if (pRoutineId.defn.routine.which == TRoutineCode.RcReadln)
        {
            {
                Operator(TInstruction.Call);
                pAsmBuffer.Put('\t');
                NameLit(ReadLine);
                pAsmBuffer.PutLine();
            };
        }

        return Globals.pDummyType;
    }

    //--------------------------------------------------------------
    //  EmitWriteWritelnCall        Emit code for a call to write or
    //                              writeln.
    //
    //  Return: ptr to the dummy type object
    //--------------------------------------------------------------
    public TType EmitWriteWritelnCall ( TSymtabNode pRoutineId )
    {
        const int defaultFieldWidth = 10;
        const int defaultPrecision = 2;

        //--Actual parameters are optional for writeln.
        GetToken();
        if (token == TTokenCode.TcLParen)
        {

            //--Loop to emit code for each parameter value.
            do
            {
                //--<expr-1>
                GetToken();
                TType pExprType = EmitExpression().Base();

                //--Push the scalar value to be written onto the stack.
                //--A string value is already on the stack.
                if (pExprType.form != TFormCode.FcArray)
                    EmitPushOperand(pExprType);

                if (token == TTokenCode.TcColon)
                {

                    //--Field width <expr-2>
                    //--Push its value onto the stack.
                    GetToken();
                    EmitExpression();
                    {
                        Operator(TInstruction.Push);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Ax);
                        pAsmBuffer.PutLine();
                    };

                    if (token == TTokenCode.TcColon)
                    {

                        //--Precision <expr-3>
                        //--Push its value onto the stack.
                        GetToken();
                        EmitExpression();
                        {
                            Operator(TInstruction.Push);
                            pAsmBuffer.Put('\t');
                            Reg(TRegister.Ax);
                            pAsmBuffer.PutLine();
                        };
                    } else if (pExprType == Globals.pRealType)
                    {

                        {
                            //--No precision: Push the default precision.
                            Operator(TInstruction.Mov);
                            pAsmBuffer.Put('\t');
                            Reg(TRegister.Ax);
                            pAsmBuffer.Put(',');
                            IntegerLit(defaultPrecision);
                            pAsmBuffer.PutLine();
                        };
                        {
                            Operator(TInstruction.Push);
                            pAsmBuffer.Put('\t');
                            Reg(TRegister.Ax);
                            pAsmBuffer.PutLine();
                        };
                    }
                } else
                {

                    //--No field width: Push the default field width and
                    //--                the default precision.
                    if (pExprType == Globals.pIntegerType)
                    {
                        {
                            Operator(TInstruction.Mov);
                            pAsmBuffer.Put('\t');
                            Reg(TRegister.Ax);
                            pAsmBuffer.Put(',');
                            IntegerLit(defaultFieldWidth);
                            pAsmBuffer.PutLine();
                        };
                        {
                            Operator(TInstruction.Push);
                            pAsmBuffer.Put('\t');
                            Reg(TRegister.Ax);
                            pAsmBuffer.PutLine();
                        };
                    } else if (pExprType == Globals.pRealType)
                    {
                        {
                            Operator(TInstruction.Mov);
                            pAsmBuffer.Put('\t');
                            Reg(TRegister.Ax);
                            pAsmBuffer.Put(',');
                            IntegerLit(defaultFieldWidth);
                            pAsmBuffer.PutLine();
                        };
                        {
                            Operator(TInstruction.Push);
                            pAsmBuffer.Put('\t');
                            Reg(TRegister.Ax);
                            pAsmBuffer.PutLine();
                        };
                        {
                            Operator(TInstruction.Mov);
                            pAsmBuffer.Put('\t');
                            Reg(TRegister.Ax);
                            pAsmBuffer.Put(',');
                            IntegerLit(defaultPrecision);
                            pAsmBuffer.PutLine();
                        };
                        {
                            Operator(TInstruction.Push);
                            pAsmBuffer.Put('\t');
                            Reg(TRegister.Ax);
                            pAsmBuffer.PutLine();
                        };
                    } else
                    {
                        {
                            Operator(TInstruction.Mov);
                            pAsmBuffer.Put('\t');
                            Reg(TRegister.Ax);
                            pAsmBuffer.Put(',');
                            IntegerLit(0);
                            pAsmBuffer.PutLine();
                        };
                        {
                            Operator(TInstruction.Push);
                            pAsmBuffer.Put('\t');
                            Reg(TRegister.Ax);
                            pAsmBuffer.PutLine();
                        };
                    }
                }

                //--Emit the code to write the value.
                if (pExprType == Globals.pIntegerType)
                {
                    {
                        Operator(TInstruction.Call);
                        pAsmBuffer.Put('\t');
                        NameLit(WriteInteger);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Add);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Sp);
                        pAsmBuffer.Put(',');
                        IntegerLit(4);
                        pAsmBuffer.PutLine();
                    };
                } else if (pExprType == Globals.pRealType)
                {
                    {
                        Operator(TInstruction.Call);
                        pAsmBuffer.Put('\t');
                        NameLit(WriteReal);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Add);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Sp);
                        pAsmBuffer.Put(',');
                        IntegerLit(8);
                        pAsmBuffer.PutLine();
                    };
                } else if (pExprType == Globals.pBooleanType)
                {
                    {
                        Operator(TInstruction.Call);
                        pAsmBuffer.Put('\t');
                        NameLit(WriteBoolean);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Add);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Sp);
                        pAsmBuffer.Put(',');
                        IntegerLit(4);
                        pAsmBuffer.PutLine();
                    };
                } else if (pExprType == Globals.pCharType)
                {
                    {
                        Operator(TInstruction.Call);
                        pAsmBuffer.Put('\t');
                        NameLit(WriteChar);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Add);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Sp);
                        pAsmBuffer.Put(',');
                        IntegerLit(4);
                        pAsmBuffer.PutLine();
                    };
                } else
                { // string

                    {
                        //--Push the string length onto the stack.
                        Operator(TInstruction.Mov);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Ax);
                        pAsmBuffer.Put(',');
                        IntegerLit(pExprType.array.elmtCount);
                        pAsmBuffer.PutLine();
                    };

                    {
                        Operator(TInstruction.Push);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Ax);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Call);
                        pAsmBuffer.Put('\t');
                        NameLit(WriteString);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Add);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Sp);
                        pAsmBuffer.Put(',');
                        IntegerLit(6);
                        pAsmBuffer.PutLine();
                    };
                }

            } while (token == TTokenCode.TcComma);

            GetToken(); // token after )
        }

        //--End the line if writeln.
        if (pRoutineId.defn.routine.which == TRoutineCode.RcWriteln)
        {
            {
                Operator(TInstruction.Call);
                pAsmBuffer.Put('\t');
                NameLit(WriteLine);
                pAsmBuffer.PutLine();
            };
        }

        return Globals.pDummyType;
    }

    //--------------------------------------------------------------
    //  EmitEofEolnCall         Emit code for a call to eof or eoln.
    //
    //  Return: ptr to the boolean type object
    //--------------------------------------------------------------
    public TType EmitEofEolnCall ( TSymtabNode pRoutineId )
    {
        {
            Operator(TInstruction.Call);
            pAsmBuffer.Put('\t');
            NameLit(pRoutineId.defn.routine.which == ((int)TRoutineCode.RcEof) != 0 ? StdEof : StdEoln);
            pAsmBuffer.PutLine();
        };

        GetToken(); // token after function name
        return Globals.pBooleanType;
    }

    //--------------------------------------------------------------
    //  EmitAbsSqrCall           Emit code for a call to abs or sqr.
    //
    //  Return: ptr to the result's type object
    //--------------------------------------------------------------
    public TType EmitAbsSqrCall ( TSymtabNode pRoutineId )
    {
        GetToken(); // (
        GetToken();

        TType pParmType = EmitExpression().Base();

        switch (pRoutineId.defn.routine.which)
        {

            case TRoutineCode.RcAbs:
            if (pParmType == Globals.pIntegerType)
            {
                int nonNegativeLabelIndex = ++Globals.asmLabelIndex;

                {
                    Operator(TInstruction.Cmp);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Ax);
                    pAsmBuffer.Put(',');
                    IntegerLit(0);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.Jge);
                    pAsmBuffer.Put('\t');
                    Label(StmtLabelPrefix, nonNegativeLabelIndex);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.Neg);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Ax);
                    pAsmBuffer.PutLine();
                };

                EmitStatementLabel(nonNegativeLabelIndex);
            } else
            {
                EmitPushOperand(pParmType);
                {
                    Operator(TInstruction.Call);
                    pAsmBuffer.Put('\t');
                    NameLit(StdAbs);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.Add);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Sp);
                    pAsmBuffer.Put(',');
                    IntegerLit(4);
                    pAsmBuffer.PutLine();
                };
            }
            break;

            case TRoutineCode.RcSqr:
            if (pParmType == Globals.pIntegerType)
            {
                {
                    Operator(TInstruction.Mov);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Dx);
                    pAsmBuffer.Put(',');
                    Reg(TRegister.Ax);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.Imul);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Dx);
                    pAsmBuffer.PutLine();
                };
            } else
            {
                EmitPushOperand(pParmType);
                EmitPushOperand(pParmType);
                {
                    Operator(TInstruction.Call);
                    pAsmBuffer.Put('\t');
                    NameLit(FloatMultiply);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.Add);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Sp);
                    pAsmBuffer.Put(',');
                    IntegerLit(8);
                    pAsmBuffer.PutLine();
                };
            }
            break;
        }

        GetToken(); // token after )
        return pParmType;
    }

    //--------------------------------------------------------------
    //  EmitArctanCosExpLnSinSqrtCall       Emit code for a call to
    //                                      arctan, cos, exp, ln,
    //                                      sin, or sqrt.
    //
    //  Return: ptr to the real type object
    //--------------------------------------------------------------
    public TType EmitArctanCosExpLnSinSqrtCall ( TSymtabNode pRoutineId )
    {
        string stdFuncName = null;

        GetToken(); // (
        GetToken();

        //--Evaluate the parameter, and convert an integer value to
        //--real if necessary.
        TType pParmType = EmitExpression().Base();
        if (pParmType == Globals.pIntegerType)
        {
            {
                Operator(TInstruction.Push);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Call);
                pAsmBuffer.Put('\t');
                NameLit(FloatConvert);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Add);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Sp);
                pAsmBuffer.Put(',');
                IntegerLit(2);
                pAsmBuffer.PutLine();
            };
        }

        EmitPushOperand(Globals.pRealType);

        switch (pRoutineId.defn.routine.which)
        {
            case TRoutineCode.RcArctan:
            stdFuncName = StdArctan;
            break;
            case TRoutineCode.RcCos:
            stdFuncName = StdCos;
            break;
            case TRoutineCode.RcExp:
            stdFuncName = StdExp;
            break;
            case TRoutineCode.RcLn:
            stdFuncName = StdLn;
            break;
            case TRoutineCode.RcSin:
            stdFuncName = StdSin;
            break;
            case TRoutineCode.RcSqrt:
            stdFuncName = StdSqrt;
            break;
        }

        {
            Operator(TInstruction.Call);
            pAsmBuffer.Put('\t');
            NameLit(stdFuncName);
            pAsmBuffer.PutLine();
        };
        {
            Operator(TInstruction.Add);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Sp);
            pAsmBuffer.Put(',');
            IntegerLit(4);
            pAsmBuffer.PutLine();
        };

        GetToken(); // token after )
        return Globals.pRealType;
    }

    //--------------------------------------------------------------
    //  EmitPredSuccCall            Emit code for a call to pred
    //                              or succ.
    //
    //  Return: ptr to the result's type object
    //--------------------------------------------------------------
    public TType EmitPredSuccCall ( TSymtabNode pRoutineId )
    {
        GetToken(); // (
        GetToken();

        TType pParmType = EmitExpression();

        {
            Operator(pRoutineId.defn.routine.which == ((int)TRoutineCode.RcPred) != 0 ? TInstruction.Decr : TInstruction.Incr);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Ax);
            pAsmBuffer.PutLine();
        };

        GetToken(); // token after )
        return pParmType;
    }

    //--------------------------------------------------------------
    //  EmitChrCall                 Emit code for a call to chr.
    //
    //  Return: ptr to the character type object
    //--------------------------------------------------------------
    public TType EmitChrCall ()
    {
        GetToken(); // (
        GetToken();
        EmitExpression();

        GetToken(); // token after )
        return Globals.pCharType;
    }

    //--------------------------------------------------------------
    //  EmitOddCall                 Emit code for a call to odd.
    //
    //  Return: ptr to the boolean type object
    //--------------------------------------------------------------
    public TType EmitOddCall ()
    {
        GetToken(); // (
        GetToken();
        EmitExpression();

        {
            Operator(TInstruction.And);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Ax);
            pAsmBuffer.Put(',');
            IntegerLit(1);
            pAsmBuffer.PutLine();
        };

        GetToken(); // token after )
        return Globals.pBooleanType;
    }

    //--------------------------------------------------------------
    //  EmitOrdCall                 Emit code for a call to ord.
    //
    //  Return: ptr to the integer type object
    //--------------------------------------------------------------
    public TType EmitOrdCall ()
    {
        GetToken(); // (
        GetToken();
        EmitExpression();

        GetToken(); // token after )
        return Globals.pIntegerType;
    }

    //--------------------------------------------------------------
    //  EmitRoundTruncCall          Emit code for a call to round
    //                              or trunc.
    //
    //  Return: ptr to the integer type object
    //--------------------------------------------------------------
    public TType EmitRoundTruncCall ( TSymtabNode pRoutineId )
    {
        GetToken(); // (
        GetToken();
        EmitExpression();

        EmitPushOperand(Globals.pRealType);
        {
            Operator(TInstruction.Call);
            pAsmBuffer.Put('\t');
            NameLit(pRoutineId.defn.routine.which == ((int)TRoutineCode.RcRound) != 0 ? StdRound : StdTrunc);
            pAsmBuffer.PutLine();
        };
        {
            Operator(TInstruction.Add);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Sp);
            pAsmBuffer.Put(',');
            IntegerLit(4);
            pAsmBuffer.PutLine();
        };

        GetToken(); // token after )
        return Globals.pIntegerType;
    }
}