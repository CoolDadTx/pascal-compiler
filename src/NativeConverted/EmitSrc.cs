//  *************************************************************
//  *                                                           *
//  *   E M I T   S O U R C E   L I N E S                       *
//  *                                                           *
//  *   Emit source lines as comments in the assembly listing.  *
//  *                                                           *
//  *   CLASSES: TCodeGenerator                                 *
//  *                                                           *
//  *   FILE:    prog13-1/emitsrc.cpp                           *
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
    //  StartComment    Start a new comment with a line number.
    //
    //      pString : ptr to the string to append
    //--------------------------------------------------------------
    public void StartComment ( int n )
    {
        Reset();
        AsmText = String.Format("; {{{0:D}}} ", n);
        Advance();
    }

    private void StartComment ()
    {
        Reset();
        PutComment("; ");
    }

    private void StartComment ( string pString )
    {
        StartComment();
        PutComment(pString);
    }


    //--------------------------------------------------------------
    //  PutComment      Append a string to the assembly comment if
    //                  it fits.  If not, emit the current comemnt
    //                  and append the string to the next comment.
    //
    //      pString : ptr to the string to append
    //--------------------------------------------------------------
    public void PutComment ( string pString )
    {
        int length = pString.Length;

        //--Start a new comment if the current one is full.
        if (!pAsmBuffer.Fit(length))
        {
            PutLine();
            StartComment();
        }

        AsmText = pString;
        Advance();
    }

    //--------------------------------------------------------------
    //  EmitProgramHeaderComment    Emit the program header as a
    //                              comment.
    //
    //      pProgramId : ptr to the program id's symbol table node
    //--------------------------------------------------------------
    public void EmitProgramHeaderComment ( TSymtabNode pProgramId )
    {
        PutLine();
        StartComment("PROGRAM ");
        PutComment(pProgramId.String()); // program name

        //--Emit the program's parameter list.
        TSymtabNode pParmId = pProgramId.defn.routine.locals.pParmIds;
        if (pParmId != null)
        {
            PutComment(" (");

            //--Loop to emit each parameter.
            do
            {
                PutComment(pParmId.String());
                pParmId = pParmId.next;
                if (pParmId != null)
                    PutComment(", ");
            } while (pParmId != null);

            PutComment(")");
        }

        PutLine();
    }

    //--------------------------------------------------------------
    //  EmitSubroutineHeaderComment     Emit a subroutine header as
    //                                  a comment.
    //
    //      pRoutineId : ptr to the subroutine id's symtab node
    //--------------------------------------------------------------
    public void EmitSubroutineHeaderComment ( TSymtabNode pRoutineId )
    {
        PutLine();
        StartComment(pRoutineId.defn.how == TDefnCode.DcProcedure ? "PROCEDURE " : "FUNCTION ");
        //--Emit the procedure or function name
        //--followed by the formal parameter list.
        PutComment(pRoutineId.String());
        EmitSubroutineFormalsComment(pRoutineId.defn.routine.locals.pParmIds);

        //--Emit a function's return type.
        if (pRoutineId.defn.how == TDefnCode.DcFunction)
        {
            PutComment(" : ");
            PutComment(pRoutineId.pType.pTypeId.String());
        }

        PutLine();
    }

    //--------------------------------------------------------------
    //  EmitSubroutineFormalsComment    Emit a formal parameter list
    //                                  as a comment.
    //
    //      pParmId : ptr to the head of the formal parm id list
    //--------------------------------------------------------------
    public void EmitSubroutineFormalsComment ( TSymtabNode pParmId )
    {
        if (pParmId == null)
            return;

        PutComment(" (");

        //--Loop to emit each sublist of parameters with
        //--common definition and type.
        do
        {
            TDefnCode commonDefn = pParmId.defn.how; // common defn
            TType pCommonType = pParmId.pType; // common type
            bool doneFlag; // true if sublist done, false if not

            if (commonDefn == TDefnCode.DcVarParm)
                PutComment("VAR ");

            //--Loop to emit the parms in the sublist.
            do
            {
                PutComment(pParmId.String());

                pParmId = pParmId.next;
                doneFlag = (pParmId == null) || (commonDefn != pParmId.defn.how) || (pCommonType != pParmId.pType);
                if (!doneFlag)
                    PutComment(", ");
            } while (!doneFlag);

            //--Print the sublist's common type.
            PutComment(" : ");
            PutComment(pCommonType.pTypeId.String());

            if (pParmId != null)
                PutComment("; ");
        } while (pParmId != null);

        PutComment(")");
    }

    //--------------------------------------------------------------
    //  EmitVarDeclComment      Emit variable declarations as
    //                          comments.
    //
    //      pVarId : ptr to the head of the variable id list
    //--------------------------------------------------------------
    public void EmitVarDeclComment ( TSymtabNode pVarId )
    {
        TType pCommonType; // ptr to common type

        if (pVarId == null)
            return;
        pCommonType = pVarId.pType;

        PutLine();
        StartComment("VAR");
        PutLine();
        StartComment();

        //--Loop to print sublists of variables with a common type.
        do
        {
            PutComment(pVarId.String());
            pVarId = pVarId.next;

            if (pVarId != null && (pVarId.pType == pCommonType))
                PutComment(", ");
            else
            {

                //--End of sublist:  Print the common type and begin
                //--                 a new sublist.
                PutComment(" : ");
                EmitTypeSpecComment(pCommonType);
                PutLine();

                if (pVarId != null)
                {
                    pCommonType = pVarId.pType;
                    StartComment();
                }
            }
        } while (pVarId != null);
    }

    //--------------------------------------------------------------
    //  EmitTypeSpecComment     Emit a type specification as a
    //                          comment.
    //
    //      pType : ptr to the type object
    //--------------------------------------------------------------
    public void EmitTypeSpecComment ( TType pType )
    {
        //--If named type, emit the name, else emit "..."
        PutComment(pType.pTypeId != null ? pType.pTypeId.String() : "...");
    }

    //--------------------------------------------------------------
    //  EmitStmtComment         Emit a statement as a comment.
    //--------------------------------------------------------------
    public void EmitStmtComment ()
    {
        SaveState(); // save icode state
        StartComment(Globals.currentLineNumber);

        switch (token)
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

    //--------------------------------------------------------------
    //  EmitAsgnOrCallComment   Emit an assignment statement or a
    //                          procedure call as a comment.
    //--------------------------------------------------------------
    public void EmitAsgnOrCallComment ()
    {
        EmitIdComment();

        if (token == TTokenCode.TcColonEqual)
        {
            PutComment(" := ");

            GetToken();
            EmitExprComment();
        }

        PutLine();
    }

    //--------------------------------------------------------------
    //  EmitREPEATComment   Emit a REPEAT statement as a comment.
    //  EmitUNTILComment
    //--------------------------------------------------------------
    public void EmitREPEATComment ()
    {
        PutComment("REPEAT");
        PutLine();
    }
    public void EmitUNTILComment ()
    {
        PutComment("UNTIL ");

        GetToken();
        EmitExprComment();

        PutLine();
    }

    //--------------------------------------------------------------
    //  EmitWHILEComment    Emit a WHILE statement as a comment.
    //--------------------------------------------------------------
    public void EmitWHILEComment ()
    {
        PutComment("WHILE ");

        GetToken();
        GetLocationMarker();

        GetToken();
        EmitExprComment();

        PutComment(" DO");
        PutLine();
    }

    //--------------------------------------------------------------
    //  EmitIFComment       Emit an IF statement as a comment.
    //--------------------------------------------------------------
    public void EmitIFComment ()
    {
        PutComment("IF ");

        GetToken();
        GetLocationMarker();

        GetToken();
        EmitExprComment();

        PutLine();
    }

    //--------------------------------------------------------------
    //  EmitFORComment      Emit a FOR statement as a comment.
    //--------------------------------------------------------------
    public void EmitFORComment ()
    {
        PutComment("FOR ");

        GetToken();
        GetLocationMarker();

        GetToken();
        EmitIdComment();
        PutComment(" := ");

        GetToken();
        EmitExprComment();
        PutComment(token == TTokenCode.TcTO ? " TO " : " DOWNTO ");

        GetToken();
        EmitExprComment();

        PutComment(" DO");
        PutLine();
    }

    //--------------------------------------------------------------
    //  EmitCASEComment     Emit a CASE statement as a comment.
    //--------------------------------------------------------------
    public void EmitCASEComment ()
    {
        PutComment("CASE ");

        GetToken();
        GetLocationMarker();
        GetToken();
        GetLocationMarker();

        GetToken();
        EmitExprComment();

        PutComment(" OF ");
        PutLine();
    }

    //--------------------------------------------------------------
    //  EmitExprComment         Emit an expression as a comment.
    //--------------------------------------------------------------
    public void EmitExprComment ()
    {
        var doneFlag = false; // true if done with expression, false if not

        //--Loop over the entire expression.
        do
        {
            switch (token)
            {
                case TTokenCode.TcIdentifier:
                EmitIdComment();
                break;

                case TTokenCode.TcNumber:
                PutComment(pToken.String);
                GetToken();
                break;

                case TTokenCode.TcString:
                PutComment(pToken.String);
                GetToken();
                break;

                case TTokenCode.TcPlus:
                PutComment(" + ");
                GetToken();
                break;
                case TTokenCode.TcMinus:
                PutComment(" - ");
                GetToken();
                break;
                case TTokenCode.TcStar:
                PutComment("*");
                GetToken();
                break;
                case TTokenCode.TcSlash:
                PutComment("/");
                GetToken();
                break;
                case TTokenCode.TcDIV:
                PutComment(" DIV ");
                GetToken();
                break;
                case TTokenCode.TcMOD:
                PutComment(" MOD ");
                GetToken();
                break;
                case TTokenCode.TcAND:
                PutComment(" AND ");
                GetToken();
                break;
                case TTokenCode.TcOR:
                PutComment(" OR ");
                GetToken();
                break;
                case TTokenCode.TcEqual:
                PutComment(" = ");
                GetToken();
                break;
                case TTokenCode.TcNe:
                PutComment(" <> ");
                GetToken();
                break;
                case TTokenCode.TcLt:
                PutComment(" < ");
                GetToken();
                break;
                case TTokenCode.TcLe:
                PutComment(" <= ");
                GetToken();
                break;
                case TTokenCode.TcGt:
                PutComment(" > ");
                GetToken();
                break;
                case TTokenCode.TcGe:
                PutComment(" >= ");
                GetToken();
                break;
                case TTokenCode.TcNOT:
                PutComment("NOT ");
                GetToken();
                break;

                case TTokenCode.TcLParen:
                PutComment("(");
                GetToken();
                EmitExprComment();
                PutComment(")");
                GetToken();
                break;

                default:
                doneFlag = true;
                break;
            }
        } while (!doneFlag);
    }

    public void EmitIdComment ()
    {
        PutComment(pToken.String);
        GetToken();

        //--Loop to print any modifiers (subscripts, record fields,
        //--or actual parameter lists).
        while (TokenIn(token, tlIdModStart) != 0)
        {

            //--Record field.
            if (token == TTokenCode.TcPeriod)
            {
                PutComment(".");
                GetToken();
                EmitIdComment();
            }

            //--Subscripts or actual parameters.
            else
            {

                //--( or [
                PutComment(token == TTokenCode.TcLParen ? "(" : "[");
                GetToken();

                while (TokenIn(token, tlIdModEnd) == 0)
                {
                    EmitExprComment();

                    //--Write and writeln field width and precision.
                    while (token == TTokenCode.TcColon)
                    {
                        PutComment(":");
                        GetToken();
                        EmitExprComment();
                    }

                    if (token == TTokenCode.TcComma)
                    {
                        PutComment(", ");
                        GetToken();
                    }
                }

                //--) or ]
                PutComment(token == TTokenCode.TcRParen ? ")" : "]");
                GetToken();
            }
        }
    }
}