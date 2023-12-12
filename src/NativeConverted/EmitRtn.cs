//fig 13-3
//  *************************************************************
//  *                                                           *
//  *   C O D E   G E N E R A T O R   (Routines)                *
//  *                                                           *
//  *   Generating and emit assembly code for declared          *
//  *   procedures and functions.                               *
//  *                                                           *
//  *   CLASSES: TCodeGenerator                                 *
//  *                                                           *
//  *   FILE:    prog13-1/emitrtn.cpp                           *
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


//--------------------------------------------------------------
//  EmitProgramPrologue         Emit the program prologue.
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitProgramEpilogue         Emit the program epilogue.
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitMain            Emit code for the main routine.
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitMainPrologue    Emit the prologue for the main routine.
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitMainEpilogue    Emit the epilogue for the main routine.
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitRoutine         Emit code for a procedure or function.
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitRoutinePrologue         Emit the prologue for a
//                              procedure or function.
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitRoutineEpilogue         Emit the epilogue for a
//                              procedure or function.
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitSubroutineCall          Emit code for a call to a
//                              procedure or a function.
//
//      pRoutineId : ptr to the subroutine name's symtab node
//
//  Return: ptr to the call's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitDeclaredSubroutineCall   Emit code for a call to a
//                               declared procedure or function.
//
//      pRoutineId : ptr to the subroutine name's symtab node
//
//  Return: ptr to the call's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitActualParameters    Emit code for the actual parameters
//                          of a declared subroutine call.
//
//      pRoutineId : ptr to the subroutine name's symtab node
//--------------------------------------------------------------

//endfig
