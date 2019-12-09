using System;

namespace OptionalUI
{
    /// <summary>
    /// Exception that's thrown when any of your UIconfig's Key is null
    /// </summary>
    [Serializable]
    public class NullKeyException : ArgumentNullException
    {
        public NullKeyException() : base("Key for this UIconfig is null!")
        {

        }

        public NullKeyException(UIconfig instance) : base(string.Concat("Key for this ", instance.GetType().Name, " is null!"))
        {

        }

        protected NullKeyException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
        public NullKeyException(string message) : base(string.Concat("Key for this UIconfig is null!", Environment.NewLine, message))
        {
        }
        public NullKeyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception that's thrown when your mod tried to access MenuObject in UIelement outside Mod Config Screen
    /// </summary>
    [Serializable]
    public class InvalidMenuObjAccessException : NullReferenceException
    {
        public InvalidMenuObjAccessException(string message) : base(message)
        {
        }
        public InvalidMenuObjAccessException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public InvalidMenuObjAccessException() : base("If you are accessing MenuObject in UIelements, make sure those don't run when \'IfConfigScreen\' is \'false\'.")
        {
        }
        public InvalidMenuObjAccessException(Exception ex) : base(string.Concat("If you are accessing MenuObject in UIelements, make sure those don't run when \'IfConfigScreen\' is \'false\'.", Environment.NewLine, ex.ToString()))
        {
        }

        protected InvalidMenuObjAccessException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// You need at least one OpTab to contain any UIelements
    /// </summary>
    [Serializable]
    public class NoTabException : FormatException
    {
        public NoTabException(string modID) : base(string.Concat("NoTabException: ", modID, " OI has No OpTabs! ",
                        Environment.NewLine, "Did you put base.Initialize() after your code?",
                        Environment.NewLine, "Leaving OI.Initialize() completely blank will prevent the mod from using LoadData/SaveData."
                        ))
        {
        }

        public NoTabException()
        {
        }

        public NoTabException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoTabException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }

        
    }

    /// <summary>
    /// You need at least one OpTab to contain any UIelements
    /// </summary>
    [Serializable]
    public class DupelicateKeyException : FormatException
    {
        public DupelicateKeyException(string tab, string key) : base(string.Concat(string.IsNullOrEmpty(tab) ? "Tab" : "Tab ", tab, " has duplicated key for UIconfig.",
                          Environment.NewLine, "(dupe key: ", key, ")"))
        {
        }

        public DupelicateKeyException()
        {
        }
        public DupelicateKeyException(string message, Exception innerException) : base(message, innerException)
        {
        }
        protected DupelicateKeyException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
        public DupelicateKeyException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Problem occured in Option Menu Update
    /// </summary>
    [Serializable]
    public class GenericUpdateException : ApplicationException
    {
        public GenericUpdateException()
        {
        }

        public GenericUpdateException(string log) : base(log)
        {
        }
        public GenericUpdateException(string message, Exception innerException) : base(message, innerException)
        {
        }
        protected GenericUpdateException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Problem occured in LoadData
    /// </summary>
    [Serializable]
    public class LoadDataException : Exception
    {
        public LoadDataException(string message) : base(message)
        {
        }

        public LoadDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public LoadDataException()
        {
        }

        protected LoadDataException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Problem occured in SaveData
    /// </summary>
    [Serializable]
    public class SaveDataException : Exception
    {
        public SaveDataException(string message) : base(message)
        {
        }

        public SaveDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SaveDataException()
        {
        }

        protected SaveDataException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class ElementFormatException : FormatException
    {

        public ElementFormatException(UIelement element, string message, string key = "") : base(
            string.Concat(element.GetType().Name, " threw exception : ", message, string.IsNullOrEmpty(key) ? string.Empty : string.Concat(" (Key : ", key, ")"))
            )
        {
        }

        public ElementFormatException(string message) : base(string.Concat("Invalid argument for UIelement : ", message))
        {
        }
        public ElementFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
        protected ElementFormatException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
        public ElementFormatException() : base("One of UIelement threw exception for Invalid arguments!")
        {
        }
    }

}
