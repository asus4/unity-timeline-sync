using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

[ExecuteInEditMode]
public class RandomMove : MonoBehaviour, ITimeControl
{
    public Vector3 posisionFreqency = Vector3.one;
    public Vector3 rotationFreqency = Vector3.one;

    public Vector3 posisionGain = Vector3.one;
    public Vector3 rotationGain = Vector3.one;

    Vector3 seedPX;
    Vector3 seedPY;
    Vector3 seedRX;
    Vector3 seedRY;
    double time;

    #region Life cycle

    void Start()
    {
        seedPX = new Vector3(0.4f, 0.8f, 0.2f);
        seedPY = new Vector3(0.1f, 0.9f, 0.6f);
        seedRX = new Vector3(0.5f, 0.7f, 0.2f);
        seedRY = new Vector3(0.7f, 0.9f, 0.3f);
    }

    void Update()
    {
        Vector3 pd = posisionFreqency * (float)time;
        Vector3 rd = rotationFreqency * (float)time;

        var p = new Vector3(
            (Mathf.PerlinNoise(seedPX.x + pd.x, seedPY.x + pd.x) - 0.5f) * posisionGain.x,
            (Mathf.PerlinNoise(seedPX.y + pd.y, seedPY.y + pd.y) - 0.5f) * posisionGain.y,
            (Mathf.PerlinNoise(seedPX.z + pd.z, seedPY.z + pd.z) - 0.5f) * posisionGain.z
        );
        var r = Quaternion.Euler(
            (Mathf.PerlinNoise(seedRX.x + rd.x, seedRY.x + rd.x) - 0.5f) * rotationGain.x,
            (Mathf.PerlinNoise(seedRX.y + rd.y, seedRY.y + rd.y) - 0.5f) * rotationGain.y,
            (Mathf.PerlinNoise(seedRX.z + rd.z, seedRY.z + rd.z) - 0.5f) * rotationGain.z
        );
        transform.SetPositionAndRotation(p, r);

        if (Application.isPlaying)
        {
            time += Time.deltaTime;
        }
    }

    #endregion // Life cycle

    #region ITimeControl
    public void OnControlTimeStart()
    {
    }

    public void OnControlTimeStop()
    {
    }

    public void SetTime(double time)
    {
        this.time = time;
    }

    #endregion // ITimeControl
}
