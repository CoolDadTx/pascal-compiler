//  *************************************************************
//  *                                                           *
//  *   P A R S E R                                             *
//  *                                                           *
//  *   Parse a Pascal source program.                          *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog11-1/parser.cpp                            *
//  *                                                           *
//  *   MODULE:  Parser                                         *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************

//--------------------------------------------------------------
//  TCaseItem           CASE item class.
//--------------------------------------------------------------

public class TCaseItem
{

    public TCaseItem next;
    public int labelValue;
    public int atBranchStmt;

    public TCaseItem ( ref TCaseItem pListHead )
    {
        next = pListHead; // insert at head of list
        pListHead = this;
        atBranchStmt = 0;
    }
}

//--------------------------------------------------------------
//  TParser             Parser class.
//--------------------------------------------------------------

public partial class TParser : IDisposable
{
    private readonly TTextScanner pScanner; // ptr to the scanner
    private TToken pToken; // ptr to the current token
    private TTokenCode token; // code of current token
    private TSymtabStack symtabStack = new TSymtabStack(); // symbol table stack
    private TIcode icode = new TIcode(); // icode buffer

    private void GetToken ()
    {
        pToken = pScanner.Get();
        token = pToken.Code();
    }

    private void GetTokenAppend ()
    {
        GetToken();
        icode.Put(token); // append token code to icode buffer
    }

    private void CondGetToken ( TTokenCode tc, TErrorCode ec )
    {
        //--Get another token only if the current one matches tc.
        if (tc == token)
            GetToken();
        else
            Globals.Error(ec); // error if no match
    }

    private void CondGetTokenAppend ( TTokenCode tc, TErrorCode ec )
    {
        //--Get another token only if the current one matches tc.
        if (tc == token)
            GetTokenAppend();
        else
            Globals.Error(ec); // error if no match
    }

    private void InsertLineMarker () => icode.InsertLineMarker();

    private int PutLocationMarker () => icode.PutLocationMarker();

    private void FixupLocationMarker ( int location ) => icode.FixupLocationMarker(location);

    private void PutCaseItem ( int value, int location ) => icode.PutCaseItem(value, location);

    private TSymtabNode SearchLocal ( string pString ) => symtabStack.SearchLocal(pString);

    private TSymtabNode SearchAll ( string pString ) => symtabStack.SearchAll(pString);

    private TSymtabNode EnterLocal ( string pString, TDefnCode dc = TDefnCode.DcUndefined ) => symtabStack.EnterLocal(pString, dc);

    private TSymtabNode EnterNewLocal ( string pString, TDefnCode dc = TDefnCode.DcUndefined ) => symtabStack.EnterNewLocal(pString, dc);

    private TSymtabNode Find ( string pString ) => symtabStack.Find(pString);

    private string CopyQuotedString ( string pQuotedString )
    {
        return pQuotedString.Substring(1, pQuotedString.Length - 2);
        //int length = pQuotedString.Length - 2; // don't count quotes
        //pString = Convert.ToString( pQuotedString[1] ).Substring( 0, length );	
    }

    //--------------------------------------------------------------
    //  Resync          Resynchronize the parser.  If the current
    //                  token is not in one of the token lists,
    //                  flag it as an error and then skip tokens
    //                  up to one that is in a list or end of file.
    //--------------------------------------------------------------

    private void Resync ( TTokenCode[] pList1, TTokenCode[] pList2 = default, TTokenCode[] pList3 = default )
    {
        //--Is the current token in one of the lists?
        var errorFlag = !Globals.TokenIn(token, pList1) && !Globals.TokenIn(token, pList2) && !Globals.TokenIn(token, pList3);

        if (errorFlag)
        {

            //--Nope.  Flag it as an error.
            var errorCode = token == TTokenCode.TcEndOfFile ? TErrorCode.ErrUnexpectedEndOfFile : TErrorCode.ErrUnexpectedToken;
            Globals.Error(errorCode);

            //--Skip tokens.
            while (!Globals.TokenIn(token, pList1) && !Globals.TokenIn(token, pList2) && !Globals.TokenIn(token, pList3) 
                    && (token != TTokenCode.TcPeriod) && (token != TTokenCode.TcEndOfFile))
                GetToken();

            //--Flag an unexpected end of file (if haven't already).
            if ((token == TTokenCode.TcEndOfFile) && (errorCode != TErrorCode.ErrUnexpectedEndOfFile))
                Globals.Error(TErrorCode.ErrUnexpectedEndOfFile);
        }
    }

    public TParser ( TTextInBuffer pBuffer )
    {
        pScanner = new TTextScanner(pBuffer);
    }

    public void Dispose () => pScanner?.Dispose();

    public TToken GetCommandToken ()
    {
        GetToken();
        return pToken;
    }


    //fig 11-12
    //--------------------------------------------------------------
    //  ParseCommandExpression      Parse the expression part of the
    //                              debugger "show" command.
    //
    //      pCmdIcode : ref to ptr to command icode
    //      cmdToken  : ref to current command token
    //--------------------------------------------------------------

    public void ParseCommandExpression ( ref TIcode pCmdIcode, ref TTokenCode cmdToken )
    {
        icode.Reset();
        GetTokenAppend(); // first token of expression

        //--Parse the expression.
        ParseExpression();
        pCmdIcode = new TIcode(icode); // copy of expression icode
        cmdToken = token; // transfer token to debugger

        //--Convert the current symbol table again in case new nodes
        //--were inserted by the expression.
        TSymtab pSymtab = symtabStack.GetCurrentSymtab();

        var items = pSymtab.NodeVector();
        if (items != null)
            Arrays.DeleteArray(items);
        //pSymtab.NodeVector()?.Dispose();
        pSymtab.Convert(Globals.vpSymtabs);
    }

    //--------------------------------------------------------------
    //  ParseCommandAssignment      Parse the assignment statement
    //                              part of the debugger "assign"
    //                              command.
    //
    //      pCmdIcode : ref to ptr to command icode
    //      cmdToken  : ref to current command token
    //
    //  Return: ptr to the symbol table node of the target variable
    //--------------------------------------------------------------

    public TSymtabNode ParseCommandAssignment ( ref TIcode pCmdIcode, ref TTokenCode cmdToken )
    {
        TSymtabNode pTargetId = null;

        icode.Reset();
        GetTokenAppend(); // first token of target variable

        if (token == TTokenCode.TcIdentifier)
        {
            pTargetId = Find(pToken.String);
            icode.Put(pTargetId);

            //--Parse the assignment.
            ParseAssignment(pTargetId);
            pCmdIcode = new TIcode(icode); // copy of statement icode
            cmdToken = token; // transfer token to debugger

            //--Convert the current symbol table again in case new
            //--nodes were inserted by the assignment expression.
            TSymtab pSymtab = symtabStack.GetCurrentSymtab();

            var items = pSymtab.NodeVector();
            if (items != null)
                Arrays.DeleteArray(items);
            //pSymtab.NodeVector().Dispose();
            pSymtab.Convert(Globals.vpSymtabs);
        } else
        {
            Globals.Error(TErrorCode.ErrUnexpectedToken);
        }

        return pTargetId;
    }

    public void DebugSetCurrentSymtab ( TSymtab pSymtab ) => symtabStack.SetCurrentSymtab(pSymtab);

    public TSymtabNode DebugSearchAll ( string pString ) => SearchAll(pString);


    //--------------------------------------------------------------
    //  Parse       Parse the source file.
    //
    //  Return: ptr to the program id's symbol table node
    //--------------------------------------------------------------

    public TSymtabNode Parse ()
    {
        //--Extract the first token and parse the program.
        GetToken();
        TSymtabNode pProgramId = ParseProgram();

        //--Print the parser's summary.
        Globals.list.PutLine();
        Globals.list.text = String.Format("{0,20:D} source lines.", Globals.currentLineNumber);
        Globals.list.PutLine();
        Globals.list.text = String.Format("{0,20:D} syntax errors.", Globals.errorCount);
        Globals.list.PutLine();

        return pProgramId;
    }
}