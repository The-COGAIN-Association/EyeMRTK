using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class DictionaryExtensions
{  
	
//	public static void Add(
//		this Dictionary<string, Dictionary<string, string>> dictionary,
//		string key1,
//		string key2,
//		string value
//	)
//	{
//		Dictionary<string, string> nested;
//		if (dictionary.TryGetValue(key1, out nested))
//			nested.Add(key2, value);
//		else
//		{
//			nested = new Dictionary<string, string>();
//			nested.Add(key2, value);
//			dictionary.Add(key1, nested);
//		}
//	}

	public static void AddToNestedDictionary<TKey, TNestedDictionary, TNestedKey, TNestedValue>(
		this IDictionary<TKey, TNestedDictionary> dictionary,
		TKey key,
		TNestedKey nestedKey,
		TNestedValue nestedValue
		) where TNestedDictionary : IDictionary<TNestedKey, TNestedValue> {
		dictionary.AddToNestedDictionary(
			key,
			nestedKey,
			nestedValue,
			() => (TNestedDictionary)(IDictionary<TNestedKey, TNestedValue>)
			new Dictionary<TNestedKey, TNestedValue>());
	}

	public static void AddToNestedDictionary<TKey, TNestedDictionary, TNestedKey, TNestedValue>(
		this IDictionary<TKey, TNestedDictionary> dictionary,
		TKey key,
		TNestedKey nestedKey,
		TNestedValue nestedValue,
		Func<TNestedDictionary> provider
		) where TNestedDictionary : IDictionary<TNestedKey, TNestedValue> {
		TNestedDictionary nested;
		if (!dictionary.TryGetValue(key, out nested)) {
			nested = provider();
			dictionary.Add(key, nested);
		}
		nested.Add(nestedKey, nestedValue);
	}
}

public class AdditionalInfoToSave : MonoBehaviour {

	public IDictionary<string, Dictionary<string, string>> SaveInfo =
		new Dictionary<string, Dictionary<string, string>> {};



}
