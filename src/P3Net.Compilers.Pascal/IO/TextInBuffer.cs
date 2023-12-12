/*
 * Copyright © Michael Taylor (P3Net)
 * All Rights Reserved
 *
 * Mak 2nd Edition
 */

/*
 * Copyright © Michael Taylor (P3Net)
 * All Rights Reserved
 *
 * Mak 2nd Edition
 */
namespace P3Net.Compilers.Pascal.IO;

//TODO: Rename
/// <summary>Abstract text input buffer class.</summary>
public abstract class TextInBuffer
{
    protected TextInBuffer ( string filename, AbortCode ac )
    {
        FileName = filename;

        try
        {
            File = new StreamReader(FileName);
        } catch (Exception e)
        {
            //TODO: Use exception
            AbortTranslation(ac);
        };
    }

    //TODO: Rename
    public char Char () => pChar;

    /// <summary>Fetch and return the next character from the text buffer.</summary>
    /// <returns>Next character from the source file or the end-of-file character.</returns>
    /// <remarks>
    /// If at the end of the buffer, read the next source line. If at the end of the file, return the end-of-file character.
    /// </remarks>
    public char GetChar ()
    {        
        if (pChar == Buffer.Eof)
            return Buffer.Eof;

        char ch;
        if (pChar == '\0')
            ch = GetLine();
        else
        {
            MoveNext();
            ++Buffer.InputPosition;
            ch = pChar;
        };

        //If tab character increment inputPosition to the next multiple of tabSize
        if (ch == '\t')
            Buffer.InputPosition += s_tabSize - Buffer.InputPosition % s_tabSize;

        return ch;
    }

    /// <summary>Put the current character back into the input buffer so that the next call to GetChar will fetch this character.
    /// </summary>
    /// <returns>The previous character.</returns>
    /// <remarks>
    /// Only called to put back a '.'.
    /// </remarks>
    public char PutBackChar ()
    {
        MovePrevious();
        --Buffer.InputPosition;

        return pChar;
    }

    #region Protected Members

    /// <summary>Input text file.</summary>
    protected StreamReader File { get; private set; }

    /// <summary>File name.</summary>
    protected string FileName { get; private set; }

    /// <summary>Input buffer</summary>
    protected string Text
    {
        get => _text;
        set {
            _text = value;
            _position = 0;
        }
    }
    private string _text; // = new char[Buffer.MaxInputBufferSize];    

    //TODO: Rename
    /// <summary>Pointer to current character in buffer</summary>
    protected char pChar => (_position >= 0 && _position < Text.Length) ? Text[_position] : '\0';

    //++pChar
    protected void MoveNext () => ++_position;

    //--pChar
    protected void MovePrevious() => --_position;

    /// <summary>Reads the next line from the source file.</summary>
    /// <returns>First character of the source line or the end-of-file character if at the end of the file.</returns>
    protected abstract char GetLine ();
    #endregion

    #region Private Members

    private const int s_tabSize = 4;
    private int _position = -1;
    #endregion
}
