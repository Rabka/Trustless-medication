using System;
using Gtk;
using System.Collections.Generic;
using MultichainCliLib;

public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnButton1ClientEvent (object o, ClientEventArgs args)
	{
		
	}

	protected async void OnButton1Clicked (object sender, EventArgs e)
	{
		//Make RPC connection to servernode 
		MultiChainClient client = new MultiChainClient("testChain"); 

		Dictionary<string,int> dictionary = new Dictionary<string, int>();
		dictionary.Add("F",1);
		dictionary.Add("S",1);

		var resp = client.PrepareLockUnspent(dictionary);

	}
}
