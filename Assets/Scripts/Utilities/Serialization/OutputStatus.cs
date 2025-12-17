using UnityEngine;

namespace Arctic.Utilities.Serialization
{
    /// <summary>
    /// The status of the serialization serivce.
    /// </summary>
    public enum OutputStatus
    {
        Failed,
        Successful,
        ErrorDeserializing,
        ErrorSerializing,
        ErrorParsing,
        StringNotValid,
        DataCorrupted,
    }
}