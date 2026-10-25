using System;
using System.IO;
using NAudio.Wave;

namespace BpmTagger;

/// <summary>
/// Fast audio onset detection and autocorrelation-based BPM estimator.
/// </summary>
public static class BpmDetector
{
    private const int MinBpm = 60;
    private const int MaxBpm = 200;

    /// <summary>
    /// Analyzes an MP3 file and estimates its BPM.
    /// </summary>
    /// <param name="filePath">Path to the MP3 file.</param>
    /// <param name="previewSeconds">Duration in seconds to sample from the middle of the track (0 for full song).</param>
    /// <returns>Estimated integer BPM, or null if detection failed.</returns>
    public static int? DetectBpm(string filePath, int previewSeconds = 60)
    {
        try
        {
            using var reader = new AudioFileReader(filePath);

            int sampleRate = reader.WaveFormat.SampleRate;
            int channels = reader.WaveFormat.Channels;
            TimeSpan totalDuration = reader.TotalTime;

            // Determine read segment (sample the middle of the song for speed & reliable rhythm)
            long startPosition = 0;
            long maxBytesToRead = long.MaxValue;

            if (previewSeconds > 0 && totalDuration.TotalSeconds > previewSeconds + 15)
            {
                // Skip the first 15-30 seconds (often intro / beatless)
                double skipSeconds = Math.Min(30, (totalDuration.TotalSeconds - previewSeconds) / 2);
                startPosition = (long)(skipSeconds * reader.WaveFormat.AverageBytesPerSecond);
                // Align to block boundary
                startPosition -= startPosition % reader.WaveFormat.BlockAlign;
                reader.Position = startPosition;

                maxBytesToRead = (long)(previewSeconds * reader.WaveFormat.AverageBytesPerSecond);
            }

            // Downsample / frame energy calculation
            // Frame size: ~10ms window (e.g., 441 samples at 44.1kHz)
            int hopSize = sampleRate / 100; // 100 energy frames per second
            if (hopSize < 64) hopSize = 64;

            float envelopeSampleRate = (float)sampleRate / hopSize; // ~100 Hz

            // Read samples in chunks and compute energy frames
            var energyList = new List<float>();
            float[] sampleBuffer = new float[hopSize * channels];
            long bytesReadTotal = 0;

            ISampleProvider sampleProvider = reader.ToSampleProvider();
            while (bytesReadTotal < maxBytesToRead)
            {
                int samplesRead = sampleProvider.Read(sampleBuffer.AsSpan());
                if (samplesRead <= 0) break;

                bytesReadTotal += samplesRead * sizeof(float);

                // Convert to mono and compute frame energy (RMS)
                float sumSquares = 0f;
                int monoCount = samplesRead / channels;
                for (int i = 0; i < monoCount; i++)
                {
                    float monoSample = 0f;
                    for (int c = 0; c < channels; c++)
                    {
                        monoSample += sampleBuffer[i * channels + c];
                    }
                    monoSample /= channels;
                    sumSquares += monoSample * monoSample;
                }

                float rms = MathF.Sqrt(sumSquares / Math.Max(1, monoCount));
                energyList.Add(rms);
            }

            if (energyList.Count < 200) // At least 2 seconds needed
            {
                return null;
            }

            // First-order onset strength: rectified spectral/energy flux
            float[] onsets = new float[energyList.Count - 1];
            for (int i = 0; i < onsets.Length; i++)
            {
                float diff = energyList[i + 1] - energyList[i];
                onsets[i] = diff > 0 ? diff : 0f;
            }

            // Autocorrelation over BPM range [MinBpm, MaxBpm]
            float bestScore = -1f;
            int bestBpm = 120;

            for (int bpm = MinBpm; bpm <= MaxBpm; bpm++)
            {
                // Number of onset frames corresponding to 1 beat at this BPM
                float lagFrames = (60f * envelopeSampleRate) / bpm;
                int lag = (int)MathF.Round(lagFrames);

                if (lag <= 0 || lag >= onsets.Length / 2) continue;

                float correlation = 0f;
                int count = onsets.Length - lag;
                for (int i = 0; i < count; i++)
                {
                    correlation += onsets[i] * onsets[i + lag];
                }
                correlation /= count;

                // Mild Gaussian weighting around 120 BPM to resolve octave ambiguity
                // (e.g. 70 bpm vs 140 bpm)
                float dev = (bpm - 120f) / 50f;
                float priorWeight = MathF.Exp(-0.5f * dev * dev);
                float score = correlation * (0.8f + 0.2f * priorWeight);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestBpm = bpm;
                }
            }

            return bestBpm;
        }
        catch
        {
            return null;
        }
    }
}
