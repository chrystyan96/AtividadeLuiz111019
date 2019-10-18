using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastMoveDirection {
    
    // é a direção que o raycast vai 
    private Vector2 raycastDirection;
    // é aonde os raycast começam e terminam
    private Vector2[] offsetPoints;
    // identifica onde colidir
    private LayerMask layerMask;
    private float addLength;

    // start: inicio do raycast
    // end: fim do raycast
    // dir: direção do raycast
    // mask: mascaras de colisões
    // parallelInset e perpendicularInset fazem com que o raycast não colida com objeto de origem 
    public RaycastMoveDirection(Vector2 start, Vector2 end, Vector2 dir, LayerMask mask, Vector2 parallelInset, Vector2 perpendicularInset) {
        this.raycastDirection = dir;
        this.offsetPoints = new Vector2[] {
            start + parallelInset + perpendicularInset, 
            end - parallelInset + perpendicularInset,
        };
        this.addLength = perpendicularInset.magnitude;
        this.layerMask = mask;
    }

    // calculo de distância
    // origin: posição do objeto que possui os raycasts
    public float DoRaycast(Vector2 origin, float distance) {
        float minDistance = distance;
        foreach(var offset in offsetPoints) {
            RaycastHit2D hit = Raycast(origin + offset, raycastDirection, distance + addLength, layerMask);
            if(hit.collider != null) {
                // atingimos algo!
                // com isso pegamos a distância entre os objetos
                minDistance = Mathf.Min(minDistance, hit.distance - addLength);
            }
        }
        // acertamos nada!
        return minDistance;
    }

    private RaycastHit2D Raycast(Vector2 start, Vector2 dir, float len, LayerMask mask) {
        Debug.DrawLine(start, start + dir * len, Color.red);
        return Physics2D.Raycast(start, dir, len, mask);
    }
}
