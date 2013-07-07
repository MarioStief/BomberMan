using UnityEngine;
using System.Collections;
using System;

public static class Common {

    public static void Assert(bool expr, string desc) 
    {
        if (!expr)
        {
            Debug.LogError("ERROR: Assertion failed: " + desc);
            throw new Exception();
        }
    }

}
