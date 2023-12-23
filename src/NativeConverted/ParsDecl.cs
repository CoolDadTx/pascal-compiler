//  *************************************************************
//  *                                                           *
//  *   P A R S E R   (Declarations)                            *
//  *                                                           *
//  *   Parse constant definitions and variable and record      *
//  *   field declarations.                                     *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog13-1/parsdecl.cpp                          *
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
    //  ParseDeclarations   Parse any constant definitions, type
    //                      definitions, variable declarations, and
    //                      procedure and function declarations, in
    //                      that order.
    //--------------------------------------------------------------
    public void ParseDeclarations ( TSymtabNode pRoutineId )
    {
        if (token == TTokenCode.TcCONST)
        {
            GetToken();
            ParseConstantDefinitions(pRoutineId);
        }

        if (token == TTokenCode.TcTYPE)
        {
            GetToken();
            ParseTypeDefinitions(pRoutineId);
        }

        if (token == TTokenCode.TcVAR)
        {
            GetToken();
            ParseVariableDeclarations(pRoutineId);
        }
        
        if (Globals.TokenIn(token, Globals.tlProcFuncStart))
            ParseSubroutineDeclarations(pRoutineId);
    }

    //              **************************
    //              *                        *
    //              *  Constant Definitions  *
    //              *                        *
    //              **************************

    //--------------------------------------------------------------
    //  ParseConstDefinitions   Parse a list of constant definitions
    //                          separated by semicolons:
    //
    //                              <id> = <constant>
    //
    //      pRoutineId : ptr to symbol table node of program,
    //                   procedure, or function identifier
    //--------------------------------------------------------------
    public void ParseConstantDefinitions ( TSymtabNode pRoutineId )
    {
        TSymtabNode pLastId = null; // ptr to last constant id node
                                    //   in local list

        //--Loop to parse a list of constant definitions
        //--separated by semicolons.
        while (token == TTokenCode.TcIdentifier)
        {

            //--<id>
            TSymtabNode pConstId = EnterNewLocal(pToken.String);

            //--Link the routine's local constant id nodes together.
            if (!pRoutineId.defn.routine.locals.pConstantIds)
                pRoutineId.defn.routine.locals.pConstantIds = pConstId;
            else
                pLastId.next = pConstId;
            pLastId = pConstId;

            //-- =
            GetToken();
            CondGetToken(TTokenCode.TcEqual, TErrorCode.ErrMissingEqual);

            //--<constant>
            ParseConstant(pConstId);
            pConstId.defn.how = TDefnCode.DcConstant;

            //-- ;
            Resync(Globals.tlDeclarationFollow, Globals.tlDeclarationStart, Globals.tlStatementStart);
            CondGetToken(TTokenCode.TcSemicolon, TErrorCode.ErrMissingSemicolon);

            //--Skip extra semicolons.
            while (token == TTokenCode.TcSemicolon)
                GetToken();
            Resync(Globals.tlDeclarationFollow, Globals.tlDeclarationStart, Globals.tlStatementStart);
        }
    }

    //--------------------------------------------------------------
    //  ParseConstant       Parse a constant.
    //
    //      pConstId : ptr to symbol table node of the identifier
    //                 being defined
    //--------------------------------------------------------------
    public void ParseConstant ( TSymtabNode pConstId )
    {
        TTokenCode sign = TTokenCode.TcDummy; // unary + or - sign, or none

        //--Unary + or -
        if (Globals.TokenIn(token, Globals.tlUnaryOps))
        {
            if (token == TTokenCode.TcMinus)
                sign = TTokenCode.TcMinus;
            GetToken();
        }

        switch (token)
        {

            //--Numeric constant:  Integer or real type.
            case TTokenCode.TcNumber:
            if (pToken.Type() == TDataType.TyInteger)
            {
                pConstId.defn.constant.value.integer = sign == TTokenCode.TcMinus ? -pToken.Value().integer : pToken.Value().integer;
                TType.SetType(pConstId.pType, Globals.pIntegerType);
            } else
            {
                pConstId.defn.constant.value.real = sign == TTokenCode.TcMinus ? -pToken.Value().real : pToken.Value().real;
                TType.SetType(pConstId.pType, Globals.pRealType);
            }

            GetToken();
            break;

            //--Identifier constant
            case TTokenCode.TcIdentifier:
            ParseIdentifierConstant(pConstId, sign);
            break;

            //--String constant:  Character or string
            //--                  (character array) type.
            case TTokenCode.TcString:
            int length = Convert.ToString(pToken.String).Length - 2; // skip quotes

            if (sign != TTokenCode.TcDummy)
                Globals.Error(TErrorCode.ErrInvalidConstant);

            //--Single character
            if (length == 1)
            {
                pConstId.defn.constant.value.character = pToken.String[1];
                TType.SetType(pConstId.pType, Globals.pCharType);
            }

            //--String (character array):  Create a new unnamed
            //--                           string type.
            else
            {
                var pString = CopyQuotedString(pToken.String);
                pConstId.defn.constant.value.pString = pString;
                TType.SetType(ref pConstId.pType, new TType(length));
            }

            GetToken();
            break;
        }
    }

    //--------------------------------------------------------------
    //  ParseIdentifierConstant     In a constant definition of the
    //                              form
    //
    //                                      <id-1> = <id-2>
    //
    //                              parse <id-2>. The type can be
    //                              integer, real, character,
    //                              enumeration, or string
    //                              (character array).
    //
    //      pId1 : ptr to symbol table node of <id-1>
    //      sign : unary + or - sign, or none
    //--------------------------------------------------------------
    public void ParseIdentifierConstant ( TSymtabNode pId1, TTokenCode sign )
    {
        TSymtabNode pId2 = Find(pToken.String); // ptr to <id-2>

        if (pId2.defn.how != TDefnCode.DcConstant)
        {
            Globals.Error(TErrorCode.ErrNotAConstantIdentifier);
            TType.SetType(ref pId1.pType, Globals.pDummyType);
            GetToken();
            return;
        }

        //--Integer identifier
        if (pId2.pType == Globals.pIntegerType)
        {
            pId1.defn.constant.value.integer = sign == TTokenCode.TcMinus ? -pId2.defn.constant.value.integer : pId2.defn.constant.value.integer;
            TType.SetType(ref pId1.pType, Globals.pIntegerType);
        }

        //--Real identifier
        else if (pId2.pType == Globals.pRealType)
        {
            pId1.defn.constant.value.real = sign == TTokenCode.TcMinus ? -pId2.defn.constant.value.real : pId2.defn.constant.value.real;
            TType.SetType(ref pId1.pType, Globals.pRealType);
        }

        //--Character identifier:  No unary sign allowed.
        else if (pId2.pType == Globals.pCharType)
        {
            if (sign != TTokenCode.TcDummy)
                Globals.Error(TErrorCode.ErrInvalidConstant);

            pId1.defn.constant.value.character = pId2.defn.constant.value.character;
            TType.SetType(ref pId1.pType, Globals.pCharType);
        }

        //--Enumeration identifier:  No unary sign allowed.
        else if (pId2.pType.form == TFormCode.FcEnum)
        {
            if (sign != TTokenCode.TcDummy)
                Globals.Error(TErrorCode.ErrInvalidConstant);

            pId1.defn.constant.value.integer = pId2.defn.constant.value.integer;
            TType.SetType(ref pId1.pType, pId2.pType);
        }

        //--Array identifier:  Must be character array, and
        //                     no unary sign allowed.
        else if (pId2.pType.form == TFormCode.FcArray)
        {
            if ((sign != TTokenCode.TcDummy) || (pId2.pType.array.pElmtType != pCharType))
                Globals.Error(TErrorCode.ErrInvalidConstant);

            pId1.defn.constant.value.pString = pId2.defn.constant.value.pString;
            TType.SetType(ref pId1.pType, pId2.pType);
        }

        GetToken();
    }

    //      ********************************************
    //      *                                          *
    //      *  Variable and Record Field Declarations  *
    //      *                                          *
    //      ********************************************

    //--------------------------------------------------------------
    //  ParseVariableDeclarations   Parse variable declarations.
    //
    //      pRoutineId : ptr to symbol table node of program,
    //                   procedure, or function identifier
    //--------------------------------------------------------------
    public void ParseVariableDeclarations ( TSymtabNode pRoutineId )
    {
        if (Globals.execFlag)
            ParseVarOrFieldDecls(pRoutineId, null, pRoutineId.defn.routine.parmCount);
        else
            ParseVarOrFieldDecls(pRoutineId, null, pRoutineId.defn.how == TDefnCode.DcProcedure ? Globals.procLocalsStackFrameOffset : Globals.funcLocalsStackFrameOffset);
    }
    
    //--------------------------------------------------------------
    //  ParseFieldDeclarations      Parse a list record field
    //                              declarations.
    //
    //      pRecordType : ptr to record type object
    //      offset      : byte offset within record
    //--------------------------------------------------------------
    public void ParseFieldDeclarations ( TType pRecordType, int offset )
    {
        ParseVarOrFieldDecls(null, pRecordType, offset);
    }

    //--------------------------------------------------------------
    //  ParseVarOrFieldDecls        Parse a list of variable or
    //                              record field declarations
    //                              separated by semicolons:
    //
    //                                  <id-sublist> : <type>
    //
    //      pRoutineId  : ptr to symbol table node of program,
    //                    procedure, or function identifier, or NULL
    //      pRecordType : ptr to record type object, or NULL
    //      offset      : variable: runtime stack offset
    //                    field: byte offset within record
    //--------------------------------------------------------------
    public void ParseVarOrFieldDecls ( TSymtabNode pRoutineId, TType pRecordType, int offset )
    {
        TSymtabNode pId; // ptrs to symtab nodes
        TSymtabNode pFirstId;
        TSymtabNode pLastId;
        TSymtabNode pPrevSublistLastId = null; // ptr to last node of
                                               //   previous sublist
        int totalSize = 0; // total byte size of
                           //   local variables

        //--Loop to parse a list of variable or field declarations
        //--separated by semicolons.
        while (token == TTokenCode.TcIdentifier)
        {

            //--<id-sublist>
            pFirstId = ParseIdSublist(pRoutineId, pRecordType, out pLastId);

            //-- :
            Resync(Globals.tlSublistFollow, Globals.tlDeclarationFollow);
            CondGetToken(TTokenCode.TcColon, TErrorCode.ErrMissingColon);

            //--<type>
            TType pType = ParseTypeSpec();

            //--Now loop to assign the type and offset to each
            //--identifier in the sublist.
            for (pId = pFirstId; pId != null; pId = pId.next)
            {
                TType.SetType(ref pId.pType, pType);

                if (pRoutineId != null)
                {

                    //--Variables
                    if (Globals.execFlag)
                        pId.defn.data.offset = offset++;
                    else
                    {
                        offset -= pType.size;
                        pId.defn.data.offset = offset;
                    }
                    totalSize += pType.size;
                } else
                {

                    //--Record fields
                    pId.defn.data.offset = offset;
                    offset += pType.size;
                }
            }

            if (pFirstId != null)
            {

                //--Set the first sublist into the routine id's symtab node.
                if (pRoutineId != null && (!pRoutineId.defn.routine.locals.pVariableIds))
                    pRoutineId.defn.routine.locals.pVariableIds = pFirstId;

                //--Link this list to the previous sublist.
                if (pPrevSublistLastId != null)
                    pPrevSublistLastId.next = pFirstId;
                pPrevSublistLastId = pLastId;
            }

            //-- ;   for variable and record field declaration, or
            //-- END for record field declaration
            if (pRoutineId != null)
            {
                Resync(Globals.tlDeclarationFollow, Globals.tlStatementStart);
                CondGetToken(TTokenCode.TcSemicolon, TErrorCode.ErrMissingSemicolon);

                //--Skip extra semicolons.
                while (token == TTokenCode.TcSemicolon)
                    GetToken();
                Resync(Globals.tlDeclarationFollow, Globals.tlDeclarationStart, Globals.tlStatementStart);
            } else
            {
                Resync(Globals.tlFieldDeclFollow);
                if (token != TTokenCode.TcEND)
                {
                    CondGetToken(TTokenCode.TcSemicolon, TErrorCode.ErrMissingSemicolon);

                    //--Skip extra semicolons.
                    while (token == TTokenCode.TcSemicolon)
                        GetToken();
                    Resync(Globals.tlFieldDeclFollow, Globals.tlDeclarationStart, Globals.tlStatementStart);
                }
            }
        }

        //--Set the routine identifier node or the record type object.
        if (pRoutineId != null)
            pRoutineId.defn.routine.totalLocalSize = totalSize;
        else
            pRecordType.size = offset;
    }
    
    //--------------------------------------------------------------
    //  ParseIdSublist      Parse a sublist of variable or record
    //                      identifiers separated by commas.
    //
    //      pRoutineId  : ptr to symbol table node of program,
    //                    procedure, or function identifier, or NULL
    //      pRecordType : ptr to record type object, or NULL
    //      pLastId     : ref to ptr that will be set to point to the
    //                    last symbol table node of the sublist
    //--------------------------------------------------------------
    public TSymtabNode ParseIdSublist ( TSymtabNode pRoutineId, TType pRecordType, out TSymtabNode pLastId )
    {
        TSymtabNode pId;
        TSymtabNode pFirstId = null;

        pLastId = null;

        //--Loop to parse each identifier in the sublist.
        while (token == TTokenCode.TcIdentifier)
        {

            //--Variable:  Enter into local  symbol table.
            //--Field:     Enter into record symbol table.
            pId = pRoutineId != null ? EnterNewLocal(pToken.String) : pRecordType.record.pSymtab.EnterNew(pToken.String);

            //--Link newly-declared identifier nodes together
            //--into a sublist.
            if (pId.defn.how == TDefnCode.DcUndefined)
            {
                pId.defn.how = pRoutineId != null ? TDefnCode.DcVariable : TDefnCode.DcField;
                if (pFirstId == null)
                    pFirstId = pLastId = pId;
                else
                {
                    pLastId.next = pId;
                    pLastId = pId;
                }
            }

            //-- ,
            GetToken();
            Resync(Globals.tlIdentifierFollow);
            if (token == TTokenCode.TcComma)
            {

                //--Saw comma.
                //--Skip extra commas and look for an identifier.
                do
                {
                    GetToken();
                    Resync(Globals.tlIdentifierStart, Globals.tlIdentifierFollow);
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

        return pFirstId;
    }

}