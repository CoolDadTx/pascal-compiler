using System;

//fig 13-6
//  *************************************************************
//  *                                                           *
//  *   C O D E   G E N E R A T O R   (Expressions)             *
//  *                                                           *
//  *   Generating and emit assembly code for expressions.      *
//  *                                                           *
//  *   CLASSES: TCodeGenerator                                 *
//  *                                                           *
//  *   FILE:    prog13-1/emitexpr.cpp                          *
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
//  EmitExpression  Emit code for an expression (binary
//                  relational operators = < > <> <= and >= ).
//
//  Return: ptr to expression's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitSimpleExpression    Emit code for a simple expression
//                          (unary operators + or -
//                          and binary operators + -
//                          and OR).
//
//  Return: ptr to expression's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitTerm            Emit code for a term (binary operators
//                      * / DIV tcMOD and AND).
//
//  Return: ptr to term's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitFactor      Emit code for a factor (identifier, number,
//                  string, NOT <factor>, or parenthesized
//                  subexpression).  An identifier can be
//                  a function, constant, or variable.
//
//  Return: ptr to factor's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitConstant        Emit code to load a scalar constant into
//                      ax or dx:ax, or to push a string address
//                      onto the runtime stack.
//
//      pId : ptr to constant identifier's symbol table node
//
//  Return: ptr to constant's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitVariable        Emit code to load a variable's value
//                      ax or dx:ax, or push its address onto
//                      the runtime stack.
//
//      pId         : ptr to variable's symbol table node
//      addressFlag : true to push address, false to load value
//
//  Return: ptr to variable's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitSubscripts      Emit code for each subscript expression
//                      to modify the data address at the top of
//                      the runtime stack.
//
//      pType : ptr to array type object
//
//  Return: ptr to subscripted variable's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  EmitField   Emit code for a field designator to modify the
//              data address at the top of the runtime stack.
//
//  Return: ptr to record field's type object
//--------------------------------------------------------------

//endfig
