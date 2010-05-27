using SayMore.Model.Files;
using SayMore.Properties;

namespace SayMore.Model
{
	/// <summary>
	/// A session is an event which is recorded, documented, transcribed, etc.
	/// Each sesssion is represented on disk as a single folder, with 1 or more files
	/// related to that even.  The one file it will always have is some meta data about
	/// the event.
	/// </summary>
	public class Session : ProjectElement
	{
		//autofac uses this
		public delegate Session Factory(string parentElementFolder, string id);

		public Session(string parentElementFolder, string id,
			ComponentFile.Factory componentFileFactory,  FileSerializer fileSerializer)
			:base(parentElementFolder,id, componentFileFactory, fileSerializer, new SessionFileType())
		{
		}

		protected override string ExtensionWithoutPeriod
		{
			get { return ExtensionWithoutPeriodStatic; }
		}

		public override string RootElementName
		{
			get { return "Session"; }
		}

		protected static string ExtensionWithoutPeriodStatic
		{
			get { return Settings.Default.SessionFileExtension.TrimStart('.'); }
		}

	}
}