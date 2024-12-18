
using System.Collections.Generic;


public class BlackBoard 
{
    Dictionary<string, BlackBoardKey> storedKeys = new();
    Dictionary<BlackBoardKey,object> storedValues = new();

    private BlackBoardKey RegisterOrGetKey(string name)
    {
        if(!storedKeys.TryGetValue(name,out BlackBoardKey key))
        {
            key = new BlackBoardKey(name);

            storedKeys[name] = key;
        }

        return key;
         
    }
    
    public void UnRegisterEntry(string name)
    {
        if (!storedKeys.ContainsKey(name))
        {
            return;
        }

        BlackBoardKey key = storedKeys[name];

        storedValues.Remove(key);

    }

    public void SetValue<T>(string key,T value,bool isNew = false)
    {
        BlackBoardKey bKey = RegisterOrGetKey(key);
        if (!isNew)
        {
            if (storedValues.TryGetValue(bKey, out object equalVal) && equalVal is BlackBoardEntry<T> passedVal)
            {
                storedValues[bKey] = passedVal.Value;
            }
            else
            {
                BlackBoardEntry<T> newEntry = new BlackBoardEntry<T>(bKey, value);
                storedValues[bKey] = newEntry;
            }
        }
        else
        {
            BlackBoardEntry<T> newEntry = new BlackBoardEntry<T>(bKey, value);
            storedValues[bKey] = newEntry;
            storedValues[bKey] = newEntry.Value;
        }
        
    }

    public bool GetValue<T>(string key, out T value)
    {
        BlackBoardKey bKey = RegisterOrGetKey(key);

        if (storedValues.TryGetValue(bKey,out object equalVal) && equalVal is BlackBoardEntry<T> passedVal)
        {
            value = passedVal.Value;
            return true;
        }

        value = default;
        return false;

    }

}


