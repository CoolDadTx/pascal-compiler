//  *************************************************************
//  *                                                           *
//  *   P A R S E R   (Types #1)                                *
//  *                                                           *
//  *   Parse type definitions, and identifier, enumeration,    *
//  *   and subrange type specifications.                       *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog7-1/parstyp1.cpp                           *
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
    //  ParseTypeDefinitions    Parse a list of type definitions
    //                          separated by semicolons:
    //
    //                              <id> = <type>
    //
    //      pRoutineId : ptr to symbol table node of program,
    //                   procedure, or function identifier
    //--------------------------------------------------------------
    public void ParseTypeDefinitions ( TSymtabNode pRoutineId )
    {
        TSymtabNode pLastId = null; // ptr to last type id node
                                    //   in local list

        //--Loop to parse a list of type definitions
        //--seperated by semicolons.
        while (token == TTokenCode.TcIdentifier)
        {

            //--<id>
            TSymtabNode pTypeId = EnterNewLocal(pToken.String);

            //--Link the routine's local type id nodes together.
            if (!pRoutineId.defn.routine.locals.pTypeIds)
                pRoutineId.defn.routine.locals.pTypeIds = pTypeId;
            else
                pLastId.next = pTypeId;
            pLastId = pTypeId;

            //-- =
            GetToken();
            CondGetToken(TTokenCode.TcEqual, TErrorCode.ErrMissingEqual);

            //--<type>
            SetType(pTypeId.pType, ParseTypeSpec());
            pTypeId.defn.how = TDefnCode.DcType;

            //--If the type object doesn't have a name yet,
            //--point it to the type id.
            if (pTypeId.pType.pTypeId == null)
                pTypeId.pType.pTypeId = pTypeId;

            //-- ;
            Resync(tlDeclarationFollow, tlDeclarationStart, tlStatementStart);
            CondGetToken(TTokenCode.TcSemicolon, TErrorCode.ErrMissingSemicolon);

            //--Skip extra semicolons.
            while (token == TTokenCode.TcSemicolon)
                GetToken();
            Resync(tlDeclarationFollow, tlDeclarationStart, tlStatementStart);
        }
    }

    //--------------------------------------------------------------
    //  ParseTypeSpec       Parse a type specification.
    //
    //  Return: ptr to type object
    //--------------------------------------------------------------
    public TType ParseTypeSpec ()
    {
        switch (token)
        {

            //--Type identifier
            case TTokenCode.TcIdentifier:
            {
                TSymtabNode pId = Find(pToken.String);

                switch (pId.defn.how)
                {
                    case TDefnCode.DcType:
                    return ParseIdentifierType(pId);
                    case TDefnCode.DcConstant:
                    return ParseSubrangeType(pId);

                    default:
                    Globals.Error(TErrorCode.ErrNotATypeIdentifier);
                    GetToken();
                    return Globals.pDummyType;
                }
            }

            //C++ TO C# CONVERTER TODO TASK: C# does not allow fall-through from a non-empty 'case':
            case TTokenCode.TcLParen:
            return ParseEnumerationType();
            case TTokenCode.TcARRAY:
            return ParseArrayType();
            case TTokenCode.TcRECORD:
            return ParseRecordType();

            case TTokenCode.TcPlus:
            case TTokenCode.TcMinus:
            case TTokenCode.TcNumber:
            case TTokenCode.TcString:
            return ParseSubrangeType(null);

            default:
            Globals.Error(TErrorCode.ErrInvalidType);
            return Globals.pDummyType;
        }
    }

    //              *********************
    //              *                   *
    //              *  Identifier Type  *
    //              *                   *
    //              *********************

    //--------------------------------------------------------------
    //  ParseIdentifierType     In a type defintion of the form
    //
    //                               <id-1> = <id-2>
    //
    //                          parse <id-2>.
    //
    //      pId2 : ptr to symbol table node of <id-2>
    //
    //  Return: ptr to type object of <id-2>
    //--------------------------------------------------------------
    public TType ParseIdentifierType ( TSymtabNode pId2 )
    {
        GetToken();
        return pId2.pType;
    }

    //              **********************
    //              *                    *
    //              *  Enumeration Type  *
    //              *                    *
    //              **********************

    //--------------------------------------------------------------
    //  ParseEnumerationType    Parse a enumeration type
    //                          specification:
    //
    //                               ( <id-1>, <id-2>, ..., <id-n> )
    //
    //  Return: ptr to type object
    //--------------------------------------------------------------
    public TType ParseEnumerationType ()
    {
        TType pType = new TType(TFormCode.FcEnum, sizeof(int), null);
        TSymtabNode pLastId = null;
        int constValue = -1; // enumeration constant value

        GetToken();
        Resync(tlEnumConstStart);

        //--Loop to parse list of constant identifiers separated by commas.
        while (token == TTokenCode.TcIdentifier)
        {
            TSymtabNode pConstId = EnterNewLocal(pToken.String);
            ++constValue;

            if (pConstId.defn.how == TDefnCode.DcUndefined)
            {
                pConstId.defn.how = TDefnCode.DcConstant;
                pConstId.defn.constant.value.integer = constValue;
                SetType(pConstId.pType, pType);

                //--Link constant identifier symbol table nodes together.
                if (pLastId == null)
                    pType.enumeration.pConstIds = pLastId = pConstId;
                else
                {
                    pLastId.next = pConstId;
                    pLastId = pConstId;
                }
            }

            //-- ,
            GetToken();
            Resync(tlEnumConstFollow);
            if (token == TTokenCode.TcComma)
            {

                //--Saw comma.  Skip extra commas and look for
                //--            an identifier.
                do
                {
                    GetToken();
                    Resync(tlEnumConstStart, tlEnumConstFollow);
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

        //-- )
        CondGetToken(TTokenCode.TcRParen, TErrorCode.ErrMissingRightParen);

        pType.enumeration.max = constValue;
        return pType;
    }

    //              *******************
    //              *                 *
    //              *  Subrange Type  *
    //              *                 *
    //              *******************

    //--------------------------------------------------------------
    //  ParseSubrangeType       Parse a subrange type specification:
    //
    //                               <min-const> .. <max-const>
    //
    //      pMinId : ptr to symbol table node of <min-const> if it
    //               is an identifier, else NULL
    //
    //  Return: ptr to type object
    //--------------------------------------------------------------
    public TType ParseSubrangeType ( TSymtabNode pMinId )
    {
        TType pType = new TType(TFormCode.FcSubrange, 0, null);

        //--<min-const>
        SetType(pType.subrange.pBaseType, ParseSubrangeLimit(pMinId, pType.subrange.min));

        //-- ..
        Resync(tlSubrangeLimitFollow, tlDeclarationStart);
        CondGetToken(TTokenCode.TcDotDot, TErrorCode.ErrMissingDotDot);

        //--<max-const>
        TType pMaxType = ParseSubrangeLimit(null, pType.subrange.max);

        //--Check limits.
        if (pMaxType != pType.subrange.pBaseType)
        {
            Globals.Error(TErrorCode.ErrIncompatibleTypes);
            pType.subrange.min = pType.subrange.max = 0;
        } else if (pType.subrange.min > pType.subrange.max)
        {
            Globals.Error(TErrorCode.ErrMinGtMax);

            int temp = pType.subrange.min;
            pType.subrange.min = pType.subrange.max;
            pType.subrange.max = temp;
        }

        pType.size = pType.subrange.pBaseType.size;
        return pType;
    }

    //--------------------------------------------------------------
    //  ParseSubrangeLimit      Parse a mininum or maximum limit
    //                          constant of a subrange type.
    //
    //      pLimitId : ptr to symbol table node of limit constant if
    //                 it is an identifier (already set for the
    //                 minimum limit), else NULL
    //      limit    : ref to limit value that will be set
    //
    //  Return: ptr to type object of limit constant
    //--------------------------------------------------------------
    public TType ParseSubrangeLimit ( TSymtabNode pLimitId, ref int limit )
    {
        TType pType = Globals.pDummyType; // type to return
        TTokenCode sign = TTokenCode.TcDummy; // unary + or - sign, or none

        limit = 0;

        //--Unary + or -
        if (TokenIn(token, tlUnaryOps) != 0)
        {
            if (token == TTokenCode.TcMinus)
                sign = TTokenCode.TcMinus;
            GetToken();
        }

        switch (token)
        {

            case TTokenCode.TcNumber:

            //--Numeric constant:  Integer type only.
            if (pToken.Type() == TDataType.TyInteger)
            {
                limit = sign == TTokenCode.TcMinus ? -pToken.Value().integer : pToken.Value().integer;
                pType = Globals.pIntegerType;
            } else
            {
                Globals.Error(TErrorCode.ErrInvalidSubrangeType);
            }
            break;

            case TTokenCode.TcIdentifier:

            //--Identifier limit:  Must be integer, character, or
            //--                   enumeration type.
            if (pLimitId == null)
                pLimitId = Find(pToken.String);

            if (pLimitId.defn.how == TDefnCode.DcUndefined)
            {
                pLimitId.defn.how = TDefnCode.DcConstant;
                pType = SetType(pLimitId.pType, Globals.pDummyType);
                break;
            } else if ((pLimitId.pType == Globals.pRealType) || (pLimitId.pType == Globals.pDummyType) || (pLimitId.pType.form == TFormCode.FcArray))
                Globals.Error(TErrorCode.ErrInvalidSubrangeType);
            else if (pLimitId.defn.how == TDefnCode.DcConstant)
            {

                //--Use the value of the constant identifier.
                if (pLimitId.pType == Globals.pIntegerType)
                    limit = sign == TTokenCode.TcMinus ? -pLimitId.defn.constant.value.integer : pLimitId.defn.constant.value.integer;
                else if (pLimitId.pType == Globals.pCharType)
                {
                    if (sign != TTokenCode.TcDummy)
                        Globals.Error(TErrorCode.ErrInvalidConstant);
                    limit = pLimitId.defn.constant.value.character;
                } else if (pLimitId.pType.form == TFormCode.FcEnum)
                {
                    if (sign != TTokenCode.TcDummy)
                        Globals.Error(TErrorCode.ErrInvalidConstant);
                    limit = pLimitId.defn.constant.value.integer;
                }
                pType = pLimitId.pType;
            } else
                Globals.Error(TErrorCode.ErrNotAConstantIdentifier);
            break;

            case TTokenCode.TcString:

            //--String limit:  Character type only.
            if (sign != TTokenCode.TcDummy)
                Globals.Error(TErrorCode.ErrInvalidConstant);

            if (Convert.ToString(pToken.String).Length != 3)
                Globals.Error(TErrorCode.ErrInvalidSubrangeType);

            limit = pToken.String[1];
            pType = Globals.pCharType;
            break;

            default:
            Globals.Error(TErrorCode.ErrMissingConstant);
            return pType; // don't get another token
        }

        GetToken();
        return pType;
    }
}