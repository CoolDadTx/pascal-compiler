//--------------------------------------------------------------
//  TFormCode           Form code: What form of data structure.
//--------------------------------------------------------------
public enum TFormCode
{
	FcNone,
	FcScalar,
	FcEnum,
	FcSubrange,
	FcArray,
	FcRecord,
}

//--------------------------------------------------------------
//  TType               Type class.
//--------------------------------------------------------------
public class TType : /*TypeDef,*/ IDisposable
{
	private int refCount; // reference count

	public TFormCode form; // form code
	public int size; // byte size of type
	public TSymtabNode pTypeId; // ptr to symtab node of type identifier    

    //Union in C++
    //TODO: Public fields for now (private fields in code)
    public EnumerationTypeDef enumeration;
    public SubrangeTypeDef subrange;
    public ArrayTypeDef array;
    public RecordTypeDef record;
    //--General and string type constructors.

    //--------------------------------------------------------------
    //  Constructors    General:
    //
    //      fc    : form code
    //      s     : byte size of type
    //      pNode : ptr to symbol table node of type identifier
    //
    //                  String: unnamed string type
    //
    //      length : string length
    //--------------------------------------------------------------

    //--General
    public TType( TFormCode fc, int s, TSymtabNode pId )
	{
		form = fc;
		size = s;
		pTypeId = pId;
		refCount = 0;

		switch ( fc )
		{
		case TFormCode.FcSubrange:
			subrange.pBaseType = null;
			break;

		case TFormCode.FcArray:
			array.pIndexType = array.pElmtType = null;
			break;

		default:
			break;
		}
	}

	//--String
	public TType( int length )
	{
		form = TFormCode.FcArray;
		size = length;
		pTypeId = null;
		refCount = 0;

		array.pIndexType = array.pElmtType = null;
		SetType( ref array.pIndexType, new TType( TFormCode.FcSubrange, sizeof( int ), null ) );
		SetType( ref array.pElmtType, Globals.pCharType );
		array.elmtCount = length;

		//--Integer subrange index type, range 1..length
		SetType( ref array.pIndexType.subrange.pBaseType, Globals.pIntegerType );
		array.pIndexType.subrange.min = 1;
		array.pIndexType.subrange.max = length;
	}


   //--------------------------------------------------------------
   //  Destructor      Delete the allocated objects according to
   //                  the form code.  Note that the objects
   //                  pointed to by enumeration.pConstIds and by
   //                  subrange.pBaseType are deleted along with
   //                  the symbol tables that contain their
   //                  identifiers.
   //--------------------------------------------------------------

   public void Dispose()
   {
	   switch ( form )
	   {

	   case TFormCode.FcSubrange:

		   //--Subrange:  Delete the base type object.
		   RemoveType( ref subrange.pBaseType );
		   break;

	   case TFormCode.FcArray:

		   //--Array:  Delete the index and element type objects.
		   RemoveType( ref array.pIndexType );
		   RemoveType( ref array.pElmtType );
		   break;

	   case TFormCode.FcRecord:

		   //--Record:  Delete the record fields symbol table.
		   if ( record.pSymtab != null )
			   record.pSymtab.Dispose();
		   break;

	   default:
		   break;
	   }
   }

    public bool IsScalar () => (form != TFormCode.FcArray) && (form != TFormCode.FcRecord);

    public TType Base () => form == TFormCode.FcSubrange ? subrange.pBaseType : this;

    public enum TVerbosityCode
	{
		VcVerbose,
		VcTerse
	}


	//--------------------------------------------------------------
	//  PrintTypeSpec       Print information about a type
	//                      specification for the cross-reference.
	//
	//      vc : vcVerbose or vcTerse to control the output
	//--------------------------------------------------------------

	public void PrintTypeSpec( TVerbosityCode vc )
	{
		//--Type form and size
		Globals.list.text = String.Format( "{0}, size {1:D} bytes.  Type identifier: ", Globals.formStrings[( int )form], size );

		//--Type identifier
		if ( pTypeId != null )
            Globals.list.text += pTypeId.String();
		else
		{
            Globals.list.text += "<unnamed>";
		    vc = TVerbosityCode.VcVerbose; // verbose output for unnamed types
		}
        Globals.list.PutLine();

		//--Print the information for the particular type.
		switch ( form )
		{
		case TFormCode.FcEnum:
			PrintEnumType( vc );
			break;
		case TFormCode.FcSubrange:
			PrintSubrangeType( vc );
			break;
		case TFormCode.FcArray:
			PrintArrayType( vc );
			break;
		case TFormCode.FcRecord:
			PrintRecordType( vc );
			break;
		}
	}

	//--------------------------------------------------------------
	//  PrintEnumType       Print information about an enumeration
	//                      type for the cross-reference.
	//
	//      vc : vcVerbose or vcTerse to control the output
	//--------------------------------------------------------------

	public void PrintEnumType( TVerbosityCode vc )
	{
		if ( vc == TVerbosityCode.VcTerse )
			return;

        //--Print the names and values of the enumeration
        //--constant identifiers.
        Globals.list.PutLine( "--- Enumeration Constant Identifiers " + "(value = name) ---" );
		for ( var pConstId = enumeration.pConstIds; pConstId != null; pConstId = pConstId.next )
		{
            Globals.list.text = String.Format( "    {0:D} = {1}", pConstId.defn.constant.value.integer, pConstId.String() );
            Globals.list.PutLine();
		}
	}

	//--------------------------------------------------------------
	//  PrintSubrangeType   Print information about a subrange
	//                      type for the cross-reference.
	//
	//      vc : vcVerbose or vcTerse to control the output
	//--------------------------------------------------------------

	public void PrintSubrangeType( TVerbosityCode vc )
	{
		if ( vc == TVerbosityCode.VcTerse )
			return;

        //--Subrange minimum and maximum values
        Globals.list.text = String.Format( "Minimum value = {0:D}, maximum value = {1:D}", subrange.min, subrange.max );
        Globals.list.PutLine();

		//--Base range type
		if (subrange.pBaseType != null)
		{
            Globals.list.PutLine( "--- Base Type ---" );
		    subrange.pBaseType.PrintTypeSpec( TVerbosityCode.VcTerse );
		}
	}

	//--------------------------------------------------------------
	//  PrintArrayType      Print information about an array
	//                      type for the cross-reference.
	//
	//      vc : vcVerbose or vcTerse to control the output
	//--------------------------------------------------------------

	public void PrintArrayType( TVerbosityCode vc )
	{
		if ( vc == TVerbosityCode.VcTerse )
			return;

        //--Element count
        Globals.list.text = String.Format( "{0:D} elements", array.elmtCount );
        Globals.list.PutLine();

		//--Index type
		if (array.pIndexType != null)
		{
            Globals.list.PutLine( "--- INDEX TYPE ---" );
		    array.pIndexType.PrintTypeSpec( TVerbosityCode.VcTerse );
		}

		//--Element type
		if (array.pElmtType != null)
		{
            Globals.list.PutLine( "--- ELEMENT TYPE ---" );
		    array.pElmtType.PrintTypeSpec( TVerbosityCode.VcTerse );
		}
	}

	//--------------------------------------------------------------
	//  PrintRecordType     Print information about a record
	//                      type for the cross-reference.
	//
	//      vc : vcVerbose or vcTerse to control the output
	//--------------------------------------------------------------

	public void PrintRecordType( TVerbosityCode vc )
	{
		if ( vc == TVerbosityCode.VcTerse )
			return;

        //--Print the names and values of the record field identifiers.
        Globals.list.PutLine( "--- Record Field Identifiers (offset : name) ---" );
        Globals.list.PutLine();
		for (var pFieldId = record.pSymtab.Root(); pFieldId != null; pFieldId = pFieldId.next )
		{
            Globals.list.text = String.Format( "    {0:D} : {1}", pFieldId.defn.data.offset, pFieldId.String() );
            Globals.list.PutLine();
		    pFieldId.PrintVarOrField();
		}
	}

	//--------------------------------------------------------------
	//  SetType     Set the target type.  Increment the reference
	//              count of the source type.
	//
	//      pTargetType : ref to ptr to target type object
	//      pSourceType : ptr to source type object
	//
	//  Return: ptr to source type object
	//--------------------------------------------------------------
	public static TType SetType( ref TType pTargetType, TType pSourceType )
	{
		if (pTargetType == null)
			RemoveType(ref pTargetType);

		++pSourceType.refCount;
		pTargetType = pSourceType;

		return pSourceType;
	}

	//--------------------------------------------------------------
	//  RemoveType  Decrement a type object's reference count, and
	//              delete the object and set its pointer to NULL
	//              if the count becomes 0.
	//
	//      pType : ref to ptr to type object
	//--------------------------------------------------------------
	public static void RemoveType( ref TType pType )
	{
		if (pType != null && (--pType.refCount == 0))
		{
            pType?.Dispose();
		    pType = null;
		}
	}

	//              ************************
	//              *                      *
	//              *  Type Compatibility  *
	//              *                      *
	//              ************************

	//--------------------------------------------------------------
	//  CheckRelOpOperands  Check that the types of the two operands
	//                      of a relational operator are compatible.
	//                      Flag an incompatible type error if not.
	//
	//      pType1 : ptr to the first  operand's type object
	//      pType2 : ptr to the second operand's type object
	//--------------------------------------------------------------

	public static void CheckRelOpOperands( TType pType1, TType pType2 )
	{
		pType1 = pType1.Base();
		pType2 = pType2.Base();

		//--Two identical scalar or enumeration types.
		if ( ( pType1 == pType2 ) && ( ( pType1.form == TFormCode.FcScalar ) || ( pType1.form == TFormCode.FcEnum ) ) )
		return;

		//--One integer operand and one real operand.
		if ( ( ( pType1 == Globals.pIntegerType ) && ( pType2 == Globals.pRealType ) ) || ( ( pType2 == Globals.pIntegerType ) && ( pType1 == Globals.pRealType ) ) )
		return;

		//--Two strings of the same length.
		if ( ( pType1.form == TFormCode.FcArray ) && ( pType2.form == TFormCode.FcArray ) && ( pType1.array.pElmtType == Globals.pCharType ) && ( pType2.array.pElmtType == Globals.pCharType ) && ( pType1.array.elmtCount == pType2.array.elmtCount ) )
		return;

		//--Else:  Incompatible types.
		Globals.Error( TErrorCode.ErrIncompatibleTypes );
	}

	//--------------------------------------------------------------
	//  CheckIntegerOrReal  Check that the type of each operand is
	//                      either integer or real.  Flag an
	//                      incompatible type error if not.
	//
	//      pType1 : ptr to the first  operand's type object
	//      pType2 : ptr to the second operand's type object or NULL
	//--------------------------------------------------------------

	public static void CheckIntegerOrReal( TType pType1, TType pType2 = null )
	{
		pType1 = pType1.Base();
		if ( ( pType1 != Globals.pIntegerType ) && ( pType1 != Globals.pRealType ) )
            Globals.Error( TErrorCode.ErrIncompatibleTypes );

		if ( pType2 != null )
		{
		pType2 = pType2.Base();
		if ( ( pType2 != Globals.pIntegerType ) && ( pType2 != Globals.pRealType ) )
			Globals.Error( TErrorCode.ErrIncompatibleTypes );
		}
	}

	//--------------------------------------------------------------
	//  CheckBoolean        Check that the type of each operand is
	//                      boolean.  Flag an incompatible type
	//                      error if not.
	//
	//      pType1 : ptr to the first  operand's type object
	//      pType2 : ptr to the second operand's type object or NULL
	//--------------------------------------------------------------

	public static void CheckBoolean( TType pType1, TType pType2 = null )
	{
		if ( ( pType1.Base() != Globals.pBooleanType ) || (pType2 != null && (pType2.Base() != Globals.pBooleanType)) )
            Globals.Error( TErrorCode.ErrIncompatibleTypes );
	}

	//--------------------------------------------------------------
	//  CheckAssignmentTypeCompatible   Check that a value's type is
	//                                  assignment compatible with
	//                                  the target's type.  Flag an
	//                                  error if not.
	//
	//      pTargetType : ptr to the target's type object
	//      pValueType  : ptr to the value's  type object
	//      ec          : error code
	//--------------------------------------------------------------

	public static void CheckAssignmentTypeCompatible( TType pTargetType, TType pValueType, TErrorCode ec )
	{
		pTargetType = pTargetType.Base();
		pValueType = pValueType.Base();

		//--Two identical types.
		if ( pTargetType == pValueType )
			return;

		//--real := integer
		if ( ( pTargetType == Globals.pRealType ) && ( pValueType == Globals.pIntegerType ) )
			return;


		//--Two strings of the same length.
		if ( ( pTargetType.form == TFormCode.FcArray ) && ( pValueType.form == TFormCode.FcArray ) && ( pTargetType.array.pElmtType == Globals.pCharType ) && ( pValueType.array.pElmtType == Globals.pCharType ) && ( pTargetType.array.elmtCount == pValueType.array.elmtCount ) )
		return;

		Globals.Error( ec );
	}


	//--------------------------------------------------------------
	//  IntegerOperands     Check that the types of both operands
	//                      are integer.
	//
	//      pType1 : ptr to the first  operand's type object
	//      pType2 : ptr to the second operand's type object
	//
	//  Return: true if yes, false if no
	//--------------------------------------------------------------

	public static bool IntegerOperands( TType pType1, TType pType2 )
	{
		pType1 = pType1.Base();
		pType2 = pType2.Base();

		return ( pType1 == Globals.pIntegerType ) && ( pType2 == Globals.pIntegerType );
	}

	//--------------------------------------------------------------
	//  RealOperands        Check that the types of both operands
	//                      are real, or that one is real and the
	//                      other is integer.
	//
	//      pType1 : ptr to the first  operand's type object
	//      pType2 : ptr to the second operand's type object
	//
	//  Return: true if yes, false if no
	//--------------------------------------------------------------

	public static bool RealOperands( TType pType1, TType pType2 )
	{
		pType1 = pType1.Base();
		pType2 = pType2.Base();

		return ( ( pType1 == Globals.pRealType ) && ( pType2 == Globals.pRealType ) ) || ( ( pType1 == Globals.pRealType ) && ( pType2 == Globals.pIntegerType ) ) || ( ( pType2 == Globals.pRealType ) && ( pType1 == Globals.pIntegerType ) );
	}
}

//From union in C++
//public class TypeDef : OneOfBase<EnumerationTypeDef, SubrangeTypeDef, ArrayTypeDef, RecordTypeDef>
//{
//    public TypeDef ( OneOf<EnumerationTypeDef, SubrangeTypeDef, ArrayTypeDef, RecordTypeDef> input ) : base(input)
//    {
//    }

//    public EnumerationTypeDef enumeration { get; set; }
//    public SubrangeTypeDef subrange { get; set; }
//    public ArrayTypeDef array { get; set; }
//    public RecordTypeDef record { get; set; }
//}

public struct EnumerationTypeDef
{
    public TSymtabNode pConstIds { get; set; } //ptr to list of const id nodes
    public int max { get; set; }
}
public struct SubrangeTypeDef
{
    public TType pBaseType; //ptr to base type object
    public int min { get; set; }
    public int max { get; set; } //min and max subrange limit values
}
public struct ArrayTypeDef
{
    public TType pIndexType; // ptr to index type object
    public TType pElmtType; // ptr to elmt  type object
    public int minIndex { get; set; }
    public int maxIndex { get; set; } // min and max index values
    public int elmtCount { get; set; } // count of array elmts
}
public struct RecordTypeDef
{
    public TSymtab pSymtab { get; set; } // ptr to record fields symtab
}