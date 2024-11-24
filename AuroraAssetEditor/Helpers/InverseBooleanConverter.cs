using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AuroraAssetEditor.Helpers
{
	public class InverseBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// If the value is a boolean, return Visible or Collapsed based on the value
			if (value is bool booleanValue)
			{
				return booleanValue ? Visibility.Visible : Visibility.Hidden;
			}
			return Visibility.Hidden; // Default if value is not a boolean
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// Not needed for this case
			throw new NotImplementedException();
		}
	}


}
