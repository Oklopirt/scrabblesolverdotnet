using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WordRead
{
    public class Trie
    {
        public bool isWord { get; set; }

        private Trie[] children;

        public Trie()
        {
            isWord = false;
            children = new Trie[26];
        }

        public bool IsWord(string w)
        {
            Trie node = GetNode(this, w);
            if (node == null || !node.isWord)
                return false;
            return true;
        }

        public bool IsPrefix(string pre)
        {
            Trie node = GetNode(this, pre);
            if (node != null)
                return true;
            return false;
        }

        private static Trie GetNode(Trie t, string s)
        {
            if (s == "")
            {
                return t;
            }

            if (t == null)
                return null;

            else
            {
                char nextLetter = s[0];
                return GetNode(t[nextLetter], s.Substring(1));
            }
        }

        public static Trie FromFile(string filename)
        {
            Trie root = new Trie();
            var words = File.ReadAllLines(filename);
            foreach (var word in words)
            {
                AddWord(root, word);
            }
            return root;
        }

        public static Trie SuffixesFromFile(string filename)
        {
            Trie root = new Trie();
            var words = File.ReadAllLines(filename);
            foreach (var word in words)
            {
                AddWord(root, new String(word.Reverse().ToArray()));
            }
            return root;
        }

        public static void AddWord(Trie root, string word)
        {
            if (word == "")
            {
                root.isWord = true;
            }

            else
            {
                var nextLetter = word[0];
                var nextTrie = root[nextLetter];
                if (nextTrie == null)
                {
                    nextTrie = new Trie();
                    root[nextLetter] = nextTrie;
                }

                AddWord(nextTrie, word.Substring(1));
            }
        }

        static int Index(char c)
        {
            if (char.IsLower(c))
                return (int)(c - 'a');
            else if (char.IsUpper(c))
                return (int)(c - 'A');
            else
                return -1;
        }

        public Trie this[char c]
        {
            get
            {
                var ind = Index(c);
                if (ind > -1)
                    return children[ind];
                else
                    return null;
            }

            set
            {
                var ind = Index(c);
                children[ind] = value;
            }
        }


    }
}
