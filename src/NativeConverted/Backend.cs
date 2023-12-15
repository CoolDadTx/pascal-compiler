//  *************************************************************
//  *                                                           *
//  *   B A C K E N D                                           *
//  *                                                           *
//  *   Common backend routines.                                *
//  *                                                           *
//  *   CLASSES: TBackend                                       *
//  *                                                           *
//  *   FILE:    prog11-1/backend.cpp                           *
//  *                                                           *
//  *   MODULE:  Back end                                       *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************

//--------------------------------------------------------------
//  TBackend            Abstract back end class.
//--------------------------------------------------------------

public abstract class TBackend : IDisposable
{
    protected TToken pToken; // ptr to the current token
    protected TTokenCode token; // code of current token
    protected TIcode pIcode; // ptr to current icode
    protected TSymtabNode pNode; // ptr to symtab node

    //--Intermediate code save area
    protected TToken[] pSaveToken;
    protected TTokenCode saveToken;
    protected string pSaveTokenString;
    protected TIcode pSaveIcode;
    protected TSymtabNode pSaveNode;
    protected int saveLocation;
    protected int saveLineNumber;

    public virtual void Dispose()
    { 
    }
    
    protected void GetToken ()
    {
        pToken = pIcode.Get();
        token = pToken.Code();
        pNode = pIcode.SymtabNode();
    }

    protected void GoTo ( int location )
    {
        pIcode.GoTo(location);
    }

    protected int CurrentLocation ()
    {
        return pIcode.CurrentLocation();
    }

    protected int GetLocationMarker ()
    {
        return pIcode.GetLocationMarker();
    }

    protected void GetCaseItem ( ref int value, ref int location )
    {
        pIcode.GetCaseItem(ref value, ref location);
    }


    //--------------------------------------------------------------
    //  SaveState           Save the current state of the
    //                      intermediate code.
    //--------------------------------------------------------------

    protected void SaveState ()
    {
        pSaveToken = pToken;
        saveToken = token;
        pSaveIcode = pIcode;
        pSaveNode = pNode;
        saveLocation = CurrentLocation();
        saveLineNumber = currentLineNumber;

        pSaveTokenString = new string(new char[Convert.ToString(pToken.String()).Length]);
        pSaveTokenString = pToken.String();
    }

    //--------------------------------------------------------------
    //  RestoreState        Restore the current state of the
    //                      intermediate code.
    //--------------------------------------------------------------

    protected void RestoreState ()
    {
        pToken = pSaveToken;
        token = saveToken;
        pIcode = pSaveIcode;
        pNode = pSaveNode;
        GoTo(saveLocation);
        currentLineNumber = saveLineNumber;

        pToken.String() = pSaveTokenString;
        pSaveTokenString = null;
    }

    public abstract void Go ( TSymtabNode pRoutineId );
}
