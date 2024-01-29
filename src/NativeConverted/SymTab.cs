//--------------------------------------------------------------
//  TDefnCode           Definition code: How an identifier
//                                       is defined.
//--------------------------------------------------------------

public enum TDefnCode
{
    DcUndefined,
    DcConstant,
    DcType,
    DcVariable,
    DcField,
    DcValueParm,
    DcVarParm,
    DcProgram,
    DcProcedure,
    DcFunction,
}

//--------------------------------------------------------------
//  TRoutineCode        Routine code: For procedures, functions,
//                                    and standard routines.
//--------------------------------------------------------------

public enum TRoutineCode
{
    RcDeclared,
    RcForward,
    RcRead,
    RcReadln,
    RcWrite,
    RcWriteln,
    RcAbs,
    RcArctan,
    RcChr,
    RcCos,
    RcEof,
    RcEoln,
    RcExp,
    RcLn,
    RcOdd,
    RcOrd,
    RcPred,
    RcRound,
    RcSin,
    RcSqr,
    RcSqrt,
    RcSucc,
    RcTrunc,
}

//--------------------------------------------------------------
//  TLocalIds           Local identifier lists structure.
//--------------------------------------------------------------

public class TLocalIds
{
    public TSymtabNode pParmIds; // ptr to local parm id list
    public TSymtabNode pConstantIds; // ptr to local constant id list
    public TSymtabNode pTypeIds; // ptr to local type id list
    public TSymtabNode pVariableIds; // ptr to local variable id list
    public TSymtabNode pRoutineIds; // ptr to local proc and func id list
}

//--------------------------------------------------------------
//  TDefn               Definition class.
//--------------------------------------------------------------

public class TDefn : System.IDisposable
{
    public TDefnCode how; // the identifier was defined

    //NOTE: Making this regular props and classes to resolve all the compile errors
    //but then we have to either push the init to this type (e.g. lazy) or ensure everyone sets it    
    //private OneOf<ConstantDefn, RoutineDefn, VariableDefn> _value;

    public ConstantDefn constant { get; set; } = new ConstantDefn();
    //{
    //    get => _value.AsT0;
    //    set => _value = OneOf<ConstantDefn, RoutineDefn, VariableDefn>.FromT0(value);
    //}

    public RoutineDefn routine { get; set; } = new RoutineDefn();
    //{
    //    get => _value.AsT1;
    //    set => _value = OneOf<ConstantDefn, RoutineDefn, VariableDefn>.FromT1(value);        
    //}

    public VariableDefn data { get; set; } = new VariableDefn();
    //{
    //    get => _value.AsT2;
    //    set => _value = OneOf<ConstantDefn, RoutineDefn, VariableDefn>.FromT2(value);        
    //}    
    
    public TDefn ( TDefnCode dc )
    {
        how = dc;
    }

    //              ****************
    //              *              *
    //              *  Definition  *
    //              *              *
    //              ****************

    //--------------------------------------------------------------
    //  Destructor      Delete the local symbol table and icode of a
    //                  program, procedure or function definition.
    //                  Note that the parameter and local identifier
    //                  chains are deleted along with the local
    //                  symbol table.
    //--------------------------------------------------------------

    public void Dispose ()
    {
        switch (how)
        {

            case TDefnCode.DcProgram:
            case TDefnCode.DcProcedure:
            case TDefnCode.DcFunction:
            {
                if (routine.which == TRoutineCode.RcDeclared)
                {
                    routine.pSymtab?.Dispose();
                    routine.pIcode?.Dispose();
                };

                break;
            };

            default:
            break;
        }
    }
}

//--------------------------------------------------------------
//  TSymtabNode         Symbol table node class.
//--------------------------------------------------------------

public class TSymtabNode : IDisposable
{
    //Needs to be visible to TSymNode
    internal TSymtabNode left; // ptrs to left and right subtrees
    internal TSymtabNode right;
    internal string pString; // ptr to symbol string
    internal TLineNumList pLineNumList; // ptr to list of line numbers

    private int xSymtab; // symbol table index
    private short xNode; // node index
    

    public TSymtabNode next; // ptr to next sibling in chain
    public TType pType; // ptr to type info

    public TDefn defn = new TDefn(TDefnCode.DcUndefined); // definition info
    public int level; // nesting level
    public int labelIndex; // index for code label


    //              ***********************
    //              *                     *
    //              *  Symbol Table Node  *
    //              *                     *
    //              ***********************

    //--------------------------------------------------------------
    //  Constructor     Construct a symbol table node by initial-
    //                  izing its subtree pointers and the pointer
    //                  to its symbol string.
    //
    //      pStr : ptr to the symbol string
    //      dc   : definition code
    //--------------------------------------------------------------

    public TSymtabNode ( string pStr, TDefnCode dc = default )
    {
        this.defn = new TDefn(dc);
        left = right = next = null;
        pLineNumList = null;
        pType = null;
        xNode = 0;
        level = Globals.currentNestingLevel;
        labelIndex = ++Globals.asmLabelIndex;

        //--Allocate and copy the symbol string.
        pString = new string(new char[pStr.Length]);
        pString = pStr;

        //--If cross-referencing, update the line number list.
        if (Globals.xrefFlag)
            pLineNumList = new TLineNumList();
    }

    public TSymtabNode ( string pStr, TDefnCode dc, int symTabIndex, short nodeIndex ) : this(pStr, dc)
    {
        xSymtab = symTabIndex;
        xNode = nodeIndex;
    }

    //--------------------------------------------------------------
    //  Destructor      Deallocate a symbol table node.
    //--------------------------------------------------------------

    public void Dispose ()
    {
        //--First the subtrees (if any).
        left?.Dispose();        
        right?.Dispose();

        //--Then delete this node's components.        
        pLineNumList?.Dispose();
        TType.RemoveType(ref pType);
    }

    public TSymtabNode LeftSubtree () => left;
    public TSymtabNode RightSubtree () => right;
    public string String () => pString;
    public int SymtabIndex () => xSymtab;
    public short NodeIndex () => xNode;

    //--------------------------------------------------------------
    //  Convert     Convert the symbol table node into a form
    //		suitable for the back end.
    //
    //	vpNodes : vector of node ptrs
    //--------------------------------------------------------------

    public void Convert ( TSymtabNode[] vpNodes )
    {
        //--First, convert the left subtree.
        left?.Convert(vpNodes);

        //--Convert the node.
        vpNodes[xNode] = this;

        //--Finally, convert the right subtree.
        right?.Convert(vpNodes);
    }


    //--------------------------------------------------------------
    //  Print       Print the symbol table node to the list file.
    //              First print the node's left subtree, then the
    //              node itself, and finally the node's right
    //              subtree.  For the node itself, first print its
    //              symbol string, and then its line numbers.
    //--------------------------------------------------------------

    public void Print ()
    {
        const int maxNamePrintWidth = 16;

        //--Pirst, print left subtree        
        left?.Print();

        //--Print the node:  first the name, then the list of line numbers,
        //--                 and then the identifier information.
        //C++ TO C# CONVERTER TODO TASK: The following line has a C format specifier which cannot be directly translated to C#:
        //ORIGINAL LINE: sprintf(list.text, "%*s", maxNamePrintWidth, pString);
        Globals.list.text = System.String.Format("%*s", maxNamePrintWidth, pString);
        if (pLineNumList != null)
            pLineNumList.Print(pString.Length > maxNamePrintWidth, maxNamePrintWidth);
        else
            Globals.list.PutLine();
        PrintIdentifier();

        //--Finally, print right subtree        
        right?.Print();
    }

    //--------------------------------------------------------------
    //  PrintIdentifier         Print information about an
    //                          identifier's definition and type.
    //--------------------------------------------------------------

    public void PrintIdentifier ()
    {
        switch (defn.how)
        {
            case TDefnCode.DcConstant:
            PrintConstant();
            break;
            case TDefnCode.DcType:
            PrintType();
            break;

            case TDefnCode.DcVariable:
            case TDefnCode.DcField:
            PrintVarOrField();
            break;
        }
    }

    //--------------------------------------------------------------
    //  PrintConstant       Print information about a constant
    //                      identifier for the cross-reference.
    //--------------------------------------------------------------

    public void PrintConstant ()
    {
        Globals.list.PutLine();
        Globals.list.PutLine("Defined constant");

        //--Value
        if ((pType == Globals.pIntegerType) || (pType.form == TFormCode.FcEnum))
            Globals.list.text = System.String.Format("Value = {0:D}", defn.constant.value.integer);
        else if (pType == Globals.pRealType)
            Globals.list.text = System.String.Format("Value = {0:g}", defn.constant.value.real);
        else if (pType == Globals.pCharType)
            Globals.list.text = System.String.Format("Value = '{0}'", defn.constant.value.character);
        else if (pType.form == TFormCode.FcArray)
            Globals.list.text = System.String.Format("Value = '{0}'", defn.constant.value.pString);
        Globals.list.PutLine();

        //--Type information        
        pType?.PrintTypeSpec(TType.TVerbosityCode.VcTerse);
        Globals.list.PutLine();
    }

    //--------------------------------------------------------------
    //  PrintVarOrField         Print information about a variable
    //                          or record field identifier for the
    //                          cross-reference.
    //--------------------------------------------------------------

    public void PrintVarOrField ()
    {
        Globals.list.PutLine();
        Globals.list.PutLine(defn.how == TDefnCode.DcVariable ? "Declared variable" : "Declared record field");

        //--Type information
        pType?.PrintTypeSpec(TType.TVerbosityCode.VcTerse);
        if ((defn.how == TDefnCode.DcVariable) || next != null)
            Globals.list.PutLine();
    }

    //--------------------------------------------------------------
    //  PrintType           Print information about a type
    //                      identifier for the cross-reference.
    //--------------------------------------------------------------

    public void PrintType ()
    {
        Globals.list.PutLine();
        Globals.list.PutLine("Defined type");

        pType?.PrintTypeSpec(TType.TVerbosityCode.VcVerbose);
        Globals.list.PutLine();
    }
}

//--------------------------------------------------------------
//  TSymtab             Symbol table class.  The symbol table is
//                      organized as a binary tree that is
//                      sorted alphabetically by the nodes'
//                      name strings.
//--------------------------------------------------------------

public class TSymtab : System.IDisposable
{
    private TSymtabNode root; // ptr to binary tree root
    private TSymtabNode[] vpNodes; // ptr to vector of node ptrs
    private short cntNodes; // node counter
    private int xSymtab; // symbol table index
    private TSymtab next; // ptr to next symbol table in list

    public TSymtab ()
    {
        root = null;
        vpNodes = null;
        cntNodes = 0;
        xSymtab = Globals.cntSymtabs++;        

        //--Insert at the head of the symbol table list.
        next = Globals.pSymtabList;
        Globals.pSymtabList = this;
    }

    public void Dispose ()
    {
        root?.Dispose();
        Arrays.DeleteArray(vpNodes);
    }


    //              ******************
    //              *                *
    //              *  Symbol Table  *
    //              *                *
    //              ******************

    //--------------------------------------------------------------
    //  Search      Search the symbol table for the node with a
    //              given name string.
    //
    //      pString : ptr to the name string to search for
    //
    //  Return: ptr to the node if found, else NULL
    //--------------------------------------------------------------

    public TSymtabNode Search ( string pString )
    {
        TSymtabNode pNode = root; // ptr to symbol table node
        int comp = 0;

        //--Loop to search the table.
        while (pNode != null)
        {
            comp = String.Compare(pString, pNode.pString); // compare names
            if (comp == 0)
                break; // found!

            //--Not yet found:  next search left or right subtree.
            pNode = comp < 0 ? pNode.left : pNode.right;
        }

        //--If found and cross-referencing, update the line number list.
        if (Globals.xrefFlag && (comp == 0))
            pNode.pLineNumList.Update();

        return pNode; // ptr to node, or NULL if not found
    }

    //--------------------------------------------------------------
    //  Enter       Search the symbol table for the node with a
    //              given name string.  If the node is found, return
    //              a pointer to it.  Else if not found, enter a new
    //              node with the name string, and return a pointer
    //              to the new node.
    //
    //      pString : ptr to the name string to enter
    //      dc      : definition code
    //
    //  Return: ptr to the node, whether existing or newly-entered
    //--------------------------------------------------------------

    public TSymtabNode Enter ( string pString, TDefnCode dc = default )
    {
        TSymtabNode pNode = null; // ptr to node
        TSymtabNode ppNode = root; // ptr to ptr to node

        //--Loop to search table for insertion point.
        int comp = 0;
        while (ppNode != null)        
        {
            pNode = ppNode;

            comp = String.Compare(pString, pNode.pString); // compare strings
            if (comp == 0)
                return pNode; // found!

            //--Not yet found:  next search left or right subtree.
            ppNode = comp < 0 ? pNode.left : pNode.right;
        }

        //--Create and insert a new node.
        var newNode = new TSymtabNode(pString, dc, xSymtab, cntNodes++); // create a new node,
        //pNode.xSymtab = xSymtab; // set its symtab and
        //pNode.xNode = cntNodes++; // node indexes,
        if (root == null)
            root = newNode;
        else if (comp < 0)
            pNode.left = newNode;
        else
            pNode.right = newNode; // insert it, and
        return newNode; // return a ptr to it
    }

    //--------------------------------------------------------------
    //  EnterNew    Search the symbol table for the given name
    //              string.  If the name is not already in there,
    //              enter it.  Otherwise, flag the redefined
    //              identifier error.
    //
    //      pString : ptr to name string to enter
    //      dc      : definition code
    //
    //  Return: ptr to symbol table node
    //--------------------------------------------------------------

    public TSymtabNode EnterNew ( string pString, TDefnCode dc = default )
    {
        TSymtabNode pNode = Search(pString);

        if (pNode == null)
            pNode = Enter(pString, dc);
        else
            Globals.Error(TErrorCode.ErrRedefinedIdentifier);

        return pNode;
    }

    public TSymtabNode Root ()
    {
        return root;
    }
    public TSymtabNode Get ( short xNode )
    {
        return vpNodes[xNode];
    }
    public TSymtab Next ()
    {
        return next;
    }
    public TSymtabNode[] NodeVector ()
    {
        return vpNodes;
    }
    public int NodeCount ()
    {
        return cntNodes;
    }
    public void Print ()
    {
        root.Print();
    }

    //--------------------------------------------------------------
    //  Convert     Convert the symbol table into a form suitable
    //		for the back end.
    //
    //	vpSymtabs : vector of symbol table pointers
    //--------------------------------------------------------------

    public void Convert ( TSymtab[] vpSymtabs )
    {
        //--Point the appropriate entry of the symbol table pointer vector
        //--to this symbol table.
        vpSymtabs[xSymtab] = this;

        //--Allocate the symbol table node pointer vector
        //--and convert the nodes.
        vpNodes = new TSymtabNode[cntNodes];
        root.Convert(vpNodes);
    }
}

//--------------------------------------------------------------
//  TSymtabStack      Symbol table stack class.
//--------------------------------------------------------------

public class TSymtabStack : System.IDisposable
{
    private const int MaxNestingLevel = 8;

    private TSymtab[] pSymtabs = Arrays.InitializeWithDefaultInstances<TSymtab>(MaxNestingLevel); // stack of symbol table ptrs


    //              ************************
    //              *		       *
    //              *  Symbol Table Stack  *
    //              *		       *
    //              ************************

    //--------------------------------------------------------------
    //  Constructor	    Initialize the global (level 0) symbol
    //		    table, and set the others to NULL.
    //--------------------------------------------------------------

    public TSymtabStack ()
    {
        Globals.currentNestingLevel = 0;
        for (int i = 1; i < MaxNestingLevel; ++i)
            pSymtabs[i] = null;

        //--Initialize the global nesting level.
        pSymtabs[0] = Globals.globalSymtab;
        Globals.InitializePredefinedTypes(pSymtabs[0]);
        Globals.InitializeStandardRoutines(pSymtabs[0]);
    }

    //--------------------------------------------------------------
    //  Destructor	    Remove the predefined types.
    //--------------------------------------------------------------

    public void Dispose ()
    {
        Globals.RemovePredefinedTypes();
    }

    public TSymtabNode SearchLocal ( string pString )
    {
        return pSymtabs[Globals.currentNestingLevel].Search(pString);
    }

    public TSymtabNode EnterLocal ( string pString, TDefnCode dc = default )
    {
        return pSymtabs[Globals.currentNestingLevel].Enter(pString, dc);
    }

    public TSymtabNode EnterNewLocal ( string pString, TDefnCode dc = default )
    {
        return pSymtabs[Globals.currentNestingLevel].EnterNew(pString, dc);
    }

    public TSymtab GetCurrentSymtab ()
    {
        return pSymtabs[Globals.currentNestingLevel];
    }

    public void SetCurrentSymtab ( TSymtab pSymtab )
    {
        pSymtabs[Globals.currentNestingLevel] = pSymtab;
    }


    //--------------------------------------------------------------
    //  SearchAll   Search the symbol table stack for the given
    //              name string.
    //
    //      pString : ptr to name string to find
    //
    //  Return: ptr to symbol table node if found, else NULL
    //--------------------------------------------------------------

    public TSymtabNode SearchAll ( string pString )
    {
        for (int i = Globals.currentNestingLevel; i >= 0; --i)
        {
            TSymtabNode pNode = pSymtabs[i].Search(pString);
            if (pNode != null)
                return pNode;
        }

        return null;
    }

    //--------------------------------------------------------------
    //  Find        Search the symbol table stack for the given
    //              name string.  If the name is not already in
    //              there, flag the undefined identifier error,
    //		and then enter the name into the local symbol
    //		table.
    //
    //      pString : ptr to name string to find
    //
    //  Return: ptr to symbol table node
    //--------------------------------------------------------------

    public TSymtabNode Find ( string pString )
    {
        TSymtabNode pNode = SearchAll(pString);

        if (pNode == null)
        {
            Globals.Error(TErrorCode.ErrUndefinedIdentifier);
            pNode = pSymtabs[Globals.currentNestingLevel].Enter(pString);
        }

        return pNode;
    }

    //--------------------------------------------------------------
    //  EnterScope	Enter a new nested scope.  Increment the nesting
    //		level.  Push new scope's symbol table onto the
    //		stack.
    //
    //      pSymtab : ptr to scope's symbol table
    //--------------------------------------------------------------

    public void EnterScope ()
    {
        if (++Globals.currentNestingLevel > MaxNestingLevel)
        {
            Globals.Error(TErrorCode.ErrNestingTooDeep);
            Globals.AbortTranslation(TAbortCode.AbortNestingTooDeep);
        }

        SetCurrentSymtab(new TSymtab());
    }

    //--------------------------------------------------------------
    //  ExitScope	Exit the current scope and return to the
    //		enclosing scope.  Decrement the nesting level.
    //		Pop the closed scope's symbol table off the
    //		stack and return a pointer to it.
    //
    //  Return: ptr to closed scope's symbol table
    //--------------------------------------------------------------

    public TSymtab ExitScope ()
    {
        return pSymtabs[Globals.currentNestingLevel--];
    }
}

//--------------------------------------------------------------
//  TLineNumNode        Line number node class.
//--------------------------------------------------------------

public class TLineNumNode
{
    //Needed by TSymbtab
    internal TLineNumNode next; // ptr to next node
    
    public TLineNumNode ()
    {
        number = Globals.currentLineNumber;
        next = null;
    }

    public int number { get; }// the line number
}

//--------------------------------------------------------------
//  TLineNumList        Line number list class.
//--------------------------------------------------------------

public class TLineNumList : IDisposable
{
    private TLineNumNode head; // list head and tail
    private TLineNumNode tail;

    public TLineNumList ()
    {
        head = tail = new TLineNumNode();
    }

    //              **********************
    //              *                    *
    //              *  Line Number List  *
    //              *                    *
    //              **********************

    //--------------------------------------------------------------
    //  Destructor      Deallocate a line number list.
    //--------------------------------------------------------------

    //REMOVE: Does nothing
    public virtual void Dispose ()
    {
        //--Loop to delete each node in the list.
        while (head != null)
        {
            TLineNumNode pNode = head; // ptr to node to delete
            head = head.next; // move down the list            
            //pNode?.Dispose();
        }
    }


    //--------------------------------------------------------------
    //  Update      Update the list by appending a new line number
    //              node if the line number isn't already in the
    //              list.
    //--------------------------------------------------------------

    public void Update ()
    {
        //--If the line number is already there, it'll be at the tail.
        if (tail != null && (tail.number == Globals.currentLineNumber))
            return;

        //--Append the new node.
        tail.next = new TLineNumNode();
        tail = tail.next;
    }

    //--------------------------------------------------------------
    //  Print       Print the line number list.  Use more than one
    //              line if necessary; indent subsequent lines.
    //
    //      newLineFlag : if true, start a new line immediately
    //      indent      : amount to indent subsequent lines
    //--------------------------------------------------------------

    public void Print ( bool newLineFlag, int indent )
    {
        const int maxLineNumberPrintWidth = 4;
        const int maxLineNumbersPerLine = 10;

        int n; // count of numbers per line
        TLineNumNode pNode; // ptr to line number node
            
        var plt = Globals.list.text;
        // ptr to where in list text to append

        n = newLineFlag ? 0 : maxLineNumbersPerLine;

        //--Loop over line number nodes in the list.
        for (pNode = head; pNode != null; pNode = pNode.next)
        {

            //--Start a new list line if the current one is full.
            if (n == 0)
            {
                Globals.list.PutLine();
                //C++ TO C# CONVERTER TODO TASK: The following line has a C format specifier which cannot be directly translated to C#:
                //ORIGINAL LINE: sprintf(list.text, "%*s", indent, " ");
                Globals.list.text = String.Format("%*s", indent, " ");
                plt = Globals.list.text;
                n = maxLineNumbersPerLine;
            }

            //--Append the line number to the list text.
            //C++ TO C# CONVERTER TODO TASK: The following line has a C format specifier which cannot be directly translated to C#:
            //ORIGINAL LINE: sprintf(plt, "%*d", maxLineNumberPrintWidth, pNode->number);
            plt += String.Format("%*d", maxLineNumberPrintWidth, pNode.number);

            //TODO: Is this correct??
            plt += maxLineNumberPrintWidth;
            --n;
        }

        Globals.list.PutLine();
    }
}

//	//--Constant
public class ConstantDefn
{
    public static ConstantDefn FromInteger ( int value ) => new ConstantDefn() { value = TDataValue.FromInteger(value)};

    public static ConstantDefn FromReal ( float value ) => new ConstantDefn() { value = TDataValue.FromReal(value)};

    public static ConstantDefn FromCharacter ( char value ) => new ConstantDefn() { value = TDataValue.FromCharacter(value) };

    public static ConstantDefn FromString ( string value ) => new ConstantDefn() { value = TDataValue.FromString(value) };

    public TDataValue value { get; set; } // value of constant
}

//	//--Procedure, function, or standard routine
public class RoutineDefn
{
    public TRoutineCode which { get; set; } // routine code
    public int parmCount { get; set; } // count of parameters
    public int totalParmSize { get; set; } // total byte size of parms
    public int totalLocalSize { get; set; } // total byte size of locals
    public TLocalIds locals { get; set; } // local identifiers
    public TSymtab pSymtab { get; set; } // ptr to local symtab
    public TIcode pIcode { get; set; } // ptr to routine's icode

    public RoutineDefn SetTotalLocalSize ( int value ) => new RoutineDefn() {
        which = this.which,
        parmCount = this.parmCount,
        totalParmSize = this.totalParmSize,
        totalLocalSize = value,
        locals = this.locals,
        pSymtab = this.pSymtab,
        pIcode = this.pIcode
    };    
}

//	//--Variable, record field, or parameter
public class VariableDefn
{
    public VariableDefn () { }

    public VariableDefn ( int value )
    {
        offset = value;
    }

    public int offset { get; set; } // vars and parms: sequence count
                                    //			 // fields: byte offset in record
}
