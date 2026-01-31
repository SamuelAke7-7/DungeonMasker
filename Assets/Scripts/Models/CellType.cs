/// <summary>
/// Tipos de celdas que puede tener el grid del mapa
/// </summary>
public enum CellType
{
    Obstacle = 0,  // Obstáculo - no se puede caminar
    Path = 1,      // Camino - se puede caminar
    Door = 2       // Puerta - entrada o salida
}
