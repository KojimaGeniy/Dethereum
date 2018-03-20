var HDWalletProvider = require("truffle-hdwallet-provider");

var infura_apikey = "RF12tXeeoCJRZz4txW2Y";
var mnemonic = "candy maple cake sugar pudding cream honey rich smooth crumble sweet treat";
var mnemonic2 = "notable decide attract ritual outer panda defy salt judge grape other bronze";


module.exports = {
  // See <http://truffleframework.com/docs/advanced/configuration>
  // to customize your Truffle configuration!
 networks: {
development: {
      host: "127.0.0.1",
      port: 9545,
      network_id: "8" // Match any network id
    },
    ropsten: {
      rovider: new HDWalletProvider(mnemonic, "https://ropsten.infura.io/"+infura_apikey),
      network_id: 3
    },
kovan: {
      provider: new HDWalletProvider(mnemonic2, "https://kovan.infura.io/"+infura_apikey),
      network_id: 42
    },
rinkeby: {
      provider: new HDWalletProvider(mnemonic2, "https://kovan.infura.io/"+infura_apikey),
      network_id: 4
    },
}
};
