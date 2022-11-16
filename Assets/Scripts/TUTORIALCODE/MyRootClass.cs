using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public struct MyStruct
{
    //public MyStruct derp;
    public int[] blah;

    public MyStruct(ref int[] source)
    {
        blah = new int[source.Length];
        for (int i = 0; i < source.Length; i++)
            blah[i] = source[i];
    }
}

public class MyRootClass
{
    public int lvl;
    public string name;
    public char[] _name;
    public MyRootClass derp;
    public StringBuilder builder;

    void TestFunc()
    {
        int x = 5;
        int[] y = new int[5];
        string a = "apple";
        string b = a;
        a += 's';

        builder.Clear();
        builder.Append("hello world!");
    }

    public MyRootClass()
    {

    }

    ~MyRootClass()
    {

    }
}

public class MyTestingClass
{
    public MyRootClass MyRoot;

    public void MakeRoot()
    {
        MyRoot = new MyRootClass();
    }

    public void DestroyRoot()
    {
        
    }
}
