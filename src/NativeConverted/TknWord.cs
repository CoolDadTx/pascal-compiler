//--------------------------------------------------------------
//  Reserved word lists (according to word length)
//--------------------------------------------------------------
public class TResWord
{
	public string pString; // ptr to word string
	public TTokenCode code; // word code
}

//--------------------------------------------------------------
//  TWordToken          Word token subclass of TToken.
//--------------------------------------------------------------

public class TWordToken : TToken
{
    //--------------------------------------------------------------
    //  CheckForReservedWord    Is the word token a reserved word?
    //                          If yes, set the its token code to
    //                          the appropriate code.  If not, set
    //                          the token code to tcIdentifier.
    //--------------------------------------------------------------
    public void CheckForReservedWord ()
    {
        int len = str.Length;
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
        TResWord* prw = new TResWord(); // ptr to elmt of reserved word table

        code = TTokenCode.TcIdentifier; // first assume it's an identifier

        //--Is it the right length?
        if ((len >= minResWordLen) && (len <= maxResWordLen))
        {

            //--Yes.  Use the word length to pick the appropriate list
            //--from the reserved word table and check to see if the word
            //--is in there.
            for (prw = rwTable[len]; prw.pString != null; ++prw)
            {
                if (string.Compare(string, prw.pString) == 0)
                {
                    code = prw.code; // yes: set reserved word token code
                    break;
                }
            }
        }
    }

    //--------------------------------------------------------------
    //  Get     Extract a word token from the source and downshift
    //          its characters.  Check if it's a reserved word.
    //
    //      pBuffer : ptr to text input buffer
    //--------------------------------------------------------------
    public override void Get ( TTextInBuffer buffer )
    {
        char ch = buffer.Char(); // char fetched from input
                                 //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
        var ps = str;

        //--Get the word.
        do
        {
            *ps++= ch;
            ch = buffer.GetChar();
        } while ((charCodeMap[ch] == TCharCode.CcLetter) || (charCodeMap[ch] == TCharCode.CcDigit));

        *ps = '\0';
        string.ToLower(); // downshift its characters

        CheckForReservedWord();
    }
    public override bool IsDelimiter ()
    {
        return false;
    }

    //--------------------------------------------------------------
    //  Print       Print the token to the list file.
    //-------------------------------------------------------------- 
    public override void Print ()
    {
        if (code == TTokenCode.TcIdentifier)
            list.text = String.Format("\t{0,-18} {1}", ">> identifier:", string);
        else
            list.text = String.Format("\t{0,-18} {1}", ">> reserved word:", string);

        list.PutLine();
    }
}