using System;
using System.IO;

namespace SayMore.Media.Audio
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Class to cache the duration and agregated samples (see comment on GetSamples) for a
	/// sound file.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class AudioFileHelper
	{
		public string AudioFilePath { get; private set; }

		/// ------------------------------------------------------------------------------------
		public AudioFileHelper(string audioFilePath)
		{
			AudioFilePath = audioFilePath;
		}

		/// ------------------------------------------------------------------------------------
		public TimeSpan AudioDuration
		{
			get
			{
				return new TimeSpan(24);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// This method will return a two-dimensional array of min and max samples, where the
		/// first dimension is the number of samples requested (assuming the file has enough)
		/// and the second dimension is the number of channels.
		///
		/// Since the number of samples requested will almost always be less than the number
		/// found in the file, the values in the file will be aggregated to provide a min and
		/// max across multiple contiguous samples.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public Tuple<float, float>[,] GetSamples(uint numberOfSamplesToReturn)
		{
			return new Tuple<float, float>[1,1];
		}

		/// ------------------------------------------------------------------------------------
		public static Tuple<float, float>[,] GetSamples(IWaveStreamReader stream,
			uint numberOfSamplesToReturn)
		{
			var sampleCount = stream.SampleCount;
			if (numberOfSamplesToReturn == 0 || sampleCount == 0 ||
				(stream.BitsPerSample == 32))
			{
				return new Tuple<float, float>[0, 0];
			}

			if (sampleCount < numberOfSamplesToReturn)
				numberOfSamplesToReturn = (uint)sampleCount;

			stream.Seek(0, SeekOrigin.Begin);
			var channels = stream.SamplingChannelCount;

			var samplesPerAggregate = sampleCount / (double)numberOfSamplesToReturn;
			var samplesToReturn = new Tuple<float, float>[numberOfSamplesToReturn, stream.NativeChannelCount];

			// To avoid compounding rounding errors as we get further along in the file, we need to separately track
			// the number of samples we've actually read and the ideal (unrounded) number of samples we should have
			// processed. Every time the rounding error builds to a whole integer, we read an extra sample to get them
			// back in synch. The buffer needs to be big enough to hold the extra sample for the times through the loop
			// when we're reading the higher number of samples.
			double idealSamplesProcessed = 0;
			var buffer = new float[(int)(Math.Ceiling(samplesPerAggregate)) * channels];
			int valuesToRead = (int)(Math.Floor(samplesPerAggregate)) * channels;

			int sampleIndex = 0;
			int read;
			long totalRead = 0;
			while (sampleIndex < numberOfSamplesToReturn && (read = stream.Read(buffer, valuesToRead)) > 0)
			{
				for (var c = 0; c < stream.NativeChannelCount; c++)
				{
					var biggestSample = float.MinValue;
					var smallestSample = float.MaxValue;

					for (int i = 0; i < read; i += channels)
					{
						biggestSample = Math.Max(biggestSample, buffer[i + c]);
						smallestSample = Math.Min(smallestSample, buffer[i + c]);
					}

					samplesToReturn[sampleIndex, c] = new Tuple<float, float>(biggestSample, smallestSample);
				}

				// See big comment above.
				valuesToRead = (int)(Math.Floor(samplesPerAggregate)) * channels;
				totalRead += (read / channels);
				idealSamplesProcessed += samplesPerAggregate;
				if (totalRead < Math.Floor(idealSamplesProcessed))
					valuesToRead += channels;

				sampleIndex++;
			}

			return samplesToReturn;
		}
	}
}
