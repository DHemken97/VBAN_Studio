namespace VBAN_Studio.Common.Attribute
{
    public class RegisterInputTypeAttribute : System.Attribute
    {

        public readonly string CommandType;

        public RegisterInputTypeAttribute(string commandType)
            => CommandType = commandType;
    }

}
