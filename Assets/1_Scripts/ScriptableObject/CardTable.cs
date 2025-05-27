using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(AssetPath = "Resources/Data/", ExcelName = "CardTable")]
public class CardTable : ScriptableObject
{
	//public List<EntityType> Definition; // Replace 'EntityType' to an actual type that is serializable.
	public List<CardData> dataTable; // Replace 'EntityType' to an actual type that is serializable.
}
