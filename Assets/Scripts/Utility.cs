using UnityEngine;
using System.Collections;

public class Utility {

    //http://answers.unity3d.com/questions/395513/vector3-comparison-efficiency-and-float-precision.html
    public static bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.001;
    }

}
