// using UnityEngine;
// using Thirdweb;
// using System.Threading.Tasks;
// using TMPro;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.UI;
// using Thirdweb.Unity;

// public class WalletConnectionHandler : MonoBehaviour
// {
//     public GameObject objectToActivate; // Assign in Inspector

//     void Start()
//     {
//         // Initial state
//         objectToActivate.SetActive(false);
        
//         // Subscribe to connection events
//         ThirdwebManager.Instance.SDK.Wallet.OnWalletConnected += OnWalletConnected;
//         ThirdwebManager.Instance.SDK.Wallet.OnWalletDisconnected += OnWalletDisconnected;
//     }

//     private void OnWalletConnected(string address)
//     {
//         // Activate object when wallet connects
//         objectToActivate.SetActive(true);
//     }

//     private void OnWalletDisconnected()
//     {
//         // Deactivate object when wallet disconnects
//         objectToActivate.SetActive(false);
//     }

//     void OnDestroy()
//     {
//         // Clean up event subscriptions
//         if (ThirdwebManager.Instance != null)
//         {
//             ThirdwebManager.Instance.SDK.Wallet.OnWalletConnected -= OnWalletConnected;
//             ThirdwebManager.Instance.SDK.Wallet.OnWalletDisconnected -= OnWalletDisconnected;
//         }
//     }
// }