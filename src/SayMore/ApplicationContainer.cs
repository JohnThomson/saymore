using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using SayMore.Model;
using SayMore.Model.Files;
using SayMore.Properties;
using SayMore.UI.ProjectChoosingAndCreating;
using SilUtils;
using SplashScreen=SayMore.UI.SplashScreen;

namespace SayMore
{
	/// <summary>
	/// This is sortof a wrapper around the DI container. I'm not thrilled with the name I've
	/// used (jh).
	/// </summary>
	public class ApplicationContainer : IDisposable
	{
		private IContainer _container;
		private ISplashScreen _splashScreen;

		/// ------------------------------------------------------------------------------------
		public ApplicationContainer() : this(false)
		{
		}

		/// ------------------------------------------------------------------------------------
		public ApplicationContainer(bool showSplashScreen)
		{
			if (showSplashScreen)
			{
				_splashScreen = new SplashScreen();
				_splashScreen.Show();
			}

			var builder = new ContainerBuilder();
			builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly());

			//this one overrides the above, for commands, so that anything requiring
			//IEnumerable<ICommand> will get a list of *instances*, one each, of each command.
			builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
				.As<ICommand>()
				.Where(t => t.GetInterfaces().Contains(typeof(ICommand)));

//			var filesTypes = GetFilesTypes(parentContainer);
//			builder.RegisterInstance(filesTypes).As(typeof(IEnumerable<FileType>));

			//when something needs the list of filetypes, get them from this method
//			builder.Register<IEnumerable<FileType>>(c => GetFilesTypes(c));
			builder.RegisterInstance(ComponentRoles).As(typeof(IEnumerable<ComponentRole>));

			_container = builder.Build();
		}

		/// ------------------------------------------------------------------------------------
		public static string GetVersionInfo(string fmt)
		{
			var asm = Assembly.GetExecutingAssembly();
			var ver = asm.GetName().Version;
			var file = asm.CodeBase.Replace("file:", string.Empty);
			file = file.TrimStart('/');
			var fi = new FileInfo(file);

			return string.Format(fmt, ver.Major, ver.Minor,
				ver.Revision, fi.CreationTime.ToString("dd-MMM-yyyy"));
		}

		/// ------------------------------------------------------------------------------------
		public void SetProjectNameOnSplashScreen(string projectPath)
		{
			if (_splashScreen != null)
				_splashScreen.Message = Path.GetFileNameWithoutExtension(projectPath);
		}

		/// ------------------------------------------------------------------------------------
		public void CloseSplashScreen()
		{
			if (_splashScreen != null)
				_splashScreen.Close();

			_splashScreen = null;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Someday, we may put this under user control. For now, they are hard-coded.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected IEnumerable<ComponentRole> ComponentRoles
		{
			get
			{
				yield return new
					ComponentRole(typeof(Event), "original", "Original Recording",
						ComponentRole.MeasurementTypes.Time,
						ComponentRole.GetIsAudioVideo, "$ElementId$_Original",
						Settings.Default.OriginalRecordingStageColor);//todo... but maybe we dont' show this as a stage?

				yield return
					new ComponentRole(typeof(Event), "carefulSpeech", "Careful Speech",
						ComponentRole.MeasurementTypes.Time,
						ComponentRole.GetIsAudioVideo, "$ElementId$_Careful",
						Settings.Default.CarefulSpeechStageColor);

				yield return
					new ComponentRole(typeof(Event), "oralTranslation", "Oral Translation",
						ComponentRole.MeasurementTypes.Time,
						ComponentRole.GetIsAudioVideo, "$ElementId$_OralTranslation",
						Settings.Default.OralTranslationStageColor);

				yield return
					new ComponentRole(typeof(Event), "transcription", "Transcription",
						ComponentRole.MeasurementTypes.Words,
						(p => Path.GetExtension(p).ToLower() == ".txt"), "$ElementId$_Transcription",
						Settings.Default.TranscriptionStageColor);

				yield return
					new ComponentRole(typeof(Event), "transcriptionN", "Written Translation",
						ComponentRole.MeasurementTypes.Words,
						(p => Path.GetExtension(p).ToLower() == ".txt"), "$ElementId$_Translation-N",
						Settings.Default.WrittenTranslationStageColor);

				yield return
					new ComponentRole(typeof(Person), "consent", "Informed Consent",
						ComponentRole.MeasurementTypes.None, (p => true), "$ElementId$_Consent",
						System.Drawing.Color.Magenta); //todo
			}
		}

		/// ------------------------------------------------------------------------------------
		public WelcomeDialog CreateWelcomeDialog()
		{
			return _container.Resolve<WelcomeDialog>();
		}

		/// ------------------------------------------------------------------------------------
		public void Dispose()
		{
			_container.Dispose();
			_container = null;
		}

		/// ------------------------------------------------------------------------------------
		public ProjectContext CreateProjectContext(string projectSettingsPath)
		{
			return new ProjectContext(projectSettingsPath, _container);
		}
	}
}