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
        // �⺻ �ʵ� �׸���
        DrawDefaultInspector();

        MonsterData data = (MonsterData)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("=== ���� ���� ���� ===", EditorStyles.boldLabel);

        // ���� ����Ʈ ���
        for (int p = 0; p < data.attackPattern.Count; ++p)
        {
            var pattern = data.attackPattern[p];

            EditorGUILayout.BeginVertical("box");
            pattern.name = EditorGUILayout.TextField("���� �̸�", pattern.name);

            // 7x7 ��� �׸���
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

            // �� ��庰 ���� Ÿ�� ����
            for (int i = 0; i < pattern.nodes.Count; i++)
            {
                var node = pattern.nodes[i];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"({node.position.x},{node.position.y})", GUILayout.Width(60));
                node.attackType = (eMonsterAttackType)EditorGUILayout.EnumPopup(node.attackType);
                EditorGUILayout.EndHorizontal();

                pattern.nodes[i] = node; // ����ü�� �ٽ� �Ҵ� �ʿ�
            }

            // ���� ���� ��ư
            if (GUILayout.Button("�� ���� ����"))
            {
                data.attackPattern.RemoveAt(p);
                EditorUtility.SetDirty(data);
                break; // ����Ʈ ���� �Ŀ��� break �ʼ�
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8);
        }

        // �� ���� �߰� ��ư
        if (GUILayout.Button("�� ���� �߰�"))
        {
            data.attackPattern.Add(new BossPattern { name = $"���� {data.attackPattern.Count + 1}" });
            EditorUtility.SetDirty(data);
        }
    }
}
#endif
