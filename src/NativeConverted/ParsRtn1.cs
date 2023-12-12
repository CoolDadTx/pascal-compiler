//fig 8-10
//  *************************************************************
//  *                                                           *
//  *   P A R S E R   (Routines #1)                             *
//  *                                                           *
//  *   Parse programs, procedures, and functions.              *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog8-1/parsrtn1.cpp                           *
//  *                                                           *
//  *   MODULE:  Parser                                         *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************



//--------------------------------------------------------------
//  ParseProgram        Parse a program:
//
//                          <program-header> ; <block> .
//
//  Return: ptr to program id's symbol table node
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseProgramHeader      Parse a program header:
//
//                              PROGRAM <id>
//
//                          or:
//
//                              PROGRAM <id> ( <id-list> )
//
//  Return: ptr to program id's symbol table node
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseSubroutineDeclarations     Parse procedures and
//                                  function declarations.
//
//      pRoutineId : ptr to symbol table node of the name of the
//                   routine that contains the subroutines
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseSubroutine     Parse a subroutine:
//
//                          <subroutine-header> ; <block>
//
//                      or:
//
//                          <subroutine-header> ; forward
//
//  Return: ptr to subroutine id's symbol table node
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseProcedureHeader    Parse a procedure header:
//
//                              PROCEDURE <id>
//
//                          or:
//
//                              PROCEDURE <id> ( <parm-list> )
//
//  Return: ptr to procedure id's symbol table node
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseFunctionHeader     Parse a function header:
//
//                              FUNCTION <id>
//
//                          or:
//
//                              FUNCTION <id> : <type-id>
//
//                          or:
//
//                              FUNCTION <id> ( <parm-list> )
//                                            : <type-id>
//
//  Return: ptr to function id's symbol table node
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseBlock      Parse a routine's block:
//
//                      <declarations> <compound-statement>
//
//      pRoutineId : ptr to symbol table node of routine's id
//--------------------------------------------------------------

//endfig
