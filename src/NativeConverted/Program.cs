public static class Program
{
    static void Main ( string[] args )
    {
        //--Check the command line arguments.
        if (args.Length != 2)
        {
            Globals.DisplayError("Usage: compile <source file> <assembly file>");
            Globals.AbortTranslation(TAbortCode.AbortInvalidCommandLineArgs);
        }

        Globals.execFlag = false;

        //--Create the parser for the source file,
        //--and then parse the file.
        var pParser = new TParser(new TSourceBuffer(args[1]));
        TSymtabNode pProgramId = pParser.Parse();
        pParser?.Dispose();

        //--If there were no syntax errors, convert the symbol tables,
        //--and create and invoke the backend code generator.
        if (Globals.errorCount == 0)
        {
            Globals.vpSymtabs = new TSymtab[Globals.cntSymtabs];
            foreach (var pSt in Globals.pSymtabList)            
                pSt.Convert(Globals.vpSymtabs);

            TBackend pBackend = new TCodeGenerator(args[2]);
            pBackend.Go(pProgramId);

            Globals.vpSymtabs = null;            
        }
    }
}