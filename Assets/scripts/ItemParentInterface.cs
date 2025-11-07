using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public interface IItemParent
{
    bool IsEquipped { get; set; }
    void Equip(GameObject player);
    void Drop();
    void ShowItem();
    void HideItem();
    void SetEquippable();
    GameObject GetItem();
}