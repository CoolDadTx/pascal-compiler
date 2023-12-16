//  *************************************************************
//  *                                                           *
//  *   C O D E   G E N E R A T O R   (Header)                  *
//  *                                                           *
//  *   CLASSES: TAssemblyBuffer, TCodeGenerator                *
//  *                                                           *
//  *   FILE:    prog13-1/codegen.h                             *
//  *                                                           *
//  *   MODULE:  Code generator                                 *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************

//--------------------------------------------------------------
//  Emit0               Emit a no-operand instruction.
//--------------------------------------------------------------

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Emit0(opcode) { Operator(opcode); pAsmBuffer->PutLine(); }

//--------------------------------------------------------------
//  Emit1               Emit a one-operand instruction.
//--------------------------------------------------------------

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Emit1(opcode, operand1) { Operator(opcode); pAsmBuffer->Put('\t'); operand1; pAsmBuffer->PutLine(); }

//--------------------------------------------------------------
//  Emit2               Emit a two-operand instruction.
//--------------------------------------------------------------

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Emit2(opcode, operand1, operand2) { Operator(opcode); pAsmBuffer->Put('\t'); operand1; pAsmBuffer->Put(','); operand2; pAsmBuffer->PutLine(); }

//--------------------------------------------------------------
//  TRegister           Machine registers.
//--------------------------------------------------------------

public enum TRegister
{
    Ax,
    Ah,
    Al,
    Bx,
    Bh,
    Bl,
    Cx,
    Ch,
    Cl,
    Dx,
    Dh,
    Dl,
    Cs,
    Ds,
    Es,
    Ss,
    Sp,
    Bp,
    Si,
    Di,
}

//--------------------------------------------------------------
//  TInstruction        Assembly instructions.
//--------------------------------------------------------------

public enum TInstruction
{
    Mov,
    RepMovsb,
    Lea,
    Xchg,
    Cmp,
    RepeCmpsb,
    Pop,
    Push,
    And,
    Or,
    Xor,
    Neg,
    Incr,
    Decr,
    Add,
    Sub,
    Imul,
    Idiv,
    Cld,
    Call,
    Ret,
    Jmp,
    Jl,
    Jle,
    Je,
    Jne,
    Jge,
    Jg,
}

//--------------------------------------------------------------
//  TCodeGenerator      Code generator subclass of TBackend.
//--------------------------------------------------------------

public partial class TCodeGenerator : TBackend
{
    private const string StmtLabelPrefix = "$L";
    private const string FloatLabelPrefix = "$F";
    private const string StringLabelPrefix = "$S";
    private const string StaticLink = "$STATIC_LINK";
    private const string ReturnValue = "$RETURN_VALUE";
    private const string HighReturnValue = "$HIGH_RETURN_VALUE";
    private const string FloatNegate = "_FloatNegate";
    private const string FloatAdd = "_FloatAdd";
    private const string FloatSubtract = "_FloatSubtract";
    private const string FloatMultiply = "_FloatMultiply";
    private const string FloatDivide = "_FloatDivide";
    private const string FloatCompare = "_FloatCompare";
    private const string FloatConvert = "_FloatConvert";
    private const string ReadInteger = "_ReadInteger";
    private const string ReadReal = "_ReadReal";
    private const string ReadChar = "_ReadChar";
    private const string ReadLine = "_ReadLine";
    private const string WriteInteger = "_WriteInteger";
    private const string WriteReal = "_WriteReal";
    private const string WriteBoolean = "_WriteBoolean";
    private const string WriteChar = "_WriteChar";
    private const string WriteString = "_WriteString";
    private const string WriteLine = "_WriteLine";
    private const string StdEof = "_StdEof";
    private const string StdEoln = "_StdEoln";
    private const string StdAbs = "_StdAbs";
    private const string StdArctan = "_StdArctan";
    private const string StdCos = "_StdCos";
    private const string StdExp = "_StdExp";
    private const string StdLn = "_StdLn";
    private const string StdSin = "_StdSin";
    private const string StdSqrt = "_StdSqrt";
    private const string StdRound = "_StdRound";
    private const string StdTrunc = "_StdTrunc";

    private readonly TAssemblyBuffer pAsmBuffer;

    //--Pointers to the list of all the float and string literals
    //--used in the source program.
    private TSymtabNode pFloatLitList;
    private TSymtabNode pStringLitList;
                                              
    //--Assembly buffer
    private string AsmText 
    {
        get => pAsmBuffer.Text();
        set 
        {
            pAsmBuffer.Reset();
            pAsmBuffer.Put(value);
        }
        //return pAsmBuffer.Text();
    }
    private void Reset ()
    {
        pAsmBuffer.Reset();
    }
    private void Put ( char ch )
    {
        pAsmBuffer.Put(ch);
    }
    private void Put ( string pString )
    {
        pAsmBuffer.Put(pString);
    }
    private void PutLine ()
    {
        pAsmBuffer.PutLine();
    }
    private void PutLine ( string pText )
    {
        pAsmBuffer.PutLine(pText);
    }
    private void Advance ()
    {
        pAsmBuffer.Advance();
    }

    public TCodeGenerator ( string pAsmName )
    {
        pAsmBuffer = new TAssemblyBuffer(pAsmName, TAbortCode.AbortAssemblyFileOpenFailed);
    }    
}