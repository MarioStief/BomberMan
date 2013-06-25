using UnityEngine;
using System.Collections;

public enum GM_GA_CellBlockType {

    /**
        * CellBlockType categorizes 4x4 Block of Cells 
        * */

    /**
        * Symbolerklärung:
        * # = Solid Stone
        * _ = Random{Free Space, Box}
        * ? = Random{Solid Stone, Random{ Box, Free Space}}
        * x = Random{Solid Stone, Random{ Box, Free Space}}
        *		( at most one solid Stone via x||? per Block) 
        * 
        * Moreover:
        * A solid stone at x often leads to:
        * #_#
        * _#_
        * x_#
        * */

    /* ??_x
        * #_#0
        * ___?
        * #_#?
        * */
    FOURDICE,

    /* ____
        * #_#_
        * _#__
        * #_#_
        * */
    FIVEDICE,

    /* ____
        * #___
        * ____
        * #_#_
        * */
    TRIANGELDOWNLEFT,

    /* ____
        * #_#_
        * ____
        * #___
        * */
    TRIANGLEUPLEFT,

    /* ____
        * #_#_
        * ___?
        * __#_
        * */
    TRIANGLEUPRIGHT,

    /* ___x
        * __#_
        * ___?
        * #_#_
        * */
    TRIANGLEDOWNRIGHT,

    /**
        * 
        * Up to three random placed stones 
        * 
        * */
    ARTIFAKT

}
