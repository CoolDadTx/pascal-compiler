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
        int len = this.String.Length;

        code = TTokenCode.TcIdentifier; // first assume it's an identifier

        //--Is it the right length?
        if ((len >= Globals.minResWordLen) && (len <= Globals.maxResWordLen))
        {

            //--Yes.  Use the word length to pick the appropriate list
            //--from the reserved word table and check to see if the word
            //--is in there.
            foreach (var prw in Globals.rwTable[len])
            {
                if (String.Compare(this.String, prw.pString) == 0)
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

        var ps = new StringBuilder();
        
        //--Get the word.
        do
        {
            ps.Append(ch);            
            ch = buffer.GetChar();
        } while ((Globals.charCodeMap[ch] == TCharCode.CcLetter) || (Globals.charCodeMap[ch] == TCharCode.CcDigit));

        this.String = ps.ToString().ToLower(); // downshift its characters

        CheckForReservedWord();
    }
    public override bool IsDelimiter () => false;

    //--------------------------------------------------------------
    //  Print       Print the token to the list file.
    //-------------------------------------------------------------- 
    public override void Print ()
    {
        if (code == TTokenCode.TcIdentifier)
            Globals.list.text = String.Format("\t{0,-18} {1}", ">> identifier:", this.String);
        else
            Globals.list.text = String.Format("\t{0,-18} {1}", ">> reserved word:", this.String);

        Globals.list.PutLine();
    }
}