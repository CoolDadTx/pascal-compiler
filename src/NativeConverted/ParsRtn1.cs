//  *************************************************************
//  *                                                           *
//  *   P A R S E R   (Routines #1)                             *
//  *                                                           *
//  *   Parse programs, procedures, and functions.              *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog8-1/parsrtn1.cpp                           *
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
    //  ParseProgram        Parse a program:
    //
    //                          <program-header> ; <block> .
    //
    //  Return: ptr to program id's symbol table node
    //--------------------------------------------------------------
    public TSymtabNode ParseProgram ()
    {
        //--<program-header>
        TSymtabNode pProgramId = ParseProgramHeader();

        //-- ;
        Resync(tlHeaderFollow, tlDeclarationStart, tlStatementStart);
        if (token == TTokenCode.TcSemicolon)
            GetToken();
        else if (Globals.TokenIn(token, tlDeclarationStart) || Globals.TokenIn(token, tlStatementStart))
            Globals.Error(TErrorCode.ErrMissingSemicolon);

        //--<block>
        ParseBlock(pProgramId);
        pProgramId.defn.routine.pSymtab = symtabStack.ExitScope();

        //-- .
        Resync(tlProgramEnd);
        CondGetTokenAppend(TTokenCode.TcPeriod, TErrorCode.ErrMissingPeriod);

        return pProgramId;
    }

    //--------------------------------------------------------------
    //  ParseProgramHeader      Parse a program header:
    //
    //                              PROGRAM <id>
    //
    //                          or:
    //
    //                              PROGRAM <id> ( <id-list> )
    //
    //  Return: ptr to program id's symbol table node
    //--------------------------------------------------------------
    public TSymtabNode ParseProgramHeader ()
    {
        TSymtabNode pProgramId; // ptr to program id node

        //--PROGRAM
        CondGetToken(TTokenCode.TcPROGRAM, TErrorCode.ErrMissingPROGRAM);

        //--<id>
        if (token == TTokenCode.TcIdentifier)
        {
            pProgramId = EnterNewLocal(pToken.String, TDefnCode.DcProgram);
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
            TType.SetType(ref pProgramId.pType, Globals.pDummyType);
            GetToken();
        } else
        {
            Globals.Error(TErrorCode.ErrMissingIdentifier);
        }

        //-- ( or ;
        Resync(tlProgProcIdFollow, tlDeclarationStart, tlStatementStart);

        //--Enter the nesting level 1 and open a new scope for the program.
        symtabStack.EnterScope();

        //--Optional ( <id-list> )
        if (token == TTokenCode.TcLParen)
        {
            TSymtabNode pPrevParmId = null;

            //--Loop to parse a comma-separated identifier list.
            do
            {
                GetToken();
                if (token == TTokenCode.TcIdentifier)
                {
                    TSymtabNode pParmId = EnterNewLocal(pToken.String, TDefnCode.DcVarParm);
                    TType.SetType(ref pParmId.pType, Globals.pDummyType);
                    GetToken();

                    //--Link program parm id nodes together.
                    if (pPrevParmId == null)
                        pProgramId.defn.routine.locals.pParmIds = pPrevParmId = pParmId;
                    else
                    {
                        pPrevParmId.next = pParmId;
                        pPrevParmId = pParmId;
                    }
                } else
                {
                    Globals.Error(TErrorCode.ErrMissingIdentifier);
                }
            } while (token == TTokenCode.TcComma);

            //-- )
            Resync(tlFormalParmsFollow, tlDeclarationStart, tlStatementStart);
            CondGetToken(TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen);
        }

        return pProgramId;
    }

    //--------------------------------------------------------------
    //  ParseSubroutineDeclarations     Parse procedures and
    //                                  function declarations.
    //
    //      pRoutineId : ptr to symbol table node of the name of the
    //                   routine that contains the subroutines
    //--------------------------------------------------------------
    public void ParseSubroutineDeclarations ( TSymtabNode pRoutineId )
    {
        TSymtabNode pLastId = null; // ptr to last routine id node
                                    //   in local list

        //--Loop to parse procedure and function definitions.
        while (Globals.TokenIn(token, tlProcFuncStart))
        {
            TSymtabNode pRtnId = ParseSubroutine();

            //--Link the routine's local (nested) routine id nodes together.
            if (!pRoutineId.defn.routine.locals.pRoutineIds)
                pRoutineId.defn.routine.locals.pRoutineIds = pRtnId;
            else
                pLastId.next = pRtnId;
            pLastId = pRtnId;

            //-- ;
            Resync(tlDeclarationFollow, tlProcFuncStart, tlStatementStart);
            if (token == TTokenCode.TcSemicolon)
                GetToken();
            else if (Globals.TokenIn(token, tlProcFuncStart) || Globals.TokenIn(token, tlStatementStart))
                Globals.Error(TErrorCode.ErrMissingSemicolon);
        }
    }

    //--------------------------------------------------------------
    //  ParseSubroutine     Parse a subroutine:
    //
    //                          <subroutine-header> ; <block>
    //
    //                      or:
    //
    //                          <subroutine-header> ; forward
    //
    //  Return: ptr to subroutine id's symbol table node
    //--------------------------------------------------------------
    public TSymtabNode ParseSubroutine ()
    {
        //--<routine-header>
        TSymtabNode pRoutineId = (token == TTokenCode.TcPROCEDURE) ? ParseProcedureHeader() : ParseFunctionHeader();

        //-- ;
        Resync(tlHeaderFollow, tlDeclarationStart, tlStatementStart);
        if (token == TTokenCode.TcSemicolon)
            GetToken();
        else if (Globals.TokenIn(token, tlDeclarationStart) || Globals.TokenIn(token, tlStatementStart))
            Globals.Error(TErrorCode.ErrMissingSemicolon);

        //--<block> or forward
        if (String.Equals(pToken.String, "forward", StringComparison.OrdinalIgnoreCase))
        {
            pRoutineId.defn.routine.which = TRoutineCode.RcDeclared;
            ParseBlock(pRoutineId);
        } else
        {
            GetToken();
            pRoutineId.defn.routine.which = TRoutineCode.RcForward;
        }

        pRoutineId.defn.routine.pSymtab = symtabStack.ExitScope();
        return pRoutineId;
    }

    //--------------------------------------------------------------
    //  ParseProcedureHeader    Parse a procedure header:
    //
    //                              PROCEDURE <id>
    //
    //                          or:
    //
    //                              PROCEDURE <id> ( <parm-list> )
    //
    //  Return: ptr to procedure id's symbol table node
    //--------------------------------------------------------------
    public TSymtabNode ParseProcedureHeader ()
    {
        TSymtabNode pProcId; // ptr to procedure id node
        var forwardFlag = false; // true if forwarded, false if not

        GetToken();

        //--<id> : If the procedure id has already been declared in
        //--       this scope, it must have been a forward declaration.
        if (token == TTokenCode.TcIdentifier)
        {
            pProcId = SearchLocal(pToken.String);
            if (pProcId == null)
            {

                //--Not already declared.
                pProcId = EnterLocal(pToken.String, TDefnCode.DcProcedure);
                pProcId.defn.routine.totalLocalSize = 0;
                TType.SetType(ref pProcId.pType, pDummyType);
            } else if ((pProcId.defn.how == TDefnCode.DcProcedure) && (pProcId.defn.routine.which == TRoutineCode.RcForward))
            {

                //--Forwarded.
                forwardFlag = true;
            } else
            {
                Globals.Error(TErrorCode.ErrRedefinedIdentifier);
            }

            GetToken();
        } else
        {
            Globals.Error(TErrorCode.ErrMissingIdentifier);
        }

        //-- ( or ;
        Resync(tlProgProcIdFollow, tlDeclarationStart, tlStatementStart);

        //--Enter the next nesting level and open a new scope
        //--for the procedure.
        symtabStack.EnterScope();

        //--Optional ( <id-list> ) : If there was a forward declaration,
        //--                         there must not be a parameter list,
        //--                         but if there is, parse it anyway
        //--                         for error recovery.
        if (token == TTokenCode.TcLParen)
        {
            int parmCount; // count of formal parms
            int totalParmSize; // total byte size of all parms
            TSymtabNode pParmList = ParseFormalParmList(parmCount, totalParmSize);

            if (forwardFlag)
                Globals.Error(TErrorCode.ErrAlreadyForwarded);
            else
            {

                //--Not forwarded.
                pProcId.defn.routine.parmCount = parmCount;
                pProcId.defn.routine.totalParmSize = totalParmSize;
                pProcId.defn.routine.locals.pParmIds = pParmList;
            }
        } else if (!forwardFlag)
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

        TType.SetType(ref pProcId.pType, Globals.pDummyType);
        return pProcId;
    }

    //--------------------------------------------------------------
    //  ParseFunctionHeader     Parse a function header:
    //
    //                              FUNCTION <id>
    //
    //                          or:
    //
    //                              FUNCTION <id> : <type-id>
    //
    //                          or:
    //
    //                              FUNCTION <id> ( <parm-list> )
    //                                            : <type-id>
    //
    //  Return: ptr to function id's symbol table node
    //--------------------------------------------------------------
    public TSymtabNode ParseFunctionHeader ()
    {
        TSymtabNode pFuncId; // ptr to function id node
        var forwardFlag = false; // true if forwarded, false if not

        GetToken();

        //--<id> : If the function id has already been declared in
        //--       this scope, it must have been a forward declaration.
        if (token == TTokenCode.TcIdentifier)
        {
            pFuncId = SearchLocal(pToken.String);
            if (pFuncId == null)
            {

                //--Not already declared.
                pFuncId = EnterLocal(pToken.String, TDefnCode.DcFunction);
                pFuncId.defn.routine.totalLocalSize = 0;
            } else if ((pFuncId.defn.how == TDefnCode.DcFunction) && (pFuncId.defn.routine.which == TRoutineCode.RcForward))
            {

                //--Forwarded.
                forwardFlag = true;
            } else
            {
                Globals.Error(TErrorCode.ErrRedefinedIdentifier);
            }

            GetToken();
        } else
        {
            Globals.Error(TErrorCode.ErrMissingIdentifier);
        }

        //-- ( or : or ;
        Resync(tlFuncIdFollow, tlDeclarationStart, tlStatementStart);

        //--Enter the next nesting level and open a new scope
        //--for the function.
        symtabStack.EnterScope();

        //--Optional ( <id-list> ) : If there was a forward declaration,
        //--                         there must not be a parameter list,
        //--                         but if there is, parse it anyway
        //--                         for error recovery.
        if (token == TTokenCode.TcLParen)
        {
            int parmCount; // count of formal parms
            int totalParmSize; // total byte size of all parms
            TSymtabNode pParmList = ParseFormalParmList(parmCount, totalParmSize);

            if (forwardFlag)
                Globals.Error(TErrorCode.ErrAlreadyForwarded);
            else
            {

                //--Not forwarded.
                pFuncId.defn.routine.parmCount = parmCount;
                pFuncId.defn.routine.totalParmSize = totalParmSize;
                pFuncId.defn.routine.locals.pParmIds = pParmList;
            }
        } else if (!forwardFlag)
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
        if (!forwardFlag || (token == TTokenCode.TcColon))
        {
            CondGetToken(TTokenCode.TcColon, TErrorCode.ErrMissingColon);
            if (token == TTokenCode.TcIdentifier)
            {
                TSymtabNode pTypeId = Find(pToken.String);
                if (pTypeId.defn.how != TDefnCode.DcType)
                    Globals.Error(TErrorCode.ErrInvalidType);

                if (forwardFlag)
                    Globals.Error(TErrorCode.ErrAlreadyForwarded);
                else
                    TType.SetType(ref pFuncId.pType, pTypeId.pType);

                GetToken();
            } else
            {
                Globals.Error(TErrorCode.ErrMissingIdentifier);
                TType.SetType(ref pFuncId.pType, Globals.pDummyType);
            }
        }

        return pFuncId;
    }

    //--------------------------------------------------------------
    //  ParseBlock      Parse a routine's block:
    //
    //                      <declarations> <compound-statement>
    //
    //      pRoutineId : ptr to symbol table node of routine's id
    //--------------------------------------------------------------
    public void ParseBlock ( TSymtabNode pRoutineId )
    {
        //--<declarations>
        ParseDeclarations(pRoutineId);

        //--<compound-statement> : Reset the icode and append BEGIN to it,
        //--                       and then parse the compound statement.
        Resync(tlStatementStart);
        if (token != TTokenCode.TcBEGIN)
            Globals.Error(TErrorCode.ErrMissingBEGIN);
        icode.Reset();
        ParseCompound();

        //--Set the program's or routine's icode.
        pRoutineId.defn.routine.pIcode = new TIcode(icode);
    }
}
