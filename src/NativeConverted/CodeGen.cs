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
    private readonly TAssemblyBuffer pAsmBuffer;

    //--Pointers to the list of all the float and string literals
    //--used in the source program.
    private TSymtabNode pFloatLitList;
    private TSymtabNode pStringLitList;
                                          
    
    //--Assembly buffer
    private string AsmText ()
    {
        return pAsmBuffer.Text();
    }
    private void Reset ()
    {
        pAsmBuffer.Reset();
    }
    private void Put ( char ch )
    {
        pAsmBuffer.Put(ch);
    }
    private void Put ( ref string pString )
    {
        pAsmBuffer.Put(pString);
    }
    private void PutLine ()
    {
        pAsmBuffer.PutLine();
    }
    private void PutLine ( ref string pText )
    {
        pAsmBuffer.PutLine(pText);
    }
    private void Advance ()
    {
        pAsmBuffer.Advance();
    }

    public TCodeGenerator ( string pAsmName )
    {
        this.pAsmBuffer = new TAssemblyBuffer(pAsmName, TAbortCode.AbortAssemblyFileOpenFailed);
        pFloatLitList = pStringLitList = null;
    }    
}