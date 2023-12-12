/*
 * Copyright © Michael Taylor (P3Net)
 * All Rights Reserved
 *
 * Mak 2nd Edition
 */
namespace P3Net.Compilers.Pascal.SourceListerHost;

public abstract class Buffer
{
    //TODO: Make larger
    public const int MaxInputBufferSize = 256;

    //TODO: Extern
    public static char EofChar { get; set; }
    public static int InputPosition { get; set; }

    //TODO: boolean?
    public static int ListFlag { get; set; }

    public static int Level { get; set; }
}
