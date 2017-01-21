namespace Verloka.HelperLib.Settings
{
    /// <summary>
    /// Interface for saving custom struct and class to register
    /// </summary>
    public interface ISettingStruct
    {
        /// <summary>
        /// Get value from fields
        /// </summary>
        /// <returns>string = "value1|value2|value3"</returns>
        string GetValue();
        /// <summary>
        /// Settings fileds by string from argument
        /// </summary>
        /// <param name="value">string = "value1|value2|value3"</param>
        void SetValue(string value);
    }
}
