#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(MonsterData))]
public class MonsterDataEditor : Editor
{
    const int WIDTH = 7;
    const int HEIGHT = 7;

    public override void OnInspectorGUI()
    {
        // 기본 필드 그리기
        DrawDefaultInspector();

        MonsterData data = (MonsterData)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("=== 공격 패턴 편집 ===", EditorStyles.boldLabel);

        // 패턴 리스트 출력
        for (int p = 0; p < data.attackPattern.Count; ++p)
        {
            var pattern = data.attackPattern[p];

            EditorGUILayout.BeginVertical("box");
            pattern.name = EditorGUILayout.TextField("패턴 이름", pattern.name);

            // 7x7 토글 그리드
            for (int y = HEIGHT - 1; y >= 0; --y)
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
                            if (index == -1)
                                pattern.nodes.Add(new NodeAttackInfo { position = cell, attackType = eMonsterAttackType.Default });
                        }
                        else
                        {
                            if (index != -1)
                                pattern.nodes.RemoveAt(index);
                        }

                        EditorUtility.SetDirty(data);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            // 각 노드별 공격 타입 설정
            for (int i = 0; i < pattern.nodes.Count; i++)
            {
                var node = pattern.nodes[i];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"({node.position.x},{node.position.y})", GUILayout.Width(60));
                node.attackType = (eMonsterAttackType)EditorGUILayout.EnumPopup(node.attackType);
                EditorGUILayout.EndHorizontal();

                pattern.nodes[i] = node; // 구조체는 다시 할당 필요
            }

            // 패턴 삭제 버튼
            if (GUILayout.Button("이 패턴 삭제"))
            {
                data.attackPattern.RemoveAt(p);
                EditorUtility.SetDirty(data);
                break; // 리스트 수정 후에는 break 필수
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8);
        }

        // 새 패턴 추가 버튼
        if (GUILayout.Button("새 패턴 추가"))
        {
            data.attackPattern.Add(new BossPattern { name = $"패턴 {data.attackPattern.Count + 1}" });
            EditorUtility.SetDirty(data);
        }
    }
}
#endif
