using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections;
using System;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
using TMPro;

namespace behealthy.main
{
    public class PurchasingService : MonoBehaviour, IStoreListener
    {
        private static IStoreController m_StoreController;
        private static IExtensionProvider m_StoreExtensionProvider;
        private static IAppleExtensions m_AppleExtensions;
        private static IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;


        //Play Market
        private static string kProductNameGooglePlayConsumable1 = "S15000";
        private static string kProductNameGooglePlayConsumable2 = "S80000";
        private static string kProductNameGooglePlayConsumable3 = "S500000";

        //App Store
        private static string kProductNameAppleConsumable1 = "com.mobileappgaming.behealthy.S15000";
        private static string kProductNameAppleConsumable2 = "com.mobileappgaming.behealthy.S80000";
        private static string kProductNameAppleConsumable3 = "com.mobileappgaming.behealthy.S500000";

        //Mac App Store
        private static string kProductNameMacAppleConsumable1 = "com.unity3d.test.services.purchasing.nonconsumable";
        private static string kProductNameMacAppleConsumable2 = "com.unity3d.test.services.purchasing.nonconsumable";
        private static string kProductNameMacAppleConsumable3 = "com.unity3d.test.services.purchasing.nonconsumable";


        //Windows Store
        private static string kProductNameWindowsConsumable1 = "com.unity3d.test.services.purchasing.nonconsumable";
        private static string kProductNameWindowsConsumable2 = "com.unity3d.test.services.purchasing.nonconsumable";
        private static string kProductNameWindowsConsumable3 = "com.unity3d.test.services.purchasing.nonconsumable";

        //Types of in-apps
        private static string kProductIDConsumable1 = "s15000consumable";           // General handle for the consumable product.
        private static string kProductIDConsumable2 = "s80000consumable";
        private static string kProductIDConsumable3 = "s500000consumable";

        private static string kProductIDNonConsumable = "nonconsumable";     // General handle for the non-consumable product.
        private static string kProductIDSubscription = "subscription";
        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /*public void buyProduct(PurchasingType type)
        {
            if (m_StoreController != null)
            {
                switch (type)
                {
                    case PurchasingType.SS3: BuyProductID(kProductIDConsumable1); break;
                    case PurchasingType.SS10: BuyProductID(kProductIDConsumable2); break;
                    case PurchasingType.SS30: BuyProductID(kProductIDConsumable3); break;
                    case PurchasingType.SS100: BuyProductID(kProductIDConsumable4); break;
                }
            }
        }*/
        public void buyProduct1()
        {
            BuyProductID(kProductIDConsumable1);
        }
        public void buyProduct2()
        {
            BuyProductID(kProductIDConsumable2);
        }
        public void buyProduct3()
        {
            BuyProductID(kProductIDConsumable3);
        }

        void BuyProductID(string productId)
        {
            // If the stores throw an unexpected exception, use try..catch to protect my logic here.
            try
            {
                // If Purchasing has been initialized ...
                if (IsInitialized())
                {
                    // ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
                    Product product = m_StoreController.products.WithID(productId);

                    // If the look up found a product for this device's store and that product is ready to be sold ... 
                    if (product != null && product.availableToPurchase)
                    {
                        Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                        m_StoreController.InitiatePurchase(product);
                    }
                    // Otherwise ...
                    else
                    {
                        // ... report the product look-up failure situation  
                        Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    }
                }
                // Otherwise ...
                else
                {
                    // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
                    Debug.Log("BuyProductID FAIL. Not initialized.");
                }
            }
            // Complete the unexpected exception handling ...
            catch (Exception e)
            {
                // ... by reporting any unexpected exception for later diagnosis.
                Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
            }
        }

        public string getLocalizedPrice(PurchasingType type)
        {
            if (m_StoreController != null)
            {
                switch (type)
                {
                    case PurchasingType.S15000: return m_StoreController.products.all[0].metadata.localizedPriceString;  break;
                    case PurchasingType.S80000: return m_StoreController.products.all[1].metadata.localizedPriceString; break;
                    case PurchasingType.S500000: return m_StoreController.products.all[2].metadata.localizedPriceString; break;
                    default: return null;
                }
            }
            else return null;

        }

        private void FixedUpdate()
        {         
            getLocalizedPrice(PurchasingType.S15000);
            getLocalizedPrice(PurchasingType.S80000);
            getLocalizedPrice(PurchasingType.S500000);

        }


        private void Start()
        {
            initializePurchasing();
        }


        public void initializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            //builder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAvB+KILMXrBUPb/bp9oGLJcLSHzXwL84OMZspQG/Sb2fFiRPIsVcTHno7JJY+QClHtZU4Sw5BlN93Bof0VePxDWHwDst/PQPNCp2ovmYg+3gLYPpSSUuJmZhved4YHUmhdrmPrYZj6QWp7UXo4IhCqwjuHVIFf431FPzQBXGoGE2EAx7rUV6GRnFgzoEhyB0S3HxJFpLSCDkJFy2Zh9SKuwvQg720Ds+wGUEEN0UqJApNfhZs/3No6cVrWwgIJ1Pna9lZKNYa4IsL7V1OrBtRemhSlFLox3y2oJve6YWDRFg8I8u9cg70xFlMdREwg8Y4uuhXGwOQ5AxwgcpTSmqmlwIDAQAB");

            // Add a product to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.
            //builder.AddProduct(kProductIDConsumable, ProductType.Consumable, new IDs() { { kProductNameAppleConsumable, AppleAppStore.Name }, { kProductNameGooglePlayConsumable, GooglePlay.Name }, });// Continue adding the non-consumable product.

            builder.AddProduct(kProductIDConsumable1, ProductType.Consumable, new IDs() { { kProductNameAppleConsumable1, AppleAppStore.Name }, { kProductNameGooglePlayConsumable1, GooglePlay.Name }, { kProductNameMacAppleConsumable1, MacAppStore.Name }, { kProductNameWindowsConsumable1, WinRT.Name }, }); // And finish adding the subscription product.
            builder.AddProduct(kProductIDConsumable2, ProductType.Consumable, new IDs() { { kProductNameAppleConsumable2, AppleAppStore.Name }, { kProductNameGooglePlayConsumable2, GooglePlay.Name }, { kProductNameMacAppleConsumable2, MacAppStore.Name }, { kProductNameWindowsConsumable2, WinRT.Name }, }); // And finish adding the subscription product.
            builder.AddProduct(kProductIDConsumable3, ProductType.Consumable, new IDs() { { kProductNameAppleConsumable3, AppleAppStore.Name }, { kProductNameGooglePlayConsumable3, GooglePlay.Name }, { kProductNameMacAppleConsumable3, MacAppStore.Name }, { kProductNameWindowsConsumable3, WinRT.Name }, }); // And finish adding the subscription product.

            //builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = false;
            //builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs() { { kProductNameAppleSubscription, AppleAppStore.Name }, { kProductNameGooglePlaySubscription, GooglePlay.Name }, });// Kick off the remainder of the set-up with an asynchrounous call, passing the configuration and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(this, builder);
        }

        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        /*public bool isSubscription()
        {
            bool isSub = false;
            if (m_StoreController != null)
            {
                Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();
                PlayerPrefs.SetInt("isSub", 0);
                foreach (var item in m_StoreController.products.all)
                {
                    // this is the usage of SubscriptionManager class
                    if (item.receipt != null)
                    {
                        if (item.definition.type == ProductType.Subscription)
                        {
                            if (checkIfProductIsAvailableForSubscriptionManager(item.receipt))
                            {
                                string intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
                                SubscriptionManager p = new SubscriptionManager(item, intro_json);
                                SubscriptionInfo info = p.getSubscriptionInfo();
                                //Debug.Log("product id is: " + info.getProductId());
                                //Debug.Log("purchase date is: " + info.getPurchaseDate());
                                //Debug.Log("subscription next billing date is: " + info.getExpireDate());
                                //Debug.Log("is subscribed? " + info.isSubscribed().ToString());
                                isSub = true;
                                PlayerPrefs.SetInt("isSub", 1);
                                PlayerPrefs.SetString("sabDate", info.getRemainingTime().ToString("yyyy-MM-dd"));
                            }
                            else
                            {
                                Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
                            }
                        }
                        else
                        {
                            Debug.Log("the product is not a subscription product");
                        }
                    }
                    else
                    {
                        Debug.Log("the product should have a valid receipt");
                    }
                }
            }
            return isSub;
        }*/


        private bool checkIfProductIsAvailableForSubscriptionManager(string receipt)
        {
            var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
            if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
            {
                Debug.Log("The product receipt does not contain enough information");
                return false;
            }
            var store = (string)receipt_wrapper["Store"];
            var payload = (string)receipt_wrapper["Payload"];

            if (payload != null)
            {
                switch (store)
                {
                    case GooglePlay.Name:
                        {
                            var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                            if (!payload_wrapper.ContainsKey("json"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
                                return false;
                            }
                            var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                            if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
                                return false;
                            }
                            var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                            var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                            if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                                return false;
                            }
                            return true;
                        }
                    case AppleAppStore.Name:
                    case AmazonApps.Name:
                    case MacAppStore.Name:
                        {
                            return true;
                        }
                    default:
                        {
                            return false;
                        }
                }
            }
            return false;
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;
            m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
            // On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
            // On non-Apple platforms this will have no effect; OnDeferred will never be called.
            m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);
            m_GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
        }

        /// <summary>
        /// iOS Specific.
        /// This is called as part of Apple's 'Ask to buy' functionality,
        /// when a purchase is requested by a minor and referred to a parent
        /// for approval.
        ///
        /// When the purchase is approved or rejected, the normal purchase events
        /// will fire.
        /// </summary>
        /// <param name="item">Item.</param>
        private void OnDeferred(Product item)
        {
            Debug.Log("Purchase deferred: " + item.definition.id);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing this reason with the user.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            // A consumable product has been purchased by this user.
            if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable1, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));//If the consumable item has been successfully purchased, add 100 coins to the player's in-game score.
                                                                                                                       //3x Help
                BudleIt(15000);
            }

            if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable2, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));//If the consumable item has been successfully purchased, add 100 coins to the player's in-game score.
                                                                                                                       //6x Help
                BudleIt(80000);
            }

            if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable3, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));//If the consumable item has been successfully purchased, add 100 coins to the player's in-game score.
                                                                                                                       //30x Help
                BudleIt(500000);
            }
            // Return a flag indicating wither this product has completely been received, or if the application needs to be reminded of this purchase at next app launch. Is useful when saving purchased products to the cloud, and when that save is delayed.
            return PurchaseProcessingResult.Complete;
        }

        public void restorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) => {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }



        public void BudleIt(int SatoshiAmmount)
        {

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "UpdateSatoshi", // Arbitrary function name (must exist in your uploaded cloud.js file)
                FunctionParameter = new { SatoshiValue = SatoshiAmmount }, // The parameter provided to your function
                GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
            }, nothing1 => { }, nothing2 => { });

            /*var purchaseRequest = new PurchaseItemRequest();
            purchaseRequest.CatalogVersion = "0";
            purchaseRequest.ItemId = id;
            purchaseRequest.VirtualCurrency = "ST";
            purchaseRequest.Price = 0;
            PlayFabClientAPI.PurchaseItem(purchaseRequest, lol => { Debug.Log("Satoshi added"); }, elol);*/
        }

        void elol(PlayFabError error)
        {
            Debug.Log("No More Satoshi");
            Debug.LogError(error.GenerateErrorReport());
        }



    }
    public enum PurchasingType
    {
        S15000,
        S80000,
        S500000
    }

    
    
}