using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Palaso.IO;
using Localization;

namespace SayMore.Media.UI
{
	#region MPlayerHelper class
	/// ----------------------------------------------------------------------------------------
	public static class MPlayerHelper
	{
		/// ------------------------------------------------------------------------------------
		public static string MPlayerPath
		{
			get { return FileLocator.GetFileDistributedWithApplication("mplayer", "mplayer.exe"); }
		}

		/// ------------------------------------------------------------------------------------
		public static ExternalProcess StartProcessToMonitor(IEnumerable<string> playbackArgs,
			DataReceivedEventHandler outputDataHandler, DataReceivedEventHandler errorDataHandler)
		{
			if (outputDataHandler == null)
				throw new ArgumentNullException("outputDataHandler");

			if (errorDataHandler == null)
				throw new ArgumentNullException("errorDataHandler");

			var prs = new ExternalProcess(MPlayerPath);
			prs.StartInfo.RedirectStandardInput = true;
			prs.StartInfo.RedirectStandardError = true;
			prs.OutputDataReceived += outputDataHandler;
			prs.ErrorDataReceived += errorDataHandler;
			prs.StartInfo.Arguments = BuildCommandLine(playbackArgs);

			if (!prs.StartProcess())
			{
				prs = null;
				var msg = LocalizationManager.GetString("CommonToMultipleViews.MediaPlayer.UnableToStartMplayerProcessMsg", "Unable to start mplayer.");
				Palaso.Reporting.ErrorReport.NotifyUserOfProblem(msg);
			}

			prs.StandardInput.AutoFlush = true;
			prs.BeginOutputReadLine();
			prs.BeginErrorReadLine();

			return prs;
		}

		#region Methods for building MPlayer command-line arguments.
		/// ------------------------------------------------------------------------------------
		public static IEnumerable<string> GetPlaybackArguments(float startPosition, float duration,
			float volume, int speed, int hwndVideo)
		{
			return GetPlaybackArguments(startPosition, duration, volume, speed, false, hwndVideo);
		}

		/// ------------------------------------------------------------------------------------
		public static IEnumerable<string> GetPlaybackArguments(float startPosition, float duration,
			float volume, int speed, bool resampleToMono, int hwndVideo)
		{
			var mplayerConfigPath = Path.Combine(GetPathToThisAssembly(), "MPlayerSettings.conf");

			if (File.Exists(mplayerConfigPath))
			{
				yield return string.Format("-include {0}", mplayerConfigPath);
			}
			else
			{
				yield return "-slave";
				yield return "-noquiet";
				yield return "-idle ";
				yield return "-msglevel identify=9:global=9";
				yield return "-nofontconfig";
				yield return "-autosync 100";
				yield return "-priority abovenormal";
				yield return "-osdlevel 0";
				yield return string.Format("-volume {0}", volume);
				yield return (resampleToMono ? "-af pan=1:1:1:1,scaletempo" : "-af scaletempo");

				if (speed != 100)
					yield return string.Format("-speed {0}", speed / 100d);

				if (startPosition > 0f)
					yield return string.Format("-ss {0}", startPosition);

				if (duration > 0f)
					yield return string.Format("-endpos {0}", duration);

				// A window handle of -1 means we're only playing back
				// the audio portion of a video file.
				if (hwndVideo == -1)
					yield return "-novideo";

				if (hwndVideo > 0)
				{
					yield return "-fixed-vo";

#if !__MonoCS__
					yield return "-vo gl";
#endif
					yield return string.Format("-wid {0}", hwndVideo);
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		private static string BuildCommandLine(IEnumerable<string> args)
		{
			var bldr = new StringBuilder();
			foreach (var arg in args)
				bldr.AppendFormat("{0} ", arg);

			return bldr.ToString();
		}

		#endregion

		/// ------------------------------------------------------------------------------------
		private static string GetPathToThisAssembly()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var mplayerConfigPath = assembly.CodeBase.Replace("file:", string.Empty).TrimStart('/');
			return Path.GetDirectoryName(mplayerConfigPath);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Extracts the audio stream from the specified video file and writes it to a wav
		/// file (i.e. raw PCM).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void ExtractAudioToWave(string videoPath, string audioOutPath)
		{
			videoPath = videoPath.Replace('\\', '/');

			var prs = new ExternalProcess(MPlayerPath);
			prs.StartInfo.Arguments =
				string.Format("\"{0}\" -nofontconfig -vo null -vc null -ao pcm:fast:file=%{1}%\"{2}\"",
				videoPath, audioOutPath.Length, audioOutPath);

			prs.StartProcess();
		}

		/// ------------------------------------------------------------------------------------
		public static string GetRawMediaInfoDump(string mediaFile)
		{
			string output = string.Empty;
			var prs = new ExternalProcess(MPlayerPath);
			prs.OutputDataReceived += (s, e) => output += (e.Data + Environment.NewLine);

			mediaFile = mediaFile.Replace('\\', '/');
			prs.StartInfo.Arguments = string.Format("-vo null -ao null -nofontconfig " +
				"-quiet -identify -endpos 0 \"{0}\"", mediaFile);

			if (prs.StartProcess())
			{
				prs.BeginOutputReadLine();
				prs.WaitForExit();
				prs.Close();
			}

			return output;
		}

		/// ------------------------------------------------------------------------------------
		public static string GetAudioEncoding(string mediaFilePath)
		{
			var mediaInfo = GetRawMediaInfoDump(mediaFilePath);
			var regex = new Regex(@"Selected audio codec:[^(]*\((?<codecName>[^()]*)");
			var match = regex.Match(mediaInfo);
			return (match.Success ? match.Result("${codecName}").Trim() : "UNKNOWN");
		}

		/// ------------------------------------------------------------------------------------
		public static int GetAudioChannels(string mediaFilePath)
		{
			var mplayerInfo = GetRawMediaInfoDump(mediaFilePath);
			int i = mplayerInfo.LastIndexOf("ID_AUDIO_NCH=", StringComparison.Ordinal);
			if (i < 0)
				return 1;

			return (int.TryParse(mplayerInfo.Substring(i + 13, 2), out i) ? i : 1);
		}

		/// ------------------------------------------------------------------------------------
		public static Image GetImageFromVideo(string videoPath, float seconds)
		{
			Image img = null;
			videoPath = videoPath.Replace('\\', '/');
			var prs = new ExternalProcess(MPlayerPath);
			var tmpFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(tmpFolder);
			var tmpFile = Path.Combine(tmpFolder, "00000001.jpg");

			try
			{
				prs.StartInfo.Arguments =
					string.Format("-nocache -nofontconfig -really-quiet -frames 1 -ss {0} -nosound -vo jpeg:outdir=\"\"\"{1}\"\"\" \"{2}\"",
					seconds, tmpFolder, videoPath);

				prs.StartProcess();
				prs.WaitForExit();
				prs.Close();

				// I'm hesitant to comment out this line, but because of SP-248, we'll see what happens.
				//ComponentFile.WaitForFileRelease(videoPath);

				if (File.Exists(tmpFile))
				{
					// I could use Image.FromFile, but that leaves
					// a lock on the file, for some reason.
					var stream = new FileStream(tmpFile, FileMode.Open);
					img = Image.FromStream(stream);
					stream.Close();
				}
			}
			finally
			{
				if (File.Exists(tmpFile))
					File.Delete(tmpFile);

				try { Directory.Delete(tmpFolder); }
				catch { }
			}

			return img;
		}
	}

	#endregion

	#region MPlayerOutputLogForm class
	/// ------------------------------------------------------------------------------------
	public class MPlayerOutputLogForm : Form
	{
		private readonly TextBox _textBox;

		/// --------------------------------------------------------------------------------
		public MPlayerOutputLogForm(string initialOutput)
		{
			_textBox = new TextBox();
			_textBox.ReadOnly = true;
			_textBox.BackColor = SystemColors.Window;
			_textBox.Multiline = true;
			_textBox.WordWrap = false;
			_textBox.ScrollBars = ScrollBars.Both;
			_textBox.Dock = DockStyle.Fill;
			_textBox.Font = SystemFonts.MessageBoxFont;
			_textBox.Text = initialOutput;

			var rc = Screen.PrimaryScreen.Bounds;
			Width = (rc.Width / 4);
			Height = (int)(rc.Height * 0.75);
			Location = new Point(rc.Right - Width, 0);
			StartPosition = FormStartPosition.Manual;
			Padding = new Padding(10);
			Text = @"Player Output Log";
			ShowIcon = false;
			ShowInTaskbar = false;
			MaximizeBox = false;
			MinimizeBox = false;
			Controls.Add(_textBox);
		}

		/// --------------------------------------------------------------------------------
		public void Clear()
		{
			Invoke((Action)(() => _textBox.Text = string.Empty));
		}

		/// --------------------------------------------------------------------------------
		public void UpdateLogDisplay(string output)
		{
			Invoke((Action)(() => _textBox.SelectionStart = _textBox.Text.Length));
			Invoke((Action<string>)(text => _textBox.SelectedText = text + Environment.NewLine), output);
		}

		/// --------------------------------------------------------------------------------
		protected override bool ShowWithoutActivation
		{
			get { return true; }
		}
	}

	#endregion
}
