using System;
using System.Runtime.CompilerServices;
using Beebyte.Obfuscator;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

[ObfuscateLiterals]
public class InAppManager : MonoBehaviour, IDetailedStoreListener, IStoreListener
{
	public static InAppManager instance;

	private static bool _initialized = false;

	private IAppleExtensions _apple;

	private IGooglePlayStoreExtensions _googleplay;

	internal IStoreController controller;

	private IExtensionProvider extensions;

	internal static CrossPlatformValidator validator;

	public static bool last_availableToPurchase;

	public static string last_transactionID;

	public static bool last_hasReceipt;

	public static bool last_tValidPurchase;

	public static bool last_tPurchasePending;

	internal static bool googleAccount = true;

	private static ConfigurationBuilder _builder;

	public static bool restore_ui_buffering;

	public static string restore_message;

	public static string validator_message;

	private IAppleExtensions apple
	{
		get
		{
			if (_apple == null)
			{
				_apple = extensions.GetExtension<IAppleExtensions>();
			}
			return _apple;
		}
	}

	private IGooglePlayStoreExtensions googleplay
	{
		get
		{
			if (_googleplay == null)
			{
				_googleplay = extensions.GetExtension<IGooglePlayStoreExtensions>();
			}
			return _googleplay;
		}
	}

	private void Start()
	{
		Debug.Log("InAppManager::Start");
		if (instance != null)
		{
			Debug.LogError("Multiple in-app managers have been instantiated.");
			return;
		}
		instance = this;
		if (PlayerConfig.instance == null || PlayerConfig.instance.data.pPossible0507)
		{
			activatePrem();
			Debug.Log("InAppManager::End");
		}
	}

	private static bool checkGoogleAccount()
	{
		if (!googleAccount)
		{
			Debug.Log("google account missing");
			ErrorWindow.errorMessage = "A Google Account is missing or you're not logged in with one.";
			ScrollWindow.get("error_with_reason").clickShow();
		}
		return googleAccount;
	}

	private static void debugPremium()
	{
	}

	private void InitializePurchasing(int pWhere = -1)
	{
		Debug.Log($"InitializePurchasing {pWhere}");
		if (!IsInitialized() && !_initialized)
		{
			_initialized = true;
			instance = this;
			_builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
			_builder.logUnavailableProducts = true;
			_builder.AddProduct("premium", ProductType.NonConsumable, new IDs
			{
				{ "premium", "GooglePlay" },
				{ "premium", "AppleAppStore" }
			});
			try
			{
				validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
				Debug.Log("validator assigned");
				validator_message = null;
			}
			catch (NotImplementedException ex)
			{
				Debug.Log("validator not assigned");
				Debug.LogError("Cross Platform Validator Not Implemented: " + ex.Message);
				validator_message = "Validator not implemented";
			}
			catch (Exception ex2)
			{
				Debug.Log("validator not assigned");
				Debug.LogError("Validator Exception: " + ex2.Message);
				validator_message = ex2.Message;
			}
			UnityPurchasing.Initialize(this, _builder);
		}
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		this.controller = controller;
		this.extensions = extensions;
		apple.RegisterPurchaseDeferredListener(OnAskToBuy);
		checkPremium();
		checkWorldnet();
		Discounts.checkDiscounts();
	}

	public void checkPremium()
	{
		bool tValidPurchase = true;
		bool tPurchasePending = false;
		Product tProduct = controller.products.WithID("premium");
		Debug.Log("[cp]");
		Debug.Log("[avl] " + tProduct.availableToPurchase);
		Debug.Log("[txi] " + tProduct.transactionID);
		Debug.Log("[hrc] " + tProduct.hasReceipt);
		last_tValidPurchase = tValidPurchase;
		last_tPurchasePending = tPurchasePending;
		Config.lockGameControls = false;
		if (!tValidPurchase && tPurchasePending)
		{
			Debug.Log("[nvp] pp");
			return;
		}
		Debug.Log("[vp] 3 " + tValidPurchase);
		Debug.Log("[hr] " + tProduct.hasReceipt);
		if ((tValidPurchase && tProduct.hasReceipt) || PlayerConfig.instance.data.premium)
		{
			Debug.Log("[phr]");
			activatePrem();
		}
		else
		{
			Debug.Log("[vp] 4 " + tValidPurchase);
			Debug.Log("[hr] " + tProduct.hasReceipt);
			Debug.Log("[hp] " + PlayerConfig.instance.data.premium);
		}
		Debug.Log("[cpd]");
	}

	public void checkWorldnet()
	{
	}

	public static void consumePremium()
	{
	}

	private void OnAskToBuy(Product item)
	{
		Debug.Log("Purchase deferred: " + item.definition.id);
		Config.lockGameControls = false;
	}

	public string getDebugInfo()
	{
		Product product = controller.products.WithID("premium");
		return string.Concat(string.Concat("" + "hasReceipt: " + product.hasReceipt, "\nreceipt: ", product.receipt), "\nprem? ", PlayerConfig.instance.data.premium.ToString());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void activatePrem(bool pShowWindowUnlocked = false)
	{
		Debug.Log("[ap]");
		PlayerConfig.instance.data.premium = true;
		PlayerConfig.saveData();
		Config.hasPremium = true;
		PremiumElementsChecker.checkElements();
		PlayerConfig.setFirebaseProp("have_premium", "yes");
		if (pShowWindowUnlocked)
		{
			if (!PlayerConfig.instance.data.tutorialFinished)
			{
				ScrollWindow.queueWindow("premium_unlocked");
			}
			else
			{
				ScrollWindow.hideAllEvent(pWithAnimation: false);
				ScrollWindow.showWindow("premium_unlocked");
			}
		}
		debugPremium();
	}

	private void activateSub(bool pShowWindowUnlocked = false)
	{
		if (pShowWindowUnlocked && PlayerConfig.instance.data.tutorialFinished)
		{
			ScrollWindow.hideAllEvent(pWithAnimation: false);
			ScrollWindow.showWindow("worldnet_sub");
		}
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.Log("Cannot initialize IAP system: " + error);
		Config.lockGameControls = false;
	}

	public void OnInitializeFailed(InitializationFailureReason error, string message)
	{
		Debug.Log("Cannot initialize IAP system: " + error);
		Debug.Log(message);
		Config.lockGameControls = false;
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
	{
		Config.lockGameControls = true;
		Debug.Log("Purchase OK: " + args?.purchasedProduct?.definition?.id);
		Debug.Log("Args: " + args?.purchasedProduct);
		if (args?.purchasedProduct?.definition?.id == "premium")
		{
			Debug.Log("[pp] process premium");
			return ProcessPremium(args);
		}
		if (args?.purchasedProduct?.definition?.id == "worldnet")
		{
			Debug.Log("[pw] process worldnet");
			return ProcessWorldnet(args);
		}
		Debug.Log("[pn] process nothing to be done");
		return PurchaseProcessingResult.Complete;
	}

	public PurchaseProcessingResult ProcessPremium(PurchaseEventArgs args)
	{
		bool validPurchase = true;
		bool purchasePending = false;
		Debug.Log("[prp]");
		if (!validPurchase && purchasePending)
		{
			Debug.Log("[np] 3");
			Config.lockGameControls = false;
			return PurchaseProcessingResult.Pending;
		}
		string tPurchasedProduct = args?.purchasedProduct?.definition?.id;
		Debug.Log("[vp] 2 " + validPurchase);
		Debug.Log("[lc] " + tPurchasedProduct);
		if (validPurchase && string.Equals(tPurchasedProduct, "premium", StringComparison.OrdinalIgnoreCase))
		{
			activatePrem(pShowWindowUnlocked: true);
			Debug.Log($"[ppp] '{tPurchasedProduct}'");
		}
		else
		{
			Debug.Log("[np] 4");
		}
		Config.lockGameControls = false;
		debugPremium();
		return PurchaseProcessingResult.Complete;
	}

	public PurchaseProcessingResult ProcessWorldnet(PurchaseEventArgs args)
	{
		bool validPurchase = true;
		bool purchasePending = false;
		PlayerConfig.instance.data.worldnet = "";
		if (!validPurchase && purchasePending)
		{
			Debug.Log("purchase pending");
			Config.lockGameControls = false;
			return PurchaseProcessingResult.Pending;
		}
		Debug.Log("check if valid");
		Config.lockGameControls = false;
		if (validPurchase && string.Equals(args.purchasedProduct.definition.id, "worldnet", StringComparison.OrdinalIgnoreCase))
		{
			Debug.Log("valid!");
			setWorldnetSubscription(args.purchasedProduct.transactionID);
			activateSub(pShowWindowUnlocked: true);
			Debug.Log($"ProcessPurchase: PASS. Product: '{args.purchasedProduct.definition.id}'");
		}
		Debug.Log("we are here");
		if (Config.lockGameControls)
		{
			Debug.Log("lockgamecontrosl locked");
		}
		else
		{
			Debug.Log("lockgamecontrosl not locked");
		}
		return PurchaseProcessingResult.Complete;
	}

	public void setWorldnetSubscription(string pTransactionID)
	{
		PlayerConfig.instance.data.worldnet = pTransactionID;
		PlayerConfig.saveData();
	}

	public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
	{
		Debug.Log("[D] WORLDBOX PURCHASE FAILED  " + p);
		Config.lockGameControls = false;
		_ = 4;
		ScrollWindow.showWindow("premium_purchase_error");
		InitializePurchasing(50);
	}

	public void OnPurchaseFailed(Product i, PurchaseFailureDescription p)
	{
		Debug.Log("[X] WORLDBOX PURCHASE FAILED  " + p.message);
		OnPurchaseFailed(i, p.reason);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void checkPremiumReceipt(string pReceipt, ref bool validPurchase, ref bool purchasePending)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void checkWorldnetReceipt(string pReceipt, ref bool validPurchase, ref bool purchasePending)
	{
	}

	public bool buyPremium()
	{
		activatePrem(pShowWindowUnlocked: true);
		Config.lockGameControls = false;
		return true;
	}

	public bool buyWorldNet()
	{
		return false;
	}

	private bool BuyProductID(string productId)
	{
		if (IsInitialized())
		{
			Product product = controller.products.WithID(productId);
			if (product == null)
			{
				return false;
			}
			if (product.availableToPurchase)
			{
				Debug.Log($"Purchasing product asychronously: '{product.definition.id}'");
				Config.lockGameControls = true;
				controller.InitiatePurchase(product);
				if (ScrollWindow.windowLoaded("premium_menu") && ScrollWindow.isCurrentWindow("premium_menu"))
				{
					ScrollWindow.get("premium_menu").clickHide();
				}
				return true;
			}
			ScrollWindow.showWindow(productId + "_purchase_error");
			Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
			Config.lockGameControls = false;
			return false;
		}
		ScrollWindow.showWindow(productId + "_purchase_error");
		Debug.Log("BuyProductID FAIL. Not initialized.");
		Config.lockGameControls = false;
		return false;
	}

	public void RestorePurchases()
	{
		restore_message = "Restoring...";
		restore_ui_buffering = true;
		if (Config.isEditor)
		{
			restore_ui_buffering = false;
		}
		if (!IsInitialized())
		{
			InitializePurchasing(66);
			Debug.Log("RestorePurchases FAIL. Not initialized.");
			restore_message = "IAP not initialized, failed to restore purchases.";
		}
	}

	private bool IsInitialized()
	{
		if (controller != null)
		{
			return extensions != null;
		}
		return false;
	}
}
