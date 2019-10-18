using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCheckTouch {

    private Vector2 raycastDirection;
    private Vector2[] offsetPoints;
    private LayerMask layerMask;
    private float raycastLength;

    public RaycastCheckTouch(Vector2 start, Vector2 end, Vector2 dir, LayerMask mask, Vector2 parallelInset, Vector2 perpendicularInset, float checkLen) {
        this.raycastDirection = dir;
        this.offsetPoints = new Vector2[] {
            start + parallelInset + perpendicularInset, 
            end - parallelInset + perpendicularInset,
        };
        this.raycastLength = perpendicularInset.magnitude + checkLen;
        this.layerMask = mask;
    }

    // retorna se atingimos algo
    public bool DoRaycast(Vector2 origin) {
        foreach(var offset in offsetPoints) {
            RaycastHit2D hit = Raycast(origin + offset, raycastDirection, raycastLength, layerMask);
            if(hit.collider != null) {
                return true;
            }
        }
        return false;
    }

    private RaycastHit2D Raycast(Vector2 start, Vector2 dir, float len, LayerMask mask) {
        Debug.DrawLine(start, start + dir * len, Color.green);
        return Physics2D.Raycast(start, dir, len, mask);
    }
}
