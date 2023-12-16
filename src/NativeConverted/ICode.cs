//--------------------------------------------------------------
//  TIcode      Intermediate code subclass of TScanner.
//--------------------------------------------------------------
public class TIcode : TScanner
{
//C++ TO C# CONVERTER NOTE: Enums must be named in C#, so the following enum has been named AnonymousEnum:
	private enum AnonymousEnum
	{
		CodeSegmentSize = 4096
	}

	private string pCode; // ptr to the code segment
	private string cursor; // ptr to current code location
	private TSymtabNode pNode; // ptr to extracted symbol table node


	//--------------------------------------------------------------
	//  CheckBounds         Guard against code segment overflow.
	//
	//      size : number of bytes to append
	//--------------------------------------------------------------

	private void CheckBounds( int size )
	{
		if ( cursor.Substring( size ) >= pCode[( int )AnonymousEnum.CodeSegmentSize] )
		{
		Error( TErrorCode.ErrCodeSegmentOverflow );
		Globals.AbortTranslation( TAbortCode.AbortCodeSegmentOverflow );
		}
	}
	
    //--------------------------------------------------------------
	//  GetSymtabNode       Extract a symbol table node pointer
	//                      from the intermediate code.
	//
	//  Return: ptr to the symbol table node
	//--------------------------------------------------------------

	private TSymtabNode GetSymtabNode()
	{
		short xSymtab; // symbol table and node indexes
		short xNode;

//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) & xSymtab, ( object ) cursor, sizeof( short ) );
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) & xNode, ( object )( cursor + sizeof( short ) ), sizeof( short ) );
		cursor += 2 * sizeof( short );

		return vpSymtabs[xSymtab].Get( xNode );
	}


	//--------------------------------------------------------------
	//  Copy constructor    Make a copy of the icode.  Only copy as
	//                      many bytes of icode as necessary.
	//
	//      icode : ref to source icode
	//
	//--------------------------------------------------------------

	public TIcode( TIcode icode )
	{
		int length = icode.cursor - icode.pCode; // length of icode

		//--Copy icode.
		pCode = cursor = new string( new char[length - 1] );
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( pCode, icode.pCode, length );
	}
	public TIcode()
	{
		pCode = cursor = new string( new char[( int )AnonymousEnum2.CodeSegmentSize - 1] );
	}
   public new void Dispose()
   {
	   Arrays.DeleteArray( pCode );
	   base.Dispose();
   }


	//--------------------------------------------------------------
	//  Put(TTokenCode)     Append a token code to the intermediate
	//                      code.
	//
	//      tc    : token code
	//      pNode : ptr to symtab node, or NULL
	//--------------------------------------------------------------

	public void Put( TTokenCode tc )
	{
		if ( errorCount > 0 )
			return;

		char code = tc;
		CheckBounds( sizeof( char ) );
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) cursor, ( object ) & code, sizeof( char ) );
		cursor += sizeof( char );
	}

	//--------------------------------------------------------------
	//  Put(TSymtabNode *)      Append a symbol table node's symbol
	//                          table and node indexes to the
	//                          intermediate code.
	//
	//      pNode : ptr to symtab node
	//--------------------------------------------------------------

	public void Put( TSymtabNode pNode )
	{
		if ( errorCount > 0 )
			return;

		short xSymtab = pNode.SymtabIndex();
		short xNode = pNode.NodeIndex();

		CheckBounds( 2 * sizeof( short ) );
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) cursor, ( object ) & xSymtab, sizeof( short ) );
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object )( cursor + sizeof( short ) ), ( object ) & xNode, sizeof( short ) );
		cursor += 2 * sizeof( short );
	}

	//--------------------------------------------------------------
	//  InsertLineMarker    Insert a line marker into the
	//                      intermediate code just before the
	//                      last appended token code.
	//--------------------------------------------------------------

	public void InsertLineMarker()
	{
		if ( errorCount > 0 )
			return;

		//--Remember the last appended token code;
		char lastCode;
		cursor -= sizeof( char );
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) & lastCode, ( object ) cursor, sizeof( char ) );

		//--Insert a statement marker code
		//--followed by the current line number.
		char code = Globals.mcLineMarker;
		short number = currentLineNumber;
		CheckBounds( sizeof( char ) + sizeof( short ) );
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) cursor, ( object ) & code, sizeof( char ) );
		cursor += sizeof( char );
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) cursor, ( object ) & number, sizeof( short ) );
		cursor += sizeof( short );

		//--Re-append the last token code;
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) cursor, ( object ) & lastCode, sizeof( char ) );
		cursor += sizeof( char );
	}

	//--------------------------------------------------------------
	//  PutLocationMarker   Append a location marker to the
	//                      intermediate code.
	//
	//      location : location to mark
	//
	//  Return: location of the location marker's offset
	//--------------------------------------------------------------

	public int PutLocationMarker()
	{
		if ( errorCount > 0 )
			return 0;

		//--Append the location marker code.
		char code = Globals.mcLocationMarker;
		CheckBounds( sizeof( char ) );
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) cursor, ( object ) & code, sizeof( char ) );
		cursor += sizeof( char );

		//--Append 0 as a placeholder for the location offset.
		//--Remember the current location of the offset itself.
		short offset = 0;
		int atLocation = CurrentLocation();
		CheckBounds( sizeof( short ) );
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) cursor, ( object ) & offset, sizeof( short ) );
		cursor += sizeof( short );

		return atLocation;
	}

	//--------------------------------------------------------------
	//  FixupLocationMarker     Fixup a location marker in the
	//                          intermediate code by patching in the
	//                          offset of the current token's
	//                          location.
	//
	//      location : location of the offset to fix up
	//--------------------------------------------------------------

	public void FixupLocationMarker( int location )
	{
		//--Patch in the offset of the current token's location.
		short offset = CurrentLocation() - 1;
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object )( pCode.Substring( location ) ), ( object ) & offset, sizeof( short ) );
	}

	//--------------------------------------------------------------
	//  GetLocationMarker       Extract a location marker from the
	//                          intermediate code.
	//
	//  Return: location offset
	//--------------------------------------------------------------

	public int GetLocationMarker()
	{
		short offset; // location offset

		//--Extract the offset from the location marker.
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) & offset, ( object ) cursor, sizeof( short ) );
		cursor += sizeof( short );

		return ( int )offset;
	}
	
	//--------------------------------------------------------------
	//  PutCaseItem         Append a CASE item to the intermediate
	//                      code.
	//
	//      value   : CASE label value
	//      location: location of CASE branch statement
	//--------------------------------------------------------------

	public void PutCaseItem( int value, int location )
	{
		if ( errorCount > 0 )
			return;

		short offset = location & 0xffff;

		CheckBounds( sizeof( int ) + sizeof( short ) );
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) cursor, ( object ) & value, sizeof( int ) );
		cursor += sizeof( int );
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) cursor, ( object ) & offset, sizeof( short ) );
		cursor += sizeof( short );
	}

	//--------------------------------------------------------------
	//  GetCaseItem         Extract a CASE item from the
	//                      intermediate code.
	//
	//      value   : ref to CASE label value
	//      location: ref to location of CASE branch statement
	//--------------------------------------------------------------

	public void GetCaseItem( ref int value, ref int location )
	{
		int val;
		short offset;

//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) & val, ( object ) cursor, sizeof( int ) );
		cursor += sizeof( int );
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) & offset, ( object ) cursor, sizeof( short ) );
		cursor += sizeof( short );

		value = val;
		location = offset;
	}

	public void Reset()
	{
		cursor = pCode;
	}
	public void GoTo( int location )
	{
		cursor = pCode.Substring( location );
	}

	public int CurrentLocation()
	{
		return cursor - pCode;
	}
	public TSymtabNode SymtabNode()
	{
		return pNode;
	}

	//--------------------------------------------------------------
	//  Get                 Extract the next token from the
	//                      intermediate code.
	//
	//  Return: ptr to the extracted token
	//--------------------------------------------------------------

	public override TToken Get()
	{
		TToken pToken; // ptr to token to return
		char code; // token code read from the file
		TTokenCode token;

		//--Loop to process any line markers
		//--and extract the next token code.
		do
		{
		//--First read the token code.
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy( ( object ) & code, ( object ) cursor, sizeof( char ) );
		cursor += sizeof( char );
		token = ( TTokenCode ) code;

		//--If it's a line marker, extract the line number.
		if ( token == Globals.mcLineMarker )
		{
			short number;

//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
			memcpy( ( object ) & number, ( object ) cursor, sizeof( short ) );
			currentLineNumber = number;
			cursor += sizeof( short );
		}
		} while ( token == Globals.mcLineMarker );

		//--Determine the token class, based on the token code.
		switch ( token )
		{
		case TTokenCode.TcNumber:
			pToken = numberToken;
			break;
		case TTokenCode.TcString:
			pToken = stringToken;
			break;

		case TTokenCode.TcIdentifier:
			pToken = wordToken;
			pToken.code = TTokenCode.TcIdentifier;
			break;

		case Globals.mcLocationMarker:
			pToken = specialToken;
			pToken.code = token;
			break;

		default:
			if ( token < TTokenCode.TcAND )
			{
			pToken = specialToken;
			pToken.code = token;
			} else
			{
			pToken = wordToken; // reserved word
			pToken.code = token;
			}
			break;
		}

		//--Extract the symbol table node and set the token string.
		switch ( token )
		{
		case TTokenCode.TcIdentifier:
		case TTokenCode.TcNumber:
		case TTokenCode.TcString:
			pNode = GetSymtabNode();
			pToken.string = pNode.String();
			break;

		case Globals.mcLocationMarker:
			pNode = null;
			pToken.string[0] = '\0';
			break;

		default:
			pNode = null;
			pToken.string = Globals.symbolStrings[code];
			break;
		}

		return pToken;
	}
}