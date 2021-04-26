using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ProfileWindow : WindowManager
{
    [SerializeField] GameObject profileDisplaySpace;
    [SerializeField] GameObject profilePrefab;
    protected override WindowType WindowType => WindowType.Profile;

    protected override void ShowMe()
    {
        base.ShowMe();
        LoadProfileData();
        MessageBroker.Default.Receive<ProfileWindowRefreshList>().Subscribe(_ => LoadProfileData()).AddTo(this);
    }

    void LoadProfileData()
    {
        foreach (Transform child in profileDisplaySpace.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var name in ApplicationExtensions.ProfileNamesWithLevelCanBeBeat)
        {
            var spawnedProfile = Instantiate(profilePrefab, profileDisplaySpace.transform);
            spawnedProfile.GetComponent<ProfileEntry>().Initialize(name);
        }
    }
}

public class ProfileWindowRefreshList { }
