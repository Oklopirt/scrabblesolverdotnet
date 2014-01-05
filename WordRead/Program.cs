/*
 * Created by SharpDevelop.
 * User: Ethan Benjamin
 * Date: 6/11/2009
 * Time: 1:33 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;



namespace WordRead
{
	class Program
	{
		public static void Main(string[] args)
		{
            Scrabble s = new Scrabble();
            Application.Run(new BoardDisplay(s));
          
			
		}
	}
}