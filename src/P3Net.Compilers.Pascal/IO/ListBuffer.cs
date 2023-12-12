/*
 * Copyright © Michael Taylor (P3Net)
 * All Rights Reserved
 *
 * Mak 2nd Edition
 */
namespace P3Net.Compilers.Pascal.IO;

public class ListBuffer : TextOutBuffer
{
    #region Construction

    public static ListBuffer List { get; } = new ListBuffer();

    #endregion

    public void Initialize ( string filename ) => throw new NotImplementedException ();

    public override void PutLine () => throw new NotImplementedException ();

    public void PutLine ( string text, int lineNumber, int nestingLevel )
    {
        Text = $"{lineNumber:####} {nestingLevel}: {text}";
        PutLine();
    }

    #region Private Members

    private string _filename;
    private string _date;

    private int _pageNumber;
    private int _lineCount;

    private void PrintPageHeader () => throw new NotImplementedException();
    #endregion
}