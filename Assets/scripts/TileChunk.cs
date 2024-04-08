using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileChunk : MonoBehaviour
{
    public GameObject tileGrid;
    TileChunker[] _tileChunks = new TileChunker[25];
    TileChunker _closestChunk;

    void Start()
    {
        for (int i = 0; i < 25; i++) {
            var tile = tileGrid.transform.GetChild(i).gameObject;
            _tileChunks[i] = new TileChunker(tile, tile.transform.GetChild(0).position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 25; i++) {
            if (Vector2.Distance(transform.position, _tileChunks[i].position) < 75f) {
                _tileChunks[i].tileMap.SetActive(true);
            } else {
                _tileChunks[i].tileMap.SetActive(false);
            }
        }
    }

    public class TileChunker {
        public GameObject tileMap;
        public Vector2 position;

        public TileChunker(GameObject _TM, Vector2 _pos) {
            tileMap = _TM;
            position = _pos;
        }
    }
}


