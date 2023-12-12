using System;

//fig 3-18
//  *************************************************************
//  *                                                           *
//  *   T O K E N S   (Numbers)                                 *
//  *                                                           *
//  *   Extract number tokens from the source file.             *
//  *                                                           *
//  *   CLASSES: TNumberToken,                                  *
//  *                                                           *
//  *   FILE:    prog3-2/tknnum.cpp                             *
//  *                                                           *
//  *   MODULE:  Scanner                                        *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************



//              *******************
//              *                 *
//              *  Number Tokens  *
//              *                 *
//              *******************

//--------------------------------------------------------------
//  Get         Extract a number token from the source and set
//              its value.
//
//      pBuffer : ptr to text input buffer
//--------------------------------------------------------------


//--------------------------------------------------------------
//  AccumulateValue     Extract a number part from the source
//                      and set its value.
//
//      pBuffer : ptr to text input buffer
//      value   : accumulated value (from one or more calls)
//      ec      : error code if failure
//
//  Return: true  if success
//          false if failure
//--------------------------------------------------------------


//--------------------------------------------------------------
//  Print       Print the token to the list file.
//--------------------------------------------------------------

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: void TNumberToken::Print() const
//endfig
