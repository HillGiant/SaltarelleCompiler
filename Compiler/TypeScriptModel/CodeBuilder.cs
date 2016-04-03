using System.Collections.Concurrent;
using System.Text;

namespace TypescriptMode.Model
{
    public class CodeBuilder
    {
        private static readonly ConcurrentDictionary<int, string> _indents = new ConcurrentDictionary<int, string>();
        private int _indentLevel = 0;
        private readonly StringBuilder _sb;
        private bool _atLineStart;
        private bool _inline;

        private string GetIndent()
        {
            if (_inline)
            {
                return "";
            }

            string result;
            if (!_indents.TryGetValue(_indentLevel, out result))
            {
                result = new string('\t', _indentLevel);
                _indents.TryAdd(_indentLevel, result);
            }

            return result;
        }

        public CodeBuilder(int indentLevel = 0, bool inline = false)
            : this(new StringBuilder(), indentLevel, inline)
        {
        }

        public CodeBuilder(StringBuilder sb, int indentLevel = 0, bool inline = false)
        {
            this._sb = sb;
            this._indentLevel = indentLevel;
            this._atLineStart = true;
            this._inline = inline;
        }

        internal int IndentLevel { get { return _indentLevel; } }

        public CodeBuilder Indent()
        {
            _indentLevel++;
            return this;
        }

        public CodeBuilder Outdent()
        {
            _indentLevel--;
            return this;
        }

        public CodeBuilder Append(string value)
        {
            if (_atLineStart)
                _sb.Append(GetIndent());
            _atLineStart = false;
            _sb.Append(value);
            return this;
        }

        public CodeBuilder AppendFormat(string format, params object[] args)
        {
            Append(string.Format(format, args));
            return this;
        }

        public CodeBuilder AppendLine(string value)
        {
            Append(value);
            AppendLine();
            return this;
        }

        public CodeBuilder AppendLine()
        {
            if (_inline)
            {
                Append(" ");
                return this;
            }
            _sb.AppendLine();
            _atLineStart = true;
            return this;
        }

        public CodeBuilder PreventIndent()
        {
            _atLineStart = false;
            return this;
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}
