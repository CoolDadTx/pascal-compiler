/*
 * Copyright © Michael Taylor (P3Net)
 * All Rights Reserved
 *
 * Mak 2nd Edition
 */
namespace P3Net.Compilers.Pascal.IO;

/// <summary>Source buffer subclass of TextInBuffer.</summary>
public class SourceBuffer : TextInBuffer
{
    public SourceBuffer ( string sourceFileName ) : base(sourceFileName, AbortCode.SourceFileOpenFailed)
    {
        //Initialize the list file and read the first source line
        if (Buffer.ListFlag)
            ListBuffer.List.Initialize(sourceFileName);

        GetLine();
    }

    /// <inheritdoc />
    protected override char GetLine ()
    {
        if (File.EndOfStream)
            pChar = Buffer.Eof;
        else //read the next source line and print it to the list file
        {
            Text = File.ReadLine();
            
        }

        return pChar;
    }       
}
