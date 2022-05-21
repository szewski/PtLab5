using PtLab.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace PtLab.View
{
    public class SortByBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            SortBy enumValue = ConvertEnum(parameter);

            if (value.Equals(enumValue))
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == false) return null;
            return ConvertEnum(parameter);
        }


        private SortBy ConvertEnum(object parameter)
        {
            int intValue = int.Parse((string)parameter);
            SortBy enumValue = (SortBy)intValue;

            return enumValue;
        }
    }
}
