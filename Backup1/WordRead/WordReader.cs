/*
 * Created by SharpDevelop.
 * User: Ethan Benjamin
 * Date: 6/11/2009
 * Time: 2:10 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace WordRead
{

	public class WordReader
	{
		// char vals 65 to 90 no good
		const byte lowerbound = 33,upperbound=96;
		const byte lowerCaseOffset = 97;
		static readonly byte[] LetterDistribution = {9,2,2,4,12,2,3,2,9,1,1,4,2,6,8,2,1,6,4,6,4,2,2,1,2,1};
        //HashSet == SPEEEEEEEED
        private HashSet<string> words;
        private HashSet<string>[] beginnings = new HashSet<string>[15];
		
		public WordReader(string filename)
		{
			words = new HashSet<string>();
            for (int i = 0; i < 15; i++)
                beginnings[i] = new HashSet<string>();

			FileStream file = new FileStream(filename, FileMode.Open,FileAccess.ReadWrite);
			StreamReader sr = new StreamReader(file);
			String curString;
			while ((curString = sr.ReadLine()) != null){
                if (isValid(curString))
                {
                    words.Add(curString);
                    //add the beginnings
                    for (int j = 0; j < curString.Length; j++)
                    {
                        string addString = curString.Substring(0, j + 1);
                        beginnings[j].Add(addString);
                    }
                }
                
			}
			//SortWords();
			
		}
		
		public bool isWord(string w){
            w = w.ToLower();
            
            return words.Contains(w);

			/*int index = words.BinarySearch(w);
			return index>=0;*/
		}
		
		public bool anyStart(string w){
            w = w.ToLower();
            //will this work?
            bool canhaz = beginnings[w.Length - 1].Contains(w);
            return canhaz;


			/*int index = words.BinarySearch(w);
			if(index>=0)
				return true;
			int fake = ~index;
			if(fake>words.Count-1)
				return false;
			string cur = Enumerable.ElementAt(words,fake);
		
			if(cur.StartsWith(w)){
				//Console.WriteLine(cur + " starts with " + w);
				return true;
			}
			
			//Console.WriteLine(cur + " doesn't start with " + w);
			return false;*/
			
		}
		
		public void ListWords(){
			foreach (string s in words)
				Console.WriteLine(s);
		}
		
		/*public int WordIndex(String s){
			return words.BinarySearch(s);
		}
		
		private void FilterWords(){
			words.RemoveAll(isValid);
		}
		
		private void SortWords(){
			words.Sort();
		}*/
		
		private static bool isValid(String s){
			byte [] usage = new byte[26];
			for(int i = 0; i<26; i++)
				usage[i] = 0;
			
			if((s.Length > 15) || (s.Length == 1)){
				//Console.WriteLine(s + " is too long");
				return false;
			}
			foreach (char c in s){
				if ((c>=lowerbound && c<=upperbound))
					return false;
				int asciival = c-lowerCaseOffset;
				if(asciival>=0){
					usage[asciival]++;
					if(usage[asciival]>LetterDistribution[asciival]){
						//Console.WriteLine(s + " is a criminal at letter " + c);
						return false;}}
			}
			return true;
		}
	}
	
	

}
