/*
 * Copyright © Michael Taylor (P3Net)
 * All Rights Reserved
 *
 * Mak 2nd Edition
 */
namespace P3Net.Compilers.Pascal.SourceListerHost;

public abstract class TextInBuffer
{
    protected TextInBuffer ( string filename, AbortCode ac)
    {
        throw new NotImplementedException();
    }

    public char Char () => (CurrentChar >= 0) ? Text[CurrentChar] : '\0';
    
    public char GetChar ()
    {
        throw new NotImplementedException();
    }

    public char PutBackChar ()
    {
        throw new NotImplementedException();
    }

    #region Protected Members

    /// <summary>Input text file.</summary>
    protected Stream file { get; private set; }

    /// <summary>File name.</summary>
    protected string FileName { get; private set; }

    /// <summary>Input buffer</summary>
    protected char[] Text { get; private set; } = new char[Buffer.MaxInputBufferSize];

    /// <summary>Pointer to current character in buffer</summary>
    protected int CurrentChar { get; private set; } = -1;//Char

    protected abstract string GetLine ();
    #endregion
}
