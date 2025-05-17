// using UnityEngine;
// using UnityEngine.UI;

// public class PlacesQuestManager : MonoBehaviour
// {
//     public GooglePlacesService placesService;
//     public QuestMapManager mapManager;
//     public GameObject questUIPrefab;
//     public Transform questsContainer;
    
//     private List<PlaceResult> nearbyPlaces = new List<PlaceResult>();

//     public void InitializeQuests()
//     {
//         // Call this after getting GPS location
//         placesService.FindNearbyQuests(mapManager.currentGPSLocation);
//     }

//     public void CreateQuestUI(PlaceResult place)
//     {
//         GameObject questUI = Instantiate(questUIPrefab, questsContainer);
//         QuestUIElement uiElement = questUI.GetComponent<QuestUIElement>();
        
//         uiElement.SetupQuest(
//             place.name,
//             place.vicinity,
//             place.rating,
//             () => { mapManager.FocusOnLocation(place.geometry.location); }
//         );
//     }

//     public void OnPlacesFound(List<PlaceResult> places)
//     {
//         nearbyPlaces = places;
        
//         foreach (var place in nearbyPlaces)
//         {
//             CreateQuestUI(place);
//             mapManager.AddQuestMarker(place);
//         }
//     }
// }