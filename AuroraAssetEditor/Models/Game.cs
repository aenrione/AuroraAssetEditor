using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraAssetEditor.Models
{
	public class Game : INotifyPropertyChanged
	{
		private string _title;
		private string _titleId;
		private string _dbId;
		private bool _isGameSelected;

		public string Title
		{
			get => _title;
			set { _title = value; OnPropertyChanged(nameof(Title)); }
		}

		public string TitleId
		{
			get => _titleId;
			set { _titleId = value; OnPropertyChanged(nameof(TitleId)); }
		}

		public string DbId
		{
			get => _dbId;
			set { _dbId = value; OnPropertyChanged(nameof(DbId)); }
		}

		public bool IsGameSelected
		{
			get => _isGameSelected;
			set { _isGameSelected = value; OnPropertyChanged(nameof(IsGameSelected)); }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}
