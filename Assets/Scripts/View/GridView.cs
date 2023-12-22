using UnityEngine;

namespace View
{
    public class GridView : MonoBehaviour
    {
        [SerializeField] private GameObject _tilePrefab;
        private int _width, _height;
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        void Start() 
        {
            _height = (int)(_camera.orthographicSize + 1);
            _width =  (int)(_camera.aspect * _height + 1);
            GenerateGrid();
        }
 
        void GenerateGrid() 
        {
            for (int x = -_width; x < _width; x++) 
            {
                for (int y = -_height; y < _height; y++) 
                {
                    var spawnedTile = Instantiate(_tilePrefab, new Vector3(x +0.5f, y+0.5f, 10), Quaternion.identity, gameObject.transform);
                    spawnedTile.name = $"Tile {x} {y}";
                }
            }
        }
    }
}
