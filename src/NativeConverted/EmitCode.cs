//fig 13-2
//  *************************************************************
//  *                                                           *
//  *   E M I T   C O D E   S E Q U E N C E S                   *
//  *                                                           *
//  *   Routines for generating and emitting various assembly   *
//  *   language code sequences.                                *
//  *                                                           *
//  *   CLASSES: TCodeGenerator                                 *
//  *                                                           *
//  *   FILE:    prog13-1/emitcode.c                            *
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
//  Go                          Start the compilation.
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitStatementLabel      Emit a statement label constructed
//                          from the label index.
//
//                          Example:  $L_007:
//
//      index : index value
//--------------------------------------------------------------


//              ******************
//              *                *
//              *  Declarations  *
//              *                *
//              ******************

//--------------------------------------------------------------
//  EmitDeclarations    Emit code for the parameter and local
//                      variable declarations of a routine.
//
//      pRoutineId : ptr to the routine's symbol table node
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitStackOffsetEquate       Emit a stack frame offset equate
//                              for a parameter id or a local
//                              variable id.
//
//                              Examples: parm_007 EQU <pb+6>
//                                        var_008  EQU <bp-10>
//
//      pId : ptr to symbol table node
//--------------------------------------------------------------


//              **********************
//              *                    *
//              *  Loads and Pushes  *
//              *                    *
//              **********************

//--------------------------------------------------------------
//  EmitAdjustBP        Emit code to adjust register bp if
//                      necessary to point to the stack frame
//                      of an enclosing subroutine.
//
//      level : nesting level of enclosing subroutine's data
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitRestoreBP       Emit code to restore register bp if
//                      necessary to point to the current
//                      stack frame.
//
//      level : nesting level of enclosing subroutine's data
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitLoadValue       Emit code to load a scalar value
//                      into ax or dx:ax.
//
//      pId : ptr to symbol table node of parm or variable
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitLoadFloatLit    Emit code to load a float literal into
//                      dx:ax. Append the literal to the float
//                      literal list.
//
//      pNode : ptr to symbol table node of literal
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitPushStringLit   Emit code to push the address of a
//                      string literal onto the runtime stack.
//                      Append the literal to the string literal
//                      list.
//
//      pNode : ptr to symbol table node of literal
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitPushOperand             Emit code to push a scalar
//                              operand value onto the stack.
//
//      pType : ptr to type of value
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitPushAddress             Emit code to push an address
//                              onto the stack.
//
//      pId : ptr to symbol table node of parm or variable
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitPushReturnValueAddress      Emit code to push the   
//                                  address of the function
//                                  return value in the
//                                  stack frame.
//
//      pId : ptr to symbol table node of function
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitPromoteToReal        Emit code to convert integer    
//                           operands to real.
//
//      pType1 : ptr to type of first  operand
//      pType2 : ptr to type of second operand
//--------------------------------------------------------------

//endfig
