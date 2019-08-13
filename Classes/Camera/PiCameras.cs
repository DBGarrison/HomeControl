using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Classes
{
	public static class PiCameras
	{
		static PiCameras() { list = new List<PiCamera>(); buildDefaultList(); }



		public static List<PiCamera> list { get; set; }
		
		public static void buildDefaultList()
		{
			list.Clear();
			list.Add(new PiCamera("rpiZero1"));
			list.Add(new PiCamera("rpiZero2"));
			list.Add(new PiCamera("rpiZero3"));
			list.Add(new PiCamera("rpiZero4", 8085));
			list.Add(new PiCamera("rpiZero5", 8085));
			list.Add(new PiCamera("rpiZero6"));
			list.Add(new PiCamera("rpiZero7"));
			//list.Add(new PiCamera("rpiZero8"));
			list.Add(new PiCamera("rpiZero9"));
			list.Add(new PiCamera("rpiZero10"));
			list.Add(new PiCamera("rpiZero11"));
			list.Add(new PiCamera("rpiZero12"));
		}
	}
}
