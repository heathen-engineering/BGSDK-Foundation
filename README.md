# Blockchain Game Software Development Kit (BGSDK): Foundation

Heathen Engineering's BGSDK Foundation allows you to manage in-game items as blockchain assets. It is a complete wrapper around [Venly](https://www.venly.io/)'s Web API. The tool simplifies integration with Venly API exposing all features and functions to C# classes and includes Editor extensions to aid in design and deployment of Contracts and Tokens.

## Requirements
Please note that you will need to register for an account with [Venly](https://www.venly.io/) to recieve the required Client ID and Secret used by the kit to connect to the [Venly](https://www.venly.io/) backend. This must be aquired from [Venly](https://www.venly.io/) directly.

## Documentaiton
* [Knowledge Base](https://kb.heathenengineering.com/assets/bgsdk)
* [Discord Server](https://discord.gg/6X3xrRc)

## Installation
* Package Manager
1) Open the package manager and click the '+' (plus) button located in the upper left of the window
2) Select `Add package from git URL...` when prompted provide the following URL:  
`https://github.com/heathen-engineering/BGSDK-Foundation.git?path=/Package`  

* A version of the package will be made availabel via the [Unity Asset Store](http://comingSoon) at a later date.  

## Features

* Define Contracts and Tokens in your game's assets as scriptable objects.
* Visualy edit Contracts and Tokens with the BGSDK Manager window
* Deploy new contracts and tokens to the [Venly](https://www.venly.io/) backend services from with in the editor
* Read existing Contracts and Tokens from the [Venly](https://www.venly.io/) backend creating the related scriptable objects automatically.
* Read wallets and update token quantities owned by the user at run time (like an inventory system where tokens are items)

## Usage
For greater details please see the [Knowledge Base](https://kb.heathenengineering.com/assets/bgsdk)

The main concept of use in a Unity game client is to get the user's content from a specific wallet. The concept of a White Label Wallet is key to the whole process. A White Label Wallet is a wallet that is owned by the App not the user, this means the app can add to and remove from this wallet without barrer, you can think of this as an "inventory" which your app can assoceate with the given use through whatever means makes the most since for your game. For example you might store the White Label Wallet ID on the user's account entity such as seen in [PlayFab](https://playfab.com/).

How you assoceate a White Label Wallet is up to you. As to the creation of a new White Label Wallet for a user that is most appropreate to be done on your [Trusted Web Server](https://kb.heathenengineering.com/assets/bgsdk/game-architecture#trusted-server-or-trusted-service). To learn more about the typical architecture of a game and how backend services such as a Trusted Web Server fit into that design please read our Knowedge Base entry on [Game Architecture](https://kb.heathenengineering.com/assets/bgsdk/game-architecture).

Once you have a White Label Wallet related to your user you can query it simply via the following code.

Fetch the contents of a specific white label wallet
```csharp
StartCoroutine(API.Server.Wallets.List(HandleListWalletResults));
```
where `HandleListWalletResults` would recieve a NFTBalanceResult which can be used to iterate over the content found in that wallet.
```csharp
private void HandleNFTBalanceResult(NFTBalanceResult balanceResult)
{
    if(!balanceResult.hasError)
    {
        foreach(var token in balanceResult.result)
        {
            // Token owned
        }
    }
    else
    {
        Debug.Log("List NFT Responce:\nHas Error: " + balanceResult.hasError + "\nMessage: " + balanceResult.message + "\nCode:" + balanceResult.httpCode);
    }
}
```

