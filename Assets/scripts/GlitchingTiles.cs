using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GlitchingTiles : MonoBehaviour
{
    public Transform bossTransform;
    public GameObject levelMap;
    public Tilemap glitchMap;
    public float range = 10f;
    public TileBase[] glitchTiles = new TileBase[15];

    TileChunker[] _tileChunks = new TileChunker[25];

    public float glitchDuration = 0.1f;
    public float glitchDurationCounter;


    // Start is called before the first frame update
    void Start()
    {
        glitchMap.size = new Vector3Int(300, 300, 0);
        glitchMap.origin = new Vector3Int(-147, -208, 0);
        glitchMap.ResizeBounds();

        glitchDurationCounter = glitchDuration;

        for (int i = 0; i < 25; i++) {
            var tile = levelMap.transform.GetChild(i).gameObject;
            _tileChunks[i] = new TileChunker(tile, tile.transform.GetChild(0).position);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (glitchDurationCounter < 0f){
            for(int x = glitchMap.cellBounds.min.x; x < glitchMap.cellBounds.max.x; x++){
                for(int y = glitchMap.cellBounds.min.y; y < glitchMap.cellBounds.max.y; y++){
                    var tilePos = new Vector3Int(x,y,0);

                    if (InRange (bossTransform.position, tilePos, range)) {
                        glitchMap.SetTile(tilePos, RandomTile(glitchTiles));
                        continue;
                    }
                    glitchMap.SetTile(tilePos, null);
                }    
            }

            glitchDurationCounter = glitchDuration;
        } else {
            glitchDurationCounter -= Time.deltaTime;
        }
    }

    TileBase RandomTile(TileBase[] _tilePool) {
        return _tilePool[Random.Range(0, _tilePool.Length)];
    }

    bool InRange(Vector2 _origin, Vector3Int _target, float _range) {

        if (Vector2.Distance(_origin, new Vector2(_target.x, _target.y)) > _range) {
            return false;
        } else {
            return true;
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
