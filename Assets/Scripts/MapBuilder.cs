using System;
using UnityEngine;
using UnityEngine.Serialization;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] private Grid _grid;               // Ссылка на компонент Grid для работы с сеткой
    [SerializeField] private Camera _camera;           // Ссылка на камеру
    [SerializeField] private Color _colorProhibiting;  // Цвет, обозначающий недопустимую позицию для тайла
    [SerializeField] private Color _colorResolving;    // Цвет, обозначающий разрешенную позицию для тайла
    [SerializeField] private Color _colorDefault;      // Исходный цвет тайла
    [SerializeField] private Vector3 _tileResize;      // Размер, на который будет увеличиватся тайл при наложении на занятую ячейку

    private string _thisIsPlane;                       // Строка, указывающая, что объект - это плоскость (например, через LayerMask - название слоя)
    private GameObject _selectedTilePrefab;            // Выбранный префаб тайла
    private Vector3Int _cellPosition;                  // Позиция в сетке в целых числах
    private Vector3 _worldCellPosition;                // Мировая позиция центра ячейки сетки
    private Renderer[] _tileElements;                  // Массив рендереров дочерних объектов тайла
    private OccupiedCells _occupiedCells;              // Экземпляр класса для отслеживания занятых ячеек
    private bool _tileInHand;                          // Флаг, указывающий, что тайл находится в руке
    private Vector3 _originalTileScale;                // Изначальный размер тайла
    
    //private bool _plane;

    private void Awake()
    {
        _camera = Camera.main;                          // Получаем главную камеру
        _occupiedCells = new OccupiedCells();           // Создаем экземпляр класса для отслеживания занятых ячеек
    }

    private void Update()
    {
        MovementTile();                                // Обрабатываем движение тайла
    }

    /// <summary>
    /// Выбор префаба тайлов
    /// </summary>
    /// <param name="tilePrefab"></param>
    public void StartPlacingTile(GameObject tilePrefab)
    {
        if (_tileInHand)
        {
            Destroy(_selectedTilePrefab);              // Уничтожаем предыдущий тайл в руке, если он был
            _tileInHand = false;                       // Сбрасываем флаг "тайл в руке"
        }

        _selectedTilePrefab = Instantiate(tilePrefab); // Создаем новый тайл
        _tileElements = _selectedTilePrefab.GetComponentsInChildren<Renderer>(); // Получаем рендереры его дочерних объектов
        _originalTileScale = _selectedTilePrefab.transform.localScale; // Сохраняем изначальный размер тайла
        _tileInHand = true;                            // Устанавливаем флаг "тайл в руке"
    }

    /// <summary>
    /// Метод перемещения тайлов
    /// </summary>
    private void MovementTile()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition); // Создаем луч из позиции мыши
        
        //Debug.DrawRay(ray.origin, ray.direction * 50, Color.green); // для дебага увидеть луч

        if (Physics.Raycast(ray, out var hitInfo, 50f)) // Пускаем луч и проверяем столкновение
        {
            // Поиск по леермаск
            int layerValue = hitInfo.collider.gameObject.layer;  // Получаем значение слоя объекта, с которым столкнулся луч
            string layerName = LayerMask.LayerToName(layerValue); // Получаем имя слоя по его значению
            _thisIsPlane = layerName;                            // Сохраняем результат в строку
            
            // Можно найти по тегу. Убрав поиск по леермаск
            // _plane = hitInfo.collider.CompareTag("Plane");

            Vector3 WorldPosition = hitInfo.point;               // Получаем мировую позицию точки столкновения
            _cellPosition = _grid.WorldToCell(WorldPosition);    // Преобразуем ее в позицию в сетке
            _worldCellPosition = _grid.GetCellCenterWorld(_cellPosition); // Получаем мировую позицию центра ячейки

            if (_selectedTilePrefab != null)
            {
                _selectedTilePrefab.transform.position = _worldCellPosition; // Перемещаем тайл в позицию ячейки

                if (_thisIsPlane != "Plane")
                {
                    SetColor(_colorProhibiting); // Устанавливаем цвет "недопустимой" позиции
                }
                else
                {
                    SetColor(_colorResolving);   // Устанавливаем цвет "разрешенной" позиции
                }
            }
        }

        if (_selectedTilePrefab != null && _thisIsPlane == "Plane")
        {
            if (Input.GetMouseButtonDown(0) && _occupiedCells.IsCellOccupied(_worldCellPosition) != true)
            {
                _selectedTilePrefab.transform.position = _worldCellPosition; // Перемещаем тайл в ячейку
                _occupiedCells.AddToDictionary(_worldCellPosition, true);   // Отмечаем ячейку как занятую
                SetColor(_colorDefault);                      // Устанавливаем цвет по умолчанию
                _selectedTilePrefab = null;                   // Сбрасываем выбранный тайл
                
            }
            else if (_occupiedCells.IsCellOccupied(_worldCellPosition) == true)
            {
                SetColor(_colorProhibiting);                  // Устанавливаем цвет "недопустимой" позиции

                float pingPongValue = Mathf.PingPong(Time.time * 2, 1f); // Получаем значение PingPong для создания эффекта мерцания
                _selectedTilePrefab.transform.localScale = Vector3.Lerp(_tileResize, _originalTileScale, pingPongValue); // Применяем измененный размер тайла
            }
            else
            {
                _selectedTilePrefab.transform.localScale = _originalTileScale; // Возвращаем изначальный размер тайла
            }
        }
    }
/// <summary>
/// Задаем цвет тайлов
/// </summary>
/// <param name="color"></param>
    private void SetColor(Color color)
    {
        foreach (var tileElement in _tileElements)
        {
            tileElement.material.color = color;             // Устанавливаем цвет всех рендереров тайла
        }
    }
}
