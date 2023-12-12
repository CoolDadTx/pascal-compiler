//fig 7-14
//  *************************************************************
//  *                                                           *
//  *   P A R S E R   (Types #2)                                *
//  *                                                           *
//  *   Parse array and record type specifications.             *
//  *                                                           *
//  *   CLASSES: TParser                                        *
//  *                                                           *
//  *   FILE:    prog7-1/parstyp2.cpp                           *
//  *                                                           *
//  *   MODULE:  Parser                                         *
//  *                                                           *
//  *   Copyright (c) 1996 by Ronald Mak                        *
//  *   For instructional purposes only.  No warranties.        *
//  *                                                           *
//  *************************************************************



//              ****************
//              *              *
//              *  Array Type  *
//              *              *
//              ****************

//--------------------------------------------------------------
//  ParseArrayType      Parse an array type specification:
//
//                          ARRAY [ <index-type-list> ]
//                              OF <elmt-type>
//
//  Return: ptr to type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseIndexType      Parse an array index type.
//
//      pArrayType : ptr to array type object
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ArraySize           Calculate the total byte size of an
//                      array type by recursively calculating
//                      the size of each dimension.
//
//      pArrayType : ptr to array type object
//
//  Return: byte size
//--------------------------------------------------------------


//              *****************
//              *               *
//              *  Record Type  *
//              *               *
//              *****************

//--------------------------------------------------------------
//  ParseRecordType     Parse a record type specification:
//
//                          RECORD
//                              <id-list> : <type> ;
//                              ...
//                          END
//
//  Return: ptr to type object
//--------------------------------------------------------------

//endfig
