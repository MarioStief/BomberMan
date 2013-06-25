using UnityEngine;
using System.Collections;

public class GM_GameArea : MonoBehaviour {

    private GM_GA_Cell[][] plane;

    public int xMax, zMax;
    public static float cWidth, cHeight;

    public GM_GA_Cell[] spawnPoints = null;

    public GM_GameArea(int xMax, int zMax, float width, float height)
    {

        this.xMax = xMax;
        this.zMax = zMax;

        cWidth = width;
        cHeight = height;

        plane = new GM_GA_Cell[xMax][];
        for (int i = 0; i < xMax; i++)
        {
            plane[i] = new GM_GA_Cell[zMax];
            for (int j = 0; j < zMax; j++)
            {
                plane[i][j] = new GM_GA_Cell(i, j, width, height, 0);
            }
        }

    }

    public GM_GA_Cell getCell(int x, int z)
    {
        //Debug.Log(x + "," + z);
        return plane[x][z];
    }

    /**
        * Gibt Zelle zurück, die x,z enthält. 
        * */
    public GM_GA_Cell getCell(float x, float z)
    {

        int x_m = (int)(x); // TODO!!
        int z_m = (int)(z);

        //Debug.Log(x_m + ", " + z_m + ": Cell-Type: " + plane[x_m][z_m].getType());

        return plane[x_m][z_m];
    }

    public int getWidth()
    {
        return xMax;
    }

    public int getHeight()
    {
        return zMax;
    }

    public Vector3 CenterOf(GM_GA_Cell cell)
    {
        return new Vector3(cell.getXPos() * cWidth + 0.5f, 0.5f, cell.getZPos() * cHeight + 0.5f);
    }

}
