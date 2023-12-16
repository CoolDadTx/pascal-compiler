//              *********************
//              *                   *
//              *  Assembly Buffer  *
//              *                   *
//              *********************

//--------------------------------------------------------------
//  TAssemblyBuffer     Assembly language buffer subclass of
//                      TTextOutBuffer.
//--------------------------------------------------------------
public class TAssemblyBuffer : TTextOutBuffer
{
    private const int MaxLength = 72;
    //private enum AnonymousEnum3
    //{
    //    MaxLength = 72,
    //}

    private StreamWriter file; // assembly output file
    //private string pText; // assembly buffer pointer
    private readonly StringBuilder pText = new StringBuilder();
    private int textLength => pText.Length; // length of assembly comment

    //--------------------------------------------------------------
    //  Constructor     Construct an assembly buffer by opening the
    //                  output assembly file.
    //
    //      pAssemblyFileName : ptr to the name of the assembly file
    //      ac                : abort code to use if open failed
    //--------------------------------------------------------------
    public TAssemblyBuffer ( string pAssemblyFileName, TAbortCode ac )
    {
        //--Open the assembly output file.  Abort if failed.
        try
        {
            file = new StreamWriter(File.OpenWrite(pAssemblyFileName));
        } catch
        {            
            Globals.AbortTranslation(ac);
        };

        Reset();
    }

    public string Text ()
    {
        return pText.ToString();
    }

    public void Reset ()
    {
        pText.Clear();
        pText.Append(text);                
    }

    public void Put ( char ch )
    {
        pText.Append(ch);        
    }
    public override void PutLine ()
    {
        file.WriteLine(text);
        //file << text << std::endl;
		Reset();
    }

    //public new void PutLine ( string pText )
    //{
    //    base.PutLine(pText);
    //}

    //--------------------------------------------------------------
    //  Advance         Advance pText to the end of the buffer
    //                  contents.
    //--------------------------------------------------------------
    public void Advance ()
    {
        //while (*pText)
        //{
        //    ++pText;
        //    ++textLength;
        //}
    }

    public void Put ( string pString )
    {
        pText.Clear();
        pText.Append(pString);
        Advance();
    }

    public void Reset ( string pString )
    {
        Reset();
        Put(pString);
    }

    public bool Fit ( int length )
    {
        return (textLength + length) < MaxLength;
    }
}

partial class TCodeGenerator
{
    //--------------------------------------------------------------
    //  Reg                 Emit a register name.  Example:  ax
    //
    //      r : register code
    //--------------------------------------------------------------
    public void Reg ( TRegister r )
    {        
        Put(Globals.registers[(int)r]);
    }

    //--------------------------------------------------------------
    //  Operator            Emit an opcode.  Example:  add
    //
    //      opcode : operator code
    //--------------------------------------------------------------
    public void Operator ( TInstruction opcode )
    {
        Put('\t');
        Put(Globals.instructions[(int)opcode]);
    }

    //--------------------------------------------------------------
    //  Label               Emit a generic label constructed from
    //                      the prefix and the label index.
    //                                                              
    //                      Example:        $L_007
    //
    //      pPrefix : ptr to label prefix
    //      index   : index value
    //--------------------------------------------------------------
    public void Label ( string pPrefix, int index )
    {
        AsmText = String.Format("{0}_{1:D3}", pPrefix, index);
        Advance();
    }

    //--------------------------------------------------------------
    //  WordLabel           Emit a word label constructed from
    //                      the prefix and the label index.
    //                                                              
    //                      Example:        WORD PTR $F_007         
    //
    //      pPrefix : ptr to label prefix
    //      index   : index value
    //--------------------------------------------------------------
    public void WordLabel ( string pPrefix, int index )
    {
        AsmText = String.Format("WORD PTR {0}_{1:D3}", pPrefix, index);
        Advance();
    }

    //--------------------------------------------------------------
    //  HighDWordLabel      Emit a word label constructed from
    //                      the prefix and the label index and
    //                      offset by 2 to point to the high Word
    //                      of a double Word.
    //
    //                      Example:        WORD PTR $F_007+2
    //
    //      pPrefix : ptr to label prefix
    //      index   : index value
    //--------------------------------------------------------------
    public void HighDWordLabel ( string pPrefix, int index )
    {
        AsmText = String.Format("WORD PTR {0}_{1:D3}+2", pPrefix, index);
        Advance();
    }

    //--------------------------------------------------------------
    //  Byte                Emit a byte label constructed from
    //                      the id name and its label index.
    //
    //                      Example:        BYTE_PTR ch_007
    //
    //      pId : ptr to symbol table node
    //--------------------------------------------------------------
    public void Byte ( TSymtabNode pId )
    {
        AsmText = String.Format("BYTE PTR {0}_{1:D3}", pId.String(), pId.labelIndex);
        Advance();
    }

    //--------------------------------------------------------------
    //  Word                Emit a word label constructed from
    //                      the id name and its label index.
    //
    //                      Example:        WORD_PTR sum_007
    //
    //      pId : ptr to symbol table node
    //--------------------------------------------------------------
    public void Word ( TSymtabNode pId )
    {
        AsmText = String.Format("WORD PTR {0}_{1:D3}", pId.String(), pId.labelIndex);
        Advance();
    }

    //--------------------------------------------------------------
    //  HighDWord           Emit a word label constructed from     
    //                      the id name and its label index and
    //                      offset by 2 to point to the high word
    //                      of a double Word.                       
    //                                                              
    //                      Example:        WORD_PTR sum_007+2      
    //
    //      pId : ptr to symbol table node
    //--------------------------------------------------------------
    public void HighDWord ( TSymtabNode pId )
    {
        AsmText = String.Format("WORD PTR {0}_{1:D3}+2", pId.String(), pId.labelIndex);
        Advance();
    }

    //--------------------------------------------------------------
    //  ByteIndirect        Emit an indirect reference to a byte
    //                      via a register.
    //
    //                      Example:        BYTE PTR [bx]
    //
    //      r : register code
    //--------------------------------------------------------------
    public void ByteIndirect ( TRegister r )
    {
        AsmText = String.Format("BYTE PTR [{0}]", Globals.registers[(int)r]);
        Advance();
    }

    //--------------------------------------------------------------
    //  WordIndirect        Emit an indirect reference to a word
    //                      via a register.
    //                                                              
    //                      Example:        WORD PTR [bx]           
    //
    //      r : register code
    //--------------------------------------------------------------
    public void WordIndirect ( TRegister r )
    {
        AsmText = String.Format("WORD PTR [{0}]", Globals.registers[(int)r]);
        Advance();
    }

    //--------------------------------------------------------------
    //  HighDWordIndirect   Emit an indirect reference to the high
    //                      word of a double word via a register.
    //                                                              
    //                      Example:        WORD PTR [bx+2]
    //
    //      r : register code
    //--------------------------------------------------------------
    public void HighDWordIndirect ( TRegister r )
    {
        AsmText = String.Format("WORD PTR [{0}+2]", Globals.registers[(int)r]);
        Advance();
    }

    //--------------------------------------------------------------
    //  TaggedName          Emit an id name tagged with the id's
    //                      label index.
    //                                                              
    //                      Example:        x_007                   
    //
    //      pId : ptr to symbol table node
    //--------------------------------------------------------------
    public void TaggedName ( TSymtabNode pId )
    {
        AsmText = String.Format("{0}_{1:D3}", pId.String(), pId.labelIndex);
        Advance();
    }

    //--------------------------------------------------------------
    //  NameLit             Emit a literal name.
    //                                                              
    //                      Example:        _FloatConvert
    //
    //      pName : ptr to name
    //--------------------------------------------------------------
    public void NameLit ( string pName )
    {
        AsmText = String.Format("{0}", pName);
        Advance();
    }

    //--------------------------------------------------------------
    //  IntegerLit          Emit an integer as a string.
    //
    //      n : integer value
    //--------------------------------------------------------------
    public void IntegerLit ( int n )
    {
        AsmText = String.Format("{0:D}", n);
        Advance();
    }

    //--------------------------------------------------------------
    //  CharLit             Emit a character surrounded by single
    //                      quotes.
    //
    //      ch : character value
    //--------------------------------------------------------------
    public void CharLit ( char ch )
    {
        AsmText = String.Format("'{0}'", ch);
        Advance();
    }
}
