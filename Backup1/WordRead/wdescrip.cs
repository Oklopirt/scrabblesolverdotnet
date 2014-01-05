/*
 * Created by SharpDevelop.
 * User: Ethan Benjamin
 * Date: 6/12/2009
 * Time: 6:29 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace WordRead
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class wdescrip : IComparable
	{
		public int Col {get {return col;}}
		public int Row {get {return row;}}
		public int Length {get {return this.word.Length;}}
		public bool Dir {get {return dir;}}
		public string word;
		int col,row;
		bool dir;
		public int value;
		public wdescrip(string wo, int co, int ro, bool di, int va)
		{
			word = new string(wo.ToCharArray());
			col = co;
			row = ro;
			dir = di;
			value = va;
		}
		
		public int CompareTo(object obj){
			wdescrip other = (wdescrip)obj;
            return other.value.CompareTo(this.value);
		}
		
		public void printOut(){
			String desc = dir == true? "down" : "right";
			Console.WriteLine(word + " " + row + " " + col + " " + desc + "\n" + value + " points");
		}
	}
}
