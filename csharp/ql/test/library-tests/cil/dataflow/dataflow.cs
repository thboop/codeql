// semmle-extractor-options: --cil

using System;

class Test
{
    static void Main(string[] args)
    {

        // Indirect call to method
        var c1 = "abc".Contains("a");          // Calls string.IndexOf()
        var c2 = "123".CompareTo("b");         // Calls string.Compare()
        var c3 = Tuple.Create("c", "d", "e");    // Calls Tuple constructor

        // Dataflow through call
        var f1 = "tainted".ToString();
        var f2 = Math.Abs(12);
        var f3 = Math.Max(2, 3);
        var f4 = Math.Round(0.5);
        var f5 = System.IO.Path.GetFullPath("tainted");

        // Tainted dataflow (there is no untainted dataflow path)
        int remainder;
        var t1 = System.Math.DivRem(1, 2, out remainder);

        // Tainted indirect call to method (there is no untainted dataflow path)
        var t2 = System.Math.IEEERemainder(1.0, 2.0);

        // Miscellaneous examples
        var m1 = System.Math.DivRem(Math.Abs(-1), Math.Max(1, 2), out remainder);
        var m2 = "tainted".ToString().Contains("t");
    }

    void DeadCode() { }

    System.Reflection.Assembly assembly;

    void CilAlwaysThrows()
    {
        assembly.GetModules(true);  // Throws NotImplementedException
        DeadCode();
    }

    void Throw() => throw new InvalidCastException();

    void CsAlwaysThrows()
    {
        Throw();
        DeadCode();
    }

    void Nullness()
    {
        var @null = default(object);
        var nonNull = default(int);
        var nullFromCil = this.GetType().DeclaringMethod;
        var nonNullFromCil = true.ToString();
        var null2 = NullFunction() ?? IndirectNull();

        // The following are not always null:
        var notNull = cond ? NullFunction() : this;
    }

    object IndirectNull() => null;

    bool cond;

    object NullFunction()
    {
        object x = IndirectNull();
        if (cond) x = null;
        return x;
    }
}
