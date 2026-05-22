using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Box2D : MonoBehaviour
{
    [SerializeField] Transform _transform;
    public new Transform transform => _transform;
    public Vector2 size = Vector2.one;
    public Vector2 offset;

    public Vector3 GetRandomPos()
    {
        Vector3 randomPos = new Vector2(
            Random.Range(-size.x * 0.5f, size.x * 0.5f),
            Random.Range(-size.y * 0.5f, size.y * 0.5f)
        );
        return transform.position + (Vector3)offset + randomPos;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_transform == null) _transform = GetComponent<Transform>();
    }

    private void OnDrawGizmos()
    {
        if (transform == null) return;
        Gizmos.DrawWireCube(transform.position + (Vector3)offset, size);
    }

    [ContextMenu("To Center")]
    void ToCenter()
    {
        if (transform == null) return;
        transform.position -= (Vector3)offset;
        offset = Vector2.zero;
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(Box2D))]
public class Box2DEditor : Editor
{
    private void OnSceneGUI()
    {
        Box2D box = (Box2D)target;

        if (box.transform == null) return;

        // Get the transform and initial properties
        Transform transform = box.transform;
        Vector3 center = transform.position + (Vector3)box.offset;
        Vector3 size = (Vector3)box.size;

        // Define handle positions
        Vector3[] handlePositions =
        {
            center + 0.5f * size.x * Vector3.right,
            center + 0.5f * size.x * Vector3.left,
            center + 0.5f * size.y * Vector3.up,
            center + 0.5f * size.y * Vector3.down
        };

        // Process each handle
        for (int i = 0; i < handlePositions.Length; i++)
        {
            Vector3 handlePos = handlePositions[i];
            Vector3 newHandlePos = Handles.FreeMoveHandle(handlePos, 0.1f, Vector3.zero, Handles.SphereHandleCap);

            if (newHandlePos != handlePos)
            {
                Undo.RecordObject(box, "Resize WireCube");

                Vector3 offset = newHandlePos - handlePos;

                if (i == 0) // Right handle
                {
                    box.size.x += offset.x;
                    box.offset.x += offset.x * 0.5f;
                }
                else if (i == 1) // Left handle
                {
                    box.size.x -= offset.x;
                    box.offset.x += offset.x * 0.5f;
                }
                else if (i == 2) // Top handle
                {
                    box.size.y += offset.y;
                    box.offset.y += offset.y * 0.5f;
                }
                else if (i == 3) // Bottom handle
                {
                    box.size.y -= offset.y;
                    box.offset.y += offset.y * 0.5f;
                }

                // Ensure minimum size
                box.size = new Vector2(Mathf.Max(box.size.x, 0.1f), Mathf.Max(box.size.y, 0.1f));
            }
        }

        // Mark object as dirty for updates
        if (GUI.changed)
        {
            EditorUtility.SetDirty(box);
        }
    }
}
#endif
