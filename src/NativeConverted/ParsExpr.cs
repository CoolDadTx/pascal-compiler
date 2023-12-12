//  *************************************************************
//  *                                                           *
//  *   P A R S E R   (Expressions)                             *
//  *                                                           *
//  *   Parse expressions.                                      *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog8-1/parsexpr.cpp                           *
//  *                                                           *
//  *   MODULE:  Parser                                         *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************



//--------------------------------------------------------------
//  ParseExpression     Parse an expression (binary relational
//                      operators = < > <> <= and >= ).
//
//  Return: ptr to the expression's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseSimpleExpression       Parse a simple expression
//                              (unary operators + or - , and
//                              binary operators + - and OR).
//
//  Return: ptr to the simple expression's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseTerm           Parse a term (binary operators * / DIV
//                      MOD and AND).
//
//  Return: ptr to the term's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseFactor         Parse a factor (identifier, number,
//                      string, NOT <factor>, or parenthesized
//                      subexpression).
//
//  Return: ptr to the factor's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseVariable       Parse a variable, which can be a simple
//                      identifier, an array identifier followed
//                      subscripts, or a record identifier
//                      followed by  fields.
//
//      pId : ptr to the identifier's symbol table node
//
//  Return: ptr to the variable's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseSubscripts     Parse a bracketed list of subscript
//                      separated by commas, following an
//                      array variable:
//
//                          [ <expr>, <expr>, ... ]
//
//      pType : ptr to the array's type object
//
//  Return: ptr to the array element's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseField          Parse a field following a record
//                      variable:
//
//                          . <id>
//
//      pType : ptr to the record's type object
//
//  Return: ptr to the field's type object
//--------------------------------------------------------------


