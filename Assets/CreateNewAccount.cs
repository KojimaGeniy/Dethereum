using System.Collections;
using System.Collections.Generic;
// Here we import the Netherum.JsonRpc methods and classes.
using Nethereum.JsonRpc.UnityClient;
using UnityEngine;

public class CreateNewAccount : MonoBehaviour
{
    //example address "0x00426144802b6F195c551b97b3b6950AaA012d35"

    // Use this for initialization
    void Start()
    {
        StartCoroutine(ManageAccount.getAccountBalance("0x00426144802b6F195c551b97b3b6950AaA012d35", (balance) => {
            Debug.Log(balance);
        }));

        // Password to encrypt the new account and a callback
        //CreateAccount("strong_password", (address, encryptedJson) => {
        //    // We just print the address and the encrypted json we just created
        //    Debug.Log(address);
        //    Debug.Log(encryptedJson);

        //    // Then we check the balance like before but in this case using the new account
        //    StartCoroutine(getAccountBalance(address, (balance) => {
        //        Debug.Log(balance);
        //    }));
        //});


    }

    // Code Comments for account creation and balance checks are available here:
    // https://gist.github.com/e11io/88f0ae5831f3aa31651f735278b5b463
    // This function will just execute a callback after it creates and encrypt a new account
    public void CreateAccount(string password, System.Action<string, string> callback) {
        // We use the Nethereum.Signer to generate a new secret key
        var ecKey = Nethereum.Signer.EthECKey.GenerateKey();

        var address = ecKey.GetPublicAddress();
        var privateKey = ecKey.GetPrivateKeyAsBytes();

        // Then we define a new KeyStore service
        var keystoreservice = new Nethereum.KeyStore.KeyStoreService();

        // And we can proceed to define encryptedJson with EncryptAndGenerateDefaultKeyStoreAsJson(),
        // and send it the password, the private key and the address to be encrypted.
        var encryptedJson = keystoreservice.EncryptAndGenerateDefaultKeyStoreAsJson(password, privateKey, address);
        // Finally we execute the callback and return our public address and the encrypted json.
        // (you will only be able to decrypt the json with the password used to encrypt it)
        callback(address, encryptedJson);
    }
}