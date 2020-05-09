using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(PrefabBrush))]
    public class PrefabBrushEditor : UnityEditor.Tilemaps.GridBrushEditor { }

    [CreateAssetMenu(fileName ="Prefab Brush", menuName = "Brushes/Prefab Brush")]
    [CustomGridBrush(false,true, false, "Prefab Brush")]
    public class PrefabBrush : UnityEditor.Tilemaps.GridBrush
    {
        public GameObject prefab;
        public int z;

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if(brushTarget.layer == 31)
            {
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if(instance != null)
            {
                Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefab Brush");
                instance.transform.SetParent(brushTarget.transform);
                instance.transform.position = gridLayout.LocalToWorld(gridLayout.CellToLocalInterpolated(
                    new Vector3Int(position.x, position.y, z) +
                    new Vector3(0.5f, 0.5f, 0.5f)));
            }
        }
    }
}
