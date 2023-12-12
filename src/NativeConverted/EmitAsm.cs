//              *********************
//              *                   *
//              *  Assembly Buffer  *
//              *                   *
//              *********************

//--------------------------------------------------------------
//  Constructor     Construct an assembly buffer by opening the
//                  output assembly file.
//
//      pAssemblyFileName : ptr to the name of the assembly file
//      ac                : abort code to use if open failed
//--------------------------------------------------------------


//--------------------------------------------------------------
//  Advance         Advance pText to the end of the buffer
//                  contents.
//--------------------------------------------------------------


//              ***************************************
//              *                                     *
//              *  Emit parts of assembly statements  *
//              *                                     *
//              ***************************************

//--------------------------------------------------------------
//  Reg                 Emit a register name.  Example:  ax
//
//      r : register code
//--------------------------------------------------------------


//--------------------------------------------------------------
//  Operator            Emit an opcode.  Example:  add
//
//      opcode : operator code
//--------------------------------------------------------------


//--------------------------------------------------------------
//  Label               Emit a generic label constructed from
//                      the prefix and the label index.
//                                                              
//                      Example:        $L_007
//
//      pPrefix : ptr to label prefix
//      index   : index value
//--------------------------------------------------------------


//--------------------------------------------------------------
//  WordLabel           Emit a word label constructed from
//                      the prefix and the label index.
//                                                              
//                      Example:        WORD PTR $F_007         
//
//      pPrefix : ptr to label prefix
//      index   : index value
//--------------------------------------------------------------


//--------------------------------------------------------------
//  HighDWordLabel      Emit a word label constructed from
//                      the prefix and the label index and
//                      offset by 2 to point to the high Word
//                      of a double Word.
//
//                      Example:        WORD PTR $F_007+2
//
//      pPrefix : ptr to label prefix
//      index   : index value
//--------------------------------------------------------------


//--------------------------------------------------------------
//  Byte                Emit a byte label constructed from
//                      the id name and its label index.
//
//                      Example:        BYTE_PTR ch_007
//
//      pId : ptr to symbol table node
//--------------------------------------------------------------


//--------------------------------------------------------------
//  Word                Emit a word label constructed from
//                      the id name and its label index.
//
//                      Example:        WORD_PTR sum_007
//
//      pId : ptr to symbol table node
//--------------------------------------------------------------


//--------------------------------------------------------------
//  HighDWord           Emit a word label constructed from     
//                      the id name and its label index and
//                      offset by 2 to point to the high word
//                      of a double Word.                       
//                                                              
//                      Example:        WORD_PTR sum_007+2      
//
//      pId : ptr to symbol table node
//--------------------------------------------------------------


//--------------------------------------------------------------
//  ByteIndirect        Emit an indirect reference to a byte
//                      via a register.
//
//                      Example:        BYTE PTR [bx]
//
//      r : register code
//--------------------------------------------------------------


//--------------------------------------------------------------
//  WordIndirect        Emit an indirect reference to a word
//                      via a register.
//                                                              
//                      Example:        WORD PTR [bx]           
//
//      r : register code
//--------------------------------------------------------------


//--------------------------------------------------------------
//  HighDWordIndirect   Emit an indirect reference to the high
//                      word of a double word via a register.
//                                                              
//                      Example:        WORD PTR [bx+2]
//
//      r : register code
//--------------------------------------------------------------


//--------------------------------------------------------------
//  TaggedName          Emit an id name tagged with the id's
//                      label index.
//                                                              
//                      Example:        x_007                   
//
//      pId : ptr to symbol table node
//--------------------------------------------------------------


//--------------------------------------------------------------
//  NameLit             Emit a literal name.
//                                                              
//                      Example:        _FloatConvert
//
//      pName : ptr to name
//--------------------------------------------------------------


//--------------------------------------------------------------
//  IntegerLit          Emit an integer as a string.
//
//      n : integer value
//--------------------------------------------------------------


//--------------------------------------------------------------
//  CharLit             Emit a character surrounded by single
//                      quotes.
//
//      ch : character value
//--------------------------------------------------------------

//endfig
