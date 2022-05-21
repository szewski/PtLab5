using PtLab.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PtLab.View
{
    public class DirectionBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Equals(ConvertEnum(parameter)))
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == false) return null;
            return ConvertEnum(parameter);
        }

        private SortOrder ConvertEnum(object parameter)
        {
            int intValue = Int32.Parse((string)parameter);
            SortOrder enumValue = (SortOrder)intValue;

            return enumValue;
        }
    }
}
