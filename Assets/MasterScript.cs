using System.Collections;
using System.Collections.Generic;
// Here we import the Netherum.JsonRpc methods and classes.
using Nethereum.JsonRpc.UnityClient;
using UnityEngine;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.Encoders;
using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;

//Okay then
//We need to create accouns for new users and link them with GooglePlay accounts of their own
//--store their public/private key and password they chose for it, for extra protection
//getBalance ofc
//Methods to Create Session contract function(args as in contract + usr address and whatever)
//Enter Session(same)
//





public class MasterScript : MonoBehaviour {

    public static string ABI = @"[{""constant"":true,""inputs"":[],""name"":""pings"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""value"",""type"":""uint256""}],""name"":""ping"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""anonymous"":false,""inputs"":[{""indexed"":false,""name"":""pong"",""type"":""uint256""}],""name"":""Pong"",""type"":""event""}]";

    // https://kovan.etherscan.io/address/0xc4b054A90676fea7E8CBb8e311fd1ed086A296e1#code
    private static string contractAddress = "0xc4b054A90676fea7E8CBb8e311fd1ed086A296e1";

    private Contract sessionContract;

    private PingContractService pingContractService = new PingContractService();
    private string _url = "https://kovan.infura.io";


    // Use this for initialization
    void Start () {
        this.sessionContract = new Contract(null, ABI, contractAddress);

        //StartCoroutine(getAccountBalance("0x00426144802b6F195c551b97b3b6950AaA012d35", (balance) => {
        //    // When the callback is called, we are just going print the balance of the account
        //    Debug.Log(balance);
        //}));
    }

    // Update is called once per frame
    void Update () {
		
	}

    public static IEnumerator getAccountBalance(string address, System.Action<decimal> callback) {

        var getBalanceRequest = new EthGetBalanceUnityRequest("https://kovan.infura.io");

        yield return getBalanceRequest.SendRequest(address, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

        // Now we check if the request has an exception
        if (getBalanceRequest.Exception == null) {
            // We define balance and assign the value that the getBalanceRequest gave us.
            var balance = getBalanceRequest.Result.Value;
            // Finally we execute the callback and we use the Netherum.Util.UnitConversion
            // to convert the balance from WEI to ETHER (that has 18 decimal places)
            callback(Nethereum.Util.UnitConversion.Convert.FromWei(balance, 18));
        }
        else {
            // If there was an error we just throw an exception.
            throw new System.InvalidOperationException("Get balance request failed");
        }
    }

    public Function GetPingFunction()
    {
        return sessionContract.GetFunction("ping");
    }

    public TransactionInput CreatePingTransactionInput( string addressFrom, BigInteger pingValue,
        HexBigInteger gas = null,
        HexBigInteger gasPrice = null,
        HexBigInteger valueAmount = null) {

        var function = GetPingFunction();
        return function.CreateTransactionInput(addressFrom, gas, gasPrice, valueAmount, pingValue);
    }

    public IEnumerator PingTransaction()
    {
        // Create the transaction input with encoded values for the function
        // We will need the public key (accountAddress), the private key (accountPrivateKey),
        // the pingValue we are going to send to our contract (10000),
        // the gas amount (50000 in this case),
        // the gas price (25), (you can send a gas price of null to get the default value)
        // and the ammount of ethers you want to transfer, remember that this contract doesn't receive
        // ethereum transfers, so we set it to 0. You can modify it and see how it fails.
        var transactionInput = pingContractService.CreatePingTransactionInput(
            "0xe11e11e11e11e11e11e11e11e11e11e11e11e11e11e11e11e11e11e11e11e11e",
            "0xe11e11e11e1privatee11e11e11e11e11e11e11e11e11e11e11e11e11e11e11e",
            new HexBigInteger(10000),
            new HexBigInteger(50000),
            new HexBigInteger(25),
            new HexBigInteger(0)
        );

        // Here we create a new signed transaction Unity Request with the url, private key, and the user address we get before
        // (this will sign the transaction automatically :D )
        var transactionSignedRequest = new TransactionSignedUnityRequest(_url, "accountPrivateKey", "accountAddress");

        // Then we send it and wait
        Debug.Log("Sending Ping transaction...");
        yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);
        if (transactionSignedRequest.Exception == null)
        {
            // If we don't have exceptions we just display the result, congrats!
            Debug.Log("Ping tx submitted: " + transactionSignedRequest.Result);
        }
        else
        {
            // if we had an error in the UnityRequest we just display the Exception error
            Debug.Log("Error submitting Ping tx: " + transactionSignedRequest.Exception.Message);
        }
    }
}
