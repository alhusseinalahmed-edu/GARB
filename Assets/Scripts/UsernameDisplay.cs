using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] PhotonView PV;
    [SerializeField] TMP_Text text;

    private void Start()
    {
        if (PV.IsMine) gameObject.SetActive(false);
        text.text = PV.Owner.NickName;
    }

}
