//  *************************************************************
//  *                                                           *
//  *   C O D E   G E N E R A T O R   (Routines)                *
//  *                                                           *
//  *   Generating and emit assembly code for declared          *
//  *   procedures and functions.                               *
//  *                                                           *
//  *   CLASSES: TCodeGenerator                                 *
//  *                                                           *
//  *   FILE:    prog13-1/emitrtn.cpp                           *
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
    //  EmitProgramPrologue         Emit the program prologue.
    //--------------------------------------------------------------
    public void EmitProgramPrologue ()
    {
        PutLine("\tDOSSEG");
        PutLine("\t.MODEL  small");
        PutLine("\t.STACK  1024");
        PutLine();
        PutLine("\t.CODE");
        PutLine();
        PutLine("\tPUBLIC\t_PascalMain");
        PutLine("\tINCLUDE\tpasextrn.inc");
        PutLine();

        //--Equates for stack frame components.
        AsmText = String.Format("{0}\t\tEQU\t<WORD PTR [bp+4]>", StaticLink);
        PutLine();
        AsmText = String.Format("{0}\t\tEQU\t<WORD PTR [bp-4]>", ReturnValue);
        PutLine();
        AsmText = String.Format("{0}\tEQU\t<WORD PTR [bp-2]>", HighReturnValue);
        PutLine();
    }

    //--------------------------------------------------------------
    //  EmitProgramEpilogue         Emit the program epilogue.
    //--------------------------------------------------------------
    public void EmitProgramEpilogue ( TSymtabNode pProgramId )
    {
        TSymtabNode pId;
        TType pType;

        PutLine();
        PutLine("\t.DATA");
        PutLine();

        //--Emit declarations for the program's global variables.
        for (pId = pProgramId.defn.routine.locals.pVariableIds; pId != null; pId = pId.next)
        {
            AsmText = String.Format("{0}_{1:D3}\t", pId.String(), pId.labelIndex);
            Advance();

            pType = pId.pType;
            if (pType == Globals.pCharType)
                AsmText = "DB\t0";
            else if (pType == Globals.pRealType)
                AsmText = "DD\t0.0";
            else if (!pType.IsScalar())
                AsmText = String.Format("DB\t{0:D} DUP(0)", pType.size);
            else
                AsmText = "DW\t0";

            PutLine();
        }

        //--Emit declarations for the program's floating point literals.
        for (pId = pFloatLitList; pId != null; pId = pId.next)
        {
            AsmText = String.Format("{0}_{1:D3}\tDD\t{2:e}", FloatLabelPrefix, pId.labelIndex, pId.defn.constant.value.real);
            PutLine();
        }

        //--Emit declarations for the program's string literals.
        for (pId = pStringLitList; pId != null; pId = pId.next)
        {
            int i;
            var pString = pId.String();
            int length = pString.Length - 2; // don't count quotes

            AsmText = String.Format("{0}_{1:D3}\tDB\t\"", StringLabelPrefix, pId.labelIndex);
            Advance();

            for (i = 1; i <= length; ++i)
                Put(pString[i]);
            Put('\"');
            PutLine();
        }

        PutLine();
        AsmText = "\tEND";
        PutLine();
    }

    //--------------------------------------------------------------
    //  EmitMain            Emit code for the main routine.
    //--------------------------------------------------------------
    public void EmitMain ( TSymtabNode pMainId )
    {
        TSymtabNode pRtnId;

        EmitProgramHeaderComment(pMainId);
        EmitVarDeclComment(pMainId.defn.routine.locals.pVariableIds);

        //--Emit code for nested subroutines.
        for (pRtnId = pMainId.defn.routine.locals.pRoutineIds; pRtnId != null; pRtnId = pRtnId.next)
            EmitRoutine(pRtnId);

        //--Switch to main's intermediate code and emit code
        //--for its compound statement.
        pIcode = pMainId.defn.routine.pIcode;
        Globals.currentNestingLevel = 1;
        EmitMainPrologue();
        EmitCompound();
        EmitMainEpilogue();
    }

    //--------------------------------------------------------------
    //  EmitMainPrologue    Emit the prologue for the main routine.
    //--------------------------------------------------------------
    public void EmitMainPrologue ()
    {
        PutLine();
        PutLine("_PascalMain\tPROC");
        PutLine();

        {
            Operator(TInstruction.Push);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Bp);
            pAsmBuffer.PutLine();
        }; // dynamic link
        {
            Operator(TInstruction.Mov);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Bp);
            pAsmBuffer.Put(',');
            Reg(TRegister.Sp);
            pAsmBuffer.PutLine();
        }; // new stack frame base
    }

    //--------------------------------------------------------------
    //  EmitMainEpilogue    Emit the epilogue for the main routine.
    //--------------------------------------------------------------
    public void EmitMainEpilogue ()
    {
        PutLine();

        {
            Operator(TInstruction.Pop);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Bp);
            pAsmBuffer.PutLine();
        }; // restore caller's stack frame
        {
            Operator(TInstruction.Ret);
            pAsmBuffer.PutLine();
        }; // return

        PutLine();
        PutLine("_PascalMain\tENDP");
    }

    //--------------------------------------------------------------
    //  EmitRoutine         Emit code for a procedure or function.
    //--------------------------------------------------------------
    public void EmitRoutine ( TSymtabNode pRoutineId )
    {
        TSymtabNode pRtnId;

        EmitSubroutineHeaderComment(pRoutineId);

        //--Emit code for the parameters and local variables.
        EmitDeclarations(pRoutineId);

        //--Emit code for nested subroutines.
        for (pRtnId = pRoutineId.defn.routine.locals.pRoutineIds; pRtnId != null; pRtnId = pRtnId.next)
            EmitRoutine(pRtnId);

        //--Switch to the routine's intermediate code and emit code
        //--for its compound statement.
        pIcode = pRoutineId.defn.routine.pIcode;
        Globals.currentNestingLevel = pRoutineId.level + 1; // level of locals
        EmitRoutinePrologue(pRoutineId);
        EmitCompound();
        EmitRoutineEpilogue(pRoutineId);
    }

    //--------------------------------------------------------------
    //  EmitRoutinePrologue         Emit the prologue for a
    //                              procedure or function.
    //--------------------------------------------------------------
    public void EmitRoutinePrologue ( TSymtabNode pRoutineId )
    {
        PutLine();
        AsmText = String.Format("{0}_{1:D3}\tPROC", pRoutineId.String(), pRoutineId.labelIndex);
        PutLine();
        PutLine();

        {
            Operator(TInstruction.Push);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Bp);
            pAsmBuffer.PutLine();
        }; // dynamic link
        {
            Operator(TInstruction.Mov);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Bp);
            pAsmBuffer.Put(',');
            Reg(TRegister.Sp);
            pAsmBuffer.PutLine();
        }; // new stack frame base

        //--Allocate stack space for a function's return value.
        if (pRoutineId.defn.how == TDefnCode.DcFunction)
        {
            {
                Operator(TInstruction.Sub);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Sp);
                pAsmBuffer.Put(',');
                IntegerLit(4);
                pAsmBuffer.PutLine();
            };
        }

        //--Allocate stack space for the local variables.
        if (pRoutineId.defn.routine.totalLocalSize > 0)
        {
            {
                Operator(TInstruction.Sub);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Sp);
                pAsmBuffer.Put(',');
                IntegerLit(pRoutineId.defn.routine.totalLocalSize);
                pAsmBuffer.PutLine();
            };
        }
    }

    //--------------------------------------------------------------
    //  EmitRoutineEpilogue         Emit the epilogue for a
    //                              procedure or function.
    //--------------------------------------------------------------
    public void EmitRoutineEpilogue ( TSymtabNode pRoutineId )
    {
        PutLine();

        //--Load a function's return value into the ax or dx:ax registers.
        if (pRoutineId.defn.how == TDefnCode.DcFunction)
        {
            {
                Operator(TInstruction.Mov);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.Put(',');
                NameLit(ReturnValue);
                pAsmBuffer.PutLine();
            };
            if (pRoutineId.pType == Globals.pRealType)
            {
                {
                    Operator(TInstruction.Mov);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Dx);
                    pAsmBuffer.Put(',');
                    NameLit(HighReturnValue);
                    pAsmBuffer.PutLine();
                };
            }
        }

        {
            Operator(TInstruction.Mov);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Sp);
            pAsmBuffer.Put(',');
            Reg(TRegister.Bp);
            pAsmBuffer.PutLine();
        }; // cut back to caller's stack
        {
            Operator(TInstruction.Pop);
            pAsmBuffer.Put('\t');
            Reg(TRegister.Bp);
            pAsmBuffer.PutLine();
        }; // restore caller's stack frame

        {
            Operator(TInstruction.Ret);
            pAsmBuffer.Put('\t');
            IntegerLit(pRoutineId.defn.routine.totalParmSize + 2);
            pAsmBuffer.PutLine();
        };
        // return and cut back stack

        PutLine();
        AsmText = String.Format("{0}_{1:D3}\tENDP", pRoutineId.String(), pRoutineId.labelIndex);
        PutLine();
    }

    //--------------------------------------------------------------
    //  EmitSubroutineCall          Emit code for a call to a
    //                              procedure or a function.
    //
    //      pRoutineId : ptr to the subroutine name's symtab node
    //
    //  Return: ptr to the call's type object
    //--------------------------------------------------------------
    public TType EmitSubroutineCall ( TSymtabNode pRoutineId )
    {
        return pRoutineId.defn.routine.which == TRoutineCode.RcDeclared ? EmitDeclaredSubroutineCall(pRoutineId) : EmitStandardSubroutineCall(pRoutineId);
    }

    //--------------------------------------------------------------
    //  EmitDeclaredSubroutineCall   Emit code for a call to a
    //                               declared procedure or function.
    //
    //      pRoutineId : ptr to the subroutine name's symtab node
    //
    //  Return: ptr to the call's type object
    //--------------------------------------------------------------
    public TType EmitDeclaredSubroutineCall ( TSymtabNode pRoutineId )
    {
        int oldLevel = Globals.currentNestingLevel; // level of caller
        int newLevel = pRoutineId.level + 1; // level of callee's locals

        //--Emit code to push the actual parameter values onto the stack.
        GetToken();
        if (token == TTokenCode.TcLParen)
        {
            EmitActualParameters(pRoutineId);
            GetToken();
        }

        //--Push the static link onto the stack.
        if (newLevel == oldLevel + 1)
        {

            //--Calling a routine nested within the caller:
            //--Push pointer to caller's stack frame.
            {
                Operator(TInstruction.Push);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Bp);
                pAsmBuffer.PutLine();
            };
        } else if (newLevel == oldLevel)
        {

            //--Calling another routine at the same level:
            //--Push pointer to stack frame of common parent.
            {
                Operator(TInstruction.Push);
                pAsmBuffer.Put('\t');
                NameLit(StaticLink);
                pAsmBuffer.PutLine();
            };
        } else
        { // newLevel < oldLevel

            //--Calling a routine at a lesser level (nested less deeply):
            //--Push pointer to stack frame of nearest common ancestor
            //--(the callee's parent).
            EmitAdjustBP(newLevel - 1);
            {
                Operator(TInstruction.Push);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Bp);
                pAsmBuffer.PutLine();
            };
            EmitRestoreBP(newLevel - 1);
        }

        {
            Operator(TInstruction.Call);
            pAsmBuffer.Put('\t');
            TaggedName(pRoutineId);
            pAsmBuffer.PutLine();
        };

        return pRoutineId.pType;
    }

    //--------------------------------------------------------------
    //  EmitActualParameters    Emit code for the actual parameters
    //                          of a declared subroutine call.
    //
    //      pRoutineId : ptr to the subroutine name's symtab node
    //--------------------------------------------------------------
    public void EmitActualParameters ( TSymtabNode pRoutineId )
    {
        TSymtabNode pFormalId; // ptr to formal parm's symtab node

        //--Loop to emit code for each actual parameter.
        for (pFormalId = pRoutineId.defn.routine.locals.pParmIds; pFormalId != null; pFormalId = pFormalId.next)
        {

            TType pFormalType = pFormalId.pType;
            GetToken();

            //--VAR parameter: EmitVariable will leave the actual
            //--               parameter's addresss on top of the stack.
            if (pFormalId.defn.how == TDefnCode.DcVarParm)
                EmitVariable(pNode, true);

            //--Value parameter: Emit code to load a scalar value into
            //--                 ax or dx:ax, or push an array or record
            //--                 address onto the stack.
            else
            {
                TType pActualType = EmitExpression();

                if (pFormalType == Globals.pRealType)
                {

                    //--Real formal parm
                    if (pActualType == Globals.pIntegerType)
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
                } else if (!pActualType.IsScalar())
                {

                    //--Block move onto the stack.  Round the next offset
                    //--up to an even number.
                    int size = pActualType.size;
                    int offset = (size & 1) != 0 ? size + 1 : size;

                    {
                        Operator(TInstruction.Cld);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Pop);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Si);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Sub);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Sp);
                        pAsmBuffer.Put(',');
                        IntegerLit(offset);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Mov);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Di);
                        pAsmBuffer.Put(',');
                        Reg(TRegister.Sp);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Mov);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Cx);
                        pAsmBuffer.Put(',');
                        IntegerLit(size);
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
                        Operator(TInstruction.RepMovsb);
                        pAsmBuffer.PutLine();
                    };
                } else
                {
                    {
                        Operator(TInstruction.Push);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Ax);
                        pAsmBuffer.PutLine();
                    };
                }
            }
        }
    }
}