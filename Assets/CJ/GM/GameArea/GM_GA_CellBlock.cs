using UnityEngine;
using System.Collections;

public class GM_GA_CellBlock : MonoBehaviour {

    int xpos, zpos;

    int[][] block;

    GM_GA_CellBlockType type;


    public GM_GA_CellBlock(int xpos, int zpos, GM_GA_CellBlockType type)
    {

        this.xpos = xpos;
        this.zpos = zpos;

        this.type = type;

        block = new int[4][];

        int random_helper;
        int random_helper2;
        switch (this.type)
        {
            /* ??_x
                * #_#0
                * ___?
                * #_#?
                * */
            case GM_GA_CellBlockType.FOURDICE:

                block[0] = new int[] { 0, 0, 0, 0 };
                block[1] = new int[] { 2, 0, 2, 0 };
                block[2] = new int[] { 0, 0, 0, 0 };
                block[3] = new int[] { 2, 0, 2, 0 };

                // calc random position
                // 0-1 -> pos 1 (?)
                // 2-3 -> pos 2 (?)
                // 4-5 -> pos 3 (?)
                // 6-7 -> pos 4 (?)
                // 8-14 -> none
                // 15 -> pos 5 ( x)
                random_helper = (int)(UnityEngine.Random.value * 15);

                //Debug.Log("Pos: " + random_helper);

                switch (random_helper)
                {
                    case 0:
                    case 1:

                        block[0][0] = 2;	// ?		

                        break;
                    case 2:
                    case 3:

                        block[0][1] = 2;	// ?

                        break;
                    case 4:
                    case 5:

                        block[2][3] = 2;	// ?

                        break;
                    case 6:
                    case 7:

                        block[3][3] = 2;	// ?

                        break;
                    case 15:

                        block[0][3] = 2;	// x

                        break;
                    default:
                        // nothing
                        break;
                }

                break;

            /* ____
                * #_#_
                * _#__
                * #_#_
                * */
            case GM_GA_CellBlockType.FIVEDICE:

                block[0] = new int[] { 0, 0, 0, 0 };
                block[1] = new int[] { 2, 0, 2, 0 };
                block[2] = new int[] { 0, 2, 0, 0 };
                block[3] = new int[] { 2, 0, 2, 0 };

                break;
            /* ____
                * #___
                * ____
                * #_#_
                * */
            case GM_GA_CellBlockType.TRIANGELDOWNLEFT:

                block[0] = new int[] { 0, 0, 0, 0 };
                block[1] = new int[] { 2, 0, 0, 0 };
                block[2] = new int[] { 0, 0, 0, 0 };
                block[3] = new int[] { 2, 0, 2, 0 };

                break;
            /* ____
                * #_#_
                * ____
                * #___
                * */
            case GM_GA_CellBlockType.TRIANGLEUPLEFT:

                block[0] = new int[] { 0, 0, 0, 0 };
                block[1] = new int[] { 2, 0, 2, 0 };
                block[2] = new int[] { 0, 0, 0, 0 };
                block[3] = new int[] { 2, 0, 0, 0 };

                break;
            /* ____
                * #_#_
                * ___?
                * __#_
                * */
            case GM_GA_CellBlockType.TRIANGLEUPRIGHT:

                block[0] = new int[] { 0, 0, 0, 0 };
                block[1] = new int[] { 2, 0, 2, 0 };
                block[2] = new int[] { 0, 0, 0, 0 };
                block[3] = new int[] { 0, 0, 2, 0 };

                break;
            /* ___x
                * __#_
                * ___?
                * #_#_
                * */
            case GM_GA_CellBlockType.TRIANGLEDOWNRIGHT:

                block[0] = new int[] { 0, 0, 0, 0 };
                block[1] = new int[] { 0, 0, 2, 0 };
                block[2] = new int[] { 0, 0, 0, 0 };
                block[3] = new int[] { 2, 0, 2, 0 };

                break;
            /**
                * 
                * Up to three random placed stones 
                * 
                * */
            case GM_GA_CellBlockType.ARTIFAKT:

                block[0] = new int[] { 0, 0, 0, 0 };
                block[1] = new int[] { 0, 0, 0, 0 };
                block[2] = new int[] { 0, 0, 0, 0 };
                block[3] = new int[] { 0, 0, 0, 0 };


                // calc amount of solid boxes:
                random_helper = (int)(UnityEngine.Random.value * 2);
                //Debug.Log("Arti: " + random_helper);
                // calc position of each stone;
                int i = 0;
                while (i - 1 <= random_helper)
                {

                    random_helper2 = (int)(UnityEngine.Random.value * 15);

                    // position already taken
                    if (block[random_helper2 / 4][random_helper2 % 4] == 2)
                    {
                        continue;
                    }

                    block[random_helper2 / 4][random_helper2 % 4] = 2;
                    i++;

                }

                break;
        }

    }

    public GM_GA_Cell setSpawnPoint(GM_GameArea area)
    {

        GM_GA_Cell spawnPoint = null;
        int rand;
        switch (type)
        {
            case GM_GA_CellBlockType.FOURDICE:
                /* ??_x
                    * #3#0
                    * 333?
                    * #3#?
                    * */
                rand = (int)(UnityEngine.Random.value * 4);
                switch (rand)
                {
                    case 0:
                        spawnPoint = area.getCell(xpos + 1, zpos + 1);
                        block[1][1] = 3;
                        block[0][2] = 3;
                        block[1][2] = 3;
                        break;
                    case 1:
                        spawnPoint = area.getCell(xpos, zpos + 2);
                        block[1][1] = 3;
                        block[0][2] = 3;
                        block[1][2] = 3;
                        break;
                    case 2:
                        spawnPoint = area.getCell(xpos + 1, zpos + 2);
                        block[1][1] = 3;
                        block[0][2] = 3;
                        block[1][2] = 3;
                        break;

                    case 3:
                        spawnPoint = area.getCell(xpos + 2, zpos + 2);
                        block[1][2] = 3;
                        block[2][2] = 3;
                        block[1][3] = 3;
                        break;

                    case 4:
                        spawnPoint = area.getCell(xpos + 1, zpos + 3);
                        block[1][2] = 3;
                        block[2][2] = 3;
                        block[1][3] = 3;
                        break;
                }

                break;
            case GM_GA_CellBlockType.FIVEDICE:
                /* __33
                    * #_#3
                    * _#__
                    * #_#_
                    * */

                rand = (int)(UnityEngine.Random.value * 2);
                switch (rand)
                {
                    case 0:
                        spawnPoint = area.getCell(xpos + 2, zpos);
                        break;
                    case 1:
                        spawnPoint = area.getCell(xpos + 3, zpos);
                        break;
                    case 2:
                        spawnPoint = area.getCell(xpos + 3, zpos + 1);
                        break;
                }
                block[0][2] = 3;
                block[0][3] = 3;
                block[1][3] = 3;
                break;
            case GM_GA_CellBlockType.TRIANGELDOWNLEFT:
                /* ____
                    * #33_
                    * 33__
                    * #_#_
                    * */
                rand = (int)(UnityEngine.Random.value * 3);

                switch (rand)
                {
                    case 0:
                        spawnPoint = area.getCell(xpos + 1, zpos + 1);
                        block[1][1] = 3;
                        block[2][1] = 3;
                        block[1][2] = 3;
                        break;
                    case 1:
                        spawnPoint = area.getCell(xpos + 2, zpos + 1);
                        block[1][1] = 3;
                        block[2][1] = 3;
                        block[1][2] = 3;
                        break;
                    case 2:
                        spawnPoint = area.getCell(xpos + 1, zpos + 2);
                        block[0][2] = 3;
                        block[1][2] = 3;
                        block[1][1] = 3;
                        break;

                    case 3:
                        spawnPoint = area.getCell(xpos, zpos + 2);
                        block[0][2] = 3;
                        block[1][2] = 3;
                        block[1][1] = 3;
                        break;
                }
                break;
            case GM_GA_CellBlockType.TRIANGLEUPLEFT:
                /* ____
                    * #3#_
                    * _33_
                    * #_3_
                    * */

                rand = (int)(UnityEngine.Random.value * 3);

                switch (rand)
                {
                    case 0:
                        spawnPoint = area.getCell(xpos + 1, zpos + 1);
                        block[1][1] = 3;
                        block[1][2] = 3;
                        block[2][2] = 3;
                        break;
                    case 1:
                        spawnPoint = area.getCell(xpos + 1, zpos + 2);
                        block[1][1] = 3;
                        block[1][2] = 3;
                        block[2][2] = 3;
                        break;
                    case 2:
                        spawnPoint = area.getCell(xpos + 2, zpos + 2);
                        block[1][1] = 3;
                        block[1][2] = 3;
                        block[2][2] = 3;
                        break;
                    case 3:
                        spawnPoint = area.getCell(xpos + 2, zpos + 3);
                        block[2][3] = 3;
                        block[1][2] = 3;
                        block[2][2] = 3;
                        break;
                }

                break;
            case GM_GA_CellBlockType.TRIANGLEUPRIGHT:
                /* ____
                    * #3#_
                    * 33_?
                    * __#_
                    * */

                rand = (int)(UnityEngine.Random.value * 2);

                switch (rand)
                {
                    case 0:
                        spawnPoint = area.getCell(xpos + 1, zpos + 1);
                        block[0][2] = 3;
                        block[1][2] = 3;
                        block[1][1] = 3;
                        break;
                    case 1:
                        spawnPoint = area.getCell(xpos + 1, zpos + 2);
                        block[0][2] = 3;
                        block[1][2] = 3;
                        block[1][1] = 3;
                        break;
                    case 2:
                        spawnPoint = area.getCell(xpos, zpos + 2);
                        block[0][2] = 3;
                        block[1][2] = 3;
                        block[1][1] = 3;
                        break;
                }
                break;
            case GM_GA_CellBlockType.TRIANGLEDOWNRIGHT:
                /* ___x
                    * _3#_
                    * 330?
                    * #_#_
                    * */
                rand = (int)(UnityEngine.Random.value * 2);

                switch (rand)
                {
                    case 0:
                        spawnPoint = area.getCell(xpos + 1, zpos + 1);
                        block[0][2] = 3;
                        block[1][2] = 3;
                        block[1][1] = 3;
                        break;
                    case 1:
                        spawnPoint = area.getCell(xpos + 1, zpos + 2);
                        block[0][2] = 3;
                        block[1][2] = 3;
                        block[1][1] = 3;
                        break;
                    case 2:
                        spawnPoint = area.getCell(xpos, zpos + 2);
                        block[0][2] = 3;
                        block[1][2] = 3;
                        block[1][1] = 3;
                        break;
                }

                break;
            case GM_GA_CellBlockType.ARTIFAKT:
                /**
                    * 
                    * Up to three random placed stones 
                    * 
                    * */
                spawnPoint = area.getCell(xpos, zpos + 2);
                block[0][2] = 3;
                block[1][2] = 3;
                block[1][1] = 3;

                break;
        }

        return spawnPoint;

    }

    public void createCells(GM_GameArea area)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                area.getCell(xpos + i, zpos + j).setType(block[i][j]);
            }
        }
    }

    public void fillWithBoxes(float density)
    {

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {

                if (block[i][j] == 0)
                {
                    if ((int)UnityEngine.Random.value < density)
                    {
                        block[i][j] = 1;
                    }
                }

            }
        }
    }

}
