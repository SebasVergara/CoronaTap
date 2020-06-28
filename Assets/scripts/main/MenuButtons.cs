using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds;
using GoogleMobileAds.Api;

using OneSignalPush.MiniJSON;
using System;

public class MenuButtons : MonoBehaviour
{
    public void Start()
    {
        OneSignal.StartInit("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee") //OneSignal ID
            .HandleNotificationOpened(HandleNotificationOpened)
            .Settings(new Dictionary<string, bool>() {
              { OneSignal.kOSSettingsAutoPrompt, false },
              { OneSignal.kOSSettingsInAppLaunchURL, false } })
            .EndInit();

        OneSignal.PromptForPushNotificationsWithUserResponse(OneSignal_promptForPushNotificationsReponse);
        void OneSignal_promptForPushNotificationsReponse(bool accepted)
        {
            Debug.Log("OneSignal_promptForPushNotificationsReponse: " + accepted);
        }
        MobileAds.Initialize(initStatus => { });
    }
    public void playGame() {
        SceneManager.LoadScene(1); 
    }
    private static void HandleNotificationReceived(OSNotification notification)
    {
        OSNotificationPayload payload = notification.payload;
        string message = payload.body;

        print("GameControllerExample:HandleNotificationReceived: " + message);
        print("displayType: " + notification.displayType);

        Dictionary<string, object> additionalData = payload.additionalData;
        if (additionalData == null)
            Debug.Log("[HandleNotificationReceived] Additional Data == null");
        else
            Debug.Log("[HandleNotificationReceived] message " + message + ", additionalData: " + Json.Serialize(additionalData) as string);
    }
    public static void HandleNotificationOpened(OSNotificationOpenedResult result)
    {
        
    }
    public static void HandlerInAppMessageClicked(OSInAppMessageAction action)
    {
        String logInAppClickEvent = "In-App Message Clicked: " +
            "\nClick Name: " + action.clickName +
            "\nClick Url: " + action.clickUrl +
            "\nFirst Click: " + action.firstClick +
            "\nCloses Message: " + action.closesMessage;

        print(logInAppClickEvent);
    }
    private void OneSignal_permissionObserver(OSPermissionStateChanges stateChanges)
    {
        Debug.Log("PERMISSION stateChanges.from.status: " + stateChanges.from.status);
        Debug.Log("PERMISSION stateChanges.to.status: " + stateChanges.to.status);
    }
    private void OneSignal_subscriptionObserver(OSSubscriptionStateChanges stateChanges)
    {
        Debug.Log("SUBSCRIPTION stateChanges: " + stateChanges);
        Debug.Log("SUBSCRIPTION stateChanges.to.userId: " + stateChanges.to.userId);
        Debug.Log("SUBSCRIPTION stateChanges.to.subscribed: " + stateChanges.to.subscribed);
    }
    private void OneSignal_emailSubscriptionObserver(OSEmailSubscriptionStateChanges stateChanges)
    {
        Debug.Log("EMAIL stateChanges.from.status: " + stateChanges.from.emailUserId + ", " + stateChanges.from.emailAddress);
        Debug.Log("EMAIL stateChanges.to.status: " + stateChanges.to.emailUserId + ", " + stateChanges.to.emailAddress);
    }
}
