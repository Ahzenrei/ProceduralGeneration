using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundToMesh : MonoBehaviour
{
    private const int sampleSize = 1024;
    private const int sampleDivision = 32;
    private const int historySize = 60;

    private int EiIndex;
    private float[] samples;
    private float[] instantSample;
    private float[,] historyBuffer;
    private float[] avgSample;
    private float[] instantDelta;

    private Texture2D texture;
    [SerializeField] private Material sideRoadMat;

    // Start is called before the first frame update
    void Start()
    {
        samples = new float[sampleSize];
        instantSample = new float[sampleDivision];
        historyBuffer = new float[historySize, sampleDivision];
        avgSample = new float[sampleDivision];
        instantDelta = new float[sampleDivision];

        texture = new Texture2D(sampleDivision, 1, TextureFormat.RFloat, false);
        texture.name = "Samples";
    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.GetSpectrumData(samples, 0, FFTWindow.Hanning);

        //Divide sample into multiples subband
        for (int i = 0; i < sampleDivision; i++)
        {
            float sum = 0;
            for (int j = i * sampleDivision; j < (i + 1) * sampleDivision; j++)
            {
                sum += samples[j];
            }
            instantSample[i] = (float)sampleDivision / sampleSize * sum;
        }

        //Compute avg
        for (int i = 0; i < sampleDivision; i++)
        {
            float sum = 0;
            for (int j = 0; j < historySize; j++)
            {
                sum += historyBuffer[j, i];
            }
            avgSample[i] = sum / historySize;
        }

        //Shift history buffer
        EiIndex = (EiIndex + 1) % historySize;

        //Add current value to history buffer and calculate instant delta
        for (int i = 0; i < sampleDivision; i++)
        {
            historyBuffer[EiIndex, i] = instantSample[i];
            instantDelta[i] = Mathf.Abs(Mathf.Log10(avgSample[i]) - Mathf.Log10(instantSample[i]));
        }

        //TO DO 
        // Make the sample division linear and send it to the bezier curve

        //Debug
        for (int i = 1; i < sampleDivision; i++)
        {
            Debug.DrawLine(new Vector3(i - 1, instantSample[i - 1] * 10000f, 0), new Vector3(i, instantSample[i] * 10000f, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log10(avgSample[i - 1]), 2), new Vector3(i, Mathf.Log10(avgSample[i]), 2), Color.cyan);
        }


        texture.SetPixelData(instantDelta, 0);

        sideRoadMat.SetTexture("_Samples", texture);
    }
}
