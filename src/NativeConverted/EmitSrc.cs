//fig 12-9
//  *************************************************************
//  *                                                           *
//  *   E M I T   S O U R C E   L I N E S                       *
//  *                                                           *
//  *   Emit source lines as comments in the assembly listing.  *
//  *                                                           *
//  *   CLASSES: TCodeGenerator                                 *
//  *                                                           *
//  *   FILE:    prog13-1/emitsrc.cpp                           *
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
//  StartComment    Start a new comment with a line number.
//
//      pString : ptr to the string to append
//--------------------------------------------------------------


//--------------------------------------------------------------
//  PutComment      Append a string to the assembly comment if
//                  it fits.  If not, emit the current comemnt
//                  and append the string to the next comment.
//
//      pString : ptr to the string to append
//--------------------------------------------------------------


//              ******************
//              *                *
//              *  Declarations  *
//              *                *
//              ******************

//--------------------------------------------------------------
//  EmitProgramHeaderComment    Emit the program header as a
//                              comment.
//
//      pProgramId : ptr to the program id's symbol table node
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitSubroutineHeaderComment     Emit a subroutine header as
//                                  a comment.
//
//      pRoutineId : ptr to the subroutine id's symtab node
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitSubroutineFormalsComment    Emit a formal parameter list
//                                  as a comment.
//
//      pParmId : ptr to the head of the formal parm id list
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitVarDeclComment      Emit variable declarations as
//                          comments.
//
//      pVarId : ptr to the head of the variable id list
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitTypeSpecComment     Emit a type specification as a
//                          comment.
//
//      pType : ptr to the type object
//--------------------------------------------------------------


//              ****************
//              *              *
//              *  Statements  *
//              *              *
//              ****************

//--------------------------------------------------------------
//  EmitStmtComment         Emit a statement as a comment.
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitAsgnOrCallComment   Emit an assignment statement or a
//                          procedure call as a comment.
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitREPEATComment   Emit a REPEAT statement as a comment.
//  EmitUNTILComment
//--------------------------------------------------------------



//--------------------------------------------------------------
//  EmitWHILEComment    Emit a WHILE statement as a comment.
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitIFComment       Emit an IF statement as a comment.
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitFORComment      Emit a FOR statement as a comment.
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitCASEComment     Emit a CASE statement as a comment.
//--------------------------------------------------------------


//              ******************
//              *                *
//              *  Expresssions  *
//              *                *
//              ******************

//--------------------------------------------------------------
//  EmitExprComment         Emit an expression as a comment.
//--------------------------------------------------------------


//endfig
