using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SayMore.Model.Fields;
using SayMore.Model.Files.DataGathering;

namespace SayMore.ClearShare
{
	public class ContributorsListControlViewModel
	{
		public event EventHandler NewContributionListAvailable;

		private readonly OlacSystem _olacSystem = new OlacSystem();
		private readonly AutoCompleteValueGatherer _autoCompleteProvider;
		private Contribution _tempContribution;

		public ContributionCollection Contributions { get; protected set; }

		/// ------------------------------------------------------------------------------------
		public ContributorsListControlViewModel(AutoCompleteValueGatherer autoCompleteProvider)
		{
			_autoCompleteProvider = autoCompleteProvider;
		}

		/// ------------------------------------------------------------------------------------
		public IEnumerable<Role> OlacRoles
		{
			get { return _olacSystem.GetRoles(); }
		}

		/// ------------------------------------------------------------------------------------
		public void SetContributionList(ContributionCollection list)
		{
			Contributions = (list ?? new ContributionCollection());

			if (NewContributionListAvailable != null)
				NewContributionListAvailable(this, EventArgs.Empty);
		}

		/// ------------------------------------------------------------------------------------
		public AutoCompleteStringCollection GetAutoCompleteNames()
		{
			var list = (_autoCompleteProvider != null ?
				_autoCompleteProvider.GetValuesForKey("person") : new List<string>(0));

			var autoCompleteValues = new AutoCompleteStringCollection();
			autoCompleteValues.AddRange(list.ToArray());
			return autoCompleteValues;
		}

		/// ------------------------------------------------------------------------------------
		public bool GetCanDeleteContribution(int index)
		{
			return (Contributions != null && index >= 0 && index < Contributions.Count);
		}

		/// ------------------------------------------------------------------------------------
		public Contribution GetContributionAt(int index)
		{
			if (index == Contributions.Count && _tempContribution != null)
				return _tempContribution;

			return (Contributions == null || index < 0 || index >= Contributions.Count ?
				null : Contributions[index]);
		}

		/// ------------------------------------------------------------------------------------
		public Contribution GetContributionCopy(int index)
		{
			if (index == Contributions.Count && _tempContribution != null)
				return _tempContribution.Clone() as Contribution;

			return (index < 0 || index >= Contributions.Count ?
				null : Contributions[index].Clone() as Contribution);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates a temporary contribution that that is not committed to the model's
		/// contribution collection, but is treated as though it is and can later be
		/// committed to the collection using the CommitTempContributionIfExists method.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public Contribution CreateTempContribution()
		{
			_tempContribution = new Contribution();
			return _tempContribution;
		}

		/// ------------------------------------------------------------------------------------
		public void DiscardTempContribution()
		{
			_tempContribution = null;
		}

		/// ------------------------------------------------------------------------------------
		public void CommitTempContributionIfExists()
		{
			if (_tempContribution != null)
			{
				Contributions.Add(_tempContribution);
				_tempContribution = null;
			}
		}

		/// ------------------------------------------------------------------------------------
		public object GetContributionValue(int index, string valueName)
		{
			if (index == Contributions.Count && _tempContribution != null)
				return GetContributionValue(_tempContribution, valueName);

			return (index < 0 || index >= Contributions.Count ?
				null : GetContributionValue(Contributions[index], valueName));
		}

		/// ------------------------------------------------------------------------------------
		public object GetContributionValue(Contribution contribution, string valueName)
		{
			switch (valueName)
			{
				default: return null;
				case "name": return contribution.ContributorName;
				case "comments": return contribution.Comments;

				case "role":
					if (contribution.Role != null)
						return contribution.Role.Name;
					break;

				case "date":
					DateTime date;
					if (DateTime.TryParse(contribution.Date, out date))
						return date;
					break;
			}

			return null;
		}

		/// ------------------------------------------------------------------------------------
		public void SetContributionValue(int index, string valueName, object value)
		{
			if (index == Contributions.Count)
			{
				if (_tempContribution == null)
					_tempContribution = new Contribution();

				SetContributionValue(_tempContribution, valueName, value);
			}
			else if (index >= 0 && index < Contributions.Count)
				SetContributionValue(Contributions[index], valueName, value);
		}

		/// ------------------------------------------------------------------------------------
		public void SetContributionValue(Contribution contribution, string valueName, object value)
		{
			switch (valueName)
			{
				default: break;
				case "name": contribution.ContributorName = value as string; break;
				case "comments": contribution.Comments = value as string; break;

				case "role":
					Role role;
					if (_olacSystem.TryGetRoleByName(value as string, out role))
						contribution.Role = role;
					break;

				case "date":
					if (value != null && value.GetType() == typeof(DateTime))
						contribution.Date = ((DateTime)value).ToShortDateString();
					break;
			}
		}

		/// ------------------------------------------------------------------------------------
		public bool DeleteContribution(int index)
		{
			if (index < 0 || index >= Contributions.Count)
				return false;

			Contributions.RemoveAt(index);
			return true;
		}
	}
}
