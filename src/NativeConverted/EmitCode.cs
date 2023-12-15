//  *************************************************************
//  *                                                           *
//  *   E M I T   C O D E   S E Q U E N C E S                   *
//  *                                                           *
//  *   Routines for generating and emitting various assembly   *
//  *   language code sequences.                                *
//  *                                                           *
//  *   CLASSES: TCodeGenerator                                 *
//  *                                                           *
//  *   FILE:    prog13-1/emitcode.c                            *
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
    //  Go                          Start the compilation.
    //--------------------------------------------------------------
    public override void Go ( TSymtabNode pProgramId )
    {
        EmitProgramPrologue();

        //--Emit code for the program.
        currentNestingLevel = 1;
        EmitMain(pProgramId);

        EmitProgramEpilogue(pProgramId);
    }

    //--------------------------------------------------------------
    //  EmitStatementLabel      Emit a statement label constructed
    //                          from the label index.
    //
    //                          Example:  $L_007:
    //
    //      index : index value
    //--------------------------------------------------------------
    public void EmitStatementLabel ( int index )
    {
        AsmText() = String.Format("{0}_{1:D3}:", DefineConstants.StmtLabelPrefix, index);
        PutLine();
    }

    //              ******************
    //              *                *
    //              *  Declarations  *
    //              *                *
    //              ******************

    //--------------------------------------------------------------
    //  EmitDeclarations    Emit code for the parameter and local
    //                      variable declarations of a routine.
    //
    //      pRoutineId : ptr to the routine's symbol table node
    //--------------------------------------------------------------
    public void EmitDeclarations ( TSymtabNode pRoutineId )
    {
        TSymtabNode pParmId = pRoutineId.defn.routine.locals.pParmIds;
        TSymtabNode pVarId = pRoutineId.defn.routine.locals.pVariableIds;

        EmitVarDeclComment(pRoutineId.defn.routine.locals.pVariableIds);
        PutLine();

        //--Subroutine parameters
        while (pParmId != null)
        {
            EmitStackOffsetEquate(pParmId);
            pParmId = pParmId.next;
        }

        //--Variables
        while (pVarId != null)
        {
            EmitStackOffsetEquate(pVarId);
            pVarId = pVarId.next;
        }
    }

    //--------------------------------------------------------------
    //  EmitStackOffsetEquate       Emit a stack frame offset equate
    //                              for a parameter id or a local
    //                              variable id.
    //
    //                              Examples: parm_007 EQU <pb+6>
    //                                        var_008  EQU <bp-10>
    //
    //      pId : ptr to symbol table node
    //--------------------------------------------------------------
    public void EmitStackOffsetEquate ( TSymtabNode pId )
    {
        //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent to pointers to value types:
        //ORIGINAL LINE: char *pName = pId->String();
        char pName = pId.String();
        int labelIndex = pId.labelIndex;
        int offset = pId.defn.data.offset;
        TType pType = pId.pType;

        if (pType == pCharType)
            //C++ TO C# CONVERTER TODO TASK: The following line has a C format specifier which cannot be directly translated to C#:
            //ORIGINAL LINE: sprintf(AsmText(), "%s_%03d\tEQU\t<BYTE PTR [bp%+d]>", pName, labelIndex, offset);
            AsmText() = String.Format("{0}_{1:D3}\tEQU\t<BYTE PTR [bp%+d]>", pName, labelIndex, offset);
        else
            //C++ TO C# CONVERTER TODO TASK: The following line has a C format specifier which cannot be directly translated to C#:
            //ORIGINAL LINE: sprintf(AsmText(), "%s_%03d\tEQU\t<WORD PTR [bp%+d]>", pName, labelIndex, offset);
            AsmText() = String.Format("{0}_{1:D3}\tEQU\t<WORD PTR [bp%+d]>", pName, labelIndex, offset);

        PutLine();
    }

    //              **********************
    //              *                    *
    //              *  Loads and Pushes  *
    //              *                    *
    //              **********************

    //--------------------------------------------------------------
    //  EmitAdjustBP        Emit code to adjust register bp if
    //                      necessary to point to the stack frame
    //                      of an enclosing subroutine.
    //
    //      level : nesting level of enclosing subroutine's data
    //--------------------------------------------------------------
    public void EmitAdjustBP ( int level )
    {
        //--Don't do anything if local or global.
        if ((level == currentNestingLevel) || (level == 1))
            return;

        {
            //--Emit code to chase static links.
            Operator(TInstruction.Mov);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Cx);
            pAsmBuffer.Put(',');
            Reg(TRegister.Bp);
            pAsmBuffer.PutLine();
        }; // save bp in cx
        do
        {
            {
                Operator(TInstruction.Mov);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Bp);
                pAsmBuffer.Put(',');
                NameLit(DefineConstants.StaticLink);
                pAsmBuffer.PutLine();
            }; // chase
        } while (++level < currentNestingLevel);
    }

    //--------------------------------------------------------------
    //  EmitRestoreBP       Emit code to restore register bp if
    //                      necessary to point to the current
    //                      stack frame.
    //
    //      level : nesting level of enclosing subroutine's data
    //--------------------------------------------------------------
    public void EmitRestoreBP ( int level )
    {
        //--Don't do anything if local or global.
        if ((level == currentNestingLevel) || (level == 1))
            return;

        {
            //--Emit code to restore bp.
            Operator(TInstruction.Mov);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Bp);
            pAsmBuffer.Put(',');
            Reg(TRegister.Cx);
            pAsmBuffer.PutLine();
        };
    }

    //--------------------------------------------------------------
    //  EmitLoadValue       Emit code to load a scalar value
    //                      into ax or dx:ax.
    //
    //      pId : ptr to symbol table node of parm or variable
    //--------------------------------------------------------------
    public void EmitLoadValue ( TSymtabNode pId )
    {
        TType pType = pId.pType;

        EmitAdjustBP(pId.level);

        if (pId.defn.how == TDefnCode.DcVarParm)
        {
            //--VAR formal parameter.
            //--ax or dx:ax = value the address points to
            {
                Operator(TInstruction.Mov);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Bx);
                pAsmBuffer.Put(',');
                Word(pId);
                pAsmBuffer.PutLine();
            };
            if (pType == pCharType)
            {

                {
                    //--Character:  al = value
                    Operator(TInstruction.Sub);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Ax);
                    pAsmBuffer.Put(',');
                    Reg(TRegister.Ax);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.Mov);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Al);
                    pAsmBuffer.Put(',');
                    ByteIndirect(TRegister.Bx);
                    pAsmBuffer.PutLine();
                };
            } else if (pType == pRealType)
            {

                {
                    //--Real: dx:ax = value
                    Operator(TInstruction.Mov);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Ax);
                    pAsmBuffer.Put(',');
                    WordIndirect(TRegister.Bx);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.Mov);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Ax);
                    pAsmBuffer.Put(',');
                    HighDWordIndirect(TRegister.Bx);
                    pAsmBuffer.PutLine();
                };
            } else
            {

                {
                    //--Integer or enumeration: ax = value
                    Operator(TInstruction.Mov);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Ax);
                    pAsmBuffer.Put(',');
                    WordIndirect(TRegister.Bx);
                    pAsmBuffer.PutLine();
                };
            }
        } else
        {

            //--Load the value into ax or dx:ax.
            if (pType == pCharType)
            {

                {
                    //--Character:  al = value
                    Operator(TInstruction.Sub);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Ax);
                    pAsmBuffer.Put(',');
                    Reg(TRegister.Ax);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.Mov);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Al);
                    pAsmBuffer.Put(',');
                    Byte(pId);
                    pAsmBuffer.PutLine();
                };
            } else if (pType == pRealType)
            {

                {
                    //--Real: dx:ax = value
                    Operator(TInstruction.Mov);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Ax);
                    pAsmBuffer.Put(',');
                    Word(pId);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.Mov);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Dx);
                    pAsmBuffer.Put(',');
                    HighDWord(pId);
                    pAsmBuffer.PutLine();
                };
            } else
            {

                {
                    //--Integer or enumeration: ax = value
                    Operator(TInstruction.Mov);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Ax);
                    pAsmBuffer.Put(',');
                    Word(pId);
                    pAsmBuffer.PutLine();
                };
            }
        }

        EmitRestoreBP(pId.level);
    }

    //--------------------------------------------------------------
    //  EmitLoadFloatLit    Emit code to load a float literal into
    //                      dx:ax. Append the literal to the float
    //                      literal list.
    //
    //      pNode : ptr to symbol table node of literal
    //--------------------------------------------------------------
    public void EmitLoadFloatLit ( TSymtabNode pNode )
    {
        TSymtabNode pf;

        {
            //--dx:ax = value
            Operator(TInstruction.Mov);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Ax);
            pAsmBuffer.Put(',');
            WordLabel(DefineConstants.FloatLabelPrefix, pNode.labelIndex);
            pAsmBuffer.PutLine();
        };
        {
            Operator(TInstruction.Mov);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Dx);
            pAsmBuffer.Put(',');
            HighDWordLabel(DefineConstants.FloatLabelPrefix, pNode.labelIndex);
            pAsmBuffer.PutLine();
        };

        //--Check if the float is already in the float literal list.
        for (pf = pFloatLitList; pf != null; pf = pf.next)
        {
            if (pf == pNode)
                return;
        }

        //--Append it to the list if it isn't already there.
        pNode.next = pFloatLitList;
        pFloatLitList = pNode;
    }

    //--------------------------------------------------------------
    //  EmitPushStringLit   Emit code to push the address of a
    //                      string literal onto the runtime stack.
    //                      Append the literal to the string literal
    //                      list.
    //
    //      pNode : ptr to symbol table node of literal
    //--------------------------------------------------------------
    public void EmitPushStringLit ( TSymtabNode pNode )
    {
        TSymtabNode ps;

        {
            //--ax = addresss of string
            Operator(TInstruction.Lea);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Ax);
            pAsmBuffer.Put(',');
            WordLabel(DefineConstants.StringLabelPrefix, pNode.labelIndex);
            pAsmBuffer.PutLine();
        };
        {
            Operator(TInstruction.Push);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Ax);
            pAsmBuffer.PutLine();
        };

        //--Check if the string is already in the string literal list.
        for (ps = pStringLitList; ps != null; ps = ps.next)
        {
            if (ps == pNode)
                return;
        }

        //--Append it to the list if it isn't already there.
        pNode.next = pStringLitList;
        pStringLitList = pNode;
    }

    //--------------------------------------------------------------
    //  EmitPushOperand             Emit code to push a scalar
    //                              operand value onto the stack.
    //
    //      pType : ptr to type of value
    //--------------------------------------------------------------
    public void EmitPushOperand ( TType pType )
    {
        if (pType.IsScalar() == 0)
            return;

        if (pType == pRealType)
        {
            Operator(TInstruction.Push);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Dx);
            pAsmBuffer.PutLine();
        };
        {
            Operator(TInstruction.Push);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Ax);
            pAsmBuffer.PutLine();
        };
    }

    //--------------------------------------------------------------
    //  EmitPushAddress             Emit code to push an address
    //                              onto the stack.
    //
    //      pId : ptr to symbol table node of parm or variable
    //--------------------------------------------------------------
    public void EmitPushAddress ( TSymtabNode pId )
    {
        int varLevel = pId.level;
        int isVarParm = (int)pId.defn.how == (int)TDefnCode.DcVarParm;

        EmitAdjustBP(varLevel);

        {
            Operator(isVarParm != 0 ? TInstruction.Mov : TInstruction.Lea);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Ax);
            pAsmBuffer.Put(',');
            Word(pId);
            pAsmBuffer.PutLine();
        }
        {
            Operator(TInstruction.Push);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Ax);
            pAsmBuffer.PutLine();
        };

        EmitRestoreBP(varLevel);
    }

    //--------------------------------------------------------------
    //  EmitPushReturnValueAddress      Emit code to push the   
    //                                  address of the function
    //                                  return value in the
    //                                  stack frame.
    //
    //      pId : ptr to symbol table node of function
    //--------------------------------------------------------------
    public void EmitPushReturnValueAddress ( TSymtabNode pId )
    {
        EmitAdjustBP(pId.level + 1);

        {
            Operator(TInstruction.Lea);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Ax);
            pAsmBuffer.Put(',');
            NameLit(DefineConstants.ReturnValue);
            pAsmBuffer.PutLine();
        };
        {
            Operator(TInstruction.Push);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Ax);
            pAsmBuffer.PutLine();
        };

        EmitRestoreBP(pId.level + 1);
    }

    //--------------------------------------------------------------
    //  EmitPromoteToReal        Emit code to convert integer    
    //                           operands to real.
    //
    //      pType1 : ptr to type of first  operand
    //      pType2 : ptr to type of second operand
    //--------------------------------------------------------------
    public void EmitPromoteToReal ( TType pType1, TType pType2 )
    {
        if (pType2 == pIntegerType)
        { // xxx_1 integer_2
            {
                Operator(TInstruction.Call);
                pAsmBuffer.Put('\t');
                NameLit(DefineConstants.FloatConvert);
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
            {
                Operator(TInstruction.Push);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Dx);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Push);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.PutLine();
            }; // xxx_1 real_2
        }

        if (pType1 == pIntegerType)
        { // integer_1 real_2
            {
                Operator(TInstruction.Pop);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Pop);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Dx);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Pop);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Bx);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Push);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Dx);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Push);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Push);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Bx);
                pAsmBuffer.PutLine();
            }; // real_2 integer_1

            {
                Operator(TInstruction.Call);
                pAsmBuffer.Put('\t');
                NameLit(DefineConstants.FloatConvert);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Add);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Sp);
                pAsmBuffer.Put(',');
                IntegerLit(2);
                pAsmBuffer.PutLine();
            }; // real_2 real_1

            {
                Operator(TInstruction.Pop);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Bx);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Pop);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Cx);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Push);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Dx);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Push);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Push);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Cx);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Push);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Bx);
                pAsmBuffer.PutLine();
            }; // real_1 real_2
        }
    }
}