//  *************************************************************
//  *                                                           *
//  *   P A R S E R   (Types #2)                                *
//  *                                                           *
//  *   Parse array and record type specifications.             *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog7-1/parstyp2.cpp                           *
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
    //  ParseArrayType      Parse an array type specification:
    //
    //                          ARRAY [ <index-type-list> ]
    //                              OF <elmt-type>
    //
    //  Return: ptr to type object
    //--------------------------------------------------------------
    public TType ParseArrayType ()
    {
        TType pArrayType = new TType(TFormCode.FcArray, 0, null);
        TType pElmtType = pArrayType;
        var indexFlag = false; // true if another array index, false if done

        //-- [
        GetToken();
        CondGetToken(TTokenCode.TcLBracket, TErrorCode.ErrMissingLeftBracket);

        //--Loop to parse each type spec in the index type list,
        //--seperated by commas.
        do
        {
            ParseIndexType(pElmtType);

            //-- ,
            Resync(Globals.tlIndexFollow, Globals.tlIndexStart);
            if ((token == TTokenCode.TcComma) || Globals.TokenIn(token, Globals.tlIndexStart))
            {

                //--For each type spec after the first, create an
                //--element type object.
                pElmtType = TType.SetType(ref pElmtType.array.pElmtType, new TType(TFormCode.FcArray, 0, null));
                CondGetToken(TTokenCode.TcComma, TErrorCode.ErrMissingComma);
                indexFlag = true;
            } else
            {
                indexFlag = false;
            }

        } while (indexFlag);

        //-- ]
        CondGetToken(TTokenCode.TcRBracket, TErrorCode.ErrMissingRightBracket);

        //--OF
        Resync(Globals.tlIndexListFollow, Globals.tlDeclarationStart, Globals.tlStatementStart);
        CondGetToken(TTokenCode.TcOF, TErrorCode.ErrMissingOF);

        //--Final element type.
        TType.SetType(ref pElmtType.array.pElmtType, ParseTypeSpec());

        //--Total byte size of the array.
        if (pArrayType.form != TFormCode.FcNone)
            pArrayType.size = ArraySize(pArrayType);

        return pArrayType;
    }

    //--------------------------------------------------------------
    //  ParseIndexType      Parse an array index type.
    //
    //      pArrayType : ptr to array type object
    //--------------------------------------------------------------
    public void ParseIndexType ( TType pArrayType )
    {
        if (Globals.TokenIn(token, Globals.tlIndexStart))
        {
            TType pIndexType = ParseTypeSpec();
            TType.SetType(ref pArrayType.array.pIndexType, pIndexType);

            switch (pIndexType.form)
            {

                //--Subrange index type
                case TFormCode.FcSubrange:
                pArrayType.array.elmtCount = pIndexType.subrange.max - pIndexType.subrange.min + 1;
                pArrayType.array.minIndex = pIndexType.subrange.min;
                pArrayType.array.maxIndex = pIndexType.subrange.max;
                return;

                //--Enumeration index type
                case TFormCode.FcEnum:
                pArrayType.array.elmtCount = pIndexType.enumeration.max + 1;
                pArrayType.array.minIndex = 0;
                pArrayType.array.maxIndex = pIndexType.enumeration.max;
                return;

                //--Error
                default:
                goto BadIndexType;
            }
        }

BadIndexType:

//--Error
        TType.SetType(ref pArrayType.array.pIndexType, Globals.pDummyType);
        pArrayType.array.elmtCount = 0;
        pArrayType.array.minIndex = pArrayType.array.maxIndex = 0;
        Globals.Error(TErrorCode.ErrInvalidIndexType);
    }

    //--------------------------------------------------------------
    //  ArraySize           Calculate the total byte size of an
    //                      array type by recursively calculating
    //                      the size of each dimension.
    //
    //      pArrayType : ptr to array type object
    //
    //  Return: byte size
    //--------------------------------------------------------------
    public int ArraySize ( TType pArrayType )
    {
        //--Calculate the size of the element type
        //--if it hasn't already been calculated.
        if (pArrayType.array.pElmtType.size == 0)
            pArrayType.array.pElmtType.size = ArraySize(pArrayType.array.pElmtType);

        return (pArrayType.array.elmtCount * pArrayType.array.pElmtType.size);
    }
    
    //--------------------------------------------------------------
    //  ParseRecordType     Parse a record type specification:
    //
    //                          RECORD
    //                              <id-list> : <type> ;
    //                              ...
    //                          END
    //
    //  Return: ptr to type object
    //--------------------------------------------------------------
    public TType ParseRecordType ()
    {
        TType pType = new TType(TFormCode.FcRecord, 0, null);
        pType.record.pSymtab = new TSymtab();

        //--Parse field declarations.
        GetToken();
        ParseFieldDeclarations(pType, 0);

        //--END
        CondGetToken(TTokenCode.TcEND, TErrorCode.ErrMissingEND);

        return pType;
    }
}