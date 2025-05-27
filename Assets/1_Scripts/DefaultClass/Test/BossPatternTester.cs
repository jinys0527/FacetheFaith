using UnityEngine;

public class BossPatternTester : MonoBehaviour
{

    [SerializeField] BossPatternManager bpm;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) bpm.Play(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) bpm.Play(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) bpm.Play(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) bpm.Play(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) bpm.Play(4);

    }
}
