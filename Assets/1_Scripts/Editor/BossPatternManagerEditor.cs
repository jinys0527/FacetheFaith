#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BossPatternManager))]
public class BossPatternManagerEditor : Editor
{
    const int WIDTH = 7;
    const int HEIGHT = 7;

    public override void OnInspectorGUI()
    {
        // 기본 필드 먼저
        DrawDefaultInspector();
        BossPatternManager bpm = (BossPatternManager)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("=== 패턴 편집 ===", EditorStyles.boldLabel);

        // 패턴 리스트
        for (int p = 0; p < bpm.patterns.Count; ++p)
        {
            var pattern = bpm.patterns[p];
            pattern.name = EditorGUILayout.TextField("Name", pattern.name);

            // 7×7 토글
            for (int y = HEIGHT - 1; y >= 0; --y)        // 위쪽이 y=6
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < WIDTH; ++x)
                {
                    Vector2Int cell = new(x, y);
                    int index = pattern.nodes.FindIndex(n => n.position == cell);
                    bool on = index != -1;
                    bool next = GUILayout.Toggle(on, "", "Button", GUILayout.Width(22), GUILayout.Height(22));

                    if (next != on)
                    {
                        if (next)
                        {
                            if (!pattern.nodes.Any(n => n.position == cell))
                                pattern.nodes.Add(new NodeAttackInfo { position = cell, attackType = eMonsterAttackType.Default });
                        }
                        else
                        {
                            if (index != -1)
                                pattern.nodes.RemoveAt(index);
                        }

                        EditorUtility.SetDirty(bpm);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            // 위치별 공격 타입 설정
            for (int i = 0; i < pattern.nodes.Count; i++)
            {
                var node = pattern.nodes[i];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"({node.position.x},{node.position.y})", GUILayout.Width(60));
                node.attackType = (eMonsterAttackType)EditorGUILayout.EnumPopup(node.attackType);
                EditorGUILayout.EndHorizontal();

                pattern.nodes[i] = node; // 구조체면 반드시 다시 할당 필요!
            }

            EditorGUILayout.Space(6);
        }

        // 새 패턴 추가 버튼
        if (GUILayout.Button("Add New Pattern"))
            bpm.patterns.Add(new BossPattern());

    }
}
#endif