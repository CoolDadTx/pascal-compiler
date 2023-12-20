//  *************************************************************
//  *                                                           *
//  *   P A R S E R   (Routines #2)                             *
//  *                                                           *
//  *   Parse formal parameters, procedure and function calls,  *
//  *   and actual parameters.                                  *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog13-1/parsrtn2.cpp                          *
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
    //  ParseFormalParmList     Parse a formal parameter list:
    //
    //                              ( VAR <id-list> : <type-id> ;
    //                                <id-list> : <type-id> ;
    //                                ... )
    //
    //      count     : ref to count of parameters
    //      totalSize : ref to total byte size of all parameters
    //
    //  Return: ptr to head of parm id symbol table node list
    //--------------------------------------------------------------
    public TSymtabNode ParseFormalParmList ( ref int count, ref int totalSize )
    {
        //C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
        //	extern int execFlag;

        TSymtabNode pParmId; // ptrs to parm symtab nodes
        TSymtabNode pFirstId;
        TSymtabNode pLastId;
        TSymtabNode pPrevSublistLastId = null;
        TSymtabNode pParmList = null; // ptr to list of parm nodes
        TDefnCode parmDefn; // how a parm is defined
        int offset = Globals.parametersStackFrameOffset;

        count = totalSize = 0;
        GetToken();

        //--Loop to parse a parameter declarations separated by semicolons.
        while ((token == TTokenCode.TcIdentifier) || (token == TTokenCode.TcVAR))
        {
            TType pParmType; // ptr to parm's type object

            pFirstId = null;

            //--VAR or value parameter?
            if (token == TTokenCode.TcVAR)
            {
                parmDefn = TDefnCode.DcVarParm;
                GetToken();
            } else
            {
                parmDefn = TDefnCode.DcValueParm;
            }

            //--Loop to parse the comma-separated sublist of parameter ids.
            while (token == TTokenCode.TcIdentifier)
            {
                pParmId = EnterNewLocal(pToken.String, parmDefn);
                ++count;
                if (pParmList == null)
                    pParmList = pParmId;

                //--Link the parm id nodes together.
                if (pFirstId == null)
                    pFirstId = pLastId = pParmId;
                else
                {
                    pLastId.next = pParmId;
                    pLastId = pParmId;
                }

                //-- ,
                GetToken();
                Resync(tlIdentifierFollow);
                if (token == TTokenCode.TcComma)
                {

                    //--Saw comma.
                    //--Skip extra commas and look for an identifier.
                    do
                    {
                        GetToken();
                        Resync(tlIdentifierStart, tlIdentifierFollow);
                        if (token == TTokenCode.TcComma)
                            Globals.Error(TErrorCode.ErrMissingIdentifier);
                    } while (token == TTokenCode.TcComma);
                    if (token != TTokenCode.TcIdentifier)
                        Globals.Error(TErrorCode.ErrMissingIdentifier);
                } else if (token == TTokenCode.TcIdentifier)
                {
                    Globals.Error(TErrorCode.ErrMissingComma);
                }
            }

            //-- :
            Resync(tlSublistFollow, tlDeclarationFollow);
            CondGetToken(TTokenCode.TcColon, TErrorCode.ErrMissingColon);

            //--<type-id>
            if (token == TTokenCode.TcIdentifier)
            {
                TSymtabNode pTypeId = Find(pToken.String);
                if (pTypeId.defn.how != TDefnCode.DcType)
                    Globals.Error(TErrorCode.ErrInvalidType);
                pParmType = pTypeId.pType;
                GetToken();
            } else
            {
                Globals.Error(TErrorCode.ErrMissingIdentifier);
                pParmType = Globals.pDummyType;
            }

            if (Globals.execFlag)
            {
                //--Loop to assign the offset and type to each
                //--parm id in the sublist.
                for (pParmId = pFirstId; pParmId != null; pParmId = pParmId.next)
                {
                    pParmId.defn.data.offset = totalSize++;
                    SetType(pParmId.pType, pParmType);
                }
            } else
            {
                //--Loop to assign the type to each parm id in the sublist.
                for (pParmId = pFirstId; pParmId != null; pParmId = pParmId.next)
                    SetType(pParmId.pType, pParmType);
            }

            //--Link this sublist to the previous sublist.
            if (pPrevSublistLastId != null)
                pPrevSublistLastId.next = pFirstId;
            pPrevSublistLastId = pLastId;

            //-- ; or )
            Resync(tlFormalParmsFollow, tlDeclarationFollow);
            if ((token == TTokenCode.TcIdentifier) || (token == TTokenCode.TcVAR))
                Globals.Error(TErrorCode.ErrMissingSemicolon);
            else
            {
                while (token == TTokenCode.TcSemicolon)
                    GetToken();
            }
        }

        if (!Globals.execFlag)
        {

            //--Assign the offset to each parm id in the entire
            //--formal parameter list in reverse order.
            ReverseNodeList(ref pParmList);
            for (pParmId = pParmList; pParmId != null; pParmId = pParmId.next)
            {
                pParmId.defn.data.offset = offset;
                offset += pParmId.defn.how == TDefnCode.DcValueParm ? pParmId.pType.size : sizeof(object*); // VAR pointer -  data value
                if ((offset & 1) != 0)
                    ++offset; // round up to even
            }
            ReverseNodeList(ref pParmList);

            totalSize = offset - Globals.parametersStackFrameOffset;
        }

        //-- )
        CondGetToken(TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen);

        return pParmList;
    }

    //--------------------------------------------------------------
    //  ParseSubroutineCall     Parse a call to a declared or a
    //                          standard procedure or function.
    //
    //      pRoutineId    : ptr to routine id's symbol table node
    //      parmCheckFlag : true to check parameter, false not to
    //
    //  Return: ptr to the subroutine's type object
    //--------------------------------------------------------------
    public TType ParseSubroutineCall ( TSymtabNode pRoutineId, bool parmCheckFlag )
    {
        GetTokenAppend();

        return (pRoutineId.defn.routine.which == TRoutineCode.RcDeclared) || (pRoutineId.defn.routine.which == TRoutineCode.RcForward) || !parmCheckFlag ? ParseDeclaredSubroutineCall(pRoutineId, parmCheckFlag) : ParseStandardSubroutineCall(pRoutineId);
    }

    //--------------------------------------------------------------
    //  ParseDeclaredSubroutineCall Parse a call to a declared
    //                              procedure or function.
    //
    //      pRoutineId    : ptr to subroutine id's symbol table node
    //      parmCheckFlag : true to check parameter, false not to
    //
    //  Return: ptr to the subroutine's type object
    //--------------------------------------------------------------
    public TType ParseDeclaredSubroutineCall ( TSymtabNode pRoutineId, bool parmCheckFlag )
    {
        ParseActualParmList(pRoutineId, parmCheckFlag);
        return pRoutineId.pType;
    }

    //--------------------------------------------------------------
    //  ParseActualParmList     Parse an actual parameter list:
    //
    //                              ( <expr-list> )
    //
    //      pRoutineId    : ptr to routine id's symbol table node
    //      parmCheckFlag : true to check parameter, false not to
    //--------------------------------------------------------------
    public void ParseActualParmList ( TSymtabNode pRoutineId, bool parmCheckFlag )
    {
        TSymtabNode pFormalId = pRoutineId != null ? pRoutineId.defn.routine.locals.pParmIds : null;

        //--If there are no actual parameters, there better not be
        //--any formal parameters either.
        if (token != TTokenCode.TcLParen)
        {
            if (parmCheckFlag && pFormalId != null)
                Globals.Error(TErrorCode.ErrWrongNumberOfParms);
            return;
        }

        //--Loop to parse actual parameter expressions
        //--separated by commas.
        do
        {
            //-- ( or ,
            GetTokenAppend();

            ParseActualParm(pFormalId, parmCheckFlag);
            if (pFormalId != null)
                pFormalId = pFormalId.next;
        } while (token == TTokenCode.TcComma);

        //-- )
        CondGetTokenAppend(TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen);

        //--There better not be any more formal parameters.
        if (parmCheckFlag && pFormalId != null)
            Globals.Error(TErrorCode.ErrWrongNumberOfParms);
    }

    //--------------------------------------------------------------
    //  ParseActualParm     Parse an actual parameter.  Make sure it
    //                      matches the corresponding formal parm.
    //
    //      pFormalId     : ptr to the corresponding formal parm
    //                      id's symbol table node
    //      parmCheckFlag : true to check parameter, false not to
    //--------------------------------------------------------------
    public void ParseActualParm ( TSymtabNode pFormalId, bool parmCheckFlag )
    {
        //--If we're not checking the actual parameters against
        //--the corresponding formal parameters (as during error
        //--recovery), just parse the actual parameter.
        if (!parmCheckFlag)
        {
            ParseExpression();
            return;
        }

        //--If we've already run out of formal parameter,
        //--we have an error.  Go into error recovery mode and
        //--parse the actual parameter anyway.
        if (pFormalId == null)
        {
            Globals.Error(TErrorCode.ErrWrongNumberOfParms);
            ParseExpression();
            return;
        }

        //--Formal value parameter: The actual parameter can be an
        //--                        arbitrary expression that is
        //--                        assignment type compatible with
        //--                        the formal parameter.
        if (pFormalId.defn.how == TDefnCode.DcValueParm)
            CheckAssignmentTypeCompatible(pFormalId.pType, ParseExpression(), TErrorCode.ErrIncompatibleTypes);

        //--Formal VAR parameter: The actual parameter must be a
        //--                      variable of the same type as the
        //--                      formal parameter.
        else if (token == TTokenCode.TcIdentifier)
        {
            TSymtabNode pActualId = Find(pToken.String);
            icode.Put(pActualId);

            if (pFormalId.pType != ParseVariable(pActualId))
                Globals.Error(TErrorCode.ErrIncompatibleTypes);
            Resync(tlExpressionFollow, tlStatementFollow, tlStatementStart);
        }

        //--Error: Parse the actual parameter anyway for error recovery.
        else
        {
            ParseExpression();
            Globals.Error(TErrorCode.ErrInvalidVarParm);
        }
    }

    //--------------------------------------------------------------
    //  ReverseNodeList 	Reverse a list of symbol table nodes.
    //
    //	head : ref to the ptr to the current head of the list
    //--------------------------------------------------------------
    public void ReverseNodeList ( ref TSymtabNode head )
    {
        TSymtabNode prev = null;
        TSymtabNode curr = head;
        TSymtabNode next;

        //--Reverse the list in place.
        while (curr != null)
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
}
