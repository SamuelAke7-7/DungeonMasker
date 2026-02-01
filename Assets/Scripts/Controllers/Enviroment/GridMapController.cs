using UnityEngine;
using System.IO;
using System;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Controlador para gestionar un grid de mapa con diferentes tipos de celdas
/// </summary>
public class GridMapController : MonoBehaviour
{
    [Header("Configuración del Grid")]
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private string jsonFileName = "dungeon_map.json";
    
    [Header("Colores de Gizmos")]
    [SerializeField] private Color obstacleColor = Color.red;
    [SerializeField] private Color pathColor = Color.green;
    [SerializeField] private Color doorColor = Color.blue;
    [SerializeField] private Color monsterColor = Color.aquamarine;

    public Material[] listMaterialWalls;
    
    public GameObject wallPrefab;
    public GameObject wallSecretPrefab;
    public GameObject[] listObjectPrefabs;

    public Transform wallParent;
    public Transform enemyParent;
    public Transform objectParent;

    public GameObject[] listMonsterPrefabs;

    private CellType[,] grid;
    private List<CellSecret> listCellSecrets;
    private List<CellMonster> listCellMonsters;
    private List<CellObject> listCellObjects;
    private Vector2Int startEntryPosition;
    private Vector2Int endExitPosition;
    
    public static GridMapController Instance { get; private set; }
    
    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        // Cargar el grid desde el JSON de ejemplo
        LoadGridFromJSON(jsonFileName);
        SetPlayerEntryPosition(startEntryPosition);
        SetWallsPosition();
    }

    void SetPlayerEntryPosition(Vector2Int position){
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 worldPosition = GridToWorld(position.x, position.y);
            player.transform.position = new Vector3(worldPosition.x + 0.5f, player.transform.position.y, worldPosition.z + 0.5f);
        }
    }

    void SetWallsPosition(){
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                CellType cellType = GetCellType(x, y);
                if (cellType == CellType.Obstacle)
                {
                    Vector3 worldPosition = GridToWorld(x, y);
                    worldPosition.x += 0.5f;
                    worldPosition.z += 0.5f;
                    GameObject wall = Instantiate(wallPrefab, worldPosition, Quaternion.identity);
                    wall.transform.parent = wallParent;
                }else if (cellType == CellType.WallChanger)
                {
                    Vector3 worldPosition = GridToWorld(x, y);
                    worldPosition.x += 0.5f;
                    worldPosition.z += 0.5f;
                    GameObject wallChanger = Instantiate(wallSecretPrefab, worldPosition, Quaternion.identity);
                    TypeMask typeMask = listCellSecrets.FirstOrDefault((cell) => cell.x == x && cell.z == y ).type;
                    wallChanger.GetComponent<WallSecretBehaviourUseCase>().type = typeMask;
                    wallChanger.transform.parent = wallParent;
                    wallChanger.GetComponent<MeshRenderer>().material = listMaterialWalls[(int)typeMask - 1];
                } else if(cellType == CellType.Monster){
                    Vector3 worldPosition = GridToWorld(x, y);
                    worldPosition.x += 0.5f;
                    worldPosition.z += 0.5f;
                    MonsterType type = listCellMonsters.FirstOrDefault((monster) => monster.x == x && monster.z == y).type;
                    GameObject enemy = Instantiate(listMonsterPrefabs[(int)type - 1], worldPosition, Quaternion.identity);
                    enemy.transform.parent = enemyParent;
                } else if (cellType == CellType.Objeto)
                {
                    Vector3 worldPosition = GridToWorld(x, y);
                    worldPosition.x += 0.5f;
                    worldPosition.z += 0.5f;
                    CellObject dataObject = listCellObjects.FirstOrDefault((objeto) => objeto.x == x && objeto.z == y);
                    Debug.Log(dataObject);
                    GameObject objeto = Instantiate(listObjectPrefabs[dataObject.idPrefab], worldPosition, Quaternion.identity);
                    objeto.transform.parent = objectParent;
                    if (dataObject.isWalkable)
                    {
                        ChangeTypeCell(x,y,CellType.Path);
                    }
                    else
                    {
                        ChangeTypeCell(x, y, CellType.Obstacle);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Establece el grid manualmente desde código
    /// </summary>
    public void SetGrid(CellType[,] newGrid)
    {
        if (newGrid == null)
        {
            Debug.LogError("Grid no puede ser null");
            return;
        }
        
        gridWidth = newGrid.GetLength(0);
        gridHeight = newGrid.GetLength(1);
        grid = newGrid;
        
        Debug.Log($"Grid establecido: {gridWidth}x{gridHeight}");
    }

    public void ChangeTypeCell(int x, int y, CellType type){
        grid[x,y] = type;
    }
    
    /// <summary>
    /// Obtiene el tipo de celda en una posición específica
    /// </summary>
    public CellType GetCellType(int x, int y)
    {
        if (grid == null || x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            return CellType.Obstacle; // Fuera de límites = obstáculo
        }
        
        return grid[x, y];
    }
    
    /// <summary>
    /// Verifica si una celda es transitable (camino, entrada o salida)
    /// </summary>
    public bool IsCellWalkable(int x, int y)
    {
        CellType cellType = GetCellType(x, y);
        return cellType == CellType.Path || cellType == CellType.Entry || cellType == CellType.Exit || cellType == CellType.Monster;
    }
    
    /// <summary>
    /// Convierte posición del mundo a coordenadas del grid
    /// </summary>
    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        int x = Mathf.FloorToInt(localPosition.x / cellSize);
        int y = Mathf.FloorToInt(localPosition.z / cellSize);
        return new Vector2Int(x, y);
    }
    
    /// <summary>
    /// Convierte coordenadas del grid a posición del mundo
    /// </summary>
    public Vector3 GridToWorld(int x, int y)
    {
        Vector3 localPosition = new Vector3(x * cellSize, 0, y * cellSize);
        return transform.TransformPoint(localPosition);
    }
    
    /// <summary>
    /// Carga el grid desde un archivo JSON
    /// </summary>
    public void LoadGridFromJSON(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        
        // Si no existe la carpeta StreamingAssets, crearla
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }
        
        // Si el archivo no existe, crear uno de ejemplo
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Archivo JSON no encontrado en {filePath}. Creando archivo de ejemplo...");
            CreateExampleMazeJSON(filePath);
        }
        
        try
        {
            string jsonContent = File.ReadAllText(filePath);
            GridData gridData = JsonUtility.FromJson<GridData>(jsonContent);
            
            if (gridData == null)
            {
                Debug.LogError("Error al parsear el JSON");
                return;
            }
            
            // Inicializar el grid
            gridWidth = gridData.width;
            gridHeight = gridData.height;
            listCellSecrets = gridData.cellSecret.ToList();
            listCellMonsters = gridData.cellMonster.ToList();
            listCellObjects = gridData.cellObject.ToList();
            grid = new CellType[gridWidth, gridHeight];
            
            // Convertir el array plano a matriz 2D
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    int index = y * gridWidth + x;
                    if (index < gridData.cells.Length)
                    {
                        grid[x, y] = (CellType)gridData.cells[index];
                    }
                    else
                    {
                        grid[x, y] = CellType.Obstacle;
                    }
                }
            }
            
            // Guardar posiciones de las puertas
            startEntryPosition = new Vector2Int(gridData.startX, gridData.startY);
            endExitPosition = new Vector2Int(gridData.endX, gridData.endY);
            
            Debug.Log($"Grid cargado desde JSON: {gridWidth}x{gridHeight}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al cargar el JSON: {e.Message}");
        }
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    /// <summary>
    /// Crea un archivo JSON de ejemplo con un laberinto
    /// </summary>
    private void CreateExampleMazeJSON(string filePath)
    {
        // Laberinto de ejemplo 15x15
        int width = 15;
        int height = 15;
        int[] cells = new int[width * height];
        
        // Inicializar todo como obstáculo
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (int)CellType.Obstacle;
        }
        
        // Crear un laberinto simple (algoritmo básico)
        // Primero crear caminos principales
        for (int y = 1; y < height - 1; y += 2)
        {
            for (int x = 1; x < width - 1; x += 2)
            {
                cells[y * width + x] = (int)CellType.Path;
            }
        }
        
        // Conectar caminos
        for (int y = 1; y < height - 1; y += 2)
        {
            for (int x = 1; x < width - 1; x += 2)
            {
                // Conectar hacia la derecha
                if (x + 2 < width - 1 && UnityEngine.Random.Range(0, 2) == 0)
                {
                    cells[y * width + (x + 1)] = (int)CellType.Path;
                }
                // Conectar hacia abajo
                if (y + 2 < height - 1 && UnityEngine.Random.Range(0, 2) == 0)
                {
                    cells[(y + 1) * width + x] = (int)CellType.Path;
                }
            }
        }
        
        // Crear un camino más claro desde inicio hasta fin
        // Camino horizontal superior
        for (int x = 1; x < width - 1; x++)
        {
            cells[1 * width + x] = (int)CellType.Path;
        }
        
        // Camino vertical derecho
        for (int y = 1; y < height - 2; y++)
        {
            cells[y * width + (width - 2)] = (int)CellType.Path;
        }
        
        // Camino horizontal inferior
        for (int x = width - 2; x > 0; x--)
        {
            cells[(height - 2) * width + x] = (int)CellType.Path;
        }
        
        // Puerta inicial (arriba a la izquierda)
        int startX = 1;
        int startY = 0;
        cells[startY * width + startX] = (int)CellType.Entry;
        
        // Puerta final (abajo a la derecha)
        int endX = width - 2;
        int endY = height - 1;
        cells[endY * width + endX] = (int)CellType.Exit;
        
        // Crear el objeto de datos
        GridData gridData = new GridData
        {
            width = width,
            height = height,
            cells = cells,
            startX = startX,
            startY = startY,
            endX = endX,
            endY = endY
        };
        
        // Serializar a JSON
        string json = JsonUtility.ToJson(gridData, true);
        
        // Guardar el archivo
        File.WriteAllText(filePath, json);
        Debug.Log($"Archivo JSON de ejemplo creado en: {filePath}");
    }
    
    /// <summary>
    /// Renderiza el grid usando Gizmos (solo visible en el editor)
    /// </summary>
    void OnDrawGizmos()
    {
        if (grid == null)
            return;
        
        // Dibujar cada celda del grid
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPos = GridToWorld(x, y);
                CellType cellType = grid[x, y];
                
                // Seleccionar color según el tipo de celda
                Color gizmoColor;
                switch (cellType)
                {
                    case CellType.Obstacle:
                        gizmoColor = obstacleColor;
                        break;
                    case CellType.Path:
                        gizmoColor = pathColor;
                        break;
                    case CellType.Entry:
                        gizmoColor = doorColor;
                        break;
                    case CellType.Exit:
                        gizmoColor = doorColor;
                        break;
                    default:
                        gizmoColor = Color.white;
                        break;
                }
                
                Gizmos.color = gizmoColor;
                
                // Dibujar un cubo para cada celda
                Vector3 center = worldPos + new Vector3(cellSize * 0.5f, 0, cellSize * 0.5f);
                Gizmos.DrawCube(center, new Vector3(cellSize * 0.9f, 0.1f, cellSize * 0.9f));
                
                // Dibujar wireframe para mejor visibilidad
                Gizmos.color = Color.black;
                Gizmos.DrawWireCube(center, new Vector3(cellSize * 0.9f, 0.1f, cellSize * 0.9f));
            }
        }
        
        // Dibujar líneas del grid
        Gizmos.color = Color.gray;
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = GridToWorld(x, 0);
            Vector3 end = GridToWorld(x, gridHeight);
            Gizmos.DrawLine(start, end);
        }
        
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = GridToWorld(0, y);
            Vector3 end = GridToWorld(gridWidth, y);
            Gizmos.DrawLine(start, end);
        }
    }
}
