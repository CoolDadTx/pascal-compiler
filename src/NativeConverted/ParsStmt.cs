//  *************************************************************
//  *                                                           *
//  *   P A R S E R   (Statements)                              *
//  *                                                           *
//  *   Parse statements.                                       *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog10-1/parsstmt.cpp                          *
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
    //  ParseStatement          Parse a statement.
    //--------------------------------------------------------------
    public void ParseStatement ()
    {
        InsertLineMarker();

        //--Call the appropriate parsing function based on
        //--the statement's first token.
        switch (token)
        {

            case TTokenCode.TcIdentifier:
            {

                //--Search for the identifier and enter it if
                //--necessary.  Append the symbol table node handle
                //--to the icode.
                TSymtabNode pNode = Find(pToken.String);
                icode.Put(pNode);

                //--Based on how the identifier is defined,
                //--parse an assignment statement or a procedure call.
                if (pNode.defn.how == TDefnCode.DcUndefined)
                {
                    pNode.defn.how = TDefnCode.DcVariable;
                    SetType(pNode.pType, Globals.pDummyType);
                    ParseAssignment(pNode);
                } else if (pNode.defn.how == TDefnCode.DcProcedure)
                {
                    ParseSubroutineCall(pNode, true);
                } else
                {
                    ParseAssignment(pNode);
                }

                break;
            }

            case TTokenCode.TcREPEAT:
            ParseREPEAT();
            break;
            case TTokenCode.TcWHILE:
            ParseWHILE();
            break;
            case TTokenCode.TcIF:
            ParseIF();
            break;
            case TTokenCode.TcFOR:
            ParseFOR();
            break;
            case TTokenCode.TcCASE:
            ParseCASE();
            break;
            case TTokenCode.TcBEGIN:
            ParseCompound();
            break;
        }

        //--Resynchronize at a proper statement ending.
        if (token != TTokenCode.TcEndOfFile)
            Resync(tlStatementFollow, tlStatementStart);
    }

    //--------------------------------------------------------------
    //  ParseStatementList      Parse a statement list until the
    //                          terminator token.
    //
    //      terminator : the token that terminates the list
    //--------------------------------------------------------------
    public void ParseStatementList ( TTokenCode terminator )
    {
        //--Loop to parse statements and to check for and skip semicolons.
        do
        {
            ParseStatement();

            if (TokenIn(token, tlStatementStart) != 0)
                Globals.Error(TErrorCode.ErrMissingSemicolon);
            else
            {
                while (token == TTokenCode.TcSemicolon)
                    GetTokenAppend();
            }
        } while ((token != terminator) && (token != TTokenCode.TcEndOfFile));
    }

    //--------------------------------------------------------------
    //  ParseAssignment         Parse an assignment statement:
    //
    //                              <id> := <expr>
    //
    //      pTargetId : ptr to target id's symbol table node
    //--------------------------------------------------------------
    public void ParseAssignment ( TSymtabNode pTargetId )
    {
        TType pTargetType = ParseVariable(pTargetId);

        //-- :=
        Resync(tlColonEqual, tlExpressionStart);
        CondGetTokenAppend(TTokenCode.TcColonEqual, TErrorCode.ErrMissingColonEqual);

        //--<expr>
        TType pExprType = ParseExpression();

        //--Check for assignment compatibility.
        CheckAssignmentTypeCompatible(pTargetType, pExprType, TErrorCode.ErrIncompatibleAssignment);
    }

    //--------------------------------------------------------------
    //  ParseREPEAT     Parse a REPEAT statement:
    //
    //                      REPEAT <stmt-list> UNTIL <expr>
    //--------------------------------------------------------------
    public void ParseREPEAT ()
    {
        GetTokenAppend();

        //--<stmt-list>
        ParseStatementList(TTokenCode.TcUNTIL);

        //--UNTIL
        CondGetTokenAppend(TTokenCode.TcUNTIL, TErrorCode.ErrMissingUNTIL);

        //--<expr> : must be boolean
        InsertLineMarker();
        CheckBoolean(ParseExpression());
    }

    //--------------------------------------------------------------
    //  ParseWHILE      Parse a WHILE statement.:
    //
    //                      WHILE <expr> DO <stmt>
    //--------------------------------------------------------------
    public void ParseWHILE ()
    {
        //--Append a placeholder location marker for the token that
        //--follows the WHILE statement.  Remember the location of this
        //--placeholder so it can be fixed up below.
        int atFollowLocationMarker = PutLocationMarker();

        //--<expr> : must be boolean
        GetTokenAppend();
        CheckBoolean(ParseExpression());

        //--DO
        Resync(tlDO, tlStatementStart);
        CondGetTokenAppend(TTokenCode.TcDO, TErrorCode.ErrMissingDO);

        //--<stmt>
        ParseStatement();
        FixupLocationMarker(atFollowLocationMarker);
    }

    //--------------------------------------------------------------
    //  ParseIF         Parse an IF statement:
    //
    //                      IF <expr> THEN <stmt-1>
    //
    //                  or:
    //
    //                      IF <expr> THEN <stmt-1> ELSE <stmt-2>
    //--------------------------------------------------------------
    public void ParseIF ()
    {
        //--Append a placeholder location marker for where to go to if
        //--<expr> is false.  Remember the location of this placeholder
        //--so it can be fixed up below.
        int atFalseLocationMarker = PutLocationMarker();

        //--<expr> : must be boolean
        GetTokenAppend();
        CheckBoolean(ParseExpression());

        //--THEN
        Resync(tlTHEN, tlStatementStart);
        CondGetTokenAppend(TTokenCode.TcTHEN, TErrorCode.ErrMissingTHEN);

        //--<stmt-1>
        ParseStatement();
        FixupLocationMarker(atFalseLocationMarker);

        if (token == TTokenCode.TcELSE)
        {

            //--Append a placeholder location marker for the token that
            //--follows the IF statement.  Remember the location of this
            //--placeholder so it can be fixed up below.
            int atFollowLocationMarker = PutLocationMarker();

            //--ELSE <stmt-2>
            GetTokenAppend();
            ParseStatement();
            FixupLocationMarker(atFollowLocationMarker);
        }
    }

    //--------------------------------------------------------------
    //  ParseFOR        Parse a FOR statement:
    //
    //                      FOR <id> := <expr-1> TO|DOWNTO <expr-2>
    //                          DO <stmt>
    //--------------------------------------------------------------
    public void ParseFOR ()
    {
        TType pControlType; // ptr to the control id's type object

        //--Append a placeholder for the location of the token that
        //--follows the FOR statement.  Remember the location of this
        //--placeholder.
        int atFollowLocationMarker = PutLocationMarker();

        //--<id>
        GetTokenAppend();
        if (token == TTokenCode.TcIdentifier)
        {

            //--Verify the definition and type of the control id.
            TSymtabNode pControlId = Find(pToken.String);
            if (pControlId.defn.how != TDefnCode.DcUndefined)
                pControlType = pControlId.pType.Base();
            else
            {
                pControlId.defn.how = TDefnCode.DcVariable;
                pControlType = pControlId.pType = Globals.pIntegerType;
            }
            if ((pControlType != Globals.pIntegerType) && (pControlType != Globals.pCharType) && (pControlType.form != TFormCode.FcEnum))
            {
                Globals.Error(TErrorCode.ErrIncompatibleTypes);
                pControlType = Globals.pIntegerType;
            }

            icode.Put(pControlId);
            GetTokenAppend();
        } else
        {
            Globals.Error(TErrorCode.ErrMissingIdentifier);
        }

        //-- :=
        Resync(tlColonEqual, tlExpressionStart);
        CondGetTokenAppend(TTokenCode.TcColonEqual, TErrorCode.ErrMissingColonEqual);

        //--<expr-1>
        CheckAssignmentTypeCompatible(pControlType, ParseExpression(), TErrorCode.ErrIncompatibleTypes);

        //--TO or DOWNTO
        Resync(tlTODOWNTO, tlExpressionStart);
        if (TokenIn(token, tlTODOWNTO) != 0)
            GetTokenAppend();
        else
            Globals.Error(TErrorCode.ErrMissingTOorDOWNTO);

        //--<expr-2>
        CheckAssignmentTypeCompatible(pControlType, ParseExpression(), TErrorCode.ErrIncompatibleTypes);

        //--DO
        Resync(tlDO, tlStatementStart);
        CondGetTokenAppend(TTokenCode.TcDO, TErrorCode.ErrMissingDO);

        //--<stmt>
        ParseStatement();
        FixupLocationMarker(atFollowLocationMarker);
    }

    //--------------------------------------------------------------
    //  ParseCASE       Parse a CASE statement:
    //
    //                      CASE <expr> OF
    //                          <case-branch> ;
    //                          ...
    //                      END
    //--------------------------------------------------------------
    public void ParseCASE ()
    {
        TCaseItem pCaseItemList; // ptr to list of CASE items
        var caseBranchFlag = false; // true if another CASE branch,
                            //   else false

        pCaseItemList = null;

        //--Append placeholders for the location of the token that
        //--follows the CASE statement and of the CASE branch table.
        //--Remember the locations of these placeholders.
        int atFollowLocationMarker = PutLocationMarker();
        int atBranchTableLocationMarker = PutLocationMarker();

        //--<expr>
        GetTokenAppend();
        TType pExprType = ParseExpression().Base();

        //--Verify the type of the CASE expression.
        if ((pExprType != Globals.pIntegerType) && (pExprType != Globals.pCharType) && (pExprType.form != TFormCode.FcEnum))
            Globals.Error(TErrorCode.ErrIncompatibleTypes);

        //--OF
        Resync(tlOF, tlCaseLabelStart);
        CondGetTokenAppend(TTokenCode.TcOF, TErrorCode.ErrMissingOF);

        //--Loop to parse CASE branches.
        caseBranchFlag = TokenIn(token, tlCaseLabelStart);
        while (caseBranchFlag != 0)
        {
            if (TokenIn(token, tlCaseLabelStart) != 0)
                ParseCaseBranch(pExprType, pCaseItemList);

            if (token == TTokenCode.TcSemicolon)
            {
                GetTokenAppend();
                caseBranchFlag = true;
            } else if (TokenIn(token, tlCaseLabelStart))
            {
                Globals.Error(TErrorCode.ErrMissingSemicolon);
                caseBranchFlag = true;
            } else
            {
                caseBranchFlag = false;
            }
        }

        //--Append the branch table to the intermediate code.
        FixupLocationMarker(atBranchTableLocationMarker);
        TCaseItem pItem = pCaseItemList;
        TCaseItem pNext;
        do
        {
            PutCaseItem(pItem.labelValue, pItem.atBranchStmt);
            pNext = pItem.next;
            pItem = null;
            pItem = pNext;
        } while (pItem != null);
        PutCaseItem(0, 0); // end of table

        //--END
        Resync(tlEND, tlStatementStart);
        CondGetTokenAppend(TTokenCode.TcEND, TErrorCode.ErrMissingEND);
        FixupLocationMarker(atFollowLocationMarker);
    }

    //--------------------------------------------------------------
    //  ParseCaseBranch     Parse a CASE branch:
    //
    //                          <case-label-list> : <stmt>
    //
    //      pExprType     : ptr to the CASE expression's type object
    //      pCaseItemList : ref to ptr to list of CASE items
    //--------------------------------------------------------------
    public void ParseCaseBranch ( TType pExprType, ref TCaseItem pCaseItemList )
    {
        var caseLabelFlag = false; // true if another CASE label, else false

        //--<case-label-list>
        do
        {
            ParseCaseLabel(pExprType, pCaseItemList);
            if (token == TTokenCode.TcComma)
            {

                //--Saw comma, look for another CASE label.
                GetTokenAppend();
                if (TokenIn(token, tlCaseLabelStart) != 0)
                    caseLabelFlag = true;
                else
                {
                    Globals.Error(TErrorCode.ErrMissingConstant);
                    caseLabelFlag = false;
                }
            } else
            {
                caseLabelFlag = false;
            }

        } while (caseLabelFlag);

        //-- :
        Resync(tlColon, tlStatementStart);
        CondGetTokenAppend(TTokenCode.TcColon, TErrorCode.ErrMissingColon);

        //--Loop to set the branch statement location into each CASE item
        //--for this branch.
        for (var pItem = pCaseItemList; pItem && !pItem.atBranchStmt; pItem = pItem.next)
            pItem.atBranchStmt = icode.CurrentLocation() - 1;

        //--<stmt>
        ParseStatement();
    }

    //--------------------------------------------------------------
    //  ParseCaseLabel      Parse a case label.
    //
    //      pExprType     : ptr to the CASE expression's type object
    //      pCaseItemList : ref to ptr to list of case items
    //--------------------------------------------------------------
    public void ParseCaseLabel ( TType pExprType, ref TCaseItem pCaseItemList )
    {
        TType pLabelType; // ptr to the CASE label's type object
        var signFlag = false; // true if unary sign, else false

        //--Allocate a new CASE item and insert it at the head of the list.
        TCaseItem pCaseItem = new TCaseItem(ref pCaseItemList);

        //--Unary + or -
        if (TokenIn(token, tlUnaryOps) != 0)
        {
            signFlag = true;
            GetTokenAppend();
        }

        switch (token)
        {

            //--Identifier:  Must be a constant whose type matches that
            //--             of the CASE expression.
            case TTokenCode.TcIdentifier:
            {

                TSymtabNode pLabelId = Find(pToken.String);
                icode.Put(pLabelId);

                if (pLabelId.defn.how != TDefnCode.DcUndefined)
                    pLabelType = pLabelId.pType.Base();
                else
                {
                    pLabelId.defn.how = TDefnCode.DcConstant;
                    SetType(pLabelId.pType, Globals.pDummyType);
                    pLabelType = Globals.pDummyType;
                }
                if (pExprType != pLabelType)
                    Globals.Error(TErrorCode.ErrIncompatibleTypes);

                //--Only an integer constant can have a unary sign.
                if (signFlag && (pLabelType != Globals.pIntegerType))
                    Globals.Error(TErrorCode.ErrInvalidConstant);

                //--Set the label value into the CASE item.
                if ((pLabelType == Globals.pIntegerType) || (pLabelType.form == TFormCode.FcEnum))
                    pCaseItem.labelValue = signFlag ? -pLabelId.defn.constant.value.integer : pLabelId.defn.constant.value.integer;
                else
                    pCaseItem.labelValue = pLabelId.defn.constant.value.character;

                GetTokenAppend();
                break;
            }

            //--Number:  Both the label and the CASE expression
            //--         must be integer.
            case TTokenCode.TcNumber:
            {

                if (pToken.Type() != TDataType.TyInteger)
                    Globals.Error(TErrorCode.ErrInvalidConstant);
                if (pExprType != Globals.pIntegerType)
                    Globals.Error(TErrorCode.ErrIncompatibleTypes);

                TSymtabNode pNode = SearchAll(pToken.String);
                if (pNode == null)
                {
                    pNode = EnterLocal(pToken.String);
                    pNode.pType = pIntegerType;
                    pNode.defn.constant.value.integer = pToken.Value().integer;
                }
                icode.Put(pNode);

                //--Set the label value into the CASE item.
                pCaseItem.labelValue = signFlag ? -pNode.defn.constant.value.integer : pNode.defn.constant.value.integer;

                GetTokenAppend();
                break;
            }

            //--String:  Must be a single character without a unary sign.
            //--         (Note that the string length includes the quotes.)
            //--         The CASE expression type must be character.
            case TTokenCode.TcString:
            {

                if (signFlag || (Convert.ToString(pToken.String).Length != 3))
                    Globals.Error(TErrorCode.ErrInvalidConstant);
                if (pExprType != Globals.pCharType)
                    Globals.Error(TErrorCode.ErrIncompatibleTypes);

                TSymtabNode pNode = SearchAll(pToken.String);
                if (pNode == null)
                {
                    pNode = EnterLocal(pToken.String);
                    pNode.pType = Globals.pCharType;
                    pNode.defn.constant.value.character = pToken.String[1];
                }
                icode.Put(pNode);

                //--Set the label value into the CASE item.
                pCaseItem.labelValue = pToken.String[1];

                GetTokenAppend();
                break;
            }
        }
    }

    //--------------------------------------------------------------
    //  ParseCompound       Parse a compound statement:
    //
    //                          BEGIN <stmt-list> END
    //--------------------------------------------------------------
    public void ParseCompound ()
    {
        GetTokenAppend();

        //--<stmt-list>
        ParseStatementList(TTokenCode.TcEND);

        //--END
        CondGetTokenAppend(TTokenCode.TcEND, TErrorCode.ErrMissingEND);
    }
}

