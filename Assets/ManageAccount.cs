using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Hex.HexTypes;
using System;

public class ManageAccount : MonoBehaviour
{

    // Here we define accountAddress (the public key; We are going to extract it later using the private key)
    private string accountAddress;
    private string accountPrivateKey = "0xe11e11e11e11e11e11e11e11e11e11e11e11e11e11e11e11e11e11e11e11e11e";
    // This is the testnet we are going to use for our contract, in this case kovan
    private string _url = "https://kovan.infura.io";

    // We define a new PingContractService (this is the file we are going to create for our contract)
    private PingContractService pingContractService = new PingContractService();

    // Use this for initialization
    void Start()
    {
        // First we'll call this function which will extract and assign the public key from the accountPrivateKey defined above.
        importAccountFromPrivateKey();

        // After this we call the PingTransaction function to actually interact with the contract.
        // This function will create a new transaction to our contract, consuming gas to pay for it's computational costs.
        StartCoroutine(PingTransaction());

        StartCoroutine(getAccountBalance(accountAddress, (balance) => {
            Debug.Log("Account balance: " + balance);
        }));
    }

    public IEnumerator PingTransaction() {
        // Create the transaction input with encoded values for the function
        // We will need the public key (accountAddress), the private key (accountPrivateKey),
        // the pingValue we are going to send to our contract (10000),
        // the gas amount (50000 in this case),
        // the gas price (25), (you can send a gas price of null to get the default value)
        // and the ammount of ethers you want to transfer, remember that this contract doesn't receive
        // ethereum transfers, so we set it to 0. You can modify it and see how it fails.
        var transactionInput = pingContractService.CreatePingTransactionInput(
            accountAddress,
            accountPrivateKey,
            new HexBigInteger(10000),
            new HexBigInteger(50000),
            new HexBigInteger(25),
            new HexBigInteger(0)
        );

        // Here we create a new signed transaction Unity Request with the url, private key, and the user address we get before
        // (this will sign the transaction automatically :D )
        var transactionSignedRequest = new TransactionSignedUnityRequest(_url, accountPrivateKey, accountAddress);

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


    public void importAccountFromPrivateKey()
    {
        // Here we try to get the public address from the secretKey we defined
        try
        { 
            accountAddress = Nethereum.Signer.EthECKey.GetPublicAddress(accountPrivateKey);
        }
        catch (Exception e)
        {
            // If we catch some error when getting the public address, we just display the exception in the console
            Debug.Log("Error importing account from PrivateKey: " + e);
        }
    }

    // We create the function which will check the balance of the address and return a callback with a decimal variable
    public static IEnumerator getAccountBalance(string address, System.Action<decimal> callback) {
        // Now we define a new EthGetBalanceUnityRequest and send it the testnet url where we are going to
        // check the address, in this case "https://kovan.infura.io".
        // (we get EthGetBalanceUnityRequest from the Netherum lib imported at the start)
        var getBalanceRequest = new EthGetBalanceUnityRequest("https://rinkeby.infura.io");
        // Then we call the method SendRequest() from the getBalanceRequest we created
        // with the address and the newest created block.
        yield return getBalanceRequest.SendRequest(address, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

        // Now we check if the request has an exception
        if (getBalanceRequest.Exception == null)
        {
            // We define balance and assign the value that the getBalanceRequest gave us.
            var balance = getBalanceRequest.Result.Value;
            // Finally we execute the callback and we use the Netherum.Util.UnitConversion
            // to convert the balance from WEI to ETHER (that has 18 decimal places)
            callback(Nethereum.Util.UnitConversion.Convert.FromWei(balance, 18));
        }
        else
        {
            // If there was an error we just throw an exception.
            throw new System.InvalidOperationException("Get balance request failed");
        }
    }

}