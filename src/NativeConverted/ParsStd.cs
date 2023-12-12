//fig 8-20
//  *************************************************************
//  *                                                           *
//  *   P A R S E R   (Standard Routines)                       *
//  *                                                           *
//  *   Parse calls to standard procedures and functions.       *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog8-1/parsstd.cpp                            *
//  *                                                           *
//  *   MODULE:  Parser                                         *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************



//--------------------------------------------------------------
//  ParseStandardSubroutineCall     Parse a call to a standard
//                                  procedure or function.
//
//      pRoutineId : ptr to the subroutine id's
//                   symbol table node
//
//  Return: ptr to the type object of the call
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseReadReadlnCall     Parse a call to read or readln.
//                          Each actual parameter must be a
//                          scalar variable.
//
//      pRoutineId : ptr to the routine id's symbol table node
//
//  Return: ptr to the dummy type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseWriteWritelnCall   Parse a call to write or writeln.
//                          Each actual parameter can be in any
//                          one of the following forms:
//
//                              <expr>
//                              <expr> : <expr>
//                              <expr> : <expr> : <expr>
//
//      pRoutineId : ptr to the routine id's symbol table node
//
//  Return: ptr to the dummy type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseEofEolnCall    Parse a call to eof or eoln.
//                      No parameters => boolean result
//
//  Return: ptr to the boolean type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseAbsSqrCall     Parse a call to abs or sqr.
//                      Integer parm => integer result
//                      Real parm    => real result
//
//  Return: ptr to the result's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseArctanCosExpLnSinSqrtCall  Parse a call to arctan, cos,
//                                  exp, ln, sin, or sqrt.
//                                  Integer parm => real result
//                                  Real parm    => real result
//
//  Return: ptr to the real type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParsePredSuccCall   Parse a call to pred or succ.
//                      Integer parm => integer result
//                      Enum parm    => enum result
//
//  Return: ptr to the result's type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseChrCall        Parse a call to chr.
//                      Integer parm => character result
//
//  Return: ptr to the character type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseOddCall        Parse a call to odd.
//                      Integer parm => boolean result
//
//  Return: ptr to the boolean type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseOrdCall        Parse a call to ord.
//                      Character parm => integer result
//                      Enum parm      => integer result
//
//  Return: ptr to the integer type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseRoundTruncCall     Parse a call to round or trunc.
//                          Real parm => integer result
//
//  Return: ptr to the integer type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  SkipExtraParms      Skip extra parameters in a call to a
//                      standard procedure or function.
//
//      pSymtab : ptr to symbol table
//--------------------------------------------------------------


//--------------------------------------------------------------
//  InitializeStandardRoutines  Initialize the standard
//                              routines by entering their
//                              identifiers into the symbol
//                              table.
//
//      pSymtab : ptr to symbol table
//--------------------------------------------------------------

public class TStdRtn
{
	public string pName;
	public TRoutineCode rc;
	public TDefnCode dc;
}