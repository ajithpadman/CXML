namespace ClangSharp {
    public class Type {

        internal Interop.Type Native { get; private set; }

        internal Type(Interop.Type native) {
            Native = native;
        }

        public Type ArrayElementType
        {
            get { return new Type(Interop.clang_getArrayElementType(Native)); }
        }

        public Type Canonical {
            get { return new Type(Interop.clang_getCanonicalType(Native)); }
        }

        public Type Pointee {
            get { return new Type(Interop.clang_getPointeeType(Native)); }
        }

        public Type Result {
            get { return new Type(Interop.clang_getResultType(Native)); }
        }

        public Cursor Declaration {
            get { return new Cursor(Interop.clang_getTypeDeclaration(Native)); }
        }

        public Kind TypeKind {
            get { return Native.kind; }
        }

        public string TypeKindSpelling {
            get {
                return Interop.clang_getTypeKindSpelling(TypeKind).ManagedString;
            }
        }

        public bool IsConstQualifiedType
        {
            get { return Interop.clang_isConstQualifiedType(Native) != 0; }
        }

        public bool IsRestrictQualifiedType
        {
            get { return Interop.clang_isRestrictQualifiedType(Native) != 0; }
        }

        public bool IsVolatileQualifiedType
        {
            get { return Interop.clang_isVolatileQualifiedType(Native) != 0; }
        }

        public bool IsFunctionTypeVariadic
        {
            get { return Interop.clang_isFunctionTypeVariadic(Native) != 0; }
        }
        public long NumOfElements
        {
            get { return Interop.clang_getNumElements(Native); }
        }
        public int NumArgTypes
        {
            get { return Interop.clang_getNumArgTypes(Native); }
        }

        public Type GetArgType(uint i)
        {
            return new Type(Interop.clang_getArgType(Native, i));
        }

        public bool Equals(Type other) {
            return Interop.clang_equalTypes(Native, other.Native) != 0;
        }

        public override bool Equals(object obj) {
            return obj is Type && Equals((Type)obj);
        }

        public override int GetHashCode() {
            return Native.GetHashCode();
        }

        public enum Kind {
            Invalid,
            Unexposed,
            Void,
            Bool,
            CharU,
            UChar,
            Char16,
            Char32,
            UShort,
            UInt,
            ULong,
            ULongLong,
            UInt128,
            CharS,
            SChar,
            WChar,
            Short,
            Int,
            Long,
            LongLong,
            Int128,
            Float,
            Double,
            LongDouble,
            NullPtr,
            Overload,
            Dependent,
            ObjCId,
            ObjCClass,
            ObjCSel,
            FirstBuiltin = 2,
            LastBuiltin = 29,
            Complex = 100,
            Pointer,
            BlockPointer,
            LValueReference,
            RValueReference,
            Record,
            Enum,
            Typedef,
            ObjCInterface,
            ObjCObjectPointer,
            FunctionNoProto,
            FunctionProto,
            ConstantArray,
            Vector,
            IncompleteArray,
            VariableArray,
            DependentSizedArray,
            MemberPointer,
            Elaborated = 0x00000077
        }
        private string _spelling;
        public string Spelling {

            get { return _spelling ?? (_spelling = Interop.clang_getTypeSpelling(Native).ManagedString); }
            set { _spelling = value; }
        
        }
    }
}