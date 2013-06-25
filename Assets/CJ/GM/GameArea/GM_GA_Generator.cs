using UnityEngine;
using System.Collections;

public class GM_GA_Generator : MonoBehaviour {

    private int width = 40;
    private int height = 40;
    private int cWidth = 1;
    private int cHeight = 1;
    private float boxDensity = 0.9f;
    private int maxPlayers = 3;

    private GM_GameArea area = null;

    public GM_GameArea Generate()
    {
        createArea();
        return area;
    }

    private void createArea()
    {
        Object pre_solidCube = Resources.Load("SolidCube");

        // Erstelle Spielfläche abhängig von den Werten aus der init-datei
        area = new GM_GameArea(width, height, cWidth, cHeight);

        fillArea();

        // Create Border-Cubes
        for (int i = 0; i < width; i++)
        {
            Instantiate(pre_solidCube, new Vector3(i + 0.5f, 0.0f, -0.5f), Quaternion.identity);
        }

        for (int i = 0; i < width; i++)
        {
            Instantiate(pre_solidCube, new Vector3(i + 0.5f, 0.0f, height + 0.5f), Quaternion.identity);
        }

        for (int i = 0; i < height + 2; i++)
        {
            Instantiate(pre_solidCube, new Vector3(-0.5f, 0.0f, i - 0.5f), Quaternion.identity);
        }

        for (int i = 0; i < height + 2; i++)
        {
            Instantiate(pre_solidCube, new Vector3(width + 0.5f, 0.0f, i - 0.5f), Quaternion.identity);
        }
    }

    private void fillArea()
    {

        // Array of Cell-Types
        //Debug.Log("Array-dim: " + width/4 + " , " + height/4);
        GM_GA_CellBlock[][] cellBlocks = new GM_GA_CellBlock[width / 4][];
        for (int i = 0; i < width / 4; i++)
        {
            cellBlocks[i] = new GM_GA_CellBlock[height / 4];
        }

        // 1.: Place Solid Rocks!
        calcSolidRocks(cellBlocks);

        // 2.: Find possible, random SpawnPoints
        area.spawnPoints = calcSpawnPoints(cellBlocks);

        // 3.: Fill remaining entries with density% BoxCubes & create cells
        float density = boxDensity;
        for (int i = 0; i < width / 4; i++)
        {
            for (int j = 0; j < height / 4; j++)
            {

                cellBlocks[i][j].fillWithBoxes(density);
                cellBlocks[i][j].createCells(area);
            }
        }

        // 4.: Set some solid rocks near north/west border
        for (int i = 0; i < width; i++)
        {

            if (UnityEngine.Random.value < 0.1f)
            {
                area.getCell(i, height - 1).setType(2);
            }
        }
        for (int j = 0; j < height; j++)
        {

            if (UnityEngine.Random.value < 0.1f)
            {
                area.getCell(0, j).setType(2);
            }
        }


        //
        freeSingleBoxes();

        // Set Player-Position(s)

        int spawn = (int)(UnityEngine.Random.value * (maxPlayers - 1));
        //Debug.Log(spawnPoints[spawn]);
        //spawnPoints[0] = area.getCell(0,0);

        // player.transform.position = new Vector3(spawnPoints[spawn].getXPos() * initFile.getValue("cWidth") + 0.5f, 0.5f, spawnPoints[spawn].getZPos() * initFile.getValue("cHeight") + 0.5f);

    }

    private void calcSolidRocks(GM_GA_CellBlock[][] cellBlocks)
    {

        //Debug.Log("YEAH");
        int blubb = 0;
        for (int i = 0; i < width; i += 4)
        {
            for (int j = 0; j < height; j += 4)
            {


                if ((i == 0 || i + 4 >= width || j == 0 || j + 4 >= height))
                {	// NO FiveDice at border-positions!
                    blubb = (int)(UnityEngine.Random.value * 96 + 1);
                }
                else
                {
                    blubb = (int)(UnityEngine.Random.value * 99 + 1);
                }

                // Probabilities of cell-blocks:
                // TODO -> put it into enum
                // 50% FourDice
                // 35% Triangle
                // 12% Artifact
                // 3% FiveDice
                //				Debug.Log(i + " , " + j);
                if (blubb < 50)
                {

                    cellBlocks[i / 4][j / 4] = new GM_GA_CellBlock(i, j, GM_GA_CellBlockType.FOURDICE);
                }
                else if (blubb < 85)
                {

                    // Determine kind of triangle
                    blubb = (int)(UnityEngine.Random.value * 3);

                    if (blubb == 0)
                    {

                        cellBlocks[i / 4][j / 4] = new GM_GA_CellBlock(i, j, GM_GA_CellBlockType.TRIANGELDOWNLEFT);

                    }
                    else if (blubb == 1)
                    {

                        cellBlocks[i / 4][j / 4] = new GM_GA_CellBlock(i, j, GM_GA_CellBlockType.TRIANGLEUPLEFT);
                    }
                    else if (blubb == 2)
                    {

                        cellBlocks[i / 4][j / 4] = new GM_GA_CellBlock(i, j, GM_GA_CellBlockType.TRIANGLEUPRIGHT);
                    }
                    else if (blubb == 3)
                    {

                        cellBlocks[i / 4][j / 4] = new GM_GA_CellBlock(i, j, GM_GA_CellBlockType.TRIANGLEDOWNRIGHT);
                    }

                }
                else if (blubb < 97)
                {

                    cellBlocks[i / 4][j / 4] = new GM_GA_CellBlock(i, j, GM_GA_CellBlockType.ARTIFAKT);
                }
                else
                {
                    cellBlocks[i / 4][j / 4] = new GM_GA_CellBlock(i, j, GM_GA_CellBlockType.FIVEDICE);
                }
            }
        }
    }

    private GM_GA_Cell[] calcSpawnPoints(GM_GA_CellBlock[][] cellBlocks)
    {

        GM_GA_Cell[] spawnPoints = new GM_GA_Cell[maxPlayers];

        int i = 0;
        float angle = 2 * Mathf.PI / maxPlayers;
        int x, z;
        float x_start = (width - 1) / 2 - 1, z_start = 0;
        while (i < maxPlayers)
        {

            x = (int)(Mathf.Cos(angle * i) * x_start - Mathf.Sin(angle * i) * z_start + (width - 1) / 2);
            z = (int)(Mathf.Sin(angle * i) * x_start + Mathf.Cos(angle * i) * z_start + (height - 1) / 2);

            //Debug.Log(x + " : " +z);

            //if ( cellBlocks[x/4][z/4].readyForSpawn()){
            spawnPoints[i++] = cellBlocks[x / 4][z / 4].setSpawnPoint(area);
            //}
            //spawnPoints[i++] = cellBlocks[x/3xaf][z/3].setSpawnPoint();	
            //}

        }

        return spawnPoints;
    }

    private void freeSingleBoxes()
    {

        GM_GA_Cell currCell;
        GM_GA_Cell left, right, up, down;
        int non_null;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                non_null = 0;

                currCell = area.getCell(i, j);

                if (i != 0)
                {
                    left = area.getCell(i - 1, j);
                    non_null++;
                }
                else
                {
                    left = null;
                }

                if (i != width - 1)
                {
                    right = area.getCell(i + 1, j);
                    non_null++;
                }
                else
                {
                    right = null;
                }

                if (j != 0)
                {
                    up = area.getCell(i, j - 1);
                    non_null++;
                }
                else
                {
                    up = null;
                }

                if (j != height - 1)
                {
                    down = area.getCell(i, j + 1);
                    non_null++;
                }
                else
                {
                    down = null;
                }

                // Is it a single box?
                if ((left == null || left.getType() == 2) &&
                    (right == null || right.getType() == 2) &&
                    (up == null || up.getType() == 2) &&
                    (down == null || down.getType() == 2))
                {
                    // cj: unhandled case left = right = up = down = null

                    int rand = (int)(UnityEngine.Random.value * (non_null - 1));

                    if (rand == 0)
                    {
                        if (left != null)
                        {
                            left.setType(1);
                        }
                        else if (right != null)
                        {
                            right.setType(1);
                        }
                        else if (up != null)
                        {
                            up.setType(1);
                        }
                        else if(down != null)
                        {
                            down.setType(1);
                        }
                    }
                    else if (rand == 1)
                    {

                        if (right != null)
                        {
                            right.setType(1);
                        }
                        else if (up != null)
                        {
                            up.setType(1);
                        }
                        else if (down != null)
                        {
                            down.setType(1);
                        }
                        else if(left != null)
                        {
                            left.setType(1);
                        }

                    }
                    else if (rand == 2)
                    {

                        if (up != null)
                        {
                            up.setType(1);
                        }
                        else if (down != null)
                        {
                            down.setType(1);
                        }
                        else if (right != null)
                        {
                            right.setType(1);
                        }
                        else if(left != null)
                        {
                            left.setType(1);
                        }

                    }
                    else if (rand == 3)
                    {

                        if (down != null)
                        {
                            down.setType(1);
                        } if (up != null)
                        {
                            up.setType(1);
                        }
                        else if (left != null)
                        {
                            left.setType(1);
                        }
                        else if(right != null)
                        {
                            right.setType(1);
                        }
                    }
                }
            }
        }
    }

}
