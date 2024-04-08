using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacler : MonoBehaviour
{
    public LineRenderer[] tentacles;
    public LayerMask groundLayer;
    public TentacleItems[] tentacleItems = new TentacleItems[5];


    public float reach = 50f;

    public Vector2[] tentacleDir;

    public RaycastHit2D tentacleHit;


    public int connectedTentacles = 0;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++) {
            tentacleHit = Physics2D.Raycast(transform.position, tentacleDir[i], reach, groundLayer);
            if (tentacleHit.collider != null) {
                tentacleItems[i] = new TentacleItems(true, tentacleHit.point, tentacles[i]);
                connectedTentacles += 1;
            } else {
                tentacleItems[i] = new TentacleItems(false, transform.position, tentacles[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 5; i++) {
            if (Vector2.Distance(tentacleItems[i].targetPosition, transform.position) >= reach) {
                tentacleItems[i].targetPosition = transform.position;
                tentacleItems[i].connected = false;
                connectedTentacles -= 1;
            }

            if (!tentacleItems[i].connected) {
                tentacleHit = Physics2D.Raycast(transform.position, tentacleDir[i], reach, groundLayer);
            
                if (tentacleHit.collider != null) {
                    tentacleItems[i] = new TentacleItems(true, tentacleHit.point, tentacles[i]);
                    connectedTentacles += 1;
                }
            }
            

            CreateTentacles(tentacleItems[i]);
        }
    }   

    void CreateTentacles(TentacleItems _tentacleItem) {
        _tentacleItem.lineRender.SetPosition(0, transform.position);
        _tentacleItem.lineRender.SetPosition(1, Vector2.MoveTowards(_tentacleItem.lineRender.GetPosition(1),
                                                                        _tentacleItem.targetPosition, reach / 3f));
    }


    public class TentacleItems {
        public bool connected;
        public Vector2 targetPosition;
        public LineRenderer lineRender;

        public TentacleItems(bool _connected, Vector2 _targetPosition, LineRenderer _lineRender) {
            connected = _connected;
            targetPosition = _targetPosition;
            lineRender = _lineRender;
        }
    }

/*
    void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, reach);
    }
    */
}
