using System;

//--------------------------------------------------------------
//  TTextInBuffer       Abstract text input buffer class.
//--------------------------------------------------------------

public abstract class TTextInBuffer : System.IDisposable
{

	protected std::fstream file = new std::fstream(); // input text file
	protected readonly string pFileName; // ptr to the file name
	protected string text = new string( new char[maxInputBufferSize] ); // input text buffer
	protected string pChar; // ptr to the current char
					//   in the text buffer

	protected abstract char GetLine();


	//--------------------------------------------------------------
	//  Constructor     Construct a input text buffer by opening the
	//                  input file.
	//
	//      pInputFileName : ptr to the name of the input file
	//      ac             : abort code to use if open failed
	//--------------------------------------------------------------

	public TTextInBuffer( string pInputFileName, TAbortCode ac )
	{
		this.pFileName = new string( new char[pInputFileName.Length] );
		//--Copy the input file name.
		pFileName = pInputFileName;

		//--Open the input file.  Abort if failed.
		file.open( pFileName, ios.in | ios._Nocreate );
		if ( !file.good() )
			AbortTranslation( ac );
	}

	public virtual void Dispose()
	{
	file.close();
	if ( pFileName != null )
		pFileName.Dispose();
	}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: char Char() const
	public char Char()
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

	public char GetChar()
	{
		const int tabSize = 8; // size of tabs
		char ch; // character to return

		if ( pChar == GlobalMembers.eofChar )
			return GlobalMembers.eofChar; // end of file
		else if ( pChar == '\0' )
			ch = GetLine(); // end of line
		else
		{ // next char
		++pChar;
		++GlobalMembers.inputPosition;
		ch = pChar;
		}

		//--If tab character, increment inputPosition to the next
		//--multiple of tabSize.
		if ( ch == '\t' )
			GlobalMembers.inputPosition += tabSize - GlobalMembers.inputPosition % tabSize;

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

	public char PutBackChar()
	{
		--pChar;
		--GlobalMembers.inputPosition;

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

	private override char GetLine()
	{
//C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
//		extern int lineNumber, currentNestingLevel;

		//--If at the end of the source file, return the end-of-file char.
		if ( file.eof() )
			pChar = GlobalMembers.eofChar;

		//--Else read the next source line and print it to the list file.
		else
		{
		file.getline( text, GlobalMembers.maxInputBufferSize );
		pChar = text; // point to first source line char

		if ( GlobalMembers.listFlag != 0 )
			GlobalMembers.list.PutLine( text, ++currentLineNumber, currentNestingLevel );
		}

		GlobalMembers.inputPosition = 0;
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

	public TSourceBuffer( string pSourceFileName ) : base( pSourceFileName, TAbortCode.AbortSourceFileOpenFailed )
	{
		//--Initialize the list file and read the first source line.
		if ( GlobalMembers.listFlag != 0 )
			GlobalMembers.list.Initialize( pSourceFileName );
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

	public string text = new string( new char[maxInputBufferSize + 16] ); // output text buffer

	public abstract void PutLine();

	public void PutLine( string pText )
	{
	text = pText;
	PutLine();
	}
}

//--------------------------------------------------------------
//  TListBuffer         List buffer subclass of TTextOutBuffer.
//--------------------------------------------------------------

public class TListBuffer : TTextOutBuffer, System.IDisposable
{
	private string pSourceFileName; // ptr to source file name (for page header)
	private string date = new string( new char[26] ); // date string for page header
	private int pageNumber; // current page number
	private int lineCount; // count of lines in the current page


	//--------------------------------------------------------------
	//  PrintPageHeader     Start a new page of the list file and
	//                      print the page header.
	//--------------------------------------------------------------

	private void PrintPageHeader()
	{
		const char formFeedChar = '\f';

		Console.Write( formFeedChar );
		Console.Write( "Page " );
		Console.Write( ++pageNumber );
		Console.Write( "   " );
		Console.Write( pSourceFileName );
		Console.Write( "   " );
		Console.Write( date );
		Console.Write( "\n" );
		Console.Write( "\n" );

		lineCount = 0;
	}

	public virtual void Dispose()
	{
		if ( pSourceFileName != null )
			pSourceFileName.Dispose();
	}


	//--------------------------------------------------------------
	//  Initialize      Initialize the list buffer.  Set the date
	//                  for the page header, and print the first
	//                  header.
	//
	//      pFileName : ptr to source file name (for page header)
	//--------------------------------------------------------------

	public void Initialize( string pFileName )
	{
		text = StringFunctions.ChangeCharacter( text, 0, '\0' );
		pageNumber = 0;

		//--Copy the input file name.
		pSourceFileName = new string( new char[pFileName.Length] );
		pSourceFileName = pFileName;

		//--Set the date.
		time_t timer = new time_t();
		time( timer );
		date = asctime( localtime( timer ) );
		date = StringFunctions.ChangeCharacter( date, date.Length - 1, '\0' ); // remove '\n' at end

		PrintPageHeader();
	}

	//--------------------------------------------------------------
	//  PutLine         Print a line of text to the list file.
	//--------------------------------------------------------------

	public override void PutLine()
	{
		//--Start a new page if the current one is full.
		if ( GlobalMembers.listFlag != 0 && ( lineCount == GlobalMembers.maxLinesPerPage ) )
			PrintPageHeader();

		//--Truncate the line if it's too long.
		text = StringFunctions.ChangeCharacter( text, GlobalMembers.maxPrintLineLength, '\0' );

		//--Print the text line, and then blank out the text.
		Console.Write( text );
		Console.Write( "\n" );
		text = StringFunctions.ChangeCharacter( text, 0, '\0' );

		++lineCount;
	}

	public new void PutLine( string pText )
	{
	base.PutLine( pText );
	}

	public void PutLine( string pText, int lineNumber, int nestingLevel )
	{
	text = string.Format( "{0,4:D} {1:D}: {2}", lineNumber, nestingLevel, pText );
	PutLine();
	}
}