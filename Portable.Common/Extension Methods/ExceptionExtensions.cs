using System.Text;

namespace System
{
    public static class ExceptionExtensions
    {
        public static string FormatAsString(this Exception e)
        {
            var sb = new StringBuilder();
            WriteExceptionToStringBuilder(e, sb);
            return sb.ToString();
        }

        static void WriteExceptionToStringBuilder(Exception e, StringBuilder sb)
        {
            if (e == null)
            {
                sb.AppendLine("null");
                return;
            }

            sb.AppendLine("Type:");
            sb.AppendLine(e.GetType().FullName);
            sb.AppendLine();

            sb.AppendLine("Message:");
            sb.AppendLine(e.Message);
            sb.AppendLine();

            sb.AppendLine("StackTrace:");
            sb.AppendLine(e.StackTrace);
            sb.AppendLine();

            sb.AppendLine("Inner Exception:");
            WriteExceptionToStringBuilder(e.InnerException, sb);
        }
    }
}
