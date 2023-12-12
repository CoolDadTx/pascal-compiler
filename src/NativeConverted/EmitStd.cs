//  *************************************************************
//  *                                                           *
//  *   C O D E   G E N E R A T O R   (Standard Routines)       *
//  *                                                           *
//  *   Generating and emit assembly code for calls to the      *
//  *   standard procedures and functions.                      *
//  *                                                           *
//  *   CLASSES: TCodeGenerator                                 *
//  *                                                           *
//  *   FILE:    prog14-1/emitstd.cpp                           *
//  *                                                           *
//  *   MODULE:  Code generator                                 *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Emit0(opcode) { Operator(opcode); pAsmBuffer->PutLine(); }
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Emit1(opcode, operand1) { Operator(opcode); pAsmBuffer->Put('\t'); operand1; pAsmBuffer->PutLine(); }
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Emit2(opcode, operand1, operand2) { Operator(opcode); pAsmBuffer->Put('\t'); operand1; pAsmBuffer->Put(','); operand2; pAsmBuffer->PutLine(); }


//fig 14-18
//--------------------------------------------------------------
//  EmitStandardSubroutineCall  Emit code for a call to a
//                              standard procedure or function.
//
//      pRoutineId : ptr to the subroutine name's symtab node
//
//  Return: ptr to the call's type object
//--------------------------------------------------------------

//endfig

//fig 14-19
//--------------------------------------------------------------
//  EmitReadReadlnCall          Emit code for a call to read or
//                              readln.
//
//  Return: ptr to the dummy type object
//--------------------------------------------------------------

//endfig

//--------------------------------------------------------------
//  EmitWriteWritelnCall        Emit code for a call to write or
//                              writeln.
//
//  Return: ptr to the dummy type object
//--------------------------------------------------------------


//fig 14-20
//--------------------------------------------------------------
//  EmitEofEolnCall         Emit code for a call to eof or eoln.
//
//  Return: ptr to the boolean type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitAbsSqrCall           Emit code for a call to abs or sqr.
//
//  Return: ptr to the result's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitArctanCosExpLnSinSqrtCall       Emit code for a call to
//                                      arctan, cos, exp, ln,
//                                      sin, or sqrt.
//
//  Return: ptr to the real type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitPredSuccCall            Emit code for a call to pred
//                              or succ.
//
//  Return: ptr to the result's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitChrCall                 Emit code for a call to chr.
//
//  Return: ptr to the character type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitOddCall                 Emit code for a call to odd.
//
//  Return: ptr to the boolean type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitOrdCall                 Emit code for a call to ord.
//
//  Return: ptr to the integer type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitRoundTruncCall          Emit code for a call to round
//                              or trunc.
//
//  Return: ptr to the integer type object
//--------------------------------------------------------------

//endfig
