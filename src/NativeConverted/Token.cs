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

    public TTokenCode Code () => code;
    public TDataType Type () => type;
    public TDataValue Value () => value;
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
    public override bool IsDelimiter () => false;
    public override void Print()
	{
	}
}