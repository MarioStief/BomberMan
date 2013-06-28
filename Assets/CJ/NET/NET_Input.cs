using UnityEngine;
using System.Collections;

public class NET_Input : MonoBehaviour {

    public class Message
    {
        public int reqId; // request id
        public int input; // payload
    }

    public const int IN_LEFT = 0;
    public const int IN_RIGHT = 1;
    public const int IN_UP = 2;
    public const int IN_DOWN = 3;
    public const int IN_PLANT = 4;

    public static int CondFlag(int bit, bool cond)
    {
        return (cond ? 1 : 0) << bit;
    }

    public static int EncodeLocalInput()
    {
        KeyCode[] keys =
            {
                // must be consistent with Globals.IN_*
                KeyCode.LeftArrow,
                KeyCode.RightArrow,
                KeyCode.UpArrow,
                KeyCode.DownArrow,
                KeyCode.Space
            };

        int input = 0;
        for (int i = 0; i < keys.Length; ++i)
            input |= CondFlag(i, Input.GetKey(keys[i]));

        // hacked-in gamepad controls
        float thres = 0.4f;
        input |= CondFlag(0, -thres > Input.GetAxis("Horizontal"));
        input |= CondFlag(1, thres < Input.GetAxis("Horizontal"));
        input |= CondFlag(2, thres < Input.GetAxis("Vertical"));
        input |= CondFlag(3, -thres > Input.GetAxis("Vertical"));
        // testing for Fire1 in *_LocalActor.cs
		
		// hacked-in iOS controls
		thres = 0.1f;
        input |= CondFlag(0, -thres > Input.acceleration.x);
        input |= CondFlag(1, thres < Input.acceleration.x);
        input |= CondFlag(2, thres < Input.acceleration.y);
        input |= CondFlag(3, -thres > Input.acceleration.y);

        return input;
    }

    public static Vector3 DecodeTranslation(int input) // unit length
    {
        Vector3[] dirs =
        {
            // must be consistent with Globals.IN_*
            new Vector3(-1.0f,  0.0f,  0.0f),
            new Vector3( 1.0f,  0.0f,  0.0f),
            new Vector3( 0.0f,  0.0f,  1.0f),
            new Vector3( 0.0f,  0.0f, -1.0f)
        };

        Vector3 trans = new Vector3();
        for(int i = 0; i < dirs.Length; ++i)
            if(0 < (input & 1 << i)) trans += dirs[i];
        trans.Normalize();

        return trans;
    }
}
