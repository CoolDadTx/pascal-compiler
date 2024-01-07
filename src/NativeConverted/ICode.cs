//--------------------------------------------------------------
//  TIcode      Intermediate code subclass of TScanner.
//--------------------------------------------------------------
public class TIcode : TScanner
{
    private const int CodeSegmentSize = 4096;

    //private byte[] pCode = new byte[CodeSegmentSize]; // ptr to the code segment
    //private int cursor = 0; // ptr to current code location    
    private int cursor 
    {
        get => (int)pCode.Position;
        set => pCode.Position = value;
    }
    private MemoryStream pCode = new MemoryStream(CodeSegmentSize);
	private TSymtabNode pNode; // ptr to extracted symbol table node


	//--------------------------------------------------------------
	//  CheckBounds         Guard against code segment overflow.
	//
	//      size : number of bytes to append
	//--------------------------------------------------------------

	private void CheckBounds( int size )
	{
		//if ( cursor.Substring( size ) >= pCode[CodeSegmentSize] )        
        if (pCode.Position + size > pCode.Length)
		{
		    Globals.Error( TErrorCode.ErrCodeSegmentOverflow );
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

        //memcpy( ( object ) & xSymtab, ( object ) cursor, sizeof( short ) );
        //memcpy( ( object ) & xNode, ( object )( cursor + sizeof( short ) ), sizeof( short ) );
        //cursor += 2 * sizeof( short );
        xSymtab = pCode.ReadInt16();
        xNode = pCode.ReadInt16();

		return Globals.vpSymtabs[xSymtab].Get( xNode );
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
        //int length = icode.cursor - icode.pCode; // length of icode
        int length = icode.cursor;

        //--Copy icode.
        //pCode = cursor = new string( new char[length - 1] );
        //memcpy( pCode, icode.pCode, length );
        pCode = new MemoryStream(length);
        icode.pCode.CopyTo(pCode);        
	}

	public TIcode()
	{
        //pCode = cursor = new string( new char[CodeSegmentSize - 1] );
        pCode = new MemoryStream(CodeSegmentSize);
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
		if (Globals.errorCount > 0)
			return;

		var code = (byte)tc;
		CheckBounds(sizeof(byte));

        //memcpy((object)cursor, (object)&code, sizeof(char));
        //cursor += sizeof(char);
        pCode.WriteByte(code);        
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
		if (Globals.errorCount > 0)
			return;

		short xSymtab = (short)pNode.SymtabIndex();
		short xNode = pNode.NodeIndex();

		CheckBounds(2 * sizeof(short));

        //memcpy((object)cursor, (object)&xSymtab, sizeof(short));
        //memcpy((object)(cursor + sizeof(short)), (object)&xNode, sizeof(short));
        pCode.Write(xSymtab);
        pCode.Write(xNode);
	}

	//--------------------------------------------------------------
	//  InsertLineMarker    Insert a line marker into the
	//                      intermediate code just before the
	//                      last appended token code.
	//--------------------------------------------------------------

	public void InsertLineMarker()
	{
		if (Globals.errorCount > 0)
			return;

        //--Remember the last appended token code;
        //memcpy( ( object ) & lastCode, ( object ) cursor, sizeof( char ) );
        cursor -= sizeof(byte);
        var lastCode = pCode.ReadByte();
        cursor -= sizeof(byte);

		//--Insert a statement marker code
		//--followed by the current line number.
		var code = (byte)Globals.mcLineMarker;
		short number = (short)Globals.currentLineNumber;
		CheckBounds(sizeof(byte) + sizeof(short));

        //memcpy( ( object ) cursor, ( object ) & code, sizeof( char ) );
        //cursor += sizeof( char );
        //memcpy( ( object ) cursor, ( object ) & number, sizeof( short ) );
        //cursor += sizeof( short );
        pCode.Write(code);
        pCode.Write(number);

        //--Re-append the last token code;
        //memcpy( ( object ) cursor, ( object ) & lastCode, sizeof( char ) );
        //cursor += sizeof( char );
        pCode.Write(lastCode);
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
		if (Globals.errorCount > 0)
			return 0;

		//--Append the location marker code.
		var code = (byte)Globals.mcLocationMarker;
		CheckBounds(sizeof(byte));

        //memcpy( ( object ) cursor, ( object ) & code, sizeof( char ) );
        //cursor += sizeof( char );
        pCode.Write(code);

		//--Append 0 as a placeholder for the location offset.
		//--Remember the current location of the offset itself.
		short offset = 0;
		int atLocation = CurrentLocation();
		CheckBounds(sizeof(short));

        //memcpy( ( object ) cursor, ( object ) & offset, sizeof( short ) );
        //cursor += sizeof( short );
        pCode.Write(offset);

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

	public void FixupLocationMarker( short location )
	{
		//--Patch in the offset of the current token's location.
		var offset = CurrentLocation() - 1;

        //memcpy( ( object )( pCode.Substring( location ) ), ( object ) & offset, sizeof( short ) );
        pCode.Overwrite(offset, location);        
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
        //memcpy( ( object ) & offset, ( object ) cursor, sizeof( short ) );
        //cursor += sizeof( short );
        offset = pCode.ReadInt16();

		return offset;
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
		if (Globals.errorCount > 0)
			return;

		short offset = (short)(location & 0xffff);

		CheckBounds(sizeof(int) + sizeof(short));

        //memcpy( ( object ) cursor, ( object ) & value, sizeof( int ) );
        //cursor += sizeof( int );
        pCode.Write(value);

        //memcpy( ( object ) cursor, ( object ) & offset, sizeof( short ) );
        //cursor += sizeof( short );
        pCode.Write(offset);
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

        //memcpy( ( object ) & val, ( object ) cursor, sizeof( int ) );
        //cursor += sizeof( int );
        val = pCode.ReadInt32();

        //memcpy( ( object ) & offset, ( object ) cursor, sizeof( short ) );
        //cursor += sizeof( short );
        offset = pCode.ReadInt16();

		value = val;
		location = offset;
	}

	public void Reset()
	{
		//cursor = pCode;
        pCode.Seek(0, SeekOrigin.Begin);
	}
	public void GoTo( int location )
	{
		//cursor = pCode.Substring( location );
        pCode.Seek(location, SeekOrigin.Begin);
	}

	public int CurrentLocation() 
	{
        //return cursor - pCode;
        return (int)pCode.Position;
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
		byte code; // token code read from the file
		TTokenCode token;

		//--Loop to process any line markers
		//--and extract the next token code.
		do
		{
            //--First read the token code.    
            //memcpy( ( object ) & code, ( object ) cursor, sizeof( char ) );
            //cursor += sizeof( char );
            code = (byte)pCode.ReadByte();
		    token = ( TTokenCode ) code;

		    //--If it's a line marker, extract the line number.
		    if ( token == Globals.mcLineMarker )
		    {
			    short number;

                //memcpy( ( object ) & number, ( object ) cursor, sizeof( short ) );			    
                //cursor += sizeof( short );
                number = pCode.ReadInt16();
                Globals.currentLineNumber = number;
            }
		} while ( token == Globals.mcLineMarker );

		//--Determine the token class, based on the token code.
		switch ( token )
		{
		    case TTokenCode.TcNumber: pToken = numberToken; break;
		    case TTokenCode.TcString: pToken = stringToken; break;

		    case TTokenCode.TcIdentifier:
            {
                pToken = wordToken;
                pToken.code = TTokenCode.TcIdentifier;
                break;
            };

		    case Globals.mcLocationMarker:
            {
                pToken = specialToken;
                pToken.code = token;
                break;
            };

		    default:
            {
                if (token < TTokenCode.TcAND)
                {
                    pToken = specialToken;
                    pToken.code = token;
                } else
                {
                    pToken = wordToken; // reserved word
                    pToken.code = token;
                }
                break;
            };
		}

		//--Extract the symbol table node and set the token string.
		switch ( token )
		{
		    case TTokenCode.TcIdentifier:
		    case TTokenCode.TcNumber:
		    case TTokenCode.TcString:
			    pNode = GetSymtabNode();
			    pToken.String = pNode.String();
			    break;

		    case Globals.mcLocationMarker:
			    pNode = null;
                pToken.String = "";
			    break;

		    default:
			    pNode = null;
			    pToken.String = Globals.symbolStrings[code];
			    break;
		}

		return pToken;
	}
}