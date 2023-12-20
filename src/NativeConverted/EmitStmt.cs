//  *************************************************************
//  *                                                           *
//  *   C O D E   G E N E R A T O R   (Statements)              *
//  *                                                           *
//  *   Generating and emit assembly code for statements.       *
//  *                                                           *
//  *   CLASSES: TCodeGenerator                                 *
//  *                                                           *
//  *   FILE:    prog14-1/emitstmt.cpp                          *
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
    //  EmitStatement       Emit code for a statement.
    //--------------------------------------------------------------
    public void EmitStatement ()
    {
        //--Emit the current statement as a comment.
        EmitStmtComment();

        switch (token)
        {

            case TTokenCode.TcIdentifier:
            {
                if (pNode.defn.how == TDefnCode.DcProcedure)
                    EmitSubroutineCall(pNode);
                else
                    EmitAssignment(pNode);
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

    //--------------------------------------------------------------
    //  EmitStatementList   Emit code for a statement list until
    //                      the terminator token.
    //
    //      terminator : the token that terminates the list
    //--------------------------------------------------------------
    public void EmitStatementList ( TTokenCode terminator )
    {
        //--Loop to emit code for statements and skip semicolons.
        do
        {
            EmitStatement();
            while (token == TTokenCode.TcSemicolon)
                GetToken();
        } while (token != terminator);
    }

    //--------------------------------------------------------------
    //  EmitAssignment      Emit code for an assignment statement.
    //--------------------------------------------------------------
    public void EmitAssignment ( TSymtabNode pTargetId )
    {
        TType pTargetType = pTargetId.pType;
        // ptr to target type object
        TType pExprType; // ptr to expression type object
        bool addressOnStack; // true if target address has been pushed
                            //   onto the runtime stack

        //--Assignment to a function name.
        if (pTargetId.defn.how == TDefnCode.DcFunction)
        {
            EmitPushReturnValueAddress(pTargetId);
            addressOnStack = true;
            GetToken();
        }

        //--Assignment to a nonscalar, a formal VAR parameter, or to
        //--a nonglobal and nonlocal variable. EmitVariable emits code
        //--that leaves the target address on top of the runtime stack.
        else if ((pTargetId.defn.how == TDefnCode.DcVarParm) || !pTargetType.IsScalar() || ((pTargetId.level > 1) && (pTargetId.level < Globals.currentNestingLevel)))
        {
            pTargetType = EmitVariable(pTargetId, true);
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
        if ((pTargetType.Base() == Globals.pIntegerType) || (pTargetType.Base().form == TFormCode.FcEnum))
        {
            if (addressOnStack)
            {
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
            } else
            {
                Operator(TInstruction.Mov);
                pAsmBuffer.Put('\t');
                Word(pTargetId);
                pAsmBuffer.Put(',');
                Reg(TRegister.Ax);
                pAsmBuffer.PutLine();
            };
        } else if (pTargetType.Base() == Globals.pCharType)
        {

            //--char := char
            if (addressOnStack)
            {
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
            } else
            {
                Operator(TInstruction.Mov);
                pAsmBuffer.Put('\t');
                Byte(pTargetId);
                pAsmBuffer.Put(',');
                Reg(TRegister.Al);
                pAsmBuffer.PutLine();
            };
        } else if (pTargetType == Globals.pRealType)
        {

            //--real := ...
            if (pExprType == Globals.pIntegerType)
            {

                {
                    //--Convert an integer value to real.
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

            //--... real
            if (addressOnStack)
            {
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
            } else
            {
                {
                    Operator(TInstruction.Mov);
                    pAsmBuffer.Put('\t');
                    Word(pTargetId);
                    pAsmBuffer.Put(',');
                    Reg(TRegister.Ax);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.Mov);
                    pAsmBuffer.Put('\t');
                    HighDWord(pTargetId);
                    pAsmBuffer.Put(',');
                    Reg(TRegister.Dx);
                    pAsmBuffer.PutLine();
                };
            }
        } else
        {

            //--array  := array
            //--record := record
            {
                Operator(TInstruction.Mov);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Cx);
                pAsmBuffer.Put(',');
                IntegerLit(pTargetType.size);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Pop);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Si);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Pop);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Di);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Mov);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.Put(',');
                Reg(TRegister.Ds);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Mov);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Es);
                pAsmBuffer.Put(',');
                Reg(TRegister.Ax);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Cld);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.RepMovsb);
                pAsmBuffer.PutLine();
            };
        }
    }

    //--------------------------------------------------------------
    //  EmitREPEAT      Emit code for a REPEAT statement:
    //
    //                      REPEAT <stmt-list> UNTIL <expr>
    //--------------------------------------------------------------
    public void EmitREPEAT ()
    {
        int stmtListLabelIndex = ++Globals.asmLabelIndex;
        int followLabelIndex = ++Globals.asmLabelIndex;

        EmitStatementLabel(stmtListLabelIndex);

        //--<stmt-list> UNTIL
        GetToken();
        EmitStatementList(TTokenCode.TcUNTIL);

        EmitStmtComment();

        //--<expr>
        GetToken();
        EmitExpression();

        {
            //--Decide whether or not to branch back to the loop start.
            Operator(TInstruction.Cmp);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Ax);
            pAsmBuffer.Put(',');
            IntegerLit(1);
            pAsmBuffer.PutLine();
        };
        {
            Operator(TInstruction.Je);
            pAsmBuffer.Put('\t');
            Label(StmtLabelPrefix, followLabelIndex);
            pAsmBuffer.PutLine();
        };
        {
            Operator(TInstruction.Jmp);
            pAsmBuffer.Put('\t');
            Label(StmtLabelPrefix, stmtListLabelIndex);
            pAsmBuffer.PutLine();
        };

        EmitStatementLabel(followLabelIndex);
    }

    //--------------------------------------------------------------
    //  EmitWHILE       Emit code for a WHILE statement:
    //
    //                      WHILE <expr> DO <stmt>
    //--------------------------------------------------------------
    public void EmitWHILE ()
    {
        int exprLabelIndex = ++Globals.asmLabelIndex;
        int stmtLabelIndex = ++Globals.asmLabelIndex;
        int followLabelIndex = ++Globals.asmLabelIndex;

        GetToken();
        GetLocationMarker(); // ignored

        EmitStatementLabel(exprLabelIndex);

        //--<expr>
        GetToken();
        EmitExpression();

        {
            Operator(TInstruction.Cmp);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Ax);
            pAsmBuffer.Put(',');
            IntegerLit(1);
            pAsmBuffer.PutLine();
        };
        {
            Operator(TInstruction.Je);
            pAsmBuffer.Put('\t');
            Label(StmtLabelPrefix, stmtLabelIndex);
            pAsmBuffer.PutLine();
        };
        {
            Operator(TInstruction.Jmp);
            pAsmBuffer.Put('\t');
            Label(StmtLabelPrefix, followLabelIndex);
            pAsmBuffer.PutLine();
        };

        EmitStatementLabel(stmtLabelIndex);

        //--DO <stmt>
        GetToken();
        EmitStatement();

        {
            Operator(TInstruction.Jmp);
            pAsmBuffer.Put('\t');
            Label(StmtLabelPrefix, exprLabelIndex);
            pAsmBuffer.PutLine();
        };
        EmitStatementLabel(followLabelIndex);
    }

    //--------------------------------------------------------------
    //  EmitIF          Emit code for an IF statement:
    //
    //                      IF <expr> THEN <stmt-1>
    //
    //                  or:
    //
    //                      IF <expr> THEN <stmt-1> ELSE <stmt-2>
    //--------------------------------------------------------------
    public void EmitIF ()
    {
        int trueLabelIndex = ++Globals.asmLabelIndex;
        int falseLabelIndex = ++Globals.asmLabelIndex;

        GetToken();
        GetLocationMarker(); // ignored

        //--<expr>
        GetToken();
        EmitExpression();

        {
            Operator(TInstruction.Cmp);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Ax);
            pAsmBuffer.Put(',');
            IntegerLit(1);
            pAsmBuffer.PutLine();
        };
        {
            Operator(TInstruction.Je);
            pAsmBuffer.Put('\t');
            Label(StmtLabelPrefix, trueLabelIndex);
            pAsmBuffer.PutLine();
        };
        {
            Operator(TInstruction.Jmp);
            pAsmBuffer.Put('\t');
            Label(StmtLabelPrefix, falseLabelIndex);
            pAsmBuffer.PutLine();
        };

        EmitStatementLabel(trueLabelIndex);

        StartComment("THEN");
        PutLine();

        //--THEN <stmt-1>
        GetToken();
        EmitStatement();

        if (token == TTokenCode.TcELSE)
        {
            GetToken();
            GetLocationMarker(); // ignored

            int followLabelIndex = ++Globals.asmLabelIndex;
            {
                Operator(TInstruction.Jmp);
                pAsmBuffer.Put('\t');
                Label(StmtLabelPrefix, followLabelIndex);
                pAsmBuffer.PutLine();
            };

            StartComment("ELSE");
            PutLine();

            EmitStatementLabel(falseLabelIndex);

            GetToken();
            EmitStatement();

            EmitStatementLabel(followLabelIndex);
        } else
        {
            EmitStatementLabel(falseLabelIndex);
        }
    }

    //--------------------------------------------------------------
    //  EmitFOR         Emit code for a FOR statement:
    //
    //                      FOR <id> := <expr-1> TO|DOWNTO <expr-2>
    //                          DO <stmt>
    //--------------------------------------------------------------
    public void EmitFOR ()
    {
        int testLabelIndex = ++Globals.asmLabelIndex;
        int stmtLabelIndex = ++Globals.asmLabelIndex;
        int terminateLabelIndex = ++Globals.asmLabelIndex;

        GetToken();
        GetLocationMarker(); // ignored

        //--Get pointers to the control variable and to its type object.
        GetToken();
        TSymtabNode pControlId = pNode;
        TType pControlType = pNode.pType;

        var charFlag = (pControlType.Base() == Globals.pCharType);

        //-- <id> := <expr-1>
        EmitAssignment(pControlId);

        //--TO or DOWNTO
        var toFlag = token == TTokenCode.TcTO;

        EmitStatementLabel(testLabelIndex);

        //--<expr-2>
        GetToken();
        EmitExpression();

        if (charFlag)
        {
            Operator(TInstruction.Cmp);
            pAsmBuffer.Put('\t');
            Byte(pControlId);
            pAsmBuffer.Put(',');
            Reg(TRegister.Al);
            pAsmBuffer.PutLine();
        } else
        {
            Operator(TInstruction.Cmp);
            pAsmBuffer.Put('\t');
            Word(pControlId);
            pAsmBuffer.Put(',');
            Reg(TRegister.Ax);
            pAsmBuffer.PutLine();
        }
        {
            Operator(toFlag ? TInstruction.Jle : TInstruction.Jge);
            pAsmBuffer.Put('\t');
            Label(StmtLabelPrefix, stmtLabelIndex);
            pAsmBuffer.PutLine();
        };
        {
            Operator(TInstruction.Jmp);
            pAsmBuffer.Put('\t');
            Label(StmtLabelPrefix, terminateLabelIndex);
            pAsmBuffer.PutLine();
        };

        EmitStatementLabel(stmtLabelIndex);

        //--DO <stmt>
        GetToken();
        EmitStatement();

        {
            Operator(toFlag ? TInstruction.Incr : TInstruction.Decr);
            pAsmBuffer.Put('\t');
            if (charFlag)
                Byte(pControlId);
            else
                Word(pControlId);
            pAsmBuffer.PutLine();
        };
        {
            Operator(TInstruction.Jmp);
            pAsmBuffer.Put('\t');
            Label(StmtLabelPrefix, testLabelIndex);
            pAsmBuffer.PutLine();
        };

        EmitStatementLabel(terminateLabelIndex);

        {
            Operator(toFlag ? TInstruction.Decr : TInstruction.Incr);
            pAsmBuffer.Put('\t');
            if (charFlag)
                Byte(pControlId);
            else
                Word(pControlId);
            pAsmBuffer.PutLine();
        };
    }

    // C# does not allow declaring types within methods - EmitCASE
    private struct TBranchEntry
    {
        public int labelValue { get; set; }
        public int branchLocation { get; set; }
        public int labelIndex {  get; set; }  
    }

    //--------------------------------------------------------------
    //  EmitCASE        Emit code for a CASE statement:
    //
    //                      CASE <expr> OF
    //                          <case-branch> ;
    //                          ...
    //                      END
    //--------------------------------------------------------------
    public void EmitCASE ()
    {
        int i;
        int j;
        int followLabelIndex = ++Globals.asmLabelIndex;

        TBranchEntry[] pBranchTable = null;

        //--Get the locations of the token that follows the
        //--CASE statement and of the branch table.
        GetToken();
        var atFollow = GetLocationMarker();
        GetToken();
        var atBranchTable = GetLocationMarker();

        //--<epxr>
        GetToken();
        TType pExprType = EmitExpression();

        int labelValue = 0;
        int branchLocation = 0;
        var charFlag = pExprType.Base() == Globals.pCharType;

        //--Loop through the branch table in the icode
        //--to count the number of entries.
        int count = 0;
        GoTo(atBranchTable + 1);
        for (; ; )
        {
            GetCaseItem(ref labelValue, ref branchLocation);
            if (branchLocation == 0)
                break;
            else
                ++count;
        }

        //--Make a copy of the branch table.
        pBranchTable = Arrays.InitializeWithDefaultInstances<TBranchEntry>(count);
        GoTo(atBranchTable + 1);
        for (i = 0; i < count; ++i)
        {
            GetCaseItem(ref labelValue, ref branchLocation);
            pBranchTable[i].labelValue = labelValue;
            pBranchTable[i].branchLocation = branchLocation;
        }

        //--Loop through the branch table copy to emit test code.
        for (i = 0; i < count; ++i)
        {
            int testLabelIndex = ++Globals.asmLabelIndex;
            int branchLabelIndex = ++Globals.asmLabelIndex;

            {
                Operator(TInstruction.Cmp);
                pAsmBuffer.Put('\t');
                if (charFlag)
                    Reg(TRegister.Al);
                else
                    Reg(TRegister.Ax);
                pAsmBuffer.Put(',');
                IntegerLit(pBranchTable[i].labelValue);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Jne);
                pAsmBuffer.Put('\t');
                Label(StmtLabelPrefix, testLabelIndex);
                pAsmBuffer.PutLine();
            };

            //--See if the branch location is already in the branch table
            //--copy. If so, reuse the branch label index.
            for (j = 0; j < i; ++j)
            {
                if (pBranchTable[j].branchLocation == pBranchTable[i].branchLocation)
                {
                    branchLabelIndex = pBranchTable[j].labelIndex;
                    break;
                }
            }

            {
                Operator(TInstruction.Jmp);
                pAsmBuffer.Put('\t');
                Label(StmtLabelPrefix, branchLabelIndex);
                pAsmBuffer.PutLine();
            };
            EmitStatementLabel(testLabelIndex);

            //--Enter the branch label index into the branch table copy
            //--only if it is new; otherwise, enter 0.
            pBranchTable[i].labelIndex = (j < i) ? 0 : branchLabelIndex;
        }
        {
            Operator(TInstruction.Jmp);
            pAsmBuffer.Put('\t');
            Label(StmtLabelPrefix, followLabelIndex);
            pAsmBuffer.PutLine();
        };

        //--Loop through the branch table copy again to emit
        //--branch statement code that hasn't already been emitted.
        for (i = 0; i < count; ++i)
        {
            if (pBranchTable[i].labelIndex != 0)
            {
                GoTo(pBranchTable[i].branchLocation);
                EmitStatementLabel(pBranchTable[i].labelIndex);

                GetToken();
                EmitStatement();
                {
                    Operator(TInstruction.Jmp);
                    pAsmBuffer.Put('\t');
                    Label(StmtLabelPrefix, followLabelIndex);
                    pAsmBuffer.PutLine();
                };
            }
        }

        pBranchTable = null;

        GoTo(atFollow);
        GetToken();

        StartComment("END");
        PutLine();

        EmitStatementLabel(followLabelIndex);
    }

    //--------------------------------------------------------------
    //  EmitCompound        Emit code for a compound statement:
    //
    //                          BEGIN <stmt-list> END
    //--------------------------------------------------------------
    public void EmitCompound ()
    {
        StartComment("BEGIN");
        PutLine();

        //--<stmt-list> END
        GetToken();
        EmitStatementList(TTokenCode.TcEND);

        GetToken();

        StartComment("END");
        PutLine();
    }
}