/// <summary>
/// Tipos de celdas que puede tener el grid del mapa
/// </summary>
public enum CellType
{
    Obstacle = 0,  // Obstáculo - no se puede caminar
    Path = 1,      // Camino - se puede caminar
    Entry = 2,       // Puerta - entrada
    Exit = 3,        // Puerta - salida, No sirve, osea no hace nada
    WallChanger = 4,
    Monster = 5,
    Objeto = 6
}
