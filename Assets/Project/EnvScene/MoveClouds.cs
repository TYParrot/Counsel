using System.Collections;
using UnityEngine;

public class MoveClouds : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(MoveCloud());
    }

    IEnumerator MoveCloud()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f); // 10초 대기
            transform.position += new Vector3(0.25f, 0, 0); // X 좌표 -0.5 이동
        }
    }
}
