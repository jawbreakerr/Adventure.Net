using System;
using System.Collections.Generic;
using System.Linq;

namespace Adventure.Net
{
    public class Grammar
    {
        public string Format { get; set; }
        public Func<bool> Action { get; set; }

        private IList<string> tokens;

        public const string Empty = "";

        public bool IsMulti
        {
            get { return Format.Contains("<multi>"); }
        }

        public bool IsMultiHeld
        {
            get { return Format.Contains("<multiheld>"); }
        }

        public IList<string> Tokens
        {
            get 
            { 
                if (tokens == null)
                {
                    string format = Format;
                    format = format.ToLower();
                    format = format.Replace(',', ' ');
                    tokens = new List<string>(format.Split(' '));
                    while (tokens.Contains(""))
                        tokens.Remove("");
                }

                return tokens;
            }
        }

        public bool IsPossibleMatch(IList<string> words)
        {
            foreach(string nonTag in NonTags)
            {
                if (!words.Contains(nonTag))
                    return false;
            }

            return true;
        }

        private IList<string> NonTags
        {
            get
            {
                return Tokens.Where(x => !x.IsTag()).ToList();
            }
        }
    }
}