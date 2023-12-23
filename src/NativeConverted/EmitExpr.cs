//  *************************************************************
//  *                                                           *
//  *   C O D E   G E N E R A T O R   (Expressions)             *
//  *                                                           *
//  *   Generating and emit assembly code for expressions.      *
//  *                                                           *
//  *   CLASSES: TCodeGenerator                                 *
//  *                                                           *
//  *   FILE:    prog13-1/emitexpr.cpp                          *
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
    //  EmitExpression  Emit code for an expression (binary
    //                  relational operators = < > <> <= and >= ).
    //
    //  Return: ptr to expression's type object
    //--------------------------------------------------------------
    public TType EmitExpression ()
    {
        TType pOperand1Type; // ptr to first  operand's type
        TType pOperand2Type; // ptr to second operand's type
        TType pResultType; // ptr to result type
        TTokenCode op; // operator
        TInstruction jumpOpcode = default; // jump instruction opcode
        int jumpLabelIndex; // assembly jump label index

        //--Emit code for the first simple expression.
        pResultType = EmitSimpleExpression();

        //--If we now see a relational operator,
        //--emit code for the second simple expression.
        if (Globals.TokenIn(token, Globals.tlRelOps))
        {
            EmitPushOperand(pResultType);
            op = token;
            pOperand1Type = pResultType.Base();

            GetToken();
            pOperand2Type = EmitSimpleExpression().Base();

            //--Perform the operation, and push the resulting value
            //--onto the stack.
            if (((pOperand1Type == Globals.pIntegerType) && (pOperand2Type == Globals.pIntegerType)) 
                || ((pOperand1Type == Globals.pCharType) && (pOperand2Type == Globals.pCharType)) || (pOperand1Type.form == TFormCode.FcEnum))
            {

                //--integer <op> integer
                //--boolean <op> boolean
                //--char    <op> char
                //--enum    <op> enum
                //--Compare dx (operand 1) to ax (operand 2).
                {
                    Operator(TInstruction.Pop);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Dx);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.Cmp);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Dx);
                    pAsmBuffer.Put(',');
                    Reg(TRegister.Ax);
                    pAsmBuffer.PutLine();
                };
            } else if ((pOperand1Type == Globals.pRealType) || (pOperand2Type == Globals.pRealType))
            {

                //--real    <op> real
                //--real    <op> integer
                //--integer <op> real
                //--Convert the integer operand to real.
                //--Call _FloatCompare to do the comparison, which
                //--returns -1 (less), 0 (equal), or +1 (greater).
                EmitPushOperand(pOperand2Type);
                EmitPromoteToReal(pOperand1Type, pOperand2Type);

                {
                    Operator(TInstruction.Call);
                    pAsmBuffer.Put('\t');
                    NameLit(FloatCompare);
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
                {
                    Operator(TInstruction.Cmp);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Ax);
                    pAsmBuffer.Put(',');
                    IntegerLit(0);
                    pAsmBuffer.PutLine();
                };
            } else
            {

                //--string <op> string
                //--Compare the string pointed to by si (operand 1)
                //--to the string pointed to by di (operand 2).
                {
                    Operator(TInstruction.Pop);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Di);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.Pop);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Si);
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
                    Operator(TInstruction.Mov);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Cx);
                    pAsmBuffer.Put(',');
                    IntegerLit(pOperand1Type.array.elmtCount);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.RepeCmpsb);
                    pAsmBuffer.PutLine();
                };
            }

            {
                Operator(TInstruction.Mov);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.Put(',');
                IntegerLit(1);
                pAsmBuffer.PutLine();
            }; // default: load 1

            switch (op)
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
            
            jumpLabelIndex = ++Globals.asmLabelIndex;
            {
                Operator(jumpOpcode);
                pAsmBuffer.Put('\t');
                Label(StmtLabelPrefix, jumpLabelIndex);
                pAsmBuffer.PutLine();
            };

            {
                Operator(TInstruction.Sub);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.Put(',');
                Reg(TRegister.Ax);
                pAsmBuffer.PutLine();
            }; // load 0 if false
            EmitStatementLabel(jumpLabelIndex);

            pResultType = Globals.pBooleanType;
        }

        return pResultType;
    }

    //--------------------------------------------------------------
    //  EmitSimpleExpression    Emit code for a simple expression
    //                          (unary operators + or -
    //                          and binary operators + -
    //                          and OR).
    //
    //  Return: ptr to expression's type object
    //--------------------------------------------------------------
    public TType EmitSimpleExpression ()
    {
        TType pOperandType; // ptr to operand's type
        TType pResultType; // ptr to result type
        TTokenCode op; // operator
        TTokenCode unaryOp = TTokenCode.TcPlus; // unary operator

        //--Unary + or -
        if (Globals.TokenIn(token, Globals.tlUnaryOps))
        {
            unaryOp = token;
            GetToken();
        }

        //--Emit code for the first term.
        pResultType = EmitTerm();

        //--If there was a unary operator, negate in integer value in ax
        //--with the neg instruction, or negate a real value in dx:ax
        //--by calling _FloatNegate.
        if (unaryOp == TTokenCode.TcMinus)
        {
            if (pResultType.Base() == Globals.pIntegerType)
            {
                Operator(TInstruction.Neg);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.PutLine();
            } else if (pResultType == Globals.pRealType)
            {
                EmitPushOperand(pResultType);
                {
                    Operator(TInstruction.Call);
                    pAsmBuffer.Put('\t');
                    NameLit(FloatNegate);
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
        }

        //--Loop to execute subsequent additive operators and terms.
        while (Globals.TokenIn(token, Globals.tlAddOps))
        {
            op = token;
            pResultType = pResultType.Base();
            EmitPushOperand(pResultType);

            GetToken();
            pOperandType = EmitTerm().Base();

            //--Perform the operation, and push the resulting value
            //--onto the stack.
            if (op == TTokenCode.TcOR)
            {

                //--boolean OR boolean => boolean
                //--ax = ax OR dx
                {
                    Operator(TInstruction.Pop);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Dx);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.Or);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Ax);
                    pAsmBuffer.Put(',');
                    Reg(TRegister.Dx);
                    pAsmBuffer.PutLine();
                };
                pResultType = Globals.pBooleanType;
            } else if ((pResultType == Globals.pIntegerType) && (pOperandType == Globals.pIntegerType))
            {

                {
                    //--integer +|- integer => integer
                    Operator(TInstruction.Pop);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Dx);
                    pAsmBuffer.PutLine();
                };
                if (op == TTokenCode.TcPlus)
                {
                    Operator(TInstruction.Add);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Ax);
                    pAsmBuffer.Put(',');
                    Reg(TRegister.Dx);
                    pAsmBuffer.PutLine();
                } else
                {
                    {
                        Operator(TInstruction.Sub);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Dx);
                        pAsmBuffer.Put(',');
                        Reg(TRegister.Ax);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Mov);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Ax);
                        pAsmBuffer.Put(',');
                        Reg(TRegister.Dx);
                        pAsmBuffer.PutLine();
                    };
                }
                pResultType = Globals.pIntegerType;
            } else
            {

                //--real    +|- real    => real
                //--real    +|- integer => real
                //--integer +|- real    => real
                //--Convert the integer operand to real and then
                //--call _FloatAdd or _FloatSubtract.
                EmitPushOperand(pOperandType);
                EmitPromoteToReal(pResultType, pOperandType);

                {
                    Operator(TInstruction.Call);
                    pAsmBuffer.Put('\t');
                    NameLit(op == TTokenCode.TcPlus ? FloatAdd : FloatSubtract);
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
                pResultType = Globals.pRealType;
            }
        }

        return pResultType;
    }

    //--------------------------------------------------------------
    //  EmitTerm            Emit code for a term (binary operators
    //                      * / DIV tcMOD and AND).
    //
    //  Return: ptr to term's type object
    //--------------------------------------------------------------
    public TType EmitTerm ()
    {
        TType pOperandType; // ptr to operand's type
        TType pResultType; // ptr to result type
        TTokenCode op; // operator

        //--Emit code for the first factor.
        pResultType = EmitFactor();

        //--Loop to execute subsequent multiplicative operators and factors.
        while (Globals.TokenIn(token, Globals.tlMulOps))
        {
            op = token;
            pResultType = pResultType.Base();
            EmitPushOperand(pResultType);

            GetToken();
            pOperandType = EmitFactor().Base();

            //--Perform the operation, and push the resulting value
            //--onto the stack.
            switch (op)
            {

                case TTokenCode.TcAND:
                {

                    {
                        //--boolean AND boolean => boolean
                        Operator(TInstruction.Pop);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Dx);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.And);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Ax);
                        pAsmBuffer.Put(',');
                        Reg(TRegister.Dx);
                        pAsmBuffer.PutLine();
                    };
                    pResultType = Globals.pBooleanType;
                    break;
                }

                case TTokenCode.TcStar:

                if ((pResultType == Globals.pIntegerType) && (pOperandType == Globals.pIntegerType))
                {

                    //--integer * integer => integer
                    //--ax = ax*dx
                    {
                        Operator(TInstruction.Pop);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Dx);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Imul);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Dx);
                        pAsmBuffer.PutLine();
                    };
                    pResultType = Globals.pIntegerType;
                } else
                {

                    //--real    * real    => real
                    //--real    * integer => real
                    //--integer * real    => real
                    //--Convert the integer operand to real
                    //--and then call _FloatMultiply, which
                    //--leaves the value in dx:ax.
                    EmitPushOperand(pOperandType);
                    EmitPromoteToReal(pResultType, pOperandType);

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
                    pResultType = Globals.pRealType;
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
                    EmitPushOperand(pOperandType);
                    EmitPromoteToReal(pResultType, pOperandType);

                    {
                        Operator(TInstruction.Call);
                        pAsmBuffer.Put('\t');
                        NameLit(FloatDivide);
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
                    pResultType = Globals.pRealType;
                    break;
                }

                case TTokenCode.TcDIV:
                case TTokenCode.TcMOD:
                {

                    //--integer DIV|MOD integer => integer
                    //--ax = ax IDIV cx
                    {
                        Operator(TInstruction.Mov);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Cx);
                        pAsmBuffer.Put(',');
                        Reg(TRegister.Ax);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Pop);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Ax);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Sub);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Dx);
                        pAsmBuffer.Put(',');
                        Reg(TRegister.Dx);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Idiv);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Cx);
                        pAsmBuffer.PutLine();
                    };
                    if (op == TTokenCode.TcMOD)
                    {
                        Operator(TInstruction.Mov);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Ax);
                        pAsmBuffer.Put(',');
                        Reg(TRegister.Dx);
                        pAsmBuffer.PutLine();
                    };
                    pResultType = Globals.pIntegerType;
                    break;
                }
            }
        }

        return pResultType;
    }

    //--------------------------------------------------------------
    //  EmitFactor      Emit code for a factor (identifier, number,
    //                  string, NOT <factor>, or parenthesized
    //                  subexpression).  An identifier can be
    //                  a function, constant, or variable.
    //
    //  Return: ptr to factor's type object
    //--------------------------------------------------------------
    public TType EmitFactor ()
    {
        TType pResultType = Globals.pDummyType; // ptr to result type

        switch (token)
        {

            case TTokenCode.TcIdentifier:
            {
                switch (pNode.defn.how)
                {

                    case TDefnCode.DcFunction:
                    pResultType = EmitSubroutineCall(pNode);
                    break;

                    case TDefnCode.DcConstant:
                    pResultType = EmitConstant(pNode);
                    break;

                    default:
                    pResultType = EmitVariable(pNode, false);
                    break;
                }
                break;
            }

            case TTokenCode.TcNumber:
            {

                //--Push the number's integer or real value onto the stack.
                if (pNode.pType == Globals.pIntegerType)
                {

                    {
                        //--ax = value
                        Operator(TInstruction.Mov);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Ax);
                        pAsmBuffer.Put(',');
                        IntegerLit(pNode.defn.constant.value.integer);
                        pAsmBuffer.PutLine();
                    };
                    pResultType = Globals.pIntegerType;
                } else
                {

                    //--dx:ax = value
                    EmitLoadFloatLit(pNode);
                    pResultType = Globals.pRealType;
                }

                GetToken();
                break;
            }

            case TTokenCode.TcString:
            {

                //--Push either a character or a string address onto the
                //--runtime stack, depending on the string length.
                int length = Convert.ToString(pNode.String()).Length - 2; // skip quotes
                if (length == 1)
                {

                    //--Character
                    //--ah = 0
                    //--al = value
                    {
                        Operator(TInstruction.Mov);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Ax);
                        pAsmBuffer.Put(',');
                        CharLit(pNode.defn.constant.value.character);
                        pAsmBuffer.PutLine();
                    };
                    pResultType = Globals.pCharType;
                } else
                {

                    //--String
                    //--ax = string address
                    EmitPushStringLit(pNode);
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
                Operator(TInstruction.Xor);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.Put(',');
                IntegerLit(1);
                pAsmBuffer.PutLine();
            };
            pResultType = Globals.pBooleanType;
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

    //--------------------------------------------------------------
    //  EmitConstant        Emit code to load a scalar constant into
    //                      ax or dx:ax, or to push a string address
    //                      onto the runtime stack.
    //
    //      pId : ptr to constant identifier's symbol table node
    //
    //  Return: ptr to constant's type object
    //--------------------------------------------------------------
    public TType EmitConstant ( TSymtabNode pId )
    {
        TType pType = pId.pType;

        if (pType == Globals.pRealType)

            //--Real: dx:ax = value
            EmitLoadFloatLit(pId);
        else if (pType == Globals.pCharType)
        {

            {
                //--Character: ax = value
                Operator(TInstruction.Mov);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.Put(',');
                CharLit(pId.defn.constant.value.character);
                pAsmBuffer.PutLine();
            };
        } else if (pType.form == TFormCode.FcArray)
        {

            //--String constant: push string address
            EmitPushStringLit(pId);
        } else
        {

            {
                //--Integer or enumeration: ax = value
                Operator(TInstruction.Mov);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.Put(',');
                IntegerLit(pId.defn.constant.value.integer);
                pAsmBuffer.PutLine();
            };
        }

        GetToken();
        return pType;
    }

    //--------------------------------------------------------------
    //  EmitVariable        Emit code to load a variable's value
    //                      ax or dx:ax, or push its address onto
    //                      the runtime stack.
    //
    //      pId         : ptr to variable's symbol table node
    //      addressFlag : true to push address, false to load value
    //
    //  Return: ptr to variable's type object
    //--------------------------------------------------------------
    public TType EmitVariable ( TSymtabNode pId, bool addressFlag )
    {
        TType pType = pId.pType;

        //--It's not a scalar, or addressFlag is true, push the
        //--data address onto the stack. Otherwise, load the
        //--data value into ax or dx:ax.
        if (addressFlag || !pType.IsScalar())
            EmitPushAddress(pId);
        else
            EmitLoadValue(pId);

        GetToken();

        //--If there are any subscripts and field designators,
        //--emit code to evaluate them and modify the address.
        if ((token == TTokenCode.TcLBracket) || (token == TTokenCode.TcPeriod))
        {
            var doneFlag = false;

            do
            {
                switch (token)
                {

                    case TTokenCode.TcLBracket:
                    pType = EmitSubscripts(pType);
                    break;

                    case TTokenCode.TcPeriod:
                    pType = EmitField();
                    break;

                    default:
                    doneFlag = true;
                    break;
                }
            } while (!doneFlag);

            //--If addresssFlag is false and the variable is scalar,
            //--pop the address off the top of the stack and use it
            //--to load the value into ax or dx:ax.
            if (!addressFlag && pType.IsScalar())
            {
                {
                    Operator(TInstruction.Pop);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Bx);
                    pAsmBuffer.PutLine();
                };
                if (pType == Globals.pRealType)
                {

                    {
                        //--Read: dx:ax = value
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
                        Reg(TRegister.Dx);
                        pAsmBuffer.Put(',');
                        HighDWordIndirect(TRegister.Bx);
                        pAsmBuffer.PutLine();
                    };
                } else if (pType.Base() == Globals.pCharType)
                {

                    {
                        //--Character: al = value
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
            }
        }

        return pType;
    }

    //--------------------------------------------------------------
    //  EmitSubscripts      Emit code for each subscript expression
    //                      to modify the data address at the top of
    //                      the runtime stack.
    //
    //      pType : ptr to array type object
    //
    //  Return: ptr to subscripted variable's type object
    //--------------------------------------------------------------
    public TType EmitSubscripts ( TType pType )
    {
        int minIndex;
        int elmtSize;

        //--Loop to executed subscript lists enclosed in brackets.
        while (token == TTokenCode.TcLBracket)
        {

            //--Loop to execute comma-separated subscript expressions
            //--within a subscript list.
            do
            {
                GetToken();
                EmitExpression();

                minIndex = pType.array.minIndex;
                elmtSize = pType.array.pElmtType.size;

                //--Convert the subscript into an offset by subtracting
                //--the minimum index from it and then multiplying the
                //--result by the element size.   Add the offset to the
                //--address at the top of the stack.
                if (minIndex != 0)
                {
                    Operator(TInstruction.Sub);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Ax);
                    pAsmBuffer.Put(',');
                    IntegerLit(minIndex);
                    pAsmBuffer.PutLine();
                };
                if (elmtSize > 1)
                {
                    {
                        Operator(TInstruction.Mov);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Dx);
                        pAsmBuffer.Put(',');
                        IntegerLit(elmtSize);
                        pAsmBuffer.PutLine();
                    };
                    {
                        Operator(TInstruction.Imul);
                        pAsmBuffer.Put('\t');
                        Reg(TRegister.Dx);
                        pAsmBuffer.PutLine();
                    };
                }
                {
                    Operator(TInstruction.Pop);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Dx);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.Add);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Dx);
                    pAsmBuffer.Put(',');
                    Reg(TRegister.Ax);
                    pAsmBuffer.PutLine();
                };
                {
                    Operator(TInstruction.Push);
                    pAsmBuffer.Put('\t');
                    Reg(TRegister.Dx);
                    pAsmBuffer.PutLine();
                };

                //--Prepare for another subscript in this list.
                if (token == TTokenCode.TcComma)
                    pType = pType.array.pElmtType;

            } while (token == TTokenCode.TcComma);

            //--Prepare for another subscript list.
            GetToken();
            if (token == TTokenCode.TcLBracket)
                pType = pType.array.pElmtType;
        }

        return pType.array.pElmtType;
    }

    //--------------------------------------------------------------
    //  EmitField   Emit code for a field designator to modify the
    //              data address at the top of the runtime stack.
    //
    //  Return: ptr to record field's type object
    //--------------------------------------------------------------
    public TType EmitField ()
    {
        GetToken();
        TSymtabNode pFieldId = pNode;
        int offset = pFieldId.defn.data.offset;

        //--Add the field's offset to the data address
        //--if the offset is greater than 0.
        if (offset > 0)
        {
            {
                Operator(TInstruction.Pop);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Add);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.Put(',');
                IntegerLit(offset);
                pAsmBuffer.PutLine();
            };
            {
                Operator(TInstruction.Push);
                pAsmBuffer.Put('\t');
                Reg(TRegister.Ax);
                pAsmBuffer.PutLine();
            };
        }

        GetToken();
        return pFieldId.pType;
    }
}