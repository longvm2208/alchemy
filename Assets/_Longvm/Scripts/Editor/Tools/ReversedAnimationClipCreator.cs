using System.IO;
using UnityEditor;
using UnityEngine;

public class ReversedAnimationClipCreator
{
    [MenuItem("Tools/My Tools/Create Reversed Animation Clip", false)]
    static void CreateReversedAnimationClip()
    {
        Object[] objects = Selection.objects;

        foreach (Object obj in objects)
        {
            CreateReversedAnimationClip(obj);
        }
    }

    [MenuItem("Tools/My Tools/Create Reversed Animation Clip", true)]
    static bool CreateReversedAnimationClipValidation()
    {
        Object[] objects = Selection.objects;

        if (objects == null || objects.Length == 0) return false;

        foreach (Object obj in objects)
        {
            if (obj != null && obj.GetType() == typeof(AnimationClip))
            {
                return true;
            }
        }

        return false;
    }

    static void CreateReversedAnimationClip(Object obj)
    {
        if (obj is AnimationClip originalClip)
        {
            string filePath = AssetDatabase.GetAssetPath(obj);
            string directoryPath = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fileExtension = Path.GetExtension(filePath);
            string copiedFilePath = Path.Combine(directoryPath, fileName + "_Reversed" + fileExtension);

            AssetDatabase.CopyAsset(filePath, copiedFilePath);
            var reversedClip = (AnimationClip)AssetDatabase.LoadAssetAtPath(copiedFilePath, typeof(AnimationClip));

            if (reversedClip == null) return;

            float clipLength = originalClip.length;

            // Get curve bindings
            EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(originalClip);
            foreach (EditorCurveBinding curveBinding in curveBindings)
            {
                // Get the animation curve for the current binding
                AnimationCurve curve = AnimationUtility.GetEditorCurve(originalClip, curveBinding);

                // Reverse the keys of the curve
                Keyframe[] keys = curve.keys;
                for (int i = 0; i < keys.Length; i++)
                {
                    Keyframe key = keys[i];
                    key.time = clipLength - key.time;
                    key.inTangent = -key.inTangent;
                    key.outTangent = -key.outTangent;
                    keys[i] = key;
                }

                // Set the reversed keys to the reversed clip
                AnimationUtility.SetEditorCurve(reversedClip, curveBinding, new AnimationCurve(keys));
            }

            // Copy animation events if any
            AnimationEvent[] events = AnimationUtility.GetAnimationEvents(originalClip);
            if (events.Length > 0)
            {
                for (int i = 0; i < events.Length; i++)
                {
                    events[i].time = clipLength - events[i].time;
                }
                AnimationUtility.SetAnimationEvents(reversedClip, events);
            }
        }
        else
        {
            Debug.LogWarning($"Selected object '{obj.name}' is not an AnimationClip.");
        }
    }
}
