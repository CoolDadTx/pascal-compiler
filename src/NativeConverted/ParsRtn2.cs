//  *************************************************************
//  *                                                           *
//  *   P A R S E R   (Routines #2)                             *
//  *                                                           *
//  *   Parse formal parameters, procedure and function calls,  *
//  *   and actual parameters.                                  *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog13-1/parsrtn2.cpp                          *
//  *                                                           *
//  *   MODULE:  Parser                                         *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************



//--------------------------------------------------------------
//  ParseFormalParmList     Parse a formal parameter list:
//
//                              ( VAR <id-list> : <type-id> ;
//                                <id-list> : <type-id> ;
//                                ... )
//
//      count     : ref to count of parameters
//      totalSize : ref to total byte size of all parameters
//
//  Return: ptr to head of parm id symbol table node list
//--------------------------------------------------------------

//fig 12-12
//endfig

//--------------------------------------------------------------
//  ParseSubroutineCall     Parse a call to a declared or a
//                          standard procedure or function.
//
//      pRoutineId    : ptr to routine id's symbol table node
//      parmCheckFlag : true to check parameter, false not to
//
//  Return: ptr to the subroutine's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseDeclaredSubroutineCall Parse a call to a declared
//                              procedure or function.
//
//      pRoutineId    : ptr to subroutine id's symbol table node
//      parmCheckFlag : true to check parameter, false not to
//
//  Return: ptr to the subroutine's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseActualParmList     Parse an actual parameter list:
//
//                              ( <expr-list> )
//
//      pRoutineId    : ptr to routine id's symbol table node
//      parmCheckFlag : true to check parameter, false not to
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseActualParm     Parse an actual parameter.  Make sure it
//                      matches the corresponding formal parm.
//
//      pFormalId     : ptr to the corresponding formal parm
//                      id's symbol table node
//      parmCheckFlag : true to check parameter, false not to
//--------------------------------------------------------------


//fig 12-13
//--------------------------------------------------------------
//  ReverseNodeList 	Reverse a list of symbol table nodes.
//
//	head : ref to the ptr to the current head of the list
//--------------------------------------------------------------

//endfig


