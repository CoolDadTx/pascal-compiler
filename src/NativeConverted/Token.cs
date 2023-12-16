//--------------------------------------------------------------
//  TToken              Abstract token class.
//--------------------------------------------------------------

public abstract class TToken
{

	protected TTokenCode code;
	protected TDataType type;
    //protected TDataValue value = new TDataValue();
    protected TDataValue value;
	//protected string str = new string( new char[maxInputBufferSize] );

	public TTokenCode Code()
	{
		return code;
	}
	public TDataType Type()
	{
		return type;
	}
	public TDataValue Value()
	{
		return value;
	}
	public string String { get; set; }
    //public string String()
	//{
    //		return str;
	//}

	public abstract void Get( TTextInBuffer buffer );
	public abstract bool IsDelimiter();
	public abstract void Print();
}

//--------------------------------------------------------------
//  TEOFToken           End-of-file token subclass of TToken.
//--------------------------------------------------------------

public class TEOFToken : TToken
{

	public TEOFToken()
	{
		code = TTokenCode.TcEndOfFile;
	}

	public override void Get( TTextInBuffer buffer )
	{
	}
	public override bool IsDelimiter()
	{
		return false;
	}
	public override void Print()
	{
	}
}