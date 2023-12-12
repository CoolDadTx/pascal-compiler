/*
 * Copyright © Michael Taylor (P3Net)
 * All Rights Reserved
 *
 * Mak 2nd Edition
 */
namespace P3Net.Compilers.Pascal.IO;

public abstract class Buffer
{
    //TODO: Make larger or use StringBuilder
    public const int MaxInputBufferSize = 256;

    //TODO: Extern, should this be part of the base type instead?    
    public const char Eof = (char)0x7F;

    /// <summary>Virtual position of the current char in the input buffer (with tabs expanded)</summary>
    public static int InputPosition { get; set; }

    //TODO: Extern, should this be part of the base type instead?
    /// <summary><see langword="true"/> if list source lines, else false</summary>
    public static bool ListFlag { get; set; } = true;

    //TODO: Extern, should this be part of the base type instead?
    public static int Level { get; set; }
}
