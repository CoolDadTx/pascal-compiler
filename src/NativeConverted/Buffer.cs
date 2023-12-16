using System;

//--------------------------------------------------------------
//  TTextInBuffer       Abstract text input buffer class.
//--------------------------------------------------------------

public abstract class TTextInBuffer : System.IDisposable
{
    //protected std::fstream file = new std::fstream(); // input text file
    protected StreamReader file;
    protected readonly string pFileName; // ptr to the file name
                                         //protected string text = new string( new char[maxInputBufferSize] );
    protected string text; // input text buffer
    protected char pChar; // ptr to the current char
                          //   in the text buffer

    protected abstract char GetLine ();


    //--------------------------------------------------------------
    //  Constructor     Construct a input text buffer by opening the
    //                  input file.
    //
    //      pInputFileName : ptr to the name of the input file
    //      ac             : abort code to use if open failed
    //--------------------------------------------------------------

    public TTextInBuffer ( string pInputFileName, TAbortCode ac )
    {
        //--Copy the input file name.
        pFileName = pInputFileName;

        //--Open the input file.  Abort if failed.
        try
        {
            //file.open( pFileName, ios.in | ios._Nocreate );
            file = new StreamReader(pFileName);
        } catch
        {
            //if (!file.good())
            Globals.AbortTranslation(ac);
        };
    }

    public virtual void Dispose ()
    {
        file.Close();
    }

    public char Char ()
    {
        return pChar;
    }

    //--------------------------------------------------------------
    //  GetChar         Fetch and return the next character from the
    //                  text buffer.  If at the end of the buffer,
    //                  read the next source line.  If at the end of
    //                  the file, return the end-of-file character.
    //
    //  Return: next character from the source file
    //          or the end-of-file character
    //--------------------------------------------------------------

    public char GetChar ()
    {
        const int tabSize = 8; // size of tabs
        char ch; // character to return

        if (pChar == Globals.eofChar)
            return Globals.eofChar; // end of file
        else if (pChar == '\0')
            ch = GetLine(); // end of line
        else
        {
            //TODO: Need enumeration
            // next char
            ++pChar;
            ++Globals.inputPosition;
            ch = pChar;
        }

        //--If tab character, increment inputPosition to the next
        //--multiple of tabSize.
        if (ch == '\t')
            Globals.inputPosition += tabSize - Globals.inputPosition % tabSize;

        return ch;
    }

    //--------------------------------------------------------------
    //  PutBackChar     Put the current character back into the
    //                  input buffer so that the next call to
    //                  GetChar will fetch this character. (Only
    //                  called to put back a '.')
    //
    //  Return: the previous character
    //--------------------------------------------------------------

    public char PutBackChar ()
    {
        //TODO: Need enumeration
        --pChar;
        --Globals.inputPosition;

        return pChar;
    }
}

//--------------------------------------------------------------
//  TSourceBuffer       Source buffer subclass of TTextInBuffer.
//--------------------------------------------------------------

public class TSourceBuffer : TTextInBuffer
{

    //--------------------------------------------------------------
    //  GetLine         Read the next line from the source file, and
    //                  print it to the list file preceded by the
    //                  line number and the current nesting level.
    //
    //  Return: first character of the source line, or the
    //          end-of-file character if at the end of the file
    //--------------------------------------------------------------

    protected override char GetLine ()
    {
        //--If at the end of the source file, return the end-of-file char.
        if (file.EndOfStream)
            pChar = Globals.eofChar;

        //--Else read the next source line and print it to the list file.
        else
        {
            text = file.ReadLine();
            pChar = (text.Length > 0) ? text[0] : '\0'; // point to first source line char

            if (Globals.listFlag)
                Globals.list.PutLine(text, ++Globals.currentLineNumber, Globals.currentNestingLevel);
        }

        Globals.inputPosition = 0;
        return pChar;
    }


    //              *******************
    //              *                 *
    //              *  Source Buffer  *
    //              *                 *
    //              *******************

    //--------------------------------------------------------------
    //  Constructor     Construct a source buffer by opening the
    //                  source file.  Initialize the list file, and
    //                  read the first line from the source file.
    //
    //      pSourceFileName : ptr to name of source file
    //--------------------------------------------------------------

    public TSourceBuffer ( string pSourceFileName ) : base(pSourceFileName, TAbortCode.AbortSourceFileOpenFailed)
    {
        //--Initialize the list file and read the first source line.
        if (Globals.listFlag)
            Globals.list.Initialize(pSourceFileName);
        GetLine();
    }
}

//              ************
//              *          *
//              *  Output  *
//              *          *
//              ************

//--------------------------------------------------------------
//  TTextOutBuffer      Abstract text output buffer class.
//--------------------------------------------------------------

public abstract class TTextOutBuffer
{

    //public string text = new string( new char[maxInputBufferSize + 16] ); // output text buffer
    public string text;

    public abstract void PutLine ();

    public void PutLine ( string pText )
    {
        text = pText;
        PutLine();
    }
}

//--------------------------------------------------------------
//  TListBuffer         List buffer subclass of TTextOutBuffer.
//--------------------------------------------------------------

public class TListBuffer : TTextOutBuffer
{
    private string pSourceFileName; // ptr to source file name (for page header)

    //private string date = new string( new char[26] ); 
    private string date;    // date string for page header

    private int pageNumber; // current page number
    private int lineCount; // count of lines in the current page


    //--------------------------------------------------------------
    //  PrintPageHeader     Start a new page of the list file and
    //                      print the page header.
    //--------------------------------------------------------------

    private void PrintPageHeader ()
    {
        const char formFeedChar = '\f';

        Console.Write(formFeedChar);
        Console.Write("Page ");
        Console.Write(++pageNumber);
        Console.Write("   ");
        Console.Write(pSourceFileName);
        Console.Write("   ");
        Console.Write(date);
        Console.Write("\n");
        Console.Write("\n");

        lineCount = 0;
    }

    //--------------------------------------------------------------
    //  Initialize      Initialize the list buffer.  Set the date
    //                  for the page header, and print the first
    //                  header.
    //
    //      pFileName : ptr to source file name (for page header)
    //--------------------------------------------------------------

    public void Initialize ( string pFileName )
    {
        text = StringFunctions.ChangeCharacter(text, 0, '\0');
        pageNumber = 0;

        //--Copy the input file name.
        pSourceFileName = new string(new char[pFileName.Length]);
        pSourceFileName = pFileName;

        //--Set the date.
        date = DateTime.Today.ToString();

        PrintPageHeader();
    }

    //--------------------------------------------------------------
    //  PutLine         Print a line of text to the list file.
    //--------------------------------------------------------------

    public override void PutLine ()
    {
        //--Start a new page if the current one is full.
        if (Globals.listFlag && (lineCount == Globals.maxLinesPerPage))
            PrintPageHeader();

        //--Truncate the line if it's too long.
        text = StringFunctions.ChangeCharacter(text, Globals.maxPrintLineLength, '\0');

        //--Print the text line, and then blank out the text.
        Console.Write(text);
        Console.Write("\n");
        text = StringFunctions.ChangeCharacter(text, 0, '\0');

        ++lineCount;
    }

    public void PutLine ( string pText, int lineNumber, int nestingLevel )
    {
        text = String.Format("{0,4:D} {1:D}: {2}", lineNumber, nestingLevel, pText);
        PutLine();
    }
}