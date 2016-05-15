using System;
using System.Diagnostics;
using System.Collections.Generic;
using MultichainCliLib.Responses;
using MultichainCliLib.Requests;
using System.Threading;

namespace MultichainCliLib
{
	public class MultiChainClient
	{ 
		private string chainName;
		public MultiChainClient(string chainName,string nodeIp)
		{
			this.chainName = chainName;
			//is multichain running?
			Process compiler = new Process();
			compiler.StartInfo.FileName = "multichain-cli";
			compiler.StartInfo.Arguments = chainName + " gettotalbalances";
			compiler.StartInfo.UseShellExecute = false;
			compiler.StartInfo.RedirectStandardOutput = true;
			compiler.StartInfo.RedirectStandardError = true;
			compiler.Start();    

			compiler.WaitForExit(); 

			var jsonIn = compiler.StandardOutput.ReadToEnd();
			var errorIn = compiler.StandardError.ReadToEnd();

			if (errorIn.Contains("couldn't connect to server") || errorIn.Contains("No credentials found for chain")) {
				compiler = new Process();
				compiler.StartInfo.FileName = "multichaind";
				compiler.StartInfo.Arguments = chainName + "@"+nodeIp+" -daemon"; 

				compiler.StartInfo.UseShellExecute = false;
				compiler.StartInfo.RedirectStandardOutput = true;
				compiler.StartInfo.RedirectStandardError = true;
				compiler.Start (); 
				Thread.Sleep (3000);
			}
		}

		public ListAssetsResponse ListAssets()
		{
			return (ListAssetsResponse) Execute (new ListAssetsRequest ());
		}

		public PrepareLockUnspentResponse PrepareLockUnspent(Dictionary<string, int> amount)
		{
			return (PrepareLockUnspentResponse) Execute (new PrepareLockUnspentRequest (amount));
		}

		public GetNewAddressResponse GetNewAddress()
		{
			return (GetNewAddressResponse)Execute(new GetNewAddressRequest());
		}

		public SendToAddressResponse SendToAddress(string address,Dictionary<string, int> amount)
		{
			return (SendToAddressResponse)Execute(new SendToAddressRequest(address, amount));
		}

		public CreateRawTransactionResponse CreateRawTransaction(string address,string txid,int vout,Dictionary<string, int> amount)
		{
			return (CreateRawTransactionResponse)Execute(new CreateRawTransactionRequest(address, txid,vout,amount));
		}

		public AppendRawChangeResponse AppendRawChange(string hex, string address)
		{
			return (AppendRawChangeResponse)Execute(new AppendRawChangeRequest(hex, address));
		}

		public GetAddressesResponse GetAddresses()
		{
			return (GetAddressesResponse)Execute(new GetAddressesRequest());
		}

		public SignRawTransactionResponse SignRawTransaction(string hex)
		{
			return (SignRawTransactionResponse)Execute(new SignRawTransactionRequest(hex));
		}

		public SendRawTransactionResponse SendRawTransaction(string hex)
		{
			return (SendRawTransactionResponse)Execute(new SendRawTransactionRequest(hex));
		}

		public LockUnspentResponse LockUnspent(bool unlock, string txid,int vout)
		{
			return (LockUnspentResponse)Execute(new LockUnspentRequest(unlock,txid,vout));
		}

		public ImportAddressResponse ImportAddress(string address)
		{
			return (ImportAddressResponse)Execute(new ImportAddressRequest(address));
		}

		public GetAddressbalancesResponse GetAddressbalances(string address)
		{
			return (GetAddressbalancesResponse)Execute(new GetAddressbalancesRequest(address));
		}

		public GetTotalBalancesResponse GetTotalBalances()
		{
			return (GetTotalBalancesResponse)Execute(new GetTotalBalancesRequest());
		}
		public IssueResponse Issue(string address,string assetName,int qty,double unit)
		{
			return (IssueResponse)Execute(new IssueRequest(address,assetName,qty,unit));
		}
		public GetTxOutResponse GetTxOut(string txid,int vout)
		{
			return (GetTxOutResponse)Execute(new GetTxOutRequest(txid,vout));
		}
		private IResponse Execute(IRequest request)
			{
				try
				{
					Process compiler = new Process();
					compiler.StartInfo.FileName = "multichain-cli";
				    compiler.StartInfo.Arguments = chainName + " " + request.GetCompleteMethodString();
					compiler.StartInfo.UseShellExecute = false;
					compiler.StartInfo.RedirectStandardOutput = true;
					compiler.StartInfo.RedirectStandardError = true;
					compiler.Start();    

					compiler.WaitForExit(); 

					var jsonIn = compiler.StandardOutput.ReadToEnd();
					var errorIn = compiler.StandardError.ReadToEnd();
					
				if (errorIn != "")
					throw new Exception(string.Format("Failed to issue JSON-RPC request. JSON: {0}, errorMessage: {1}",request.ToString(), errorIn));
				// return...
				return request.GenerateResponse(jsonIn);
				}
				catch (Exception ex)
				{
				throw new InvalidOperationException(string.Format("Failed to issue JSON-RPC request. JSON: {0}",request.ToString()), ex);
				}
		}
	}
}

