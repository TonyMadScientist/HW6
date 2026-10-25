using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace CosmicRetro.Plants
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class CR_PlantSwitcherV3 : MonoBehaviour
    {
        [Header("Slots")]
        public Transform plantSlot;
        public Transform potSlot;

        [Header("Plant Growth Stages")]
        [Tooltip("Order: L1, L2, L3")]
        public List<GameObject> plantStages = new List<GameObject>();

        [SerializeField]
        [Tooltip("-1 = Empty / No Plant")]
        private int currentStage = -1;

        [Header("Pot Variants")]
        public List<GameObject> potVariants = new List<GameObject>();

        [SerializeField]
        [Tooltip("-1 = None")]
        private int currentPot = -1;

        [Header("Plant Height")]
        [Tooltip("Plant Slot Y when a pot is selected.")]
        public float plantYWithPot = 0.5f;

        [Tooltip("Plant Slot Y when no pot is selected.")]
        public float plantYWithoutPot = 0f;

        [Header("Options")]
        public bool resetSpawnedLocalTransform = true;

        public int CurrentStage => currentStage;
        public int CurrentPot => currentPot;

        private void Reset()
        {
            CreateSlotsIfMissing();
        }

        public void CreateSlotsIfMissing()
        {
            if (plantSlot == null)
                plantSlot = GetOrCreateChild("Plant Slot");

            if (potSlot == null)
                potSlot = GetOrCreateChild("Pot Slot");
        }

        private Transform GetOrCreateChild(string childName)
        {
            Transform found = transform.Find(childName);
            if (found != null)
                return found;

            GameObject go = new GameObject(childName);

#if UNITY_EDITOR
            if (!Application.isPlaying)
                Undo.RegisterCreatedObjectUndo(go, "Create " + childName);
#endif

            go.transform.SetParent(transform, false);
            return go.transform;
        }

        public void SetStage(int index)
        {
            if (plantStages == null || plantStages.Count == 0)
            {
                currentStage = -1;
                ClearSlot(plantSlot);
                return;
            }

            currentStage = Mathf.Clamp(index, -1, plantStages.Count - 1);
            RebuildPlant();
        }

        public void SetNoPlant()
        {
            currentStage = -1;
            RebuildPlant();
        }

        public void NextStage()
        {
            if (plantStages == null || plantStages.Count == 0) return;

            int next = currentStage + 1;
            if (next >= plantStages.Count)
                next = -1;

            SetStage(next);
        }

        public void PreviousStage()
        {
            if (plantStages == null || plantStages.Count == 0) return;

            int previous = currentStage - 1;
            if (previous < -1)
                previous = plantStages.Count - 1;

            SetStage(previous);
        }

        public void RandomStage()
        {
            if (plantStages == null || plantStages.Count == 0) return;

            // Random growth stage selects an actual plant.
            // Empty remains available manually.
            SetStage(Random.Range(0, plantStages.Count));
        }

        public void SetPot(int index)
        {
            if (potVariants == null || potVariants.Count == 0)
                currentPot = -1;
            else
                currentPot = Mathf.Clamp(index, -1, potVariants.Count - 1);

            ApplyPlantHeight();
            RebuildPot();
        }

        public void SetNoPot()
        {
            currentPot = -1;
            ApplyPlantHeight();
            RebuildPot();
        }

        public void NextPot()
        {
            if (potVariants == null || potVariants.Count == 0)
            {
                SetNoPot();
                return;
            }

            int next = currentPot + 1;
            if (next >= potVariants.Count)
                next = -1;

            SetPot(next);
        }

        public void PreviousPot()
        {
            if (potVariants == null || potVariants.Count == 0)
            {
                SetNoPot();
                return;
            }

            int previous = currentPot - 1;
            if (previous < -1)
                previous = potVariants.Count - 1;

            SetPot(previous);
        }

        public void RandomPot()
        {
            if (potVariants == null || potVariants.Count == 0)
            {
                SetNoPot();
                return;
            }

            // Intentionally excludes None.
            SetPot(Random.Range(0, potVariants.Count));
        }

        public void RandomizeAll()
        {
            if (plantStages != null && plantStages.Count > 0)
                currentStage = Random.Range(0, plantStages.Count);
            else
                currentStage = -1;

            if (potVariants != null && potVariants.Count > 0)
                currentPot = Random.Range(0, potVariants.Count);
            else
                currentPot = -1;

            RebuildAll();
        }

        public void RebuildAll()
        {
            CreateSlotsIfMissing();
            ClampValues();
            ApplyPlantHeight();
            RebuildPlant();
            RebuildPot();
        }

        public void RebuildPlant()
        {
            CreateSlotsIfMissing();
            ApplyPlantHeight();

            GameObject source = null;
            if (plantStages != null &&
                currentStage >= 0 &&
                currentStage < plantStages.Count)
            {
                source = plantStages[currentStage];
            }

            ReplaceSlot(plantSlot, source);
        }

        public void RebuildPot()
        {
            CreateSlotsIfMissing();

            GameObject source = null;
            if (potVariants != null &&
                currentPot >= 0 &&
                currentPot < potVariants.Count)
            {
                source = potVariants[currentPot];
            }

            ReplaceSlot(potSlot, source);
        }

        public void SaveCurrentPlantSlotYAsPotHeight()
        {
            CreateSlotsIfMissing();
            if (plantSlot != null)
                plantYWithPot = plantSlot.localPosition.y;

            ApplyPlantHeight();
        }

        public void ClearPreview()
        {
            ClearSlot(plantSlot);
            ClearSlot(potSlot);
        }

        private void ApplyPlantHeight()
        {
            if (plantSlot == null)
                return;

            Vector3 p = plantSlot.localPosition;
            p.y = currentPot >= 0 ? plantYWithPot : plantYWithoutPot;
            plantSlot.localPosition = p;
        }

        private void ClampValues()
        {
            if (plantStages == null || plantStages.Count == 0)
                currentStage = -1;
            else
                currentStage = Mathf.Clamp(currentStage, -1, plantStages.Count - 1);

            if (potVariants == null || potVariants.Count == 0)
                currentPot = -1;
            else
                currentPot = Mathf.Clamp(currentPot, -1, potVariants.Count - 1);
        }

        private void ReplaceSlot(Transform slot, GameObject source)
        {
            if (slot == null)
                return;

            ClearSlot(slot);

            if (source == null)
                return;

            GameObject instance = Spawn(source, slot);
            if (instance == null)
                return;

            instance.name = source.name;

            if (resetSpawnedLocalTransform)
            {
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;
                instance.transform.localScale = Vector3.one;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
                EditorUtility.SetDirty(instance);
#endif
        }

        private GameObject Spawn(GameObject source, Transform parent)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (PrefabUtility.IsPartOfPrefabAsset(source))
                {
                    GameObject prefabInstance =
                        PrefabUtility.InstantiatePrefab(source, parent) as GameObject;

                    if (prefabInstance != null)
                        return prefabInstance;
                }

                GameObject copy = Object.Instantiate(source, parent);
                Undo.RegisterCreatedObjectUndo(copy, "Spawn Plant Part");
                return copy;
            }
#endif

            return Object.Instantiate(source, parent);
        }

        private void ClearSlot(Transform slot)
        {
            if (slot == null)
                return;

            for (int i = slot.childCount - 1; i >= 0; i--)
            {
                GameObject child = slot.GetChild(i).gameObject;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    Undo.DestroyObjectImmediate(child);
                else
                    Object.Destroy(child);
#else
                Object.Destroy(child);
#endif
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CR_PlantSwitcherV3))]
    public class CR_PlantSwitcherV3Editor : UnityEditor.Editor
    {
        private CR_PlantSwitcherV3 Tool => (CR_PlantSwitcherV3)target;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Cosmic Retro Plant Switcher V3", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Assign L1/L2/L3 and pot prefabs. Use Empty for a planted-but-empty pot, then switch through L1, L2 and L3 as the plant grows.",
                MessageType.Info);

            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(10);
            DrawStageButtons();

            EditorGUILayout.Space(8);
            DrawPotButtons();

            EditorGUILayout.Space(10);

            if (GUILayout.Button("RANDOMIZE ALL", GUILayout.Height(30)))
            {
                Undo.RecordObject(Tool, "Randomize Plant");
                Tool.RandomizeAll();
                MarkDirty();
            }

            if (GUILayout.Button("REBUILD ALL", GUILayout.Height(26)))
            {
                Undo.RecordObject(Tool, "Rebuild Plant");
                Tool.RebuildAll();
                MarkDirty();
            }

            if (GUILayout.Button("Use Current Plant Slot Y As Pot Height"))
            {
                Undo.RecordObject(Tool, "Save Pot Height");
                Tool.SaveCurrentPlantSlotYAsPotHeight();
                MarkDirty();
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Create / Repair Slots"))
            {
                Undo.RecordObject(Tool, "Create Plant Slots");
                Tool.CreateSlotsIfMissing();
                MarkDirty();
            }

            if (GUILayout.Button("Clear Preview"))
            {
                Undo.RecordObject(Tool, "Clear Plant Preview");
                Tool.ClearPreview();
                MarkDirty();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawStageButtons()
        {
            EditorGUILayout.LabelField("Growth Stage", EditorStyles.boldLabel);

            int count = Tool.plantStages != null ? Tool.plantStages.Count : 0;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Empty", GUILayout.Height(25)))
            {
                Undo.RecordObject(Tool, "Remove Plant");
                Tool.SetNoPlant();
                MarkDirty();
            }

            for (int i = 0; i < count; i++)
            {
                string label = i < 3 ? $"L{i + 1}" : $"Stage {i + 1}";

                if (GUILayout.Button(label, GUILayout.Height(25)))
                {
                    Undo.RecordObject(Tool, "Change Growth Stage");
                    Tool.SetStage(i);
                    MarkDirty();
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Previous"))
            {
                Undo.RecordObject(Tool, "Previous Growth Stage");
                Tool.PreviousStage();
                MarkDirty();
            }

            if (GUILayout.Button("Next"))
            {
                Undo.RecordObject(Tool, "Next Growth Stage");
                Tool.NextStage();
                MarkDirty();
            }

            if (GUILayout.Button("Random"))
            {
                Undo.RecordObject(Tool, "Random Growth Stage");
                Tool.RandomStage();
                MarkDirty();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawPotButtons()
        {
            EditorGUILayout.LabelField("Pot", EditorStyles.boldLabel);

            string currentName = "None";

            if (Tool.CurrentPot >= 0 &&
                Tool.potVariants != null &&
                Tool.CurrentPot < Tool.potVariants.Count &&
                Tool.potVariants[Tool.CurrentPot] != null)
            {
                currentName = Tool.potVariants[Tool.CurrentPot].name;
            }

            EditorGUILayout.LabelField("Current", currentName);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("None"))
            {
                Undo.RecordObject(Tool, "Remove Pot");
                Tool.SetNoPot();
                MarkDirty();
            }

            if (GUILayout.Button("Previous"))
            {
                Undo.RecordObject(Tool, "Previous Pot");
                Tool.PreviousPot();
                MarkDirty();
            }

            if (GUILayout.Button("Next"))
            {
                Undo.RecordObject(Tool, "Next Pot");
                Tool.NextPot();
                MarkDirty();
            }

            if (GUILayout.Button("Random"))
            {
                Undo.RecordObject(Tool, "Random Pot");
                Tool.RandomPot();
                MarkDirty();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void MarkDirty()
        {
            EditorUtility.SetDirty(Tool);

            if (!Application.isPlaying && Tool.gameObject.scene.IsValid())
                EditorSceneManager.MarkSceneDirty(Tool.gameObject.scene);

            Repaint();
        }
    }
#endif
}
