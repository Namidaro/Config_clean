namespace ULA.Common.Formatters
{
    /// <summary>
    ///     Exposes value formatting functionality
    ///     Описывает функционал форматеров
    /// </summary>
    public interface IValueFormatter
    {
        /// <summary>
        ///     Provedes formatting action over value
        /// </summary>
        /// <param name="value">Value to format</param>
        /// <returns>Resulted formatted value</returns>
        object Format(object value);

        /// <summary>
        ///     Provides backward formatting action over value
        /// </summary>
        /// <param name="value">Value to format backward</param>
        /// <param name="currentValue">Value to apply formatting to</param>
        /// <returns>Resulted formatted value</returns>
        object FormatBack(object value, object currentValue);
       
    }
}