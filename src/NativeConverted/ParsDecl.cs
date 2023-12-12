using System;

			//   false for code generator back end

//--------------------------------------------------------------
//  ParseDeclarations   Parse any constant definitions, type
//                      definitions, variable declarations, and
//                      procedure and function declarations, in
//                      that order.
//--------------------------------------------------------------


//              **************************
//              *                        *
//              *  Constant Definitions  *
//              *                        *
//              **************************

//--------------------------------------------------------------
//  ParseConstDefinitions   Parse a list of constant definitions
//                          separated by semicolons:
//
//                              <id> = <constant>
//
//      pRoutineId : ptr to symbol table node of program,
//                   procedure, or function identifier
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseConstant       Parse a constant.
//
//      pConstId : ptr to symbol table node of the identifier
//                 being defined
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseIdentifierConstant     In a constant definition of the
//                              form
//
//                                      <id-1> = <id-2>
//
//                              parse <id-2>. The type can be
//                              integer, real, character,
//                              enumeration, or string
//                              (character array).
//
//      pId1 : ptr to symbol table node of <id-1>
//      sign : unary + or - sign, or none
//--------------------------------------------------------------


//      ********************************************
//      *                                          *
//      *  Variable and Record Field Declarations  *
//      *                                          *
//      ********************************************

//--------------------------------------------------------------
//  ParseVariableDeclarations   Parse variable declarations.
//
//      pRoutineId : ptr to symbol table node of program,
//                   procedure, or function identifier
//--------------------------------------------------------------

//fig 12-10
//endfig

//--------------------------------------------------------------
//  ParseFieldDeclarations      Parse a list record field
//                              declarations.
//
//      pRecordType : ptr to record type object
//      offet       : byte offset within record
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ParseVarOrFieldDecls        Parse a list of variable or
//                              record field declarations
//                              separated by semicolons:
//
//                                  <id-sublist> : <type>
//
//      pRoutineId  : ptr to symbol table node of program,
//                    procedure, or function identifier, or NULL
//      pRecordType : ptr to record type object, or NULL
//      offset      : variable: runtime stack offset
//                    field: byte offset within record
//--------------------------------------------------------------

//fig 12-11
//endfig

//--------------------------------------------------------------
//  ParseIdSublist      Parse a sublist of variable or record
//                      identifiers separated by commas.
//
//      pRoutineId  : ptr to symbol table node of program,
//                    procedure, or function identifier, or NULL
//      pRecordType : ptr to record type object, or NULL
//      pLastId     : ref to ptr that will be set to point to the
//                    last symbol table node of the sublist
//--------------------------------------------------------------


