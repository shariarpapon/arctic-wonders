using UnityEngine;

namespace Arctic.Serialization
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