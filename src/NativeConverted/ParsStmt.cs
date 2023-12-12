using System;

//  *************************************************************
//  *                                                           *
//  *   P A R S E R   (Statements)                              *
//  *                                                           *
//  *   Parse statements.                                       *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog10-1/parsstmt.cpp                          *
//  *                                                           *
//  *   MODULE:  Parser                                         *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************



//--------------------------------------------------------------
//  ParseStatement          Parse a statement.
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseStatementList      Parse a statement list until the
//                          terminator token.
//
//      terminator : the token that terminates the list
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseAssignment         Parse an assignment statement:
//
//                              <id> := <expr>
//
//      pTargetId : ptr to target id's symbol table node
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseREPEAT     Parse a REPEAT statement:
//
//                      REPEAT <stmt-list> UNTIL <expr>
//--------------------------------------------------------------


//fig 10-8
//--------------------------------------------------------------
//  ParseWHILE      Parse a WHILE statement.:
//
//                      WHILE <expr> DO <stmt>
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseIF         Parse an IF statement:
//
//                      IF <expr> THEN <stmt-1>
//
//                  or:
//
//                      IF <expr> THEN <stmt-1> ELSE <stmt-2>
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseFOR        Parse a FOR statement:
//
//                      FOR <id> := <expr-1> TO|DOWNTO <expr-2>
//                          DO <stmt>
//--------------------------------------------------------------

//endfig

//fig 10-11
//--------------------------------------------------------------
//  ParseCASE       Parse a CASE statement:
//
//                      CASE <expr> OF
//                          <case-branch> ;
//                          ...
//                      END
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseCaseBranch     Parse a CASE branch:
//
//                          <case-label-list> : <stmt>
//
//      pExprType     : ptr to the CASE expression's type object
//      pCaseItemList : ref to ptr to list of CASE items
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseCaseLabel      Parse a case label.
//
//      pExprType     : ptr to the CASE expression's type object
//      pCaseItemList : ref to ptr to list of case items
//--------------------------------------------------------------

//endfig

//--------------------------------------------------------------
//  ParseCompound       Parse a compound statement:
//
//                          BEGIN <stmt-list> END
//--------------------------------------------------------------


