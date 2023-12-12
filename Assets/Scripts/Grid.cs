using UnityEngine;
using System.Collections.Generic;
using Pathfinding;
using System.Threading;

public class Grid : MonoBehaviour
{
   public GameObject creatureprefab;
   public GameObject[] treePrefabs;
   public Material terrainMaterial;
   public Material edgeMaterial;
   public int creaturenum = 50;
   public float waterlevel = 0.2f;
   public float scale = 0.1f;
   public float treeNoiseScale = 0.75f;
   public float treeDensity = .25f;
   public float treeNum = 150f;
   public int currentTreeNum = 0;
   public float creatureDensity = .2f;
   public int size = 100;
   public int bigTreeFoodValue = 40;
   public int bigTreeWaterValue = 20;
   public int medTreeFoodValue = 20;
   public int medTreeWaterValue = 10;
   public int smallTreeFoodValue = 10;
   public int smallTreeWaterValue = 5;
   public int small = 0;
   public int med = 0;
   public int large = 0;
   public int edited = 0;
   
    Cell[,] grid;

    void Start()
    {
        Setting();
        float[,] noiseMap = new float[size,size];
        float xOffset = Random.Range(-10000,10000);
        float yOffset = Random.Range(-10000,10000);
        for(int y = 0;y < size; y++)
        {
            for(int x = 0; x< size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset  , y * scale + yOffset );
                noiseMap[x, y] = noiseValue;
            }
        }

        float[,] falloffMap = new float[size,size];
        for(int y = 0; y<size; y++)
        {
            for(int x = 0; x<size; x++)
            {
                float xv = x / (float)size * 2 - 1;
                float yv = y / (float)size *2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv),Mathf.Abs(yv));
                falloffMap[x,y] = Mathf.Pow(v,3f) / (Mathf.Pow(v,3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
            }
        }
        
        grid = new Cell[size,size];
        for(int y = 0;y < size; y++)
        {
            for(int x = 0; x< size; x++)
            {
                Cell cell = new Cell();
                float noiseValue = noiseMap[x,y];
                noiseValue -= falloffMap[x,y];
                cell.isWater= noiseValue < waterlevel;
                grid[x,y] = cell;
            }
        }
        DrawTerrainMesh(grid);
        DrawTexture(grid);
        DrawEdgeMesh(grid);
        AstarPath.active.Scan();       
        SpawnCreatures(grid);
        InvokeRepeating("Tree",0f,120f);
        
    }
    void Setting()
    {
        creaturenum = PlayerPrefs.GetInt("CreatureNum", creaturenum);
        treeDensity = PlayerPrefs.GetFloat("TreeDensity", treeDensity);
        bigTreeFoodValue = PlayerPrefs.GetInt("BigTreeFoodValue", bigTreeFoodValue);
        bigTreeWaterValue = PlayerPrefs.GetInt("BigTreeWaterValue", bigTreeWaterValue);
        medTreeFoodValue = PlayerPrefs.GetInt("MedTreeFoodValue", medTreeFoodValue);
        medTreeWaterValue = PlayerPrefs.GetInt("MedTreeWaterValue", medTreeWaterValue);
        smallTreeFoodValue = PlayerPrefs.GetInt("SmallTreeFoodValue", smallTreeFoodValue);
        smallTreeWaterValue = PlayerPrefs.GetInt("SmallTreeWaterValue", smallTreeWaterValue);
        small = PlayerPrefs.GetInt("ToggleSmall",small);
        med= PlayerPrefs.GetInt("ToggleMed",med);
        large= PlayerPrefs.GetInt("ToggleLarge",large);
        List<GameObject> tempList = new List<GameObject>(treePrefabs);
        for (int i = tempList.Count - 1; i>=0; i--)
        {
            if(small == 0 && tempList[i].name == "Tree1")
            {
                tempList.RemoveAt(i);
                
            }
            else if( med == 0&& tempList[i].name == "Tree2")
            {
                tempList.RemoveAt(i);
  
            }
            else if(large == 0&& tempList[i].name == "Tree3")
            {
                tempList.RemoveAt(i);

            }
        }
        treePrefabs = tempList.ToArray();
    }

    void Tree()
    {
         GenerateTrees(grid);  
    }


    void DrawTerrainMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        for(int y= 0; y<size;y++)
        {
            for(int x = 0; x<size; x++)
            {
                Cell cell = grid[x,y];
                 if(!cell.isWater) 
                { 
                    Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                    Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                    Vector3 c = new Vector3(x - .5f, 0, y - .5f);
                    Vector3 d = new Vector3(x + .5f, 0, y - .5f);
                    Vector2 uvA = new Vector2(x / (float)size, y / (float)size);
                    Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                    Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                    Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);
                    Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                    Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                    for(int k = 0; k < 6; k++) {
                        vertices.Add(v[k]);
                        triangles.Add(triangles.Count);
                        uvs.Add(uv[k]);
                    }
                 }
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = false;
    }

    void DrawEdgeMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        for(int y = 0; y<size; y++)
        {
            for(int x= 0; x<size; x++)
            {
                Cell cell = grid[x,y];
                if(!cell.isWater)
                {
                    if(x>0)
                    {
                        Cell left = grid[x-1,y];
                        if(left.isWater)
                        {
                            Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                            Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                            Vector3 c = new Vector3(x - .5f, 0, y - .5f);
                            Vector3 d = new Vector3(x + .5f, 0, y - .5f);
                            Vector3[] v = new Vector3[] {a,b,c,b,d,c};
                            for(int k=0;k<6;k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if(x < size - 1) {
                        Cell right = grid[x + 1, y];
                        if(right.isWater) 
                        {
                            Vector3 a = new Vector3(x + .5f, 0, y - .5f);
                            Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                            Vector3 c = new Vector3(x + .5f, -1, y - .5f);
                            Vector3 d = new Vector3(x + .5f, -1, y + .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for(int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if(y > 0) {
                        Cell down = grid[x, y - 1];
                        if(down.isWater) 
                        {
                            Vector3 a = new Vector3(x - .5f, 0, y - .5f);
                            Vector3 b = new Vector3(x + .5f, 0, y - .5f);
                            Vector3 c = new Vector3(x - .5f, -1, y - .5f);
                            Vector3 d = new Vector3(x + .5f, -1, y - .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for(int k = 0; k < 6; k++) 
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if(y < size - 1)
                     {
                        Cell up = grid[x, y + 1];
                        if(up.isWater) 
                        {
                            Vector3 a = new Vector3(x + .5f, 0, y + .5f);
                            Vector3 b = new Vector3(x - .5f, 0, y + .5f);
                            Vector3 c = new Vector3(x + .5f, -1, y + .5f);
                            Vector3 d = new Vector3(x - .5f, -1, y + .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for(int k = 0; k < 6; k++) 
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                }

            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        GameObject edgeObj = new GameObject("Edge");
        edgeObj.transform.SetParent(transform);

        MeshFilter meshFilter = edgeObj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = edgeObj.AddComponent<MeshRenderer>();
        meshRenderer.material = edgeMaterial;
    }
    


    void DrawTexture(Cell[,] grid)
    {
        Texture2D texture = new Texture2D(size,size);
        Color[] colorMap = new Color[size *size];
        for(int y = 0; y<size; y++)
        {
            for(int x = 0;x<size ; x++)
            {
                Cell cell = grid[x,y];
                if(cell.isWater)
                {
                    colorMap[y * size + x] = Color.blue;
                }
                else
                {
                    colorMap[y * size + x] = Color.green;
                }
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colorMap);
        texture.Apply();

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = terrainMaterial;
        meshRenderer.material.mainTexture = texture;
        
    }

    void GenerateTrees(Cell[,] grid)
    {
        
        float[,] noiseMap = new float[size,size];
        float xOffset = Random.Range(-10000,10000);
        float yOffset = Random.Range(-10000,10000);
        for(int y = 0;y < size; y++)
        {
            for(int x = 0; x< size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * treeNoiseScale + xOffset  , y * treeNoiseScale + yOffset );
                noiseMap[x, y] = noiseValue;
            }
        }

        for(int y=0;y<size; y++)
        {
            for(int x=0;x<size; x++)
            {
                Cell cell = grid [x,y];
                if(!cell.isWater && currentTreeNum <= treeNum)
                {
                    float v = Random.Range(0f, treeDensity);
                    if(noiseMap[x,y]<v)
                    {
                        int numofpre = Random.Range(0,treePrefabs.Length);
                        GameObject prefab = treePrefabs[numofpre];
                        GameObject tree = Instantiate(prefab,transform);
                        List<GameObject> tempList = new List<GameObject>(treePrefabs);
                        currentTreeNum += 1;
                        if(tempList[numofpre].name == "Tree1")
                        {
                            TreeManager script =tree.AddComponent<TreeManager>();
                            script.HeightReq = 5f;
                            script.maxfoodcount = 5;
                            script.foodvalue = smallTreeFoodValue;
                            script.watervalue = smallTreeWaterValue;
                            script.AddFood();
                        }
                        else if(tempList[numofpre].name == "Tree2")
                        {
                            TreeManager script =tree.AddComponent<TreeManager>();
                            script.HeightReq = 10f;
                            script.maxfoodcount = 10;
                            script.foodvalue = medTreeFoodValue;
                            script.watervalue = medTreeWaterValue;
                            script.AddFood();
                        }
                        else if(tempList[numofpre].name == "Tree3")
                        {
                            TreeManager script =tree.AddComponent<TreeManager>();
                            script.HeightReq = 15f;
                            script.maxfoodcount = 20;
                            script.foodvalue = bigTreeFoodValue;
                            script.watervalue = bigTreeWaterValue;
                            script.AddFood();
                        }
                        tree.transform.position = new Vector3(x,0,y);
                        tree.transform.rotation = Quaternion.Euler(0, Random.Range(0,360f),0);
                        
                    }
                }
            }
        }

    }

    void SpawnCreatures(Cell[,] grid)
    {
        int currentCreatureCount = 0;
        float[,] noiseMap = new float[size,size];
        float xOffset = Random.Range(-10000,10000);
        float yOffset = Random.Range(-10000,10000);
        for(int y=0;y<size; y++)
        {
            for(int x=0;x<size; x++)
            {                  
                float noiseValue = Mathf.PerlinNoise(x * scale+ xOffset  , y * scale+ yOffset);
                noiseMap[x, y] = noiseValue;
                
            }
        }
        for(int y=0;y<size; y++)
        {
            for(int x=0;x<size; x++)
            {
                Cell cell = grid [x,y];
                if(!cell.isWater)
                {
                    if(creaturenum> currentCreatureCount)
                    {
                        float v = Random.Range(0f, creatureDensity);
                        if(noiseMap[x,y]<v)
                        {
                            GameObject creature = Instantiate(creatureprefab,transform);
                            FirstGen(creature);
                            creature.transform.position = new Vector3(x,0f,y);
                            currentCreatureCount ++;
                        }
                    }
                    
                }
            }
        }

    }

    void FirstGen(GameObject creature)
    {
        Movement script = creature.GetComponent<Movement>();
        script.fov = Random.Range(1,24);
        script.height = Random.Range(1,25 - (int)script.fov);
        script.moveSpeed = 25 - script.fov - script.height;
    }


   // void OnDrawGizmos()
   // {
       // if(!Application.isPlaying) return;
        //for(int y = 0; y< size;y++)
        //{
           // for(int x = 0; x< size ; x++)
            //{
              //  Cell cell = grid[x,y];
               // if(cell.isWater)
               // {
                //    Gizmos.color = Color.blue;
               // }
               // else
               // {
               //     Gizmos.color = Color.green;
               // }
               // Vector3 pos = new Vector3(x, 0, y);
               // Gizmos.DrawCube(pos,Vector3.one);
           // }
        //}
   // }
}
