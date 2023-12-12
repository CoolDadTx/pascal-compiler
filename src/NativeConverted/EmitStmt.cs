//  *************************************************************
//  *                                                           *
//  *   C O D E   G E N E R A T O R   (Statements)              *
//  *                                                           *
//  *   Generating and emit assembly code for statements.       *
//  *                                                           *
//  *   CLASSES: TCodeGenerator                                 *
//  *                                                           *
//  *   FILE:    prog14-1/emitstmt.cpp                          *
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
//  EmitStatement       Emit code for a statement.
//--------------------------------------------------------------

//fig 14-1
//endfig

//--------------------------------------------------------------
//  EmitStatementList   Emit code for a statement list until
//                      the terminator token.
//
//      terminator : the token that terminates the list
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitAssignment      Emit code for an assignment statement.
//--------------------------------------------------------------


//fig 14-2
//--------------------------------------------------------------
//  EmitREPEAT      Emit code for a REPEAT statement:
//
//                      REPEAT <stmt-list> UNTIL <expr>
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitWHILE       Emit code for a WHILE statement:
//
//                      WHILE <expr> DO <stmt>
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitIF          Emit code for an IF statement:
//
//                      IF <expr> THEN <stmt-1>
//
//                  or:
//
//                      IF <expr> THEN <stmt-1> ELSE <stmt-2>
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitFOR         Emit code for a FOR statement:
//
//                      FOR <id> := <expr-1> TO|DOWNTO <expr-2>
//                          DO <stmt>
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitCASE        Emit code for a CASE statement:
//
//                      CASE <expr> OF
//                          <case-branch> ;
//                          ...
//                      END
//--------------------------------------------------------------

//endfig

//--------------------------------------------------------------
//  EmitCompound        Emit code for a compound statement:
//
//                          BEGIN <stmt-list> END
//--------------------------------------------------------------

