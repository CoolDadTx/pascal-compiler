/*
 * Copyright © Michael Taylor (P3Net)
 * All Rights Reserved
 *
 * Mak 2nd Edition
 */
namespace P3Net.Compilers.Pascal.IO;

//TODO: Rename
/// <summary>Abstract text output buffer class.</summary>
public abstract class TextOutBuffer
{
    //TODO: Is this really needed?
    //public char[] Text { get; private set; } = new char[Buffer.MaxInputBufferSize + 16];
    public string Text { get; set; }
    
    public abstract void PutLine ();

    public void PutLine ( string text )
    {
        Text = text;
        PutLine();
    }
}
