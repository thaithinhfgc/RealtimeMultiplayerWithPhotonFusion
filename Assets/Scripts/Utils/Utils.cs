using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetRandomSpanwPosition()
    {
        return new Vector3(Random.Range(-8f, 8f), 10, Random.Range(-8f, 8f));
    }

    public static void SetRenderLayerInChildren(Transform transform, int layerNumber)
    {
        foreach (Transform tran in transform.GetComponentsInChildren<Transform>(true))
        {
            tran.gameObject.layer = layerNumber;
        }
    }
}
