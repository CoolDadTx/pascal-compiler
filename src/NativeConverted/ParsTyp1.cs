using System;

//fig 7-13
//  *************************************************************
//  *                                                           *
//  *   P A R S E R   (Types #1)                                *
//  *                                                           *
//  *   Parse type definitions, and identifier, enumeration,    *
//  *   and subrange type specifications.                       *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog7-1/parstyp1.cpp                           *
//  *                                                           *
//  *   MODULE:  Parser                                         *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************



//--------------------------------------------------------------
//  ParseTypeDefinitions    Parse a list of type definitions
//                          separated by semicolons:
//
//                              <id> = <type>
//
//      pRoutineId : ptr to symbol table node of program,
//                   procedure, or function identifier
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseTypeSpec       Parse a type specification.
//
//  Return: ptr to type object
//--------------------------------------------------------------


//              *********************
//              *                   *
//              *  Identifier Type  *
//              *                   *
//              *********************

//--------------------------------------------------------------
//  ParseIdentifierType     In a type defintion of the form
//
//                               <id-1> = <id-2>
//
//                          parse <id-2>.
//
//      pId2 : ptr to symbol table node of <id-2>
//
//  Return: ptr to type object of <id-2>
//--------------------------------------------------------------


//              **********************
//              *                    *
//              *  Enumeration Type  *
//              *                    *
//              **********************

//--------------------------------------------------------------
//  ParseEnumerationType    Parse a enumeration type
//                          specification:
//
//                               ( <id-1>, <id-2>, ..., <id-n> )
//
//  Return: ptr to type object
//--------------------------------------------------------------


//              *******************
//              *                 *
//              *  Subrange Type  *
//              *                 *
//              *******************

//--------------------------------------------------------------
//  ParseSubrangeType       Parse a subrange type specification:
//
//                               <min-const> .. <max-const>
//
//      pMinId : ptr to symbol table node of <min-const> if it
//               is an identifier, else NULL
//
//  Return: ptr to type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseSubrangeLimit      Parse a mininum or maximum limit
//                          constant of a subrange type.
//
//      pLimitId : ptr to symbol table node of limit constant if
//                 it is an identifier (already set for the
//                 minimum limit), else NULL
//      limit    : ref to limit value that will be set
//
//  Return: ptr to type object of limit constant
//--------------------------------------------------------------

//endfig

