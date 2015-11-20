using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MACCABE_JAVA
{
    class Function
    {
        public string Source { get; set; }
        public string NameOfFunction { get; set;  }
        public int McCabe { get; set; }
        public int Meyer { get; set; }
    }
    class Analize
    {
        public Analize(string src)
        {
            DeleteLiterals(ref src);
            DeleteComments(ref src);
            Functions = new List<Function>();
            FindFunctions(src);

        }
        public List<Function> Functions { get; set; }

        public void DeleteLiterals(ref string src)
        {
            Regex find = new Regex("\"(?:\\\\\"|[^\"])*?\"", RegexOptions.IgnoreCase);
            src = find.Replace(src, "");
        }

        public void DeleteComments(ref string src)
        {
            Regex find = new Regex(@"(/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/)|(//.*)");
            src = find.Replace(src, "");
        }

        private int EndBraketsPosition(string src)
        {
            int countEnd = 0;
            int resultEnd = 0;
            for (int i = 0; (i < src.Length); i++)
            {
                if (src[i] == '{')
                {
                    countEnd++;
                }
                if (src[i] == '}')
                {
                    countEnd--;
                    if (countEnd == 0)
                    {
                        resultEnd = i;
                        break;
                    }
                }
            }
            return resultEnd;
        }
        private string NextTextInBrakes(string src)
        {
            string result = src.Remove(0, src.IndexOf('{') + 1).Remove(1 + EndBraketsPosition(src.Remove(0, src.IndexOf('{'))));

            return result;
        }

        public void FindFunctions(string src)
        {
            List<int> indexes = new List<int>();
            int priority = 0;
            int i = 0;
            foreach ( var lit in src)
            {
                if (lit == '{')
                {
                    priority++;
                    if (priority == 2)
                    {
                        string reversed = new string(src.Remove(i).Reverse().ToArray());
                        if ( reversed.IndexOf(")") < 32)
                        {
                            indexes.Add(i);
                        }
                        

                    }
                }
                if (lit == '}')
                {
                    priority--;
                }
                i++;
            }
            foreach ( var index in indexes)
            {
                var currentString = src.Remove(index);
                var delimiterstr =  currentString.Split('(');
                var delimiterstrfinaly = delimiterstr[delimiterstr.Count() - 2];
                delimiterstr = delimiterstrfinaly.Split(' ');

                currentString = NextTextInBrakes(src.Remove(0, index-3));

                Functions.Add(new Function() {NameOfFunction = delimiterstr[delimiterstr.Count()-1], Source = currentString,
                    McCabe = CalculateMcCabeNumber(currentString), Meyer = CalculateMeyersNumber(currentString) });        
            }
        }
        public int CalculateMcCabeNumber(string body)
        {
            int result = 1;
            Regex ifPattern = new Regex(@"if |if\(", RegexOptions.IgnoreCase);
            int a = ifPattern.Matches(body).Count;
            result += ifPattern.Matches(body).Count;

            Regex asksignpattern = new Regex(@"\?");
            result += asksignpattern.Matches(body).Count;

            Regex casePattern = new Regex("case ", RegexOptions.IgnoreCase);
            result += casePattern.Matches(body).Count;

            Regex forPattern = new Regex(@"for |for\(", RegexOptions.IgnoreCase);
            result += forPattern.Matches(body).Count;

            Regex whilePattern = new Regex(@"while |for\(", RegexOptions.IgnoreCase);
            result += whilePattern.Matches(body).Count;
            return result;
        }
        public int CalculateMeyersNumber(string body)
        {
            int result = 1;
            Regex Predicates = new Regex(@"( and)|( or)|(&&)|(\|\|)");
            List<int> indexes = new List<int>(); 

            Regex asksignpattern = new Regex(@"\?");
            result += asksignpattern.Matches(body).Count;

            Regex casePattern = new Regex("case ", RegexOptions.IgnoreCase);
            result += casePattern.Matches(body).Count;

            Regex ifPattern = new Regex(@"if |if\(", RegexOptions.IgnoreCase);
            for (int i = 0; i < ifPattern.Matches(body).Count; i++)
            {
                indexes.Add(ifPattern.Matches(body)[i].Index);
            }


            Regex forPattern = new Regex(@"for |for\(", RegexOptions.IgnoreCase);
            for (int i = 0; i < forPattern.Matches(body).Count; i++)
            {
                indexes.Add(forPattern.Matches(body)[i].Index);
            }

            Regex whilePattern = new Regex(@"while|while\(", RegexOptions.IgnoreCase);
            for (int i = 0; i < whilePattern.Matches(body).Count; i++)
            {
                indexes.Add(whilePattern.Matches(body)[i].Index);
            }
            foreach (var i in indexes)
            {
                result += 1 + Predicates.Matches(TextInBrakets(body.Remove(0, i))).Count;
            }

            return result;

        }
        private string TextInBrakets(string body)
        {

            string result = body.Remove(0, body.IndexOf('(') + 1).Remove(1 + EndPosition(body.Remove(0, body.IndexOf('('))));
            return result;
        }

        private int EndPosition(string src)
        {
            int counter = 0;
            int result = 0;
            for (int i = 0; (i < src.Length); i++)
            {
                if (src[i] == '(')
                {
                    counter++;
                }
                if (src[i] == ')')
                {
                    counter--;
                    if (counter == 0)
                    {
                        result = i;
                        break;
                    }
                }
            }
            return result;
        }


    }
    
}