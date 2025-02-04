namespace VBAN_Studio.Common.Attributes
{
    public class RegisterOutputTypeAttribute : System.Attribute
    {

        public readonly string CommandType;

        public RegisterOutputTypeAttribute(string commandType)
            => CommandType = commandType;    
    }

}
