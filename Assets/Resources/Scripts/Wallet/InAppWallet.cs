using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

namespace Thirdweb.Unity
{

    public class InAppWallet : MonoBehaviour
    {
        [Header("Connection UI")]
        [SerializeField] private Button connectWalletButton;
        [SerializeField] private GameObject emailInputCanvas;
        [SerializeField] private TMP_InputField emailInputField;
        [SerializeField] private Button emailSubmitButton;
        [SerializeField] TMP_Text statusText;
        [SerializeField] private ulong chainId = 1115;

        [Header("Wallet Info Display")]
        [SerializeField] private GameObject walletInfoPanel;
        [SerializeField] private TMP_Text walletAddressText;
        [SerializeField] private TMP_Text walletBalanceText;
        [SerializeField] private Button refreshBalanceButton;

        private void Start()
        {
            //  Initialize UI Staetes
            if (walletInfoPanel != null)
                walletInfoPanel.SetActive(false);
            //Hide email Input Canvas
            if (emailInputCanvas != null)
                emailInputCanvas.SetActive(false);
            //Set up connect wallet button
            if (connectWalletButton != null)
            {
                connectWalletButton.onClick.RemoveAllListeners();
                connectWalletButton.onClick.AddListener(ShowEmailInput);
            }
            //set up email authentication
            if (emailSubmitButton != null)
            {
                emailSubmitButton.onClick.RemoveAllListeners();
                emailSubmitButton.onClick.AddListener(ConnectWithEmail);
            }
            CheckExistingConnection();
        }
        private void ShowEmailInput()
        {
            //show email input canves
            if (emailInputCanvas != null)
                emailInputCanvas.SetActive(true);

            //hide connect wallet button
            if (connectWalletButton != null)
                connectWalletButton.gameObject.SetActive(false);

            if (statusText != null)
                statusText.text = "Enter your email to connect";
        }

        private async void ConnectWithEmail()
        {
            if (emailInputField == null || string.IsNullOrEmpty(emailInputField.text))
            {
                if (statusText != null)
                    statusText.text = "Please enter a valid email address";
                return;

                var InAppWalletOptions = new InAppWalletOptions(email: emailInputField.text);
                var options = new WalletOptions(
                    provider: WalletProvider.InAppWallet,
                    chainId: chainId,
                    inAppWalletOptions: InAppWalletOptions
                );
                var wallet = await ThirdwebManager.Instance.ConnectWallet(options);
                //handle successful connection
                if (wallet != null)
                {
                    HandleSuccessfulConnection(wallet);
                }
            }
            try
            {
                if (statusText != null)
                    statusText.text = "Connecting...";
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error connecting with email: {e.Message}");
                if (statusText != null)
                    statusText.text = $"Connection error + {e.Message}";
            }
        }
        private async void HandleSuccessfulConnection(IThirdwebWallet wallet)
        {
            try
            {
                //Hide connect wallet button
                if (connectWalletButton != null)
                    connectWalletButton.gameObject.SetActive(false);

                //Hide email input canvas
                if (emailInputCanvas != null)
                    emailInputCanvas.SetActive(false);

                //Show wallet info panel
                if (walletInfoPanel != null)
                    walletInfoPanel.SetActive(true);

                //Get and Display wallet address
                var address = await wallet.GetAddress();
                if (walletAddressText != null)
                {
                    string formattedAddress = FormatAddress(address);
                    walletAddressText.text = $"Wallet Address: {formattedAddress}";
                }
                //Get and Display wallet balance
                await UpdateWalletBalance(wallet);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error handling successful connection: {e.Message}");
                if (statusText != null)
                    statusText.text = "Error displaying wallet info";
            }
        }
        private async System.Threading.Tasks.Task UpdateWalletBalance(IThirdwebWallet wallet)
        {
            if (walletBalanceText != null)
            {
                try
                {
                    walletBalanceText.text = "Loading balance...";

                    var balance = await wallet.GetBalance(chainId: chainId);
                    var chainDetails = await Utils.GetChainMetadata(
                        client: ThirdwebManager.Instance.Client,
                        chainId: chainId
                    );
                    var symbol = chainDetails?.NativeCurrency?.Symbol ?? "ETH";
                    var balanceEth = Utils.ToEth(
                        wei: balance.ToString(),
                        decimalsToDisplay: 4,
                        addCommas: true
                    );
                    walletBalanceText.text = $"Balance: {balanceEth} {symbol}";
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error updating wallet balance: {e.Message}");
                    walletBalanceText.text = "Error loading balance";
                }
            }
        }

        private async void RefreshWalletBalance()
        {
            var wallet = ThirdwebManager.Instance.GetActiveWallet();
            if (wallet != null)
            {
                await UpdateWalletBalance(wallet);
            }
        }
        private string FormatAddress(string address)
        {
            //Format address to show first 6 and last 4 characters
            return address.Length > 10
            ? $"{address.Substring(0, 6)}...{address.Substring(address.Length - 4)}"
            : address;
        }
        private async void  CheckExistingConnection();
        }
}
